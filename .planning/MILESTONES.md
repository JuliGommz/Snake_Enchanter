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

*This file tracks completed milestones. Current milestone: v0.3*
