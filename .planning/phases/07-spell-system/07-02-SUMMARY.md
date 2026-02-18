---
phase: 07-spell-system
plan: 02
subsystem: tune-system
tags: [unity, csharp, new-input-system, events, ui, canvasgroup, tmpro, scriptableobject]

# Dependency graph
requires:
  - phase: 07-01
    provides: GameEvents.OnTuneUnlocked event, SpellScrollPickup, SpellUnlockSystem event infrastructure
provides:
  - TuneController v3.0 with 3-element TuneConfig array and bool[] unlock gate
  - SpellHUDController.cs — dynamic HUD that starts empty and grows one slot per scroll collection
affects:
  - 07-03 (ShieldComponent fires GameEvents — TuneController must accept Tune3=Shield correctly)
  - 07-04 (HUD polish subscribes to same OnTuneUnlocked event; SpellHUDController is the base)

# Tech tracking
tech-stack:
  added: []
  patterns:
    - "Array-based multi-tune management with loop-driven EnableInput/DisableInput (replaces 4 explicit blocks)"
    - "Closure capture via local variable in loop: int tuneNum = i + 1 before lambda (prevents B-001 array equivalent)"
    - "Dynamic UI slot instantiation: Instantiate prefab + GetComponent + CanvasGroup fade-in on event"
    - "RevealSlot coroutine with Time.unscaledDeltaTime for timeScale=0 safety"

key-files:
  created:
    - Assets/_Project/Scripts/UI/SpellHUDController.cs
  modified:
    - Assets/_Project/Scripts/TuneSystem/TuneController.cs

key-decisions:
  - "Tunes silently ignore locked presses — no feedback, no error. Clean UX: player only learns via HUD appearance."
  - "bool[] _tuneUnlocked NOT serialized — runtime-only state driven by GameEvents. No Inspector override possible."
  - "RevealSlot uses Time.unscaledDeltaTime — same reasoning as SpellUnlockSystem (timeScale may be 0 or transitioning at unlock moment)"
  - "Duplicate guard (_slots[idx] != null) prevents double-instantiation if OnTuneUnlocked fires multiple times for same tune"

patterns-established:
  - "Array delegate caching: _onTuneStarted[i] / _onTuneCanceled[i] indexed arrays for Input System loop-subscribe pattern"
  - "Slot configuration via Transform.Find('ChildName') — explicit named child lookup for prefab wiring"

# Metrics
duration: 3min
completed: 2026-02-18
---

# Phase 7 Plan 02: Spell System Foundation Summary

**TuneController refactored from 4 fixed fields to a 3-element array with scroll-based unlock gate, plus SpellHUDController for dynamic HUD slots that fade in as scrolls are collected**

## Performance

- **Duration:** ~3 min
- **Started:** 2026-02-18T12:12:21Z
- **Completed:** 2026-02-18T12:14:59Z
- **Tasks:** 2
- **Files modified:** 2

## Accomplishments

- TuneController v3.0: 4 individual TuneConfig fields collapsed to `TuneConfig[3]` array; 4 InputActions + 8 delegates collapsed to `InputAction[3]` + `Action[][3]` with loop-based enable/disable
- Unlock gate: `bool[] _tuneUnlocked = new bool[3]` (all false at start); `OnTuneUnlockedEvent` subscribes to `GameEvents.OnTuneUnlocked` and sets flags
- EndTune case 3 now triggers `SpellShield` animator (was `SpellAttack`); Tune4/Freeze and all references removed
- SpellHUDController.cs: subscribes to `OnTuneUnlocked`, instantiates slot prefab per event, configures Background/KeyIcon/KeyLabel/SpellName, fades in via CanvasGroup coroutine with `Time.unscaledDeltaTime`

## Task Commits

Each task was committed atomically:

1. **Task 1: Refactor TuneController to 3-tune array with unlock gate** - `e620b6d` (feat)
2. **Task 2: Create SpellHUDController.cs for dynamic tune slots** - `c117d0c` (feat)

**Plan metadata:** (this commit — docs)

## Files Created/Modified

- `Assets/_Project/Scripts/TuneSystem/TuneController.cs` — v3.0: 3-tune array, bool[] unlock gate, loop-based input, SpellShield trigger for case 3, Tune4 fully removed
- `Assets/_Project/Scripts/UI/SpellHUDController.cs` — New file: dynamic HUD with OnTuneUnlocked subscription, slot instantiation, CanvasGroup fade-in via unscaledDeltaTime coroutine

## Decisions Made

- Tunes silently ignore locked key presses — no negative feedback, no error. Player learns unlocked state from the HUD growing.
- `_tuneUnlocked` is NOT serialized — runtime state only, fully controlled by the event system. No Inspector override possible (intentional).
- `RevealSlot` uses `Time.unscaledDeltaTime` for the same reason as SpellUnlockSystem: the reveal may be called at the moment timeScale transitions back to 1 after the unlock panel dismiss.
- Duplicate guard on slot creation: `_slots[idx] != null` prevents double-instantiation if the event fires more than once for the same tune.

## Deviations from Plan

None — plan executed exactly as written.

## Issues Encountered

None.

## User Setup Required

None — no external service configuration required.

After wiring up in Unity:
1. In the Hierarchy, create a `SpellHUD` GameObject on the Canvas. Attach `SpellHUDController`.
2. Create a child `SlotsContainer` (RectTransform + HorizontalLayoutGroup). Assign to `_slotsContainer`.
3. Build a `SlotPrefab` with the documented structure (Background Image, KeyIcon Image + KeyLabel TMPro child, SpellName TMPro). Assign to `_slotPrefab`.
4. Assign 3 key-shape sprites to `_keyIconSprites` in Inspector (placeholder sprites acceptable for v1.0).
5. `_tuneConfigs[0..2]` in TuneController Inspector: assign Move/Daze/Shield TuneConfig ScriptableObjects.

## Next Phase Readiness

- TuneController v3.0 is ready. Key 1/2/3 do nothing until scrolls are collected — the unlock flow (SpellScrollPickup → SpellUnlockSystem → GameEvents.TuneUnlocked) was built in 07-01.
- SpellHUDController is ready to receive unlock events and grow the HUD dynamically.
- 07-03 (ShieldComponent) was already committed before this plan executed — it integrates cleanly because TuneController now correctly fires `SpellShield` on Tune3 success.
- No blockers.

## Self-Check: PASSED

Files verified present:
- `Assets/_Project/Scripts/TuneSystem/TuneController.cs` — FOUND
- `Assets/_Project/Scripts/UI/SpellHUDController.cs` — FOUND

Commits verified:
- `e620b6d` — FOUND (feat(07-02): refactor TuneController to 3-tune array with unlock gate)
- `c117d0c` — FOUND (feat(07-02): create SpellHUDController for dynamic tune slot HUD)

---
*Phase: 07-spell-system*
*Completed: 2026-02-18*
