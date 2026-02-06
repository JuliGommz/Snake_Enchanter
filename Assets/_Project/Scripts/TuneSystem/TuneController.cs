/*
====================================================================
* TuneController - Genshin-Style Hold & Release Slider System
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-03
* Version: 2.1 - NEW INPUT SYSTEM

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
====================================================================
*/

using UnityEngine;
using UnityEngine.InputSystem;
using SnakeEnchanter.Core;

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
    /// Player holds keys 1-4, slider moves, release in triggerzone = success.
    /// Uses New Input System exclusively.
    /// </summary>
    public class TuneController : MonoBehaviour
    {
        #region Tune Configuration
        [Header("Tune Configurations (ScriptableObjects)")]
        [Tooltip("Tune 1 - Move Command")]
        [SerializeField] private TuneConfig _tune1Config;
        [Tooltip("Tune 2 - Sleep Command")]
        [SerializeField] private TuneConfig _tune2Config;
        [Tooltip("Tune 3 - Attack Command")]
        [SerializeField] private TuneConfig _tune3Config;
        [Tooltip("Tune 4 - Freeze Command (Advanced Mode)")]
        [SerializeField] private TuneConfig _tune4Config;

        [Header("Fallback Values (if no ScriptableObject)")]
        [Tooltip("Total slider duration in seconds")]
        [SerializeField] private float _defaultDuration = 3.0f;
        [Tooltip("Triggerzone start position (0-1)")]
        [SerializeField] private float _defaultZoneStart = 0.4f;
        [Tooltip("Triggerzone end position (0-1)")]
        [SerializeField] private float _defaultZoneEnd = 0.65f;

        [Header("Tune 4 Availability")]
        [SerializeField] private bool _tune4Unlocked = false;

        [Header("Mode Settings")]
        [Tooltip("Simple Mode adds this to zone size")]
        [SerializeField] private float _simpleModeBonus = 0.1f;
        [SerializeField] private bool _isSimpleMode = true;

        [Header("Input")]
        [SerializeField] private InputActionAsset _inputActions;
        #endregion

        #region Private Fields
        // Current tune state
        private bool _isHolding = false;
        private int _currentTuneNumber = 0;
        private float _sliderPosition = 0f;

        // Active tune parameters
        private float _activeDuration;
        private float _activeZoneStart;
        private float _activeZoneEnd;

        // Reference to health system for damage
        private Player.HealthSystem _healthSystem;

        // Input System actions
        private InputAction _tune1Action;
        private InputAction _tune2Action;
        private InputAction _tune3Action;
        private InputAction _tune4Action;
        #endregion

        #region Properties
        /// <summary>
        /// Is player currently holding a tune key?
        /// </summary>
        public bool IsHolding => _isHolding;

        /// <summary>
        /// Current tune number being cast (1-4), 0 if none.
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
        /// Is Tune 4 available?
        /// </summary>
        public bool IsTune4Unlocked => _tune4Unlocked;

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
            _healthSystem = GetComponent<Player.HealthSystem>();
            if (_healthSystem == null)
            {
                _healthSystem = GetComponentInParent<Player.HealthSystem>();
            }

            SetupInputActions();
        }

        private void OnEnable()
        {
            EnableInput();
        }

        private void OnDisable()
        {
            DisableInput();
        }

        private void Update()
        {
            if (_healthSystem != null && _healthSystem.IsDead) return;

            if (_isHolding)
            {
                UpdateSlider();
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
                    _tune1Action = playerMap.FindAction("Tune1");
                    _tune2Action = playerMap.FindAction("Tune2");
                    _tune3Action = playerMap.FindAction("Tune3");
                    _tune4Action = playerMap.FindAction("Tune4");
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
            if (_tune1Action != null)
            {
                _tune1Action.Enable();
                _tune1Action.started += ctx => OnTuneKeyPressed(1);
                _tune1Action.canceled += ctx => OnTuneKeyReleased(1);
            }

            if (_tune2Action != null)
            {
                _tune2Action.Enable();
                _tune2Action.started += ctx => OnTuneKeyPressed(2);
                _tune2Action.canceled += ctx => OnTuneKeyReleased(2);
            }

            if (_tune3Action != null)
            {
                _tune3Action.Enable();
                _tune3Action.started += ctx => OnTuneKeyPressed(3);
                _tune3Action.canceled += ctx => OnTuneKeyReleased(3);
            }

            if (_tune4Action != null)
            {
                _tune4Action.Enable();
                _tune4Action.started += ctx => OnTuneKeyPressed(4);
                _tune4Action.canceled += ctx => OnTuneKeyReleased(4);
            }
        }

        private void DisableInput()
        {
            _tune1Action?.Disable();
            _tune2Action?.Disable();
            _tune3Action?.Disable();
            _tune4Action?.Disable();
        }

        private void OnTuneKeyPressed(int tuneNumber)
        {
            // Don't start new tune if already holding one
            if (_isHolding) return;

            // Check if Tune 4 is unlocked
            if (tuneNumber == 4 && !_tune4Unlocked) return;

            TuneConfig config = tuneNumber switch
            {
                1 => _tune1Config,
                2 => _tune2Config,
                3 => _tune3Config,
                4 => _tune4Config,
                _ => null
            };

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
                    // Success - Snake charmed, healing via event
                    GameEvents.TuneSuccess();
                    GameEvents.TuneSuccessWithId(tuneNumber);
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

        #region Public Methods
        /// <summary>
        /// Unlocks Tune 4 (Advanced Mode scroll pickup).
        /// </summary>
        public void UnlockTune4()
        {
            if (_tune4Unlocked) return;
            _tune4Unlocked = true;
            Debug.Log("TuneController: Tune 4 (Freeze) unlocked!");
        }

        /// <summary>
        /// Sets game mode (Simple vs Advanced).
        /// </summary>
        public void SetSimpleMode(bool isSimple)
        {
            _isSimpleMode = isSimple;
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
            GUILayout.BeginArea(new Rect(10, 170, 400, 250));

            GUIStyle headerStyle = new GUIStyle(GUI.skin.label) { richText = true, fontSize = 14 };
            GUILayout.Label("<b>TuneController Debug (ADR-008 Slider)</b>", headerStyle);

            GUILayout.Label($"Holding: {_isHolding} | Tune: {_currentTuneNumber}");
            GUILayout.Label($"Slider Position: {_sliderPosition:F3}");
            GUILayout.Label($"State: {CurrentTimingState}");
            GUILayout.Label($"Mode: {(_isSimpleMode ? "Simple" : "Advanced")}");
            GUILayout.Label($"Tune 4 Unlocked: {_tune4Unlocked}");

            GUILayout.Space(10);
            GUILayout.Label($"<b>Active Zone:</b> {_activeZoneStart:F2} - {_activeZoneEnd:F2}", headerStyle);
            GUILayout.Label($"Duration: {_activeDuration:F1}s");

            // Visual slider representation
            GUILayout.Space(10);
            DrawSliderVisualization();

            GUILayout.Space(10);
            if (GUILayout.Button("Unlock Tune 4"))
            {
                UnlockTune4();
            }
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
