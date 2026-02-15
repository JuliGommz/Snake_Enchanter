# Snake AI Detection & Targeting Bugs - Fix Summary

**Debug Session:** snake-ai-detection-targeting-bugs
**Date:** 2026-02-15
**Status:** Code fixes applied, Animator fix requires manual Unity Editor work

---

## Bugs Fixed

### 1. Spells Targeting Snakes Through Walls ✅ FIXED
**Root Cause:** IsPlayerInRange property only checked distance, not line-of-sight.

**The Problem:**
```csharp
// BEFORE (v1.6.0):
public bool IsPlayerInRange
{
    get
    {
        if (_playerTransform == null) return false;
        return Vector3.Distance(transform.position, _playerTransform.position) <= _commandRange;
    }
}
// Result: Returns true even if player behind wall
```

**The Fix:**
```csharp
// AFTER (v1.7.0):
public bool IsPlayerInRange
{
    get
    {
        if (_playerTransform == null) return false;

        // Check distance first (cheap check)
        float distance = Vector3.Distance(transform.position, _playerTransform.position);
        if (distance > _commandRange) return false;

        // Check line-of-sight (requires visibility for spell targeting)
        return _canSeePlayer; // Updated by UpdateProximityDetection() every frame
    }
}
// Result: Returns true ONLY if in range AND visible (raycast line-of-sight)
```

**Impact:**
- All 4 Tunes (Move, Daze, Attack, Freeze*) now require line-of-sight
- Spells won't target snakes behind walls/obstacles
- Uses existing `_canSeePlayer` field (already maintained by `UpdateProximityDetection()`)

*Note: Freeze (Tune 4) bypasses IsPlayerInRange check, affects all snakes globally

---

### 2. Dead Code Cleanup ✅ FIXED
**Root Cause:** `_playerLayer` field was declared but never used in code.

**The Problem:**
- Field visible in Inspector: `[SerializeField] private LayerMask _playerLayer;`
- All snake prefabs had `m_Bits: 0` (nothing selected)
- Code NEVER referenced this field (not in UpdateProximityDetection, not anywhere)
- Confusion: Looked like it should be used for something, but wasn't

**The Fix:**
- Removed `_playerLayer` field entirely from SnakeAI.cs
- No functional change (field was unused)
- Cleanup reduces confusion

**Impact:**
- Cleaner code, less confusing Inspector
- Prefabs will auto-update when Unity recompiles (field disappears)

---

### 3. Die Animation Not Playing ⚠️ MANUAL FIX REQUIRED
**Root Cause:** Animator Controller missing Idle → Die transition.

**The Problem:**
- Die PARAMETER exists (Type 9 = Trigger)
- Die STATE exists (with Die animation clip)
- NO TRANSITION from Idle → Die using Die trigger
- When `SetTrigger("Die")` called, animator has no path to Die state
- Result: Snake turns gray, collider disables, but model doesn't collapse

**The Fix:**
📋 **See:** `.planning/debug/ANIMATOR_FIX_INSTRUCTIONS.md` for step-by-step

**Required Steps:**
1. Open Toon Cobra Controller in Animator window
2. Create transition: Idle → Die
3. Configure:
   - Condition: Die (trigger)
   - Has Exit Time: UNCHECK
   - Transition Duration: 0.25s
4. Save Animator Controller

**Impact:**
- Die animation will play when snake neutralized (Tune 3 or other death triggers)
- Visual feedback: Snake collapses/falls when entering Dead state

---

## Testing Checklist

### IsPlayerInRange Fix (Line-of-Sight)
- [ ] Stand in front of snake (clear line-of-sight), cast Tune 1 → Should work
- [ ] Stand behind wall, cast Tune 1 on snake through wall → Should NOT work
- [ ] Move out from behind wall → Spell should now work
- [ ] Check console: No "[SPELL] Tune X" log when line-of-sight blocked
- [ ] Test all 4 Tunes (Move, Daze, Attack, Freeze)

### Die Animation Fix (After Animator Manual Fix)
- [ ] Cast Tune 3 (Attack) on snake with RobotKyle (tag "Creature") nearby
- [ ] Wait 1.5s for neutralization
- [ ] Snake plays Die animation (collapse)
- [ ] Snake turns gray
- [ ] Snake collider disabled
- [ ] Console: "[STATE] AttackingEnemy → Dead (Gray, collision OFF, Die trigger)"

### Prefab Cleanup (Automatic)
- [ ] Open any snake prefab → _playerLayer field should be gone from Inspector
- [ ] No errors in console related to missing field

---

## Files Changed

1. **Assets/_Project/Scripts/Snakes/SnakeAI.cs** (v1.7.0)
   - IsPlayerInRange: Now checks distance + line-of-sight
   - Removed: _playerLayer field (unused dead code)
   - Version history updated

2. **.planning/debug/ANIMATOR_FIX_INSTRUCTIONS.md** (created)
   - Step-by-step manual fix for Animator Controller

3. **.planning/debug/snake-ai-detection-targeting-bugs.md** (debug session log)
   - Evidence, hypothesis testing, root cause analysis

---

## Known Issues (Not Fixed)

### RobotKyle Missing "Creature" Tag
**Symptom:** "[ATTACK CREATURE] No targetable creature found"

**Cause:** RobotKyle (or other enemy) doesn't have tag "Creature"

**Fix:** Tag RobotKyle GameObject with "Creature" tag in Unity Inspector

**Code Reference:** `FindNearestCreature()` searches `GameObject.FindGameObjectsWithTag("Creature")`

---

## Technical Notes

### Why Line-of-Sight Matters
The game has two detection systems:
1. **Proximity Detection** (`UpdateProximityDetection()`) - Used for snake AI behavior (patrol, attack player)
   - Updates `_canSeePlayer` every frame via raycast
   - Range: `_detectionRange` (10f default)

2. **Spell Targeting** (`IsPlayerInRange`) - Used for spell targeting (which snake reacts to tune)
   - Range: `_commandRange` (8f default)
   - NOW also checks `_canSeePlayer` (line-of-sight)

**Before v1.7.0:** Spell targeting ignored walls (distance-only)
**After v1.7.0:** Spell targeting requires line-of-sight (distance + visibility)

### UpdateProximityDetection() Already Handles Raycasting
The fix leverages existing logic:
```csharp
// UpdateProximityDetection() runs every frame in Update():
if (Physics.Raycast(rayOrigin, directionToPlayer, out RaycastHit hit, _detectionRange))
{
    _canSeePlayer = hit.collider.CompareTag("Player");
}
else
{
    _canSeePlayer = false;
}
```

IsPlayerInRange now simply reads this already-updated value. No additional raycasts needed.

---

## Performance Impact
**Minimal - Actually improves performance slightly:**
- Removed unused `_playerLayer` field (less memory per snake instance)
- IsPlayerInRange now returns false faster when line-of-sight blocked
  - Prevents `IsClosestTargetableSnake()` from running (expensive FindObjectsByType call)
- No new raycasts added (reuses existing `_canSeePlayer` from UpdateProximityDetection)

---

## Commit Message

```
fix: SnakeAI v1.7.0 - Spell line-of-sight + Die animation fix

CRITICAL FIXES:
- IsPlayerInRange now checks line-of-sight (not just distance)
  Spells no longer target snakes through walls
- Removed unused _playerLayer field (dead code cleanup)
- Added ANIMATOR_FIX_INSTRUCTIONS.md for manual Die transition fix

ROOT CAUSES:
1. IsPlayerInRange ignored _canSeePlayer (only checked distance)
2. Animator Controller missing Idle→Die transition (Die trigger unused)
3. _playerLayer field declared but never referenced in code

TESTING REQUIRED:
- Verify spells require line-of-sight (no wall-piercing)
- Apply Animator fix manually in Unity Editor
- Tag RobotKyle with "Creature" for Tune 3 to work
```

---

## Next Steps

1. **Immediate:** Test IsPlayerInRange fix (line-of-sight)
2. **Before Tune 3 testing:** Apply Animator Controller fix (ANIMATOR_FIX_INSTRUCTIONS.md)
3. **Before Tune 3 testing:** Tag RobotKyle with "Creature"
4. **Optional:** Cache snake list in GameManager to avoid FindObjectsByType (performance optimization)
