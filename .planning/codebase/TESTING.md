# Testing Patterns

**Analysis Date:** 2026-02-13

## Test Framework

**Framework Status:**
- **No automated test framework detected** — No Jest, Vitest, NUnit, or Unity Test Framework installed
- **No test files found** — No `*.test.cs`, `*.spec.cs`, or `Tests/` directory in project
- **Current Testing Approach:** Manual testing via Editor and Debug GUI visualizations
- **Build Target:** Phase 1 (Playable) — automated testing planned for Phase 3+

**Assertion Library:**
- Not applicable (no framework installed)
- Future: Unity Test Framework (UTF) recommended when formal testing begins

**Run Commands:**
```bash
# Manual Testing via Unity Editor
# Play Mode in editor with built-in Debug GUI
# Console window shows Debug.Log output

# No automated test runner available in current project
```

## Test File Organization

**Current Structure:**
- Tests are manual and integrated into scene gameplay
- No separate test directories or test fixtures
- Debug systems embedded in Editor-only code blocks

**Location Pattern (for when tests are added):**
- Recommended: `Assets/Tests/` directory (separate from production code)
- Unit tests: `Assets/Tests/[System]Tests.cs`
- Integration tests: `Assets/Tests/Integration/`
- Example: `Assets/Tests/HealthSystemTests.cs`, `Assets/Tests/SnakeAITests.cs`

**Naming Convention (for future tests):**
- `[ClassName]Tests.cs` - Unit test class
- `[ClassName]IntegrationTests.cs` - Integration test class
- Test methods: `Test[Scenario][ExpectedResult]` (e.g., `TestHealthDamageReducesHP`, `TestSnakeFreezeLocksState`)

## Manual Testing & Debug Infrastructure

**Editor Debug GUI System:**

Every major system includes Editor-only `OnGUI()` debug panel (pattern from `CLAUDE.md`):

**HealthSystem Debug (PlayerController.cs, lines 298-329):**
```csharp
#if UNITY_EDITOR
    [Header("Debug - Editor Only")]
    [SerializeField] private bool _showDebugInfo = true;

    private void OnGUI()
    {
        if (!_showDebugInfo) return;
        GUILayout.BeginArea(new Rect(10, 10, 300, 170));
        GUILayout.Label($"<b>HealthSystem Debug</b>", ...);
        GUILayout.Label($"Current HP: {_currentHealth:F1} / {_maxHealth}");

        if (GUILayout.Button("Test Damage (20)"))
            TakeDamage(20);
        if (GUILayout.Button("Test Heal (15)"))
            Heal(15);
    }
#endif
```

**GameManager Debug (GameManager.cs, lines 368-413):**
```csharp
#if UNITY_EDITOR
    [Header("Debug - Editor Only")]
    [SerializeField] private bool _showDebugInfo = true;

    private void OnGUI()
    {
        if (!_showDebugInfo) return;
        GUILayout.BeginArea(new Rect(Screen.width - 310, 10, 300, 220));
        GUILayout.Label($"<b>GameManager Debug</b>", ...);
        GUILayout.Label($"State: {_currentState} | Mode: {_gameMode}");
        GUILayout.Label($"Session Time: {SessionTime:F1}s");

        if (GUILayout.Button("Restart Game"))
            RestartGame();
        if (GUILayout.Button("Toggle Mode"))
            ApplyModeSettings(...);
    }
#endif
```

**TuneController Debug (TuneController.cs, lines 551-615):**
```csharp
#if UNITY_EDITOR
    [Header("Debug - Editor Only")]
    [SerializeField] private bool _showDebugInfo = true;

    private void OnGUI()
    {
        if (!_showDebugInfo) return;
        GUILayout.BeginArea(new Rect(10, 170, 400, 250));
        GUILayout.Label("<b>TuneController Debug (ADR-008 Slider)</b>", ...);
        GUILayout.Label($"Holding: {_isHolding} | Tune: {_currentTuneNumber}");
        GUILayout.Label($"Slider Position: {_sliderPosition:F3}");
        GUILayout.Label($"State: {CurrentTimingState}");

        DrawSliderVisualization();  // ASCII bar representation

        if (GUILayout.Button("Unlock Tune 4"))
            UnlockTune4();
        if (GUILayout.Button("Toggle Mode"))
            SetSimpleMode(!_isSimpleMode);
    }
#endif
```

**SnakeAI Debug (SnakeAI.cs, lines 488-508):**
```csharp
#if UNITY_EDITOR
    [Header("Debug - Editor Only")]
    [SerializeField] private bool _showDebugLabel = true;

    private void OnGUI()
    {
        if (!_showDebugLabel) return;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2f);
        if (screenPos.z > 0)
        {
            float labelX = screenPos.x - 60;
            float labelY = Screen.height - screenPos.y - 15;
            GUI.color = _currentState == SnakeState.Aggressive ? Color.red : Color.white;
            GUI.Label(new Rect(labelX, labelY, 120, 30),
                $"{_snakeName}: {_currentState}");
        }
    }
#endif
```

**Gizmo Visualization Patterns:**

Debug gizmos for spatial debugging (always active in Scene view):

```csharp
// SnakeAI.cs, lines 472-486
private void OnDrawGizmosSelected()
{
    // Command range (yellow sphere)
    Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
    Gizmos.DrawWireSphere(transform.position, _commandRange);

    // Move away target (blue line)
    if (_moveAwayTarget != null)
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, _moveAwayTarget.position);
        Gizmos.DrawWireSphere(_moveAwayTarget.position, 0.5f);
    }
}
```

```csharp
// ExitTrigger.cs, lines 112-138
private void OnDrawGizmosSelected()
{
    // Draw trigger bounds (green wireframe)
    Collider col = GetComponent<Collider>();
    if (col != null)
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.matrix = transform.localToWorldMatrix;

        if (col is BoxCollider box)
            Gizmos.DrawCube(box.center, box.size);
        else if (col is SphereCollider sphere)
            Gizmos.DrawSphere(sphere.center, sphere.radius);
    }
}
```

## Testing Strategies (Current & Future)

### Current: Manual Integration Testing

**Session Testing (HealthSystem.cs lines 142-150):**
```csharp
private void Update()
{
    if (_isDead) return;

    if (_enablePassiveDrain)
    {
        ApplyPassiveDrain();  // Continuously drain HP — monitor via Debug GUI
    }
}
```

**Event-Driven Session Tracking (GameManager.cs lines 308-365):**
```csharp
// Tracks during gameplay
private int _successfulTuneCasts = 0;
private int _failedTuneCasts = 0;
private int _snakeAttackCount = 0;

private void OnTuneSuccessTracking()
{
    _successfulTuneCasts++;  // Counted each cast
}

private void LogSessionSummary(bool success)
{
    Debug.Log("========== SESSION SUMMARY ==========");
    Debug.Log($"Successful Tunes: {_successfulTuneCasts}");
    Debug.Log($"Failed Tunes: {_failedTuneCasts} (Early: {_tooEarlyCount}, Late: {_tooLateCount})");
    Debug.Log($"Snake Attacks: {_snakeAttackCount}");
    Debug.Log("=====================================");
}
```

### Future: Unit Test Patterns (Recommended)

**When adding automated tests (Phase 3+), follow these patterns:**

**Health System Test (example structure):**
```csharp
[TestFixture]
public class HealthSystemTests
{
    private HealthSystem _healthSystem;
    private GameObject _testObj;

    [SetUp]
    public void Setup()
    {
        _testObj = new GameObject();
        _healthSystem = _testObj.AddComponent<HealthSystem>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(_testObj);
    }

    [Test]
    public void TestTakeDamageReducesHealth()
    {
        int initialHP = Mathf.RoundToInt(_healthSystem.CurrentHealth);
        _healthSystem.TakeDamage(10);
        Assert.AreEqual(initialHP - 10, Mathf.RoundToInt(_healthSystem.CurrentHealth));
    }

    [Test]
    public void TestHealIncreasesHealth()
    {
        int initialHP = Mathf.RoundToInt(_healthSystem.CurrentHealth);
        _healthSystem.Heal(15);
        Assert.AreEqual(initialHP + 15, Mathf.RoundToInt(_healthSystem.CurrentHealth));
    }

    [Test]
    public void TestNegativeDamageIsRejected()
    {
        int initialHP = Mathf.RoundToInt(_healthSystem.CurrentHealth);
        _healthSystem.TakeDamage(-10);  // Should be rejected
        Assert.AreEqual(initialHP, Mathf.RoundToInt(_healthSystem.CurrentHealth));
    }
}
```

**Snake AI State Machine Test (example structure):**
```csharp
[TestFixture]
public class SnakeAITests
{
    private SnakeAI _snake;
    private GameObject _snakeObj;

    [SetUp]
    public void Setup()
    {
        _snakeObj = new GameObject();
        _snakeObj.AddComponent<Collider>();
        _snakeObj.AddComponent<Renderer>();
        _snake = _snakeObj.AddComponent<SnakeAI>();
    }

    [Test]
    public void TestSnakeStartsInIdleState()
    {
        Assert.AreEqual(SnakeState.Idle, _snake.CurrentState);
    }

    [Test]
    public void TestApplyFreezeLocksSnake()
    {
        _snake.ApplyFreeze();
        Assert.AreEqual(SnakeState.Frozen, _snake.CurrentState);
    }

    [Test]
    public void TestFreezeTimeoutReturnsToIdle()
    {
        _snake.ApplyFreeze();
        _snake.Update();  // Frame 1
        // Wait for timeout...
        Assert.AreEqual(SnakeState.Idle, _snake.CurrentState);
    }
}
```

## Mocking & Isolation

**Framework:** Not currently used (would use NSubstitute or Moq if added)

**Current Event-Based Isolation (GameEvents.cs):**

Systems communicate through static events instead of direct references — naturally testable:

```csharp
// GameEvents provides clean injection points
public static event Action OnTuneSuccess;
public static event Action<int> OnTuneSuccessWithId;
public static event Action<bool> OnTuneFailed;

// Test can invoke events without full game setup
[Test]
public void TestSnakeReactsToTuneSuccess()
{
    // Setup
    var snake = new SnakeAI();

    // Invoke event
    GameEvents.TuneSuccess();

    // Verify state change
    Assert.IsTrue(snake.IsInCharmState);
}
```

**What to Mock (future tests):**
- GameEvents — publish test events
- Input actions — simulate key presses without InputSystem
- Colliders — mock trigger enters/exits
- Animators — mock trigger/parameter sets

**What NOT to Mock:**
- MonoBehaviour lifecycle (Awake, Start, Update) — use PlayMode tests
- CharacterController movement — integration test only
- State machines — test actual state transitions
- Physics/colliders — use real physics

## Fixtures & Test Data

**Test Data Location (recommended for Phase 3+):**
- `Assets/Tests/Fixtures/` - Test configurations
- `Assets/Tests/Fixtures/TuneConfigs/` - Sample TuneConfig ScriptableObjects
- `Assets/Tests/Fixtures/Scenes/` - Minimal test scenes

**Recommended Fixtures:**

**TuneConfig Test Fixture:**
```csharp
public static TuneConfig CreateTestTune(
    float duration = 3f,
    float zoneStart = 0.4f,
    float zoneEnd = 0.65f)
{
    var config = ScriptableObject.CreateInstance<TuneConfig>();
    config.duration = duration;
    config.triggerZoneStart = zoneStart;
    config.triggerZoneEnd = zoneEnd;
    return config;
}
```

**Factory Pattern (current usage in SnakeAI.cs):**
```csharp
// Snakes created via Prefab instantiation in scene
// No factory class needed — direct instantiation works for current phase
```

## Coverage & Current State

**Current Coverage:**
- **Manual:** ~60% via Play Mode + Debug GUI visualization
- **Automated:** 0% — no test framework active
- **Gap:** Core state machine paths (Snake AI) untested automatically

**Key Untested Areas (automated):**
- State transitions (Idle → Aggressive → Frozen)
- Event handler chains (Tune success → Heal → Health UI update)
- Timer/cooldown logic (Aggressive duration, Freeze timeout)
- Edge cases (null player, out-of-range snakes, rapid tune casting)
- Input System edge cases (simultaneous key presses, rapid key release)

**Manual Test Checklist (for Phase 1 Sign-Off):**

**Snake AI:**
- [ ] Idle state displays green, doesn't move
- [ ] Aggressive state displays red, stays aggressive for configured duration
- [ ] Frozen state displays cyan, cannot be charmed
- [ ] MovedAway state displays gray, moves to target
- [ ] Sleeping state displays blue, collision disabled
- [ ] Freeze (Tune 4) affects all snakes globally
- [ ] Closest snake only reacts to tunes
- [ ] Out-of-range snakes ignore commands

**Tune System:**
- [ ] Slider appears on key press
- [ ] Position moves left-to-right over duration
- [ ] Green zone visible before input
- [ ] Orange zone success area
- [ ] Red zone for too-late
- [ ] SUCCESS releases when in zone
- [ ] TOO EARLY releases before zone (safe fail)
- [ ] TOO LATE releases after zone (snake attacks)
- [ ] Simple Mode expands zone size

**Health System:**
- [ ] HP displays current value (0-100)
- [ ] HP bar fills 100% at max health
- [ ] Passive drain reduces HP over time (if enabled)
- [ ] Successful tune heals +15 HP
- [ ] Snake attack deals configured damage
- [ ] HP caps at max (no over-heal)
- [ ] HP clamps at 0 (no negative)
- [ ] Death animation triggers at HP=0

## Session Tracking (Data Collection)

**Current Tracking (GameManager.cs, lines 120-128):**
```csharp
private int _successfulTuneCasts = 0;
private int _failedTuneCasts = 0;
private int _tooEarlyCount = 0;
private int _tooLateCount = 0;
private int _snakeAttackCount = 0;
private int _totalDamageTaken = 0;
private int _totalHPRestored = 0;
private int _startingHP;
```

**Logged at Game End (Session Summary):**
- Mode (Simple/Advanced)
- Result (WIN/LOSE)
- Duration
- Starting/Ending HP
- Successful/Failed tune casts
- Snake attack count
- Total damage taken
- Total HP restored

**Future (Phase 2):** Send to backend API via POST `/api/game-session`

---

*Testing analysis: 2026-02-13*
