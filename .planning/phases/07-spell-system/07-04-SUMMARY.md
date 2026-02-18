---
phase: 07-spell-system
plan: 04
subsystem: tune-system
tags: [unity, csharp, spell-system, cooldown, range-check, charges, heal-on-charm, shield, hud, events]

# Dependency graph
requires:
  - phase: 07-02
    provides: TuneController v3.0 with 3-tune array, unlock gate, SpellHUDController
  - phase: 07-03
    provides: ShieldComponent.ActivateShield(), IsShieldActive, TryAbsorbAttack()

provides:
  - TuneController v3.1 — range gating (Move/Daze), cooldown (all), charges (Advanced), Shield wiring, TuneSuccessWithId only for tunes 1-2
  - HealthSystem v1.5 — heals via OnSnakeCharmed instead of OnTuneSuccess
  - SnakeAI v1.9 — fires SnakeCharmed event on Move/Daze, Attack/Freeze dead code removed
  - SpellHUDController v1.1 — cooldown overlay per slot, range indicator on Move/Daze slots

affects:
  - HUD wiring in Unity Editor (Inspector setup for CooldownOverlay child in SlotPrefab)
  - Any future system subscribing to OnSnakeCharmed vs OnTuneSuccess

# Tech tracking
tech-stack:
  added: []
  patterns:
    - "Cooldown timer array: float[] _cooldownTimers ticked in Update, fires TuneCooldownStarted/Expired"
    - "Range polling: HasSnakeInRange via Physics.OverlapSphere in Update, debounced by _lastSnakeInRange"
    - "SnakeCharmed as heal trigger: SnakeAI fires event AFTER applying state — heal and state transition are coupled"
    - "Coroutine guard pattern: StopCoroutine before reassigning for CooldownTickCoroutine"
    - "TuneSuccessWithId conditional: only fires for tuneNumber <= 2 (snake-targeting tunes)"

key-files:
  created: []
  modified:
    - Assets/_Project/Scripts/TuneSystem/TuneController.cs
    - Assets/_Project/Scripts/Player/HealthSystem.cs
    - Assets/_Project/Scripts/Snakes/SnakeAI.cs
    - Assets/_Project/Scripts/UI/SpellHUDController.cs

key-decisions:
  - "OverlapSphere for range check (not FindObjectsByType) — simpler, cheaper for low snake counts"
  - "Range polling in TuneController.Update() not SnakeAI.Update() — single poll source, not per-snake"
  - "CooldownTickCoroutine uses Time.deltaTime (not unscaled) — cooldown should pause with game like shield timer"
  - "Range indicator uses Color.Lerp with _rangeHighlightAlpha=0.3 — subtle visual, not jarring"
  - "SnakeCharmed fires AFTER SetState() — ensures state is applied before heal event propagates"
  - "TuneSuccessWithId guard: if (tuneNumber <= 2) — Shield never fires snake-targeting event"
  - "Advanced mode charges initialized in Awake AND in OnTuneUnlockedEvent — lazy init per-tune as unlocked"

patterns-established:
  - "Multi-guard pattern in OnTuneKeyPressed: unlock → cooldown → charge → range → shield-recast (order matters)"
  - "Event-driven HUD updates: cooldown and range indicator fully driven by GameEvents subscriptions"

# Metrics
duration: ~9min (auto tasks only — Task 4 is human-verify checkpoint)
completed: 2026-02-18
---

# Phase 7 Plan 04: Spell Casting Rules + HUD Polish Summary

**Range-gated Move/Daze casting, per-spell cooldowns, Advanced-mode charge depletion, and heal-on-charm-only wired through TuneController, SnakeAI, HealthSystem, and SpellHUDController**

## Performance

- **Duration:** ~9 min (Tasks 1-3 auto tasks; Task 4 is human-verify checkpoint, pending)
- **Started:** 2026-02-18T12:17:27Z
- **Completed:** 2026-02-18T12:26:21Z (auto tasks)
- **Tasks:** 3 of 4 complete (Task 4 = human verification checkpoint)
- **Files modified:** 4

## Accomplishments

- TuneController v3.1: 5-guard system in OnTuneKeyPressed (unlock, cooldown, charges, range, shield-recast), cooldown timers tick in Update, HasSnakeInRange via OverlapSphere, TuneSuccessWithId only fires for snake-targeting tunes (1 and 2), Shield activation on Tune 3 success
- HealthSystem v1.5: OnTuneSuccess subscription removed, OnSnakeCharmed subscription added, heals only for tuneNumber 1-2 (Move/Daze) — Shield casts and empty-range casts never heal
- SnakeAI v1.9: GameEvents.SnakeCharmed(1/2) fires in ApplyTuneEffect after SetState, removed SnakeState.AttackingEnemy + Frozen + all related fields and methods (StartAttackingEnemy, FindNearestCreature, NeutralizeAfterAttack, ApplyFreeze, _freezeDuration, _frozenColor)
- SpellHUDController v1.1: Subscribes to OnTuneCooldownStarted/Expired/OnSnakeInRangeChanged, CooldownTickCoroutine drains radial fill Image, range indicator Color.Lerps Move/Daze backgrounds, Shield slot unaffected

## Task Commits

Each task was committed atomically:

1. **Task 1: Add range check, cooldown, charges, Shield wiring to TuneController** - `c482e2d` (feat)
2. **Task 2a: Fix heal-on-charm in HealthSystem** - `cbe9151` (feat)
3. **Task 2b: SnakeAI fire SnakeCharmed + remove Attack/Freeze dead code** - `12c1b03` (feat)
4. **Task 3: Add cooldown + range indicator to SpellHUDController** - `f139ff4` (feat)

**Task 4 (checkpoint:human-verify):** Pending user verification in Unity Editor

## Files Created/Modified

- `Assets/_Project/Scripts/TuneSystem/TuneController.cs` — v3.1: 5 casting guards, cooldown timers, charges, HasSnakeInRange, Shield activation, conditional TuneSuccessWithId
- `Assets/_Project/Scripts/Player/HealthSystem.cs` — v1.5: OnSnakeCharmed subscription replaces OnTuneSuccess, heal-on-charm only for tuneNumber 1-2
- `Assets/_Project/Scripts/Snakes/SnakeAI.cs` — v1.9: SnakeCharmed event fired in ApplyTuneEffect, removed AttackingEnemy + Frozen states and all related dead code
- `Assets/_Project/Scripts/UI/SpellHUDController.cs` — v1.1: cooldown overlay coroutine, range indicator via background color lerp

## Decisions Made

- `Physics.OverlapSphere` for range check (not `FindObjectsByType`) — single poll per frame from TuneController, not per-snake
- `CooldownTickCoroutine` uses `Time.deltaTime` (not unscaled) — consistent with ShieldTimerCoroutine, pauses with game
- Range indicator uses `Color.Lerp` with configurable alpha (`_rangeHighlightAlpha = 0.3f`) — subtle, Inspector-tunable
- `GameEvents.SnakeCharmed` fires AFTER `SetState()` — state applied before subscribers run
- Advanced mode charge array initialized in both `Awake()` (for initial mode) and `OnTuneUnlockedEvent()` (lazy per-tune unlock)

## Deviations from Plan

None — plan executed exactly as written.

## Issues Encountered

None.

## User Setup Required

For Task 4 verification, the following Inspector setup is required before entering Play Mode:

1. Attach `ShieldComponent` to Player GameObject (if not already done from 07-03)
2. Add `CooldownOverlay` Image child to SlotPrefab in Unity Editor:
   - Set Image Type = Filled, Fill Method = Radial 360, Fill Origin = Top
   - Set color to semi-transparent dark (e.g., black at 0.6 alpha)
   - Disable GameObject by default (SpellHUDController.Awake controls visibility)
3. Assign `ShieldComponent.BorderGlowImage` to a fullscreen overlay Image on Screen Space canvas
4. Assign `TuneController._snakeLayerMask` (optional — default layer works fine)

## Next Phase Readiness

- Full spell system code complete: scroll unlock, 3 tunes, Shield lifecycle, cooldown, charges, heal-on-charm
- Human verification (Task 4) is the final gate before Phase 7 is marked done
- No code blockers for subsequent phases

## Self-Check: PASSED

Files verified present:
- `Assets/_Project/Scripts/TuneSystem/TuneController.cs` — FOUND
- `Assets/_Project/Scripts/Player/HealthSystem.cs` — FOUND
- `Assets/_Project/Scripts/Snakes/SnakeAI.cs` — FOUND
- `Assets/_Project/Scripts/UI/SpellHUDController.cs` — FOUND

Commits verified:
- `c482e2d` — FOUND (feat(07-04): add range check, cooldown, charges, Shield wiring to TuneController v3.1)
- `cbe9151` — FOUND (feat(07-04): fix heal-on-charm in HealthSystem v1.5)
- `12c1b03` — FOUND (feat(07-04): fire SnakeCharmed event + remove Attack/Freeze dead code in SnakeAI v1.9)
- `f139ff4` — FOUND (feat(07-04): add cooldown overlay + range indicator to SpellHUDController v1.1)

---
*Phase: 07-spell-system*
*Completed: 2026-02-18 (auto tasks — human-verify checkpoint pending)*
