/*
====================================================================
* SnakeAI - Basic snake behavior and tune interaction
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-04
* Version: 1.0

* ⚠️ WICHTIG: KOMMENTIERUNG NICHT LÖSCHEN! ⚠️
* Diese detaillierte Authorship-Dokumentation ist für die akademische
* Bewertung erforderlich und darf nicht entfernt werden!

* AUTHORSHIP CLASSIFICATION:

* [AI-ASSISTED]
* - Snake state machine architecture
* - Tune interaction system
* - Command range detection
* - Human reviewed and will modify as needed

* DEPENDENCIES:
* - GameEvents.cs (SnakeEnchanter.Core)
* - TuneConfig.cs (SnakeEnchanter.Tunes) — for SnakeEffect enum
* - Unity NavMeshAgent (optional, Phase 2)
* - Unity Animator (optional, when Toon Snakes Pack imported)

* DESIGN RATIONALE:
* - GDD Section 4: Snakes are both danger and tool
* - State machine: Idle → Charmed/Sleeping/Attacking/Frozen
* - Player must be in command range to cast tunes
* - Phase 1: Static snakes, no patrol (boceto approach)
* - Phase 2: Add NavMeshAgent patrol, animations

* NOTES:
* - Phase 1 implementation — minimal viable snake
* - No NavMesh patrol yet (Phase 2)
* - No animations yet (waiting for Toon Snakes Pack import)
* - Collider damage on contact when aggressive

* VERSION HISTORY:
* - v1.0: Initial — state machine, tune reaction, command range
====================================================================
*/

using UnityEngine;
using SnakeEnchanter.Core;
using SnakeEnchanter.Tunes;

namespace SnakeEnchanter.Snakes
{
    /// <summary>
    /// Snake states for the state machine.
    /// </summary>
    public enum SnakeState
    {
        Idle,           // Default — blocking path, not aggressive
        Aggressive,     // Attacks player on contact (after failed tune)
        MovedAway,      // Charmed with Move tune — cleared path
        Sleeping,       // Charmed with Sleep tune — passive, no collision
        AttackingEnemy, // Charmed with Attack tune — attacking other target
        Frozen,         // Freeze tune effect — temporarily immobile
        Dead            // Killed (Phase 2+)
    }

    /// <summary>
    /// Basic snake AI with state machine and tune interaction.
    /// Phase 1: Static position, reacts to tune results.
    /// Player must be within command range to cast tunes on this snake.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class SnakeAI : MonoBehaviour
    {
        #region Configuration
        [Header("Snake Identity")]
        [Tooltip("Display name for UI/debug")]
        [SerializeField] private string _snakeName = "Snake";

        [Tooltip("Unique ID for this snake instance")]
        [SerializeField] private int _snakeId = 0;

        [Header("Command Range")]
        [Tooltip("Max distance for player to cast tunes on this snake")]
        [SerializeField] private float _commandRange = 8f;

        [Tooltip("Layer mask for player detection")]
        [SerializeField] private LayerMask _playerLayer;

        [Header("Behavior")]
        [Tooltip("Damage dealt to player on contact when aggressive")]
        [SerializeField] private int _contactDamage = 10;

        [Tooltip("Time snake stays aggressive before returning to idle (seconds)")]
        [SerializeField] private float _aggressiveDuration = 5f;

        [Tooltip("Time snake stays frozen (seconds)")]
        [SerializeField] private float _freezeDuration = 4f;

        [Header("Move Away")]
        [Tooltip("Position to move to when charmed with Move tune")]
        [SerializeField] private Transform _moveAwayTarget;

        [Tooltip("Speed of move-away transition")]
        [SerializeField] private float _moveSpeed = 3f;

        [Header("Visual Feedback (Phase 1 — Color Change)")]
        [SerializeField] private Color _idleColor = Color.green;
        [SerializeField] private Color _aggressiveColor = Color.red;
        [SerializeField] private Color _sleepColor = new Color(0.5f, 0.5f, 1f, 1f); // Light blue
        [SerializeField] private Color _frozenColor = Color.cyan;
        [SerializeField] private Color _movedColor = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Gray transparent
        #endregion

        #region Private Fields
        private SnakeState _currentState = SnakeState.Idle;
        private float _stateTimer = 0f;
        private Transform _playerTransform;
        private Renderer _renderer;
        private Color _originalColor;
        private Collider _collider;
        private Vector3 _originalPosition;
        private Quaternion _originalRotation;
        private bool _isMoving = false;
        #endregion

        #region Properties
        /// <summary>Current snake state.</summary>
        public SnakeState CurrentState => _currentState;

        /// <summary>Snake display name.</summary>
        public string SnakeName => _snakeName;

        /// <summary>Snake ID.</summary>
        public int SnakeId => _snakeId;

        /// <summary>Is this snake currently targetable (can receive tune commands)?</summary>
        public bool IsTargetable => _currentState == SnakeState.Idle || _currentState == SnakeState.Aggressive;

        /// <summary>Is player within command range?</summary>
        public bool IsPlayerInRange
        {
            get
            {
                if (_playerTransform == null) return false;
                return Vector3.Distance(transform.position, _playerTransform.position) <= _commandRange;
            }
        }
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _renderer = GetComponentInChildren<Renderer>();
            _originalPosition = transform.position;
            _originalRotation = transform.rotation;

            if (_renderer != null)
            {
                _originalColor = _renderer.material.color;
            }
        }

        private void Start()
        {
            // Find player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
            else
            {
                Debug.LogWarning($"SnakeAI ({_snakeName}): No GameObject with tag 'Player' found!");
            }

            SetState(SnakeState.Idle);
        }

        private void OnEnable()
        {
            // Subscribe to tune events
            GameEvents.OnTuneSuccessWithId += OnTuneSuccessWithId;
            GameEvents.OnTuneFailed += OnTuneFailed;
        }

        private void OnDisable()
        {
            GameEvents.OnTuneSuccessWithId -= OnTuneSuccessWithId;
            GameEvents.OnTuneFailed -= OnTuneFailed;
        }

        private void Update()
        {
            UpdateState();
        }

        private void OnTriggerEnter(Collider other)
        {
            // Only deal damage when aggressive
            if (_currentState != SnakeState.Aggressive) return;
            if (!other.CompareTag("Player")) return;

            // Deal contact damage
            var healthSystem = other.GetComponent<Player.HealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(_contactDamage);
                Debug.Log($"SnakeAI ({_snakeName}): Contact damage! {_contactDamage} HP");
            }
        }
        #endregion

        #region State Machine
        /// <summary>
        /// Updates current state behavior each frame.
        /// </summary>
        private void UpdateState()
        {
            switch (_currentState)
            {
                case SnakeState.Idle:
                    // Phase 1: Static, just waiting
                    break;

                case SnakeState.Aggressive:
                    _stateTimer -= Time.deltaTime;
                    if (_stateTimer <= 0f)
                    {
                        SetState(SnakeState.Idle);
                    }
                    break;

                case SnakeState.MovedAway:
                    if (_isMoving && _moveAwayTarget != null)
                    {
                        // Smooth move to target position
                        transform.position = Vector3.MoveTowards(
                            transform.position,
                            _moveAwayTarget.position,
                            _moveSpeed * Time.deltaTime);

                        if (Vector3.Distance(transform.position, _moveAwayTarget.position) < 0.1f)
                        {
                            _isMoving = false;
                        }
                    }
                    break;

                case SnakeState.Sleeping:
                    // Stays asleep — no timer in Phase 1
                    break;

                case SnakeState.Frozen:
                    _stateTimer -= Time.deltaTime;
                    if (_stateTimer <= 0f)
                    {
                        SetState(SnakeState.Idle);
                    }
                    break;

                case SnakeState.AttackingEnemy:
                    // Phase 1: Just stays in this state (no enemy targets yet)
                    break;
            }
        }

        /// <summary>
        /// Transitions to a new state with appropriate setup.
        /// </summary>
        private void SetState(SnakeState newState)
        {
            SnakeState previousState = _currentState;
            _currentState = newState;

            switch (newState)
            {
                case SnakeState.Idle:
                    SetVisualColor(_idleColor);
                    EnableCollider(true);
                    break;

                case SnakeState.Aggressive:
                    _stateTimer = _aggressiveDuration;
                    SetVisualColor(_aggressiveColor);
                    EnableCollider(true);
                    break;

                case SnakeState.MovedAway:
                    _isMoving = true;
                    SetVisualColor(_movedColor);
                    EnableCollider(false);
                    break;

                case SnakeState.Sleeping:
                    SetVisualColor(_sleepColor);
                    EnableCollider(false); // GDD: collision disabled when sleeping
                    break;

                case SnakeState.AttackingEnemy:
                    SetVisualColor(Color.yellow);
                    EnableCollider(false);
                    break;

                case SnakeState.Frozen:
                    _stateTimer = _freezeDuration;
                    SetVisualColor(_frozenColor);
                    EnableCollider(true); // Still blocks path when frozen
                    break;
            }

            Debug.Log($"SnakeAI ({_snakeName}): {previousState} → {newState}");
        }
        #endregion

        #region Tune Interaction
        /// <summary>
        /// Called when a tune succeeds globally (with tune number).
        /// Only reacts if this snake is the closest targetable snake in range.
        /// </summary>
        private void OnTuneSuccessWithId(int tuneNumber)
        {
            // Tune 4 (Freeze) affects ALL snakes — no range/closest check
            if (tuneNumber == 4)
            {
                ApplyFreeze();
                return;
            }

            // Other tunes: Only react if player is in range and snake is targetable
            if (!IsPlayerInRange || !IsTargetable) return;

            // Check if this is the closest targetable snake
            if (!IsClosestTargetableSnake()) return;

            // Map tune number to effect
            SnakeEffect effect = tuneNumber switch
            {
                1 => SnakeEffect.Move,
                2 => SnakeEffect.Sleep,
                3 => SnakeEffect.Attack,
                _ => SnakeEffect.Move
            };

            ApplyTuneEffect(effect);
        }

        /// <summary>
        /// Called when a tune fails globally.
        /// </summary>
        private void OnTuneFailed(bool snakeAttacks)
        {
            if (!snakeAttacks) return; // Too early = safe fail, no reaction
            if (!IsPlayerInRange || !IsTargetable) return;
            if (!IsClosestTargetableSnake()) return;

            // Snake becomes aggressive on too-late failure
            SetState(SnakeState.Aggressive);
        }

        /// <summary>
        /// Applies the effect of a successful tune on this snake.
        /// </summary>
        public void ApplyTuneEffect(SnakeEffect effect)
        {
            switch (effect)
            {
                case SnakeEffect.Move:
                    SetState(SnakeState.MovedAway);
                    break;

                case SnakeEffect.Sleep:
                    SetState(SnakeState.Sleeping);
                    break;

                case SnakeEffect.Attack:
                    SetState(SnakeState.AttackingEnemy);
                    break;

                case SnakeEffect.Freeze:
                    SetState(SnakeState.Frozen);
                    break;
            }

            Debug.Log($"SnakeAI ({_snakeName}): Tune effect applied — {effect}");
        }

        /// <summary>
        /// Checks if this snake is the closest targetable snake to the player.
        /// Prevents multiple snakes from reacting to the same tune.
        /// </summary>
        private bool IsClosestTargetableSnake()
        {
            if (_playerTransform == null) return false;

            float myDistance = Vector3.Distance(transform.position, _playerTransform.position);

            // Find all snakes and check if any targetable one is closer
            SnakeAI[] allSnakes = FindObjectsByType<SnakeAI>(FindObjectsSortMode.None);
            foreach (var snake in allSnakes)
            {
                if (snake == this) continue;
                if (!snake.IsTargetable) continue;
                if (!snake.IsPlayerInRange) continue;

                float otherDistance = Vector3.Distance(
                    snake.transform.position, _playerTransform.position);

                if (otherDistance < myDistance)
                {
                    return false; // Another targetable snake is closer
                }
            }

            return true;
        }
        #endregion

        #region Visual Feedback
        /// <summary>
        /// Sets the snake's visual color for state feedback.
        /// Phase 1: Simple color change. Phase 3: Particles, glow, etc.
        /// </summary>
        private void SetVisualColor(Color color)
        {
            if (_renderer != null)
            {
                _renderer.material.color = color;
            }
        }

        private void EnableCollider(bool enabled)
        {
            if (_collider != null)
            {
                _collider.enabled = enabled;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Resets snake to original state and position (for game restart).
        /// </summary>
        public void ResetSnake()
        {
            transform.position = _originalPosition;
            transform.rotation = _originalRotation;
            _isMoving = false;
            SetState(SnakeState.Idle);
        }

        /// <summary>
        /// Forces snake into aggressive state (e.g., from external trigger).
        /// </summary>
        public void BecomeAggressive()
        {
            SetState(SnakeState.Aggressive);
        }

        /// <summary>
        /// Applies Freeze effect from Tune 4 (called on all snakes).
        /// </summary>
        public void ApplyFreeze()
        {
            if (_currentState == SnakeState.Sleeping || _currentState == SnakeState.Dead) return;
            SetState(SnakeState.Frozen);
        }
        #endregion

        #region Debug Visualization
        private void OnDrawGizmosSelected()
        {
            // Command range
            Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, _commandRange);

            // Move away target
            if (_moveAwayTarget != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, _moveAwayTarget.position);
                Gizmos.DrawWireSphere(_moveAwayTarget.position, 0.5f);
            }
        }

#if UNITY_EDITOR
        [Header("Debug - Editor Only")]
        [SerializeField] private bool _showDebugLabel = true;

        private void OnGUI()
        {
            if (!_showDebugLabel) return;

            // World-to-screen label
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2f);
            if (screenPos.z > 0) // Only if in front of camera
            {
                float labelX = screenPos.x - 60;
                float labelY = Screen.height - screenPos.y - 15;

                GUI.color = _currentState == SnakeState.Aggressive ? Color.red : Color.white;
                GUI.Label(new Rect(labelX, labelY, 120, 30),
                    $"{_snakeName}: {_currentState}");
            }
        }
#endif
        #endregion
    }
}
