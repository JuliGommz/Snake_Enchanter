# Phase 7: Spell System - Research

**Researched:** 2026-02-18
**Domain:** Unity C# — pickup triggers, game pause, dynamic HUD, shield state, cooldown/charge system
**Confidence:** HIGH (all findings verified against existing codebase)

---

<user_constraints>
## User Constraints (from CONTEXT.md)

### Locked Decisions

#### Scroll Pickup Behavior
- Collection method: walk-over trigger collider OR mouse click — both should work
- Scrolls glow brighter as player approaches (proximity-based intensity)
- On collection: game pauses, center-screen panel appears showing scroll name, 2-line description, and assigned key number
- Player presses any key to dismiss the panel and resume gameplay
- Scroll disappears from world after collection (no VFX needed — instant removal is fine)
- Key rebinding is nice-to-have (not v1.0 scope) — keys stay hardcoded as 1, 2, 3

#### Scroll Placement & Progression
- Fixed unlock order: Tune 1 (Move) -> Tune 2 (Daze) -> Tune 3 (Shield)
- 1 scroll per QuestRoom: clean 1:1 mapping
- Scrolls are not inside the QuestRooms but in the cave system, placed strategically before each room's snake confrontation
- 2 scrolls on the main path, 1 in a side exploration area (the last one too, probably)
- Player starts with zero tunes — first scroll pickup is the tutorial moment

#### HUD / Locked Tune UI
- HUD starts completely empty — no tune slots visible at all
- Each scroll pickup adds a new slot to the HUD
- Each slot shows: key icon with number (sketch/simplified key shape) + spell name + color
- Keys are always visible on each slot — no memorization needed
- Pressing an unassigned key does nothing (silently ignored)
- When a scroll is collected, the new HUD slot appears with a clear visual transition (color fill, noticeable)

#### Tune 3: Shield Behavior
- Duration: 8 seconds ([SerializeField] for tuning)
- Blocks the next incoming attack (bite, breath, or projectile), then breaks
- If no attack comes within 8s, shield expires naturally
- Visual: screen edge glow (blue/gold) while shield is active — first-person friendly, no 3D model work
- Block feedback: screen flash + shatter sound when shield absorbs an attack
- Cannot recast while active — Tune 3 key is locked/non-responsive while shield is up
- Shield state tracked on player (HealthSystem or new ShieldComponent)

#### Spell Casting Rules
- HP heal only when snake is charmed — Move/Daze must actually affect a snake to restore HP. Shield casts do NOT heal. Casting with no snake in range = no heal.
- Range requirement for Move/Daze: Player must be within spell range of a snake to cast Move or Daze. If no snake in range, cast is blocked.
- Shield castable anywhere: Shield is self-targeted, no range check needed.
- HUD range indicator: Subtle indicator on tune slots when a castable snake is in range. No world-space visuals.
- Spell cooldown (both modes): Each spell has a [SerializeField] cooldown timer. Cannot recast until cooldown expires.
- Basic mode: Unlimited spell charges, only cooldown limits casting.
- Advanced mode: Limited charges per spell ([SerializeField] configurable per spell). When charges are depleted, spell is unavailable. Charges do NOT regenerate.
- Charge counts deferred to Phase 13 balancing pass — implement the system, use placeholder values.

### Claude's Discretion
- Scroll 3D model/visual design (can be a simple glowing scroll mesh or particle placeholder)
- Exact proximity glow curve (linear, ease-in, etc.)
- Pause panel layout and styling details
- Shield screen-edge glow implementation approach (post-processing, UI overlay, etc.)
- HUD slot layout/positioning (horizontal bar, vertical stack, etc.)
- Exact shield color palette
- Cooldown UI representation (timer overlay, grayed slot, radial fill, etc.)
- Range detection method (sphere overlap, distance check, etc.)
- Charge display on HUD (number, pips, etc.)

### Deferred Ideas (OUT OF SCOPE)
- Key rebinding (let player choose which key per tune) — nice-to-have, not Phase 7
- Tune 4 slot — removed entirely. If a 4th tune is ever wanted, it would be a new phase
- Attack Creature tune — requires second creature + fight system, deferred to Phase 12 (EXT-02 RobotKyle)
</user_constraints>

---

## Summary

Phase 7 is a pure Unity C# scripting phase. There are no new external libraries to install — everything needed (New Input System, Unity UI, TextMeshPro, URP post-processing) is already in the project. The work divides into four clearly separable systems: scroll pickup, TuneController refactor, Shield behavior, and spell casting rules.

The most important architectural insight is that the existing codebase already has all the wiring points needed. `GameEvents` has `OnTuneSuccessWithId` which every `SnakeAI` listens to. `TuneController` already gates Tune 4 behind a boolean flag — the same pattern extends cleanly to Tunes 1-3 via a `bool[] _tuneUnlocked` array. `HealthSystem` already has `TakeSnakeAttack()` — the Shield needs to intercept calls here. The pause mechanic (`Time.timeScale = 0`) is already referenced by `GameEvents.OnGamePaused`.

The second important insight is around the **heal-on-charm** rule. Currently `HealthSystem.OnTuneSuccessHealing()` fires on every `OnTuneSuccess` event, regardless of whether a snake was actually in range. This must change: the heal must only trigger when `SnakeAI.ApplyTuneEffect()` actually executes on a snake (not on the global success event). The cleanest fix is a new `GameEvents.SnakeCharmed(int tuneNumber)` event that SnakeAI fires, and HealthSystem heals only on that.

**Primary recommendation:** Build all four sub-systems in strict isolation — SpellUnlockSystem, TuneController refactor, ShieldComponent, and SpellCastingRules. Wire them together through GameEvents to match the existing architecture. Do not modify HealthSystem's core damage path; intercept it in a new ShieldComponent instead.

---

## Standard Stack

### Core (already in project — no installation needed)
| Component | Version | Purpose | Why Standard |
|-----------|---------|---------|--------------|
| Unity New Input System | project-installed | Tune key detection (Tune1-3 actions already in .inputactions) | Project rule — NEVER legacy Input |
| UnityEngine.UI + TextMeshPro | project-installed | HUD slots, pause panel, cooldown display | Already used in HealthBarUI, TuneSliderUI |
| Unity URP | project-installed | Screen-edge glow via Image overlay (or URP post-processing volume) | Already configured |
| GameEvents.cs | project | Event bus for all cross-system communication | Already used by all systems |
| ScriptableObject (TuneConfig) | project | Spell data — duration, zone, effect | Already exists, needs Shield effect added |

### Supporting (discretion areas)
| Component | Version | Purpose | When to Use |
|-----------|---------|---------|-------------|
| Physics.OverlapSphere | Unity built-in | Range check for Move/Daze cast — "is any snake in range?" | Simpler than per-snake distance queries |
| Coroutine | Unity built-in | Shield duration timer, HUD slot reveal animation | Cleaner than Update-based timers for one-shot events |
| Time.timeScale = 0 | Unity built-in | Pause during scroll pickup panel | The existing GameEvents.OnGamePaused already signals this |

### Alternatives Considered
| Instead of | Could Use | Tradeoff |
|------------|-----------|----------|
| UI Image overlay for screen edge glow | URP Full-Screen Render Pass | Overlay is simpler, immediate. URP pass requires a custom renderer feature — overkill for a border glow |
| OverlapSphere range check in TuneController | Query each SnakeAI individually | Sphere is O(n) physics query vs. O(n) manual loop — equivalent, but sphere is standard Unity idiom |
| New ShieldComponent on Player | Adding shield logic directly to HealthSystem | Separate component keeps HealthSystem focused on HP only (Single Responsibility, matches existing architecture) |

---

## Architecture Patterns

### Recommended Project Structure

New scripts to create:

```
Assets/_Project/Scripts/
├── TuneSystem/
│   └── SpellUnlockSystem.cs   # Tracks which tunes are unlocked, exposes IsUnlocked(int)
├── Level/
│   └── SpellScrollPickup.cs   # Trigger + click pickup, proximity glow, fires unlock event
├── Player/
│   └── ShieldComponent.cs     # Shield state, duration timer, intercepts TakeSnakeAttack()
└── UI/
    └── SpellHUDController.cs  # Dynamic slot creation/reveal, range indicator, cooldown display
```

TuneConfig.cs gets a new `SnakeEffect.Shield` value added.
TuneController.cs gets refactored from 4 fixed configs to an unlock array and casting guards.
GameEvents.cs gets 3 new events.
HealthSystem.cs gets a small change: heal-on-charm logic removed from `OnTuneSuccess`, moved to new event.

### Pattern 1: Unlock Gate in TuneController

The current code uses a single `_tune4Unlocked` bool. Replace with an array:

```csharp
// TuneController.cs — replace 4 individual config fields with array approach
[SerializeField] private TuneConfig[] _tuneConfigs = new TuneConfig[3]; // index 0=Tune1, 1=Tune2, 2=Tune3
private bool[] _tuneUnlocked = new bool[3]; // all false at start

private void OnTuneKeyPressed(int tuneNumber)
{
    if (_isHolding) return;
    int idx = tuneNumber - 1;
    if (idx < 0 || idx >= _tuneUnlocked.Length) return;
    if (!_tuneUnlocked[idx]) return; // Silently ignore — key does nothing
    // ... existing cooldown + charge check ...
    StartTune(tuneNumber, _tuneConfigs[idx]);
}

public void UnlockTune(int tuneNumber)
{
    int idx = tuneNumber - 1;
    if (idx < 0 || idx >= _tuneUnlocked.Length) return;
    _tuneUnlocked[idx] = true;
    GameEvents.TuneUnlocked(tuneNumber); // New event for HUD
}
```

**Confidence:** HIGH — directly extends the existing `_tune4Unlocked` pattern already in TuneController.

### Pattern 2: Scroll Pickup — Dual Collection Method

```csharp
// SpellScrollPickup.cs
public class SpellScrollPickup : MonoBehaviour
{
    [SerializeField] private int _tuneNumberToUnlock = 1; // 1, 2, or 3
    [SerializeField] private string _scrollName = "Scroll of Movement";
    [SerializeField] private string _scrollDescription = "Hold [1] and release in the glowing zone.";
    [SerializeField] private float _glowMaxDistance = 5f;
    [SerializeField] private float _glowMaxIntensity = 2f;

    private Renderer _renderer;
    private Transform _playerTransform;
    private bool _collected = false;

    private void Update()
    {
        if (_collected) return;
        UpdateProximityGlow();
    }

    private void OnTriggerEnter(Collider other) // Walk-over
    {
        if (_collected || !other.CompareTag("Player")) return;
        Collect();
    }

    // Mouse click: use OnMouseDown() or a Raycast from PlayerController
    private void OnMouseDown()
    {
        if (_collected) return;
        Collect();
    }

    private void Collect()
    {
        _collected = true;
        gameObject.SetActive(false); // Instant removal
        GameEvents.ScrollCollected(_tuneNumberToUnlock, _scrollName, _scrollDescription);
    }

    private void UpdateProximityGlow()
    {
        float dist = Vector3.Distance(transform.position, _playerTransform.position);
        float t = 1f - Mathf.Clamp01(dist / _glowMaxDistance);
        // Apply to emission intensity (URP Lit material)
        _renderer.material.SetFloat("_EmissionIntensity", t * _glowMaxIntensity);
    }
}
```

**Important:** `OnMouseDown()` only works if the GameObject has a Collider and the camera can raycast it. In first-person with Cinemachine, the Main Camera still performs raycasts. This is confirmed to work in Unity — no special setup needed.

**Confidence:** HIGH — standard Unity pattern. `OnTriggerEnter` and `OnMouseDown` are both documented Unity collision/input callbacks.

### Pattern 3: Game Pause During Panel Display

The project already has `GameEvents.OnGamePaused` and `Time.timeScale`. The scroll pickup panel uses:

```csharp
// SpellUnlockSystem.cs — handles the pause panel
private void OnScrollCollected(int tuneNumber, string name, string description)
{
    // 1. Unlock the tune
    _tuneController.UnlockTune(tuneNumber);

    // 2. Show panel
    ShowScrollPanel(tuneNumber, name, description);

    // 3. Pause game
    Time.timeScale = 0f;
    GameEvents.GamePaused(true);

    // 4. Wait for any key — use New Input System
    StartCoroutine(WaitForAnyKey());
}

private IEnumerator WaitForAnyKey()
{
    // WaitForSecondsRealtime ignores timeScale=0
    yield return new WaitForSecondsRealtime(0.1f); // Small buffer to avoid instant dismiss
    yield return new WaitUntil(() => Keyboard.current.anyKey.wasPressedThisFrame);
    HideScrollPanel();
    Time.timeScale = 1f;
    GameEvents.GamePaused(false);
}
```

**Critical:** `WaitForSeconds` is blocked by `Time.timeScale = 0`. Use `WaitForSecondsRealtime` for the any-key buffer. This is a known Unity gotcha.

**Confidence:** HIGH — verified against Unity coroutine documentation behavior.

### Pattern 4: Shield Component — Intercept Attack Path

The Shield must not modify HealthSystem internals. Instead it sits between the snake attack and HealthSystem:

```csharp
// ShieldComponent.cs — on Player GameObject, same as HealthSystem
public class ShieldComponent : MonoBehaviour
{
    [SerializeField] private float _shieldDuration = 8f;
    private bool _isShieldActive = false;
    private HealthSystem _healthSystem;

    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();
    }

    public bool IsShieldActive => _isShieldActive;

    public void ActivateShield()
    {
        if (_isShieldActive) return; // No recast
        _isShieldActive = true;
        GameEvents.ShieldActivated(); // New event — HUD/screen glow reacts
        StartCoroutine(ShieldTimer());
    }

    private IEnumerator ShieldTimer()
    {
        yield return new WaitForSeconds(_shieldDuration);
        if (_isShieldActive)
        {
            DeactivateShield(absorbed: false);
        }
    }

    // Called by HealthSystem.TakeSnakeAttack() — checked BEFORE applying damage
    public bool TryAbsorbAttack()
    {
        if (!_isShieldActive) return false;
        DeactivateShield(absorbed: true);
        GameEvents.ShieldAbsorbedAttack(); // New event — screen flash + shatter sound
        return true;
    }

    private void DeactivateShield(bool absorbed)
    {
        _isShieldActive = false;
        GameEvents.ShieldDeactivated(absorbed);
    }
}
```

**HealthSystem.TakeSnakeAttack() modification** (minimal — one guard):
```csharp
public void TakeSnakeAttack()
{
    // Check shield first — shield component is on same GameObject
    var shield = GetComponent<ShieldComponent>(); // Cache in Awake in practice
    if (shield != null && shield.TryAbsorbAttack()) return; // Absorbed!
    TakeDamage(_snakeAttackDamage);
}
```

**Confidence:** HIGH — follows existing component composition pattern (TuneController already gets HealthSystem via GetComponent).

### Pattern 5: Heal-on-Charm Fix

Currently HealthSystem heals on every `OnTuneSuccess`. This must change to heal only when a snake is actually affected.

**New event in GameEvents:**
```csharp
public static event Action<int> OnSnakeCharmed; // int = tuneNumber
public static void SnakeCharmed(int tuneNumber) => OnSnakeCharmed?.Invoke(tuneNumber);
```

**SnakeAI.ApplyTuneEffect() fires it after successfully applying Move or Daze:**
```csharp
public void ApplyTuneEffect(SnakeEffect effect)
{
    switch (effect)
    {
        case SnakeEffect.Move:
            SetState(SnakeState.MovedAway);
            GameEvents.SnakeCharmed(1); // Fires heal
            break;
        case SnakeEffect.Daze:
            SetState(SnakeState.Dazed);
            _stateTimer = _dazeDuration;
            GameEvents.SnakeCharmed(2); // Fires heal
            break;
        // Shield = no snake interaction, no heal
    }
}
```

**HealthSystem removes** `OnTuneSuccess` subscription and subscribes to `OnSnakeCharmed` instead.

**Range gate in TuneController:** For Tune 1 and 2, before calling `StartTune`, check if any snake is in range. If not, block the cast silently. For Tune 3 (Shield), no range check.

```csharp
private bool HasSnakeInRange(float range)
{
    Collider[] hits = Physics.OverlapSphere(transform.position, range);
    foreach (var hit in hits)
    {
        if (hit.GetComponent<SnakeAI>() != null) return true;
    }
    return false;
}
```

**Confidence:** HIGH — OverlapSphere is standard Unity; the pattern mirrors existing SnakeAI proximity detection.

### Pattern 6: Dynamic HUD Slots

```csharp
// SpellHUDController.cs
public class SpellHUDController : MonoBehaviour
{
    [SerializeField] private GameObject _slotPrefab; // One slot: key icon + name label + color bg
    [SerializeField] private Transform _slotsContainer; // Horizontal layout group
    [SerializeField] private TuneController _tuneController;

    // Per-slot data
    private GameObject[] _slots = new GameObject[3];
    private Image[] _slotBgs = new Image[3];
    private TextMeshProUGUI[] _slotLabels = new TextMeshProUGUI[3];
    private Image[] _cooldownOverlays = new Image[3]; // Radial fill for cooldown

    private void OnEnable()
    {
        GameEvents.OnTuneUnlocked += OnTuneUnlocked;
        GameEvents.OnTuneCooldownStarted += OnCooldownStarted;
        GameEvents.OnSnakeInRangeChanged += OnRangeChanged;
    }

    private void OnTuneUnlocked(int tuneNumber)
    {
        int idx = tuneNumber - 1;
        // Instantiate slot if not exists
        if (_slots[idx] == null)
        {
            _slots[idx] = Instantiate(_slotPrefab, _slotsContainer);
            // Configure: key number, name, color
            // Animate reveal: start transparent, lerp to full alpha
            StartCoroutine(RevealSlot(_slots[idx]));
        }
    }
}
```

**Slot reveal animation:** Use a Coroutine that lerps `CanvasGroup.alpha` from 0 to 1 over 0.5s. `CanvasGroup` is the standard Unity tool for fading UI elements — no animation clips needed.

**Confidence:** HIGH — CanvasGroup alpha lerp is documented Unity UI pattern.

### Pattern 7: Cooldown System

```csharp
// In TuneController — per-tune cooldown tracking
private float[] _cooldownTimers = new float[3]; // time remaining
[SerializeField] private float[] _cooldownDurations = { 3f, 4f, 5f }; // default per tune

// In OnTuneKeyPressed — before StartTune:
if (_cooldownTimers[idx] > 0f) return; // On cooldown, do nothing

// In EndTune on Success:
_cooldownTimers[idx] = _cooldownDurations[idx];
GameEvents.TuneCooldownStarted(tuneNumber, _cooldownDurations[idx]);

// In Update:
for (int i = 0; i < _cooldownTimers.Length; i++)
{
    if (_cooldownTimers[i] > 0f)
        _cooldownTimers[i] -= Time.deltaTime;
}
```

**Advanced mode charge system:**
```csharp
[SerializeField] private int[] _spellCharges = { 5, 5, 3 }; // placeholder values
private int[] _remainingCharges;

// In Awake (Advanced mode): _remainingCharges = (int[])_spellCharges.Clone();
// In OnTuneKeyPressed: if (_remainingCharges[idx] <= 0) return;
// In EndTune on Success: _remainingCharges[idx]--;
```

**Confidence:** HIGH — straightforward array-indexed timer pattern.

### Anti-Patterns to Avoid

- **Do not call `GameEvents.TuneSuccess()` for Shield casts.** The existing `OnTuneSuccess` is wired to the heal path. Shield success should fire a new `GameEvents.ShieldActivated()` event only.
- **Do not modify `Time.timeScale` from multiple places.** Only `SpellUnlockSystem` pauses for scroll panels. If the game already has a pause system in GameManager, coordinate through a shared pause stack or a bool guard.
- **Do not use `WaitForSeconds` in coroutines that run during `timeScale=0`.** Always use `WaitForSecondsRealtime` for real-time delays during pauses.
- **Do not cache SnakeAI references in TuneController.** Use `Physics.OverlapSphere` at cast time — snakes can die, be added, or move. A cached list becomes stale.
- **Do not add shield logic to HealthSystem directly.** HealthSystem has one responsibility. ShieldComponent is the correct place.
- **Do not modify TuneConfig ScriptableObjects for runtime state** (cooldown timers, charges). ScriptableObjects persist changes in Editor. Runtime state belongs in MonoBehaviour fields.

---

## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| Fade-in UI slot reveal | Custom alpha tweening system | `CanvasGroup` + Coroutine lerp | One-liner, built into Unity, no extra code |
| Screen edge glow | Custom shader | UI `Image` with a border texture + `CanvasGroup` alpha | Screen-space overlay is trivial; shader requires URP feature authoring |
| "Any key" detection during pause | Poll all KeyCodes in Update | `Keyboard.current.anyKey.wasPressedThisFrame` | New Input System already in project; works even at timeScale=0 |
| Range detection for spell targeting | Manual distance loop over all snakes | `Physics.OverlapSphere` | Unity's physics broadphase is optimized; direct loop is equivalent but longer |
| Proximity glow curve | Custom easing function | `Mathf.Clamp01(dist / maxDist)` linear, or `Mathf.SmoothStep` | Built-in, zero allocation |

---

## Common Pitfalls

### Pitfall 1: WaitForSeconds Freezes During Pause
**What goes wrong:** Coroutine using `WaitForSeconds` appears to hang forever when `Time.timeScale = 0f` is set for the scroll panel.
**Why it happens:** `WaitForSeconds` scales with `Time.timeScale`. At 0 it never advances.
**How to avoid:** Use `WaitForSecondsRealtime` for the any-key buffer. Shield timer uses `WaitForSeconds` (should pause with game — this is correct behavior).
**Warning signs:** Panel appears, game freezes, pressing keys does nothing.

### Pitfall 2: OnMouseDown Blocked by UI
**What goes wrong:** Clicking a scroll does nothing because a UI Canvas (set to Screen Space - Overlay) intercepts all mouse events.
**Why it happens:** Screen Space Overlay Canvas captures all input before it reaches 3D GameObjects.
**How to avoid:** The Canvas in this project already exists. Ensure the HUD Canvas does NOT have a full-screen transparent `Image` raycast target. Check `Graphic Raycaster` component. Alternatively, use a Raycast from PlayerController (cast forward from camera) instead of `OnMouseDown` — more control and more robust for first-person.
**Warning signs:** Walk-over works but click never fires.

### Pitfall 3: ScriptableObject Data Mutated at Runtime
**What goes wrong:** Cooldown timers or charge counts accidentally stored in TuneConfig ScriptableObject fields get saved to disk in Editor play mode.
**Why it happens:** ScriptableObjects are asset instances; modifying fields in Play Mode mutates the asset.
**How to avoid:** All runtime state (timers, charges, cooldown) lives in TuneController MonoBehaviour fields. TuneConfig remains a read-only data container.
**Warning signs:** After stopping play mode, TuneConfig inspector shows changed values.

### Pitfall 4: Heal Fires on Empty Casts
**What goes wrong:** Player holds Tune 1, releases in zone with no snake nearby, and still receives healing.
**Why it happens:** Current `HealthSystem.OnTuneSuccessHealing` fires on every `OnTuneSuccess`, regardless of snake proximity.
**How to avoid:** Remove the `OnTuneSuccess` subscription from HealthSystem. Add `OnSnakeCharmed` event. Only fire it from `SnakeAI.ApplyTuneEffect()`. This is the central architectural change for Phase 7.
**Warning signs:** HP increases even when no snake is visible/nearby.

### Pitfall 5: Shield Can Be Recast During Active Duration
**What goes wrong:** Player casts Shield, then immediately casts it again (double-stack or reset timer).
**Why it happens:** TuneController doesn't know about Shield's active state unless ShieldComponent exposes it.
**How to avoid:** In `TuneController.OnTuneKeyPressed`, for tune 3 (Shield): check `_shieldComponent.IsShieldActive` before calling `StartTune`. If active, return silently.
**Warning signs:** Shield glow flickers or duration resets on second press.

### Pitfall 6: Tune3 Still Fires SpellAttack Animator Trigger
**What goes wrong:** TuneController v2.4 has `case 3 => "SpellAttack"` hardcoded in the animator trigger switch. After Phase 7 refactor, Tune 3 is now Shield — the animation name is wrong.
**Why it happens:** The animator trigger mapping was written for the old 4-tune layout.
**How to avoid:** Update the animator switch in `EndTune` to `case 3 => "SpellShield"` (or whatever animation exists). Add a Shield spell animation or reuse an existing one. Check with user what animation to use.
**Warning signs:** Console log shows "Triggered animation 'SpellAttack'" for Shield cast.

### Pitfall 7: GameEvents Static State Across Play Sessions
**What goes wrong:** If `SpellUnlockSystem` or `TuneController` doesn't reset unlock state on game restart, the player starts with all spells unlocked after first play.
**Why it happens:** Static events in `GameEvents` are cleared by `ClearAllEvents()` but unlock flags (in MonoBehaviour fields) persist until scene reload or explicit reset.
**How to avoid:** Add a `ResetUnlocks()` call in `SpellUnlockSystem` that resets `_tuneUnlocked[]` to all-false. Call it from `GameManager.RestartGame()`.
**Warning signs:** Restarting game starts with HUD showing all 3 spell slots.

---

## Code Examples

### New Events to Add to GameEvents.cs

```csharp
// In GameEvents.cs — add to existing event hub
// Source: follows existing pattern in file

// Scroll / Unlock
public static event Action<int, string, string> OnScrollCollected; // tuneNumber, name, desc
public static event Action<int> OnTuneUnlocked; // tuneNumber

// Shield
public static event Action OnShieldActivated;
public static event Action<bool> OnShieldDeactivated; // bool = absorbed an attack
public static event Action OnShieldAbsorbedAttack;

// Snake charmed (replaces heal-on-TuneSuccess)
public static event Action<int> OnSnakeCharmed; // tuneNumber

// Cooldown
public static event Action<int, float> OnTuneCooldownStarted; // tuneNumber, duration
public static event Action<int> OnTuneCooldownExpired; // tuneNumber

// Range indicator
public static event Action<bool, int> OnSnakeInRangeChanged; // inRange, tuneNumber

// Invokers (follow existing pattern)
public static void ScrollCollected(int tune, string name, string desc)
    => OnScrollCollected?.Invoke(tune, name, desc);
public static void TuneUnlocked(int tune) => OnTuneUnlocked?.Invoke(tune);
public static void ShieldActivated() => OnShieldActivated?.Invoke();
public static void ShieldDeactivated(bool absorbed) => OnShieldDeactivated?.Invoke(absorbed);
public static void ShieldAbsorbedAttack() => OnShieldAbsorbedAttack?.Invoke();
public static void SnakeCharmed(int tune) => OnSnakeCharmed?.Invoke(tune);
public static void TuneCooldownStarted(int tune, float duration)
    => OnTuneCooldownStarted?.Invoke(tune, duration);
```

### SnakeEffect Enum Update (TuneConfig.cs)

```csharp
// Replace existing SnakeEffect enum in TuneConfig.cs
public enum SnakeEffect
{
    Move,    // Snake moves out of the way (Tune 1)
    Daze,    // Snake becomes dazed/stunned (Tune 2)
    Shield   // Player gains a shield — no snake effect (Tune 3)
}
// Remove: Attack, Freeze (no longer used in Phase 7)
```

### Proximity Glow via URP Emission

```csharp
// SpellScrollPickup.cs — UpdateProximityGlow()
// Source: URP Material property setter pattern
private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

private void UpdateProximityGlow()
{
    if (_playerTransform == null || _renderer == null) return;
    float dist = Vector3.Distance(transform.position, _playerTransform.position);
    float t = 1f - Mathf.Clamp01(dist / _glowMaxDistance);
    // t=0 at max distance, t=1 at player position
    Color baseColor = Color.yellow;
    _renderer.material.SetColor(EmissionColor, baseColor * (t * _glowMaxIntensity));
    // Ensure emission keyword is enabled
    _renderer.material.EnableKeyword("_EMISSION");
}
```

**Note:** URP Lit shader uses `_EmissionColor` (not `_EmissionIntensity`). The color value's brightness controls intensity. `renderer.material` creates a material instance per-renderer — correct for runtime modification.

### Screen Edge Glow via UI Image Overlay

Simplest approach: a full-screen UI Image with a border texture (transparent center, colored edges). Controlled by alpha:

```csharp
// ShieldScreenGlow.cs — attached to a Canvas Image
[SerializeField] private Image _borderImage; // Full-screen, transparent center sprite
[SerializeField] private Color _shieldColor = new Color(0.3f, 0.5f, 1f, 0.6f); // Blue

private void OnEnable()
{
    GameEvents.OnShieldActivated += ShowGlow;
    GameEvents.OnShieldDeactivated += HideGlow;
    GameEvents.OnShieldAbsorbedAttack += FlashGlow;
}

private void ShowGlow()
{
    _borderImage.color = _shieldColor;
    _borderImage.gameObject.SetActive(true);
}

private void HideGlow(bool absorbed) => _borderImage.gameObject.SetActive(false);

private void FlashGlow()
{
    StopAllCoroutines();
    StartCoroutine(FlashCoroutine());
}

private IEnumerator FlashCoroutine()
{
    _borderImage.color = Color.white; // White flash on absorb
    yield return new WaitForSeconds(0.1f);
    _borderImage.gameObject.SetActive(false);
}
```

A border sprite can be a simple radial gradient texture (black center transparent, colored edges) created in Unity via Texture2D or imported as a sprite.

---

## Integration Map: What Changes, What Stays

| File | Change Type | What Changes |
|------|-------------|--------------|
| `GameEvents.cs` | Additive | +8 new events (ScrollCollected, TuneUnlocked, Shield*, SnakeCharmed, Cooldown*) |
| `TuneConfig.cs` | Modify enum | SnakeEffect: Remove Attack/Freeze, Add Shield |
| `TuneController.cs` | Refactor | 4 fixed configs → array[3]; unlock gate; cooldown/charge logic; Shield range skip; Tune 3 animator trigger rename |
| `HealthSystem.cs` | Small fix | Remove OnTuneSuccess heal subscription; subscribe to OnSnakeCharmed instead |
| `SnakeAI.cs` | Small addition | Fire GameEvents.SnakeCharmed() in ApplyTuneEffect for Move and Daze |
| `SpellUnlockSystem.cs` | New | Listens to ScrollCollected; calls TuneController.UnlockTune(); manages pause panel |
| `SpellScrollPickup.cs` | New | Walk-over + click pickup; proximity glow; fires ScrollCollected |
| `ShieldComponent.cs` | New | Shield state, duration, TryAbsorbAttack(), events |
| `SpellHUDController.cs` | New | Dynamic slots; reveals on TuneUnlocked; range indicator; cooldown overlay |

---

## Open Questions

1. **Shield Animator Trigger Name**
   - What we know: TuneController fires animator triggers on success (`SpellMove`, `SpellDaze`, `SpellAttack`, `SpellFear`). Tune 3 is now Shield.
   - What's unclear: Does a "SpellShield" animation state exist in the Pirate animator? Or should it reuse an existing trigger?
   - Recommendation: Check Animator window for Pirate character. If no Shield animation, reuse `SpellMove` or add a `SpellShield` trigger that maps to idle/pose. Clarify with user before implementing.

2. **Scroll Prefab Visual**
   - What we know: "Claude's Discretion" — can be simple glowing mesh.
   - What's unclear: Is there any scroll-like mesh in the existing asset packs (Caves Parts Set, Dwarven Pack)?
   - Recommendation: Check project assets for a scroll or book mesh. If none, use a Cylinder with emissive material as placeholder. Decision doesn't block any code.

3. **Range Check Value for Move/Daze**
   - What we know: Existing SnakeAI uses `_commandRange` to gate spell reception. TuneController needs a cast-side range check.
   - What's unclear: Should the cast-side range equal `_commandRange` from SnakeAI, or be a separate TuneController value?
   - Recommendation: Add a `[SerializeField] private float _spellCastRange = 8f` to TuneController. Start with a value matching SnakeAI's detection range. Tune separately — they serve different purposes (can I cast vs. can the snake respond).

4. **SnakeAI Targeting for Shield Cast**
   - What we know: When Tune 3 is cast (Shield), `GameEvents.TuneSuccessWithId(3)` fires. SnakeAI currently maps tune 3 to `SnakeEffect.Attack`. This mapping must be removed.
   - What's unclear: Should SnakeAI simply ignore tune 3, or should GameEvents not fire TuneSuccessWithId for Shield at all?
   - Recommendation: In TuneController.EndTune, for Tune 3 (Shield), do NOT fire `GameEvents.TuneSuccessWithId(3)`. Instead fire only `GameEvents.ShieldActivated()`. This is cleaner — no SnakeAI code changes for tune routing needed.

5. **Mouse-Click Pickup vs. First-Person Camera**
   - What we know: Cursor is locked during gameplay (`Cursor.lockState = CursorLockMode.Locked`). `OnMouseDown()` requires a visible, unlocked cursor OR a physics Raycast from the camera forward.
   - What's unclear: With cursor locked, `OnMouseDown()` may not fire correctly in all Unity versions.
   - Recommendation: Implement click pickup as a Raycast from camera forward direction on left mouse button press (read from New Input System), fired in `PlayerController` or a separate `PlayerInteraction` script. More robust and consistent with the first-person setup.

---

## Sources

### Primary (HIGH confidence)
- Existing codebase: `TuneController.cs` v2.4 — unlock pattern, input action subscriptions, EndTune logic
- Existing codebase: `GameEvents.cs` v1.1 — event hub pattern, static event + invoker structure
- Existing codebase: `HealthSystem.cs` v1.3 — OnTuneSuccess heal subscription (to be replaced)
- Existing codebase: `SnakeAI.cs` v1.8.5 — ApplyTuneEffect, OnTuneSuccessWithId, command range check
- Existing codebase: `SnakeEnchanter.inputactions` — confirmed Tune1/Tune2/Tune3/Tune4 actions already exist
- Existing codebase: `TuneSliderUI.cs` v2.1 — CanvasGroup/dynamic UI patterns to follow
- Unity Documentation (training data, HIGH confidence for stable APIs): `Time.timeScale`, `WaitForSecondsRealtime`, `Physics.OverlapSphere`, `OnMouseDown`, `OnTriggerEnter`, `CanvasGroup.alpha`, `Keyboard.current.anyKey`

### Secondary (MEDIUM confidence)
- Training data: URP Lit material `_EmissionColor` property — standard URP property, stable across 2022 LTS
- Training data: `Shader.PropertyToID` for cached property access — Unity best practice since 2019+

### Tertiary (LOW confidence — verify in editor)
- `OnMouseDown()` behavior with locked cursor in first-person mode — recommend testing; fallback is camera Raycast approach documented in Open Questions #5

---

## Metadata

**Confidence breakdown:**
- Standard Stack: HIGH — all components already in project, no new dependencies
- Architecture: HIGH — all patterns extend existing code with verified extension points
- Pitfalls: HIGH — 6 of 7 pitfalls derived directly from existing code patterns; 1 (OnMouseDown + locked cursor) is LOW, flagged
- Code Examples: HIGH — all examples follow existing code conventions and verified Unity APIs

**Research date:** 2026-02-18
**Valid until:** 2026-03-18 (stable Unity 2022 LTS APIs; no fast-moving dependencies)
