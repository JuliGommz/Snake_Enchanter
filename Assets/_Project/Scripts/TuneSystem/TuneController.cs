/*
====================================================================
* TuneController - Genshin-Style Hold & Release Slider System
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-03
* Version: 3.1 - Spell casting rules: range, cooldown, charges, Shield wiring

* ⚠️ WICHTIG: KOMMENTIERUNG NICHT LÖSCHEN! ⚠️
* Diese detaillierte Authorship-Dokumentation ist für die akademische
* Bewertung erforderlich und darf nicht entfernt werden!

* AUTHORSHIP CLASSIFICATION:

* [AI-ASSISTED]
* - Slider-based timing implementation (ADR-008)
* - Position-based evaluation system
* - TuneConfig ScriptableObject integration
* - New Input System migration (v2.1)
* - Human reviewed and will modify as needed

* DEPENDENCIES:
* - GameEvents.cs (SnakeEnchanter.Core)
* - TuneConfig.cs (ScriptableObject)
* - HealthSystem.cs (for damage on failure)
* - ShieldComponent.cs (for Shield activation on Tune 3 success)
* - Unity New Input System (InputSystem package)
* - SnakeEnchanter.inputactions asset

* DESIGN RATIONALE (ADR-008):
* - Genshin Impact Cooking-style mechanic
* - Hold key = Slider moves from 0 to 1
* - Release key = Position evaluated against Triggerzone
* - Too early (before zone) = Safe fail
* - In zone = Success + Heal
* - Too late (after zone) = Snake attacks!

* VERSION HISTORY:
* - v1.0: Time-based window system (deprecated)
* - v2.0: ADR-008 compliant Slider system
* - v2.1: New Input System only (project rule)
* - v2.2: TuneSuccessWithId event for snake targeting
* - v2.3: Fix lambda-leak (B-001), proper unsubscribe in DisableInput
* - v2.4: Spell animation integration (triggers SpellMove/Daze/Attack/Fear)
* - v3.0: Refactor to 3-tune array + unlock gate; tunes locked by default,
*          unlocked via scroll collection via OnTuneUnlocked event;
*          Tune4/Freeze removed; Tune3 = Shield (SpellShield trigger)
* - v3.1: Spell casting rules: range check (Move/Daze), cooldown (all spells),
*          Advanced mode charges, Shield activation + no-recast-while-active,
*          TuneSuccessWithId only fires for snake-targeting tunes (1 and 2)
====================================================================
*/

using UnityEngine;
using UnityEngine.InputSystem;
using SnakeEnchanter.Core;
using SnakeEnchanter.Player;

namespace SnakeEnchanter.Tunes
{
    /// <summary>
    /// Timing result enum for tune evaluation.
    /// </summary>
    public enum TuneResult
    {
        TooEarly,   // Released before triggerzone - safe fail
        Success,    // Released within triggerzone - snake charmed
        TooLate     // Released after triggerzone OR held too long - snake attacks
    }

    /// <summary>
    /// Manages Genshin-style Hold & Release timing mechanic.
    /// Player holds keys 1-3, slider moves, release in triggerzone = success.
    /// Tunes are locked by default — unlocked via scroll collection (OnTuneUnlocked event).
    /// Spell casting rules enforced: range gating (Move/Daze), cooldown (all), charges (Advanced),
    /// Shield activation + no-recast-while-active, TuneSuccessWithId only for snake-targeting tunes.
    /// Uses New Input System exclusively.
    /// </summary>
    public class TuneController : MonoBehaviour
    {
        #region Tune Configuration
        [Header("Tune Configurations (ScriptableObjects)")]
        [Tooltip("Index 0=Move, 1=Daze, 2=Shield")]
        [SerializeField] private TuneConfig[] _tuneConfigs = new TuneConfig[3];

        [Header("Fallback Values (if no ScriptableObject)")]
        [Tooltip("Total slider duration in seconds")]
        [SerializeField] private float _defaultDuration = 3.0f;
        [Tooltip("Triggerzone start position (0-1)")]
        [SerializeField] private float _defaultZoneStart = 0.4f;
        [Tooltip("Triggerzone end position (0-1)")]
        [SerializeField] private float _defaultZoneEnd = 0.65f;

        [Header("Mode Settings")]
        [Tooltip("Simple Mode adds this to zone size")]
        [SerializeField] private float _simpleModeBonus = 0.1f;
        [SerializeField] private bool _isSimpleMode = true;

        [Header("Spell Casting Rules")]
        [Tooltip("Range for Move/Daze casting — snake must be within this distance")]
        [SerializeField] private float _spellCastRange = 8f;

        [Tooltip("Cooldown duration per tune (seconds): [0]=Move, [1]=Daze, [2]=Shield")]
        [SerializeField] private float[] _cooldownDurations = { 3f, 5f, 8f };

        [Tooltip("Max charges per spell in Advanced mode (placeholder — Phase 13 balancing): [0]=Move, [1]=Daze, [2]=Shield")]
        [SerializeField] private int[] _spellCharges = { 5, 5, 3 };

        [Tooltip("Layer mask for snake range check (leave default for all layers)")]
        [SerializeField] private LayerMask _snakeLayerMask;

        [Header("Input")]
        [SerializeField] private InputActionAsset _inputActions;
        #endregion

        #region Private Fields
        // Unlock state — all locked by default, set to true via OnTuneUnlocked event
        // NOT serialized: runtime state managed by scroll pickup system
        private bool[] _tuneUnlocked = new bool[3];

        // Current tune state
        private bool _isHolding = false;
        private int _currentTuneNumber = 0;
        private float _sliderPosition = 0f;

        // Active tune parameters
        private float _activeDuration;
        private float _activeZoneStart;
        private float _activeZoneEnd;

        // Reference to health system for damage
        private HealthSystem _healthSystem;

        // Reference to animator for spell animations
        private Animator _animator;

        // Reference to shield component for Tune 3 activation
        private ShieldComponent _shieldComponent;

        // Cooldown timers — remaining time per tune (seconds), 0 = ready
        private float[] _cooldownTimers = new float[3];

        // Remaining charges per tune (Advanced mode only)
        private int[] _remainingCharges = new int[3];

        // Last known range state for debounce
        private bool _lastSnakeInRange = false;

        // Input System actions — array[0]=Tune1, [1]=Tune2, [2]=Tune3
        private InputAction[] _tuneActions = new InputAction[3];

        // Cached delegates (fix B-001: lambdas can't be unsubscribed)
        private System.Action<InputAction.CallbackContext>[] _onTuneStarted =
            new System.Action<InputAction.CallbackContext>[3];
        private System.Action<InputAction.CallbackContext>[] _onTuneCanceled =
            new System.Action<InputAction.CallbackContext>[3];
        #endregion

        #region Properties
        /// <summary>
        /// Is player currently holding a tune key?
        /// </summary>
        public bool IsHolding => _isHolding;

        /// <summary>
        /// Current tune number being cast (1-3), 0 if none.
        /// </summary>
        public int CurrentTuneNumber => _currentTuneNumber;

        /// <summary>
        /// Current slider position (0-1).
        /// </summary>
        public float SliderPosition => _sliderPosition;

        /// <summary>
        /// Active triggerzone start (0-1).
        /// </summary>
        public float ZoneStart => _activeZoneStart;

        /// <summary>
        /// Active triggerzone end (0-1).
        /// </summary>
        public float ZoneEnd => _activeZoneEnd;

        /// <summary>
        /// Is the given tune number unlocked? tuneNumber is 1-based (1, 2, or 3).
        /// </summary>
        public bool IsTuneUnlocked(int tuneNumber) =>
            tuneNumber >= 1 && tuneNumber <= 3 && _tuneUnlocked[tuneNumber - 1];

        /// <summary>
        /// Current timing state for UI feedback.
        /// </summary>
        public string CurrentTimingState
        {
            get
            {
                if (!_isHolding) return "None";
                if (_sliderPosition < _activeZoneStart) return "TooEarly";
                if (_sliderPosition <= _activeZoneEnd) return "InZone";
                return "TooLate";
            }
        }
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _healthSystem = GetComponent<HealthSystem>();
            if (_healthSystem == null)
            {
                _healthSystem = GetComponentInParent<HealthSystem>();
            }

            // Get Animator component (looks in children for Pirate model)
            _animator = GetComponentInChildren<Animator>();
            if (_animator == null)
            {
                Debug.LogWarning("TuneController: No Animator found! Spell animations will not play.");
            }

            // Cache ShieldComponent — optional, game works without it
            _shieldComponent = GetComponent<ShieldComponent>();
            if (_shieldComponent == null)
            {
                _shieldComponent = GetComponentInParent<ShieldComponent>();
            }

            // Initialize Advanced mode charges (Simple mode has unlimited)
            if (!_isSimpleMode)
            {
                _remainingCharges = (int[])_spellCharges.Clone();
            }

            // Cache delegates to enable proper unsubscription (B-001 fix)
            // IMPORTANT: capture tuneNum in local variable — do NOT use loop variable i in lambda (closure gotcha)
            for (int i = 0; i < 3; i++)
            {
                int tuneNum = i + 1;
                _onTuneStarted[i] = ctx => OnTuneKeyPressed(tuneNum);
                _onTuneCanceled[i] = ctx => OnTuneKeyReleased(tuneNum);
            }

            SetupInputActions();
        }

        private void OnEnable()
        {
            EnableInput();
            GameEvents.OnTuneUnlocked += OnTuneUnlockedEvent;
        }

        private void OnDisable()
        {
            DisableInput();
            GameEvents.OnTuneUnlocked -= OnTuneUnlockedEvent;
        }

        private void Update()
        {
            if (_healthSystem != null && _healthSystem.IsDead) return;

            if (_isHolding)
            {
                UpdateSlider();
            }

            // Tick cooldown timers
            for (int i = 0; i < 3; i++)
            {
                if (_cooldownTimers[i] > 0f)
                {
                    _cooldownTimers[i] -= Time.deltaTime;
                    if (_cooldownTimers[i] <= 0f)
                    {
                        _cooldownTimers[i] = 0f;
                        GameEvents.TuneCooldownExpired(i + 1);
                    }
                }
            }

            // Range indicator update — only check when not actively casting
            if (!_isHolding)
            {
                bool snakeInRange = HasSnakeInRange(_spellCastRange);
                if (snakeInRange != _lastSnakeInRange)
                {
                    _lastSnakeInRange = snakeInRange;
                    GameEvents.SnakeInRangeChanged(snakeInRange);
                }
            }
        }
        #endregion

        #region Input System Setup
        /// <summary>
        /// Sets up Input System actions from the InputActionAsset.
        /// </summary>
        private void SetupInputActions()
        {
            if (_inputActions == null)
            {
                _inputActions = Resources.Load<InputActionAsset>("SnakeEnchanter");
            }

            if (_inputActions != null)
            {
                var playerMap = _inputActions.FindActionMap("Player");
                if (playerMap != null)
                {
                    _tuneActions[0] = playerMap.FindAction("Tune1");
                    _tuneActions[1] = playerMap.FindAction("Tune2");
                    _tuneActions[2] = playerMap.FindAction("Tune3");
                }
                else
                {
                    Debug.LogError("TuneController: 'Player' action map not found!");
                }
            }
            else
            {
                Debug.LogError("TuneController: InputActionAsset not assigned!");
            }
        }

        private void EnableInput()
        {
            for (int i = 0; i < 3; i++)
            {
                if (_tuneActions[i] != null)
                {
                    _tuneActions[i].Enable();
                    _tuneActions[i].started += _onTuneStarted[i];
                    _tuneActions[i].canceled += _onTuneCanceled[i];
                }
            }
        }

        private void DisableInput()
        {
            for (int i = 0; i < 3; i++)
            {
                if (_tuneActions[i] != null)
                {
                    _tuneActions[i].started -= _onTuneStarted[i];
                    _tuneActions[i].canceled -= _onTuneCanceled[i];
                    _tuneActions[i].Disable();
                }
            }
        }

        private void OnTuneKeyPressed(int tuneNumber)
        {
            // Don't start new tune if already holding one
            if (_isHolding) return;

            // Validate tune number
            int idx = tuneNumber - 1;
            if (idx < 0 || idx >= 3) return;

            // Silently ignore locked tunes — player must collect scroll first
            if (!_tuneUnlocked[idx]) return;

            // Cooldown guard — silently blocked while on cooldown
            if (_cooldownTimers[idx] > 0f) return;

            // Charge guard — Advanced mode only, silently blocked when charges depleted
            if (!_isSimpleMode && _remainingCharges[idx] <= 0) return;

            // Range guard for Move/Daze (tunes 1 and 2) — silently blocked if no snake in range
            if (tuneNumber <= 2 && !HasSnakeInRange(_spellCastRange)) return;

            // Shield active guard (tune 3 only) — cannot recast Shield while already active
            if (tuneNumber == 3 && _shieldComponent != null && _shieldComponent.IsShieldActive) return;

            TuneConfig config = _tuneConfigs[idx];
            StartTune(tuneNumber, config);
        }

        private void OnTuneKeyReleased(int tuneNumber)
        {
            // Only process release if we're holding this tune
            if (_isHolding && _currentTuneNumber == tuneNumber)
            {
                ReleaseTune();
            }
        }
        #endregion

        #region Unlock System
        /// <summary>
        /// Called when a tune is unlocked via scroll collection.
        /// Sets the corresponding unlock flag so the tune key becomes functional.
        /// </summary>
        private void OnTuneUnlockedEvent(int tuneNumber)
        {
            int idx = tuneNumber - 1;
            if (idx >= 0 && idx < _tuneUnlocked.Length)
            {
                _tuneUnlocked[idx] = true;
                // Initialize charges for this tune in Advanced mode
                if (!_isSimpleMode)
                {
                    _remainingCharges[idx] = _spellCharges[idx];
                }
                Debug.Log($"TuneController: Tune {tuneNumber} unlocked!");
            }
        }
        #endregion

        #region Slider System (ADR-008)
        /// <summary>
        /// Starts a new tune - slider begins at 0.
        /// </summary>
        private void StartTune(int tuneNumber, TuneConfig config)
        {
            _currentTuneNumber = tuneNumber;
            _sliderPosition = 0f;
            _isHolding = true;

            // Load config or use defaults
            if (config != null)
            {
                _activeDuration = config.duration;
                _activeZoneStart = config.triggerZoneStart;
                _activeZoneEnd = config.triggerZoneEnd;

                // Apply simple mode bonus
                if (_isSimpleMode)
                {
                    float bonus = config.simpleModeZoneBonus;
                    _activeZoneStart = Mathf.Max(0f, _activeZoneStart - bonus / 2f);
                    _activeZoneEnd = Mathf.Min(1f, _activeZoneEnd + bonus / 2f);
                }
            }
            else
            {
                // Fallback values
                _activeDuration = _defaultDuration;
                _activeZoneStart = _defaultZoneStart;
                _activeZoneEnd = _defaultZoneEnd;

                if (_isSimpleMode)
                {
                    _activeZoneStart = Mathf.Max(0f, _activeZoneStart - _simpleModeBonus / 2f);
                    _activeZoneEnd = Mathf.Min(1f, _activeZoneEnd + _simpleModeBonus / 2f);
                }
            }

            // Notify systems
            GameEvents.TuneStarted(tuneNumber);
            Debug.Log($"TuneController: Started Tune {tuneNumber} | Duration: {_activeDuration}s | Zone: {_activeZoneStart:F2}-{_activeZoneEnd:F2}");
        }

        /// <summary>
        /// Updates slider position while holding.
        /// ADR-008: Slider moves from 0 to 1 over duration.
        /// </summary>
        private void UpdateSlider()
        {
            // Calculate speed: complete bar in _activeDuration seconds
            float speed = 1f / _activeDuration;
            _sliderPosition += speed * Time.deltaTime;

            // Auto-fail if slider reaches end (held too long)
            if (_sliderPosition >= 1f)
            {
                _sliderPosition = 1f;
                EndTune(TuneResult.TooLate);
            }
        }

        /// <summary>
        /// Called when player releases the tune key.
        /// Evaluates position against triggerzone.
        /// </summary>
        private void ReleaseTune()
        {
            TuneResult result = EvaluatePosition(_sliderPosition);
            EndTune(result);
        }

        /// <summary>
        /// Evaluates slider position against triggerzone.
        /// ADR-008 Three-outcome system.
        /// </summary>
        private TuneResult EvaluatePosition(float position)
        {
            if (position < _activeZoneStart)
            {
                return TuneResult.TooEarly; // Safe fail - no damage
            }
            else if (position <= _activeZoneEnd)
            {
                return TuneResult.Success; // In zone - charmed!
            }
            else
            {
                return TuneResult.TooLate; // Past zone - snake attacks!
            }
        }

        /// <summary>
        /// Ends tune and applies consequences based on result.
        /// </summary>
        private void EndTune(TuneResult result)
        {
            int tuneNumber = _currentTuneNumber;
            float finalPosition = _sliderPosition;

            // Reset state
            _isHolding = false;
            _currentTuneNumber = 0;
            _sliderPosition = 0f;

            // Notify release
            GameEvents.TuneReleased();

            // Apply consequences
            switch (result)
            {
                case TuneResult.TooEarly:
                    // Safe fail - zurück zu Start, kein Schaden
                    GameEvents.TuneFailed(false);
                    Debug.Log($"TuneController: Tune {tuneNumber} FAIL (Too Early) | Position: {finalPosition:F2} < Zone {_activeZoneStart:F2}");
                    break;

                case TuneResult.Success:
                    // Success — start cooldown and consume charge
                    GameEvents.TuneSuccess();

                    // Start cooldown for this tune
                    int idx = tuneNumber - 1;
                    if (idx >= 0 && idx < 3)
                    {
                        _cooldownTimers[idx] = _cooldownDurations[idx];
                        GameEvents.TuneCooldownStarted(tuneNumber, _cooldownDurations[idx]);

                        // Consume charge in Advanced mode
                        if (!_isSimpleMode)
                        {
                            _remainingCharges[idx]--;
                        }
                    }

                    // Only Move and Daze (tunes 1 and 2) fire TuneSuccessWithId — snakes react to these.
                    // Shield (tune 3) has no snake effect — SnakeAI never processes it.
                    if (tuneNumber <= 2)
                    {
                        GameEvents.TuneSuccessWithId(tuneNumber);
                    }

                    // Shield activation — Tune 3 success activates the shield directly
                    if (tuneNumber == 3 && _shieldComponent != null)
                    {
                        _shieldComponent.ActivateShield();
                    }

                    // Trigger spell animation based on tune number
                    if (_animator != null)
                    {
                        string triggerName = tuneNumber switch
                        {
                            1 => "SpellMove",
                            2 => "SpellDaze",
                            3 => "SpellShield",
                            _ => null
                        };

                        if (triggerName != null)
                        {
                            _animator.SetTrigger(triggerName);
                            Debug.Log($"TuneController: Triggered animation '{triggerName}'");
                        }
                    }

                    Debug.Log($"TuneController: Tune {tuneNumber} SUCCESS! | Position: {finalPosition:F2} in Zone [{_activeZoneStart:F2}-{_activeZoneEnd:F2}]");
                    break;

                case TuneResult.TooLate:
                    // Fail - Snake attacks!
                    GameEvents.TuneFailed(true);
                    if (_healthSystem != null)
                    {
                        _healthSystem.TakeSnakeAttack();
                    }
                    Debug.Log($"TuneController: Tune {tuneNumber} FAIL (Too Late) - SNAKE ATTACKS! | Position: {finalPosition:F2} > Zone {_activeZoneEnd:F2}");
                    break;
            }
        }
        #endregion

        #region Range Check
        /// <summary>
        /// Checks if any SnakeAI is within the given range using OverlapSphere.
        /// Used to gate Move/Daze casting — snake must be nearby to charm it.
        /// </summary>
        private bool HasSnakeInRange(float range)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, range);
            foreach (var hit in hits)
            {
                if (hit.GetComponent<SnakeEnchanter.Snakes.SnakeAI>() != null)
                    return true;
            }
            return false;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets game mode (Simple vs Advanced).
        /// </summary>
        public void SetSimpleMode(bool isSimple)
        {
            _isSimpleMode = isSimple;
            // Re-initialize charges when switching to Advanced mode
            if (!isSimple)
            {
                _remainingCharges = (int[])_spellCharges.Clone();
            }
            Debug.Log($"TuneController: Mode set to {(isSimple ? "Simple" : "Advanced")}");
        }

        /// <summary>
        /// Enables or disables tune input.
        /// </summary>
        public void SetTuneInputEnabled(bool enabled)
        {
            this.enabled = enabled;
            if (!enabled && _isHolding)
            {
                // Cancel active tune
                _isHolding = false;
                _currentTuneNumber = 0;
                _sliderPosition = 0f;
                GameEvents.TuneReleased();
            }
        }
        #endregion

        #region Debug Helpers
#if UNITY_EDITOR
        [Header("Debug - Editor Only")]
        [SerializeField] private bool _showDebugInfo = true;

        private void OnGUI()
        {
            if (!_showDebugInfo) return;

            GUI.color = Color.white;
            GUILayout.BeginArea(new Rect(10, 170, 400, 320));

            GUIStyle headerStyle = new GUIStyle(GUI.skin.label) { richText = true, fontSize = 14 };
            GUILayout.Label("<b>TuneController Debug (ADR-008 Slider)</b>", headerStyle);

            GUILayout.Label($"Holding: {_isHolding} | Tune: {_currentTuneNumber}");
            GUILayout.Label($"Slider Position: {_sliderPosition:F3}");
            GUILayout.Label($"State: {CurrentTimingState}");
            GUILayout.Label($"Mode: {(_isSimpleMode ? "Simple" : "Advanced")}");
            GUILayout.Label($"Snake in range: {_lastSnakeInRange}");
            GUILayout.Label($"Tune 1 (Move) Unlocked: {_tuneUnlocked[0]} | CD: {_cooldownTimers[0]:F1}s");
            GUILayout.Label($"Tune 2 (Daze) Unlocked: {_tuneUnlocked[1]} | CD: {_cooldownTimers[1]:F1}s");
            GUILayout.Label($"Tune 3 (Shield) Unlocked: {_tuneUnlocked[2]} | CD: {_cooldownTimers[2]:F1}s");
            if (!_isSimpleMode)
            {
                GUILayout.Label($"Charges: Move={_remainingCharges[0]}, Daze={_remainingCharges[1]}, Shield={_remainingCharges[2]}");
            }

            GUILayout.Space(10);
            GUILayout.Label($"<b>Active Zone:</b> {_activeZoneStart:F2} - {_activeZoneEnd:F2}", headerStyle);
            GUILayout.Label($"Duration: {_activeDuration:F1}s");

            // Visual slider representation
            GUILayout.Space(10);
            DrawSliderVisualization();

            GUILayout.Space(10);
            if (GUILayout.Button("Toggle Mode"))
            {
                SetSimpleMode(!_isSimpleMode);
            }

            GUILayout.EndArea();
        }

        private void DrawSliderVisualization()
        {
            // Draw ASCII-style slider bar
            int barWidth = 30;
            int sliderPos = Mathf.RoundToInt(_sliderPosition * barWidth);
            int zoneStartPos = Mathf.RoundToInt(_activeZoneStart * barWidth);
            int zoneEndPos = Mathf.RoundToInt(_activeZoneEnd * barWidth);

            string bar = "[";
            for (int i = 0; i < barWidth; i++)
            {
                if (i == sliderPos && _isHolding)
                    bar += "▼";
                else if (i >= zoneStartPos && i <= zoneEndPos)
                    bar += "█";
                else
                    bar += "░";
            }
            bar += "]";

            GUILayout.Label(bar);
            GUILayout.Label("   ░=Fail  █=Zone  ▼=Slider");
        }
#endif
        #endregion
    }
}
