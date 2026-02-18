---
phase: 07-spell-system
plan: 01
subsystem: spell-system
tags: [unity, csharp, events, scriptableobject, new-input-system, tmpro, cinemachine]

# Dependency graph
requires:
  - phase: existing-core
    provides: GameEvents.cs static event hub and TuneConfig.cs ScriptableObject pattern
provides:
  - 9 new Phase 7 spell system events in GameEvents.cs (scroll, shield, cooldown, range)
  - SnakeEffect enum updated to Move/Daze/Shield (Attack and Freeze removed)
  - SpellScrollPickup.cs — walk-over + raycast collection with proximity emission glow
  - SpellUnlockSystem.cs — Zelda-style pause panel with any-key dismiss and timeScale control
affects:
  - 07-02 (TuneController will fire OnTuneCooldownStarted/Expired, OnSnakeCharmed)
  - 07-03 (Shield system subscribes to OnShieldActivated/Deactivated/AbsorbedAttack)
  - 07-04 (HUD subscribes to OnTuneUnlocked, OnSnakeInRangeChanged, OnTuneCooldownStarted)

# Tech tracking
tech-stack:
  added: []
  patterns:
    - Static event hub with region-separated events and one-liner invokers (GameEvents pattern)
    - Instance material for per-object emission glow (avoids modifying shared asset)
    - WaitForSecondsRealtime for coroutines at Time.timeScale = 0
    - Interact() public method pattern for New Input System raycast compatibility

key-files:
  created:
    - Assets/_Project/Scripts/Level/SpellScrollPickup.cs
    - Assets/_Project/Scripts/TuneSystem/SpellUnlockSystem.cs
  modified:
    - Assets/_Project/Scripts/Core/GameEvents.cs
    - Assets/_Project/Scripts/TuneSystem/TuneConfig.cs

key-decisions:
  - "No OnMouseDown in SpellScrollPickup — legacy Unity callback, violates New Input System rule. Uses public Interact() called by PlayerController raycast instead."
  - "WaitForSecondsRealtime (not WaitForSeconds) in SpellUnlockSystem — required when Time.timeScale=0 or coroutine never resumes"
  - "Instance material (_renderer.material) for glow — avoids modifying shared material asset in project"
  - "SpellScrollPickup uses GetComponentInChildren<Renderer>() fallback — scroll mesh may be a child GameObject"

patterns-established:
  - "Event invokers: public static void EventName(params) => OnEventName?.Invoke(params) — one-liner, no null check ceremony"
  - "Emission glow: cache Shader.PropertyToID as static readonly, use _renderer.material (instance), EnableKeyword _EMISSION"
  - "Pickup collection guard: bool _collected field, check at top of all methods, set true before SetActive(false)"

# Metrics
duration: 15min
completed: 2026-02-18
---

# Phase 7 Plan 01: Spell System Foundation Summary

**Static event hub extended with 9 spell system events, SnakeEffect enum reduced to Move/Daze/Shield, and two new scripts (SpellScrollPickup + SpellUnlockSystem) providing Zelda-style scroll collection with proximity glow and time-pausing unlock panel**

## Performance

- **Duration:** ~15 min
- **Started:** 2026-02-18T12:07:58Z
- **Completed:** 2026-02-18T12:23:00Z
- **Tasks:** 2
- **Files modified:** 4

## Accomplishments

- GameEvents.cs v1.2: 9 new spell system events with invokers and ClearAllEvents cleanup — zero existing events modified
- TuneConfig.cs SnakeEffect enum simplified from 4 values (Move/Daze/Attack/Freeze) to 3 (Move/Daze/Shield)
- SpellScrollPickup.cs: walk-over trigger, public Interact() for raycast collection, and distance-based emission glow
- SpellUnlockSystem.cs: subscribes to OnScrollCollected, pauses game (timeScale=0), shows TMPro panel, waits for real-time any key

## Task Commits

Each task was committed atomically:

1. **Task 1: Add Phase 7 events to GameEvents.cs + update SnakeEffect enum** - `b40d554` (feat)
2. **Task 2: Create SpellScrollPickup.cs and SpellUnlockSystem.cs** - `d46844f` (feat)

**Plan metadata:** (this commit — docs)

## Files Created/Modified

- `Assets/_Project/Scripts/Core/GameEvents.cs` — v1.2: 9 new spell system events (OnScrollCollected, OnTuneUnlocked, OnShieldActivated, OnShieldDeactivated, OnShieldAbsorbedAttack, OnSnakeCharmed, OnTuneCooldownStarted, OnTuneCooldownExpired, OnSnakeInRangeChanged) with invokers and cleanup
- `Assets/_Project/Scripts/TuneSystem/TuneConfig.cs` — SnakeEffect enum: Move/Daze/Shield (Attack+Freeze removed), keyNumber Range 1-3
- `Assets/_Project/Scripts/Level/SpellScrollPickup.cs` — New file: walk-over trigger, Interact() raycast hook, proximity emission glow
- `Assets/_Project/Scripts/TuneSystem/SpellUnlockSystem.cs` — New file: Zelda-style pause panel, timeScale control, WaitForSecondsRealtime dismiss

## Decisions Made

- No `OnMouseDown` in SpellScrollPickup — it is a legacy Unity callback that violates the project's New Input System rule. Used `Interact()` public method called by PlayerController raycast instead.
- `WaitForSecondsRealtime` is mandatory in SpellUnlockSystem — `WaitForSeconds` would never resume when `Time.timeScale = 0`.
- Instance material (`_renderer.material`) used for glow — modifying shared material would affect all prefab instances.

## Deviations from Plan

None — plan executed exactly as written.

## Issues Encountered

None.

## User Setup Required

None — no external service configuration required.

After wiring up in Unity:
1. Attach `SpellScrollPickup` to each scroll prefab. Set IsTrigger=true on its Collider. Assign tune number, name, description in Inspector.
2. Create a `SpellUnlockManager` GameObject in the scene. Attach `SpellUnlockSystem`. Assign the scroll panel UI root and TMPro label references in Inspector.
3. The scroll panel UI GameObject should be **disabled by default** in the Hierarchy.

## Next Phase Readiness

- Event infrastructure is complete. Plans 07-02, 07-03, 07-04 can consume all new events immediately.
- TuneController (07-02) should now fire `GameEvents.TuneCooldownStarted/Expired` and `GameEvents.SnakeCharmed` instead of raw `TuneSuccess`.
- Shield system (07-03) can subscribe to `OnShieldActivated/Deactivated/AbsorbedAttack`.
- HUD (07-04) can subscribe to `OnTuneUnlocked` and `OnSnakeInRangeChanged` for dynamic HUD growth.
- No blockers.

## Self-Check: PASSED

Files verified present:
- `Assets/_Project/Scripts/Core/GameEvents.cs` — FOUND
- `Assets/_Project/Scripts/TuneSystem/TuneConfig.cs` — FOUND
- `Assets/_Project/Scripts/Level/SpellScrollPickup.cs` — FOUND
- `Assets/_Project/Scripts/TuneSystem/SpellUnlockSystem.cs` — FOUND

Commits verified:
- `b40d554` — FOUND (feat(07-01): add Phase 7 spell system events and update SnakeEffect enum)
- `d46844f` — FOUND (feat(07-01): create SpellScrollPickup and SpellUnlockSystem)

---
*Phase: 07-spell-system*
*Completed: 2026-02-18*
