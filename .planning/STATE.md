# Snake Enchanter - Project State

## Current Position

**Phase:** 6 (Cleanup & Polish — IN PROGRESS)
**Plan:** 06-01 (checkpoint:human-verify — awaiting user)
**Status:** Plan 06-01 Task 1 done. SnakeAI v1.8.5 committed (fd41f0d). Awaiting human verification in Unity Editor (Console check in Play mode).
**Last activity:** 2026-02-17 — Phase 6 Plan 01: SnakeAI v1.8.5 cleanup committed, checkpoint reached

## Project Reference

See: `.planning/PROJECT.md` (updated 2026-02-16)

**Core value:** Precise timing gameplay that feels rewarding when mastered and punishing when failed
**Current focus:** v0.3 Bug Fixes & Stability

## Recent Progress

**2026-02-17 (Phase 6 - Cleanup & Polish):**
- ⏳ Plan 06-01: SnakeAI v1.8.5 — Debug.Log cleanup (checkpoint:human-verify pending)
- ✅ Task 1 done: Removed 16 Debug.Log calls, updated NOTES + version header (commit fd41f0d)
- ✅ Preserved: all 5 Debug.LogWarning, LookAtPlayer(), _isPatrolling bool
- 🔔 NEXT STEP: Open Unity, Play mode, verify Console has no log spam, type "approved"

**2026-02-17 (Phase 5 - NavMesh Migration COMPLETE):**
- ✅ Plan 05-03 complete: SnakeAI v1.8.3 — all movement via NavMeshAgent (commit 7ef80c6)
- ✅ FollowPlayer() → _agent.SetDestination(_playerTransform.position) at _chaseSpeed
- ✅ StartMoveAwayMovement() → _agent.SetDestination(_moveAwayTarget.position) at _moveSpeed
- ✅ MovedAway arrival: Vector3.Distance + 2s timeout → HasAgentArrived()
- ✅ UpdateMovementAnimation(): _isPatrolling bool → _agent.velocity.magnitude > 0.1f (animation bug fixed)
- ✅ MoveTowardsSafe() method deleted (~40 lines)
- ✅ _lastMoveDirection field deleted
- 📋 Next: Plan 05-04 (final validation)
- ✅ Plan 05-02 complete: SnakeAI v1.8.2 — patrol via SetDestination (commit 5d8ac55)
- ✅ UpdatePatrol() MoveTowardsSafe() → _agent.SetDestination(_currentPatrolTarget)
- ✅ Arrival check: Vector3.Distance → HasAgentArrived() (fixes remainingDistance=Infinity bug)
- ✅ Waypoint validation: GenerateNewPatrolWaypoint() now uses NavMesh.SamplePosition (5 attempts, fallback to _originalPosition)
- ✅ Velocity-based rotation in patrol (agent.velocity direction, not target-based)
- ✅ _agent.ResetPath() on player-spotted and on waypoint arrival
- ✅ Plan 05-01 complete: SnakeAI v1.8.1 — NavMeshAgent active (updatePosition=true)
- ✅ nextPosition sync before enabling (prevents teleport snap)
- ✅ HasAgentArrived() helper added (4-condition, fixes remainingDistance=Infinity bug)
- ✅ SetState() wired: Frozen→isStopped, Dazed/Dead/AttackingEnemy→isStopped+ResetPath, Idle/Aggressive/MovedAway→isStopped=false

**2026-02-17 (Earlier):**
- ✅ Phase 4 complete: NavMeshAgent added to 6 snake prefabs
- ✅ SnakeAI v1.8.0: Awake() passive init (updatePosition=false, updateRotation=false, isStopped=true)
- ✅ NavMesh rebaked: snakes excluded from obstacle geometry
- ✅ Play mode verified: no errors, dual system stable

**2026-02-17 (Today):**
- ✅ Phase 3 complete: NavMesh baked in GameLevel scene
- ✅ NavMeshSurface configured (Collect Objects: All, Height: 0.5, Radius: 0.3)
- ✅ Blue overlay confirmed on all cave floor surfaces
- ✅ Play mode: snakes still patrol normally (animation bug still present — expected)

**2026-02-16 (Earlier):**
- ✅ GSD milestone v0.3 initialized (PROJECT.md, STATE.md, MILESTONES.md)
- ✅ Research phase complete (4 files: STACK, FEATURES, ARCHITECTURE, PITFALLS)
- ✅ Requirements defined (REQUIREMENTS.md - 4 core requirements)
- ✅ Roadmap created (ROADMAP.md - 5 phases, 5 hours estimated)

**2026-02-16 (Earlier):**
- ✅ Player ground detection fix (Start() method with `_velocity.y = -5f`)
- ✅ Session 17 documentation complete (Arbeitsprotokoll, PHASE3_SCOPE, MERGE_CHECKLIST)
- ✅ GSD milestone initialization started

**2026-02-15 (Session 17):**
- ✅ SnakeAI v1.7.2: Fixed 4 critical bugs (IsDazed, attack cooldown, die animation, Tune 4 unlock)
- ✅ Testing complete: Tune 1-3 working, Tune 4 unlocked but non-functional
- ✅ Phase 2 declared feature-complete (with Tune 4 moved to backlog)
- 🔄 User placing Snake prefabs in scene manually

**2026-02-14 (Session 16):**
- ✅ SnakeAI v1.6.0: Directional slither animations + debug logging
- ✅ Tune 2 Sleep → Daze rename (all files)
- ✅ Attack non-snake creatures targeting
- ✅ BACKLOG section created (7 features deferred to Phase 3)

## Active Issues

**HIGH PRIORITY (v0.3 Scope):**
1. ✅ **Snake patrol animation jump bug — FIXED**
   - Root cause: MoveTowardsSafe() blocked → _isPatrolling bool stayed true → animation reset frame 0
   - Fix 1: NavMeshAgent.SetDestination() replaces MoveTowardsSafe() — pathfinding around obstacles
   - Fix 2: Animator clips W Root → In Place (removed position data from animation)
   - Fix 3: applyRootMotion=false + LateUpdate() manual sync (updatePosition=false)
   - Verified in Play mode: snakes patrol without snapping ✅

2. ✅ **NavMesh migration — COMPLETE**
   - ✅ Phase 3: NavMesh baked in GameLevel scene
   - ✅ Phase 4: NavMeshAgent on all 6 prefabs
   - ✅ Phase 5: Full migration — SetDestination, HasAgentArrived(), velocity animation, MoveTowardsSafe() deleted

3. ⏳ **Full feature testing** (Next: Phase 6)
   - Test Slither Left/Right in game
   - Verify all 4 Tunes work correctly
   - Confirm no regressions from NavMesh migration

**DEFERRED (Phase 3+):**
- Tune 4 (Freeze): Implemented but not functional — Phase 3 debugging
- 3 Areas: Only 1 exists — Phase 3 level design
- Backend API: Not integrated — Phase 3 backend work
- Menu/Result Screen polish — Phase 3 UI work

## Accumulated Context

**Project Structure:**
- Unity 2022 LTS, URP, New Input System, Cinemachine v3.x
- Branch: `feature/enemy-setup` (9 commits, ready for merge after NavMesh)
- Namespace: `SnakeEnchanter.*` (Core, Player, Snakes, Tunes, Level, UI, Data)

**Core Systems:**
- PlayerController v1.8 (First-person, crouch, Cinemachine pitch-only)
- HealthSystem v1.3 (Drain, restoration, death animations)
- TuneController v2.5 (4 Tunes, Genshin-style slider, Tune 4 unlocked)
- SnakeAI v1.8.5 (7-state machine, NavMeshAgent, full NavMesh movement, submission-clean, zero Debug.Log spam)
- GameManager v1.1.1 (Win/Lose, Mode selection)

**Key Files:**
- `Assets/_Project/Scripts/Player/PlayerController.cs` - Player movement + camera
- `Assets/_Project/Scripts/Snakes/SnakeAI.cs` - Snake behavior v1.8.5 (full NavMesh, submission-clean)
- `Assets/_Project/Scripts/TuneSystem/TuneController.cs` - Spell casting
- `Assets/_Project/Scenes/GameLevel.unity` - Main gameplay scene

**Recent Lessons:**
- Always add Start() for initial state setup (ground detection)
- Velocity-based animation triggers more robust than booleans
- Teacher feedback: Use Unity native solutions over custom code
- Documentation must be kept in sync with actual implementation
- **NavMesh + Animation:** Use "In Place" animation clips (not "W Root") when NavMeshAgent drives position
- **Root Motion:** applyRootMotion=false alone is not enough — the FBX clip type must also be "In Place"
- **updatePosition=false + LateUpdate sync** = safest NavMesh setup for animated characters

**Key Decisions (Plan 05-01):**
- NavMeshAgent position sync (nextPosition before updatePosition=true) prevents teleport snap
- 4-condition HasAgentArrived() required — Unity remainingDistance returns Infinity on multi-segment paths
- Frozen uses isStopped only (preserves path for resume); Dazed/Dead/AttackingEnemy use ResetPath()

**Key Decisions (Plan 05-02):**
- GenerateNewPatrolWaypoint() uses 5 attempts with sampleRadius=1.0f (2x agent height, per Unity docs)
- ResetPath() chosen over isStopped=true at waypoint arrival — definitively stopped, not a pause
- _isPatrolling bool retained (still used in UpdateMovementAnimation) — velocity-based animation is 05-03+ scope

**Key Decisions (Plan 06-01):**
- LookAtPlayer() RETAINED — NavMeshAgent drives position only, not rotation; LookAtPlayer() is required for Y-axis facing in all idle interaction ranges
- _isPatrolling bool RETAINED — live state guard in UpdatePatrol() prevents per-frame waypoint regeneration
- Debug.LogWarning kept for 5 edge cases (no player, no patrol waypoint, no MoveAwayTarget, no creature target, renderer not found)

---
*Last updated: 2026-02-17 after Plan 06-01 Task 1 (checkpoint:human-verify pending)*
