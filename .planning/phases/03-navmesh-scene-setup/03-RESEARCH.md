# Phase 3: NavMesh Scene Setup - Research

**Researched:** 2026-02-17
**Domain:** Unity AI Navigation (com.unity.ai.navigation v2.0.9) — Editor-only NavMesh baking
**Confidence:** HIGH

---

## Summary

Phase 3 is a pure editor operation: bake a NavMesh in the GameLevel scene using the NavMeshSurface component from the already-installed `com.unity.ai.navigation v2.0.9` package. Zero code changes. The bake simply adds a NavMesh data asset that snake prefabs can use in Phase 4 when NavMeshAgent components are added.

The key decision point is: use the modern `NavMeshSurface` component approach (not the legacy Window > AI > Navigation bake tab). With the AI Navigation package installed, the legacy tab no longer shows the Bake button — only NavMeshSurface works correctly. NavMeshSurface with "Collect Objects: All Game Objects" will pick up all active renderers without requiring the legacy "Navigation Static" flag. The modern replacement for Navigation Static is the `NavMeshModifier` component.

For a cave/indoor scene, the single most important bake setting is **Agent Height**. If Agent Height is larger than the vertical clearance inside the cave, the NavMesh baker will see the floor as "not reachable under ceiling" and will skip those areas. Start with Agent Height = 0.5 (snakes are low to the ground) and Agent Radius = 0.3. Verify by opening the Navigation window in the editor — a blue overlay on floor surfaces confirms a successful bake.

**Primary recommendation:** Add one NavMeshSurface GameObject to GameLevel scene, set Agent Height = 0.5, Agent Radius = 0.3, Collect Objects = All Game Objects, Use Geometry = Render Meshes, then click Bake. Verify blue floor overlay in Scene view with Navigation window open.

---

## Standard Stack

### Core
| Component | Version | Purpose | Why This |
|-----------|---------|---------|----------|
| `com.unity.ai.navigation` | 2.0.9 (already installed) | NavMeshSurface baking, NavMeshModifier | Modern replacement for legacy built-in nav system; required for NavMeshSurface component |
| `com.unity.modules.ai` | 1.0.0 (built-in module) | Low-level NavMesh runtime (NavMeshAgent, etc.) | Already present; provides the NavMesh data store |

### Supporting Components
| Component | Purpose | When to Use |
|-----------|---------|-------------|
| `NavMeshSurface` | Defines geometry to bake and stores the NavMesh asset | One per scene (or per agent type if different agents need different meshes) |
| `NavMeshModifier` | Per-object override — include/exclude objects from baking, change area type | Use on props/walls to mark as "Not Walkable", or to exclude dynamic objects |
| `NavMeshModifierVolume` | Box-shaped volume override of area type | Use to mark an entire room zone as a different area type |

### Alternatives Considered
| Standard Choice | Alternative | Why We Don't Use It |
|-----------------|-------------|---------------------|
| NavMeshSurface | Legacy Window > AI > Navigation > Bake tab | Legacy bake tab has no "Bake" button when AI Navigation package is installed; Navigation Static flag is deprecated |
| Render Meshes (geometry source) | Physics Colliders | Render Meshes produce more accurate floor shapes for visual assets; Physics Colliders can work but may differ from rendered geometry |

**Package already installed — no installation step required.**

---

## Architecture Patterns

### Pattern 1: Single NavMeshSurface for the Entire Scene

**What:** One empty GameObject named "NavMeshSurface" with a `NavMeshSurface` component. Placed at scene root or inside an "AI" or "Navigation" empty group object. Collects ALL active renderers.

**When to use:** This is the correct approach for a single-agent-type game with one playable area. Snake Enchanter has one agent type (snake, Humanoid/generic size) and one cave level.

**Workflow:**
1. In GameLevel scene hierarchy: Right-click → Create Empty → rename to "NavMeshSurface"
2. In Inspector → Add Component → Navigation → NavMesh Surface
3. Set Agent Type = "Humanoid" (or the default agent type — verify in Window > AI > Navigation > Agents)
4. Set Collect Objects = All Game Objects
5. Set Use Geometry = Render Meshes
6. Expand Advanced: check that Override Voxel Size is OFF (auto = Agent Radius / 3)
7. Click **Bake**
8. Open Window > AI > Navigation → blue overlay appears on walkable floors

**Result:** NavMesh data asset stored at `Assets/_Project/Scenes/GameLevel/NavMesh.asset` (inside a folder named after the scene).

### Pattern 2: NavMeshModifier for Obstacle/Prop Exclusion

**What:** Add `NavMeshModifier` component to props, decorations, or walls that should not be walkable on top.

**When to use:**
- Barrels, crates, or props that are walkable on their top surface (NavMesh incorrectly includes them as walkable)
- Ceiling geometry that bakes as walkable because NavMesh can't distinguish up vs down
- Any object that should be a hard obstacle, not a traversable surface

**Settings:**
- Mode = **Remove Object** (excludes this object entirely from baking)
- OR: Mode = Add or Modify Object, Area Type = Not Walkable (marks surface as blocked)
- Apply to Children = ON (affects entire hierarchy of a prop group)

### Pattern 3: Layer-Based Filtering (Optional Enhancement)

**What:** Assign cave floor geometry to one layer (e.g., "Ground") and use `Include Layers` on NavMeshSurface.

**When to use:** When the scene has many dynamic objects (characters, items) that should never contribute to baking. For Phase 3 this is optional — "All Game Objects" works if there are no NavMeshAgent or NavMeshObstacle components yet (baking automatically excludes them).

**Note:** NavMeshAgent and NavMeshObstacle components are automatically excluded from baking regardless of layer. This means existing snake prefabs (no NavMeshAgent yet in Phase 3) WILL be included as static geometry during bake. This is acceptable for Phase 3 because snakes are placed in the scene and their collider shapes contribute to obstacle baking correctly.

### Anti-Patterns to Avoid

- **Using legacy Navigation window Bake tab:** When `com.unity.ai.navigation` is installed, the legacy Navigation window no longer shows the Bake button. Trying to bake there will produce no result. Always use the NavMeshSurface component's Bake button.
- **Setting Agent Height too large:** For snake-height agents (low to ground), a default humanoid Agent Height of 2.0 units can cause cave ceilings to block floor baking. Keep Agent Height ≤ 0.5 for snake agents.
- **Marking "Navigation Static" in Inspector:** This is the OLD workflow. With NavMeshSurface + "Collect Objects: All Game Objects", Navigation Static is irrelevant. Using it does NOT cause harm but creates confusion about why objects include/exclude from baking.
- **Removing the NavMeshSurface component to "delete" the NavMesh:** This leaves an orphaned .asset file. Use the **Clear** button in the inspector first, then remove the component.

---

## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| NavMesh obstacle marking | Custom collider-only scripts | NavMeshModifier with "Remove Object" or "Not Walkable" area | Built into the baking pipeline; hierarchical; updates on rebake |
| Floor coverage verification | Runtime debug drawing scripts | Built-in Scene view blue overlay (Navigation window open) | Zero cost, available immediately, toggled by closing/opening nav window |
| Agent type configuration | Custom agent size constants in code | Window > AI > Navigation > Agents tab | Centralised; same settings used by both baking and NavMeshAgent components |

**Key insight:** Phase 3 requires ZERO code. Everything is editor UI. Any attempt to write code for Phase 3 is scope creep.

---

## Common Pitfalls

### Pitfall 1: Blue Overlay Not Appearing After Bake

**What goes wrong:** Click Bake, no console errors, but no blue floor visible.
**Why it happens:** The Navigation window must be open AND visible for the overlay to render. It is NOT always shown — it only renders while the window tab is active.
**How to avoid:** After baking, ensure Window > AI > Navigation is open and the Scene view is visible simultaneously.
**Warning signs:** NavMesh.asset file exists in Project window but no blue in scene.

### Pitfall 2: Agent Height Blocks Indoor Floor Baking

**What goes wrong:** Cave rooms appear baked correctly from outside, but floors inside rooms show no NavMesh (no blue).
**Why it happens:** The baking system checks vertical clearance. If Agent Height = 2.0 and the cave ceiling is 2.5 units high, clearance = 0.5 — possibly below the effective threshold for the voxel resolution.
**How to avoid:** Set Agent Height to match the actual snake height (0.4–0.6 units). Snakes move low to the ground; a humanoid-default height of 2.0 is wrong.
**Warning signs:** Open areas bake fine but enclosed cave sections have no blue overlay.

### Pitfall 3: Ceiling Geometry Baked as Walkable Surface

**What goes wrong:** The top face of cave ceiling mesh appears in the blue overlay as a "walkable" area far above the floor.
**Why it happens:** NavMesh baking looks at upward-facing polygons with correct slope and marks them walkable, regardless of reachability.
**How to avoid:** Add a NavMeshModifier (Mode = Remove Object) to the ceiling mesh objects. OR accept it — ceiling walkable surface is unreachable and won't affect agent pathfinding in practice since no agent starts there.
**Warning signs:** Blue overlay visible both on floor AND on top of the cave ceiling mesh.

### Pitfall 4: FBX Mesh Compression Prevents Baking

**What goes wrong:** A specific imported FBX model's floor section has no NavMesh baked on it.
**Why it happens:** Unity Issue Tracker confirmed bug — FBX files with Mesh Compression set to **Low** or **Medium** are ignored during NavMesh baking. Only **Off** and **High** are baked correctly.
**How to avoid:** Select floor/cave FBX in Project window → Inspector → Model tab → Mesh Compression → set to **Off** or **High**. Apply.
**Warning signs:** Some floor sections baked, others not, with no obvious geometry reason. Specifically affects the Caves Parts Set and Dwarven Pack imported meshes.

### Pitfall 5: Dynamic Props Permanently Baked as Obstacles

**What goes wrong:** Props placed in the scene are baked into NavMesh as permanent obstacles. If they later become dynamic (picked up, destroyed), the NavMesh will be wrong at runtime.
**Why it happens:** NavMeshSurface "All Game Objects" includes everything renderable. Phase 3 only bakes once (editor-time); NavMesh won't update at runtime unless rebaked.
**How to avoid:** For Phase 3 this is acceptable. Snakes don't pathfind yet. If props are static cave scenery they SHOULD be obstacles. Flag this for Phase 5 if any props need to be dynamic obstacles.
**Warning signs:** N/A for Phase 3 — static cave geometry being permanent is correct behavior.

### Pitfall 6: Snake Prefabs in Scene Contribute to Bake Geometry

**What goes wrong:** Snake colliders/renderers get baked into the NavMesh surface as obstacles. After Phase 4 adds NavMeshAgent, snakes are excluded from future bakes — causing inconsistency.
**Why it happens:** In Phase 3, snake prefab instances have NO NavMeshAgent component yet. NavMeshSurface includes them as geometry.
**How to avoid:** This is a Phase 3 limitation to accept. After adding NavMeshAgent in Phase 4, rebake. The Phase 5 plan should include a final rebake step after NavMeshAgent components are confirmed.
**Warning signs:** After adding NavMeshAgent in Phase 4 and rebaking, NavMesh coverage changes slightly around snake starting positions — this is expected and correct.

---

## Code Examples

Phase 3 is ZERO code. All steps are editor UI. No code examples apply.

The closest thing to "code" is the NavMeshSurface component inspector settings. For reference, the settings serialized in a scene file look like:

```yaml
# Serialized NavMeshSurface component settings (for reference only — edit in Inspector)
# Source: com.unity.ai.navigation 2.0.9 official docs
agentTypeID: 0           # 0 = Humanoid agent type
collectObjects: 0        # 0 = All Game Objects
useGeometry: 0           # 0 = Render Meshes, 1 = Physics Colliders
layerMask: -1            # Everything
overrideTileSize: 0      # OFF
overrideVoxelSize: 0     # OFF (auto = Agent Radius / 3)
buildHeightMesh: 0       # OFF for Phase 3 (enable in Phase 5 if needed)
```

---

## State of the Art

| Old Approach | Current Approach | When Changed | Impact for This Project |
|--------------|------------------|--------------|------------------------|
| Legacy Window > AI > Navigation > Bake tab | NavMeshSurface component + Bake button in component inspector | Unity 2022 / AI Navigation package | Bake tab has NO Bake button when AI Navigation package is installed. Must use NavMeshSurface component. |
| "Navigation Static" flag on GameObjects | NavMeshModifier component | AI Navigation package | Navigation Static is deprecated. With NavMeshSurface "All Game Objects", all active objects are collected regardless of static flag. |
| Global scene-wide NavMesh | Per-surface NavMesh assets | AI Navigation package | NavMesh data stored as .asset file per NavMeshSurface instance, not as a single global scene asset. |

**Deprecated/outdated in Unity 2022 + AI Navigation package:**
- "Navigation Static" checkbox in Inspector > Static flags: Deprecated. Still works as a filter in legacy workflow but ignored by NavMeshSurface.
- Legacy Window > AI > Navigation > Bake tab: The Bake button is removed when com.unity.ai.navigation is installed. The window still shows agent configuration but baking happens through NavMeshSurface component.

---

## Open Questions

1. **Cave Ceiling Clearance — Actual Dimensions Unknown**
   - What we know: Agent Height must be less than vertical clearance in cave sections; snakes are low to ground
   - What's unclear: Exact ceiling height of Caves Parts Set + Dwarven Pack assets in this scene
   - Recommendation: Start with Agent Height = 0.5. If some rooms fail to bake, reduce to 0.3. Check Scene view blue overlay coverage.

2. **FBX Mesh Compression Settings for Caves Parts Set + Dwarven Pack**
   - What we know: Unity bug — Low/Medium compression ignores mesh during bake
   - What's unclear: What compression is currently set for these specific assets
   - Recommendation: As part of bake verification, check compression settings for all cave floor FBX files if any floor areas fail to bake.

3. **Agent Type — Should Snakes Use a Custom Agent Type?**
   - What we know: Phase 4 will add NavMeshAgent to snake prefabs; snake body is low and compact
   - What's unclear: Whether the default Humanoid agent type (height ~2.0, radius ~0.5) will be used, or a custom "Snake" agent type should be defined
   - Recommendation: Phase 3 can use default Humanoid agent type. The actual agent type used in Phase 4 must match what was baked in Phase 3. If Phase 4 needs a custom "Snake" agent type, Phase 3 will need to be rebaked. Flag this for Phase 4 planning.

---

## Sources

### Primary (HIGH confidence)
- `com.unity.ai.navigation@2.0` official docs — NavMeshSurface component reference: https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/NavMeshSurface.html
- `com.unity.ai.navigation@2.0` official docs — Create a NavMesh: https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/CreateNavMesh.html
- `com.unity.ai.navigation@2.0` official docs — NavMeshModifier component: https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/NavMeshModifier.html
- `com.unity.ai.navigation@2.0` official docs — Navigation Window reference: https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/NavigationWindow.html
- Project `Packages/manifest.json` — confirmed `com.unity.ai.navigation: 2.0.9` installed

### Secondary (MEDIUM confidence)
- Unity Issue Tracker — FBX Mesh Compression baking bug: https://issuetracker.unity3d.com/issues/navmesh-baking-ignores-fbx-files-with-mesh-compression-set-to-low-or-medium-when-baking-navmesh (verified from official Unity tracker)
- Unity Discussions — NavMeshSurface includes non-Navigation Static objects (confirmed Navigation Static is deprecated for NavMeshSurface workflow): https://discussions.unity.com/t/nav-mesh-surface-includes-non-static-items/862885
- Unity Discussions — New Navigation package guide for Unity 2022 LTS: https://discussions.unity.com/t/a-guide-on-using-the-new-ai-navigation-package-in-unity-2022-lts-and-above/371872
- Unity Discussions — Baking with new navigation (NavMeshSurface replaces legacy bake tab): https://discussions.unity.com/t/baking-with-the-new-navigation/918952

### Tertiary (LOW confidence)
- Multiple Medium/community articles (2024) confirm NavMeshSurface + cave indoor ceiling height pitfall — consistent with official doc behavior description, not independently verified from official source.

---

## Metadata

**Confidence breakdown:**
- Standard stack: HIGH — confirmed from official AI Navigation 2.0.9 docs and project manifest.json
- Architecture patterns: HIGH — directly from official component reference documentation
- Pitfalls: MEDIUM-HIGH — FBX compression bug from official Unity Issue Tracker (HIGH); ceiling height issue from official docs description of Agent Height behavior (HIGH); mesh compression solution from official docs (HIGH); blue overlay behavior from official docs (HIGH)
- Code examples: N/A — Phase 3 is zero code

**Research date:** 2026-02-17
**Valid until:** 2026-03-17 (stable, 30 days) — AI Navigation 2.0.9 is a stable release for Unity 2022 LTS

---

## Project-Specific Notes

### SnakeAI.cs is ZERO-IMPACT for Phase 3

Read the full SnakeAI.cs (v1.7.0). Confirmed:
- No NavMeshAgent component used yet (comments say "Phase 2" for NavMesh)
- Movement uses manual `MoveTowardsSafe()` with SphereCast
- Patrol uses `GenerateNewPatrolWaypoint()` with random offsets from `_originalPosition`
- All 7 states (Idle, Aggressive, MovedAway, Dazed, AttackingEnemy, Frozen, Dead) are preserved
- Phase 3 baking does NOT touch SnakeAI.cs — snakes continue using old movement system

### NavMeshSurface Will Include Snake Prefabs in Phase 3 Bake

Snake instances in the scene have colliders + renderers but no NavMeshAgent. They WILL be included in the Phase 3 bake as static obstacle geometry. This is acceptable and correct for Phase 3 because:
1. Snakes are currently statically placed
2. Phase 3 only verifies that the cave geometry bakes correctly
3. After Phase 4 adds NavMeshAgent to snake prefabs, a single rebake removes them from the obstacle geometry

### The "feature/enemy-setup" Branch is Correct

Phase 3 is part of the enemy NavMesh migration. All changes (adding NavMeshSurface GameObject to scene, baking) happen on the current `feature/enemy-setup` branch.
