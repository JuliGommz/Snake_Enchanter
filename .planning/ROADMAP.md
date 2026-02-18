# Roadmap: Snake Enchanter

## Milestones

- ✅ **v0.1 SPIELBAR** — Phase 1 (shipped 2026-02-09)
- ✅ **v0.2 KOMPLETT** — Phase 2 (shipped 2026-02-15)
- ✅ **v0.3 Bug Fixes & Stability** — Phases 3-6 (shipped 2026-02-18)
- 🚧 **v1.0 Submission Ready** — Phases 7-13 (in progress, deadline ~2026-03-03)

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

### 🚧 v1.0 Submission Ready (Phases 7-13)

**Milestone Goal:** Complete the game for PIP-3 Theme B academic submission — scroll-based spell progression, menu UI, backend stats, audio, and a stable Windows build by ~2026-03-03.

---

### Phase 7: Spell System
**Goal**: Players earn spells by finding scrolls (3 tunes: Move, Daze, Shield), tunes are locked until collected, HUD grows dynamically
**Depends on**: Phase 6 (TuneController v2.5, SnakeAI v1.8.5)
**Requirements**: SPELL-01, SPELL-02, SPELL-03, SPELL-04
**Scope change**: Reduced from 4 tunes to 3. Tune 3 (Attack) removed (needs creature system). Tune 4 (Freeze) removed (overlaps Daze). New Tune 3 = Shield (defensive, blocks next attack for 8s).
**Success Criteria** (what must be TRUE):
  1. 3 scroll prefabs exist in the cave (1 per QuestRoom path) and can be collected via walk-over or click
  2. Collecting a scroll permanently unlocks the matching tune slot, fires event, shows pause panel with description
  3. HUD starts empty — each scroll pickup adds a tune slot (key icon + spell name + color)
  4. Casting Tune 3 (Shield) gives player an 8s shield that blocks the next snake attack with screen-edge glow + shatter feedback
**Plans**: TBD

Plans:
- [ ] 07-01: Scroll pickup prefabs + SpellUnlockSystem (collect, persist unlock state, pause panel, fire events)
- [ ] 07-02: TuneController 3-tune refactor + dynamic HUD (empty→grow) + lock integration
- [ ] 07-03: Tune 3 Shield implementation (duration, block, visual feedback, no-recast-while-active)

### Phase 8: Menu & Win Screen
**Goal**: Players start a game from a proper menu with mode selection and see stats when they win
**Depends on**: Phase 7
**Requirements**: MENU-01, MENU-02, MENU-03, MENU-04
**Success Criteria** (what must be TRUE):
  1. Launching the game shows a Main Menu scene with Simple and Advanced mode buttons
  2. Pressing Start Game loads GameLevel and the selected mode is active immediately
  3. Reaching the exit triggers a visible screen fade before the Win screen appears
  4. Win screen shows time played, spells cast, and HP remaining from that session
**Plans**: TBD

Plans:
- [ ] 08-01: MainMenu scene — mode selection UI + Start Game button → GameLevel load
- [ ] 08-02: Win screen stats panel + fade-out transition wired to ExitTrigger

### Phase 9: Backend API
**Goal**: Session data posts automatically on game end and leaderboard/stats are retrievable
**Depends on**: Phase 8 (Win/Lose events and session stats exist)
**Requirements**: API-01, API-02, API-03
**Success Criteria** (what must be TRUE):
  1. On game end (win or lose) session data is automatically posted to the backend API with no manual action
  2. Leaderboard entries are retrievable filtered by mode (Simple / Advanced)
  3. Aggregated player stats are fetched and visible in the Win screen or a dedicated stats panel
**Plans**: TBD

Plans:
- [ ] 09-01: SessionData model + ApiService + POST /api/game-session on Win/Lose
- [ ] 09-02: GET /api/leaderboard + GET /api/player-stats + UI display integration

### Phase 10: Audio
**Goal**: The game sounds alive — melodies play during casts, snakes make noise, cave has atmosphere, UI gives audio feedback
**Depends on**: Phase 7 (tune cast events exist)
**Requirements**: AUDIO-01, AUDIO-02, AUDIO-03, AUDIO-04
**Success Criteria** (what must be TRUE):
  1. Each of the 4 tune casts plays its unique flute melody (5-12 seconds, does not interrupt gameplay controls)
  2. Snakes emit ambient hiss, bite impact, and breath attack sounds at the correct moments
  3. A cave ambient music loop plays continuously in the background throughout GameLevel
  4. Slider movement, spell success, and spell failure each trigger a distinct UI sound
**Plans**: TBD

Plans:
- [ ] 10-01: AudioManager singleton + cave ambient loop + snake SFX wiring to SnakeAI events
- [ ] 10-02: Tune melody playback per cast + UI feedback sounds (slider tick, success chime, fail sting)

### Phase 11: Gameplay Enhancements (SHOULD)
**Goal**: Jump works, slider feels tuned per HP, narrative sets the scene, and visual feedback makes damage legible
**Depends on**: Phase 10 (all MUST features complete)
**Requirements**: GAME-01, GAME-02, GAME-03, GAME-04, GAME-05
**Success Criteria** (what must be TRUE):
  1. Player can jump using the New Input System — no legacy Input calls
  2. Slider speed and trigger zone width vary per spell and scale with current player HP
  3. A story intro text panel displays after the menu before GameLevel loads
  4. Cave lights show warm yellow tones, player screen flashes red on damage hit, a red vignette appears at low HP
**Plans**: TBD

Plans:
- [ ] 11-01: Jump mechanic — New Input System binding, PlayerController integration, animation
- [ ] 11-02: Dynamic slider balancing — speed/zone per spell + HP scaling in TuneController
- [ ] 11-03: Story intro panel + SerializeField tooltip English pass + visual polish (lights, damage flash, vignette)

### Phase 12: Extended Features (COULD)
**Goal**: Optional extras that add depth — only executed if time remains after Phase 11
**Depends on**: Phase 11
**Requirements**: EXT-01, EXT-02, EXT-03, EXT-04, EXT-05, EXT-06, EXT-07
**Success Criteria** (what must be TRUE):
  1. A minimap shows player position within the cave layout in real time
  2. A second enemy type (RobotKyle) exists with HP-based melee combat
  3. Each spell has a configurable cooldown timer visible in the Inspector
  4. Success rate scaling, spell range indicator, snake particle glow, and arm clip fix are each implemented as capacity allows
**Plans**: TBD

Plans:
- [ ] 12-01: MiniMap — camera + RenderTexture + UI panel (EXT-01)
- [ ] 12-02: Second enemy RobotKyle with HP and melee combat (EXT-02)
- [ ] 12-03: Spell cooldown per tune (Inspector configurable) (EXT-03)
- [ ] 12-04: Success rate system (EXT-04), Spell range indicator (EXT-05), Snake particle glow (EXT-06), Arm clip fix (EXT-07)

### Phase 13: Submission
**Goal**: A stable 60 FPS Windows .exe is packaged and the full game plays through without crashes
**Depends on**: Phase 12 (or Phase 11 if Phase 12 is skipped)
**Requirements**: SUB-01, SUB-02, SUB-03
**Success Criteria** (what must be TRUE):
  1. HP drain rates, timing windows, and trigger zones are tuned so both Simple and Advanced modes feel appropriately difficult
  2. A Windows .exe plus all required files are packaged in a single ZIP ready for submission
  3. The game runs at stable 60 FPS on school laptops with no significant frame drops during snake combat
**Plans**: TBD

Plans:
- [ ] 13-01: Balancing pass — drain rates, timing windows, trigger zones tested in both modes
- [ ] 13-02: Windows build + ZIP packaging + smoke test on school hardware

---

## Progress

**Execution Order:** 7 → 8 → 9 → 10 → 11 → 12 → 13

| Phase | Milestone | Plans Complete | Status | Completed |
|-------|-----------|----------------|--------|-----------|
| 1. Core Implementation | v0.1 | 1/1 | Complete | 2026-02-09 |
| 2. Enemy Setup | v0.2 | 1/1 | Complete | 2026-02-15 |
| 3. NavMesh Scene Setup | v0.3 | 1/1 | Complete | 2026-02-17 |
| 4. Component Integration | v0.3 | 2/2 | Complete | 2026-02-17 |
| 5. Movement Migration | v0.3 | 4/3 | Complete | 2026-02-17 |
| 6. Cleanup & Polish | v0.3 | 1/1 | Complete | 2026-02-17 |
| 7. Spell System | v1.0 | 0/3 | Not started | - |
| 8. Menu & Win Screen | v1.0 | 0/2 | Not started | - |
| 9. Backend API | v1.0 | 0/2 | Not started | - |
| 10. Audio | v1.0 | 0/2 | Not started | - |
| 11. Gameplay Enhancements | v1.0 | 0/3 | Not started | - |
| 12. Extended Features | v1.0 | 0/4 | Not started | - |
| 13. Submission | v1.0 | 0/2 | Not started | - |
