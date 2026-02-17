# Roadmap - v0.3 Bug Fixes & Stability

**Milestone:** v0.3
**Goal:** Fix Snake patrol animation bug + verify all Phase 2 features
**Status:** Phase 3 (Planned — ready to execute)

---

## Phase Overview

| Phase | Name | Status | Duration | Focus |
|-------|------|--------|----------|-------|
| 3 | NavMesh Scene Setup | ⏳ Planned | 1 hour | Bake NavMesh, zero code changes |
| 4 | Component Integration | ⏳ Pending | 1 hour | Add NavMeshAgent to prefabs, configure |
| 5 | Movement Migration | ⏳ Pending | 1.5 hours | Replace custom movement with NavMesh |
| 6 | Cleanup & Polish | ⏳ Pending | 30 min | Delete obsolete code, clean up |
| 7 | Testing & Verification | ⏳ Pending | 1 hour | Full feature testing, documentation |

**Total:** 5 hours estimated

---

## Phase 3: NavMesh Scene Setup

**Goal:** Bake NavMesh in GameLevel scene with zero code changes

**Plans:** 1 plan

Plans:
- [ ] 03-01-PLAN.md — Fix FBX compression, create NavMeshSurface, bake, verify

**Why this phase:**
- Zero-risk first step validates environment is NavMesh-compatible
- Discovers baking issues early (before code changes)
- Completely reversible (just delete NavMesh data)

**Tasks:**
1. Create GameObject "NavMeshSurface" in GameLevel scene
2. Add NavMeshSurface component
3. Mark cave geometry as "Navigation Static" in Inspector
4. Click "Bake" button in NavMeshSurface inspector
5. Verify blue overlay visible in Scene view
6. Test: Snakes still use old movement (no behavioral change)

**Success Criteria:**
- [ ] NavMesh visible in Scene view (blue overlay on walkable surfaces)
- [ ] No console errors during bake
- [ ] NavMesh covers all floor areas where snakes patrol
- [ ] Props/walls marked as obstacles (not walkable)
- [ ] Snakes still patrol normally (old system still active)

**Deliverables:**
- GameLevel.unity with baked NavMesh data
- NavMeshSurface GameObject configured
- Screenshot: Scene view showing NavMesh blue overlay

**Rollback Plan:**
- Delete NavMeshSurface GameObject
- Unity removes NavMesh data automatically

**Time:** 1 hour
**Risk:** LOW
**Blocking:** None

---

## Phase 4: Component Integration

**Goal:** Add NavMeshAgent to snake prefabs, configure settings, but don't connect to movement code yet

**Plans:** 2 plans

Plans:
- [ ] 04-01-PLAN.md — Add NavMeshAgent component to 6 snake prefabs (Inspector, user action)
- [ ] 04-02-PLAN.md — SnakeAI.cs Awake() initialization + NavMesh rebake

**Why this phase:**
- Component configured but not controlling movement (dual systems exist)
- Snakes can have both old and new systems running side-by-side
- Validates component configuration before removing old code

**Tasks:**
1. Add NavMeshAgent component to all 6 snake prefabs (Unity Inspector)
2. Configure agent settings:
   - Radius: 0.3f (matches SphereCast size)
   - Height: 0.5f (snake vertical size)
   - Base Offset: 0f (ground level)
   - Speed: 1.5f (patrolSpeed default)
   - Stopping Distance: 0.2f (matches arrival threshold)
   - Auto Braking: true
   - Update Position: true (Inspector default — code overrides to false in Awake)
   - Update Rotation: true (Inspector default — code overrides to false in Awake)
3. Add Awake() initialization in SnakeAI.cs:
   ```csharp
   private NavMeshAgent _agent;

   void Awake() {
       // Existing code...

       // NavMesh setup (Phase 4) - passive initialization only
       _agent = GetComponent<NavMeshAgent>();
       if (_agent != null) {
           _agent.updatePosition = false;  // CRITICAL: prevent position fight with MoveTowardsSafe()
           _agent.updateRotation = false;  // prevent rotation fight with LookAtPlayer()
           _agent.speed = _moveSpeed;
           _agent.stoppingDistance = 0.2f;
           _agent.isStopped = true;
       }
   }
   ```
4. Rebake NavMesh (snakes now excluded as geometry, not obstacles)
5. Test: Snakes still use old movement (agent present but isStopped=true)

**Success Criteria:**
- [ ] All 6 snake prefabs have NavMeshAgent component
- [ ] Agent settings visible in Inspector (speed, radius, etc.)
- [ ] Awake() initializes agent without errors
- [ ] Snakes still patrol normally (old movement still active)
- [ ] No console errors/warnings

**Deliverables:**
- 6 updated snake prefabs with NavMeshAgent
- SnakeAI.cs with Awake() agent initialization
- Commit: "feat: SnakeAI v1.8.0 - Add NavMeshAgent component (inactive)"

**Rollback Plan:**
- Remove NavMeshAgent component from prefabs
- Delete Awake() agent initialization code
- Git revert commit

**Time:** 1 hour
**Risk:** LOW (dual systems, old movement still works)
**Blocking:** Phase 3 (needs baked NavMesh)

---

## Phase 5: Movement Migration

**Goal:** Replace custom movement methods with NavMeshAgent pathfinding

**Why this phase:**
- Core migration work - replaces MoveTowardsSafe() with agent.SetDestination()
- Fixes patrol animation bug (velocity-based triggers)
- Eliminates fragile collision detection code

**Tasks:**

### 5.1: Patrol Replacement (20 min)
1. Update UpdatePatrol() method:
   ```csharp
   // OLD (DELETE)
   MoveTowardsSafe(_currentPatrolTarget, patrolSpeed);

   // NEW (REPLACE)
   _agent.SetDestination(_currentPatrolTarget);
   _agent.speed = patrolSpeed;
   ```
2. Update GenerateNewPatrolWaypoint() to validate destinations:
   ```csharp
   Vector3 randomPoint = _spawnPosition + Random.insideUnitSphere * _patrolRadius;
   NavMeshHit hit;
   if (NavMesh.SamplePosition(randomPoint, out hit, 3f, NavMesh.AllAreas)) {
       _currentPatrolTarget = hit.position;
   } else {
       _currentPatrolTarget = _spawnPosition; // Fallback to spawn
   }
   ```
3. Update arrival check:
   ```csharp
   // OLD
   if (Vector3.Distance(transform.position, _currentPatrolTarget) < 0.2f)

   // NEW
   if (_agent.remainingDistance < 0.2f && _agent.velocity.magnitude < 0.1f)
   ```
4. Test: Snakes patrol using NavMesh (smooth, no animation jump)
5. Commit: "feat: SnakeAI v1.8.1 - NavMesh patrol system"

### 5.2: Chase Replacement (10 min)
1. Update FollowPlayer() method:
   ```csharp
   // OLD (DELETE)
   MoveTowardsSafe(_playerTransform.position, _chaseSpeed);
   LookAtPlayer();

   // NEW (REPLACE)
   _agent.SetDestination(_playerTransform.position);
   _agent.speed = _chaseSpeed;
   // NavMeshAgent handles movement + rotation automatically
   ```
2. Test: Snakes chase player using NavMesh (smooth pursuit)
3. Commit: "feat: SnakeAI v1.8.2 - NavMesh chase behavior"

### 5.3: Animation Update (10 min)
1. Update UpdateMovementAnimation() method:
   ```csharp
   // OLD
   bool isMoving = (_currentState == SnakeState.Aggressive) ||
                   (_currentState == SnakeState.Idle && _isPatrolling) ||
                   (_currentState == SnakeState.MovedAway && _isMoving);

   // NEW
   bool isMoving = _agent.velocity.magnitude > 0.1f &&
                   (_currentState == SnakeState.Aggressive ||
                    _currentState == SnakeState.Idle ||
                    _currentState == SnakeState.MovedAway);
   ```
2. Update directional logic to use agent.velocity:
   ```csharp
   // OLD
   Vector3 localDirection = transform.InverseTransformDirection(_lastMoveDirection);

   // NEW
   Vector3 localDirection = transform.InverseTransformDirection(_agent.velocity.normalized);
   ```
3. Test: Directional slither animations match actual movement
4. Commit: "feat: SnakeAI v1.8.3 - NavMesh velocity-based animations"

### 5.4: State Integration (15 min)
1. Update SetState() method with agent.isStopped control:
   ```csharp
   private void SetState(SnakeState newState) {
       // Existing state transition logic...

       // NavMeshAgent control
       if (_agent != null) {
           switch (newState) {
               case SnakeState.Dazed:
               case SnakeState.Frozen:
               case SnakeState.Dead:
                   _agent.isStopped = true;
                   break;
               default:
                   _agent.isStopped = false;
                   break;
           }
       }

       _currentState = newState;
   }
   ```
2. Test: Spells control movement (Daze/Freeze stop snake, Move resumes)
3. Commit: "feat: SnakeAI v1.8.4 - NavMesh state machine integration"

**Success Criteria:**
- [ ] Snakes patrol without animation restart glitch ✅ MAIN GOAL
- [ ] Snakes chase player smoothly (no collision issues)
- [ ] Directional slither animations match movement direction
- [ ] Dazed state stops movement completely
- [ ] Frozen state stops movement completely
- [ ] MovedAway state moves to target smoothly
- [ ] No console errors during gameplay

**Deliverables:**
- SnakeAI.cs v1.8.4 with full NavMesh integration
- 4 atomic commits (patrol, chase, animation, state)

**Rollback Plan:**
- Git revert commits 1-4 in reverse order
- Restore MoveTowardsSafe() from v1.7.2

**Time:** 1.5 hours
**Risk:** MEDIUM (core gameplay changes, but incremental + testable)
**Blocking:** Phase 4 (needs NavMeshAgent component)

---

## Phase 6: Cleanup & Polish

**Goal:** Remove obsolete movement code and clean up SnakeAI.cs

**Why this phase:**
- Eliminates dead code (MoveTowardsSafe, LookAtPlayer)
- Removes unused fields (_lastMoveDirection, _isPatrolling bool)
- Improves code maintainability

**Tasks:**
1. Delete obsolete methods:
   - `MoveTowardsSafe(Vector3 target, float speed)` - ~50 lines
   - `LookAtPlayer()` - ~10 lines
2. Remove unused fields:
   - `private Vector3 _lastMoveDirection;` (if not used elsewhere)
   - Can keep `_isPatrolling` bool if used for other logic checks
3. Update code comments to reflect NavMesh usage
4. Clean up debug logs (remove temporary logging)
5. Test: No compiler errors, all features still work

**Success Criteria:**
- [ ] No unused methods in SnakeAI.cs
- [ ] No unused fields (or commented with "kept for X reason")
- [ ] No compiler warnings
- [ ] Code comments accurate (reflect NavMesh implementation)
- [ ] All Phase 5 features still work

**Deliverables:**
- SnakeAI.cs v1.8.5 (cleaned up, production-ready)
- Commit: "refactor: SnakeAI v1.8.5 - Remove obsolete movement methods"

**Rollback Plan:**
- Git revert commit (brings back deleted code)

**Time:** 30 minutes
**Risk:** LOW (only deleting dead code)
**Blocking:** Phase 5 (needs new movement working first)

---

## Phase 7: Testing & Verification

**Goal:** Verify all Phase 2 features work correctly, document v0.3 completion

**Why this phase:**
- End-to-end validation of all systems
- Ensures no regressions from NavMesh migration
- Documents milestone completion

**Tasks:**

### 7.1: Full Feature Testing (30 min)
1. **Player Testing:**
   - [ ] Spawn grounded (no float)
   - [ ] WASD movement smooth
   - [ ] Mouse look responsive
   - [ ] Crouch works (Ctrl)
   - [ ] Camera follows smoothly (Cinemachine)

2. **Tune Casting Testing:**
   - [ ] Tune 1 (Move): Hold 1, release in zone = Snake moves away
   - [ ] Tune 2 (Daze): Hold 2, release in zone = Snake collapses 8s
   - [ ] Tune 3 (Attack): Hold 3, release in zone = Snake attacks creature
   - [ ] Tune 4 (Freeze): Hold 4, slider appears (known: freeze doesn't work)
   - [ ] Failed cast: Release outside zone = Snake attacks player

3. **Snake AI Testing:**
   - [ ] Patrol: Snake moves between waypoints, no animation jump ✅ KEY TEST
   - [ ] Proximity: Snake detects player (line-of-sight)
   - [ ] Chase: Snake follows player when aggressive
   - [ ] Attack: Snake uses Bite/Breath/Projectile based on distance
   - [ ] MovedAway: Snake moves to MoveAwayTarget (Tune 1)
   - [ ] Dazed: Snake collapses, stops moving, 8s timer (Tune 2)
   - [ ] AttackingEnemy: Snake attacks RobotKyle (Tune 3)

4. **Animation Testing:**
   - [ ] Slither Forward when moving forward
   - [ ] Slither Left when moving left (FIRST TIME TESTED)
   - [ ] Slither Right when moving right (FIRST TIME TESTED)
   - [ ] Die animation when Dazed
   - [ ] Bite/Breath/Projectile attack animations

5. **System Testing:**
   - [ ] Health drains passively
   - [ ] Successful spell restores HP
   - [ ] Failed spell triggers attack damage
   - [ ] Reach exit = Win screen
   - [ ] HP reaches 0 = Lose screen
   - [ ] Mode selection works (Simple/Advanced)

6. **Performance Testing:**
   - [ ] Stable 60 FPS in GameLevel scene
   - [ ] No frame drops during spell casting
   - [ ] No console errors/warnings
   - [ ] Memory usage stable (no leaks)

### 7.2: Documentation Update (30 min)
1. Update STATE.md:
   - Current position: v0.3 complete
   - Recent progress: NavMesh migration, full testing
   - Next milestone: Phase 3 (SCHÖN - Polish)

2. Update MILESTONES.md:
   - Add v0.3 section with completion details
   - Last phase number: 7 (Testing & Verification)

3. Update Arbeitsprotokoll:
   - Session 18 entry (date: 2026-02-16)
   - Tasks: NavMesh migration, full testing
   - Screenshot: `Media/Screenshots/2026-02-16_v0.3Complete.png`

4. Take screenshot:
   - GameLevel scene in Play mode
   - Snake patrolling (no animation glitch visible)
   - Player visible, UI showing

5. Commit: "docs: v0.3 Bug Fixes & Stability complete"

**Success Criteria:**
- [ ] All 6 test categories passed (Player, Tune Casting, Snake AI, Animation, System, Performance)
- [ ] Slither Left/Right tested for first time (Phase 2 carryover)
- [ ] No blocking bugs found (minor bugs acceptable, log in STATE.md)
- [ ] Documentation updated and committed
- [ ] Screenshot captured

**Deliverables:**
- Test results documented (pass/fail per category)
- Updated documentation (STATE.md, MILESTONES.md, Arbeitsprotokoll)
- Screenshot showing v0.3 working
- Commit: "docs: v0.3 Bug Fixes & Stability complete"

**Rollback Plan:**
- Not applicable (readonly testing + documentation)

**Time:** 1 hour
**Risk:** ZERO (readonly, no code changes)
**Blocking:** Phase 6 (needs clean codebase)

---

## Completion Criteria

**v0.3 Milestone DONE when:**
- ✅ Player spawns grounded (Phase completed in earlier session)
- [ ] Snake patrol animations don't jump/restart (Phase 5.1 success criteria)
- [ ] All Phase 2 features verified working (Phase 7.1 test results)
- [ ] Slither Left/Right tested (Phase 7.1 animation testing)
- [ ] No console errors in Play mode (Phase 7.1 performance testing)
- [ ] Documentation updated (Phase 7.2 deliverables)
- [ ] feature/enemy-setup ready for merge (all tests passing)

**Ready to merge when:**
- User confirms: "v0.3 tests look good, approved for merge"
- Follow `.planning/MERGE_CHECKLIST.md`
- Merge feature/enemy-setup → main
- Delete branch (local + remote)
- Begin Phase 3 (SCHÖN - Polish)

---

## Risk Management

**LOW RISK Phases:**
- Phase 3 (Scene Setup): Zero code changes, reversible
- Phase 4 (Component Integration): Dual systems, old movement still works
- Phase 6 (Cleanup): Only deleting dead code
- Phase 7 (Testing): Readonly, no changes

**MEDIUM RISK Phases:**
- Phase 5 (Movement Migration): Core gameplay changes
  - Mitigation: Incremental (4 sub-phases), atomic commits, independent testing
  - Rollback: Git revert each commit individually

**HIGH RISK Areas (None Identified):**
- All phases have clear rollback plans
- Research identified no blockers
- Incremental approach minimizes risk

---

## Dependencies

**Phase 3 depends on:**
- GameLevel scene accessible
- Cave geometry has colliders

**Phase 4 depends on:**
- Phase 3 complete (NavMesh baked)

**Phase 5 depends on:**
- Phase 4 complete (NavMeshAgent component added)

**Phase 6 depends on:**
- Phase 5 complete (new movement working)

**Phase 7 depends on:**
- Phase 6 complete (clean codebase)

**No external dependencies or blockers.**

---

*Roadmap created: 2026-02-16*
*Phase 3 planned: 2026-02-17*
*Phase 4 planned: 2026-02-17*
*Next action: `/gsd:execute-phase 04-component-integration` to execute Phase 4*
