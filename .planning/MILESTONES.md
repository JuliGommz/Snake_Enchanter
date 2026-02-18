# Milestones - Snake Enchanter

## v0.2 - Phase 2: KOMPLETT (Completed)

**Completed:** 2026-02-15 (Session 17)
**Duration:** Feb 10-15, 2026 (6 days)
**Branch:** `feature/enemy-setup` (9 commits)

### What Shipped

**Core Features:**
- All 4 Tunes implemented (Move, Daze, Attack, Freeze - though Freeze non-functional)
- Complete Snake AI system (SnakeAI v1.7.2)
- 7-state machine (Idle, Aggressive, MovedAway, Dazed, AttackingEnemy, Frozen, Dead)
- Patrol system with random waypoints
- Proximity detection with line-of-sight
- Range-based attacks (Bite < 0.5m, Breath 4-7m, Projectile 8m+)
- Directional slither animations (Forward/Left/Right)
- Visual feedback (Material Emission glow by state)
- Collision detection (Environment + Props + Snakes)
- Debug logging system for testing

**Bug Fixes (Session 17):**
1. IsDazed parameter missing - Fixed controller mismatch
2. Attack cooldown after Daze - Reset `_lastAttackTime`
3. Die animation loop - Set IsDazed in Dead state
4. Tune 4 UI missing - Unlocked for testing

**Documentation:**
- Session 17 Arbeitsprotokoll entry
- Phase 2→3 handoff section in DESIGN_CHANGES.md
- MERGE_CHECKLIST.md created
- PHASE3_SCOPE.md created
- Projektplan updated (Phase 2 complete)

**Last Phase Number:** 2

### Deferred to Next Milestone

- Tune 4 (Freeze) functionality - Code exists but doesn't work
- 3 Areas implementation - Only 1 area (GameLevel) exists
- Backend API integration
- Main Menu polish
- Result Screen polish

---

## v0.1 - Phase 1: SPIELBAR (Completed)

**Completed:** 2026-02-09 (Session 9)
**Duration:** Feb 3-9, 2026 (7 days)

### What Shipped

**Core Systems:**
- PlayerController v1.7 (WASD movement, mouse look, crouch, Cinemachine)
- HealthSystem v1.2.1 (passive drain, restoration, death)
- TuneController v2.3 (slider-based spell casting, ADR-008)
- GameManager v1.1 (Win/Lose conditions, mode selection)
- ExitTrigger (Win condition)

**Assets & Setup:**
- Pirate character (FBX, Avatar, Materials, 13 Animations)
- Cave environment (Caves Parts Set + Dwarven Pack)
- Toon Snakes Pack (6 prefabs)
- Canvas UI (HealthBarUI v3.1, TuneSliderUI v2.1, Steampunk theme)

**Animations:**
- Player: 4 Movement, 4 Spell, 2 Death animations
- MC_Controller with 10 states total

**Last Phase Number:** 1

---

*This file tracks completed milestones.*

---

## v0.3 - Bug Fixes & Stability (Completed)

**Completed:** 2026-02-18 (Sessions 18-19)
**Duration:** Feb 16-18, 2026 (3 days)
**Branches:** `feature/enemy-setup` + `feature/cleanup-polish` (merged to main)
**SnakeAI:** v1.7.2 → v1.8.5

### What Shipped

**NavMesh Migration (Phases 3-6):**
- NavMesh baked in GameLevel scene — snakes navigate around obstacles
- NavMeshAgent on all 6 snake prefabs with dual-system transition
- Full movement migration — SetDestination replaces MoveTowardsSafe()
- Patrol animation bug fixed — velocity-based triggers instead of booleans
- SnakeAI v1.8.5 submission-clean — zero Debug.Log spam

**Bug Fixes:**
1. Snake patrol animation jump/restart — NavMesh pathfinding eliminates collision-based movement
2. Player floating on spawn — ground detection fix in Start()
3. Animator W Root → In Place clips for all Slither states

**Key Technical Decisions:**
- updatePosition=false + LateUpdate sync for animated NavMesh characters
- 4-condition HasAgentArrived() (Unity remainingDistance=Infinity bug workaround)
- Velocity-based animation triggers replace boolean-based system
- LookAtPlayer() retained — NavMeshAgent drives position only, not rotation

**Last Phase Number:** 6 (Phase 7 testing done manually, not GSD-tracked)

### Deferred to Next Milestone

- Tune 4 (Freeze) — code exists but non-functional
- Backend API integration
- Menu Scene UI
- Audio/Visual polish
- Spell gathering system (new feature)

---

