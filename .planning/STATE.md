# Snake Enchanter - Project State

## Project Reference

See: `.planning/PROJECT.md` (updated 2026-02-18)

**Core value:** Precise timing gameplay that feels rewarding when mastered and punishing when failed
**Current focus:** Phase 7 — Spell System (07-04 next)

## Current Position

Phase: 7 of 13 (Spell System)
Plan: 4 of 4 in current phase (07-01, 07-02, 07-03 complete)
Status: Executing
Last activity: 2026-02-18 — 07-03 complete: ShieldComponent + HealthSystem shield intercept

Progress: [###░░░░░░░] 30% (v1.0 phases, 3/10 executable plans done)

## Performance Metrics

**Velocity:**
- Total plans completed: 3 (v1.0)
- Average duration: ~12 min
- Total execution time: ~35 min

**By Phase:**

| Phase | Plans | Total | Avg/Plan |
|-------|-------|-------|----------|
| 07-spell-system | 3/4 done | ~35 min | ~12 min |

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

### Pending Todos

- Wire up SpellScrollPickup on scroll prefabs in scene (trigger collider + Inspector fields)
- Create SpellUnlockManager GameObject, attach SpellUnlockSystem, assign panel + TMPro labels
- Disable scroll panel UI by default in Hierarchy
- Attach ShieldComponent to Player GameObject, create border/vignette UI Image on Overlay Canvas, assign to ShieldComponent Inspector field
- Attach SpellHUDController to HUD Canvas, assign tune slot prefabs and container

### Blockers/Concerns

- ~~Tune 4 (Freeze) non-functional~~ — RESOLVED: Tune system reduced to 3 tunes (Move, Daze, Shield). Freeze removed.
- FindObjectsByType O(n) scan per tune event — acceptable for now, flag if performance issues appear

## Session Continuity

Last session: 2026-02-18
Stopped at: 07-02 complete (executed after 07-03 — TuneController v3.0 + SpellHUDController committed e620b6d, c117d0c). Next: 07-04
Resume file: .planning/phases/07-spell-system/07-02-SUMMARY.md
