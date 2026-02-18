# Requirements: Snake Enchanter

**Defined:** 2026-02-18
**Core Value:** Precise timing gameplay that feels rewarding when mastered and punishing when failed

## v1.0 Requirements

Requirements for submission release. Each maps to roadmap phases.

### Spell System

- [ ] **SPELL-01**: Player can collect scroll prefabs placed at key positions in cave
- [ ] **SPELL-02**: Collecting a scroll permanently unlocks the corresponding tune
- [ ] **SPELL-03**: Tunes are locked until their scroll is collected (UI reflects locked state)
- [ ] **SPELL-04**: Tune 4 (Freeze) freezes all snakes when cast successfully

### Menu & UI

- [ ] **MENU-01**: Main Menu scene with mode selection (Simple/Advanced)
- [ ] **MENU-02**: Start Game button loads GameLevel scene
- [ ] **MENU-03**: Win screen shows game stats (time, spells cast, HP remaining)
- [ ] **MENU-04**: Brief fade transition when reaching exit before Win screen

### Backend

- [ ] **API-01**: Game session data posted to backend API on Win/Lose
- [ ] **API-02**: Leaderboard retrievable by mode (Simple/Advanced)
- [ ] **API-03**: Player stats aggregated and displayed

### Audio

- [ ] **AUDIO-01**: Flute melody plays during each tune cast (4 melodies, 5-12s each)
- [ ] **AUDIO-02**: Snake SFX (hiss ambient, bite, breath attack)
- [ ] **AUDIO-03**: Cave ambient music loop
- [ ] **AUDIO-04**: UI feedback sounds (slider, success, fail)

### Submission

- [ ] **SUB-01**: Game balancing pass (HP drain rates, timing windows, trigger zones)
- [ ] **SUB-02**: Windows .exe build + ZIP package
- [ ] **SUB-03**: Stable 60 FPS on school laptops

### Gameplay Enhancements (SHOULD)

- [ ] **GAME-01**: Player can jump (mapped to New Input System)
- [ ] **GAME-02**: Dynamic slider balancing (speed/zone variation per spell, HP-based scaling)
- [ ] **GAME-03**: Story/narrative intro displayed after menu before game starts
- [ ] **GAME-04**: Essential visual polish (fix yellow cave lights, damage flash, low HP vignette)
- [ ] **GAME-05**: SerializeField tooltips translated to English for consistency

### Extended Features (COULD)

- [ ] **EXT-01**: MiniMap showing player position in cave
- [ ] **EXT-02**: Second enemy system (RobotKyle with HP-based combat)
- [ ] **EXT-03**: Player spell cooldown (Inspector-configurable per spell)
- [ ] **EXT-04**: Player success rate system (50-90% based on HP)
- [ ] **EXT-05**: Spell range system with visual indicator
- [ ] **EXT-06**: Particle glow system (replace Material Emission for snakes)
- [ ] **EXT-07**: Arm animation clipping fix (spell animation through wall collider)

## Out of Scope

| Feature | Reason |
|---------|--------|
| Two-Level Success System | Too complex for timeline, discarded |
| Full visual polish (particles, screen shake, animation polish) | Only essential polish in scope |
| Mobile/other platforms | Windows only for academic submission |
| Multiplayer | Solo game, not in GDD |

## Traceability

| Requirement | Phase | Status |
|-------------|-------|--------|
| SPELL-01 | Phase 7 | Pending |
| SPELL-02 | Phase 7 | Pending |
| SPELL-03 | Phase 7 | Pending |
| SPELL-04 | Phase 7 | Pending |
| MENU-01 | Phase 8 | Pending |
| MENU-02 | Phase 8 | Pending |
| MENU-03 | Phase 8 | Pending |
| MENU-04 | Phase 8 | Pending |
| API-01 | Phase 9 | Pending |
| API-02 | Phase 9 | Pending |
| API-03 | Phase 9 | Pending |
| AUDIO-01 | Phase 10 | Pending |
| AUDIO-02 | Phase 10 | Pending |
| AUDIO-03 | Phase 10 | Pending |
| AUDIO-04 | Phase 10 | Pending |
| GAME-01 | Phase 11 | Pending |
| GAME-02 | Phase 11 | Pending |
| GAME-03 | Phase 11 | Pending |
| GAME-04 | Phase 11 | Pending |
| GAME-05 | Phase 11 | Pending |
| EXT-01 | Phase 12 | Pending |
| EXT-02 | Phase 12 | Pending |
| EXT-03 | Phase 12 | Pending |
| EXT-04 | Phase 12 | Pending |
| EXT-05 | Phase 12 | Pending |
| EXT-06 | Phase 12 | Pending |
| EXT-07 | Phase 12 | Pending |
| SUB-01 | Phase 13 | Pending |
| SUB-02 | Phase 13 | Pending |
| SUB-03 | Phase 13 | Pending |

**Coverage:**
- v1.0 requirements: 30 total (18 MUST, 5 SHOULD, 7 COULD)
- Mapped to phases: 30
- Unmapped: 0

---
*Requirements defined: 2026-02-18*
*Last updated: 2026-02-18 — traceability mapped to phases 7-13*
