# Coding Conventions

**Analysis Date:** 2026-02-13

## Naming Patterns

**Files:**
- `[SystemName].cs` - Single responsibility per file
- Examples: `SnakeAI.cs`, `PlayerController.cs`, `TuneController.cs`, `GameEvents.cs`
- Pattern: PascalCase, descriptive names reflecting class/system purpose
- Namespace folders match file organization: `Scripts/Core/`, `Scripts/Snakes/`, `Scripts/Tunes/`, etc.

**Classes/Types:**
- PascalCase: `SnakeAI`, `GameManager`, `TuneController`, `HealthSystem`
- Public enums: `SnakeState`, `GameMode`, `GameState`, `TuneResult`, `SnakeEffect`
- Apply `[RequireComponent]` attribute on MonoBehaviour classes that depend on specific components

**Functions/Methods:**
- Public methods: PascalCase (`StartGame`, `ApplyTuneEffect`, `ResetSnake`, `TakeDamage`)
- Private methods: PascalCase (`UpdateState`, `SetState`, `OnTuneSuccessWithId`, `HandleCameraLook`)
- Event handlers: `On[Event]` pattern (`OnTuneSuccessWithId`, `OnTuneFailed`, `OnTriggerEnter`, `OnGUI`)
- Coroutines: PascalCase ending with `Coroutine` or describing action (`UpdateSlider`)
- No get/set prefix for simple accessors — use properties instead

**Properties:**
- Public properties: PascalCase with backing private field (`CurrentState`, `IsTargetable`, `SliderPosition`)
- Backing fields: `_camelCase` with leading underscore (`_currentHealth`, `_sliderPosition`, `_isHolding`)
- Read-only properties expose private fields: `public float CurrentHealth => _currentHealth;`

**Variables:**
- Local variables: `camelCase` (`position`, `damage`, `tuneNumber`, `activeSegmentIndex`)
- Private SerializeFields: `_camelCase` (`_commandRange`, _tuneController`, `_sliderPosition`)
- Static readonly hashes for Animator: `private static readonly int [ParameterName]Hash = Animator.StringToHash("[name]");` — example: `private static readonly int SpeedHash = Animator.StringToHash("Speed");`

**Constants & Static Values:**
- Constants: UPPERCASE_WITH_UNDERSCORES if configurable constants, otherwise inline
- Magic numbers: Extract to named SerializeField for balancing (`_commandRange = 8f`, `_maxHealth = 100`)
- No raw string literals in code for repeated values — use constants or SerializeField

## Code Style

**Formatting:**
- No explicit formatter (no .editorconfig or CSharpier detected)
- Observed style: Allman-style braces (opening brace on new line for methods)
- Indentation: 4 spaces (verified in source files)
- Line length: No hard limit observed, reasonable wrapping (~100-120 characters)

**Linting:**
- No ESLint/Roslyn analyzer configured
- Unity 2022 LTS default C# conventions apply
- Nullable reference types not enforced

## Import Organization

**Order:**
1. `using System;` - Base system types
2. `using UnityEngine;` - Core Unity types
3. `using UnityEngine.InputSystem;` - Package imports
4. `using UnityEngine.UI;` - UI-specific imports
5. `using TMPro;` - TextMesh Pro (when needed)
6. `using SnakeEnchanter.Core;` - Custom namespace imports (alphabetical)
7. `using SnakeEnchanter.Tunes;` - Other project namespaces

**Namespace Imports - DO NOT USE:**
- `UnityEngine.Input` (Legacy Input) — MUST use `UnityEngine.InputSystem` only
- Internal/private APIs outside SnakeEnchanter namespace

**Path Aliases:**
- Not detected in codebase
- No alias usage needed; project namespaces are explicit and flat

## Error Handling

**Patterns:**

**Null Checks:**
```csharp
// Pattern 1: Early return
if (_playerTransform == null) return;

// Pattern 2: Defensive FindComponent
if (_renderer == null)
{
    _renderer = GetComponentInChildren<Renderer>();
}

// Pattern 3: Validation at Awake
if (_controller == null)
{
    Debug.LogError("PlayerController: CharacterController component missing!", this);
    return;
}
```

**Edge Case Handling (HealthSystem.cs):**
```csharp
// Reject invalid negative values
if (amount < 0)
{
    Debug.LogWarning($"HealthSystem: TakeDamage called with negative value ({amount}). Ignoring.");
    return;
}

// Clamp to valid ranges
_currentHealth = Mathf.Max(_currentHealth, 0f);
_currentHealth = Mathf.Min(_currentHealth, _maxHealth);

// Dead state check prevents duplicate processing
if (_isDead) return;
```

**State Validation (SnakeAI.cs):**
```csharp
// Check targetability before applying effects
if (!IsPlayerInRange || !IsTargetable) return;
if (!IsClosestTargetableSnake()) return;

// Guard against re-entrance
if (_hasBeenTriggered) return;
```

**No custom exceptions** — Uses Unity Debug logging and return early pattern

## Logging

**Framework:** `Debug.Log()`, `Debug.LogWarning()`, `Debug.LogError()`

**Patterns:**

**Success/State Changes:**
```csharp
Debug.Log($"SnakeAI ({_snakeName}): {previousState} → {newState}");
Debug.Log($"TuneController: Started Tune {tuneNumber} | Duration: {_activeDuration}s | Zone: {_activeZoneStart:F2}-{_activeZoneEnd:F2}");
Debug.Log($"GameManager: Game started — Mode: {mode}");
```

**Failures/Warnings:**
```csharp
Debug.LogWarning($"SnakeAI ({_snakeName}): No GameObject with tag 'Player' found!");
Debug.LogWarning("HealthSystem: Heal called with negative value ({amount}). Ignoring.");
```

**Critical Errors:**
```csharp
Debug.LogError("GameManager: No instance found in scene!");
Debug.LogError("TuneController: InputActionAsset not assigned!");
```

**Formatted Output (with context):**
- Include class name: `$"SnakeAI ({_snakeName}): "`
- Include values: `{value:F2}` for floats, `{count}` for ints
- Include state transitions: `{previousState} → {newState}`
- Include result: `| Result: {outcome}`

**Conditional Debug GUI (Editor only):**
```csharp
#if UNITY_EDITOR
    [SerializeField] private bool _showDebugInfo = true;

    private void OnGUI()
    {
        if (!_showDebugInfo) return;
        // Debug display code
    }
#endif
```

## Comments

**When to Comment:**

**DO Comment:**
- Complex state machine logic: `// Phase 1: Static, just waiting` (SnakeAI.cs, line 224)
- Non-obvious design decisions: `// Cinemachine handles position (follows CameraTarget)` (PlayerController.cs comment header)
- Authorship/AI-assistance marks (academic requirement): Header comments in every file classify AI assistance
- ADR references: `// ADR-008: Slider moves from 0 to 1`
- Workarounds: `// Fix B-001: lambdas can't be unsubscribed`

**DO NOT Comment:**
- Self-documenting code: `_currentHealth = Mathf.Max(_currentHealth, 0f);` needs no comment
- Obvious loops/conditionals: `if (_isHolding) return;` is clear
- Method names explain intent: `ApplyTuneEffect()` doesn't need "// Apply the tune effect"

**JSDoc/XML Comments:**

Extensive use of `/// <summary>` documentation:
```csharp
/// <summary>
/// Updates current state behavior each frame.
/// </summary>
private void UpdateState()

/// <summary>
/// Is player within command range?
/// </summary>
public bool IsPlayerInRange { ... }

/// <summary>
/// Applies the effect of a successful tune on this snake.
/// </summary>
public void ApplyTuneEffect(SnakeEffect effect)
```

**Authorship Header (REQUIRED - Academic Standards):**
Every script includes 45+ line header with:
- File name and purpose
- Project info
- Developer name
- Version history
- Authorship classification (`[AI-ASSISTED]` mark)
- Dependencies
- Design rationale
- NEVER REMOVE — academic requirement

Example from `SnakeAI.cs` (lines 1-44):
```csharp
/*
====================================================================
* SnakeAI - Basic snake behavior and tune interaction
====================================================================
* Project: Snake Enchanter
* Developer: Julian Gomez
* Version: 1.0
* AUTHORSHIP CLASSIFICATION: [AI-ASSISTED]
====================================================================
*/
```

## Function Design

**Size:** 20-50 lines average (verified in codebase)
- State machine methods: ~40 lines (`UpdateState`)
- Event handlers: ~10-20 lines
- Utility methods: ~10-15 lines
- No excessive nesting — max 3-4 levels

**Parameters:**
- Max 2-3 parameters per function
- Use SerializeFields for configuration instead of long parameter lists
- Event handlers use standard signatures: `Action`, `Action<T>`, `Action<T, U>`
- Avoid output parameters — return new values or use events

**Return Values:**
- Boolean for checks: `IsPlayerInRange`, `IsTargetable`, `IsClosestTargetableSnake()`
- Enum for multi-state: `EvaluatePosition()` returns `TuneResult`
- Void for state updates: `UpdateState()`, `SetState()`, `ApplyTuneEffect()`
- Properties expose private fields: read-only where appropriate (`CurrentState => _currentState`)

**Exit Patterns:**
```csharp
// Early return for guards
if (_isDead) return;
if (!_isHolding) return;

// Guard clauses prevent nesting
if (!IsPlayerInRange || !IsTargetable) return;
if (!IsClosestTargetableSnake()) return;

// Switch statements instead of cascading ifs
switch (_currentState)
{
    case SnakeState.Idle: break;
    case SnakeState.Aggressive: break;
}
```

## Module Design

**Exports:**
- One public class per file (exception: data enums in same file as class)
- Public properties expose state: `CurrentHealth`, `SliderPosition`, `IsTargetable`
- Public methods handle interactions: `StartGame()`, `ApplyTuneEffect()`, `TakeDamage()`
- Private methods handle internal state: `UpdateState()`, `SetState()`, `ApplyPassiveDrain()`

**Internal Organization (#region blocks):**
```csharp
#region Configuration
    // SerializeField declarations
#endregion

#region Private Fields
    // Cached references, state
#endregion

#region Properties
    // Public getters
#endregion

#region Unity Lifecycle
    // Awake, Start, OnEnable, OnDisable, Update, LateUpdate, OnTriggerEnter
#endregion

#region Core Logic
    // Main methods: UpdateState, SetState, ApplyTuneEffect
#endregion

#region Event Handlers
    // Subscription callbacks: OnTuneSuccessWithId, OnTuneFailed
#endregion

#region Public Methods
    // API methods: StartGame, ResetSnake, ApplyFreeze
#endregion

#region Debug Helpers
#if UNITY_EDITOR
    // Debug GUI, gizmos
#endif
#endregion
```

**No barrel files** — Each system is directly imported
- Import: `using SnakeEnchanter.Snakes;` then `SnakeAI snake = ...`
- No index.ts or aggregator pattern

**Event Communication Pattern:**
- All game-wide events in `GameEvents.cs` (static class)
- Systems subscribe in `OnEnable()`, unsubscribe in `OnDisable()`
- Use cached delegates for event handlers to enable proper unsubscription (B-001 pattern in TuneController.cs)
- Example: `private System.Action<InputAction.CallbackContext> _onTune1Started; ... OnEnable: _tune1Action.started += _onTune1Started;`

**Caching Pattern (Performance):**
```csharp
// Cache GetComponent references in Awake
private CharacterController _controller;

private void Awake()
{
    _controller = GetComponent<CharacterController>();
}

// Cache Animator parameter hashes
private static readonly int SpeedHash = Animator.StringToHash("Speed");
_animator.SetFloat(SpeedHash, speed);  // Use hash, not string
```

---

*Convention analysis: 2026-02-13*
