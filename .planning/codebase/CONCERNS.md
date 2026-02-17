# Codebase Concerns

**Analysis Date:** 2026-02-13

---

## 🔴 CRITICAL ISSUES

### Snake AI System Complete Failure
**Status:** Blocking Phase 2 gameplay
**Severity:** Critical

#### 1. Patrol System Non-Functional
- **Issue:** Snakes do not move during patrol (UpdatePatrol not working)
- **Files:** `Assets/_Project/Scripts/Snakes/SnakeAI.cs` (lines 219-266)
- **Problem:** SnakeAI v1.0 was designed for static snakes only. No patrol code exists despite comments mentioning UpdatePatrol method that is never implemented.
- **Impact:** Snakes remain frozen in place. No waypoint-based movement. No dynamic threat.
- **Current Code:** Only MovedAway state has movement logic (lines 236-248). All other states have movement stubs.
- **Fix approach:**
  1. Implement full patrol system with waypoint generation
  2. Add random walk within patrol radius during Idle state
  3. Integrate with animator for movement animations (requires snake animator)
  4. Test with debug visualization

#### 2. Move Away Movement Infinite Loop
- **Issue:** Snakes move toward MoveAwayTarget but never stop
- **Files:** `Assets/_Project/Scripts/Snakes/SnakeAI.cs` (lines 235-248)
- **Problem:** `_isMoving` flag is set to false when distance < 0.1f, but MoveTowards continues to apply even after flag is false. State never transitions away from MovedAway.
- **Symptoms:** After Tune 1 success, snake glides infinitely or gets stuck at target
- **Fix approach:**
  1. Add state transition after reaching target (e.g., SetState(SnakeState.Idle))
  2. Alternatively, change MovedAway to temporary state that auto-expires
  3. Add timeout to MovedAway state to prevent infinite sticking

#### 3. Collision - Snakes Phase Through Walls
- **Issue:** Snakes move through cave walls and obstacles (no physics boundaries)
- **Files:** `Assets/_Project/Scripts/Snakes/SnakeAI.cs` (lines 239-242)
- **Problem:** Using raw `transform.position = Vector3.MoveTowards()` bypasses all colliders. No CharacterController, Rigidbody, or NavMesh agent.
- **Impact:**
  - Snakes can escape the level
  - Cannot block paths with geometry
  - No visual obstruction
- **Fix approach:**
  1. **Option A (Recommended for Phase 2):** Use NavMeshAgent with SetDestination() instead of manual movement
  2. **Option B:** Implement raycast collision checks before applying MoveTowards
  3. **Option C:** Use CharacterController (like Player) but separate from player's controller
  4. **Preferred:** Option A requires baking NavMesh once, then is automatic

#### 4. ALL Attack Animations Broken
- **Issue:** No attack animations play (Bite, Breath, Projectile)
- **Files:** `Assets/_Project/Scripts/Snakes/SnakeAI.cs` (no animator integration)
- **Problem:** SnakeAI has no animator reference or animation triggers. Damage system works but animations don't.
- **Previous State:** Bite worked, Breath didn't (per BACKLOG.md, Session 10)
- **Current State:** NONE work after v1.3.1 changes
- **Symptoms:** Snakes attack player but remain silent/static
- **Root Cause:** No Animator component assigned, no animator setup in Toon Snake prefabs
- **Fix approach:**
  1. Assign Animator reference to SnakeAI (currently missing)
  2. Ensure Toon Snake prefabs have Animator component with proper controller
  3. Add trigger/parameter calls matching Animator controller:
     - SetTrigger("Bite") for bite attacks
     - SetBool("BreathAttack", true/false) for breath (note: it's a bool, not trigger!)
     - SetTrigger("ProjectileAttack") for projectile
  4. Match CLAUDE.md specifications: "Parameter haben Leerzeichen" (parameters have spaces!)

#### 5. Attack Damage System Possibly Broken
- **Issue:** Damage delivery mechanism unclear
- **Files:** `Assets/_Project/Scripts/Snakes/SnakeAI.cs` (lines 199-212)
- **Problem:** OnTriggerEnter checks for SnakeState.Aggressive, but snakes enter states during animation delays and attack cooldowns. GetComponent<HealthSystem> called per collision (performance + potential null refs).
- **Risk:** Damage may not fire during animation windows, or may miss because timing doesn't match attack animation duration
- **Current Implementation:** Raycast-based damage is mentioned in CLAUDE.md but not in SnakeAI.cs code - inconsistency
- **Fix approach:**
  1. Clarify: Is damage raycast-based (per CLAUDE.md) or OnTriggerEnter-based (per code)?
  2. If raycast: Implement proper raycast damage with line-of-sight check
  3. If trigger: Verify collision state matches attack animation frame
  4. Cache HealthSystem reference to avoid repeated GetComponent

---

## 🟠 HIGH PRIORITY ISSUES

### Exit Trigger Animation Hang
- **Issue:** Animation freezes when reaching win condition
- **Files:** `Assets/_Project/Scripts/Level/ExitTrigger.cs`
- **Problem:** ExitTrigger calls GameManager but doesn't properly transition to Win state or disable player input. Animation plays but may not have proper fade-out or next-level logic.
- **Impact:** Player can't proceed after winning. UX broken.
- **Fix approach:**
  1. Implement proper Win state transition in GameManager
  2. Fade to black (UI canvas)
  3. Disable player input via PlayerController.SetMovementEnabled(false)
  4. Trigger win screen or load next level
  5. Add state machine to GameManager for Win/Lost state handling

### Spell Animation Timing - Triggered at Wrong Time
- **Issue:** Spell animations play AFTER key release instead of DURING hold
- **Files:** `Assets/_Project/Scripts/TuneSystem/TuneController.cs` (lines 476-495)
- **Problem:** Animation trigger called in EndTune() at key release (line 493), not at key press
- **Expected:** Animation should start while slider is active (during hold)
- **Current:** Animation starts only after success/failure evaluation
- **Impact:** Visual feedback is delayed and disconnected from player action
- **Fix approach:**
  1. Move animation trigger to StartTune() method (when key pressed)
  2. Use simple idle/casting state animations that loop
  3. Use EndTune() for result-specific animations (success glow, fail shake) overlaid on top
  4. Test visual synchronization with slider movement

---

## 🟡 MEDIUM PRIORITY ISSUES

### Performance: GetComponent Calls in Loop
- **Issue:** GetComponent called per collision frame
- **Files:** `Assets/_Project/Scripts/Snakes/SnakeAI.cs` (line 206)
- **Problem:** OnTriggerEnter calls `other.GetComponent<Player.HealthSystem>()` every frame contact happens
- **Impact:** 5-10% performance hit on snake-heavy scenes (Unity Audit 2026-02-11 noted)
- **Fix approach:**
  1. Cache HealthSystem reference at Awake (same way Collider and Renderer are cached)
  2. Store as private field: `private HealthSystem _playerHealthSystem`
  3. Find reference in OnTriggerEnter if null (lazy initialization)
  4. Similar issue in line 400 (FindObjectsByType called every OnTuneSuccessWithId)

### FindObjectsByType Called Repeatedly
- **Issue:** `FindObjectsByType<SnakeAI>()` called per tune event
- **Files:** `Assets/_Project/Scripts/Snakes/SnakeAI.cs` (line 400) and GameManager
- **Problem:** Every successful tune triggers IsClosestTargetableSnake() which scans all snakes in scene
- **Impact:** O(n²) complexity with snake count. Performance degrades with more snakes (Phase 2 goal: 6+ snakes)
- **Fix approach:**
  1. Cache all snakes in GameManager.OnSceneLoad()
  2. Use singleton registry pattern: SnakeRegistry.Register/Unregister
  3. Pass snake list to tune events instead of scanning per snake
  4. Or use spatial partitioning for nearest-neighbor queries

### Animator Parameter Names Require Exact Spacing
- **Issue:** Toon Cobra animator has parameters with SPACES in names (unusual)
- **Files:** All animator integration points, particularly SnakeAI if/when it gets animator
- **Problem:** Parameter names like `"Bite Attack"` (with space) must match exactly or animation won't trigger
- **Risk:** Easy to misspell or use wrong casing. No compile-time checking.
- **Current Code:** SnakeAI has no animator integration, so not triggered yet
- **Fix approach:**
  1. Document all parameter names clearly in SnakeAI class comments
  2. Use string constants: `private const string BITE_ATTACK = "Bite Attack"`
  3. Consider renaming parameters to remove spaces if possible (editor/animator side)
  4. Test each animation trigger manually before deploying

---

## 🔶 FRAGILE AREAS

### Snake State Machine Architecture Too Simple
- **Issue:** State machine lacks guards and transition logic
- **Files:** `Assets/_Project/Scripts/Snakes/SnakeAI.cs` (lines 215-314)
- **Problem:**
  - No conflict resolution if multiple tunes target same snake simultaneously
  - State transitions don't check if valid (e.g., can't move away while already moved away)
  - Attack state has no target management (only AttackingEnemy.Empty/unused in Phase 1)
  - Frozen state interrupts but doesn't restore previous state after freeze expires
- **Impact:** Edge cases can break snake behavior. Example: Freeze during MovedAway → can't restore movement
- **Safe Modification:**
  1. Add transition guards: `if (_currentState == SnakeState.MovedAway) return;` before SetState
  2. Add previous state memory for stateful transitions
  3. Implement proper Frozen state that saves/restores prior state
  4. Add comprehensive debug logging at every state transition

### Player Input System Assumption
- **Issue:** PlayerController assumes input actions exist
- **Files:** `Assets/_Project/Scripts/Player/PlayerController.cs` (lines 250-261)
- **Problem:** SetupInputActions loads from Resources but crashes if `SnakeEnchanter.inputactions` is missing or malformed
- **Risk:** No fallback if input asset is deleted or in wrong location
- **Safe Modification:**
  1. Add try-catch around InputActionAsset loading
  2. Provide sensible defaults (no-op input)
  3. Test with missing asset to verify graceful degradation

### Canvas UI Generation via Editor Script
- **Issue:** UI created at runtime by CanvasUICreator.cs
- **Files:** `Assets/_Project/Scripts/Editor/CanvasUICreator.cs`
- **Problem:** Multiple GetComponent calls in sequence. No error handling if hierarchy doesn't match expectations.
- **Risk:** If hierarchy is wrong, UI breaks silently
- **Safe Modification:**
  1. Add null checks after every Find() call
  2. Log warnings if expected UI elements not found
  3. Consider pre-creating UI in scene instead of runtime generation

---

## 📊 TEST COVERAGE GAPS

### Snake AI Combat System Untested
- **What's not tested:** Damage delivery, animation triggers, attack cooldowns, range detection
- **Files:** `Assets/_Project/Scripts/Snakes/SnakeAI.cs`
- **Risk:** Death_by_Snakes animation never tested (BACKLOG notes "waiting on Snake Damage")
- **Priority:** HIGH - Core gameplay loop depends on this
- **Needed Tests:**
  1. Verify damage fires when player in bite range (0-0.5 units)
  2. Verify breath attack triggers at 4-7 units
  3. Verify projectile attack at 8+ units
  4. Verify attack cooldown (4 seconds between attacks)
  5. Verify animation plays alongside damage
  6. Verify death animation triggers on player death from snake

### UI Slider Feedback Untested
- **What's not tested:** Slider visual feedback accuracy, zone display, timing state updates
- **Files:** `Assets/_Project/Scripts/UI/TuneSliderUI.cs`
- **Risk:** Slider may not update smoothly or may show wrong visual state
- **Needed Tests:**
  1. Slider position updates correctly (0-1 range)
  2. Zone visualization shows correct green zone
  3. Marker position aligns with slider position
  4. Timing state text (TooEarly/InZone/TooLate) updates correctly

### Mode-Specific Drain Rates Not Verified
- **What's not tested:** Simple vs Advanced mode drain rates, 15% difference
- **Files:** `Assets/_Project/Scripts/Player/HealthSystem.cs` (lines 69-72)
- **Risk:** Balancing may be incorrect. Advanced mode may be too easy or too hard.
- **Needed Tests:**
  1. Verify Simple mode drain: 0.1 HP/sec
  2. Verify Advanced mode drain: 0.115 HP/sec (15% faster)
  3. Calculate actual survival time from 30 HP: ~5 min (simple), ~4.35 min (advanced)

---

## ⚠️ ARCHITECTURAL CONCERNS

### Mixed Animation Responsibility
- **Issue:** Animations controlled from multiple places (HealthSystem, TuneController, potentially SnakeAI)
- **Files:** Multiple - HealthSystem, TuneController, SnakeAI
- **Problem:** No single authority for animation state. Can lead to conflicting triggers (spell animation vs death animation).
- **Fix approach:**
  1. Centralize animation control in new AnimationController component
  2. HealthSystem and TuneController call AnimationController methods instead of Animator directly
  3. AnimationController handles all parameter sets/triggers
  4. Ensures single source of truth for animation state

### Event System Scalability
- **Issue:** GameEvents broadcasts to all listeners on every tune
- **Files:** `Assets/_Project/Scripts/Core/GameEvents.cs`
- **Problem:** As Phase 2/3 adds more listeners (UI, VFX, particles, etc.), event processing grows
- **Risk:** Not immediate concern with current features, but scales poorly with Phase 3 polish
- **Fix approach:**
  1. Monitor event listener count in profiler
  2. Consider event channel pattern (tune-specific events vs broadcast)
  3. Lazy-subscribe UI elements (only subscribe when active)

### SnakeAI Closest-Snake Logic Inefficient
- **Issue:** O(n) scan per tune to find closest targetable snake
- **Files:** `Assets/_Project/Scripts/Snakes/SnakeAI.cs` (lines 393-417)
- **Problem:** With 6+ snakes, every tune success scans all snakes multiple times
- **Impact:** Noticeable frame drop on tune cast in advanced mode with many snakes
- **Fix approach:**
  1. Maintain snake proximity cache updated in fixed time intervals
  2. Use spatial hash grid for O(1) nearest-neighbor lookups
  3. Cache result and invalidate only when snakes move significantly

---

## 🔒 SECURITY CONSIDERATIONS

### No Input Validation on Tune Parameters
- **Issue:** TuneConfig values loaded from ScriptableObject without validation
- **Files:** `Assets/_Project/Scripts/TuneSystem/TuneController.cs` (lines 368-394)
- **Problem:** If triggerZoneStart > triggerZoneEnd, or zone outside 0-1 range, system breaks silently
- **Risk:** Low (designer mistake mostly), but could cause frustration
- **Recommendation:**
  1. Add Odin/inspector validation or ScriptableObject PostImport validation
  2. Clamp zone values to 0-1 range
  3. Swap start/end if backwards

### HealthSystem Drain Rate Exposed in Inspector
- **Issue:** Drain rates are public SerializeFields editable in scene
- **Files:** `Assets/_Project/Scripts/Player/HealthSystem.cs` (lines 69-72)
- **Risk:** Designer could accidentally change drain rates per-level, causing imbalance
- **Recommendation:**
  1. Make drain rates read-only properties
  2. Create separate GameMode configuration ScriptableObject
  3. Load mode-specific drain rates from GameManager, not local serialization

---

## 📋 KNOWN ISSUES FROM BACKLOG (STILL OPEN)

### Spell Animation Timing - Phase 2 High Priority
- Status: OPEN
- Related to: TuneController v2.4 (triggers at key release, not key press)
- See: BACKLOG.md, Line 9-18
- Assigned to: Phase 2 Animation Polish

### Camera Position During Crouch
- Status: OPEN
- Issue: Camera doesn't lower when player crouches (Cinemachine offset not updated)
- Files: PlayerController crouch handling
- Priority: Medium (BACKLOG Medium Priority)

### Cave Textures Neon-Yellow
- Status: OPEN
- Issue: Some cave materials render with wrong shader or emission
- Priority: Medium (visual polish)

### Snake MoveAwayTarget Stacking
- Status: OPEN
- Issue: Multiple snakes move to same target (need per-snake waypoints)
- Priority: Low
- Workaround: Can be disabled in Phase 1

---

## 🎯 RECOMMENDATIONS BY PHASE

### Immediate (Blocking Phase 2 Completion)
1. **Implement Snake AI Patrol & Movement** (Critical)
   - Implement UpdatePatrol or integrate NavMeshAgent
   - Fix infinite MovedAway movement
   - Test with actual snakes moving around cave

2. **Debug All Attack Animations** (Critical)
   - Assign animator to SnakeAI
   - Trigger all 3 attack types
   - Verify damage timing matches animation

3. **Fix Exit Trigger State Transition** (Critical)
   - Implement Win state properly
   - Test full game loop: Play → Win → Results

### Phase 2 Polish
1. Fix Spell Animation Timing (trigger at key press, not release)
2. Implement Closest-Snake caching for performance
3. Fix Camera Crouch Position
4. Update Cave Textures

### Phase 3+ Maintenance
1. Implement proper Animation Controller (centralize animation logic)
2. Optimize Event System (scale to many listeners)
3. Add proper Snake pathfinding with obstacle avoidance
4. Implement spatial partitioning for performance

---

*Concerns audit: 2026-02-13*
