/*
====================================================================
* SnakeAI - Basic snake behavior and tune interaction
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-14
* Version: 1.9 - Phase 7: fires SnakeCharmed event, removes Attack/Freeze dead code

* ⚠️ WICHTIG: KOMMENTIERUNG NICHT LÖSCHEN! ⚠️
* Diese detaillierte Authorship-Dokumentation ist für die akademische
* Bewertung erforderlich und darf nicht entfernt werden!

* AUTHORSHIP CLASSIFICATION:

* [AI-ASSISTED]
* - Snake state machine architecture
* - Tune interaction system
* - Command range detection
* - Attack system (v1.1): Range-based attacks (Bite/Breath/Projectile)
* - Attack VFX system (v1.4.0): Particle spawning, lifetime management
* - Human reviewed and will modify as needed

* DEPENDENCIES:
* - GameEvents.cs (SnakeEnchanter.Core)
* - TuneConfig.cs (SnakeEnchanter.Tunes) — for SnakeEffect enum
* - Unity NavMeshAgent (optional, Phase 2)
* - Unity Animator (optional, when Toon Snakes Pack imported)

* DESIGN RATIONALE:
* - GDD Section 4: Snakes are both danger and tool
* - State machine: Idle → Charmed/Dazed
* - Player must be in command range to cast tunes
* - Phase 1: Static snakes, no patrol (boceto approach)
* - Phase 2: Add NavMeshAgent patrol, animations

* NOTES:
* - Phase 5 implementation — full NavMesh movement
* - NavMeshAgent drives position (updatePosition=false, manual LateUpdate sync)
* - LookAtPlayer() handles rotation — NavMeshAgent updateRotation=false
* - Toon Snakes Pack animations: Slither Forward/Left/Right, Bite, Breath, Die
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
* - v1.4.0: Attack VFX system (Session 15) — Visual effects for Breath/Projectile attacks:
*         ADDED: SerializeField references for 3 FX prefabs (Poison Breath, Projectile, Impact)
*         ADDED: _fxSpawnPoint Transform for precise VFX positioning (e.g., mouth bone)
*         ADDED: SpawnAttackFX() method — spawns, plays, auto-destroys particle effects
*         INTEGRATION: TriggerAttack() now calls SpawnAttackFX() after animation trigger
*         FEATURES: Projectile FX auto-rotates to face player, lifetime-based cleanup
*         PREFAB FIX: Disabled playOnAwake + looping on FX prefabs (manual .Play() control)
*         Result: Breath/Projectile attacks now have visual particle effects (2026-02-14)
* - v1.4.1: Restore Session 14 Fixes (Session 15) — Fixes lost during git revert:
*         RESTORED: Target-Detach in Awake() (v1.3.14) - MoveAwayTarget detached at start
*         RESTORED: Raycast Distance 1.0 unit min (v1.3.13) - Props collision working
*         RESTORED: SetVisualColor() URP support (v1.3.11) - _BaseColor property
*         Result: Props block movement, MoveAwayTarget reaches destination (2026-02-14)
* - v1.4.2: SphereCast collision fix (Session 15) — FINAL FIX for Props/Snake collision:
*         ROOT CAUSE: Physics.Raycast fails when origin inside collider volume
*         SOLUTION: Replaced Raycast with SphereCast(radius=0.3f)
*         EFFECT: Snakes now detect Props/other Snakes even when overlapping
*         CLEANUP: Removed debug logs (Debug.Log, Debug.DrawRay)
*         Result: Props collision WORKING, Snakes no longer overlap (2026-02-14)
* - v1.5.0: Tune 2 & 3 Spell Behaviors (Session 16) — Complete spell response system:
*         TUNE 2 (Daze): Snake becomes dazed/stunned, collider disabled, DIE animation, 8s timer
*         RENAME: Sleeping → Dazed (no sleep animation, Die animation shows unconscious/collapsed)
*         Result: Tune behaviors functional (Move, Daze) (2026-02-14)
* - v1.6.0: Directional Slither & Debug Logging (Session 16) — Movement animations + logs:
*         SLITHER DIRECTIONAL: UpdateMovementAnimation() supports all 3 directions (Forward/Left/Right)
*         ADDED: _lastMoveDirection Vector3 - Tracks movement vector for directional slither
*         UPDATED: MoveTowardsSafe() - Captures movement direction from position delta
*         DEBUG LOGGING: All spell states (Move, Daze) with detailed parameters
*         Result: Directional slither animations (2026-02-14)
* - v1.7.0: Spell Targeting & Animation Fixes (Session 17) — Critical bug fixes:
*         FIXED: IsPlayerInRange now checks BOTH distance AND line-of-sight (_canSeePlayer)
*         REASON: Spells were targeting snakes through walls (only checked distance)
*         REMOVED: Unused _playerLayer field
*         Result: Spells now require line-of-sight (2026-02-15)
* - v1.8.0: NavMeshAgent Component Integration (Phase 4) — Passive dual-system setup
* - v1.8.1: NavMeshAgent Activation (Phase 5) — Active dual-system replacement
* - v1.8.2: NavMesh Patrol Replacement (Phase 5 Plan 02) — Patrol via SetDestination
* - v1.8.3: NavMesh Full Migration (Phase 5 Plan 03) — All movement via NavMeshAgent
* - v1.8.4: Root Motion Fix (Phase 5 Bug-Fix) — Patrol animation snap FIXED
* - v1.8.5: Cleanup & Polish (Phase 6) — Submission-ready, zero console spam
* - v1.9: Phase 7 spell system integration:
*         ADDED: GameEvents.SnakeCharmed(tuneNumber) fired in ApplyTuneEffect for Move and Daze
*         REMOVED: SnakeState.AttackingEnemy and SnakeState.Frozen (Attack/Freeze removed from system)
*         REMOVED: _freezeDuration and _frozenColor fields (Freeze tune removed in Phase 7)
*         REMOVED: StartAttackingEnemy(), FindNearestCreature(), NeutralizeAfterAttack(), ApplyFreeze()
*         REMOVED: SetState() and UpdateState() cases for AttackingEnemy and Frozen
*         UPDATED: OnTuneSuccessWithId() — no longer maps tune 3 to Attack or tune 4 to Freeze
*         NOTE: TuneController no longer fires TuneSuccessWithId for tune 3 (Shield)
====================================================================
*/

using UnityEngine;
using UnityEngine.AI;
using SnakeEnchanter.Core;
using SnakeEnchanter.Tunes;

namespace SnakeEnchanter.Snakes
{
    /// <summary>
    /// Snake states for the state machine.
    /// </summary>
    public enum SnakeState
    {
        Idle,       // Default — blocking path, not aggressive
        Aggressive, // Attacks player on contact (after failed tune)
        MovedAway,  // Charmed with Move tune — cleared path
        Entranced,  // Listening to Daze melody — faces player, no attack, 3s then → Dazed
        Dazed,      // Charmed with Daze tune — passive, stunned, no collision
        Dead        // Killed (Phase 2+)
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

        [Header("Behavior")]
        [Tooltip("Damage dealt to player on contact when aggressive")]
        [SerializeField] private int _contactDamage = 10;

        [Tooltip("Time snake stays aggressive before returning to idle (seconds)")]
        [SerializeField] private float _aggressiveDuration = 5f;

        [Tooltip("Time snake listens/is entranced before falling asleep (seconds)")]
        [SerializeField] private float _entrancedDuration = 3f;

        [Tooltip("Time snake stays dazed before returning to idle (seconds)")]
        [SerializeField] private float _dazedDuration = 8f;

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
        [SerializeField] private Color _entrancedColor = new Color(1f, 0.85f, 0.2f, 1f); // Amber/gold — listening
        [SerializeField] private Color _dazedColor = new Color(0.5f, 0.5f, 1f, 1f); // Light blue
        [SerializeField] private Color _movedColor = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Gray transparent

        [Header("Attack VFX (Phase 2)")]
        [Tooltip("Poison Breath VFX prefab (spawned at snake mouth during breath attack)")]
        [SerializeField] private GameObject _poisonBreathFXPrefab;

        [Tooltip("Poison Projectile VFX prefab (spawned and travels to player during projectile attack)")]
        [SerializeField] private GameObject _projectileFXPrefab;

        [Tooltip("Poison Impact VFX prefab (spawned at hit location when projectile hits)")]
        [SerializeField] private GameObject _impactFXPrefab;

        [Tooltip("Transform point for FX spawn (e.g., mouth bone). If null, uses snake position.")]
        [SerializeField] private Transform _fxSpawnPoint;
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

        // NavMesh (Phase 4+)
        private NavMeshAgent _agent;
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

        /// <summary>
        /// Is player within command range AND visible (line-of-sight)?
        /// Used for spell targeting - prevents spells from targeting snakes through walls.
        /// </summary>
        public bool IsPlayerInRange
        {
            get
            {
                if (_playerTransform == null) return false;

                // Check distance first (cheap check)
                float distance = Vector3.Distance(transform.position, _playerTransform.position);
                if (distance > _commandRange) return false;

                // Check line-of-sight (requires visibility for spell targeting)
                // _canSeePlayer is updated by UpdateProximityDetection() every frame
                return _canSeePlayer;
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

            // CRITICAL (Phase 5): Disable Root Motion — NavMeshAgent drives position.
            // Apply Root Motion = true in Animator fights with agent.updatePosition = true,
            // causing the snake to snap back to spawn every frame.
            if (_animator != null)
                _animator.applyRootMotion = false;

            if (_renderer != null)
            {
                _originalColor = _renderer.material.color;
            }

            // CRITICAL FIX (v1.3.14): Detach MoveAwayTarget from Snake hierarchy at START
            // This prevents target from moving with Snake during Patrol/Follow
            if (_moveAwayTarget != null && _moveAwayTarget.parent == transform)
            {
                Vector3 worldPos = _moveAwayTarget.position;
                _moveAwayTarget.SetParent(null);
                _moveAwayTarget.position = worldPos;
            }

            // NavMeshAgent activation (Phase 5)
            // updatePosition = FALSE: we manually sync transform.position from agent.nextPosition
            // in LateUpdate(). This prevents the agent-body vs root-motion mesh desync.
            _agent = GetComponent<NavMeshAgent>();
            if (_agent != null)
            {
                _agent.nextPosition = transform.position;
                _agent.updatePosition = false;  // Manual sync in LateUpdate()
                _agent.updateRotation = false;  // Manual rotation via LookAtPlayer()
                _agent.speed = _moveSpeed * 0.75f;
                _agent.stoppingDistance = 0.2f;
                if (_agent.isOnNavMesh)
                    _agent.isStopped = false;
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
            UpdateMovementAnimation();
        }

        private void LateUpdate()
        {
            // Sync transform.position from NavMeshAgent internal position.
            // updatePosition=false gives us control over WHEN the write happens —
            // after animation, so root-motion can't fight the agent this frame.
            if (_agent != null && _agent.isOnNavMesh)
            {
                transform.position = _agent.nextPosition;
            }
        }

        /// <summary>
        /// Updates movement animation based on NavMeshAgent velocity.
        /// Uses agent.velocity.magnitude for movement detection (fixes animation restart bug).
        /// OLD: _isPatrolling bool → caused restart when blocked (bool=true but no movement)
        /// NEW: velocity check → only true when agent is ACTUALLY moving
        /// </summary>
        private void UpdateMovementAnimation()
        {
            if (_animator == null) return;

            // CRITICAL FIX: Use actual agent velocity, not boolean state
            // _isPatrolling bool was true even when blocked → animation restarted every frame
            // velocity.magnitude is 0 when truly stopped → animation holds last frame
            bool isActuallyMoving = _agent != null &&
                                    _agent.velocity.magnitude > 0.1f &&
                                    (_currentState == SnakeState.Aggressive ||
                                     _currentState == SnakeState.Idle ||
                                     _currentState == SnakeState.MovedAway);

            // Reset all slither bools
            _animator.SetBool("Slither Forward", false);
            _animator.SetBool("Slither Left", false);
            _animator.SetBool("Slither Right", false);

            if (!isActuallyMoving) return;

            // Derive direction from agent velocity (NOT _lastMoveDirection — that field is deleted)
            Vector3 localDirection = transform.InverseTransformDirection(_agent.velocity.normalized);

            float forwardAmount = localDirection.z;
            float rightAmount = localDirection.x;

            if (Mathf.Abs(forwardAmount) > Mathf.Abs(rightAmount))
            {
                _animator.SetBool("Slither Forward", true);
            }
            else if (rightAmount > 0.1f)
            {
                _animator.SetBool("Slither Right", true);
            }
            else if (rightAmount < -0.1f)
            {
                _animator.SetBool("Slither Left", true);
            }
            else
            {
                _animator.SetBool("Slither Forward", true);
            }
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
                return;
            }

            // Stop patrol if player is visible
            if (_canSeePlayer)
            {
                _isPatrolling = false;
                _isWaitingAtWaypoint = false;
                if (_agent != null && _agent.isOnNavMesh && _agent.hasPath)
                {
                    _agent.ResetPath(); // Stop moving while watching player
                }
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
                GenerateNewPatrolWaypoint();
                _isPatrolling = true;
            }

            // Move toward patrol target via NavMeshAgent
            if (_agent != null && _agent.isOnNavMesh)
            {
                float patrolSpeed = _moveSpeed * 0.75f;
                _agent.speed = patrolSpeed;
                _agent.SetDestination(_currentPatrolTarget);

                // Rotate toward movement direction (velocity-based, not target-based)
                // updateRotation=false means we handle rotation manually
                if (_agent.velocity.sqrMagnitude > 0.01f)
                {
                    Vector3 moveDir = new Vector3(_agent.velocity.x, 0f, _agent.velocity.z).normalized;
                    if (moveDir != Vector3.zero)
                    {
                        transform.rotation = Quaternion.Slerp(
                            transform.rotation,
                            Quaternion.LookRotation(moveDir),
                            Time.deltaTime * 5f);
                    }
                }
            }

            // Check if reached waypoint (HasAgentArrived avoids remainingDistance=Infinity bug)
            if (HasAgentArrived())
            {
                _isWaitingAtWaypoint = true;
                _patrolWaitTimer = _patrolWaitTime;
                if (_agent != null && _agent.isOnNavMesh)
                {
                    _agent.ResetPath(); // Clear destination while waiting
                }
            }
        }

        /// <summary>
        /// Generates a new random patrol waypoint around start position.
        /// Validates point is on NavMesh using SamplePosition — prevents off-mesh destinations.
        /// Falls back to _originalPosition if no valid point found after 5 attempts.
        /// </summary>
        private void GenerateNewPatrolWaypoint()
        {
            const int maxAttempts = 5;
            const float sampleRadius = 1.0f; // Search radius around candidate point

            for (int i = 0; i < maxAttempts; i++)
            {
                float radius = Random.Range(_patrolRadiusMin, _patrolRadiusMax);
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

                Vector3 candidatePoint = _originalPosition + new Vector3(
                    Mathf.Cos(angle) * radius,
                    0f,
                    Mathf.Sin(angle) * radius);

                // Validate point is on NavMesh
                NavMeshHit hit;
                if (NavMesh.SamplePosition(candidatePoint, out hit, sampleRadius, NavMesh.AllAreas))
                {
                    _currentPatrolTarget = hit.position;
                    return;
                }
            }

            // Fallback: return to spawn position (always on NavMesh)
            _currentPatrolTarget = _originalPosition;
            Debug.LogWarning($"SnakeAI ({_snakeName}): No valid patrol waypoint found after {maxAttempts} attempts, using spawn position");
        }

        /// <summary>
        /// Checks if the NavMeshAgent has arrived at its destination.
        /// Simple remainingDistance check FAILS on multi-segment paths (returns Infinity).
        /// This 3-condition check is the Unity-recommended workaround.
        /// </summary>
        private bool HasAgentArrived()
        {
            if (_agent == null || !_agent.isOnNavMesh) return false;
            if (_agent.pathPending) return false;
            if (_agent.remainingDistance > _agent.stoppingDistance) return false;
            // Agent velocity still draining to zero — not fully stopped yet
            if (_agent.hasPath && _agent.velocity.sqrMagnitude > 0.01f) return false;
            return true;
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
                        // Only move if spell animation delay has passed (StartMoveAwayMovement sets _isMoving)
                        if (!_isMoving)
                        {
                            break;
                        }

                        // Check if NavMeshAgent has arrived at MoveAwayTarget
                        if (HasAgentArrived())
                        {
                            _isMoving = false;
                            TransitionFromMoveAwayToRootState();
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"SnakeAI ({_snakeName}): MovedAway state but no MoveAwayTarget! Returning to Idle.");
                        SetState(SnakeState.Idle);
                    }
                    break;

                case SnakeState.Entranced:
                    // Entranced phase — snake faces player, listens to melody
                    LookAtPlayer();
                    _stateTimer -= Time.deltaTime;
                    if (_stateTimer <= 0f)
                    {
                        Debug.Log($"SnakeAI ({_snakeName}): Entranced → Dazed transition");
                        SetState(SnakeState.Dazed); // Melody worked → snake falls asleep
                    }
                    break;

                case SnakeState.Dazed:
                    // Dazed with timer - snake lies on ground until timer expires
                    _stateTimer -= Time.deltaTime;
                    if (_stateTimer <= 0f)
                    {
                        SetState(SnakeState.Idle);
                    }
                    break;

                case SnakeState.Dead:
                    // Dead snakes do nothing
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
        /// Follow player smoothly via NavMeshAgent (used in 0.5-3.5 unit range).
        /// updateRotation=false means LookAtPlayer() still handles facing.
        /// </summary>
        private void FollowPlayer()
        {
            if (_playerTransform == null) return;
            if (_agent == null || !_agent.isOnNavMesh) return;

            _agent.speed = _chaseSpeed;
            _agent.SetDestination(_playerTransform.position);

            LookAtPlayer();
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
        /// Uses NavMeshAgent to navigate to _moveAwayTarget position.
        /// </summary>
        private void StartMoveAwayMovement()
        {
            if (_currentState != SnakeState.MovedAway) return;
            if (_moveAwayTarget == null) return;
            if (_agent == null || !_agent.isOnNavMesh) return;

            _isMoving = true;
            _agent.speed = _moveSpeed;
            _agent.SetDestination(_moveAwayTarget.position);
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
        }

        /// <summary>
        /// Transitions to a new state with appropriate setup.
        /// </summary>
        private void SetState(SnakeState newState)
        {
            SnakeState previousState = _currentState;
            _currentState = newState;

            // Cancel any pending Invoke calls from previous state
            // Prevents: MovedAway's StartMoveAwayMovement firing in a different state,
            // Attack VFX resets, or damage timers carrying over
            CancelInvoke();

            // NavMeshAgent state control (Phase 5)
            // isStopped=true preserves the current path (can resume with isStopped=false)
            // ResetPath() clears destination entirely (for states that don't resume movement)
            if (_agent != null && _agent.isOnNavMesh)
            {
                switch (newState)
                {
                    case SnakeState.Entranced:
                    case SnakeState.Dazed:
                    case SnakeState.Dead:
                        // These states never resume agent movement — clear path
                        _agent.isStopped = true;
                        _agent.ResetPath();
                        break;

                    case SnakeState.Idle:
                    case SnakeState.Aggressive:
                    case SnakeState.MovedAway:
                        // Mobile states — agent should be active
                        _agent.isStopped = false;
                        break;
                }
            }

            // Clear IsDazed bool + Reset attack cooldown when leaving Dazed state
            if (previousState == SnakeState.Dazed && newState != SnakeState.Dazed)
            {
                if (_animator != null)
                {
                    _animator.SetBool("IsDazed", false);
                }
                // Reset attack cooldown so snake can attack immediately after daze
                _lastAttackTime = 0f;
            }

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

                case SnakeState.Entranced:
                    SetVisualColor(_entrancedColor);
                    EnableCollider(true); // Still solid while listening — not yet neutralized
                    _stateTimer = _entrancedDuration > 0f ? _entrancedDuration : 3f; // Safety: fallback 3s
                    Debug.Log($"SnakeAI ({_snakeName}): Entering Entranced | Timer: {_stateTimer}s (config: {_entrancedDuration}s)");
                    // Snake faces player (handled in UpdateState via LookAtPlayer)
                    break;

                case SnakeState.Dazed:
                    SetVisualColor(_dazedColor);
                    EnableCollider(false); // GDD: collision disabled when dazed
                    _stateTimer = _dazedDuration; // Set timer for how long snake stays dazed
                    // Trigger Die animation + IsDazed bool keeps it there
                    if (_animator != null)
                    {
                        _animator.SetTrigger("Die");
                        _animator.SetBool("IsDazed", true);
                    }
                    Debug.Log($"SnakeAI ({_snakeName}): Entering Dazed | Timer: {_stateTimer}s | Animator: {(_animator != null ? "OK" : "NULL")}");
                    break;

                case SnakeState.Dead:
                    SetVisualColor(Color.gray); // Grayed out visual
                    EnableCollider(false); // No collision
                    // Trigger death animation + Set IsDazed=true to stay in Die animation
                    if (_animator != null)
                    {
                        _animator.SetTrigger("Die");
                        _animator.SetBool("IsDazed", true); // Prevents Die → Idle transition
                    }
                    break;
            }
        }
        #endregion

        #region Tune Interaction
        /// <summary>
        /// Called when a tune succeeds globally (with tune number).
        /// Only reacts if this snake is the closest targetable snake in range.
        /// TuneController no longer fires TuneSuccessWithId for tune 3 (Shield) — only 1 and 2.
        /// </summary>
        private void OnTuneSuccessWithId(int tuneNumber)
        {
            // Only react if player is in range and snake is targetable
            if (!IsPlayerInRange || !IsTargetable) return;

            // Check if this is the closest targetable snake
            if (!IsClosestTargetableSnake()) return;

            // Map tune number to effect — only Move and Daze are snake-targeting tunes
            SnakeEffect effect = tuneNumber switch
            {
                1 => SnakeEffect.Move,
                2 => SnakeEffect.Daze,
                _ => SnakeEffect.Move // Fallback (should not occur — Shield no longer fires this event)
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
        /// Fires GameEvents.SnakeCharmed() so HealthSystem can heal the player.
        /// </summary>
        public void ApplyTuneEffect(SnakeEffect effect)
        {
            switch (effect)
            {
                case SnakeEffect.Move:
                    SetState(SnakeState.MovedAway);
                    GameEvents.SnakeCharmed(1); // Heal triggers here — snake is actually charmed
                    break;

                case SnakeEffect.Daze:
                    SetState(SnakeState.Entranced); // Phase 1: listen → then Dazed
                    GameEvents.SnakeCharmed(2); // Heal triggers here — spell was successful
                    break;

                // Shield has no snake effect — SnakeAI never processes it
                // (TuneController no longer fires TuneSuccessWithId for tune 3)
            }
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

            // Spawn attack VFX
            SpawnAttackFX(attackType);
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
        /// Spawns attack VFX based on attack type.
        /// Uses _fxSpawnPoint if assigned, otherwise spawns at snake position.
        /// </summary>
        private void SpawnAttackFX(AttackType attackType)
        {
            GameObject fxPrefab = null;
            Vector3 spawnPosition = _fxSpawnPoint != null ? _fxSpawnPoint.position : transform.position;
            Quaternion spawnRotation = transform.rotation;

            switch (attackType)
            {
                case AttackType.Breath:
                    fxPrefab = _poisonBreathFXPrefab;
                    break;

                case AttackType.Projectile:
                    fxPrefab = _projectileFXPrefab;
                    // Projectile should face player
                    if (_playerTransform != null)
                    {
                        Vector3 directionToPlayer = (_playerTransform.position - spawnPosition).normalized;
                        spawnRotation = Quaternion.LookRotation(directionToPlayer);
                    }
                    break;

                case AttackType.Bite:
                    // Bite has no VFX (close-range physical attack)
                    return;
            }

            // Spawn FX if prefab is assigned
            if (fxPrefab != null)
            {
                GameObject fxInstance = Instantiate(fxPrefab, spawnPosition, spawnRotation);

                // Get ParticleSystem and play
                ParticleSystem ps = fxInstance.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Play();

                    // Auto-destroy after particle lifetime
                    float lifetime = ps.main.duration + ps.main.startLifetime.constantMax;
                    Destroy(fxInstance, lifetime);
                }
                else
                {
                    // Fallback: destroy after 3 seconds if no ParticleSystem found
                    Destroy(fxInstance, 3f);
                }
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
                    }
                }
            }
        }
        #endregion

        #region Visual Feedback
        /// <summary>
        /// Sets the snake's visual color for state feedback.
        /// Phase 1: Simple color change. Phase 3: Particles, glow, etc.
        /// URP Lit shader compatible (uses _BaseColor property).
        /// </summary>
        private void SetVisualColor(Color color)
        {
            if (_renderer != null)
            {
                // URP Lit shader uses "_BaseColor" property, not "color"
                if (_renderer.material.HasProperty("_BaseColor"))
                {
                    _renderer.material.SetColor("_BaseColor", color);
                }
                else
                {
                    // Fallback for other shaders
                    _renderer.material.color = color;
                }
            }
            else
            {
                Debug.LogWarning($"SnakeAI ({_snakeName}): Renderer not found, cannot change color!");
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
            if (Camera.main == null) return;

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
