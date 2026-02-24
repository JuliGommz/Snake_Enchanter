# Snake Enchanter

## What This Is

A Unity 3D timing-based puzzle game where a wounded warrior trapped in a collapsed ancient ruin uses magical melodies to command snakes. Players navigate a cave system, collecting spell scrolls and precisely timing slider inputs to cast spells that move, daze, or shield against snakes while racing against continuously depleting health. The core mechanic is a Genshin-style hold-and-release slider system where timing determines success or failure.

## Core Value

Precise timing gameplay that feels rewarding when mastered and punishing when failed. Every slider release must feel consequential - success extends survival, mistakes cost health.

## Current Milestone: v1.0 MVP Submission

**Goal:** Ship a playable MVP for PIP-3 Theme B academic submission by ~Mar 3, 2026. Cave system rebuilt from scratch (simplified), enemy and spell systems wired, minimal menu + backend + audio.

**Restructured 2026-02-24:** Old Phase 7-13 plan replaced. Cave must be rebuilt first (new layout), then everything wired on top.

**Target features (MUST):**
- Cave Rebuild (new simplified layout from scratch)
- NavMesh + Snake placement in new cave
- Spell System editor wiring (code already complete)
- Menu Scene with mode selection + Win screen
- Backend API + Leaderboard (academic requirement: database stats)
- Minimal audio (3 flute melodies + cave ambient)
- Windows build + submission prep

**Cut for MVP:**
- Jump mechanic
- Dynamic Slider Balancing
- Visual polish (damage flash, vignette, light fixes)
- Story/narrative intro
- MiniMap, Second Enemy, Particle Glow
- Snake SFX, UI feedback sounds
- SerializeField tooltip translation

## Requirements

### Validated

<!-- Phase 1 - Shipped v0.1 -->

- ✓ Player can move with WASD + mouse look (first-person) — v0.1
- ✓ Player can crouch with Ctrl — v0.1
- ✓ Player has passively draining health bar — v0.1
- ✓ Player wins by reaching exit with HP > 0 — v0.1
- ✓ Player loses when HP reaches 0 — v0.1

<!-- Phase 2 - Shipped v0.2 -->

- ✓ Player can cast tunes with hold-and-release slider timing — v0.2
- ✓ Snake AI patrols, detects player, and responds to spells — v0.2
- ✓ Snakes can attack player at range (Bite/Breath/Projectile) — v0.2
- ✓ Tune 1 (Move): Snake moves away to MoveAwayTarget — v0.2
- ✓ Tune 2 (Daze): Snake collapses for 8 seconds — v0.2
- ✓ Successful spell casts restore player HP — v0.2
- ✓ Failed spell casts trigger snake attack — v0.2
- ✓ Simple and Advanced modes selectable — v0.2

<!-- v0.3 - Bug Fixes & Stability -->

- ✓ Snakes use NavMesh pathfinding — v0.3
- ✓ Player spawns fully grounded — v0.3

<!-- Phase 7 code - Spell System (code complete, editor wiring pending) -->

- ✓ Spell system code: 3 tunes (Move, Daze, Shield), scroll pickup, unlock system, HUD controller, shield component — code v0.3+

### Active

<!-- v1.0 MVP - Restructured 2026-02-24 -->

- [ ] Cave rebuilt with simplified layout (new from scratch)
- [ ] NavMesh baked on new cave for snake navigation
- [ ] Snakes placed in new cave with patrol routes
- [ ] Spell scrolls placed in cave (3 locations)
- [ ] All spell system editor wiring complete (HUD, shield, unlock panels)
- [ ] Menu Scene with mode selection + start game
- [ ] Win screen with stats + fade transition
- [ ] Backend API (POST session, GET leaderboard, GET stats)
- [ ] Flute melodies play during tune cast (3 existing MP3s)
- [ ] Cave ambient music loop
- [ ] Windows .exe build + ZIP package
- [ ] Stable 60 FPS on school laptops

### Out of Scope

- **Two-Level Success System** — Too complex, discarded
- **Tune 3 Attack (creature targeting)** — Removed, needs creature system
- **Tune 4 Freeze** — Removed, overlaps Daze
- **Jump mechanic** — Cut for MVP timeline
- **Dynamic Slider Balancing** — Cut for MVP timeline
- **Visual polish (damage flash, vignette, light fixes)** — Cut for MVP timeline
- **Story/narrative intro** — Cut for MVP timeline
- **MiniMap** — Cut for MVP timeline
- **Second Enemy (RobotKyle)** — Cut for MVP timeline
- **Snake SFX** — Cut for MVP timeline
- **UI feedback sounds** — Cut for MVP timeline
- **Particle Glow System** — Cut for MVP timeline
- **Arm clip fix** — Cut for MVP timeline
- **SerializeField tooltip translation** — Cut for MVP timeline

## Context

**Project Type:** Academic solo project (PIP-3 Theme B)
**Engine:** Unity 6 (6000.0.62f1) with URP
**Duration:** ~1 month (Feb 3 - Mar 3, 2026)
**Target:** Windows, 60 FPS, 1920x1080 + ultrawide
**Deadline:** ~2026-03-03 (Finale Abgabe)
**Presentation:** 2026-02-24 (Fortschritts-Prasentation 2 — today)

**v0.3 Completion:** Feb 18, 2026
- NavMesh migration complete (SnakeAI v1.7.2 → v1.8.5)
- Spell system code complete (3 tunes, all scripts)
- Player ground detection fixed

**Cave Rebuild Context (2026-02-24):**
- Old cave layout too complex and broken
- Rebuilding from scratch with simplified structure
- 211 uncommitted changes on `feature/cave-rebuild` branch
- All scene-dependent work (scrolls, snakes, NavMesh) must be redone

**Known Technical Debt:**
- FindObjectsByType O(n) scan per tune event — acceptable for MVP
- Animation centralization (split across HealthSystem, TuneController, SnakeAI)
- German/English mix in SerializeField tooltips — deferred

**Teacher Feedback:**
- "Use NavMeshAgent + NavMeshObstacle" (ADDRESSED v0.3)
- "Player should find ground at start" (ADDRESSED v0.3)

## Constraints

- **Timeline:** ~7 days remaining (Feb 24 - Mar 3, 2026), MVP target 2-3 days
- **Academic:** Must meet PIP-3 Theme B requirements (core mechanic, 2 modes, database stats)
- **Platform:** School laptops - stable 60 FPS required
- **Tech Stack:** Unity New Input System (NEVER legacy Input), Cinemachine v3.x, URP
- **Git Workflow:** Feature branches from main, atomic commits

## Key Decisions

| Decision | Rationale | Outcome |
|----------|-----------|---------|
| Genshin-style slider (ADR-008) | Hold & release feels more engaging than button mashing | ✓ Good - core mechanic works well |
| First-person with visible body | Immersion + allows full character animations | ✓ Good - Pirate character setup complete |
| Cinemachine v3.x for camera | Industry standard, handles FP camera smoothly | ✓ Good - PlayerController v1.8 stable |
| Material Emission for visual feedback | Simple, works with URP, no particle complexity | ✓ Good - snakes glow in state colors |
| NavMesh migration for snakes | Fix animation jump bug, teacher-approved approach | ✓ Good - v0.3 complete, patrol smooth |
| 3 tunes not 4 | Attack needs creature system, Freeze overlaps Daze | ✓ Good - scope realistic |
| Scroll permanent unlock | Zelda-style item progression, simpler than consumables | — Pending |
| Cave rebuild from scratch | Old layout too complex/broken, simplify for MVP | — Pending |
| Cut SHOULD/COULD for MVP | 2-3 day timeline, focus on academic requirements only | — Pending |

---
*Last updated: 2026-02-24 after v1.0 MVP restructure*
