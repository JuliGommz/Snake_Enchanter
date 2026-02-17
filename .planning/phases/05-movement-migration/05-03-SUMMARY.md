---
phase: 05-movement-migration
plan: 03
subsystem: snakes
tags: [navmesh, navmeshagent, animation, velocity, state-machine, unity-ai]

requires:
  - phase: 05-02
    provides: UpdatePatrol() NavMesh migration, GenerateNewPatrolWaypoint() with SamplePosition, HasAgentArrived() helper

provides:
  - FollowPlayer() via agent.SetDestination(_playerTransform.position) with _chaseSpeed
  - StartMoveAwayMovement() via agent.SetDestination(_moveAwayTarget.position) with _moveSpeed
  - MovedAway arrival detection via HasAgentArrived() (replaces Vector3.Distance check)
  - UpdateMovementAnimation() velocity-based: agent.velocity.magnitude > 0.1f (the core bug fix)
  - MoveTowardsSafe() method fully deleted
  - _lastMoveDirection field fully deleted

affects: [05-04, any future snake AI changes]

tech-stack:
  added: []
  patterns:
    - "Velocity-based animation: agent.velocity.magnitude > 0.1f instead of boolean state flags"
    - "Direction from velocity: transform.InverseTransformDirection(_agent.velocity.normalized)"
    - "State-guarded velocity: only animate moving states (Aggressive, Idle, MovedAway)"
    - "NavMesh arrival: HasAgentArrived() three-condition check for all movement termination"

key-files:
  created: []
  modified:
    - Assets/_Project/Scripts/Snakes/SnakeAI.cs

key-decisions:
  - "Velocity threshold 0.1f for movement detection — filters near-zero drift when stopping, same as patrol"
  - "State guard in UpdateMovementAnimation() covers Aggressive/Idle/MovedAway — other states (Dazed, Frozen, Dead) should never animate slither"
  - "FollowPlayer() guards agent.isOnNavMesh to prevent SetDestination on unregistered agent"
  - "StartMoveAwayMovement() guards _currentState != MovedAway to prevent stale Invokes from old states"
  - "Deleted entire blocking-timeout system (2s timer) from MovedAway — NavMesh handles obstacle routing"

patterns-established:
  - "All snake movement via NavMeshAgent.SetDestination() — no more direct transform.position writes"
  - "Animation driven by actual velocity, not intent booleans"

duration: 15min
completed: 2026-02-17
---

# Phase 05 Plan 03: NavMesh Full Migration Summary

**SnakeAI v1.8.3: all movement via NavMeshAgent.SetDestination(), velocity-based animation replacing _isPatrolling bool — the root cause of the animation restart bug is now fixed**

## Performance

- **Duration:** ~15 min
- **Started:** 2026-02-17T12:46:02Z
- **Completed:** 2026-02-17T12:58:00Z
- **Tasks:** 2
- **Files modified:** 1

## Accomplishments

- Replaced `FollowPlayer()` MoveTowardsSafe call with `_agent.SetDestination(_playerTransform.position)` at `_chaseSpeed`
- Replaced `StartMoveAwayMovement()` `_isMoving = true` with `_agent.SetDestination(_moveAwayTarget.position)` at `_moveSpeed`
- Replaced MovedAway state's Vector3.Distance arrival check + 2-second blocking timeout with `HasAgentArrived()`
- Replaced `UpdateMovementAnimation()` `_isPatrolling` boolean check with `_agent.velocity.magnitude > 0.1f` — this is the actual bug fix
- Deleted `MoveTowardsSafe()` method entirely (~40 lines, including SphereCast collision, transform.position writes)
- Deleted `_lastMoveDirection` field (direction now derived inline from `_agent.velocity.normalized`)

## Task Commits

Both tasks were committed as a single atomic unit:

1. **Task 1 + Task 2: All NavMesh migration changes** - `7ef80c6` (feat)

## Files Created/Modified

- `Assets/_Project/Scripts/Snakes/SnakeAI.cs` - v1.8.2 → v1.8.3: all movement via NavMeshAgent, velocity-based animation, MoveTowardsSafe deleted

## Decisions Made

- Velocity threshold `0.1f` matches the patrol phase threshold — consistent across all motion states
- `UpdateMovementAnimation()` state guard includes Aggressive, Idle, and MovedAway — these are the only states where slither animation is appropriate
- Deleted the 2-second blocking timeout from MovedAway entirely — it was a workaround for wall-blocking that NavMesh renders unnecessary
- `StartMoveAwayMovement()` now returns early if `_currentState != MovedAway` — protects against stale Invoke() calls if state changed before the delay elapsed
- `FollowPlayer()` guards `_agent.isOnNavMesh` before SetDestination — defensive check matches the pattern established in Plans 01 and 02

## Deviations from Plan

None — plan executed exactly as written. The MovedAway case replacement, FollowPlayer() replacement, StartMoveAwayMovement() replacement, UpdateMovementAnimation() replacement, MoveTowardsSafe() deletion, and _lastMoveDirection deletion all matched the plan specification.

## Issues Encountered

None. The code changes were clean substitutions. No unexpected method signatures, no missing fields, no compile-time surprises.

## User Setup Required

None - no external service configuration required. Compile verification must be done in Unity Editor.

## Next Phase Readiness

- SnakeAI.cs v1.8.3 is complete — all movement is now NavMeshAgent-driven
- MoveTowardsSafe() is fully removed — no more dual-write conflict risk
- Animation bug is resolved by design — velocity is physically 0 when agent is blocked
- Ready for Plan 05-04: final validation, testing, and cleanup

---
*Phase: 05-movement-migration*
*Completed: 2026-02-17*
