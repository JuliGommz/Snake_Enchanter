# Snake Enchanter

## What This Is

A Unity 3D timing-based puzzle game where a wounded warrior trapped in a collapsed ancient ruin uses magical melodies to command snakes. Players must precisely time slider inputs to cast spells that move, daze, or redirect snakes while racing against continuously depleting health. The core mechanic is a Genshin-style hold-and-release slider system where timing determines success or failure.

## Core Value

Precise timing gameplay that feels rewarding when mastered and punishing when failed. Every slider release must feel consequential - success extends survival, mistakes cost health.

## Current Milestone: v0.3 Bug Fixes & Stability

**Goal:** Fix critical bugs and stabilize Phase 2 features before Phase 3 polish work

**Target features:**
- Fix Snake patrol animation jump bug (NavMesh migration)
- Test and verify all existing Phase 2 features
- Ensure player spawns grounded (ground detection fix - COMPLETE)
- Prepare stable foundation for Phase 3 audio/visual polish

## Requirements

### Validated

<!-- Phase 1 & 2 - Shipped and working -->

- ✓ Player can move with WASD + mouse look (first-person) — Phase 1
- ✓ Player can crouch with Ctrl — Phase 1
- ✓ Player has passively draining health bar — Phase 1
- ✓ Player can cast 4 tunes with hold-and-release slider timing — Phase 2
- ✓ Player wins by reaching exit with HP > 0 — Phase 1
- ✓ Player loses when HP reaches 0 — Phase 1
- ✓ Snake AI patrols, detects player, and responds to spells — Phase 2
- ✓ Snakes can attack player at range (Bite/Breath/Projectile) — Phase 2
- ✓ Tune 1 (Move): Snake moves away to MoveAwayTarget — Phase 2
- ✓ Tune 2 (Daze): Snake collapses for 8 seconds — Phase 2
- ✓ Tune 3 (Attack): Snake attacks nearby creature — Phase 2
- ✓ Successful spell casts restore player HP — Phase 2
- ✓ Failed spell casts trigger snake attack — Phase 2
- ✓ Simple and Advanced modes selectable from menu — Phase 2

### Active

<!-- v0.3 - Bug fixes and stability -->

- [ ] Snake patrol animations don't jump/restart when blocked by colliders
- [ ] Snakes use NavMesh pathfinding instead of custom movement
- [ ] Player spawns fully grounded, not floating above ground
- [ ] All Phase 2 features verified working (no regressions)
- [ ] Slither Left/Right animations tested and working
- [ ] Scene has proper NavMesh baked for snake navigation

### Out of Scope

- **Tune 4 (Freeze) functionality** — Code exists but not working. Deferred to Phase 3+ (too complex for bug-fix milestone)
- **3 Areas (Tutorial/Main/Finale)** — Only 1 area needed for v0.3 stability
- **Backend API integration** — Not blocking gameplay stability
- **Audio system** — Phase 3 polish work
- **Visual effects (particles, screen shake)** — Phase 3 polish work
- **UI polish (animated health bar)** — Phase 3 polish work

## Context

**Project Type:** Academic solo project (PIP-3 Theme B)
**Engine:** Unity 2022 LTS with URP
**Duration:** ~1 month (Feb 3 - Mar 3, 2026)
**Target:** Windows, 60 FPS, 1920x1080 + ultrawide

**Phase 2 Completion:** Feb 15, 2026 (Session 17)
- 9 commits on feature/enemy-setup branch
- 4 critical bugs fixed (IsDazed parameter, attack cooldown, die animation loop, Tune 4 unlock)
- SnakeAI v1.7.2 complete with 7-state machine
- All documentation updated

**Current Branch:** feature/enemy-setup (ready for merge after NavMesh migration)

**Known Technical Debt:**
- Custom SnakeAI movement system fragile (8 collision fix revisions)
- Patrol animation triggered by boolean instead of velocity
- Player CharacterController floating on spawn (FIXED in commit aad1aac)

**Teacher Feedback:**
- "Use NavMeshAgent + NavMeshObstacle instead of custom movement solution"
- "Player should find ground at start and stay at ground level" (ADDRESSED)

## Constraints

- **Timeline:** v0.3 should complete quickly (2-3 days) - it's a bug-fix milestone, not feature development
- **Academic:** Must meet PIP-3 Theme B requirements (core mechanic, 2 modes, database stats)
- **Platform:** School laptops - stable 60 FPS required
- **Tech Stack:** Unity New Input System (NEVER legacy Input), Cinemachine v3.x, URP
- **Git Workflow:** Feature branches from main, atomic commits, proper documentation

## Key Decisions

| Decision | Rationale | Outcome |
|----------|-----------|---------|
| Genshin-style slider (ADR-008) | Hold & release feels more engaging than button mashing | ✓ Good - core mechanic works well |
| First-person with visible body | Immersion + allows full character animations | ✓ Good - Pirate character setup complete |
| Cinemachine v3.x for camera | Industry standard, handles FP camera smoothly | ✓ Good - PlayerController v1.7+ stable |
| Material Emission for visual feedback | Simple, works with URP, no particle complexity | ✓ Good - snakes glow in state colors |
| NavMesh migration for snakes | Fix animation jump bug, teacher-approved approach | — Pending (v0.3) |
| Defer Tune 4 debugging | Too complex for bug-fix milestone, not blocking | — Pending (Phase 3+) |

---
*Last updated: 2026-02-16 after milestone v0.3 initialization*
