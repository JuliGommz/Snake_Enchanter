# Stack Research - NavMesh Migration

**Domain:** Unity AI Navigation (NavMesh pathfinding)
**Researched:** 2026-02-16
**Confidence:** HIGH

## Current Installation Status

### Already Installed (COMPLETE)

| Package | Version | Status | Dependencies |
|---------|---------|--------|--------------|
| com.unity.ai.navigation | 2.0.9 | INSTALLED | com.unity.modules.ai@1.0.0 |
| com.unity.modules.ai | 1.0.0 | INSTALLED | Built-in module |

**Installation Required:** NONE - All packages already present in manifest.json

## Required Components (Add to GameObjects)

### NavMeshAgent (6 Snake Prefabs)

| Component | Purpose | Required On | Configuration Priority |
|-----------|---------|-------------|------------------------|
| NavMeshAgent | AI pathfinding & movement | All 6 snake prefabs | HIGH |

**Key Properties to Configure:**
- **Agent Type:** Humanoid (default) or create custom "Snake" type
- **Speed:** 2.0-3.5 units/sec (match current SnakeAI chase speed)
- **Angular Speed:** 120-180 degrees/sec (smooth turning)
- **Acceleration:** 8-10 units/sec² (responsive movement)
- **Stopping Distance:** 0.5-1.0 units (bite range threshold)
- **Auto Braking:** TRUE (stops at destination)
- **Radius:** 0.3-0.5 units (snake body width)
- **Height:** 0.5-1.0 units (vertical clearance)
- **Obstacle Avoidance Quality:** Medium (balance performance/accuracy)
- **Priority:** 50 (default, all snakes equal priority)

### NavMeshSurface (GameLevel Scene)

| Component | Purpose | Required On | Configuration Priority |
|-----------|---------|-------------|------------------------|
| NavMeshSurface | Defines walkable area | Empty GameObject in scene root | HIGH |

**Key Properties:**
- **Agent Type:** Humanoid (or custom "Snake")
- **Default Area:** Walkable
- **Generate Links:** FALSE (manual OffMeshLink placement)
- **Use Geometry:** Render Meshes (faster baking)
- **Collect Objects:** All Game Objects (scan entire scene)

### NavMeshObstacle (Optional - Player)

| Component | Purpose | Required On | Configuration Priority |
|-----------|---------|-------------|------------------------|
| NavMeshObstacle | Dynamic avoidance | Player GameObject (optional) | LOW |

**Configuration:**
- **Shape:** Capsule (matches CharacterController)
- **Carve:** FALSE (player moves quickly, carving causes overhead)
- **Move Threshold:** 0.1 (default, triggers recalculation)

## Unity Editor Workflow

### Baking Workflow (GameLevel Scene)

**Step 1: Create NavMeshSurface**
1. GameObject > Create Empty (name: "NavMesh")
2. Add Component > Navigation > NavMesh Surface
3. Configure properties (see table above)

**Step 2: Mark Static Geometry**
1. Select cave floor/walls in hierarchy
2. Inspector > Static dropdown > Navigation Static (checked)
3. Repeat for all walkable/obstacle geometry

**Step 3: Bake NavMesh**
1. Select NavMeshSurface GameObject
2. Inspector > NavMesh Surface component
3. Click "Bake" button
4. Wait for baking (5-30 seconds depending on scene complexity)
5. Blue overlay appears in Scene view (walkable areas)

**Step 4: Verify Baked NavMesh**
1. Window > AI > Navigation (legacy window for visualization)
2. Scene view shows blue overlay (walkable)
3. Check for gaps/holes in walkable areas
4. Rebake if issues found

### Testing Tools

**Scene View Visualization:**
- **Blue overlay:** Walkable NavMesh areas
- **Pink overlay:** Height mesh (if Advanced > Build Height Mesh enabled)
- **Toggle:** Gizmos button in Scene view > NavMesh

**Runtime Debugging:**
- **NavMeshAgent.pathPending:** Path calculation in progress
- **NavMeshAgent.hasPath:** Valid path exists to destination
- **NavMeshAgent.remainingDistance:** Distance to destination
- **NavMeshAgent.velocity:** Current movement speed (read-only)

**Console Commands (Debug):**
```csharp
// Verify NavMesh exists at position
NavMesh.SamplePosition(snakePosition, out NavMeshHit hit, 5.0f, NavMesh.AllAreas);
Debug.Log($"On NavMesh: {hit.hit}");

// Check path validity
NavMeshPath path = new NavMeshPath();
agent.CalculatePath(targetPosition, path);
Debug.Log($"Path status: {path.status}"); // Complete, Partial, Invalid
```

## Migration Strategy (SnakeAI.cs)

### What Stays (Keep Existing)

| Existing Code | Status | Reason |
|---------------|--------|--------|
| State machine (Idle/Aggressive/Dazed/etc) | KEEP | NavMesh does NOT handle state logic |
| Line-of-sight detection (raycast) | KEEP | NavMesh does NOT do visibility checks |
| Attack system (Bite/Breath/Projectile) | KEEP | NavMesh does NOT handle combat |
| Spell responses (Move/Daze/Attack/Freeze) | KEEP | Game-specific logic |
| Visual feedback (Material Emission) | KEEP | NavMesh does NOT handle visuals |
| Collision detection (MoveTowardsSafe) | REMOVE | NavMesh replaces manual movement |

### What Changes (NavMesh Integration)

| Current Implementation | NavMesh Replacement |
|------------------------|---------------------|
| `transform.position += direction * speed * Time.deltaTime` | `agent.SetDestination(targetPosition)` |
| `MoveTowardsSafe()` manual raycast | NavMeshAgent automatic avoidance |
| Random waypoint patrol (Vector3 calculation) | `agent.SetDestination(RandomNavMeshPoint())` |
| Chase player (manual movement) | `agent.SetDestination(playerPosition)` |
| Collision detection (SphereCast) | NavMeshAgent.obstacleAvoidanceType |

### New Helper Methods Needed

```csharp
// Generate random patrol point on NavMesh
Vector3 RandomNavMeshPoint(Vector3 center, float range) {
    Vector3 randomDirection = Random.insideUnitSphere * range;
    randomDirection += center;
    NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, range, NavMesh.AllAreas);
    return hit.position;
}

// Stop NavMeshAgent movement (for Dazed/Frozen states)
void StopMovement() {
    if (agent.enabled) {
        agent.isStopped = true;
        agent.ResetPath();
    }
}

// Resume NavMeshAgent movement
void ResumeMovement() {
    if (agent.enabled) {
        agent.isStopped = false;
    }
}
```

## Component Requirements Summary

### GameObjects Needing Components

| GameObject | Component | Required | Priority |
|------------|-----------|----------|----------|
| Toon Cobra - Green.prefab | NavMeshAgent | YES | HIGH |
| Toon Cobra - Magenta.prefab | NavMeshAgent | YES | HIGH |
| Toon Cobra - Purple.prefab | NavMeshAgent | YES | HIGH |
| Toon Snake - Green.prefab | NavMeshAgent | YES | HIGH |
| Toon Snake - Magenta.prefab | NavMeshAgent | YES | HIGH |
| Toon Snake - Purple.prefab | NavMeshAgent | YES | HIGH |
| GameLevel scene (new GameObject) | NavMeshSurface | YES | HIGH |
| Cave floors/walls (existing) | Navigation Static flag | YES | HIGH |
| Player GameObject | NavMeshObstacle | OPTIONAL | LOW |

### Scene Requirements

| Requirement | Status | Action Needed |
|-------------|--------|---------------|
| NavMesh baked | NOT DONE | Bake via NavMeshSurface component |
| Static geometry marked | NOT DONE | Check Navigation Static on floors/walls |
| NavMesh visualization | AVAILABLE | Window > AI > Navigation (Scene view toggle) |

## What's Missing (NONE)

**Package Installation:** Complete - com.unity.ai.navigation@2.0.9 already installed

**Additional Packages Needed:** NONE

**Unity Modules Required:** com.unity.modules.ai@1.0.0 (already installed as dependency)

## Alternatives Considered

| Recommended | Alternative | When to Use Alternative |
|-------------|-------------|-------------------------|
| NavMeshAgent | Custom A* pathfinding | NEVER (academic project, limited time) |
| NavMeshSurface | Legacy Navigation window | NEVER (deprecated in Unity 2022+) |
| Unity AI Navigation 2.0.9 | A* Pathfinding Project (asset store) | NEVER (adds external dependency, overkill for simple AI) |

## What NOT to Use

| Avoid | Why | Use Instead |
|-------|-----|-------------|
| Legacy Navigation window (Window > AI > Navigation) | Deprecated baking workflow, replaced by NavMeshSurface component | NavMeshSurface component on GameObject |
| Manual pathfinding (current implementation) | Unreliable collision, no obstacle avoidance, high maintenance | NavMeshAgent component |
| NavMesh.CalculatePath() in Update() | Performance overhead, NavMeshAgent handles this automatically | NavMeshAgent.SetDestination() |
| Physics.Raycast for pathfinding | Only checks single ray, misses complex obstacles | NavMeshAgent automatic avoidance |

## Version Compatibility

| Package | Compatible With | Notes |
|---------|-----------------|-------|
| com.unity.ai.navigation@2.0.9 | Unity 2022.3 LTS | Stable release, production-ready |
| com.unity.ai.navigation@2.0.9 | URP 17.0.4 | No conflicts, NavMesh is render-pipeline agnostic |
| NavMeshAgent | CharacterController | Can coexist - disable one during NavMesh migration |

## Performance Considerations

### Baking Performance
- **Scene Size:** GameLevel scene (estimated 50-100 static objects)
- **Bake Time:** 5-30 seconds (acceptable for academic project)
- **Runtime Cost:** Zero (baked data is static)

### NavMeshAgent Performance
- **6 Active Agents:** Low overhead (<1ms/frame on modern hardware)
- **Obstacle Avoidance:** Medium quality recommended (balance CPU/accuracy)
- **Path Recalculation:** Automatic, only when destination changes

### Optimization Tips
- **Avoid Runtime Baking:** Use edit-time baking (NavMeshSurface.Bake() in Editor only)
- **Disable Agents in Dazed/Frozen:** `agent.enabled = false` (saves CPU)
- **Reduce Avoidance Quality:** Use "Low" if 60 FPS not maintained

## Testing Checklist

### Pre-Migration Testing
- [ ] Record current SnakeAI behavior (video/screenshots)
- [ ] Document patrol radius (2-3 units)
- [ ] Document chase speed (current movement speed)
- [ ] Document attack ranges (Bite: 0-0.5, Breath: 4-7, Projectile: 8+)

### Post-Migration Testing
- [ ] NavMesh baked successfully (blue overlay in Scene view)
- [ ] Snakes patrol random points (no stuck snakes)
- [ ] Snakes chase player (smooth pathfinding)
- [ ] Snakes stop at attack ranges (stopping distance working)
- [ ] Snakes avoid obstacles (walls, props)
- [ ] Snakes avoid each other (no overlap)
- [ ] Dazed snakes stop movement (agent.isStopped = true)
- [ ] Frozen snakes stop movement (agent.enabled = false)
- [ ] Attack animations play correctly (no movement interruption)

## Sources

**HIGH CONFIDENCE (Official Unity Documentation):**
- [Unity AI Navigation Package Manual](https://docs.unity3d.com/Manual/com.unity.ai.navigation.html) - Package overview
- [NavMesh Surface Component Reference](https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/NavMeshSurface.html) - Baking workflow, properties
- [NavMesh Agent Component Reference](https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/NavMeshAgent.html) - Agent properties, steering, avoidance
- [NavMesh Obstacle Component Reference](https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/NavMeshObstacle.html) - Obstacle configuration

**MEDIUM CONFIDENCE (Unity Community Guides):**
- [Unity 2022 LTS AI Navigation Guide](https://discussions.unity.com/t/a-guide-on-using-the-new-ai-navigation-package-in-unity-2022-lts-and-above/371872) - Setup workflow (403 error, could not verify)
- [Unity Learn: NavMesh Baking Tutorial](https://learn.unity.com/tutorial/navmesh-baking) - Step-by-step baking
- [Unity Learn: Working with NavMesh Agents](https://learn.unity.com/tutorial/working-with-navmesh-agents) - Agent configuration

**PROJECT CONTEXT (Existing Codebase):**
- Packages/manifest.json - com.unity.ai.navigation@2.0.9 verified installed
- Packages/packages-lock.json - Dependency on com.unity.modules.ai@1.0.0 verified
- SnakeAI.cs v1.7.0 - Current manual movement implementation (to be replaced)

---
*Stack research for: Unity AI Navigation (NavMesh) Migration*
*Researched: 2026-02-16*
*Project: Snake Enchanter (Unity 2022 LTS, URP 17.0.4)*
