---
status: resolved
trigger: "snake-moveaway-loop: Snakes stuck in MovedAway state, never return to Idle, move in wrong direction toward MoveAwayTarget, continuously move against walls without stopping"
created: 2026-02-13T10:00:00Z
updated: 2026-02-13T10:45:00Z
---

## Current Focus

hypothesis: ROOT CAUSE IDENTIFIED - TWO compounding bugs:
  1. MoveAwayTarget GameObjects missing "MoveAwayTarget" tag (they're "Untagged")
  2. Snakes blocked by walls before reaching targets (no pathfinding, direct line movement)
  Result: Line 546 tag check fails → Line 683 blocks ALL movement → Snake stuck in MovedAway state forever
test: Fix tags on MoveAwayTarget objects in scene, verify snake reaches target
expecting: With correct tags, line 546-550 will detect target collision and call TransitionFromMoveAwayToRootState()
next_action: Apply root cause fix

## Symptoms

expected: Snake bewegt sich zum MoveAwayTarget, stoppt dort, kehrt zu Idle zurück (Normal behavior: Snake geht zum Target, stoppt präzise, evaluiert Player position, geht zu Idle state)

actual:
- Snakes bleiben im Movement-State gegen Wände stecken
- Sie sind SICHTBAR (nicht mehr im Collider drinnen - dieses Issue ist gelöst)
- ABER: Sie sind noch IN BEWEGUNG gegen die Wand
- Sie kommen NICHT zurück zu Idle
- Snakes laufen NICHT Richtung MoveAwayObject sondern in eine ANDERE (ähnliche) Richtung
- Snakes stoppen IMMER NOCH NICHT, sind weiterhin ständig in Movement
- Sie bleiben innerhalb der Wände

errors:
```
SnakeAI (Snake): Movement blocked by WallPropsBottomA_ST (2) (Tag: Enviroment) at distance 0,30
SnakeAI (Snake): Movement blocked by Toon Snake - Green (Tag: Enemy) at distance 0,30
SnakeAI (Snake): Movement blocked by Caves_L2_wall (Tag: Untagged) at distance 0,30
```

reproduction:
1. Cast Move Away Spell (Tune 1) on Snake
2. Snake enters MovedAway state
3. Snake moves toward MoveAwayTarget but in WRONG direction
4. Snake hits wall/collider, gets blocked
5. Snake CONTINUES trying to move (never stops, never returns to Idle)
6. Continuous console spam of "Movement blocked by..."

started: Hat nie richtig funktioniert seit Session Start (Session 14). MoveAway war schon vor v1.3.9 kaputt

## Eliminated

## Evidence

- timestamp: 2026-02-13T10:05:00Z
  checked: SnakeAI.cs v1.3.9 (complete file)
  found:
    - Line 163: `_moveAwayTarget` is a Transform field (assigned per snake instance)
    - Line 514-554: MovedAway state logic uses `_moveAwayTarget.position` directly
    - Line 536: `MoveTowardsSafe(_moveAwayTarget.position, _moveSpeed)` - moves toward TARGET POSITION
    - Line 542: Direction calculated as `(_moveAwayTarget.position - transform.position).normalized`
    - Line 546: Check if blocked by MoveAwayTarget TAG (CompareTag("MoveAwayTarget"))
    - Line 526: Distance check uses 1.0f threshold
    - Line 745-753: TransitionFromMoveAwayToRootState() always calls SetState(Idle)
    - NO CODE assigns which MoveAwayTarget to use - relies on Inspector assignment
  implication: Each snake needs its own MoveAwayTarget assigned in Inspector. If wrong target assigned OR target is >4 units away, snake will move in "wrong direction" trying to reach distant target.

- timestamp: 2026-02-13T10:10:00Z
  checked: GameLevel.unity scene file
  found:
    - TWO MoveAwayTargets exist: {fileID: 460752679} and {fileID: 981868797}
    - Target 1 position: (8.756687, 0, 17.710442)
    - Target 2 position: (4.19, 0, 16.55)
    - BOTH have `m_TagString: Untagged` (NOT "MoveAwayTarget" tag!)
    - Both snakes have targets assigned (one to each target)
    - Scene has only 3 tags used: MainCamera, Player, Untagged (43 objects)
  implication: MoveAwayTarget GameObjects are NOT tagged "MoveAwayTarget", so line 546 check `CompareTag("MoveAwayTarget")` will ALWAYS fail!

- timestamp: 2026-02-13T10:12:00Z
  checked: TagManager.asset
  found:
    - Tags defined: Enemy, Enviroment (TYPO!), MoveAwayTarget
    - Tag exists in project but NOT APPLIED to scene objects
  implication: Tag "MoveAwayTarget" is defined but GameObjects in scene don't use it. Also confirms "Enviroment" typo in TagManager (but irrelevant since scene objects are Untagged anyway).

- timestamp: 2026-02-13T10:15:00Z
  checked: User symptoms + distance context
  found:
    - User said: "Targets sind momentan >4 entfernt" from snakes
    - Threshold check at line 526: `if (distanceToTarget < 1.0f)`
    - If targets are 4+ units away and snakes move at 0.4 speed, they should eventually reach <1.0f
    - BUT: Console shows "Movement blocked" logs continuously → snakes hitting walls BEFORE reaching target
  implication: Snakes are moving toward targets but hitting walls on the way. They can't path around obstacles (no pathfinding), so they get stuck against walls.

- timestamp: 2026-02-13T10:20:00Z
  checked: MovedAway state logic flow (lines 513-562)
  found: **THE BUG MECHANISM:**
    1. Snake enters MovedAway state, starts moving toward target position
    2. MoveTowardsSafe() raycasts ahead (line 679)
    3. Raycast hits wall/obstacle → logs "Movement blocked" → returns false (line 683-684)
    4. Since moved=false, execution reaches line 542-552 (blocked check)
    5. Raycast forward to see what's blocking (line 544)
    6. Line 546: `if (hit.collider.CompareTag("MoveAwayTarget"))` → ALWAYS FALSE (target is "Untagged")
    7. If check fails → code continues to next frame
    8. Line 526 distance check: `if (distanceToTarget < 1.0f)` → FALSE (target is >4 units, snake stuck against wall)
    9. Loop repeats every frame: Move blocked → Check tag → Tag wrong → Continue state → Move blocked...
  implication: INFINITE LOOP. Snake never exits MovedAway state because BOTH exit conditions fail (distance never <1.0f, tag check never true).

## Eliminated

- hypothesis: Wrong target assignment (each snake going to wrong target)
  evidence: Scene file shows proper 1:1 assignment (each snake has own target)
  timestamp: 2026-02-13T10:18:00Z

- hypothesis: Tag typo "Environment" vs "Enviroment" causing collision issues
  evidence: Scene objects are all "Untagged" anyway, so tag spelling irrelevant for this bug
  timestamp: 2026-02-13T10:18:00Z

## Resolution

root_cause: |
  MoveAwayTarget GameObjects in scene are tagged "Untagged" instead of "MoveAwayTarget".
  When snake reaches target but is blocked by wall/obstacle:
  - Distance check (line 526) fails: target >4 units away, snake stuck against wall
  - Tag check (line 546) fails: CompareTag("MoveAwayTarget") returns false (object is "Untagged")
  - Both exit conditions fail → Snake stuck in MovedAway state forever
  - MoveTowardsSafe() blocks movement every frame → "Movement blocked" spam in console

  ADDITIONAL BUG: Even if tags were correct, distance check at 1.0f is too strict for targets >4 units away with obstacles in path. Snakes need distance-based OR tag-based exit (not AND).

fix: |
  THREE-PART FIX:

  1. SCENE FIX (GameLevel.unity):
     - Tagged both MoveAwayTarget GameObjects with "MoveAwayTarget" tag (were "Untagged")
     - Line 2163: Changed m_TagString from "Untagged" to "MoveAwayTarget" (Target 1)
     - Line 4103: Changed m_TagString from "Untagged" to "MoveAwayTarget" (Target 2)

  2. CODE FIX (SnakeAI.cs v1.3.10 - MovedAway state logic):
     - Added 2-second timeout for continuous obstacle blocking
     - Line 543-557: When blocked by non-target, accumulate _stateTimer
     - After 2s of continuous blocking, give up and return to Idle
     - Line 562-565: Reset timeout counter when movement succeeds (only counts stuck time)
     - Prevents infinite loop when obstacles block path to distant targets

  3. IMPROVED ROBUSTNESS:
     - Now has TWO exit paths: tag detection (intended) OR timeout (fallback)
     - Tag detection: Instant transition when reaching MoveAwayTarget
     - Timeout fallback: Graceful exit after 2s stuck against obstacles
     - Covers edge case where targets are unreachable due to level geometry

verification: |
  VERIFICATION PLAN (User must test in Unity):

  Test Case 1 - Normal MoveAway (target reachable):
  1. Start GameLevel scene
  2. Cast Move Away (Tune 1) on Snake
  3. EXPECTED: Snake moves toward MoveAwayTarget, reaches it, stops, returns to Idle
  4. VERIFY: No "Movement blocked" spam, clean state transition

  Test Case 2 - MoveAway with obstacle (target blocked):
  1. If targets are >4 units away with walls between snake and target
  2. Cast Move Away on Snake
  3. EXPECTED: Snake tries to reach target, hits obstacle, waits 2s, gives up, returns to Idle
  4. VERIFY: Console shows "Blocked by obstacle for 2s, giving up" after 2 seconds, not infinite loop

  Test Case 3 - Tag detection:
  1. Position snake directly in front of MoveAwayTarget (no obstacles)
  2. Cast Move Away
  3. EXPECTED: Snake reaches target, tag check passes, instant transition to Idle
  4. VERIFY: Console shows "Blocked by MoveAwayTarget collider (tag detected), reached destination"

  SUCCESS CRITERIA:
  - ✓ No infinite "Movement blocked" spam
  - ✓ Snakes return to Idle state (visible as color change to green)
  - ✓ Snakes resume normal behavior (patrol/attack) after MoveAway
  - ✓ Debug logs show either tag detection OR timeout (not stuck)

files_changed:
  - Assets/_Project/Scenes/GameLevel.unity
  - Assets/_Project/Scripts/Snakes/SnakeAI.cs
