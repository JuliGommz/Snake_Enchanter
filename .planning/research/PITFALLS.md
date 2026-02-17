# Pitfalls Research - NavMesh Migration

**Domain:** Unity NavMesh integration into existing AI system (brownfield migration)
**Researched:** 2026-02-16
**Confidence:** HIGH

## Critical Pitfalls

### Pitfall 1: Transform Position Ownership Conflict

**What goes wrong:**
NavMeshAgent and custom movement scripts fight for control of `transform.position`, causing jittering, teleporting, or one system completely overriding the other. Your `MoveTowardsSafe()` method directly modifies `transform.position` while NavMeshAgent also wants to control position.

**Why it happens:**
NavMeshAgent automatically updates `transform.position` every frame by default (`updatePosition = true`). When your custom script ALSO writes to `transform.position`, you create a race condition where both systems try to control the same property.

**How to avoid:**
**Option A (Recommended for your project):** Replace ALL custom movement logic with NavMeshAgent
- Remove `MoveTowardsSafe()` calls
- Replace with `agent.SetDestination(target)`
- Let NavMeshAgent handle all pathfinding and movement

**Option B (Hybrid approach):** Disable NavMeshAgent's position control
```csharp
// In Start()
_navMeshAgent.updatePosition = false;
_navMeshAgent.updateRotation = false;

// Use NavMesh for pathfinding ONLY, custom movement for execution
Vector3 direction = (_navMeshAgent.nextPosition - transform.position).normalized;
MoveTowardsSafe(_navMeshAgent.nextPosition, _moveSpeed);
```

**Warning signs:**
- Snake teleports instead of smoothly moving
- Snake "vibrates" or jitters in place
- Position updates don't take effect
- Snake moves at wrong speed (NavMeshAgent speed != your _moveSpeed)

**Phase to address:**
**Immediate** - First commit of NavMesh integration. This breaks EVERYTHING if not handled correctly.

---

### Pitfall 2: State Machine Freeze Control Conflict

**What goes wrong:**
Your existing state machine uses timers and state transitions to control when snakes should/shouldn't move. NavMeshAgent doesn't know about your states (Dazed, Frozen, Dead) and keeps pathfinding/moving during states that should be immobile.

**Why it happens:**
NavMeshAgent runs independently of your state machine. Setting `_currentState = SnakeState.Frozen` doesn't automatically tell the agent to stop. You have multiple states that disable movement:
- `Dazed` (8s timer, should be immobile)
- `Frozen` (4s timer, should be immobile)
- `Dead` (permanent, should be immobile)
- `MovedAway` (waiting for spell animation delay)

**How to avoid:**
Use `agent.isStopped` to sync NavMeshAgent with state machine:

```csharp
private void SetState(SnakeState newState)
{
    _currentState = newState;

    // Sync NavMeshAgent with state
    if (_navMeshAgent != null)
    {
        bool shouldStop = (newState == SnakeState.Dazed) ||
                         (newState == SnakeState.Frozen) ||
                         (newState == SnakeState.Dead) ||
                         (newState == SnakeState.MovedAway && !_isMoving);

        _navMeshAgent.isStopped = shouldStop;
    }

    // Existing state transition code...
}
```

**CRITICAL:** Do NOT use deprecated `agent.Stop()` / `agent.Resume()` - use `isStopped` property instead.

**Warning signs:**
- Frozen snakes keep moving
- Dazed snakes chase player
- Dead snakes pathfind to player
- Spell animation delay doesn't work (snake moves immediately)

**Phase to address:**
**Immediate** - Same commit as NavMeshAgent addition. Must be in place BEFORE testing any spell behaviors.

---

### Pitfall 3: Collision Detection System Becomes Redundant/Broken

**What goes wrong:**
Your custom `MoveTowardsSafe()` uses raycasts to detect obstacles before moving. NavMeshAgent has its own obstacle avoidance system that operates completely differently. You end up with:
1. Redundant collision checks (performance waste)
2. Conflicts between systems (NavMesh avoids obstacle, but raycast says blocked)
3. Your carefully tuned collision logic (8 fix attempts!) becomes useless

**Why it happens:**
NavMeshAgent uses the NavMesh surface for pathfinding - it already knows where obstacles are because they're baked into the NavMesh. Your raycasts are solving a problem that NavMesh solves differently.

**How to avoid:**
**Step 1:** Trust NavMesh obstacle avoidance, remove custom collision detection:
```csharp
// DELETE this method entirely
private bool MoveTowardsSafe(Vector3 target, float speed) { ... }

// REPLACE with
agent.SetDestination(target);
```

**Step 2:** Configure NavMeshAgent obstacle avoidance:
```csharp
// Inspector settings
agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
agent.avoidancePriority = 50; // 0-99, lower = higher priority
agent.radius = 0.5f; // Match your snake's actual size
```

**Step 3:** For dynamic obstacles (other snakes), ensure prefabs have NavMeshObstacle component when NOT using NavMeshAgent:
- **Dazed/Frozen/Dead snakes:** Should have NavMeshObstacle enabled (other snakes avoid them)
- **Active snakes:** NavMeshAgent handles mutual avoidance

**EXCEPTION:** Keep raycasts ONLY for spell targeting (`_canSeePlayer` line-of-sight checks).

**Warning signs:**
- Snakes overlap/stack (NavMesh avoidance not configured)
- Snakes get stuck at walls (NavMesh not baked correctly around environment)
- Performance degradation (both systems running collision checks)
- MoveAwayTarget timeout logic triggers (shouldn't happen with NavMesh)

**Phase to address:**
**First NavMesh commit** - Remove `MoveTowardsSafe()` entirely. Phase out collision logic incrementally if concerned, but keeping both systems is a recipe for bugs.

---

### Pitfall 4: Animator State Conflicts (Root Motion vs NavMeshAgent)

**What goes wrong:**
Your snakes use directional slither animations (Forward/Left/Right bools). When you add NavMeshAgent, you risk:
1. **Root motion conflict:** If animations have root motion enabled, both Animator AND NavMeshAgent try to move the snake
2. **Animation desync:** NavMeshAgent moves snake at `agent.speed`, animations play at fixed speed → foot sliding
3. **Direction mismatch:** NavMeshAgent's path direction doesn't match your `_lastMoveDirection` tracking

**Why it happens:**
You've carefully tracked movement direction for directional animations:
```csharp
Vector3 localDirection = transform.InverseTransformDirection(movementVector.normalized);
```

NavMeshAgent's velocity won't automatically populate `_lastMoveDirection`. Animations become disconnected from actual movement.

**How to avoid:**
**Step 1:** Ensure root motion is DISABLED (you already have this, but verify):
```csharp
// In your animator setup
_animator.applyRootMotion = false; // CRITICAL
```

**Step 2:** Drive animations from NavMeshAgent velocity:
```csharp
private void UpdateMovementAnimation()
{
    if (_animator == null || _navMeshAgent == null) return;

    // Use NavMeshAgent velocity instead of _lastMoveDirection
    Vector3 velocity = _navMeshAgent.velocity;
    bool isMoving = velocity.sqrMagnitude > 0.1f;

    _animator.SetBool("Slither Forward", false);
    _animator.SetBool("Slither Left", false);
    _animator.SetBool("Slither Right", false);

    if (!isMoving) return;

    // Convert velocity to local space (same logic as before)
    Vector3 localDirection = transform.InverseTransformDirection(velocity.normalized);

    float forwardAmount = localDirection.z;
    float rightAmount = localDirection.x;

    // Same directional logic...
}
```

**Step 3:** Match animation speed to agent speed (advanced):
```csharp
// Optional: Scale animation speed based on agent velocity
float speedRatio = _navMeshAgent.velocity.magnitude / _navMeshAgent.speed;
_animator.SetFloat("SpeedMultiplier", speedRatio);
```

**Warning signs:**
- Foot sliding (animations don't match movement speed)
- Snakes move backward/sideways while playing forward animation
- Animations don't play at all during NavMesh movement
- Snake "ice skating" (smooth gliding without leg movement)

**Phase to address:**
**Second commit** - After basic NavMesh movement works, fix animation sync separately. Can be deferred to polish phase if time-constrained.

---

### Pitfall 5: MoveAwayTarget Destination Validation Missing

**What goes wrong:**
`agent.SetDestination(target)` can fail silently if the target position isn't on the NavMesh. Your MoveAwayTarget GameObjects might be placed in locations that aren't part of the baked NavMesh surface. Snake gets stuck, `agent.pathStatus == NavMeshPathStatus.PathInvalid`, but no error is shown.

**Why it happens:**
NavMesh only covers walkable areas. If your MoveAwayTarget is:
- Outside the NavMesh bounds
- On a disconnected NavMesh island
- Below/above the NavMesh surface tolerance
...the agent can't pathfind there.

**How to avoid:**
**Step 1:** Validate destinations before setting:
```csharp
private bool TrySetDestination(Vector3 target)
{
    NavMeshHit hit;
    if (NavMesh.SamplePosition(target, out hit, 2.0f, NavMesh.AllAreas))
    {
        _navMeshAgent.SetDestination(hit.position);
        return true;
    }
    else
    {
        Debug.LogWarning($"SnakeAI ({_snakeName}): Target position not on NavMesh: {target}");
        return false;
    }
}

// Usage
if (_moveAwayTarget != null)
{
    TrySetDestination(_moveAwayTarget.position);
}
```

**Step 2:** Validate MoveAwayTarget positions in scene:
```csharp
private void OnValidate()
{
    if (_moveAwayTarget != null)
    {
        NavMeshHit hit;
        bool onNavMesh = NavMesh.SamplePosition(_moveAwayTarget.position, out hit, 2.0f, NavMesh.AllAreas);
        if (!onNavMesh)
        {
            Debug.LogError($"SnakeAI ({_snakeName}): MoveAwayTarget '{_moveAwayTarget.name}' is NOT on NavMesh!");
        }
    }
}
```

**Step 3:** Check path status after setting destination:
```csharp
agent.SetDestination(target);

// Wait a frame for path calculation
yield return null;

if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
{
    Debug.LogError($"No valid path to {target}");
    SetState(SnakeState.Idle); // Fallback
}
```

**Warning signs:**
- Snake sets destination but doesn't move
- `agent.hasPath == false` after SetDestination
- `agent.pathPending == true` forever
- MoveAwayTarget timeout logic triggers frequently

**Phase to address:**
**First NavMesh commit** - Add validation immediately. Prevents silent failures that waste debugging time.

---

### Pitfall 6: Attack Range Logic Becomes Distance-Only (Ignores Pathfinding)

**What goes wrong:**
Your attack system uses straight-line distance checks:
```csharp
float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);
if (distanceToPlayer < _biteRange) { /* Bite */ }
```

With NavMesh, straight-line distance can be misleading. Player might be 2 units away straight-line, but 8 units away via walkable path (wall between). Snake thinks it can bite, but can't reach player.

**Why it happens:**
NavMesh pathfinding respects obstacles. `Vector3.Distance` doesn't. You have a semantic mismatch between attack logic (Euclidean distance) and movement logic (path distance).

**How to avoid:**
**Option A:** Use path distance instead of Euclidean distance:
```csharp
private float GetPathDistanceToPlayer()
{
    if (_navMeshAgent.hasPath && _navMeshAgent.destination == _player.position)
    {
        return _navMeshAgent.remainingDistance;
    }
    else
    {
        // Calculate path without setting it as destination
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, _player.position, NavMesh.AllAreas, path))
        {
            return GetPathLength(path);
        }
        return float.MaxValue;
    }
}

private float GetPathLength(NavMeshPath path)
{
    float length = 0f;
    for (int i = 1; i < path.corners.Length; i++)
    {
        length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
    }
    return length;
}
```

**Option B (Simpler):** Keep Euclidean distance BUT add line-of-sight requirement:
```csharp
// You already have this for spells - extend to attacks
private bool CanAttackPlayer()
{
    float distance = Vector3.Distance(transform.position, _player.position);
    if (distance > _maxAttackRange) return false;

    // Must have line-of-sight (already implemented in v1.7.0)
    return _canSeePlayer;
}
```

**Recommendation:** Stick with Option B. Matches your existing spell system design (requires LOS as of v1.7.0). Simpler, no performance overhead from path calculations.

**Warning signs:**
- Snake tries to bite through walls
- Breath/Projectile attacks trigger when player unreachable
- Attack animations play but player takes no damage (too far via path)

**Phase to address:**
**Validation commit** (after NavMesh works) - Not critical for Phase 2, but important for polish. Can defer to Phase 3.

---

### Pitfall 7: Patrol System Needs Complete Rewrite

**What goes wrong:**
Your current patrol system generates random waypoints and uses `MoveTowardsSafe()`. With NavMesh, you can't just pick random points - they must be ON the NavMesh surface. `_currentPatrolTarget` might be invalid (off-mesh, inside walls, etc.).

**Why it happens:**
```csharp
// Current approach (BREAKS with NavMesh)
Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
_currentPatrolTarget = _originalPosition + offset;
agent.SetDestination(_currentPatrolTarget); // Can fail silently
```

Random point might land inside a wall, on a cliff, or outside NavMesh bounds.

**How to avoid:**
**Step 1:** Sample NavMesh for valid patrol points:
```csharp
private void GenerateNewPatrolWaypoint()
{
    // Try up to 10 times to find a valid point
    for (int attempts = 0; attempts < 10; attempts++)
    {
        float radius = Random.Range(_patrolRadiusMin, _patrolRadiusMax);
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        Vector3 randomPoint = _originalPosition + new Vector3(
            Mathf.Cos(angle) * radius,
            0f,
            Mathf.Sin(angle) * radius
        );

        // Check if point is on NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 2.0f, NavMesh.AllAreas))
        {
            _currentPatrolTarget = hit.position;
            return; // Success
        }
    }

    // Fallback: stay at current position
    Debug.LogWarning($"SnakeAI ({_snakeName}): Couldn't find valid patrol point, staying put");
    _currentPatrolTarget = transform.position;
}
```

**Step 2:** Use NavMeshAgent.SetDestination instead of MoveTowardsSafe:
```csharp
private void UpdatePatrol()
{
    // Existing timer/state logic...

    if (_isPatrolling && !_isWaitingAtWaypoint)
    {
        agent.SetDestination(_currentPatrolTarget);

        // Check if reached waypoint using NavMesh distances
        if (!agent.pathPending && agent.remainingDistance <= 0.2f)
        {
            _isWaitingAtWaypoint = true;
            _patrolWaitTimer = _patrolWaitTime;
        }
    }
}
```

**Step 3:** Remove manual rotation (NavMeshAgent handles this):
```csharp
// DELETE this code
transform.rotation = Quaternion.Slerp(...);

// NavMeshAgent.updateRotation = true handles rotation automatically
```

**Warning signs:**
- Snakes don't patrol (waypoints off NavMesh)
- Snakes walk into walls during patrol
- Patrol radius feels wrong (most points invalid)
- Snakes cluster in certain areas (only some areas have valid NavMesh)

**Phase to address:**
**First NavMesh commit** - Patrol is a core behavior, must work from the start.

---

## Technical Debt Patterns

| Shortcut | Immediate Benefit | Long-term Cost | When Acceptable |
|----------|-------------------|----------------|-----------------|
| Keep `MoveTowardsSafe()` alongside NavMesh | Gradual migration, less scary | Double maintenance, conflicting systems, hard-to-debug issues | **NEVER** - creates race conditions |
| Skip destination validation (`SamplePosition`) | Faster implementation | Silent failures, snakes stuck, hard to debug | **NEVER** - validation is 3 lines of code |
| Use Euclidean distance for attacks with NavMesh movement | Simple, no refactoring | Attacks trigger when unreachable, poor player experience | Phase 2 MVP only (fix in Phase 3) |
| Ignore `agent.isStopped` in state machine | Less code changes | Frozen/Dazed/Dead snakes move, breaks game | **NEVER** - core bug |
| Disable NavMesh obstacle avoidance | Simpler setup | Snakes overlap, poor visual quality | Phase 2 testing only (enable before Phase 3) |

## Integration Gotchas

| Integration | Common Mistake | Correct Approach |
|-------------|----------------|------------------|
| **NavMeshAgent + Custom Movement** | Run both systems simultaneously | Choose ONE: NavMesh controls position OR custom script does (via `updatePosition = false`) |
| **NavMeshAgent + Animator** | Leave `applyRootMotion = true` | Set `applyRootMotion = false`, drive animations from `agent.velocity` |
| **NavMeshAgent + State Machine** | Assume states auto-pause agent | Manually sync `agent.isStopped` with state transitions |
| **SetDestination + Random Points** | Pass any Vector3 | Always validate with `NavMesh.SamplePosition()` first |
| **Attack Range + NavMesh** | Use straight-line distance only | Add line-of-sight check OR calculate path distance |
| **NavMeshAgent + Collider** | Add Rigidbody for physics | **NEVER** - creates race condition. Use NavMeshObstacle for static obstacles |

## Performance Traps

| Trap | Symptoms | Prevention | When It Breaks |
|------|----------|------------|----------------|
| Calling `SetDestination()` every frame | Frame drops, pathfinding overhead | Only call when destination changes, cache current destination | 10+ agents recalculating paths every frame |
| Using `NavMesh.CalculatePath()` in Update | Stuttering, CPU spikes | Use `agent.remainingDistance` instead, or calculate paths async | 5+ agents using CalculatePath per frame |
| High obstacle avoidance quality on all agents | FPS drops with many agents | Use HighQuality only for player-visible agents, LowQuality for distant | 20+ agents with HighQuality avoidance |
| Large patrol radius with many invalid samples | Long pauses during waypoint generation | Pre-bake patrol zones, OR reduce sample attempts to 5 | Patrol radius > 10 units in complex geometry |
| NavMesh + Custom Raycast collision both active | Double collision checks, wasted CPU | Remove custom raycasts, trust NavMesh avoidance | Already a problem at 6 snakes with current code |

## "Looks Done But Isn't" Checklist

- [ ] **NavMeshAgent added to prefabs:** Agent component exists BUT `updatePosition`/`updateRotation` sync not configured
- [ ] **Destinations set:** `SetDestination()` called BUT never validated with `SamplePosition()`
- [ ] **State machine transitions:** States change BUT `agent.isStopped` not synced, agents move during Frozen state
- [ ] **Animations play:** Slither animations trigger BUT don't match `agent.velocity`, foot sliding visible
- [ ] **Patrol works:** Snakes walk around BUT waypoints off-NavMesh, frequent failures
- [ ] **Attacks trigger:** Animations play BUT range checks ignore NavMesh paths, attacks through walls
- [ ] **Custom movement removed:** `MoveTowardsSafe()` deleted BUT rotation logic also removed, snakes don't face movement direction
- [ ] **NavMesh baked:** Surface exists BUT coverage incomplete, snakes stuck in certain areas

## Recovery Strategies

| Pitfall | Recovery Cost | Recovery Steps |
|---------|---------------|----------------|
| Transform position conflict (race condition) | **HIGH** | 1. Set `agent.updatePosition = false` immediately. 2. Test if NavMesh still pathfinds. 3. If yes, switch to full NavMesh control. 4. If no, keep `updatePosition = false` and manually sync positions. |
| State machine not syncing `isStopped` | **LOW** | 1. Add `agent.isStopped = true/false` to `SetState()`. 2. Test all spell behaviors. 3. Verify Frozen/Dazed/Dead states immobile. |
| Invalid patrol waypoints | **MEDIUM** | 1. Add `NavMesh.SamplePosition()` to `GenerateNewPatrolWaypoint()`. 2. Increase sample radius to 2.0f. 3. Add fallback to current position if all attempts fail. |
| Animation desync | **MEDIUM** | 1. Change `UpdateMovementAnimation()` to read `agent.velocity`. 2. Keep directional logic unchanged. 3. Test Forward/Left/Right animations. |
| MoveAwayTarget off NavMesh | **LOW** | 1. Add `OnValidate()` check for MoveAwayTarget. 2. Open scene, check Console for errors. 3. Reposition MoveAwayTarget GameObjects onto NavMesh surface. |
| Attack range ignores obstacles | **LOW** | Already fixed in v1.7.0 (line-of-sight check added). Ensure `_canSeePlayer` used in attack logic. |
| Both custom collision AND NavMesh avoidance running | **MEDIUM** | 1. Comment out `MoveTowardsSafe()` calls. 2. Test if NavMesh avoidance works. 3. If yes, delete method entirely. 4. If no, debug NavMesh obstacle settings. |

## Pitfall-to-Phase Mapping

| Pitfall | Prevention Phase | Verification |
|---------|------------------|--------------|
| Transform position conflict | **Immediate (First Commit)** | Snake moves smoothly, no jittering, `MoveTowardsSafe()` removed |
| State machine freeze control | **Immediate (First Commit)** | Frozen/Dazed/Dead snakes are immobile, `agent.isStopped` logs show correct values |
| Collision detection redundancy | **Immediate (First Commit)** | Snakes don't overlap, no raycast debug logs, `MoveTowardsSafe()` deleted |
| Animator state conflicts | **Second Commit (Animation Sync)** | Slither animations match movement, no foot sliding, direction correct |
| MoveAwayTarget destination validation | **First Commit** | OnValidate() logs no errors, snakes reach MoveAwayTarget 100% success rate |
| Attack range logic | **Phase 3 (Polish)** | Attacks don't trigger through walls, LOS check in attack logic |
| Patrol system rewrite | **First Commit** | Patrol waypoints always valid, `SamplePosition()` used, no stuck snakes |

## Sources

**Unity Official Documentation:**
- [Using NavMesh Agent with Other Components](https://docs.unity3d.com/Packages/com.unity.ai.navigation@1.1/manual/MixingComponents.html) - Transform control conflicts, updatePosition/updateRotation
- [Coupling Animation and Navigation](https://docs.unity3d.com/Packages/com.unity.ai.navigation@1.1/manual/CouplingAnimationAndNavigation.html) - Root motion conflicts, OnAnimatorMove patterns
- [NavMeshAgent API Reference](https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent.html) - isStopped, SetDestination, path validation
- [NavMesh.SamplePosition API](https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html) - Destination validation

**Community Issues:**
- [NavMeshAgent updates position even when updatePosition is false](https://issuetracker.unity3d.com/issues/navmeshagent-updates-position-even-when-updateposition-is-false) - Known Unity bug affecting hybrid approaches
- [NavMeshAgent.isStopped == false does not resume movement](https://discussions.unity.com/t/navmeshagent-isstopped-false-does-not-resume-movement/251465) - State pause/resume pitfalls
- [NavMesh Agent internal position](https://forum.unity.com/threads/navmesh-agent-internal-position.283709/) - Transform vs nextPosition confusion
- [Conflict between CharacterController and NavMeshAgent](https://discussions.unity.com/t/conflict-between-charactercontroller-and-navmeshagent/50537) - Multi-component conflicts

**Migration Patterns:**
- [Unity 3D – Smooth Turning with NavMeshAgent AI](https://keithmaggio.wordpress.com/2019/07/05/unity-3d-smooth-turning-with-navmeshagent-ai/) - Rotation handling when migrating
- [Unity NavMeshAgent Destination Invalid: Complete Guide](https://copyprogramming.com/howto/unity-nav-mesh-agent-destination-invalid) - Destination validation best practices
- [Upgrade projects for use with AI Navigation package](https://docs.unity3d.com/Packages/com.unity.ai.navigation@1.1/manual/UpgradeGuide.html) - Migration checklist

**Performance:**
- [Horde of NavMeshAgents - stops to recalculate path](https://answers.unity.com/questions/867642/horde-of-navmeshagents-stops-to-recalculate-path.html) - SetDestination overhead
- [How to Use NavMesh Agent in Unity - Complete Guide](https://outscal.com/blog/how-to-use-navmesh-agent-in-unity) - Component caching, optimization

---

*Pitfalls research for: Snake Enchanter - NavMesh Brownfield Migration*
*Researched: 2026-02-16*
*Confidence: HIGH (Official docs + community issues + project code analysis)*
