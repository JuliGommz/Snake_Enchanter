# Animator Controller Fix - Die Transition Missing

## Problem
The Die trigger parameter exists in the Animator Controller, but there's no transition from Idle → Die state that uses it. When `SetTrigger("Die")` is called, nothing happens because the animator has no path to reach the Die state.

## Fix Required
**File:** `Assets/_Project/Art-Visuals/3D_Assets/Snakes/Controllers/Toon Cobra Controller.controller`

### Steps in Unity Editor:

1. Open the Animator window:
   - Select any Toon Cobra prefab in Project view
   - Click "Open in Animator" or Window → Animation → Animator

2. Locate the Idle state (should be the default state)

3. Right-click Idle state → Make Transition → Click on Die state

4. Configure the new transition:
   - Select the transition arrow (Idle → Die)
   - In Inspector:
     - **Has Exit Time:** UNCHECK (trigger should work immediately)
     - **Transition Duration:** 0.25 seconds (smooth blend)
     - **Conditions:** Click "+" button
       - Add condition: Die (the trigger parameter)

5. Verify:
   - The transition should show in the list: Idle → Die with condition "Die"
   - Die state should be reachable when Die trigger fires

6. Save the Animator Controller (Ctrl+S)

## Why This Is Needed
Currently, when a snake enters the Dead state (via Tune 3 Attack or other death logic), the code calls `_animator.SetTrigger("Die")` but the animator has no transition configured to respond to that trigger. The Die animation state exists but is unreachable.

## Testing After Fix
1. Cast Tune 3 (Attack) on a snake
2. Wait for the snake to neutralize after attack
3. The snake should play the Die animation (collapse/fall) when entering Dead state
4. Snake should turn gray and have collider disabled

Expected console log: `[STATE] AttackingEnemy → Dead (Gray, collision OFF, Die trigger)`
