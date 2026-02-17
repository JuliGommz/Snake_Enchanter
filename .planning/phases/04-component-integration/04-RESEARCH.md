# Phase 4: Component Integration - Research

**Researched:** 2026-02-17
**Domain:** Unity NavMeshAgent — component configuration on prefabs, dual-system coexistence with transform.position movement
**Confidence:** HIGH

---

## Summary

Phase 4 adds NavMeshAgent components to 6 snake prefabs and initializes them in code as inactive (`isStopped = true`). The purpose is to have the component present and configured without it controlling movement yet — the old `MoveTowardsSafe()` system stays in place.

The most critical finding from research: **`updatePosition = true` (the NavMeshAgent default) will conflict with `MoveTowardsSafe()`.** When `updatePosition = true`, the NavMeshAgent simulation position is synchronized bidirectionally with `transform.position`. The simulation constrains the position to the NavMesh surface — meaning every frame after `MoveTowardsSafe()` moves the snake, the agent may "correct" the transform position back to the nearest NavMesh surface point. This causes position jitter or snapping.

**The fix is mandatory for Phase 4**: Set `_agent.updatePosition = false` in `Awake()`. This decouples the agent simulation from the transform. The agent exists but does not affect movement. This is the standard Unity pattern for dual-system agents (animation root motion, physics, or legacy movement coexisting with NavMeshAgent). When Phase 5 enables NavMesh movement, `updatePosition` is set back to `true` and the agent takes over.

The agent type must match what was baked in Phase 3: Humanoid agent type with Height 0.5, Radius 0.3. The NavMeshAgent component defaults (Height 2.0, Radius 0.5) are wrong for this project and must be changed to match.

**Primary recommendation:** Add NavMeshAgent to all 6 prefabs in the Unity Inspector, configure settings to match Phase 3 bake, then in SnakeAI.Awake() call `GetComponent<NavMeshAgent>()`, set `updatePosition = false`, `updateRotation = false`, and `isStopped = true`. Verify by running Play mode and confirming snakes still move via old system with no position jitter.

---

## Standard Stack

### Core Components
| Component | Version | Purpose | Why This |
|-----------|---------|---------|----------|
| `NavMeshAgent` (UnityEngine.AI) | Built-in module, Unity 2022.3 | Pathfinding agent component | Required for Phase 5 NavMesh movement; must be present on prefab before movement code |
| `com.unity.ai.navigation` | 2.0.9 (already installed) | NavMeshSurface integration | Provides the NavMesh data the agent traverses |

### Key Properties for Phase 4 Initialization
| Property | Phase 4 Value | Default | Why |
|----------|--------------|---------|-----|
| `isStopped` | `true` | false | Prevents agent from generating or following any path |
| `updatePosition` | **`false`** | **true** | CRITICAL: Prevents agent from overriding `transform.position` set by `MoveTowardsSafe()` |
| `updateRotation` | `false` | true | Prevents agent from overriding snake's custom rotation logic |
| `speed` | `patrolSpeed` (1.5f) | 3.5f | Match existing patrol speed for when Phase 5 enables movement |
| `stoppingDistance` | `0.2f` | 0f | Match current arrival threshold in UpdatePatrol() |
| `autoBraking` | `true` | true | Standard — keep enabled |
| `radius` | `0.3f` | 0.5f | Match Phase 3 bake setting AND SphereCast radius in MoveTowardsSafe() |
| `height` | `0.5f` | 2.0f | Match Phase 3 bake setting (snake vertical size) |
| `baseOffset` | `0f` | 0f | Snake pivot is at ground level — no adjustment needed |

---

## Architecture Patterns

### Pattern 1: Passive Agent Initialization (Phase 4 Pattern)

**What:** NavMeshAgent added to prefab but fully passive — no path, no movement, no position control.

**When to use:** Exactly Phase 4's scenario — agent component must exist on the prefab, but the old movement system must not be disrupted.

**The dual-system coexistence contract:**
- `updatePosition = false` → agent does NOT override `transform.position`
- `updateRotation = false` → agent does NOT override `transform.rotation`
- `isStopped = true` → agent has no active path to follow even if updatePosition were true
- No call to `SetDestination()` → agent simulation is idle

This pattern is verified by Unity official documentation for `updatePosition`:
> "Setting updatePosition to false can be used to enable explicit control of the transform position via script."

**Awake() initialization (correct pattern):**
```csharp
// Source: Unity ScriptReference AI.NavMeshAgent, verified 2026-02-17
private NavMeshAgent _agent;

void Awake()
{
    // Existing SnakeAI Awake() code (keep all of it) ...
    _collider = GetComponent<Collider>();
    _renderer = GetComponentInChildren<Renderer>();
    _animator = GetComponent<Animator>();
    _originalPosition = transform.position;
    _originalRotation = transform.rotation;
    // ... (MoveAwayTarget detach code) ...

    // Phase 4 addition — MUST BE AFTER existing Awake() code
    _agent = GetComponent<NavMeshAgent>();
    if (_agent != null)
    {
        // CRITICAL: Decouple agent from transform before any movement occurs
        _agent.updatePosition = false;   // Do NOT let agent override transform.position
        _agent.updateRotation = false;   // Do NOT let agent override transform.rotation

        // Set agent config to match Phase 5 values (no effect while stopped)
        _agent.speed = _moveSpeed;       // Will be patrolSpeed in Phase 5
        _agent.stoppingDistance = 0.2f;

        // Stop agent — no path, no movement
        _agent.isStopped = true;
    }
}
```

**Why Awake() and not Start():**
- `GetComponent<NavMeshAgent>()` works in Awake() — the component is already attached to the GameObject
- `Start()` is too late if any other script in Start() queries the agent state
- The existing SnakeAI.Awake() already runs — the NavMeshAgent init appends to it
- `Start()` is reserved for finding other GameObjects (player reference, GameManager) — keep that separation clean

### Pattern 2: Agent Type Matching

**What:** The NavMeshAgent's `agentTypeID` must match the agent type used when NavMesh was baked in Phase 3.

**Phase 3 bake settings (from SUMMARY.md):**
- Agent Type: Humanoid
- Height: 0.5
- Radius: 0.3

**NavMeshAgent component defaults (wrong for this project):**
- Height: 2.0 (must change to 0.5)
- Radius: 0.5 (must change to 0.3)
- Agent Type: Humanoid (correct, leave as is)

**Verification in Inspector:** After adding NavMeshAgent to a prefab, the "Agent Type" dropdown must show "Humanoid". The "Radius" and "Height" fields in the component are the OBSTACLE AVOIDANCE geometry settings — they should match the bake to produce consistent behavior, but the `agentTypeID` is what determines which NavMesh surface the agent can use.

**"Failed to create agent" error root cause:** This error appears when the agent's `agentTypeID` does not match any baked NavMesh surface in the scene, OR when the agent's position at spawn is too far from the NavMesh. Since Phase 3 baked with Humanoid agent type, all NavMeshAgent components must also use Humanoid agent type.

### Pattern 3: Inspector Settings Order

**What:** When adding NavMeshAgent to prefabs, set all inspector fields in one pass to avoid unnecessary prefab diffs.

**Correct order for 6 prefab updates:**
1. Open prefab in Prefab Mode (double-click in Project window)
2. Select root GameObject (the one with SnakeAI component)
3. Add Component → AI → Nav Mesh Agent
4. Set in Inspector (one pass):
   - Radius: **0.3**
   - Height: **0.5**
   - Base Offset: **0** (verify — should already be 0)
   - Speed: **1.5** (matches `_moveSpeed` serialized field default)
   - Stopping Distance: **0.2**
   - Auto Braking: **enabled** (checkmark)
   - Auto Traverse Off Mesh Link: **enabled** (leave default)
   - Agent Type: **Humanoid** (should already be set)
5. Save prefab (Ctrl+S)
6. Repeat for all 6 prefabs

**Do NOT touch in the Inspector:**
- Priority: leave default (50) — not relevant for Phase 4
- Obstacle Avoidance Type: leave default (High Quality) — not relevant for Phase 4
- Height Mesh: do not enable — not needed

### Anti-Patterns to Avoid

- **Leaving `updatePosition = true`:** The agent will fight with `MoveTowardsSafe()`. Snake position will jitter or snap to NavMesh surface. This is the #1 failure mode for Phase 4.
- **Setting `isStopped = false` without `updatePosition = false`:** If `updatePosition = true` and a destination is accidentally set, the agent takes over movement completely, breaking all custom behavior.
- **Calling `SetDestination()` anywhere in Phase 4:** Phase 4 must NOT call SetDestination. That belongs to Phase 5.
- **Initializing in Start() before setting updatePosition = false:** If another script calls something on the NavMeshAgent between Awake() and Start(), a one-frame position snap could occur. Initialize fully in Awake().
- **Using a different Agent Type on the prefab than was baked:** This causes "Failed to create agent" errors because no matching NavMesh exists for that agent type ID.

---

## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| Agent position sync during dual-system | Manual position copy scripts | `updatePosition = false` property | Built into NavMeshAgent; exact documented solution for this scenario |
| Agent type verification | Runtime type ID checks | Match Inspector "Agent Type" to Navigation window agent type name | Type IDs are internal ints; matching by name in Inspector is reliable |
| Arrival detection | Distance checks in custom code | `stoppingDistance` on NavMeshAgent (Phase 5) | Built-in; handles path calculation edge cases |

**Key insight:** Phase 4 is purely configuration — zero new systems to build. The critical "don't hand-roll" here is the dual-system compatibility: don't write position sync scripts, use `updatePosition = false`.

---

## Common Pitfalls

### Pitfall 1: updatePosition=true Position Jitter (CRITICAL)

**What goes wrong:** Snakes jitter or snap their position every frame. Movement becomes erratic.
**Why it happens:** `MoveTowardsSafe()` sets `transform.position` each frame. With `updatePosition = true`, the NavMeshAgent simulation reads that position as the "agent position" and immediately constrains it to the nearest NavMesh surface point, then writes it back to `transform.position`. This creates a per-frame fight between the two systems.
**How to avoid:** Set `_agent.updatePosition = false` in Awake() BEFORE any movement occurs.
**Warning signs:** Snakes visually "stutter" or snap to floor/NavMesh edges during patrol. Position logs show rapid oscillation between two nearby positions.

### Pitfall 2: "Failed to create agent because it is not close enough to the NavMesh"

**What goes wrong:** Console shows this error. Agent component shows a warning icon. Agent's `isOnNavMesh` returns false.
**Why it happens:** Two root causes:
- Agent type mismatch (NavMeshAgent uses a different agent type than what was baked)
- Snake spawned/placed too far from NavMesh surface (more than ~1 unit vertical offset from floor)
**How to avoid:**
- Verify Agent Type = Humanoid on all 6 prefabs
- Confirm snakes in GameLevel.unity are positioned on the cave floor (not floating above it)
- After Phase 4, re-bake NavMesh (Phase 3 baked with snake colliders as obstacles; Phase 4 adds NavMeshAgent which excludes them from future bakes — rebake removes this inconsistency)
**Warning signs:** Console errors at Play mode start. `_agent.isOnNavMesh` returns false in debug logs.

### Pitfall 3: Modifying NavMeshAgent Inspector Fields After Awake() Sets Values

**What goes wrong:** Inspector values on the prefab are overridden by code in Awake(), or vice versa — the developer changes the prefab inspector value but the code overrides it, causing confusion.
**Why it happens:** The code sets `_agent.speed = _moveSpeed` in Awake() which overrides whatever was set in the Inspector for Speed. This is intentional but can confuse debugging.
**How to avoid:** Accept that code overrides Inspector for `speed` and `stoppingDistance`. Keep them consistent: if you set Speed = 1.5 in Inspector AND set `_agent.speed = _moveSpeed` in code, they should result in the same value (1.5). Document this clearly in the code comment.
**Warning signs:** Changing Speed in the prefab Inspector has no runtime effect (code overrides it). This is expected — document it.

### Pitfall 4: Agent Snapback When updateRotation=true

**What goes wrong:** Snake rotation jerks to face pathfinding direction even when it should be facing the player or rotating via `LookAtPlayer()`.
**Why it happens:** NavMeshAgent with `updateRotation = true` overrides `transform.rotation` to face the calculated path direction. This conflicts with `LookAtPlayer()` and the patrol rotation code.
**How to avoid:** Set `_agent.updateRotation = false` in Awake() along with `updatePosition = false`.
**Warning signs:** Snake briefly faces a wrong direction during Patrol or Follow states.

### Pitfall 5: Agent Component on Wrong Child Object

**What goes wrong:** NavMeshAgent is added to a child bone/mesh object instead of the root GameObject that has SnakeAI.cs.
**Why it happens:** Snake prefabs have deep hierarchies (rig bones, mesh children). Adding NavMeshAgent to the wrong node means `GetComponent<NavMeshAgent>()` in SnakeAI.cs finds nothing (returns null). The null check `if (_agent != null)` silently passes Phase 4 without error, but Phase 5 breaks completely.
**How to avoid:** When in Prefab Mode, always select the ROOT GameObject (the one with SnakeAI, Animator, and BoxCollider) before clicking Add Component. Verify by checking the component list: root should have SnakeAI + Animator + BoxCollider + NavMeshAgent.
**Warning signs:** `_agent` is null in runtime. Phase 4 passes silently (null check) but Phase 5 breaks when calling `_agent.SetDestination()`.

---

## Code Examples

Verified patterns from official sources:

### Complete Phase 4 Awake() Addition
```csharp
// Phase 4: Add to existing Awake() in SnakeAI.cs
// Source: Unity ScriptReference AI.NavMeshAgent.updatePosition
//         https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-updatePosition.html
// NOTE: Appended AFTER all existing Awake() code. Do not replace existing code.

_agent = GetComponent<NavMeshAgent>();
if (_agent != null)
{
    // CRITICAL: Must set updatePosition=false BEFORE any frame update runs.
    // Prevents NavMeshAgent from overriding transform.position set by MoveTowardsSafe().
    // Official pattern for dual-system coexistence (animation root motion, legacy movement, etc.)
    _agent.updatePosition = false;
    _agent.updateRotation = false;

    // Pre-configure for Phase 5 values (no effect while stopped, avoids Inspector drift)
    _agent.speed = _moveSpeed;          // 0.4f default; Phase 5 will use patrolSpeed
    _agent.stoppingDistance = 0.2f;     // Matches arrival threshold in UpdatePatrol()

    // Ensure agent is fully passive
    _agent.isStopped = true;
}
// If _agent is null: NavMeshAgent not yet added to prefab — skip silently (Phase 4 task)
```

### Inspector Settings Summary (per prefab)
```
NavMeshAgent Component Settings:
  Agent Type:       Humanoid        ← MUST match Phase 3 bake
  Radius:           0.3             ← Matches SphereCast + Phase 3 bake
  Height:           0.5             ← Matches snake size + Phase 3 bake
  Base Offset:      0               ← Snake pivot at floor level
  Speed:            1.5             ← Will be overridden by Awake() code anyway
  Stopping Distance: 0.2            ← Matches arrival threshold
  Auto Braking:     enabled         ← Standard
  Angular Speed:    leave default   ← Not used in Phase 4
  Acceleration:     leave default   ← Not used in Phase 4
```

### Phase 4 Verification Test in Play Mode
```csharp
// Add this temporary debug to Update() to verify Phase 4 works correctly
// Remove before Phase 5
#if UNITY_EDITOR
private void DebugPhase4()
{
    if (_agent == null) { Debug.LogWarning($"{_snakeName}: _agent is NULL"); return; }
    // These should ALL be true in Phase 4:
    Debug.Log($"{_snakeName}: " +
              $"isOnNavMesh={_agent.isOnNavMesh}, " +
              $"isStopped={_agent.isStopped}, " +
              $"updatePosition={_agent.updatePosition}, " +
              $"updateRotation={_agent.updateRotation}");
}
#endif
```

---

## State of the Art

| Old Approach | Current Approach | When Changed | Impact for This Project |
|--------------|------------------|--------------|------------------------|
| NavMeshAgent fully controls position | `updatePosition = false` for hybrid systems | Unity 5.x+ | Allows NavMeshAgent to coexist with any custom movement system (animation root motion, physics, legacy scripts) |
| `NavMeshAgent.Stop()` (removed) | `NavMeshAgent.isStopped = true` | Unity 5.6 | `Stop()` was deprecated; isStopped is the current API |
| Single-component movement | Separated pathfinding (NavMeshAgent) from movement execution | Standard pattern | Allows transitional phases like Phase 4 |

**Deprecated/outdated:**
- `NavMeshAgent.Stop()`: Removed. Use `isStopped = true`.
- `NavMeshAgent.Resume()`: Removed. Use `isStopped = false`.
- Manually adding NavMesh data to the scene via legacy Window > AI > Navigation > Bake: Superseded by NavMeshSurface component (Phase 3 already used the correct approach).

---

## Open Questions

1. **SnakeAI.cs Version Discrepancy**
   - What we know: Phase context says v1.7.2 but the file on disk shows v1.6.0 in the header
   - What's unclear: There may be unreflected commits; actual patrol speed field name needs verification at implementation time
   - Recommendation: At implementation time, read the current SnakeAI.cs and verify the field name for patrol speed before setting `_agent.speed = <fieldName>`

2. **patrolSpeed vs _moveSpeed Field Name**
   - What we know: Phase context says `_agent.speed = patrolSpeed` but SnakeAI.cs has `_moveSpeed` (not `patrolSpeed`). Patrol speed is `_moveSpeed * 0.75f` computed inline.
   - What's unclear: The phase description uses `patrolSpeed` as a variable name that doesn't exist in the current code
   - Recommendation: Set `_agent.speed = _moveSpeed` (the actual serialized field). The 0.75f patrol multiplier is applied at movement time, not at agent speed config time. Document this discrepancy.

3. **Rebake After Phase 4**
   - What we know: Phase 3 baked with snake GameObjects as regular geometry (no NavMeshAgent). After Phase 4 adds NavMeshAgent, snakes are excluded from future bakes.
   - What's unclear: Whether Phase 4 requires a rebake or can defer to Phase 5
   - Recommendation: Include a rebake step at the end of Phase 4 (or beginning of Phase 5). The Phase 3 RESEARCH.md noted this: "After adding NavMeshAgent in Phase 4, rebake. The Phase 5 plan should include a final rebake step." A Phase 4 rebake is preferable because it removes snakes from obstacle geometry before Phase 5 tests pathfinding.

---

## Sources

### Primary (HIGH confidence)
- Unity ScriptReference — `AI.NavMeshAgent.updatePosition`: https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-updatePosition.html — Verified: `false` decouples transform from simulation; "can be used to enable explicit control of the transform position via script"
- Unity ScriptReference — `AI.NavMeshAgent.isStopped`: https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-isStopped.html — Verified: controls path-following only, not the broader position sync
- Unity ScriptReference — `AI.NavMeshAgent.nextPosition`: https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-nextPosition.html — Verified: when `updatePosition=true`, transform reflects simulation (confirming the conflict)
- Phase 3 SUMMARY.md — confirmed Humanoid agent type, Height 0.5, Radius 0.3 bake settings
- SnakeAI.cs v1.6.0 — verified: uses `transform.position` via `MoveTowardsSafe()`, BoxCollider only (no CharacterController, no Rigidbody), existing `Awake()` method present

### Secondary (MEDIUM confidence)
- Unity Discussions — `updatePosition=false` for dual system: https://discussions.unity.com/t/navmeshagent-doesnt-update-to-transform-position/564512 — Multiple sources confirm pattern; official docs are the primary confirmation
- Unity Issue Tracker — Custom agent type "Failed to create agent": https://issuetracker.unity3d.com/issues/custom-navmesh-agent-type-causes-an-error-failed-to-create-agent-in-the-console — Confirms agent type mismatch error
- Unity Discussions — "Failed to create agent because it is not close enough to NavMesh": https://community.gamedev.tv/t/failed-to-create-agent-because-it-is-not-close-enough-to-navmesh/159082 — Deactivating/reactivating component or using Start() instead of Awake() as workarounds described; multiple sources agree

### Tertiary (LOW confidence)
- None — all critical findings verified against official Unity ScriptReference.

---

## Metadata

**Confidence breakdown:**
- Standard stack: HIGH — verified from Unity ScriptReference
- Architecture (updatePosition pattern): HIGH — directly from official Unity ScriptReference documentation; this is the documented pattern for dual-system agents
- NavMeshAgent settings: HIGH — values cross-referenced with Phase 3 SUMMARY.md bake settings and SnakeAI.cs SphereCast radius
- Pitfalls: HIGH — updatePosition conflict is documented behavior; "Failed to create agent" confirmed on Unity Issue Tracker
- Code field names: MEDIUM — SnakeAI.cs on disk is v1.6.0, phase context says v1.7.2; field names verified against actual file but version uncertainty remains

**Research date:** 2026-02-17
**Valid until:** 2026-03-17 (stable — NavMeshAgent API is stable in Unity 2022 LTS)

---

## Project-Specific Notes

### 6 Prefabs to Update
All located in `Assets/_Project/Prefabs/Snakes/Prefabs/`:
- `Toon Cobra - Green.prefab`
- `Toon Cobra - Magenta.prefab`
- `Toon Cobra - Purple.prefab`
- `Toon Snake - Green.prefab`
- `Toon Snake - Magenta.prefab`
- `Toon Snake - Purple.prefab`

FX prefabs (3 files also in same folder) do NOT get NavMeshAgent.

### SnakeAI.cs Already Has Awake()

The existing Awake() in SnakeAI.cs performs:
1. Component caching (`_collider`, `_renderer`, `_animator`)
2. Original position/rotation storage
3. MoveAwayTarget detach

Phase 4 adds NavMeshAgent initialization at the END of Awake(), after all existing code. Do not restructure existing Awake() logic.

### patrolSpeed Field Discrepancy

The Phase 4 description references a `patrolSpeed` variable that does not exist in SnakeAI.cs v1.6.0. The actual field is `_moveSpeed` (SerializeField, default 0.4f). Patrol uses `_moveSpeed * 0.75f`. Set `_agent.speed = _moveSpeed` in Awake() — Phase 5 can refine this when movement code is integrated.

### Rebake Timing

Phase 3 baked NavMesh with snake colliders included (no NavMeshAgent present). Phase 4 adds NavMeshAgent — this automatically excludes snakes from future bakes. Recommendation: After all 6 prefabs have NavMeshAgent and the code change is confirmed working, do one NavMesh rebake in GameLevel scene to produce the clean "snakes excluded from obstacle geometry" NavMesh that Phase 5 will use. This is a 2-click operation (NavMeshSurface GameObject → Clear → Bake).
