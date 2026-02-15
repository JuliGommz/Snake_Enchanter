---
status: verifying
trigger: "snake-ai-detection-targeting-bugs"
created: 2026-02-15T10:00:00Z
updated: 2026-02-15T10:30:00Z
---

## Current Focus

hypothesis: CONFIRMED - Two critical bugs found + secondary issues
test: Ready to fix
expecting: Fix IsPlayerInRange to include line-of-sight, Add Die transition to Animator
next_action: Apply fixes and verify

## Symptoms

expected:
1. Snakes should consistently detect player when in range and line-of-sight
2. Player spells (Tunes 1-4) should consistently target closest snake in range
3. Snakes should transition out of charmed states (MovedAway, Dazed, AttackingEnemy) after timers expire
4. Die animation should play when snake enters Dead state
5. Attack animations (Bite, Breath, Projectile) should play consistently

actual:
1. Snakes sometimes don't detect player even when standing directly in front
2. Player spells sometimes don't recognize snake as target (even when snake is right in front)
3. At least one snake remained stuck in charmed state indefinitely
4. Die animation not working at all (snake model doesn't collapse)
5. Bite animation uncertain if working, attacks trigger inconsistently after first spell

errors:
```
Parameter 'IsDazed' does not exist.
UnityEngine.Animator:SetBool (string,bool)
SnakeEnchanter.Snakes.SnakeAI:SetState (SnakeEnchanter.Snakes.SnakeState)

[ATTACK CREATURE] No targetable creature found (tag:Creature, NOT SnakeAI), returning to Idle
```

reproduction:
1. Start game in GameLevel scene
2. Cast Tune 1 (Move) on snake - sometimes works, sometimes snake doesn't react
3. Cast Tune 2 (Daze) on snake - IsDazed parameter error, animation unclear
4. Cast Tune 3 (Attack) on snake - sometimes finds RobotKyle, sometimes reports "No targetable creature found"
5. Wait for timers - some snakes transition back to Idle, otherwise remain stuck
6. Observe animations - Die animation never plays, Bite uncertain

started: Session 17 first comprehensive testing, IsDazed parameter FIXED

## Eliminated

## Evidence

- timestamp: 2026-02-15T10:05:00Z
  checked: Animator Controller (Toon Cobra Controller.controller)
  found: IsDazed parameter DUPLICATE at lines 155-160 AND 197-202 (parameter exists twice!)
  implication: Animator parameter issue was reported fixed, but duplicate might cause confusion. Parameter exists, so that error should be gone.

- timestamp: 2026-02-15T10:06:00Z
  checked: SnakeAI.cs IsPlayerInRange property (lines 335-342)
  found: Uses _commandRange (8f default) for distance check, no line-of-sight validation
  implication: IsPlayerInRange only checks distance, NOT visibility. Snake could be "in range" even if player behind wall.

- timestamp: 2026-02-15T10:07:00Z
  checked: SnakeAI.cs IsClosestTargetableSnake() (lines 1192-1216)
  found: Uses FindObjectsByType<SnakeAI>() EVERY CALL - no caching, O(n²) complexity
  implication: Performance issue, but should still work. Not the cause of targeting failures.

- timestamp: 2026-02-15T10:08:00Z
  checked: TuneController.cs spell success flow (lines 476-498)
  found: GameEvents.TuneSuccessWithId(tuneNumber) fires on success
  implication: Event fires correctly, SnakeAI should receive it if subscribed.

- timestamp: 2026-02-15T10:09:00Z
  checked: SnakeAI.cs OnTuneSuccessWithId() (lines 1121-1146)
  found: THREE checks before applying effect: IsPlayerInRange, IsTargetable, IsClosestTargetableSnake()
  implication: If ANY of these fail, snake won't react. Need to check which one is failing.

- timestamp: 2026-02-15T10:11:00Z
  checked: ALL Snake prefabs (_playerLayer field in prefabs)
  found: _playerLayer m_Bits: 0 in ALL snake prefabs (Toon Snake + Toon Cobra, all colors)
  implication: **CRITICAL BUG** - LayerMask is not configured! m_Bits: 0 = Nothing selected = Layer mask is effectively broken

- timestamp: 2026-02-15T10:12:00Z
  checked: SnakeAI.cs proximity detection (lines 498-534)
  found: UpdateProximityDetection() uses Physics.Raycast with default layers (NO layerMask parameter)
  implication: _playerLayer field is DECLARED but NEVER USED in code! Field exists for nothing.

- timestamp: 2026-02-15T10:13:00Z
  checked: SnakeAI.cs IsPlayerInRange property (lines 335-342)
  found: Only checks distance ≤ _commandRange (8f), NO line-of-sight check, NO _canSeePlayer check
  implication: **ROOT CAUSE #1** - IsPlayerInRange returns true even if player behind wall or not visible!

- timestamp: 2026-02-15T10:14:00Z
  checked: _canSeePlayer usage across SnakeAI.cs
  found: _canSeePlayer is used for Idle behavior (patrol, attack) but NOT for IsPlayerInRange property
  implication: Spell targeting uses IsPlayerInRange (distance only), NOT _canSeePlayer (line-of-sight)

- timestamp: 2026-02-15T10:16:00Z
  checked: Animator Controller transitions for Die trigger
  found: Die PARAMETER exists (line 191, Type 9 = Trigger), but NO TRANSITION from Idle→Die using Die trigger
  implication: **ROOT CAUSE #2** - SetTrigger("Die") does nothing because no transition uses it! Die animation never plays.

- timestamp: 2026-02-15T10:17:00Z
  checked: Idle state transitions (lines 641-650)
  found: 9 transitions from Idle: Bite Attack, Projectile Attack, Breath Attack, Cast Spell, Take Damage, IsDazed, Slither Forward/Left/Right
  implication: Missing transition: Idle → Die (on Die trigger). Die state exists (line 754) but unreachable.

- timestamp: 2026-02-15T10:19:00Z
  checked: AttackingEnemy state logic (lines 923-971)
  found: If FindNearestCreature() returns null, snake DOES return to Idle (line 969)
  implication: AttackingEnemy state timer is working correctly - not a bug source for stuck snakes

- timestamp: 2026-02-15T10:20:00Z
  checked: FindNearestCreature() (lines 978-1004)
  found: Searches "Creature" tag, skips all SnakeAI components (line 989-993)
  implication: RobotKyle must have tag "Creature" for Tune 3 to work

## Resolution

root_cause:
1. **IsPlayerInRange() bug** - Property only checks distance (_commandRange), ignores line-of-sight (_canSeePlayer). Snakes react to spells through walls.
2. **Die animation missing transition** - Animator has Die parameter and Die state, but NO transition from Idle→Die on Die trigger. SetTrigger("Die") does nothing.
3. **Unused _playerLayer field** - Field declared and visible in Inspector (set to m_Bits: 0 in all prefabs), but never used in code. Dead code.

fix:
1. **SnakeAI.cs IsPlayerInRange property** - Changed from distance-only check to distance + line-of-sight check using _canSeePlayer
   - Before: `return Vector3.Distance(...) <= _commandRange;`
   - After: Check distance first, then return `_canSeePlayer` (updated by UpdateProximityDetection)
   - Effect: Spells now require line-of-sight, can't target snakes through walls

2. **SnakeAI.cs _playerLayer field removal** - Removed unused SerializeField
   - Field was declared but never used in code
   - All prefabs had m_Bits: 0 (nothing selected)
   - Dead code cleanup

3. **Animator Controller Die transition** - MANUAL FIX REQUIRED IN UNITY EDITOR
   - Instructions: .planning/debug/ANIMATOR_FIX_INSTRUCTIONS.md
   - Must add transition: Idle → Die (condition: Die trigger, HasExitTime: false)
   - This enables Die animation to play when snake neutralized

verification:
**CODE FIX (IsPlayerInRange) - Ready to test:**
1. Cast Tune 1-4 on snake with clear line-of-sight → Should work (SUCCESS expected)
2. Stand behind wall, cast spell on snake through wall → Should NOT work (snake ignores spell)
3. Console should NOT show "[SPELL] Tune X" log if snake behind wall
4. All 4 Tunes should respect line-of-sight equally

**ANIMATOR FIX (Die transition) - Requires Unity Editor manual fix first:**
1. Follow instructions in ANIMATOR_FIX_INSTRUCTIONS.md
2. After adding Idle→Die transition:
   - Cast Tune 3 (Attack) on snake with RobotKyle tagged "Creature" nearby
   - Wait 1.5s for neutralization
   - Snake should play Die animation (collapse)
   - Console: "[STATE] AttackingEnemy → Dead (Gray, collision OFF, Die trigger)"

**PREFAB UPDATE - Automatic when code recompiles:**
- All snake prefabs will lose _playerLayer field (was unused)
- No functional change, just cleanup

files_changed:
- Assets/_Project/Scripts/Snakes/SnakeAI.cs (v1.7.0)
- .planning/debug/ANIMATOR_FIX_INSTRUCTIONS.md (created)

root_cause:
fix:
verification:
files_changed: []
