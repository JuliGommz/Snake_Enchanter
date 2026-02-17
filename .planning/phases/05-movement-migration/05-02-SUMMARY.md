---
phase: 05-movement-migration
plan: 02
subsystem: snake-ai
tags: [navmesh, patrol, animation-bug-fix, setdestination, sampleposition]
dependency_graph:
  requires:
    - 05-01  # HasAgentArrived() added, updatePosition=true enabled
  provides:
    - NavMesh-driven patrol (SetDestination replaces MoveTowardsSafe in UpdatePatrol)
    - NavMesh.SamplePosition waypoint validation
  affects:
    - SnakeAI patrol behavior (obstacle navigation, animation correctness)
tech_stack:
  added: []
  patterns:
    - "NavMeshAgent.SetDestination() for patrol movement"
    - "NavMesh.SamplePosition() for waypoint validation with fallback"
    - "Velocity-based rotation: _agent.velocity direction (not target-based)"
    - "_agent.ResetPath() on player-spotted and on waypoint arrival"
key_files:
  modified:
    - Assets/_Project/Scripts/Snakes/SnakeAI.cs  # v1.8.1 → v1.8.2
decisions:
  - "Kept _isPatrolling bool (not removed) — still used for animation check in UpdateMovementAnimation(); will be replaced in a later plan when animation system migrates to velocity-based check"
  - "MoveTowardsSafe() method definition retained — still called from FollowPlayer() and MovedAway state; removal scope is 05-03+"
  - "GenerateNewPatrolWaypoint() uses maxAttempts=5 with sampleRadius=1.0f (2x agent height, per Unity docs)"
  - "ResetPath() chosen over isStopped=true at waypoint arrival — snake has definitively stopped moving, not a temporary pause"
metrics:
  duration_minutes: 2
  completed_date: "2026-02-17"
  tasks_completed: 2
  files_modified: 1
---

# Phase 5 Plan 02: NavMesh Patrol Replacement Summary

**One-liner:** Patrol driven by NavMeshAgent.SetDestination with NavMesh.SamplePosition waypoint validation, fixing the animation restart bug caused by MoveTowardsSafe returning false on wall contact.

## What Was Done

Replaced the manual movement system in `UpdatePatrol()` and `GenerateNewPatrolWaypoint()` with NavMeshAgent-based navigation.

### Task 1: Replace UpdatePatrol() movement and arrival check

Three changes in `UpdatePatrol()`:

**A) Movement section** — removed `MoveTowardsSafe(_currentPatrolTarget, patrolSpeed)` and the target-based Slerp rotation. Replaced with:
```csharp
if (_agent != null && _agent.isOnNavMesh)
{
    float patrolSpeed = _moveSpeed * 0.75f;
    _agent.speed = patrolSpeed;
    _agent.SetDestination(_currentPatrolTarget);

    if (_agent.velocity.sqrMagnitude > 0.01f)
    {
        Vector3 moveDir = new Vector3(_agent.velocity.x, 0f, _agent.velocity.z).normalized;
        if (moveDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * 5f);
    }
}
```

**B) Arrival check** — replaced `Vector3.Distance(transform.position, _currentPatrolTarget) < 0.2f` with `HasAgentArrived()` (the 3-condition check added in 05-01). Added `_agent.ResetPath()` on arrival.

**C) Player-visible stop** — added `_agent.ResetPath()` in the early-return block when player is spotted, preventing the agent from continuing movement while patrol is suspended.

### Task 2: Replace GenerateNewPatrolWaypoint() with NavMesh.SamplePosition

Replaced the raw offset calculation with a validated loop:
```csharp
for (int i = 0; i < maxAttempts; i++)
{
    // generate candidatePoint from random angle + radius
    NavMeshHit hit;
    if (NavMesh.SamplePosition(candidatePoint, out hit, sampleRadius, NavMesh.AllAreas))
    {
        _currentPatrolTarget = hit.position;
        return;
    }
}
// Fallback: _currentPatrolTarget = _originalPosition
```

## Why This Matters

The root cause of the patrol animation restart bug: `MoveTowardsSafe()` returns `false` when blocked by a wall, so the snake's position doesn't change — but `_isPatrolling` stays `true`, causing `UpdateMovementAnimation()` to set `Slither Forward = true` every frame with zero actual movement. With Unity's animator, setting a bool trigger when already in that state causes a transition restart.

With `SetDestination()`, the NavMeshAgent navigates *around* obstacles. When the agent genuinely stops (arrived or cannot move), `_agent.velocity` drops to near-zero naturally. This means `UpdateMovementAnimation()` will correctly detect non-movement — the animation won't restart incorrectly.

## Verification Checklist

- `_agent.SetDestination(_currentPatrolTarget)` present in UpdatePatrol(): YES
- `_agent.speed = patrolSpeed` set before SetDestination: YES
- `HasAgentArrived()` used for arrival (no raw Vector3.Distance): YES
- `_agent.ResetPath()` called on arrival AND on player-spotted: YES
- No `MoveTowardsSafe` in UpdatePatrol(): YES (remains only in FollowPlayer and MovedAway — 05-03+ scope)
- `NavMesh.SamplePosition` in GenerateNewPatrolWaypoint(): YES
- `_currentPatrolTarget` set to `hit.position`: YES
- Fallback to `_originalPosition` on all attempts failed: YES
- Version v1.8.2 in header: YES
- `using UnityEngine.AI` still present: YES

## Deviations from Plan

None — plan executed exactly as written.

## Commits

| Hash | Description |
|------|-------------|
| 5d8ac55 | feat(05-02): SnakeAI v1.8.2 - NavMesh patrol replacement |

## Self-Check

- [x] `Assets/_Project/Scripts/Snakes/SnakeAI.cs` — exists and contains `NavMesh.SamplePosition`
- [x] Commit `5d8ac55` — verified above

## Self-Check: PASSED
