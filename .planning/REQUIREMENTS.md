# Requirements: Snake Enchanter

**Defined:** 2026-02-18 (original), **Restructured:** 2026-02-24
**Core Value:** Precise timing gameplay that feels rewarding when mastered and punishing when failed

## v1.0 MVP Requirements

Requirements for academic submission. Restructured 2026-02-24 for MVP focus (cave rebuild + simplified scope).

### Cave System

- [ ] **CAVE-01**: New simplified cave layout exists with clear path from start to exit
- [ ] **CAVE-02**: Cave has 3 distinct areas/rooms where scroll pickups can be placed
- [ ] **CAVE-03**: NavMesh is baked and functional for snake pathfinding in new cave

### Enemy Setup

- [ ] **ENEMY-01**: Snakes are placed in new cave with patrol waypoints
- [ ] **ENEMY-02**: Snake AI functions correctly (patrol, detect, attack, respond to spells)

### Spell Wiring

- [ ] **SPELL-01**: 3 scroll objects placed in cave, collectible via walk-over or interact
- [ ] **SPELL-02**: Collecting scroll unlocks tune + shows pause panel with lore text
- [ ] **SPELL-03**: SpellHUD starts empty, grows dynamically per scroll collected
- [ ] **SPELL-04**: Shield blocks next snake attack (8s duration, screen glow)
- [ ] **SPELL-05**: HP heals only on successful charm (Move/Daze), not Shield

### Menu & UI

- [ ] **MENU-01**: Main Menu scene with Simple/Advanced mode selection
- [ ] **MENU-02**: Start Game loads GameLevel with selected mode active
- [ ] **MENU-03**: Win screen shows time played, spells cast, HP remaining
- [ ] **MENU-04**: Fade transition before Win screen appears

### Backend

- [ ] **API-01**: Session data posted to backend API on game end (Win/Lose)
- [ ] **API-02**: Leaderboard retrievable by mode (Simple/Advanced)
- [ ] **API-03**: Player stats aggregated and displayed

### Audio (Minimal)

- [ ] **AUDIO-01**: Flute melody plays during each tune cast (3 existing MP3s)
- [ ] **AUDIO-02**: Cave ambient music loop plays in GameLevel

### Submission

- [ ] **SUB-01**: Game balancing pass (HP drain, timing windows, trigger zones)
- [ ] **SUB-02**: Windows .exe build + ZIP package
- [ ] **SUB-03**: Stable 60 FPS on school laptops

## v2 Requirements (Deferred)

Features cut from MVP. Can be added post-submission if time allows.

### Gameplay Enhancements

- **GAME-01**: Player can jump (New Input System)
- **GAME-02**: Dynamic slider balancing (speed/zone per spell, HP scaling)
- **GAME-03**: Story/narrative intro after menu
- **GAME-04**: Visual polish (damage flash, low HP vignette, yellow light fix)
- **GAME-05**: SerializeField tooltips translated to English

### Extended Features

- **EXT-01**: MiniMap showing player position
- **EXT-02**: Second enemy system (RobotKyle with HP)
- **EXT-04**: Player success rate system (HP-based)
- **EXT-06**: Particle glow system (replace Material Emission)
- **EXT-07**: Arm animation clipping fix

### Audio Extended

- **AUDIO-03**: Snake SFX (hiss, bite, breath)
- **AUDIO-04**: UI feedback sounds (slider tick, success chime, fail sting)

## Out of Scope

| Feature | Reason |
|---------|--------|
| Two-Level Success System | Too complex, discarded |
| Tune 3 Attack (creature targeting) | Removed — needs creature system not in game |
| Tune 4 Freeze | Removed — overlaps Daze functionality |
| Slither Left/Right testing | Not necessary per user |
| Mobile/other platforms | Windows only for academic submission |
| Multiplayer | Solo game, not in GDD |

## Traceability

Which phases cover which requirements. Updated 2026-02-24.

| Requirement | Phase | Status |
|-------------|-------|--------|
| CAVE-01 | Phase 7 | Pending |
| CAVE-02 | Phase 7 | Pending |
| CAVE-03 | Phase 7 | Pending |
| ENEMY-01 | Phase 8 | Pending |
| ENEMY-02 | Phase 8 | Pending |
| SPELL-01 | Phase 8 | Pending |
| SPELL-02 | Phase 8 | Pending |
| SPELL-03 | Phase 8 | Pending |
| SPELL-04 | Phase 8 | Pending |
| SPELL-05 | Phase 8 | Pending |
| AUDIO-01 | Phase 8 | Pending |
| AUDIO-02 | Phase 8 | Pending |
| MENU-01 | Phase 9 | Pending |
| MENU-02 | Phase 9 | Pending |
| MENU-03 | Phase 9 | Pending |
| MENU-04 | Phase 9 | Pending |
| API-01 | Phase 10 | Pending |
| API-02 | Phase 10 | Pending |
| API-03 | Phase 10 | Pending |
| SUB-01 | Phase 11 | Pending |
| SUB-02 | Phase 11 | Pending |
| SUB-03 | Phase 11 | Pending |

**Coverage:**
- v1.0 MVP requirements: 21 total (all MUST)
- Mapped to phases: 21/21
- Unmapped: 0

---
*Requirements defined: 2026-02-18*
*Restructured: 2026-02-24 for MVP focus (cave rebuild + simplified scope)*
*Traceability updated: 2026-02-24 (Phases 7-11)*
