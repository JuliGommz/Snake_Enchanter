# Phase 5: Movement Migration - Research

**Researched:** 2026-02-17
**Domain:** Unity NavMeshAgent movement control — replacing MoveTowardsSafe() with SetDestination(), velocity-based animation
**Confidence:** HIGH

---

## Summary

Phase 5 completes the NavMesh migration by activating the `_agent` field added in Phase 4. The core task is replacing three custom movement methods (`UpdatePatrol()`, `FollowPlayer()`, and `StartMoveAwayMovement()`) with `agent.SetDestination()`, enabling `updatePosition = true` so the agent now controls the transform, and switching animation triggers from the boolean flag `_isPatrolling` to `_agent.velocity.magnitude > 0.1f`.

The most critical technical finding is the **position snap issue**: when `updatePosition` changes from `false` (Phase 4) to `true` (Phase 5), Unity immediately moves the transform to match the agent's internal simulation position (`nextPosition`). Since Phase 4 kept these diverged, enabling `updatePosition = true` without syncing first will cause a one-frame teleport. The fix is one line: `_agent.nextPosition = transform.position` immediately before setting `_agent.updatePosition = true`. This must happen in Awake() when the agent is activated.

The second critical finding is **`remainingDistance` unreliability**. This property returns `Infinity` on any path with more than one segment (Unity bug status: postponed, unfixed). Using `remainingDistance < 0.2f` alone for arrival detection will fail. The correct arrival check requires combining three conditions: `!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance && !_agent.hasPath`.

The third critical finding about rotation: keep `updateRotation = false` in Phase 5 as well. NavMeshAgent with `updateRotation = true` fights with `LookAtPlayer()`. Since the snake project needs custom facing logic (look at player in Aggressive/Idle, face movement direction in Patrol), manual rotation control throughout all states is the correct design.

**Primary recommendation:** Activate agent by syncing `nextPosition`, enabling `updatePosition = true` while keeping `updateRotation = false`, then replace movement methods with `SetDestination()`, use the three-condition arrival check, and drive animation with `_agent.velocity.magnitude`.

---

## Standard Stack

### Core
| Component | Version | Purpose | Why Standard |
|-----------|---------|---------|--------------|
| `NavMeshAgent` (UnityEngine.AI) | Built-in, Unity 2022.3 LTS | Path calculation and position update | Already on prefabs from Phase 4; the only NavMesh movement component |
| `NavMesh.SamplePosition()` | Built-in, Unity 2022.3 LTS | Validate waypoints before SetDestination | Prevents "invalid destination" when random points land off NavMesh |

### Key APIs for Phase 5
| API | Return Type | Reliable? | Notes |
|-----|-------------|-----------|-------|
| `agent.SetDestination(Vector3)` | bool | HIGH | Returns false if agent not on NavMesh or position invalid |
| `agent.isStopped` | bool (read/write) | HIGH | Pauses movement; path is preserved |
| `agent.ResetPath()` | void | HIGH | Clears path and destination |
| `agent.velocity.magnitude` | float | HIGH (1-frame lag) | Actual velocity from crowd simulation |
| `agent.desiredVelocity` | Vector3 (read-only) | HIGH | Intended velocity including avoidance |
| `agent.remainingDistance` | float | LOW for multi-segment paths | Returns Infinity until last path segment |
| `agent.pathPending` | bool | HIGH | True while path calculation is in progress |
| `agent.hasPath` | bool | HIGH | True when a valid path is calculated |
| `agent.nextPosition` | Vector3 (read/write) | HIGH | Internal simulation position |
| `agent.speed` | float (read/write) | HIGH | Can be changed at runtime for different states |

### Alternatives Considered
| Standard Choice | Alternative | Why Not Used |
|-----------------|-------------|--------------|
| `updateRotation = false` + manual LookAt | `updateRotation = true` | Agent rotation fights LookAtPlayer(); manual control required for state-specific facing |
| `agent.velocity.magnitude` for animation | `_isPatrolling` boolean | The bug: boolean stays true when snake hits wall; velocity naturally goes 0 when stopped |
| Three-condition arrival check | `remainingDistance < 0.2f` alone | remainingDistance is Infinity on multi-segment paths; single check causes missed arrivals |

---

## Architecture Patterns

### Recommended Code Structure After Phase 5

```
SnakeAI.cs Awake():
├── (existing code — collider, renderer, animator caching)
├── (existing — MoveAwayTarget detach)
└── NavMesh activation block:
    ├── _agent.nextPosition = transform.position   ← SYNC FIRST
    ├── _agent.updatePosition = true               ← Agent now controls position
    ├── _agent.updateRotation = false              ← Keep manual rotation
    ├── _agent.speed = _moveSpeed * 0.75f          ← Patrol speed default
    └── _agent.isStopped = false                   ← Agent active

UpdatePatrol():
├── Replaced with: _agent.SetDestination(_currentPatrolTarget)
├── _agent.speed = _moveSpeed * 0.75f              ← Patrol is slower
├── Arrival check: HasAgentArrived()               ← Three-condition check
└── Waypoint rotation: manual (updateRotation=false)

FollowPlayer() / FollowPlayerForFailedTune():
├── Replaced with: _agent.SetDestination(_playerTransform.position)
└── _agent.speed = _chaseSpeed                     ← Chase speed

SetState():
├── isStopped = true  → Dazed, Frozen, Dead
└── isStopped = false → Idle, Aggressive, MovedAway, AttackingEnemy

UpdateMovementAnimation():
└── _agent.velocity.magnitude > 0.1f              ← Replace _isPatrolling bool
```

### Pattern 1: Agent Activation (Awake Swap)

**What:** Transitioning from Phase 4's passive agent to Phase 5's active agent.

**Critical step:** Sync `nextPosition` before enabling `updatePosition`. Without this, the agent teleports the snake to the navmesh-simulated position (which may have drifted from transform.position during Phase 4).

**Example:**
```csharp
// Source: Unity ScriptReference AI.NavMeshAgent-nextPosition
// https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-nextPosition.html
// Source: Unity ScriptReference AI.NavMeshAgent-updatePosition
// https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-updatePosition.html

_agent = GetComponent<NavMeshAgent>();
if (_agent != null)
{
    // CRITICAL: Sync simulation position to current transform BEFORE enabling updatePosition.
    // Without this, the agent teleports transform to its internal nextPosition (which
    // diverged during Phase 4 when updatePosition was false).
    _agent.nextPosition = transform.position;

    _agent.updatePosition = true;   // Agent now controls transform.position
    _agent.updateRotation = false;  // Keep manual rotation — LookAtPlayer() controls facing

    _agent.speed = _moveSpeed * 0.75f;  // Patrol speed; chase overrides in FollowPlayer()
    _agent.stoppingDistance = 0.2f;
    _agent.isStopped = false;
}
```

### Pattern 2: SetDestination for Patrol

**What:** Replace `MoveTowardsSafe(_currentPatrolTarget, speed)` and the manual rotation code in `UpdatePatrol()`.

**Key changes:**
- Validate waypoint with `NavMesh.SamplePosition()` before calling `SetDestination()`
- Arrival check uses the three-condition compound check, NOT just `remainingDistance`
- Agent handles pathfinding around obstacles; remove SphereCast collision code for patrol
- Manual rotation still required (`updateRotation = false`): face movement direction using `_agent.velocity`

**Example:**
```csharp
// Source: Unity ScriptReference AI.NavMeshAgent.SetDestination
// https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent.SetDestination.html
// Source: Unity ScriptReference AI.NavMesh.SamplePosition
// https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html

private void UpdatePatrol()
{
    if (_currentState != SnakeState.Idle) return;
    if (_canSeePlayer)
    {
        _isPatrolling = false;
        _agent.isStopped = true;
        return;
    }

    if (_isWaitingAtWaypoint)
    {
        _patrolWaitTimer -= Time.deltaTime;
        if (_patrolWaitTimer <= 0f)
        {
            _isWaitingAtWaypoint = false;
            SetNewPatrolDestination();
        }
        return;
    }

    if (!_isPatrolling)
    {
        SetNewPatrolDestination();
        _isPatrolling = true;
    }

    // Rotate toward movement direction (not target — agent handles pathfinding)
    if (_agent.velocity.sqrMagnitude > 0.01f)
    {
        Quaternion targetRot = Quaternion.LookRotation(_agent.velocity.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
    }

    // Arrival check
    if (HasAgentArrived())
    {
        _isWaitingAtWaypoint = true;
        _patrolWaitTimer = _patrolWaitTime;
        _isPatrolling = false;
        _agent.isStopped = true;
    }
}

private void SetNewPatrolDestination()
{
    float radius = Random.Range(_patrolRadiusMin, _patrolRadiusMax);
    float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
    Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
    Vector3 candidateTarget = _originalPosition + offset;

    // Validate on NavMesh before using
    // Max distance: 2 * agent height (0.5) = 1.0f recommended by Unity docs
    NavMeshHit hit;
    if (NavMesh.SamplePosition(candidateTarget, out hit, 1.0f, NavMesh.AllAreas))
    {
        _currentPatrolTarget = hit.position;
        _agent.isStopped = false;
        _agent.speed = _moveSpeed * 0.75f;
        _agent.SetDestination(_currentPatrolTarget);
    }
    else
    {
        // Waypoint off NavMesh — try again next frame or wait
        _isWaitingAtWaypoint = true;
        _patrolWaitTimer = 0.5f;
    }
}
```

### Pattern 3: SetDestination for Chase

**What:** Replace `MoveTowardsSafe(_playerTransform.position, _chaseSpeed)` in `FollowPlayer()`.

**Key changes:**
- Single line replaces the MoveTowardsSafe call
- Update `_agent.speed` to `_chaseSpeed` before calling SetDestination
- Call `SetDestination()` every frame for moving target (player) — this is correct and efficient
- Keep `LookAtPlayer()` call — manual rotation still needed

**Example:**
```csharp
// Source: Unity ScriptReference AI.NavMeshAgent.SetDestination
// https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent.SetDestination.html

private void FollowPlayer()
{
    if (_playerTransform == null) return;
    _agent.isStopped = false;
    _agent.speed = _chaseSpeed;
    _agent.SetDestination(_playerTransform.position);
    LookAtPlayer();  // Keep — agent does NOT control rotation (updateRotation=false)
}
```

### Pattern 4: Reliable Arrival Detection

**What:** Replace the simple `Vector3.Distance < 0.2f` check with a NavMeshAgent-aware arrival check.

**Why:** `remainingDistance` returns `Infinity` on any path with more than one segment (Unity bug, status: postponed). A combined check is required.

**Example:**
```csharp
// Source: Community consensus verified by Unity docs
// https://answers.unity.com/questions/324589/how-can-i-tell-when-a-navmesh-has-reached-its-dest.html
// https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-pathPending.html
// https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-hasPath.html

private bool HasAgentArrived()
{
    if (_agent == null || !_agent.isOnNavMesh) return false;

    // pathPending: still calculating — not arrived yet
    if (_agent.pathPending) return false;

    // remainingDistance is Infinity while multi-segment path, valid when near destination
    // stoppingDistance (0.2f) is the threshold
    if (_agent.remainingDistance > _agent.stoppingDistance) return false;

    // hasPath=false OR velocity near zero confirms fully stopped
    if (_agent.hasPath && _agent.velocity.sqrMagnitude > 0.01f) return false;

    return true;
}
```

### Pattern 5: Velocity-Based Animation

**What:** Replace `_isPatrolling` boolean with `_agent.velocity.magnitude` threshold check.

**Why this fixes the bug:** The current bug — snake animation resets to frame 0 when blocked by a wall — occurs because `_isPatrolling` stays `true` even when `MoveTowardsSafe()` returns `false` (blocked). With NavMesh movement, the agent stops generating velocity when blocked. `_agent.velocity.magnitude` naturally drops to near-zero when stopped.

**Note:** `_agent.velocity` is the actual velocity from crowd simulation (one-frame lag). `_agent.desiredVelocity` is the intended velocity. For animation triggering, use `velocity` (what the snake actually does) not `desiredVelocity` (what it wants to do).

**Example:**
```csharp
// Source: Unity AI Navigation docs — Coupling Animation and Navigation
// https://docs.unity3d.com/Packages/com.unity.ai.navigation@1.1/manual/CouplingAnimationAndNavigation.html
// Source: Unity ScriptReference AI.NavMeshAgent-velocity
// https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-velocity.html

private void UpdateMovementAnimation()
{
    if (_animator == null) return;

    // velocity.magnitude: actual speed from crowd simulation
    // Threshold 0.1f: filters out near-zero drift when stopping
    bool isActuallyMoving = _agent != null && _agent.velocity.magnitude > 0.1f;

    // Reset all slither bools
    _animator.SetBool("Slither Forward", false);
    _animator.SetBool("Slither Left", false);
    _animator.SetBool("Slither Right", false);

    if (!isActuallyMoving) return;

    // Directional slither: convert agent world velocity to local space
    // Agent handles pathfinding, so velocity direction = actual movement direction
    Vector3 localVelocity = transform.InverseTransformDirection(_agent.velocity);
    float forwardAmount = localVelocity.z;
    float rightAmount = localVelocity.x;

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
```

### Pattern 6: State Machine isStopped Control

**What:** Replace manual `_isMoving` tracking with `agent.isStopped` in `SetState()`.

**isStopped vs ResetPath:**
- `isStopped = true`: Pauses movement. **Path is preserved.** `isStopped = false` resumes same path.
- `ResetPath()`: **Clears the path.** Agent has no destination until `SetDestination()` is called again.

**Rule for this project:** Use `isStopped = true` for temporary pauses (Frozen, between waypoints). Use `ResetPath()` when the snake definitively stops moving (Dead, entering Dazed).

**Example:**
```csharp
// Source: Unity ScriptReference AI.NavMeshAgent-isStopped
// https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-isStopped.html
// Source: Unity ScriptReference AI.NavMeshAgent.ResetPath
// https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent.ResetPath.html

private void SetState(SnakeState newState)
{
    // ... (existing IsDazed bool handling)

    switch (newState)
    {
        case SnakeState.Idle:
            if (_agent != null) _agent.isStopped = false;  // Will patrol
            // ... existing visual/collider code
            break;

        case SnakeState.Aggressive:
            if (_agent != null) _agent.isStopped = false;  // Will chase
            // ...
            break;

        case SnakeState.MovedAway:
            if (_agent != null) _agent.isStopped = false;  // Will move to target
            // ...
            break;

        case SnakeState.Dazed:
            if (_agent != null) { _agent.isStopped = true; _agent.ResetPath(); }
            // ...
            break;

        case SnakeState.AttackingEnemy:
            if (_agent != null) { _agent.isStopped = true; _agent.ResetPath(); }
            // ...
            break;

        case SnakeState.Frozen:
            if (_agent != null) _agent.isStopped = true;   // Preserve path for resume
            // ...
            break;

        case SnakeState.Dead:
            if (_agent != null) { _agent.isStopped = true; _agent.ResetPath(); }
            // ...
            break;
    }
}
```

### Pattern 7: MovedAway State with NavMesh

**What:** The MovedAway state needs to reach `_moveAwayTarget.position` via NavMesh.

**How it works:**
- Call `_agent.SetDestination(_moveAwayTarget.position)` when movement starts (after spell animation delay)
- Use `HasAgentArrived()` for arrival detection — no more SphereCast timeout logic needed
- No manual position interpolation needed — agent handles obstacle navigation

**Example:**
```csharp
private void StartMoveAwayMovement()
{
    if (_currentState == SnakeState.MovedAway && _agent != null && _moveAwayTarget != null)
    {
        _isMoving = true;
        _agent.isStopped = false;
        _agent.speed = _moveSpeed;
        _agent.SetDestination(_moveAwayTarget.position);
    }
}

// In UpdateState() case MovedAway:
if (_isMoving && HasAgentArrived())
{
    _isMoving = false;
    _agent.isStopped = true;
    TransitionFromMoveAwayToRootState();
}
```

### Anti-Patterns to Avoid

- **Enabling `updatePosition = true` without syncing `nextPosition` first:** Causes one-frame teleport of the snake to the agent's internal simulation position. Fix: `_agent.nextPosition = transform.position` before enabling.
- **Using `remainingDistance < 0.2f` alone for arrival:** Returns Infinity on multi-segment paths — snake never detects arrival. Fix: use the three-condition `HasAgentArrived()` check.
- **Setting `updateRotation = true`:** Conflicts with `LookAtPlayer()`. The agent would override rotation to face the path direction, not the player. Keep `false`.
- **Calling `SetDestination()` while `isStopped = true`:** The agent won't move. Always set `isStopped = false` before or with `SetDestination()`.
- **Keeping `MoveTowardsSafe()` active alongside `updatePosition = true`:** Two systems writing to `transform.position` simultaneously causes jitter. Remove `MoveTowardsSafe()` calls in the same update as the agent is active.
- **Setting `agent.speed` once and forgetting:** Patrol uses `_moveSpeed * 0.75f`, chase uses `_chaseSpeed`. Must update `_agent.speed` when switching between these behaviors.
- **Not updating `SetDestination()` each frame for player chase:** For a moving target (player), `SetDestination()` must be called each frame. This is efficient — the agent recalculates only the changed portion of the path.

---

## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| Obstacle avoidance during patrol | SphereCast + MoveTowardsSafe() | `agent.SetDestination()` | NavMesh handles walls, corners, props via pre-baked graph — no runtime collision needed |
| Path calculation | Custom waypoint graph | NavMesh + SetDestination | NavMesh A* pathfinding handles all topology; custom graph can't handle dynamic obstacles |
| Arrival detection | `Vector3.Distance < threshold` alone | Three-condition HasAgentArrived() | Simple distance check misses pathPending state; combined check handles all timing cases |
| Waypoint validation | Try-catch around SetDestination | `NavMesh.SamplePosition()` before calling | SetDestination silently fails off-NavMesh; SamplePosition validates and snaps to nearest valid point |
| Speed-based animation | `_isPatrolling` bool flag | `_agent.velocity.magnitude > threshold` | Bool misses the "blocked by wall" case; velocity naturally drops when movement is blocked |
| Directional animation vectors | Storing `_lastMoveDirection` from MoveTowardsSafe | `transform.InverseTransformDirection(_agent.velocity)` | Agent velocity is already the correct world-space movement vector; no manual tracking needed |

**Key insight:** The entire custom obstacle avoidance system (MoveTowardsSafe, SphereCast, blocked timeout, MoveAwayTarget tag detection) was a workaround for not having NavMesh. NavMesh renders all of it unnecessary. Phase 5 removes complexity, not adds it.

---

## Common Pitfalls

### Pitfall 1: Position Snap on updatePosition Activation (CRITICAL)

**What goes wrong:** Snake teleports a short distance when Phase 5 first enables `updatePosition = true`.
**Why it happens:** During Phase 4, `updatePosition = false` allowed `transform.position` and the agent's internal `nextPosition` to drift apart. When `updatePosition = true` is set, Unity immediately writes `nextPosition` to `transform.position`. If they diverged, this is a visible teleport.
**How to avoid:** In Awake(), immediately before setting `updatePosition = true`, write: `_agent.nextPosition = transform.position`. This syncs the simulation to the current transform, making the switch seamless.
**Warning signs:** Snake jumps a short distance at scene start. Happens once, then normal movement begins.

### Pitfall 2: remainingDistance Returns Infinity (CRITICAL)

**What goes wrong:** `HasAgentArrived()` check based on `remainingDistance` never triggers. Snake arrives at waypoint but patrol loop doesn't advance.
**Why it happens:** Unity NavMeshAgent `remainingDistance` returns `Infinity` for any path that has more than one segment (multi-corner paths). This is an unfixed Unity bug (issue tracker status: Postponed).
**How to avoid:** Never use `remainingDistance` alone. Use the three-condition check: `!pathPending && remainingDistance <= stoppingDistance && (!hasPath || velocity.sqrMagnitude < 0.01f)`.
**Warning signs:** Snake reaches waypoint location but stands there without transitioning. `Debug.Log(_agent.remainingDistance)` shows `Infinity`.

### Pitfall 3: SetDestination Called While isStopped=true

**What goes wrong:** Agent has a valid destination but the snake never moves.
**Why it happens:** `isStopped = true` prevents the agent from following any path, including newly set destinations.
**How to avoid:** Always `_agent.isStopped = false` before or simultaneously with `_agent.SetDestination()`. In SetState(), set `isStopped = false` for active states first, then in the movement methods call SetDestination.
**Warning signs:** `_agent.pathPending` briefly shows true then false, but snake doesn't move. `_agent.isStopped` is true in debug log.

### Pitfall 4: Agent Speed Not Updated Between States

**What goes wrong:** Snake chases player at patrol speed (too slow) or patrols at chase speed (too fast).
**Why it happens:** `_agent.speed` is a single value. The project has two speeds: `_moveSpeed * 0.75f` (patrol) and `_chaseSpeed` (chase). If only set in Awake(), all movement uses the same speed.
**How to avoid:** Set `_agent.speed` at the point of `SetDestination()` call: patrol code sets patrol speed, chase code sets chase speed.
**Warning signs:** Snake patrol visually looks the same speed as aggressive chase. Inspector shows agent moving at wrong speed.

### Pitfall 5: Rotation Conflict When updateRotation=true

**What goes wrong:** Snake faces a wrong direction — the navmesh path direction — instead of facing the player during Aggressive state.
**Why it happens:** `updateRotation = true` makes the agent override `transform.rotation` to face the calculated path direction. This conflicts with `LookAtPlayer()` which also writes to `transform.rotation`.
**How to avoid:** Keep `updateRotation = false` (already set in Phase 4). Drive rotation manually using `_agent.velocity` for patrol direction and `LookAtPlayer()` for player-facing states.
**Warning signs:** Snake faces away from player while chasing. Rotation snaps between two directions.

### Pitfall 6: MoveTowardsSafe Not Removed (Dual Write)

**What goes wrong:** Snake jitters or moves erratically. Two systems write `transform.position` simultaneously.
**Why it happens:** If `MoveTowardsSafe()` is still called AND `updatePosition = true`, both the agent and the method write to `transform.position` in the same frame.
**How to avoid:** Remove ALL calls to `MoveTowardsSafe()` in the same update cycle where the agent is active. This means: `UpdatePatrol()` removes its `MoveTowardsSafe()` call, `FollowPlayer()` removes its call, `UpdateState()` case MovedAway removes its call.
**Warning signs:** Snake oscillates between two positions. Debug shows position changing by non-agent amounts.

### Pitfall 7: Waypoint Lands Off NavMesh

**What goes wrong:** `SetDestination()` returns `false` silently. Snake stands still for patrol wait time, then generates another invalid waypoint.
**Why it happens:** Random waypoint generation uses `_originalPosition + offset` without validating the resulting point is on the NavMesh. Cave corners, walls, and raised terrain can produce off-NavMesh points.
**How to avoid:** Wrap waypoint generation in `NavMesh.SamplePosition()`. If it returns false, fall back to a short wait and try again.
**Warning signs:** Snake doesn't patrol despite Idle state. `_agent.SetDestination()` returns false (check with debug log).

---

## Code Examples

Verified patterns from official sources:

### Complete Awake() Activation Block
```csharp
// Source: https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-nextPosition.html
// Source: https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-updatePosition.html

_agent = GetComponent<NavMeshAgent>();
if (_agent != null)
{
    // Phase 5: Sync internal simulation to current transform BEFORE enabling position control.
    // Prevents teleport snap (nextPosition drifted from transform.position during Phase 4).
    _agent.nextPosition = transform.position;

    _agent.updatePosition = true;   // Agent now drives transform.position
    _agent.updateRotation = false;  // Manual rotation — LookAtPlayer() controls facing
    _agent.speed = _moveSpeed * 0.75f;  // Default patrol speed
    _agent.stoppingDistance = 0.2f;
    _agent.isStopped = false;
}
```

### NavMesh.SamplePosition Waypoint Validation
```csharp
// Source: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
// maxDistance = twice agent height (0.5 * 2 = 1.0f) per Unity docs recommendation

NavMeshHit hit;
if (NavMesh.SamplePosition(candidatePoint, out hit, 1.0f, NavMesh.AllAreas))
{
    // hit.position is the nearest valid NavMesh point
    _agent.SetDestination(hit.position);
}
```

### Three-Condition Arrival Check
```csharp
// Source: https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-pathPending.html
// Source: https://answers.unity.com/questions/324589/how-can-i-tell-when-a-navmesh-has-reached-its-dest.html

private bool HasAgentArrived()
{
    if (_agent == null || !_agent.isOnNavMesh) return false;
    if (_agent.pathPending) return false;
    if (_agent.remainingDistance > _agent.stoppingDistance) return false;
    if (_agent.hasPath && _agent.velocity.sqrMagnitude > 0.01f) return false;
    return true;
}
```

### Velocity-Based Animation Directional Slither
```csharp
// Source: https://docs.unity3d.com/Packages/com.unity.ai.navigation@1.1/manual/CouplingAnimationAndNavigation.html
// Source: https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-velocity.html

bool isActuallyMoving = _agent != null && _agent.velocity.magnitude > 0.1f;

_animator.SetBool("Slither Forward", false);
_animator.SetBool("Slither Left", false);
_animator.SetBool("Slither Right", false);

if (isActuallyMoving)
{
    Vector3 localVelocity = transform.InverseTransformDirection(_agent.velocity);
    float f = localVelocity.z;
    float r = localVelocity.x;

    if (Mathf.Abs(f) >= Mathf.Abs(r))
        _animator.SetBool("Slither Forward", true);
    else if (r > 0.1f)
        _animator.SetBool("Slither Right", true);
    else
        _animator.SetBool("Slither Left", true);
}
```

### isStopped vs ResetPath — When to Use Each
```csharp
// isStopped=true: pause movement, keep path (use for Frozen — can resume)
_agent.isStopped = true;

// isStopped=false: resume on same path (use when thawing from Frozen)
_agent.isStopped = false;

// ResetPath(): clear destination entirely (use for Dead, Dazed)
_agent.isStopped = true;
_agent.ResetPath();
// After this, hasPath=false, agent won't move until SetDestination called again
```

---

## State of the Art

| Old Approach (Phase 4 and before) | New Approach (Phase 5) | Impact |
|-----------------------------------|------------------------|--------|
| `MoveTowardsSafe()` + SphereCast collision | `agent.SetDestination()` + NavMesh pathfinding | Eliminates wall-collision bugs; no custom collision code needed |
| `_isPatrolling` bool for animation trigger | `_agent.velocity.magnitude > 0.1f` | Fixes animation reset bug — velocity drops when actually stopped |
| `Vector3.Distance` arrival check | Three-condition `HasAgentArrived()` | Reliable on all path geometries, not just straight-line paths |
| Random point + MoveTowardsSafe | `NavMesh.SamplePosition()` + `SetDestination()` | Validates waypoint on NavMesh before use |
| `_lastMoveDirection` tracking | `transform.InverseTransformDirection(_agent.velocity)` | Removes manual tracking; agent velocity is already correct |
| Blocked timeout (2s) in MovedAway | NavMesh path to MoveAwayTarget | Agent finds route around obstacles; no timeout needed |
| `updatePosition = false` (Phase 4) | `updatePosition = true` (Phase 5) | Agent now owns position control |

**Removed entirely in Phase 5:**
- `MoveTowardsSafe()` method — replaced by NavMesh pathfinding
- `_lastMoveDirection` Vector3 field — replaced by `_agent.velocity`
- `_blockedTimer` logic in MovedAway — NavMesh handles obstacle routing
- SphereCast collision detection for movement — NavMesh handles this

---

## Open Questions

1. **Agent speed for MoveAwayTarget (MovedAway state)**
   - What we know: `_moveSpeed` is the field used in the old MoveTowardsSafe for MovedAway
   - What's unclear: Should MovedAway use `_moveSpeed` or a different speed? Current code uses `_moveSpeed` without the 0.75f patrol reduction
   - Recommendation: Use `_moveSpeed` (no reduction) for MovedAway — snake charmed by player should move at full speed to clear path quickly

2. **Chase SetDestination frequency for player**
   - What we know: Calling SetDestination every frame for a moving target is the standard Unity pattern
   - What's unclear: Whether this causes path recalculation overhead for 6 snakes
   - Recommendation: Use it every frame — NavMeshAgent caches partial paths and only recalculates when player position changes significantly

3. **_spawnPosition field name discrepancy**
   - What we know: Phase 5 description references `_spawnPosition` but SnakeAI.cs uses `_originalPosition`
   - What's confirmed: Field is `_originalPosition` (verified in SnakeAI.cs Awake())
   - Recommendation: Use `_originalPosition` in all Phase 5 code — do NOT create a new `_spawnPosition` field

4. **_patrolRadius field name discrepancy**
   - What we know: Phase 5 description references `_patrolRadius` but SnakeAI.cs has `_patrolRadiusMin` and `_patrolRadiusMax`
   - Recommendation: Use `Random.Range(_patrolRadiusMin, _patrolRadiusMax)` for waypoint generation

5. **HandleIdlePlayerInteraction() FollowPlayer calls**
   - What we know: This method calls `FollowPlayer()` for distance ranges 0.5-3.5 and 3.5-4 and 7-8 (gap ranges)
   - What's unchanged: These calls become `agent.SetDestination(_playerTransform.position)` internally — the existing structure stays
   - Recommendation: No structural change to HandleIdlePlayerInteraction(); only FollowPlayer() internals change

---

## Sources

### Primary (HIGH confidence)
- Unity ScriptReference `AI.NavMeshAgent.SetDestination` — https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent.SetDestination.html — returns bool, fails if not on NavMesh
- Unity ScriptReference `AI.NavMeshAgent-updatePosition` — https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-updatePosition.html — CRITICAL: when re-enabled, transform moves to nextPosition immediately
- Unity ScriptReference `AI.NavMeshAgent-nextPosition` — https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-nextPosition.html — set before enabling updatePosition to prevent teleport
- Unity ScriptReference `AI.NavMeshAgent-velocity` — https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-velocity.html — actual velocity from crowd simulation, one-frame lag
- Unity ScriptReference `AI.NavMeshAgent-desiredVelocity` — https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-desiredVelocity.html — read-only intended velocity including avoidance
- Unity ScriptReference `AI.NavMeshAgent-remainingDistance` — https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-remainingDistance.html — Infinity when path not calculated
- Unity ScriptReference `AI.NavMeshAgent-pathPending` — https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-pathPending.html — true while path calculation in progress
- Unity ScriptReference `AI.NavMeshAgent-hasPath` — https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-hasPath.html — true when valid path calculated
- Unity ScriptReference `AI.NavMeshAgent-isStopped` — https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-isStopped.html — pauses movement, preserves path
- Unity ScriptReference `AI.NavMeshAgent.ResetPath` — https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent.ResetPath.html — clears path entirely
- Unity ScriptReference `AI.NavMesh.SamplePosition` — https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html — waypoint validation; maxDistance = 2x agent height
- Unity AI Navigation 1.1 Manual — Coupling Animation and Navigation — https://docs.unity3d.com/Packages/com.unity.ai.navigation@1.1/manual/CouplingAnimationAndNavigation.html — official pattern for velocity-driven animation
- SnakeAI.cs v1.8.0 — verified field names: `_originalPosition`, `_patrolRadiusMin`, `_patrolRadiusMax`, `_moveSpeed`, `_chaseSpeed`, `_currentPatrolTarget`, `_playerTransform`, `_agent`

### Secondary (MEDIUM confidence)
- Unity Issue Tracker — remainingDistance Infinity bug (status: Postponed): https://issuetracker.unity3d.com/issues/the-remaining-distance-of-the-nav-mesh-agent-is-equal-to-infinity-even-though-the-agent-is-moving-along-the-path — confirms bug exists, unfixed
- Unity Discussions — reliable arrival detection: https://answers.unity.com/questions/324589/how-can-i-tell-when-a-navmesh-has-reached-its-dest.html — three-condition check is community consensus, verified against official docs
- copyprogramming.com — remainingDistance Infinity analysis: https://copyprogramming.com/howto/why-does-navmeshagent-remainingdistance-return-values-of-infinity-and-then-a-float-in-unity-for-unreachable-destinations — confirms Unity 2019.3+ behavior: Infinity until last segment

### Tertiary (LOW confidence)
- None required — all critical patterns verified against Unity official documentation

---

## Metadata

**Confidence breakdown:**
- SetDestination/isStopped/ResetPath API: HIGH — verified from Unity ScriptReference
- Position snap fix (nextPosition sync): HIGH — directly from Unity ScriptReference for `nextPosition` and `updatePosition`
- remainingDistance Infinity: HIGH — confirmed in Unity ScriptReference AND Unity Issue Tracker (status: Postponed)
- Three-condition arrival check: MEDIUM — community consensus pattern; individual property docs support each condition
- Velocity-based animation pattern: HIGH — from official Unity AI Navigation manual (CouplingAnimationAndNavigation)
- Field names from SnakeAI.cs: HIGH — read directly from v1.8.0 source file

**Research date:** 2026-02-17
**Valid until:** 2026-03-17 (NavMeshAgent API is stable in Unity 2022 LTS; remainingDistance bug unlikely to be fixed in this timeframe)

---

## Project-Specific Implementation Notes

### SnakeAI.cs Field Names (Verified from v1.8.0)

| Phase 5 Description References | Actual Field in Code | Notes |
|--------------------------------|----------------------|-------|
| `_spawnPosition` | `_originalPosition` | Set in Awake() as `transform.position` |
| `_patrolRadius` | `_patrolRadiusMin`, `_patrolRadiusMax` | Two separate SerializeField floats |
| `patrolSpeed` | `_moveSpeed * 0.75f` | Computed inline, not a separate field |
| `_agent.velocity.magnitude > 0.1f` | Replaces `_isPatrolling` in UpdateMovementAnimation() | Root cause of the animation bug |

### Methods to Remove Entirely
- `MoveTowardsSafe()` — entire method can be removed; NavMesh handles all collision avoidance
- `_lastMoveDirection` field — replace with `_agent.velocity` inline

### Methods to Update (Not Replace)
- `UpdatePatrol()` — change destination setting to agent.SetDestination, arrival check to HasAgentArrived()
- `FollowPlayer()` — change MoveTowardsSafe call to agent.SetDestination
- `UpdateMovementAnimation()` — change `_isPatrolling` to `_agent.velocity.magnitude`
- `SetState()` — add agent.isStopped control per state
- `StartMoveAwayMovement()` — change to agent.SetDestination(_moveAwayTarget.position)
- `Awake()` — change `updatePosition = false` to `true`, add `nextPosition` sync

### Methods Unchanged
- `LookAtPlayer()` — unchanged; still needed since updateRotation=false
- `UpdatePatrol()` rotation code — change to use `_agent.velocity` direction instead of target direction
- `GenerateNewPatrolWaypoint()` — wrap with NavMesh.SamplePosition validation, otherwise same
- `HandleIdlePlayerInteraction()` — unchanged structurally; FollowPlayer() internals change
- `UpdateProximityDetection()` — unchanged; raycast-based detection still correct
- All spell reaction methods — unchanged; SetState() handles agent control

### Agent Speed Assignments
| State / Action | Speed to Set |
|----------------|-------------|
| Patrol (UpdatePatrol) | `_moveSpeed * 0.75f` |
| Chase player (FollowPlayer, FollowPlayerForFailedTune) | `_chaseSpeed` |
| Move away (StartMoveAwayMovement) | `_moveSpeed` |
| AttackingEnemy approach (if added) | `_chaseSpeed` |
