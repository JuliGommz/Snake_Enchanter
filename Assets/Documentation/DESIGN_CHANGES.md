# Design Changes - Snake Enchanter

## Session 16 (2026-02-14): Tune 2 Rename

### Sleep → Dazed

**Reason:** No dedicated sleep animation exists in Toon Snakes Pack. Snake uses Idle animation for stunned/dazed state.

**Changes:**
- **Tune 2** is now **"Daze Command"** (previously "Sleep Command")
- **SnakeState.Sleeping** renamed to **SnakeState.Dazed**
- **Effect:** Snake becomes stunned/dazed (benommen), passive, no collision
- **Animation:** Die (snake collapses and lies on ground)
- **Duration:** 8 seconds (configurable via `_dazedDuration` in Inspector)
- **Visual Feedback:** Blue glow (Material Emission)
- **After Timer:** Snake returns to Idle state (stands up, resumes normal behavior)
- **Future:** UI prompt above snake ("Dazed" + countdown timer)

**Code References:**
- `SnakeAI.cs v1.5.0`: All `Sleeping` → `Dazed`
- `TuneConfig.cs`: Tune 2 effect = Daze
- Player Animation: MC_Cast_Spell_2 (SpellDaze trigger)

**GDD Updates Needed:**
- GDD v1.5: Line 142, 174, 180, 273, 368, 508, 523
- Replace "Sleep" → "Daze" in all future documentation
- Tune 2 description: "Snake becomes dazed/stunned (benommen), passive, no collision"

---

## Session 16 (2026-02-14): Movement Animations

### Directional Slither Animations (Forward/Left/Right)

**Added:** `UpdateMovementAnimation()` method in SnakeAI v1.6.0
- **Trigger:** 3 Bool parameters in Animator
  - `Slither Forward` - Dominant forward/backward movement
  - `Slither Left` - Dominant leftward movement (local space)
  - `Slither Right` - Dominant rightward movement (local space)
- **Conditions:**
  - Aggressive state (chasing player)
  - Idle + Patrolling
  - MovedAway + Moving
- **Auto-disabled:** When snake stops moving (Idle, Dazed, Frozen, Dead)

**Direction Logic:**
- `_lastMoveDirection` tracks movement vector from `MoveTowardsSafe()`
- `InverseTransformDirection()` converts world direction to local (relative to snake's forward)
- Compare `localDirection.z` (forward) vs `localDirection.x` (right) to determine dominant axis
- Selection: `|forward| > |right|` → Forward, `right > 0.1` → Right, `right < -0.1` → Left

**Code:** SnakeAI.cs v1.6.0, Update() + UpdateMovementAnimation() + MoveTowardsSafe()

**Debug Logs:** Slither direction selection with local direction values (fwd, right)

---

## Session 16 (2026-02-14): Attack Creature Targeting Rules

### Snakes ONLY Attack Non-Snake Creatures

**Design Decision:** Snakes do not attack other snakes when using Tune 3 (Attack).

**Reason:**
- Game design: Snakes are tools/allies, not enemies to each other
- Future-proof: Allows adding other creature types (monsters, enemies)
- Tag "Creature" is generic, but SnakeAI components are excluded

**Implementation:**
- `FindNearestCreature()` skips ALL GameObjects with SnakeAI component
- Only targets creatures WITHOUT SnakeAI (future enemies, monsters, etc.)

**Testing:**
- Phase 1: No non-snake creatures exist → Tune 3 will find no target, return to Idle
- Phase 2+: Add other creature types with tag "Creature" → Attack will work

**Code:** SnakeAI.cs v1.5.0, FindNearestCreature() line ~914

---

## Session 16 (2026-02-14): Debug Logging System

### Comprehensive Debug Logs for Testing

**Added:** Debug.Log statements for all critical snake behaviors

**Spell States (SetState method):**
- **Tune 1 (Move):** `[SPELL] Tune 1 (Move) → MovedAway (White glow, collision OFF, delay)`
- **Tune 2 (Daze):** `[SPELL] Tune 2 (Daze) → Dazed (Blue glow, IsDazed=true, collision OFF, timer)`
- **Tune 3 (Attack):** `[SPELL] Tune 3 (Attack) → AttackingEnemy (Yellow glow, collision OFF, delay)`
- **Tune 4 (Freeze):** `[SPELL] Tune 4 (Freeze) → Frozen (Cyan glow, collision ON, timer)`
- **Daze End:** `[DAZE END] Leaving Dazed state, IsDazed=false`
- **State Transitions:** `[STATE] PreviousState → NewState` with timers/colors

**Attack System (TriggerAttack method):**
- **Attack Triggered:** `[ATTACK] AttackType triggered! (Damage, Distance, Delay)`
- Shows which attack type (Bite/Breath/Projectile), damage value, player distance, animation delay

**Attack Creature (StartAttackingEnemy method):**
- **Target Found:** `[ATTACK CREATURE] Target: 'name' at distance X`
- **Target Type:** `Target is Snake, both will neutralize` OR `Target is NOT Snake, destroying`
- **No Target:** `[ATTACK CREATURE] No targetable creature found (tag:Creature, NOT SnakeAI), returning to Idle`

**Slither Animation (UpdateMovementAnimation method):**
- **Direction Selected:** `Slither FORWARD/LEFT/RIGHT (fwd:X, right:Y)`
- Shows local direction values for debugging animation selection

**Purpose:** User requested logs for testing all spell behaviors, slither, dazed state, attack system

**Code:** SnakeAI.cs v1.6.0

---

## BACKLOG - Future Features (Session 16)

### Spell System Enhancements

**1. Two-Level Success System:**
- Level 1: Spell Cast Success (Player timing on slider)
- Level 2: Enemy Enchanted Success (Random chance based on game state)

**2. Player Spell Cooldown:**
- Prevents spam-casting spells
- Inspector-configurable cooldown duration per spell

**3. Player Success Rate System:**
- Percentage-based enchantment chance (50-90%)
- Varies based on Player Health state
- Random roll for each enemy in range

**4. Spell Range System:**
- Inspector-definable range per spell type
- Only enemies within range can be affected
- Visual feedback for range indicator

**5. Dynamic Slider Balancing:**
- Speed variation per spell
- Success zone variation per spell
- Variation based on Player Health state
- Difficulty scaling system

**6. Particle Glow System (Visual):**
- Replace Material Color Change with particle effects
- Maintain original snake color
- External particle glow for state visualization
- State-based particle colors (Move: White, Daze: Blue, Attack: Yellow, Freeze: Cyan)

**7. Enemy Attack System Completion:**
- Current implementation incomplete
- Requires additional development

**Priority:** Medium (Phase 3 - Polish)
**Reason:** Core mechanics functional, these are balancing/polish features

---

## Phase 2 → Phase 3 Handoff (Session 17: 2026-02-15)

### Phase 2 Completion Status

**COMPLETE ✅:**
- SnakeAI v1.7.2 - All core behaviors implemented
- Patrol System (random waypoints)
- Proximity Detection (line-of-sight)
- Range-based Attacks (Bite/Breath/Projectile)
- Spell Responses (Move/Daze/Attack/Freeze)
- Directional Slither Animations (Forward/Left/Right)
- Visual Feedback (Material Emission glow)
- Collision Detection (Environment + Props + Snakes)
- Debug Logging (All behaviors)

**TESTED ✅:**
- Tune 1 (Move): Snake moves to MoveAwayTarget, White glow
- Tune 2 (Daze): 8s timer, Blue glow, Die animation, IsDazed transitions
- Tune 3 (Attack): Snake attacks RobotKyle (Creature), both die
- Attack System: Bite/Breath/Projectile attacks working

**KNOWN ISSUES → Phase 3 Backlog:**

### 🔴 Critical Carryover Items

**1. Tune 4 (Freeze) - Not Functional**
- Status: Code implemented, UI unlocked, but spell doesn't freeze snakes
- Symptom: Slider appears, spell can be cast, but no freeze effect occurs
- Priority: HIGH - core feature not working
- Location: SnakeAI.cs ApplyTuneEffect() + TuneController.cs
- Phase 3 Action: Debug and fix freeze behavior

**2. 3 Areas Implementation**
- Status: Only 1 area (GameLevel scene) exists
- Needed: Tutorial → Main → Finale progression
- Alternative: Scope down to 1 polished area
- Priority: MEDIUM - design decision needed

**3. Backend API Integration**
- POST `/api/game-session` - Session stats
- GET `/api/leaderboard` - Bestenliste
- GET `/api/player-stats` - Aggregated stats
- Priority: MEDIUM - not critical for gameplay

**4. Main Menu Polish**
- Status: Basic functional menu
- Needed: Mode selection, settings, quit
- Priority: LOW - Phase 4 acceptable

**5. Result Screen Polish**
- Status: Basic Win/Lose screen
- Needed: Stats display, retry button, leaderboard integration
- Priority: LOW - Phase 4 acceptable

**6. Untested Features**
- Slither Left/Right animations (code exists, only Forward tested)
- Death_by_Snakes animation (no test scenario exists)

### 🎯 Phase 3 Focus Areas

**Audio System (NEW):**
- Flute melodies (4 tracks, 5-12s each)
- Snake SFX (hiss, bite, breath, daze sounds)
- UI sounds (slider, success/fail feedback)
- Ambient music (cave atmosphere, tension)

**Visual Feedback (NEW):**
- Particle effects (spell cast, attack impact, HP restoration)
- Screen effects (shake on fail/damage, vignette on low HP)
- Animation polish (smooth transitions, hit reactions)

**UI Polish (NEW):**
- Health Bar animations (drain/fill, gradient, pulse)
- Timing Meter visual polish
- Menu transitions

**Level Polish (NEW):**
- Atmospheric lighting
- Props for visual interest
- Exit portal glow

**Creature Combat System (BACKLOG):**
- Kampf-System: Snake vs Creature mit HP
- Creature kann Snake angreifen
- Snake überlebt/stirbt basierend auf HP-Interaktion
- Current: Both die (Phase 1 simplified) - works for now

### 📊 Branch Merge Plan

**Branch:** `feature/enemy-setup`
**Target:** `main`
**Status:** Ready after user completes scene work (Snake prefabs placement)

**Merge Checklist:** See `.planning/MERGE_CHECKLIST.md`

### 🎮 Testing Before Phase 3

**Required:**
- [ ] Slither Left/Right test (code exists, untested)
- [ ] All 6 Snake prefabs placed in GameLevel scene
- [ ] All MoveAwayTargets positioned correctly
- [ ] Full playthrough: Cast all 4 Tunes at least once
- [ ] Scene saved (Ctrl+S in Unity)

**Branch Merge:**
- [ ] Git status clean (no uncommitted Unity meta files)
- [ ] Merge feature/enemy-setup → main
- [ ] Delete branch (local + remote)
- [ ] Screenshot: `Media/Screenshots/2026-02-15_Phase2Complete.png`
