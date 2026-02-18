# Snake Enchanter - Project State

## Project Reference

See: `.planning/PROJECT.md` (updated 2026-02-18)

**Core value:** Precise timing gameplay that feels rewarding when mastered and punishing when failed
**Current focus:** Phase 7 — Spell System (ALL CODE DONE, Unity Editor setup in progress)

## Current Position

Phase: 7 of 13 (Spell System)
Plan: 4 of 4 in current phase — all code committed, Unity Editor manual setup partially done
Status: Inspector setup — user building UI panels, prefabs, and placing scroll objects
Last activity: 2026-02-18 — Editor setup: TuneConfigs assigned, ShieldComponent added, ScrollUnlockPanel built, SpellHUD partially built

Progress: [###░░░░░░░] 30% (v1.0 phases, code for 4/4 plans done — needs Editor setup + play-test verification)

## Performance Metrics

**Velocity:**
- Total plans completed: 4 code plans (v1.0), Editor setup in progress
- Average duration: ~10 min per code plan
- Total execution time: ~44 min code + manual Editor work

**By Phase:**

| Phase | Plans | Total | Avg/Plan |
|-------|-------|-------|----------|
| 07-spell-system | 4/4 code done, Editor setup WIP | ~44 min code | ~11 min |

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

### Pending Todos — Unity Editor Setup

**Done:**
- ✅ TuneConfigs Array zugewiesen (3 SOs: Move, Daze, Shield) auf TuneController
- ✅ ShieldComponent auf Player hinzugefügt
- ✅ ScrollUnlockPanel gebaut (3 TMPro Labels, deaktiviert)
- ✅ SpellUnlockManager erstellt + SpellUnlockSystem zugewiesen
- ✅ ShieldBorderGlow Image erstellt (fullscreen, deaktiviert)
- ✅ ShieldComponent → Border Glow Image zugewiesen
- ✅ SpellSlotsContainer erstellt (HorizontalLayoutGroup)
- ✅ SpellSlotPrefab gebaut (Background, KeyIcon/KeyLabel, SpellName, CooldownOverlay)
- ✅ fix(07): UnlockTune4 aus GameManager + TuneConfigCreator bereinigt

**Noch offen:**
- ☐ SpellSlotPrefab als Prefab-Asset speichern (nach Assets/_Project/Prefabs/UI/)
- ☐ SpellHUDManager erstellen + SpellHUDController zuweisen
- ☐ 3 Scroll-GameObjects in der Cave platzieren (3D, mit Collider + SpellScrollPickup)
- ☐ 14-Punkte Play-Test Verification

### Blockers/Concerns

- ~~Tune 4 (Freeze) non-functional~~ — RESOLVED: Tune system reduced to 3 tunes (Move, Daze, Shield). Freeze removed.
- ~~UnlockTune4 compile error~~ — RESOLVED: Removed from GameManager.cs + TuneConfigCreator.cs updated
- FindObjectsByType O(n) scan per tune event — acceptable for now, flag if performance issues appear

## Session Continuity

Last session: 2026-02-18
Stopped at: Phase 7 Editor setup — SpellSlotPrefab needs to be saved as asset, SpellHUDManager + Scrolls in Cave remaining
Next steps: Complete Editor setup (Teil 5c + Teil 6), then 14-point play-test
Resume file: .planning/phases/07-spell-system/07-04-SUMMARY.md
Git: main branch, last commit df665e1
