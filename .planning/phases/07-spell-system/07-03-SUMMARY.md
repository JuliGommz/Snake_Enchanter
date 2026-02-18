---
phase: 07-spell-system
plan: 03
subsystem: player
tags: [unity, csharp, shield, coroutine, events, ui, health-system]

# Dependency graph
requires:
  - phase: 07-01
    provides: GameEvents.ShieldActivated, ShieldDeactivated, ShieldAbsorbedAttack events and invokers
provides:
  - ShieldComponent.cs — 8s shield state machine with activate, absorb, expiry lifecycle and screen-edge glow
  - HealthSystem.cs v1.4 — shield intercept in TakeSnakeAttack before damage
affects:
  - 07-04 (TuneController calls ShieldComponent.ActivateShield() on Tune 3 success)
  - 07-04 (HUD subscribes to OnShieldActivated/Deactivated for glow overlay toggle)

# Tech tracking
tech-stack:
  added: []
  patterns:
    - Optional component pattern — ShieldComponent cached via GetComponent in Awake, null guard in TakeSnakeAttack ensures game works without it
    - Single-absorb shield — TryAbsorbAttack() calls DeactivateShield(absorbed:true) before returning true, preventing double absorb
    - Coroutine timer stopped before reassignment — StopCoroutine in DeactivateShield prevents ghost coroutines on early absorb

key-files:
  created:
    - Assets/_Project/Scripts/Player/ShieldComponent.cs
  modified:
    - Assets/_Project/Scripts/Player/HealthSystem.cs

key-decisions:
  - "WaitForSeconds (not WaitForSecondsRealtime) in ShieldTimerCoroutine — shield timer should pause with game (timeScale=0), unlike SpellUnlockSystem which needed realtime"
  - "AbsorbFlashCoroutine handles glow hide on absorb — DeactivateShield skips glow hide when absorbed=true to avoid race with flash coroutine"
  - "ShieldComponent is optional (no warning if null in HealthSystem) — game is fully functional without the shield component attached"

patterns-established:
  - "Single-absorb guard: DeactivateShield called before firing AbsorbedAttack event so _isShieldActive is false before any subscriber runs"
  - "Screen glow lifecycle: SetActive(false) in Awake, SetActive(true) in Activate, SetActive(false) in expire or after flash"

# Metrics
duration: 10min
completed: 2026-02-18
---

# Phase 7 Plan 03: Shield Component Summary

**ShieldComponent state machine (8s timer, single-absorb, screen-edge glow) wired into HealthSystem.TakeSnakeAttack() as optional shield intercept before damage**

## Performance

- **Duration:** ~10 min
- **Started:** 2026-02-18T12:12:19Z
- **Completed:** 2026-02-18T12:22:00Z
- **Tasks:** 2
- **Files modified:** 2 (1 created, 1 modified)

## Accomplishments

- ShieldComponent.cs: full shield lifecycle — ActivateShield() with recast guard, ShieldTimerCoroutine (8s WaitForSeconds), TryAbsorbAttack() returning bool, DeactivateShield(absorbed) with coroutine stop, AbsorbFlashCoroutine (0.15s white flash)
- Screen edge glow via `UnityEngine.UI.Image`: active color on shield up, white flash on absorb then hide, clean hide on natural expiry
- HealthSystem.cs v1.4: single-line shield intercept in TakeSnakeAttack() — null-safe, zero impact on existing heal logic

## Task Commits

Each task was committed atomically:

1. **Task 1: Create ShieldComponent.cs with full shield lifecycle** - `245d489` (feat)
2. **Task 2: Add shield intercept to HealthSystem.TakeSnakeAttack()** - `dd8a818` (feat)

**Plan metadata:** (this commit — docs)

## Files Created/Modified

- `Assets/_Project/Scripts/Player/ShieldComponent.cs` — New file: MonoBehaviour on Player with ActivateShield(), TryAbsorbAttack(), screen-edge glow via UI Image, fires ShieldActivated/Deactivated/AbsorbedAttack events
- `Assets/_Project/Scripts/Player/HealthSystem.cs` — v1.4: added `_shieldComponent` field, GetComponent in Awake, shield intercept in TakeSnakeAttack() before TakeDamage()

## Decisions Made

- `WaitForSeconds` (not `WaitForSecondsRealtime`) in `ShieldTimerCoroutine` — the shield timer should pause alongside the game when `timeScale = 0`. This is the opposite of `SpellUnlockSystem` which needed realtime behavior.
- `AbsorbFlashCoroutine` handles glow hide in the absorb path — `DeactivateShield(absorbed: true)` skips `SetActive(false)` so the flash coroutine can control the glow independently, avoiding a race condition.
- ShieldComponent is optional in HealthSystem — no warning logged if null, ensuring scenes without a shield still work without console noise.

## Deviations from Plan

None — plan executed exactly as written.

## Issues Encountered

None.

## User Setup Required

None — no external service configuration required.

After wiring up in Unity (07-04 will do this via TuneController):
1. Attach `ShieldComponent` to the Player GameObject (same object as HealthSystem).
2. Create a full-screen UI Image with a border/vignette sprite on a Screen Space - Overlay Canvas.
3. Assign the Image reference to `Border Glow Image` in ShieldComponent Inspector.
4. The Image's GameObject should be **disabled by default** — ShieldComponent.Awake() enforces this.

## Next Phase Readiness

- ShieldComponent public API is ready: `ActivateShield()` and `TryAbsorbAttack()`.
- HealthSystem intercept is live — any attached ShieldComponent will block the next snake attack automatically.
- 07-04 (TuneController) can call `ShieldComponent.ActivateShield()` directly on Tune 3 success.
- 07-04 (HUD) can subscribe to `GameEvents.OnShieldActivated` and `OnShieldDeactivated` for HUD overlay state.
- No blockers.

## Self-Check: PASSED

Files verified present:
- `Assets/_Project/Scripts/Player/ShieldComponent.cs` — FOUND
- `Assets/_Project/Scripts/Player/HealthSystem.cs` — FOUND

Commits verified:
- `245d489` — FOUND (feat(07-03): create ShieldComponent with full shield lifecycle)
- `dd8a818` — FOUND (feat(07-03): add shield intercept to HealthSystem.TakeSnakeAttack)

---
*Phase: 07-spell-system*
*Completed: 2026-02-18*
