# Snake Enchanter

## What This Is

A Unity 3D timing-based puzzle game where a wounded warrior trapped in a collapsed ancient ruin uses magical melodies to command snakes. Players navigate 3 QuestRooms, collecting spell scrolls and precisely timing slider inputs to cast spells that move, daze, or redirect snakes while racing against continuously depleting health. The core mechanic is a Genshin-style hold-and-release slider system where timing determines success or failure.

## Core Value

Precise timing gameplay that feels rewarding when mastered and punishing when failed. Every slider release must feel consequential - success extends survival, mistakes cost health.

## Current State (after v0.3)

**Branch:** `main` (all feature branches merged)
**SnakeAI:** v1.8.5 (NavMesh, submission-clean)
**PlayerController:** v1.8 (ground detection, Cinemachine)

**What works:**
- Player movement (WASD, mouse look, crouch) with Cinemachine FP camera
- 4 Tune slider system (Move, Daze, Attack — Freeze non-functional)
- Snake AI: 7-state machine with NavMesh pathfinding, 3 attack types
- Health system with passive drain + spell restoration
- Win/Lose conditions (ExitTrigger, HP=0)
- 3 QuestRooms in GameLevel scene
- Pirate character with full animation set

**What doesn't work / is missing:**
- Tune 4 (Freeze) — code exists, non-functional
- Spell gathering system — tunes should require scroll collection
- Menu Scene UI — scene exists, no UI
- Backend API — academic requirement, not started
- Audio — no sounds at all
- Visual polish — no particles, screen effects, lighting polish
- Jump — not implemented
- MiniMap — not implemented
- Story/narrative intro — not implemented

## Requirements

### Validated

<!-- Phase 1 - Shipped v0.1 -->

- ✓ Player can move with WASD + mouse look (first-person) — v0.1
- ✓ Player can crouch with Ctrl — v0.1
- ✓ Player has passively draining health bar — v0.1
- ✓ Player wins by reaching exit with HP > 0 — v0.1
- ✓ Player loses when HP reaches 0 — v0.1

<!-- Phase 2 - Shipped v0.2 -->

- ✓ Player can cast 4 tunes with hold-and-release slider timing — v0.2
- ✓ Snake AI patrols, detects player, and responds to spells — v0.2
- ✓ Snakes can attack player at range (Bite/Breath/Projectile) — v0.2
- ✓ Tune 1 (Move): Snake moves away to MoveAwayTarget — v0.2
- ✓ Tune 2 (Daze): Snake collapses for 8 seconds — v0.2
- ✓ Tune 3 (Attack): Snake attacks nearby creature — v0.2
- ✓ Successful spell casts restore player HP — v0.2
- ✓ Failed spell casts trigger snake attack — v0.2
- ✓ Simple and Advanced modes selectable from menu — v0.2

<!-- v0.3 - Bug Fixes & Stability -->

- ✓ Snake patrol animations don't jump/restart — v0.3 (NavMesh migration)
- ✓ Snakes use NavMesh pathfinding instead of custom movement — v0.3
- ✓ Player spawns fully grounded — v0.3
- ✓ Scene has proper NavMesh baked — v0.3

### Active

<!-- Next milestone — to be defined by /gsd:new-milestone -->

(Empty — next milestone will define requirements)

### Out of Scope

- **Two-Level Success System** — Too complex, discarded
- **Slither Left/Right testing** — Not necessary per user

## Context

**Project Type:** Academic solo project (PIP-3 Theme B)
**Engine:** Unity 2022 LTS with URP
**Duration:** ~1 month (Feb 3 - Mar 3, 2026)
**Target:** Windows, 60 FPS, 1920x1080 + ultrawide
**Deadline:** ~2026-03-03 (Finale Abgabe)
**Presentation:** ~2026-02-24 (Fortschritts-Prasentation 2)

**v0.3 Completion:** Feb 18, 2026
- NavMesh migration complete (SnakeAI v1.7.2 → v1.8.5)
- Both feature branches merged to main
- Player ground detection fixed
- Patrol animation bug eliminated

**Known Technical Debt:**
- Tune 4 (Freeze) non-functional — needs debugging
- FindObjectsByType O(n) scan per tune event
- Animation centralization (split across HealthSystem, TuneController, SnakeAI)
- German/English mix in SerializeField tooltips

**Teacher Feedback:**
- "Use NavMeshAgent + NavMeshObstacle instead of custom movement solution" (ADDRESSED v0.3)
- "Player should find ground at start and stay at ground level" (ADDRESSED v0.3)

## Constraints

- **Timeline:** ~2 weeks remaining (Feb 18 - Mar 3, 2026)
- **Academic:** Must meet PIP-3 Theme B requirements (core mechanic, 2 modes, database stats)
- **Platform:** School laptops - stable 60 FPS required
- **Tech Stack:** Unity New Input System (NEVER legacy Input), Cinemachine v3.x, URP
- **Git Workflow:** Feature branches from main, atomic commits, proper documentation

## Key Decisions

| Decision | Rationale | Outcome |
|----------|-----------|---------|
| Genshin-style slider (ADR-008) | Hold & release feels more engaging than button mashing | ✓ Good - core mechanic works well |
| First-person with visible body | Immersion + allows full character animations | ✓ Good - Pirate character setup complete |
| Cinemachine v3.x for camera | Industry standard, handles FP camera smoothly | ✓ Good - PlayerController v1.8 stable |
| Material Emission for visual feedback | Simple, works with URP, no particle complexity | ✓ Good - snakes glow in state colors |
| NavMesh migration for snakes | Fix animation jump bug, teacher-approved approach | ✓ Good - v0.3 complete, patrol smooth |
| Defer Tune 4 debugging | Too complex for bug-fix milestone, not blocking | — Pending (next milestone) |
| Discard Two-Level Success System | Too complex for timeline | ✓ Good - keeps scope realistic |
| 3 QuestRooms in 1 scene | Simpler than 3 separate scenes | ✓ Good - already implemented |

---
*Last updated: 2026-02-18 after v0.3 milestone completion*
