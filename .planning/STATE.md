# Snake Enchanter - Project State

## Current Position

**Phase:** 5 (Movement Migration — In progress)
**Plan:** 03 (next)
**Status:** Plan 05-02 complete. SnakeAI v1.8.2 — patrol now uses SetDestination + NavMesh.SamplePosition waypoint validation + HasAgentArrived() arrival check. Ready for Plan 05-03 (FollowPlayer + MovedAway NavMesh migration).
**Last activity:** 2026-02-17 — Plan 05-02 NavMesh Patrol Replacement complete (commit 5d8ac55)

## Project Reference

See: `.planning/PROJECT.md` (updated 2026-02-16)

**Core value:** Precise timing gameplay that feels rewarding when mastered and punishing when failed
**Current focus:** v0.3 Bug Fixes & Stability

## Recent Progress

**2026-02-17 (Today - continued):**
- ✅ Plan 05-02 complete: SnakeAI v1.8.2 — patrol via SetDestination (commit 5d8ac55)
- ✅ UpdatePatrol() MoveTowardsSafe() → _agent.SetDestination(_currentPatrolTarget)
- ✅ Arrival check: Vector3.Distance → HasAgentArrived() (fixes remainingDistance=Infinity bug)
- ✅ Waypoint validation: GenerateNewPatrolWaypoint() now uses NavMesh.SamplePosition (5 attempts, fallback to _originalPosition)
- ✅ Velocity-based rotation in patrol (agent.velocity direction, not target-based)
- ✅ _agent.ResetPath() on player-spotted and on waypoint arrival
- 📋 Next: Plan 05-03 (FollowPlayer + MovedAway NavMesh migration)
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
1. ⚠️ **Snake patrol animation jump bug**
   - When blocked by collider, animation restarts from frame 0
   - Root cause confirmed: MoveTowardsSafe() + boolean _isPatrolling. Fix requires BOTH NavMesh movement AND velocity-based animation trigger (Phase 5). Teacher confirmed.
   - Solution: Migrate to NavMeshAgent (Phase 5.3: velocity-based animation triggers)
   - Teacher-approved approach

2. 🔄 **NavMesh migration**
   - ✅ Phase 3 complete (baked in GameLevel scene)
   - ✅ Phase 4 complete (NavMeshAgent on all 6 prefabs, passive init in code)
   - ✅ Plan 05-01 complete (NavMeshAgent active, HasAgentArrived(), SetState wired)
   - ✅ Plan 05-02 complete (UpdatePatrol → SetDestination + SamplePosition validation)
   - 📋 Plan 05-03 next: FollowPlayer() + MovedAway state → SetDestination
   - 📋 Phase 5 includes: Update animation triggers from boolean to velocity check

3. ⏳ **Full feature testing** (After NavMesh)
   - Test Slither Left/Right (code exists, only Forward tested)
   - Verify all 4 Tunes work correctly
   - Confirm no regressions from ground fix

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
- SnakeAI v1.8.2 (7-state machine, NavMeshAgent active, patrol via SetDestination+SamplePosition, HasAgentArrived(), SetState agent control)
- GameManager v1.1.1 (Win/Lose, Mode selection)

**Key Files:**
- `Assets/_Project/Scripts/Player/PlayerController.cs` - Player movement + camera
- `Assets/_Project/Scripts/Snakes/SnakeAI.cs` - Snake behavior (patrol migrated, FollowPlayer+MovedAway still pending)
- `Assets/_Project/Scripts/TuneSystem/TuneController.cs` - Spell casting
- `Assets/_Project/Scenes/GameLevel.unity` - Main gameplay scene

**Recent Lessons:**
- Always add Start() for initial state setup (ground detection)
- Velocity-based animation triggers more robust than booleans
- Teacher feedback: Use Unity native solutions over custom code
- Documentation must be kept in sync with actual implementation

**Key Decisions (Plan 05-01):**
- NavMeshAgent position sync (nextPosition before updatePosition=true) prevents teleport snap
- 4-condition HasAgentArrived() required — Unity remainingDistance returns Infinity on multi-segment paths
- Frozen uses isStopped only (preserves path for resume); Dazed/Dead/AttackingEnemy use ResetPath()

**Key Decisions (Plan 05-02):**
- GenerateNewPatrolWaypoint() uses 5 attempts with sampleRadius=1.0f (2x agent height, per Unity docs)
- ResetPath() chosen over isStopped=true at waypoint arrival — definitively stopped, not a pause
- _isPatrolling bool retained (still used in UpdateMovementAnimation) — velocity-based animation is 05-03+ scope

---
*Last updated: 2026-02-17 after Plan 05-02 completion*
