# Snake Enchanter - Project State

## Project Reference

See: `.planning/PROJECT.md` (updated 2026-02-18)

**Core value:** Precise timing gameplay that feels rewarding when mastered and punishing when failed
**Current focus:** Phase 7 — Spell System (07-04 auto tasks done, Task 4 human-verify checkpoint pending)

## Current Position

Phase: 7 of 13 (Spell System)
Plan: 4 of 4 in current phase (07-01, 07-02, 07-03 complete; 07-04 auto tasks complete, awaiting human verify)
Status: Checkpoint — awaiting human verification in Unity Editor
Last activity: 2026-02-18 — 07-04 auto tasks: range check, cooldown, charges, heal-on-charm, HUD polish

Progress: [###░░░░░░░] 30% (v1.0 phases, 3/10 executable plans done — 07-04 counts when checkpoint clears)

## Performance Metrics

**Velocity:**
- Total plans completed: 3 (v1.0) + 07-04 auto tasks done
- Average duration: ~10 min
- Total execution time: ~44 min

**By Phase:**

| Phase | Plans | Total | Avg/Plan |
|-------|-------|-------|----------|
| 07-spell-system | 3/4 done (4th at checkpoint) | ~44 min | ~11 min |

*Updated after each plan completion*

## Accumulated Context

### Decisions

Recent decisions affecting current work:

- Scroll permanent unlock (Zelda-style) — simpler than consumables, chosen for v1.0
- **3 tunes (not 4)**: Move, Daze, Shield. Attack removed (needs creature system), Freeze removed (overlaps Daze)
- Tune 3 Shield: 8s duration, blocks next attack, screen-edge glow, no recast while active
- 1 scroll per QuestRoom path, fixed unlock order, HUD grows dynamically from empty
- **Heal-on-charm only**: HP restores only when Move/Daze actually charms a snake. Shield/empty casts = no heal.
- **Range check**: Move/Daze need snake in range. Shield castable anywhere. HUD shows range indicator.
- **Cooldown + Charges**: All spells have cooldown (both modes). Advanced mode adds limited charges per spell (SerializeField).
- EXT-03 (cooldown) and EXT-05 (range) promoted from Phase 12 COULD → Phase 7 MUST
- Phase 12 (EXT) skippable — all COULD features, execute only if time allows after Phase 11
- Submission prep (SUB) is Phase 13, always last
- **No OnMouseDown in pickups** — legacy callback, violates New Input System rule. Use Interact() called by PlayerController raycast.
- **WaitForSecondsRealtime required** when Time.timeScale=0 — WaitForSeconds never resumes at timeScale 0.
- **Instance material for glow** — _renderer.material (instance) not .sharedMaterial — avoids modifying shared asset.
- **WaitForSeconds correct for ShieldTimerCoroutine** — shield timer should pause with game (timeScale=0), opposite of SpellUnlockSystem
- **AbsorbFlashCoroutine owns glow hide on absorb** — DeactivateShield(absorbed:true) skips SetActive(false) to avoid race with flash coroutine
- **ShieldComponent is optional in HealthSystem** — no warning if null, game fully functional without shield attached
- **OverlapSphere for range check** (not FindObjectsByType) — single poll per frame from TuneController, not per-snake
- **CooldownTickCoroutine uses Time.deltaTime** (not unscaled) — consistent with ShieldTimerCoroutine, pauses with game
- **TuneSuccessWithId only fires for tuneNumber <= 2** — Shield never fires snake-targeting event
- **SnakeCharmed fires AFTER SetState()** — state applied before subscribers (HealthSystem) run

### Pending Todos

- Wire up SpellScrollPickup on scroll prefabs in scene (trigger collider + Inspector fields)
- Create SpellUnlockManager GameObject, attach SpellUnlockSystem, assign panel + TMPro labels
- Disable scroll panel UI by default in Hierarchy
- Attach ShieldComponent to Player GameObject, create border/vignette UI Image on Overlay Canvas, assign to ShieldComponent Inspector field
- Attach SpellHUDController to HUD Canvas, assign tune slot prefabs and container
- **Add CooldownOverlay Image child to SlotPrefab** (new for v1.1) — Filled, Radial 360, initially disabled
- Verify 14-point checklist in Task 4 (human-verify checkpoint)

### Blockers/Concerns

- ~~Tune 4 (Freeze) non-functional~~ — RESOLVED: Tune system reduced to 3 tunes (Move, Daze, Shield). Freeze removed.
- FindObjectsByType O(n) scan per tune event — acceptable for now, flag if performance issues appear
- **CURRENT BLOCKER**: Task 4 human-verify checkpoint — user must enter Play Mode and run 14-point checklist

## Session Continuity

Last session: 2026-02-18
Stopped at: 07-04 checkpoint (Tasks 1-3 auto tasks committed c482e2d, cbe9151, 12c1b03, f139ff4). Task 4 = human-verify.
Resume file: .planning/phases/07-spell-system/07-04-SUMMARY.md
