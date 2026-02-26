# Roadmap: Snake Enchanter

## Milestones

- ✅ **v0.1 SPIELBAR** — Phase 1 (shipped 2026-02-09)
- ✅ **v0.2 KOMPLETT** — Phase 2 (shipped 2026-02-15)
- ✅ **v0.3 Bug Fixes & Stability** — Phases 3-6 (shipped 2026-02-18)
- 🚧 **v1.0 MVP Submission** — Phases 7-11 (in progress, deadline ~2026-03-03)

## Phases

<details>
<summary>✅ v0.1 SPIELBAR (Phase 1) — SHIPPED 2026-02-09</summary>

- [x] Phase 1: Core Implementation — PlayerController, HealthSystem, TuneController, GameManager, ExitTrigger, Pirate character, Cave environment, Canvas UI

</details>

<details>
<summary>✅ v0.2 KOMPLETT (Phase 2) — SHIPPED 2026-02-15</summary>

- [x] Phase 2: Enemy Setup — SnakeAI v1.7.2, 7-state machine, 4 Tunes, 3 attack types, directional slither, visual feedback

</details>

<details>
<summary>✅ v0.3 Bug Fixes & Stability (Phases 3-6) — SHIPPED 2026-02-18</summary>

- [x] Phase 3: NavMesh Scene Setup (1/1 plans) — completed 2026-02-17
- [x] Phase 4: Component Integration (2/2 plans) — completed 2026-02-17
- [x] Phase 5: Movement Migration (3/3 plans executed) — completed 2026-02-17
- [x] Phase 6: Cleanup & Polish (1/1 plans) — completed 2026-02-17

See: `.planning/milestones/v0.3-ROADMAP.md` for full details

</details>

---

### 🚧 v1.0 MVP Submission (Phases 7-11)

**Milestone Goal:** Complete the game for PIP-3 Theme B academic submission — new cave layout, all spell systems wired in-editor, menu UI, backend stats, audio, and a stable Windows build by ~2026-03-03.

**Note:** Old Phase 7-13 plan replaced 2026-02-24. Spell system CODE is already complete. Work remaining is cave rebuild + editor wiring + peripheral systems.

---

### Phase 7: Cave Rebuild
**Goal**: Players can walk a coherent path through a playable cave with a clear start, 3 distinct rooms, and a reachable exit
**Depends on**: Phase 6
**Requirements**: CAVE-01, CAVE-02, CAVE-03
**Success Criteria** (what must be TRUE):
  1. Player can walk from the spawn point to the exit without hitting invisible walls or falling through the floor
  2. The cave has 3 identifiable areas (rooms) that each have floor space large enough to place a scroll and a snake
  3. NavMesh is baked and covers all walkable floor surfaces — no snake pathfinding gaps between rooms
**Plans**: 2 plans

Plans:
- [ ] 07-01-PLAN.md — Delete old cave, build new simplified modular layout (start hall, 3 rooms, exit corridor) with correct colliders
- [ ] 07-02-PLAN.md — NavMesh bake: configure agent settings, verify bake covers all 3 rooms, smoke test snake pathfinding

### Phase 8: Scene Integration
**Goal**: The cave is fully populated — snakes patrol, scrolls exist and work, every spell system component is wired and functional in-editor
**Depends on**: Phase 7 (cave layout + NavMesh)
**Requirements**: ENEMY-01, ENEMY-02, SPELL-01, SPELL-02, SPELL-03, SPELL-04, SPELL-05, AUDIO-01, AUDIO-02
**Success Criteria** (what must be TRUE):
  1. Each room has at least one snake on patrol waypoints that detects the player, attacks on failed spells, and responds correctly to Move and Daze
  2. Three scroll objects exist in the cave (one per room) and are collected by walking over them
  3. Collecting a scroll shows the pause panel with the correct lore text, then permanently unlocks the tune slot in the HUD
  4. SpellHUD starts empty at game launch and gains one slot per scroll collected (correct icon, key, color)
  5. Casting Shield gives an 8-second block (screen-edge glow visible), blocks the next snake attack, and HP heals only on Move/Daze charm success
  6. Flute melody plays when a tune is cast; cave ambient music loops continuously from game start
**Plans**: 2 plans

Plans:
- [ ] 08-01-PLAN.md — Place snakes + waypoints in 3 rooms, wire all Inspector references (SnakeAI, TuneController layer mask, ShieldComponent border glow), verify patrol + detection
- [ ] 08-02-PLAN.md — Place 3 scroll pickups, wire SpellUnlockSystem + SpellHUDController + lore panels, wire AudioSources for melodies + ambient loop

### Phase 9: Menu & Win Screen
**Goal**: Players start from a proper menu with mode selection and see their session stats on the Win screen
**Depends on**: Phase 8
**Requirements**: MENU-01, MENU-02, MENU-03, MENU-04
**Success Criteria** (what must be TRUE):
  1. Launching the game shows a Main Menu scene with Simple and Advanced mode buttons
  2. Pressing Start Game loads GameLevel and the selected mode (drain rate, timing windows) is active from the first second
  3. Reaching the exit triggers a visible screen fade before the Win screen appears
  4. Win screen displays time played, spells cast, and HP remaining from that run
**Plans**: 2 plans

Plans:
- [ ] 09-01-PLAN.md — MainMenu scene: mode selection UI + Start Game button, mode state passed to GameManager on scene load
- [ ] 09-02-PLAN.md — Win screen stats panel (time, spells cast, HP) + fade-out transition wired to ExitTrigger

### Phase 10: Backend API
**Goal**: Session data posts automatically on game end and leaderboard/player stats are retrievable without manual action
**Depends on**: Phase 9 (Win/Lose events and session stats wired)
**Requirements**: API-01, API-02, API-03
**Success Criteria** (what must be TRUE):
  1. On game end (win or lose) session data posts to the backend API automatically with no manual action required
  2. Leaderboard entries are retrievable filtered by mode (Simple / Advanced)
  3. Aggregated player stats are fetched and visible on the Win screen or a dedicated stats panel
**Plans**: 2 plans

Plans:
- [ ] 10-01-PLAN.md — SessionData model + ApiService + POST /api/game-session on Win/Lose events
- [ ] 10-02-PLAN.md — GET /api/leaderboard + GET /api/player-stats + display on Win screen UI

### Phase 11: Balancing & Build
**Goal**: Both game modes feel correctly tuned and a stable 60 FPS Windows .exe is packaged and ready for submission
**Depends on**: Phase 10
**Requirements**: SUB-01, SUB-02, SUB-03
**Success Criteria** (what must be TRUE):
  1. Simple mode feels forgiving — timing windows are wide, HP drain is slow enough that a new player can complete a run
  2. Advanced mode feels tense — timing windows are stricter, HP drain is 15% faster, surviving requires using all 3 spells
  3. A Windows .exe plus all required files are packaged in a single ZIP with no missing dependencies
  4. The game runs at stable 60 FPS on school laptops with no significant frame drops during snake combat
**Plans**: 2 plans

Plans:
- [ ] 11-01-PLAN.md — Balancing pass: HP drain rates, timing windows, trigger zones tested in both modes and adjusted
- [ ] 11-02-PLAN.md — Windows build + ZIP packaging + smoke test on school hardware

---

## Progress

**Execution Order:** 7 → 8 → 9 → 10 → 11

| Phase | Milestone | Plans Complete | Status | Completed |
|-------|-----------|----------------|--------|-----------|
| 1. Core Implementation | v0.1 | 1/1 | Complete | 2026-02-09 |
| 2. Enemy Setup | v0.2 | 1/1 | Complete | 2026-02-15 |
| 3. NavMesh Scene Setup | v0.3 | 1/1 | Complete | 2026-02-17 |
| 4. Component Integration | v0.3 | 2/2 | Complete | 2026-02-17 |
| 5. Movement Migration | v0.3 | 4/3 | Complete | 2026-02-17 |
| 6. Cleanup & Polish | v0.3 | 1/1 | Complete | 2026-02-17 |
| 7. Cave Rebuild | v1.0 | 0/2 | Not started | - |
| 8. Scene Integration | v1.0 | 0/2 | Not started | - |
| 9. Menu & Win Screen | v1.0 | 0/2 | Not started | - |
| 10. Backend API | v1.0 | 0/2 | Not started | - |
| 11. Balancing & Build | v1.0 | 0/2 | Not started | - |
