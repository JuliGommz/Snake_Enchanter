/*
====================================================================
* SnakeAI - Basic snake behavior and tune interaction
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-13
* Version: 1.3.10 - Fix MoveAway Infinite Loop (Session 14)

* ⚠️ WICHTIG: KOMMENTIERUNG NICHT LÖSCHEN! ⚠️
* Diese detaillierte Authorship-Dokumentation ist für die akademische
* Bewertung erforderlich und darf nicht entfernt werden!

* AUTHORSHIP CLASSIFICATION:

* [AI-ASSISTED]
* - Snake state machine architecture
* - Tune interaction system
* - Command range detection
* - Attack system (v1.1): Range-based attacks (Bite/Breath/Projectile)
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
* - v1.1: Attack system — Bite/Breath/Projectile with range detection,
*         4s cooldown, interrupts tunes, raycast-based damage (2026-02-10)
* - v1.2: Movement improvements — Chase behavior for aggressive snakes,
*         spell animation delay for Move Away, separate normal/chase speeds (2026-02-10)
* - v1.3: Patrol & Proximity system — Random waypoint patrol in 2-3 units radius,
*         line-of-sight detection, range-based behaviors (Bite 0-0.5, Follow 0.5-3.5,
*         Breath 4-7, Projectile 8+), patrol stops when player visible (2026-02-10)
* - v1.3.1: Restored from Git (Session 14) — Full feature set recovered after
*         accidental v1.0 revert (2026-02-13)
* - v1.3.2: Bug fixes (Session 14) — Fixed Move Away infinite movement (added state
*         transition to Idle), added MoveTowardsSafe() helper for collision detection
*         via raycast, prevents phasing through walls (2026-02-13)
* - v1.3.3: Collision & Patrol debug (Session 14) — Added debug logs for patrol state,
*         increased MoveAwayTarget threshold to 1.5f (accounts for collider size),
*         added escape mechanism for snakes stuck in colliders (OverlapSphere check),
*         better blocked movement logging (2026-02-13)
* - v1.3.4: Collision fix (Session 14) — Removed OverlapSphere (caused self-detection),
*         simplified MoveTowardsSafe to only raycast for environment (not snakes),
*         snakes can now pass through each other, reduced debug spam (2026-02-13)
* - v1.3.5: Fix raycast LayerMask bug (Session 14) — CRITICAL: Fixed inverted layerMask
*         (~DefaultLayer blocked EVERYTHING), now uses Physics.Raycast default (all layers)
*         with tag-based filtering (ignore Player/Snake), added MoveAwayTarget null check,
*         improved MovedAway state logic clarity (2026-02-13)
* - v1.3.6: Environment tag system (Session 14) — Simplified collision detection to use
*         positive tag check (CompareTag("Environment") = block), all other tags passthrough,
*         cleaner semantic ("Environment blocks movement"), Unity Best Practice 2026 (2026-02-13)
* - v1.3.7: Proper tag-based collision (Session 14) — Fixed collision logic to block on ALL tags
*         EXCEPT Player (user feedback: snakes should not overlap, MoveAwayTarget should block),
*         reduced threshold to 0.5f (from 1.5f) for precise stopping at targets, tag-based
*         system now prevents snake stacking like in user's previous project (2026-02-13)
* - v1.3.8: Fix Player collision bug (Session 14) — CRITICAL: Player must BLOCK not passthrough
*         (user feedback: snakes attack player, must stop near player for bite range), removed
*         Player passthrough logic, ALL objects now block movement correctly (2026-02-13)
* - v1.3.9: Fix MoveAwayTarget stopping + Attack ranges (Session 14) — THREE CRITICAL FIXES:
*         1) MoveAwayTarget stopping: Detect MoveAwayTarget collider blocking snake (raycast check),
*            increased threshold to 1.0f to account for collider size, snake stops when blocked
*         2) State transition: Added TransitionFromMoveAwayToRootState() method, evaluates player
*            position after reaching target, returns to Idle (resumes attacks/patrol intelligently)
*         3) Attack range gaps: Fixed 3.5-4 units and 7-8 units gaps where snakes did nothing,
*            now approach player in gaps (FollowPlayer), ensures continuous engagement (2026-02-13)
* - v1.3.10: Fix MoveAway infinite loop (Session 14) — CRITICAL FIX for stuck snakes:
*         ROOT CAUSE: MoveAwayTarget GameObjects had "Untagged" instead of "MoveAwayTarget" tag
*         + snakes blocked by obstacles before reaching distant targets (>4 units) with no timeout
*         FIXES: 1) Scene: Tagged both MoveAwayTarget objects with "MoveAwayTarget" tag
*         2) Code: Added 2-second timeout when blocked by non-target obstacles (prevents infinite loop)
*         3) Code: Reset timeout counter when movement succeeds (only counts continuous blocking)
*         Result: Snakes now properly exit MovedAway state via tag detection OR timeout (2026-02-13)
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
    /// Snake types with different attack capabilities.
    /// </summary>
    public enum SnakeType
    {
        Basic,    // Toon Snake - Only Bite Attack
        Advanced  // Toon Cobra - Bite, Breath, Projectile
    }

    /// <summary>
    /// Attack types based on range.
    /// </summary>
    public enum AttackType
    {
        None,
        Bite,
        Breath,
        Projectile
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

        [Tooltip("Normal movement speed (for Move Away, patrol, etc.)")]
        [SerializeField] private float _moveSpeed = 0.4f;

        [Tooltip("Delay before snake starts moving after spell cast (seconds)")]
        [SerializeField] private float _spellAnimationDelay = 3.5f;

        [Header("Attack System")]
        [Tooltip("Snake type determines attack capabilities")]
        [SerializeField] private SnakeType _snakeType = SnakeType.Basic;

        [Tooltip("Bite attack range (close combat, 0-0.5 units)")]
        [SerializeField] private float _biteRange = 0.5f;

        [Tooltip("Breath attack range (medium range, 4-7 units)")]
        [SerializeField] private float _breathRange = 7f;

        [Tooltip("Projectile attack range (far range, Cobra Advanced mode only)")]
        [SerializeField] private float _projectileRange = 8f;

        [Tooltip("Damage dealt by bite attack")]
        [SerializeField] private int _biteDamage = 15;

        [Tooltip("Damage dealt by breath attack")]
        [SerializeField] private int _breathDamage = 10;

        [Tooltip("Damage dealt by projectile attack")]
        [SerializeField] private int _projectileDamage = 8;

        [Tooltip("Cooldown between attacks (seconds)")]
        [SerializeField] private float _attackCooldown = 4f;

        [Header("Proximity & Behavior")]
        [Tooltip("Maximum distance to detect and react to player")]
        [SerializeField] private float _detectionRange = 10f;

        [Tooltip("Follow player in this range (0.5-3.5 units)")]
        [SerializeField] private float _followRangeMin = 0.5f;
        [SerializeField] private float _followRangeMax = 3.5f;

        [Tooltip("Breath attack range (4-7 units)")]
        [SerializeField] private float _breathRangeMin = 4f;
        [SerializeField] private float _breathRangeMax = 7f;

        [Tooltip("Speed when following/chasing player")]
        [SerializeField] private float _chaseSpeed = 1f;

        [Header("Patrol System")]
        [Tooltip("Patrol radius around start position (2-3 units)")]
        [SerializeField] private float _patrolRadiusMin = 2f;
        [SerializeField] private float _patrolRadiusMax = 3f;

        [Tooltip("Time to wait at each patrol waypoint (seconds)")]
        [SerializeField] private float _patrolWaitTime = 2f;

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

        // Attack system
        private Animator _animator;
        private float _lastAttackTime = -999f;
        private bool _isAdvancedMode = false;

        // Patrol system
        private Vector3 _currentPatrolTarget;
        private bool _isPatrolling = false;
        private float _patrolWaitTimer = 0f;
        private bool _isWaitingAtWaypoint = false;

        // Proximity detection
        private bool _canSeePlayer = false;
        private float _playerDistance = Mathf.Infinity;
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
            _animator = GetComponent<Animator>();
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

            // Get game mode from GameManager (now it's initialized)
            if (Core.GameManager.Instance != null)
            {
                _isAdvancedMode = Core.GameManager.Instance.CurrentMode == Core.GameMode.Advanced;
                // Debug.Log($"SnakeAI ({_snakeName}): Game Mode = {(_isAdvancedMode ? "Advanced" : "Simple")}");
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
            UpdateProximityDetection();
            UpdatePatrol();
            UpdateState();
            CheckAndTriggerAttack();
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
                // Debug.Log($"SnakeAI ({_snakeName}): Contact damage! {_contactDamage} HP");
            }
        }
        #endregion

        #region Proximity Detection
        /// <summary>
        /// Updates proximity detection - checks if snake can see player and distance.
        /// Uses raycast for line-of-sight detection.
        /// </summary>
        private void UpdateProximityDetection()
        {
            if (_playerTransform == null)
            {
                _canSeePlayer = false;
                _playerDistance = Mathf.Infinity;
                return;
            }

            // Calculate distance
            _playerDistance = Vector3.Distance(transform.position, _playerTransform.position);

            // Check if player is within detection range
            if (_playerDistance > _detectionRange)
            {
                _canSeePlayer = false;
                return;
            }

            // Raycast for line-of-sight
            Vector3 directionToPlayer = (_playerTransform.position - transform.position).normalized;
            Vector3 rayOrigin = transform.position + Vector3.up * 0.5f; // Slightly elevated

            if (Physics.Raycast(rayOrigin, directionToPlayer, out RaycastHit hit, _detectionRange))
            {
                bool canSee = hit.collider.CompareTag("Player");
                if (canSee != _canSeePlayer) // State changed
                {
                    // Debug.Log($"SnakeAI ({_snakeName}): Player visibility changed: {canSee} (hit: {hit.collider.name}, distance: {_playerDistance:F2})");
                }
                _canSeePlayer = canSee;
            }
            else
            {
                _canSeePlayer = false;
            }
        }
        #endregion

        #region Patrol System
        /// <summary>
        /// Updates patrol behavior - moves to random waypoints around start position.
        /// Only active in Idle state when player is NOT visible.
        /// </summary>
        private void UpdatePatrol()
        {
            // Patrol only in Idle state
            if (_currentState != SnakeState.Idle)
            {
                // Silent - Patrol blocked by non-Idle state (normal behavior)
                return;
            }

            // Stop patrol if player is visible
            if (_canSeePlayer)
            {
                if (_isPatrolling)
                {
                    // Debug.Log($"SnakeAI ({_snakeName}): Patrol stopped - Player visible");
                }
                _isPatrolling = false;
                _isWaitingAtWaypoint = false;
                return;
            }

            // Waiting at waypoint
            if (_isWaitingAtWaypoint)
            {
                _patrolWaitTimer -= Time.deltaTime;
                if (_patrolWaitTimer <= 0f)
                {
                    _isWaitingAtWaypoint = false;
                    GenerateNewPatrolWaypoint();
                }
                return;
            }

            // Start patrolling if not yet started
            if (!_isPatrolling)
            {
                Debug.Log($"SnakeAI ({_snakeName}): Starting patrol from {_originalPosition}");
                GenerateNewPatrolWaypoint();
                _isPatrolling = true;
            }

            // Move toward patrol target (with collision detection)
            float patrolSpeed = _moveSpeed * 0.75f; // 25% slower than normal movement
            MoveTowardsSafe(_currentPatrolTarget, patrolSpeed);

            // Rotate toward target
            Vector3 directionToTarget = (_currentPatrolTarget - transform.position).normalized;
            if (directionToTarget != Vector3.zero)
            {
                Vector3 lookTarget = new Vector3(_currentPatrolTarget.x, transform.position.y, _currentPatrolTarget.z);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(directionToTarget),
                    Time.deltaTime * 2f);
            }

            // Check if reached waypoint
            if (Vector3.Distance(transform.position, _currentPatrolTarget) < 0.2f)
            {
                _isWaitingAtWaypoint = true;
                _patrolWaitTimer = _patrolWaitTime;
            }
        }

        /// <summary>
        /// Generates a new random patrol waypoint around start position.
        /// </summary>
        private void GenerateNewPatrolWaypoint()
        {
            float radius = Random.Range(_patrolRadiusMin, _patrolRadiusMax);
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * radius,
                0f,
                Mathf.Sin(angle) * radius);

            _currentPatrolTarget = _originalPosition + offset;
            // Debug.Log($"SnakeAI ({_snakeName}): New patrol waypoint: {_currentPatrolTarget} (radius: {radius:F2})");
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
                    // Idle behavior: Patrol OR react to player if visible
                    if (_canSeePlayer)
                    {
                        HandleIdlePlayerInteraction();
                    }
                    // Patrol happens in UpdatePatrol()
                    break;

                case SnakeState.Aggressive:
                    // Aggressive state now only used for Failed Tune reaction
                    _stateTimer -= Time.deltaTime;
                    if (_stateTimer <= 0f)
                    {
                        SetState(SnakeState.Idle);
                    }

                    // Follow player and bite once
                    FollowPlayerForFailedTune();
                    break;

                case SnakeState.MovedAway:
                    if (_moveAwayTarget != null)
                    {
                        // Only move if spell animation delay has passed
                        if (!_isMoving)
                        {
                            // Waiting for spell animation delay (3.5s)
                            break;
                        }

                        float distanceToTarget = Vector3.Distance(transform.position, _moveAwayTarget.position);

                        // Check if reached target (distance-based)
                        if (distanceToTarget < 1.0f)
                        {
                            _isMoving = false;
                            TransitionFromMoveAwayToRootState();
                            Debug.Log($"SnakeAI ({_snakeName}): Reached MoveAwayTarget at distance {distanceToTarget:F2}, transitioning to root state");
                        }
                        else
                        {
                            // Smooth move to target position (with collision detection)
                            bool moved = MoveTowardsSafe(_moveAwayTarget.position, _moveSpeed);

                            // Check if blocked - could be obstacle OR the target itself
                            if (!moved)
                            {
                                // Raycast to see what's blocking
                                Vector3 direction = (_moveAwayTarget.position - transform.position).normalized;
                                RaycastHit hit;
                                if (Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, out hit, 1.5f))
                                {
                                    // If blocked by MoveAwayTarget, we've arrived
                                    if (hit.collider.CompareTag("MoveAwayTarget"))
                                    {
                                        _isMoving = false;
                                        TransitionFromMoveAwayToRootState();
                                        Debug.Log($"SnakeAI ({_snakeName}): Blocked by MoveAwayTarget collider (tag detected), reached destination");
                                    }
                                    // If blocked by obstacle and not making progress, give up after timeout
                                    else
                                    {
                                        _stateTimer += Time.deltaTime;
                                        if (_stateTimer > 2.0f) // 2 seconds of continuous blocking
                                        {
                                            _isMoving = false;
                                            _stateTimer = 0f;
                                            TransitionFromMoveAwayToRootState();
                                            Debug.LogWarning($"SnakeAI ({_snakeName}): Blocked by obstacle for 2s, giving up on MoveAwayTarget");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Successfully moved - reset timeout counter
                                _stateTimer = 0f;
                            }
                        }
                    }
                    else
                    {
                        // No target set - return to Idle
                        Debug.LogWarning($"SnakeAI ({_snakeName}): MovedAway state but no MoveAwayTarget! Returning to Idle.");
                        SetState(SnakeState.Idle);
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
        /// Handles player interaction during Idle state based on distance ranges.
        /// - 0-0.5: Bite Attack (look at player)
        /// - 0.5-3.5: Follow player
        /// - 3.5-4: Approach (close gap for breath attack)
        /// - 4-7: Breath Attack (look at player)
        /// - 7-8: Approach (close gap for projectile)
        /// - 8+: Projectile (look at player, Advanced mode only)
        /// Default: Look at player if visible
        /// </summary>
        private void HandleIdlePlayerInteraction()
        {
            if (_playerTransform == null || !_canSeePlayer) return;

            // Range-based behavior
            if (_playerDistance <= _biteRange)
            {
                // Bite Attack range (0-0.5 units)
                // Attack is handled in CheckAndTriggerAttack()
                // Just look at player
                LookAtPlayer();
            }
            else if (_playerDistance > _followRangeMin && _playerDistance <= _followRangeMax)
            {
                // Follow range (0.5-3.5 units)
                FollowPlayer();
            }
            else if (_playerDistance > _followRangeMax && _playerDistance < _breathRangeMin)
            {
                // Gap range (3.5-4 units) - approach for breath attack
                FollowPlayer();
            }
            else if (_playerDistance >= _breathRangeMin && _playerDistance <= _breathRangeMax)
            {
                // Breath Attack range (4-7 units)
                // Attack is handled in CheckAndTriggerAttack()
                // Just look at player
                LookAtPlayer();
            }
            else if (_playerDistance > _breathRangeMax && _playerDistance < _projectileRange)
            {
                // Gap range (7-8 units) - approach for projectile (or look if Simple mode)
                if (_isAdvancedMode)
                {
                    FollowPlayer(); // Close gap to projectile range
                }
                else
                {
                    LookAtPlayer(); // Simple mode: no projectile, just watch
                }
            }
            else if (_playerDistance >= _projectileRange && _isAdvancedMode)
            {
                // Projectile range (8+ units, Advanced only)
                // Attack is handled in CheckAndTriggerAttack()
                // Just look at player
                LookAtPlayer();
            }
            else
            {
                // Default: Look at player (covers any edge cases)
                LookAtPlayer();
            }
        }

        /// <summary>
        /// Follow player smoothly (used in 0.5-3.5 units range).
        /// </summary>
        private void FollowPlayer()
        {
            if (_playerTransform == null) return;

            // Move toward player (with collision detection)
            MoveTowardsSafe(_playerTransform.position, _chaseSpeed);

            LookAtPlayer();
        }

        /// <summary>
        /// Moves snake toward target with collision detection.
        /// Uses raycast to check for obstacles.
        /// Returns true if movement was successful, false if blocked.
        ///
        /// Tag-based blocking (EVERYTHING blocks):
        /// - Environment: BLOCKS (walls, floors)
        /// - Snake: BLOCKS (prevent snake overlap)
        /// - MoveAwayTarget: BLOCKS (snake stops at target)
        /// - Player: BLOCKS (snake stops near player for attack)
        /// - Untagged: BLOCKS (default safe behavior)
        /// </summary>
        private bool MoveTowardsSafe(Vector3 targetPosition, float speed)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            float distance = speed * Time.deltaTime;

            // Raycast to check for obstacles ahead
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, out hit, distance + 0.3f))
            {
                // ALL objects block movement (including Player)
                // This is correct: Snake stops near Player to attack, doesn't phase through
                Debug.Log($"SnakeAI ({_snakeName}): Movement blocked by {hit.collider.name} (Tag: {hit.collider.tag}) at distance {hit.distance:F2}");
                return false;
            }

            // Safe to move
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, distance);
            return true;
        }

        /// <summary>
        /// Rotate to face player (Y-axis only).
        /// </summary>
        private void LookAtPlayer()
        {
            if (_playerTransform == null) return;

            Vector3 lookTarget = new Vector3(_playerTransform.position.x, transform.position.y, _playerTransform.position.z);
            Vector3 direction = (lookTarget - transform.position).normalized;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction),
                    Time.deltaTime * 5f);
            }
        }

        /// <summary>
        /// Follow player and bite once (Failed Tune reaction).
        /// Used in Aggressive state after failed tune.
        /// </summary>
        private void FollowPlayerForFailedTune()
        {
            if (_playerTransform == null) return;

            // Follow player until bite range
            if (_playerDistance > _biteRange)
            {
                FollowPlayer();
            }
            // Bite attack is triggered automatically in CheckAndTriggerAttack()
        }

        /// <summary>
        /// Starts the Move Away movement after spell animation delay.
        /// Called via Invoke() from SetState(MovedAway).
        /// </summary>
        private void StartMoveAwayMovement()
        {
            if (_currentState == SnakeState.MovedAway)
            {
                _isMoving = true;
            }
        }

        /// <summary>
        /// Transitions from MovedAway state back to appropriate root state.
        /// Evaluates player position/visibility to decide next state:
        /// - If player visible and in range → Idle (will trigger attack/follow in next Update)
        /// - If player not visible → Idle (will resume patrol)
        /// </summary>
        private void TransitionFromMoveAwayToRootState()
        {
            // Always return to Idle state
            // Idle state logic will handle player interaction if visible
            // Or resume patrol if player not visible
            SetState(SnakeState.Idle);

            Debug.Log($"SnakeAI ({_snakeName}): MoveAway complete → Idle (Player visible: {_canSeePlayer}, Distance: {_playerDistance:F1})");
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
                    _isMoving = false; // Wait for spell animation delay
                    SetVisualColor(_movedColor);
                    EnableCollider(false);
                    // Start movement after spell animation delay
                    Invoke(nameof(StartMoveAwayMovement), _spellAnimationDelay);
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

            // Debug.Log($"SnakeAI ({_snakeName}): {previousState} → {newState}");
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

            // Debug.Log($"SnakeAI ({_snakeName}): Tune effect applied — {effect}");
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

        #region Attack System
        /// <summary>
        /// Checks player distance and triggers appropriate attack if in range.
        /// Called every frame. Respects cooldown and state requirements.
        /// Only attacks if player is visible (line of sight).
        /// </summary>
        private void CheckAndTriggerAttack()
        {
            // Only attack if Idle or Aggressive
            if (_currentState != SnakeState.Idle && _currentState != SnakeState.Aggressive)
                return;

            // Only attack if player is visible
            if (!_canSeePlayer)
                return;

            // Check cooldown
            if (Time.time - _lastAttackTime < _attackCooldown)
                return;

            // Need player reference
            if (_playerTransform == null)
                return;

            // Determine attack type based on distance and snake type
            AttackType attackType = DetermineAttackType(_playerDistance);

            if (attackType != AttackType.None)
            {
                TriggerAttack(attackType);
            }
        }

        /// <summary>
        /// Determines which attack to use based on distance and snake capabilities.
        /// New ranges: Bite (0-0.5), Breath (4-7), Projectile (8+)
        /// </summary>
        private AttackType DetermineAttackType(float distance)
        {
            // Projectile (Advanced Cobra only, 8+ units)
            if (_snakeType == SnakeType.Advanced && _isAdvancedMode &&
                distance >= _projectileRange)
            {
                return AttackType.Projectile;
            }

            // Breath (Cobra, 4-7 units)
            if (_snakeType == SnakeType.Advanced &&
                distance >= _breathRangeMin && distance <= _breathRangeMax)
            {
                return AttackType.Breath;
            }

            // Bite (All snakes, 0-0.5 units)
            if (distance <= _biteRange)
            {
                return AttackType.Bite;
            }

            return AttackType.None;
        }

        /// <summary>
        /// Triggers attack animation and schedules damage.
        /// </summary>
        private void TriggerAttack(AttackType attackType)
        {
            _lastAttackTime = Time.time;

            // Trigger animator
            if (_animator != null)
            {
                switch (attackType)
                {
                    case AttackType.Bite:
                        _animator.SetTrigger("Bite Attack");
                        break;
                    case AttackType.Breath:
                        // Breath Attack is a BOOL (Type 4) in Animator Controller
                        _animator.SetBool("Breath Attack", true);
                        Invoke(nameof(ResetBreathBool), 2f); // Reset after animation
                        break;
                    case AttackType.Projectile:
                        _animator.SetTrigger("Projectile Attack");
                        break;
                }
            }

            // Determine damage and delay
            int damage = attackType switch
            {
                AttackType.Bite => _biteDamage,
                AttackType.Breath => _breathDamage,
                AttackType.Projectile => _projectileDamage,
                _ => 0
            };

            // Animation delays (approximate)
            float damageDelay = attackType switch
            {
                AttackType.Bite => 0.3f,        // Quick bite
                AttackType.Breath => 0.5f,      // Breath wind-up
                AttackType.Projectile => 0.6f,  // Projectile shoot
                _ => 0f
            };

            // Schedule damage after animation delay
            Invoke(nameof(DealScheduledDamage), damageDelay);
            _scheduledDamage = damage;

            // Debug.Log($"SnakeAI ({_snakeName}): {attackType} attack triggered! Damage: {damage}");
        }

        private int _scheduledDamage = 0;

        /// <summary>
        /// Resets Breath Attack bool after animation completes.
        /// </summary>
        private void ResetBreathBool()
        {
            if (_animator != null)
            {
                _animator.SetBool("Breath Attack", false);
            }
        }

        /// <summary>
        /// Deals damage to player after attack animation delay.
        /// Uses Raycast to check if player is still in line of sight.
        /// </summary>
        private void DealScheduledDamage()
        {
            if (_playerTransform == null) return;

            // Raycast to player (line of sight check)
            Vector3 directionToPlayer = (_playerTransform.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

            if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out RaycastHit hit, distanceToPlayer + 1f))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    var healthSystem = hit.collider.GetComponent<Player.HealthSystem>();
                    if (healthSystem != null)
                    {
                        healthSystem.TakeDamage(_scheduledDamage);
                        // Debug.Log($"SnakeAI ({_snakeName}): Hit player for {_scheduledDamage} damage!");
                    }
                }
            }
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
            // Command range (yellow)
            Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, _commandRange);

            // Attack ranges
            // Bite range (red)
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, _biteRange);

            // Breath range (orange, Cobra only)
            if (_snakeType == SnakeType.Advanced)
            {
                Gizmos.color = new Color(1f, 0.5f, 0f, 0.2f);
                Gizmos.DrawWireSphere(transform.position, _breathRange);
            }

            // Projectile range (purple, Cobra Advanced only)
            if (_snakeType == SnakeType.Advanced)
            {
                Gizmos.color = new Color(0.5f, 0f, 1f, 0.1f);
                Gizmos.DrawWireSphere(transform.position, _projectileRange);
            }

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
