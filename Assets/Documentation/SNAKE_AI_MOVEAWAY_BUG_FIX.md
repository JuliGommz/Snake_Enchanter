# SnakeAI v1.3.10 - MoveAway Infinite Loop Bug Fix

**Date:** 2026-02-13
**Session:** 14
**Bug ID:** snake-moveaway-loop
**Severity:** CRITICAL - Game-breaking (snakes stuck forever)

---

## Bug Summary

Snakes entering MovedAway state (after successful Move Away tune) would get stuck in an infinite movement loop against walls, never returning to Idle state, continuously spamming "Movement blocked" console messages.

---

## Root Cause Analysis

### The Bug Mechanism

The MovedAway state has TWO exit conditions (both must be met OR one must trigger):

1. **Distance check** (line 533): `if (distanceToTarget < 1.0f)` → Transition to Idle
2. **Tag check** (line 553): `if (hit.collider.CompareTag("MoveAwayTarget"))` → Transition to Idle

**THE PROBLEM:**

When MoveAwayTargets were >4 units away with obstacles (walls) in the path:

1. Snake moves toward target position
2. Hits wall BEFORE reaching <1.0f distance → Distance check FAILS
3. Raycast detects wall blocking
4. Checks if wall has "MoveAwayTarget" tag → Tag check FAILS (wall is "Untagged" or "Environment")
5. Code continues to next frame
6. Loop repeats: Move → Blocked → Check tag → Fail → Repeat...

**Result:** Infinite loop. Snake NEVER exits MovedAway state.

### Why It Happened

**TWO compounding bugs:**

1. **Scene Configuration Error:**
   - MoveAwayTarget GameObjects in scene had `m_TagString: Untagged`
   - Should have been `m_TagString: MoveAwayTarget`
   - Tag "MoveAwayTarget" existed in TagManager but wasn't APPLIED to objects

2. **Code Logic Gap:**
   - No timeout/fallback when blocked by obstacles
   - Assumed snake would always reach <1.0f distance OR hit target collider
   - Didn't account for unreachable targets (blocked by level geometry)

---

## The Fix

### 1. Scene Fix (GameLevel.unity)

**Changed:**
- Line 2163: `m_TagString: Untagged` → `m_TagString: MoveAwayTarget` (Target 1)
- Line 4103: `m_TagString: Untagged` → `m_TagString: MoveAwayTarget` (Target 2)

**Effect:**
- Tag check (line 553) now succeeds when snake reaches MoveAwayTarget
- Instant transition to Idle when target collider detected

### 2. Code Fix (SnakeAI.cs v1.3.10)

**Added timeout mechanism:**

```csharp
// Check if blocked - could be obstacle OR the target itself
if (!moved)
{
    // Raycast to see what's blocking
    Vector3 direction = (_moveAwayTarget.position - transform.position).normalized;
    RaycastHit hit;
    if (Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, out hit, 1.5f))
    {
        // If blocked by MoveAwayTarget, we've arrived
        if (hit.collider.CompareTag("MoveAwayTarget"))
        {
            _isMoving = false;
            TransitionFromMoveAwayToRootState();
            Debug.Log($"SnakeAI ({_snakeName}): Blocked by MoveAwayTarget collider (tag detected), reached destination");
        }
        // NEW: If blocked by obstacle and not making progress, give up after timeout
        else
        {
            _stateTimer += Time.deltaTime;
            if (_stateTimer > 2.0f) // 2 seconds of continuous blocking
            {
                _isMoving = false;
                _stateTimer = 0f;
                TransitionFromMoveAwayToRootState();
                Debug.LogWarning($"SnakeAI ({_snakeName}): Blocked by obstacle for 2s, giving up on MoveAwayTarget");
            }
        }
    }
}
else
{
    // NEW: Successfully moved - reset timeout counter
    _stateTimer = 0f;
}
```

**Effect:**
- If snake stuck against obstacle for 2 consecutive seconds → Give up, return to Idle
- Only counts CONTINUOUS blocking (resets when movement succeeds)
- Prevents infinite loop even if tags configured incorrectly

---

## Verification

### Test Cases

**Test 1 - Normal MoveAway (target reachable):**
- Cast Move Away → Snake reaches target → Stops → Returns to Idle
- Expected console: "Blocked by MoveAwayTarget collider (tag detected), reached destination"

**Test 2 - MoveAway with obstacle (target blocked):**
- Cast Move Away → Snake hits wall → Waits 2s → Gives up → Returns to Idle
- Expected console: "Blocked by obstacle for 2s, giving up on MoveAwayTarget"

**Test 3 - Infinite loop prevented:**
- No more continuous "Movement blocked" spam
- Snakes ALWAYS return to Idle (within 2s max if blocked)

### Success Criteria

✅ Snakes return to Idle state after MoveAway (color changes to green)
✅ No infinite "Movement blocked" console spam
✅ Snakes resume normal behavior (patrol/attack) after returning to Idle
✅ Debug logs show either tag detection OR timeout (never stuck)

---

## Lessons Learned

### For Future Development

1. **Always verify scene object tags match code expectations**
   - TagManager defines tags, but objects must be TAGGED in Inspector
   - Unity doesn't auto-apply tags when added to TagManager

2. **Add timeouts/fallbacks for state transitions**
   - Never rely on a single exit condition for critical states
   - Always have a "give up" mechanism for unreachable goals

3. **Test with realistic level geometry**
   - Targets >4 units away exposed the bug
   - Close targets (<1 unit) would have hidden it

4. **Better debug logging**
   - Current version logs when stuck → helps identify issues
   - Could add distance tracking to detect "not making progress" earlier

---

## Files Changed

- `Assets/_Project/Scenes/GameLevel.unity` (MoveAwayTarget tag fixes)
- `Assets/_Project/Scripts/Snakes/SnakeAI.cs` (v1.3.9 → v1.3.10, timeout mechanism)

---

## Related Issues

- v1.3.2: Fixed Move Away infinite movement (missing Idle transition)
- v1.3.3: Fixed self-collision bug (OverlapSphere removal)
- v1.3.5: Fixed inverted LayerMask bug
- v1.3.8: Fixed Player passthrough bug
- **v1.3.10: Fixed MoveAway infinite loop (THIS FIX)**

---

## Author Notes

This bug demonstrates the importance of:
- Complete testing with realistic scenarios (distant targets, obstacles)
- Defensive coding (timeouts, fallbacks)
- Scene configuration validation (tags actually applied, not just defined)
- Systematic debugging (evidence gathering → hypothesis → fix → verify)

The fix is robust: Works correctly with proper tags (preferred path) AND gracefully handles misconfiguration (timeout fallback).
