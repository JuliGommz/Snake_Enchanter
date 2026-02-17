# Architecture

**Analysis Date:** 2026-02-13

## Pattern Overview

**Overall:** Event-driven, layer-based architecture with decoupled game systems using GameEvents as central communication hub.

**Key Characteristics:**
- Event-driven communication prevents tight coupling between systems
- Layered architecture: Core (GameManager) → Systems (Player, Snakes, Tunes) → UI
- State machines for complex behaviors (Snake AI, Tune System)
- ScriptableObject-based configuration for balancing
- Single Responsibility Principle: Each script owns one system aspect

## Layers

**Core Layer:**
- Purpose: Manages game state, mode switching, and session tracking
- Location: `Assets/_Project/Scripts/Core/`
- Contains: GameManager (state machine), GameEvents (static event hub)
- Depends on: Nothing (isolated foundation)
- Used by: All systems subscribe to GameEvents

**Player System Layer:**
- Purpose: Player input, movement, camera control, health management
- Location: `Assets/_Project/Scripts/Player/`
- Contains: PlayerController (Cinemachine-based first-person), HealthSystem (HP drain/damage/healing)
- Depends on: GameEvents, InputSystem, Cinemachine, CharacterController
- Used by: Game loop, UI systems

**Tune/Spell System Layer:**
- Purpose: Genshin-style hold-and-release timing mechanic (ADR-008)
- Location: `Assets/_Project/Scripts/TuneSystem/`
- Contains: TuneController (slider logic, input handling), TuneConfig (ScriptableObject configuration)
- Depends on: GameEvents, InputSystem, HealthSystem, TuneConfig
- Used by: Player casting tunes, Snake AI reacting to results

**Snake AI System Layer:**
- Purpose: Snake behavior, state management, tune reaction, attack systems
- Location: `Assets/_Project/Scripts/Snakes/`
- Contains: SnakeAI (state machine with 7 states)
- Depends on: GameEvents, HealthSystem, TuneConfig (for SnakeEffect enum)
- Used by: Game loop for AI updates, Event-driven for tune responses

**Level/Environment Layer:**
- Purpose: Win condition and level mechanics
- Location: `Assets/_Project/Scripts/Level/`
- Contains: ExitTrigger (trigger-based win detection)
- Depends on: GameEvents, HealthSystem
- Used by: Game state transitions

**UI Layer:**
- Purpose: Visual feedback and HUD display
- Location: `Assets/_Project/Scripts/UI/`
- Contains: HealthBarUI (gradient health bar with pulse), TuneSliderUI (segmented slider with marker)
- Depends on: GameEvents, TuneController, HealthSystem
- Used by: Display only (read-only access)

## Data Flow

**Game Start Flow:**

1. GameManager.Awake() → Singleton initialization, auto-find systems
2. GameManager.Start() → Calls StartGame(mode)
3. StartGame(mode) → ApplyModeSettings() to all systems, resets session data
4. PlayerController.Awake() → Loads InputActions, sets up Input System
5. HealthSystem.Awake() → Initializes health, sets drain rate
6. TuneController.Awake() → Loads tune configs, caches delegates
7. SnakeAI.Start() → Finds player, subscribes to tune events, sets Idle state

**Tune Casting Flow:**

1. Player presses key (1-4) → New Input System fires Tune1/2/3/4 action
2. TuneController.OnTuneKeyPressed(n) → StartTune(n, config)
3. TuneController emits GameEvents.TuneStarted(n)
4. TuneSliderUI.OnTuneStarted(n) → Shows slider panel, pre-colors segments
5. Player holds key → TuneController.UpdateSlider() advances _sliderPosition (0→1)
6. Player releases key → ReleaseTune() evaluates position
7. EvaluatePosition() returns TuneResult (TooEarly/Success/TooLate)
8. EndTune(result) applies consequences:
   - TooEarly: GameEvents.TuneFailed(false) - safe fail
   - Success: GameEvents.TuneSuccess() + GameEvents.TuneSuccessWithId(n)
   - TooLate: GameEvents.TuneFailed(true) + HealthSystem.TakeSnakeAttack()

**Tune Success → Snake Reaction:**

1. GameEvents.TuneSuccessWithId(n) fires
2. SnakeAI.OnTuneSuccessWithId(n) receives event
3. Tune 4 (Freeze): ApplyFreeze() called on all snakes (no range check)
4. Tunes 1-3: IsPlayerInRange + IsTargetable + IsClosestTargetableSnake checks
5. ApplyTuneEffect(effect) → SetState(newState)
6. SetState() updates collider, color, animations

**Health & Death:**

1. Passive drain: HealthSystem.ApplyPassiveDrain() each Update (if enabled)
2. Snake attack: SnakeAI.OnTriggerEnter() calls HealthSystem.TakeDamage()
3. Tune success: TuneController.EndTune(Success) triggers GameEvents.TuneSuccess()
4. HealthSystem subscribes OnTuneSuccess → Heal(_healPerTuneSuccess)
5. HP ≤ 0: HealthSystem.Die() plays death animation, emits GameEvents.GameOver()
6. GameManager.OnGameLost() sets state to Lost, disables input

**Win Condition:**

1. Player reaches exit collider
2. ExitTrigger.OnTriggerEnter() fires GameEvents.GameWin()
3. GameManager.OnGameWin() sets state to Won, disables input

**State Management:**

- GameState: MainMenu → Playing → Won/Lost (managed by GameManager)
- SnakeState: Idle ↔ Aggressive, MovedAway, Sleeping, AttackingEnemy, Frozen, Dead
- TuneResult: TooEarly, Success, TooLate (evaluated on key release)

## Key Abstractions

**GameEvents (Static Event Hub):**
- Purpose: Decouple all systems through publish-subscribe pattern
- Examples: `GameEvents.cs`
- Pattern: Static class with C# events, invoked by publishers, subscribed by listeners
- Key events: OnHealthChanged, OnTuneSuccess, OnTuneFailed, OnGameWin, OnGameOver

**Snake State Machine:**
- Purpose: Encapsulate snake behavior in discrete states
- Examples: `Assets/_Project/Scripts/Snakes/SnakeAI.cs` - SnakeState enum
- Pattern: Switch-based state machine in UpdateState(), state transitions via SetState()
- States: Idle, Aggressive, MovedAway, Sleeping, AttackingEnemy, Frozen, Dead

**Tune Slider System (ADR-008):**
- Purpose: Convert time-based input into position-evaluated result (Genshin-style)
- Examples: `Assets/_Project/Scripts/TuneSystem/TuneController.cs`
- Pattern: Position (0-1) evaluated against TriggerZone bounds
- Three outcomes: TooEarly (safe), Success (in zone), TooLate (snake attacks)

**ScriptableObject Configuration:**
- Purpose: Decouple balance values from code, enable Inspector tweaking
- Examples: `Assets/_Project/ScriptableObjects/TuneConfigs/*.asset`
- Pattern: TuneConfig holds duration, triggerZone, audio, visual data per tune

**Targeting System:**
- Purpose: Only closest targetable snake in range reacts to tunes (prevents multi-target)
- Examples: `Assets/_Project/Scripts/Snakes/SnakeAI.cs:IsClosestTargetableSnake()`
- Pattern: Loop through all snakes, compare distances, early return if closer found

## Entry Points

**GameManager:**
- Location: `Assets/_Project/Scripts/Core/GameManager.cs`
- Triggers: Auto-runs on scene load (Awake → Start)
- Responsibilities: Initialize game state, manage mode settings, track session stats

**PlayerController:**
- Location: `Assets/_Project/Scripts/Player/PlayerController.cs`
- Triggers: Awake (input setup), Update (movement/look), LateUpdate (camera pitch after Cinemachine)
- Responsibilities: Read player input, move character, control pitch (Cinemachine handles yaw/position)

**TuneController:**
- Location: `Assets/_Project/Scripts/TuneSystem/TuneController.cs`
- Triggers: OnEnable (input subscription), Update (slider advance), key press/release
- Responsibilities: Manage tune state, evaluate slider position, emit tune events

**SnakeAI:**
- Location: `Assets/_Project/Scripts/Snakes/SnakeAI.cs`
- Triggers: OnEnable (subscribe to tune events), Update (state behavior), OnTriggerEnter (damage)
- Responsibilities: Execute AI state, react to tunes, deal damage on contact

**ExitTrigger:**
- Location: `Assets/_Project/Scripts/Level/ExitTrigger.cs`
- Triggers: OnTriggerEnter (player collision)
- Responsibilities: Detect win condition, emit GameEvents.GameWin()

## Error Handling

**Strategy:** Event-driven with null checks, debug logging, and graceful degradation.

**Patterns:**
- Null checks on component references (e.g., PlayerController auto-finds Camera.main)
- Early returns if preconditions fail (e.g., SnakeAI: if player not in range, return)
- Debug.LogWarning for missing required assets (e.g., InputActionAsset, Animator)
- Clamping values to valid ranges (e.g., HP 0-100, slider 0-1)
- Event handler unsubscription in OnDisable() to prevent memory leaks

## Cross-Cutting Concerns

**Logging:** Debug.Log statements in key entry points, system state changes, and event triggers. Filtered by Editor-only #if UNITY_EDITOR for debug GUI display.

**Validation:** OnValidate() in ScriptableObjects (TuneConfig) ensures zone order and numeric bounds. MonoBehaviour serialization warns if required references missing.

**Authentication:** New Input System only (project rule). No Legacy Input used. InputActionAsset centralized at `Assets/_Project/Data/SnakeEnchanter.inputactions`.

**Cinemachine Integration:** Cinemachine v3.x owns camera position and yaw. PlayerController handles only pitch (vertical look) in LateUpdate() to not conflict with Cinemachine's scheduled update order.

---

*Architecture analysis: 2026-02-13*
