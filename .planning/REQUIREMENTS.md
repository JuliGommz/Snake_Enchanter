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
| SPELL-01 | Pending | Pending |
| SPELL-02 | Pending | Pending |
| SPELL-03 | Pending | Pending |
| SPELL-04 | Pending | Pending |
| MENU-01 | Pending | Pending |
| MENU-02 | Pending | Pending |
| MENU-03 | Pending | Pending |
| MENU-04 | Pending | Pending |
| API-01 | Pending | Pending |
| API-02 | Pending | Pending |
| API-03 | Pending | Pending |
| AUDIO-01 | Pending | Pending |
| AUDIO-02 | Pending | Pending |
| AUDIO-03 | Pending | Pending |
| AUDIO-04 | Pending | Pending |
| SUB-01 | Pending | Pending |
| SUB-02 | Pending | Pending |
| SUB-03 | Pending | Pending |
| GAME-01 | Pending | Pending |
| GAME-02 | Pending | Pending |
| GAME-03 | Pending | Pending |
| GAME-04 | Pending | Pending |
| GAME-05 | Pending | Pending |
| EXT-01 | Pending | Pending |
| EXT-02 | Pending | Pending |
| EXT-03 | Pending | Pending |
| EXT-04 | Pending | Pending |
| EXT-05 | Pending | Pending |
| EXT-06 | Pending | Pending |
| EXT-07 | Pending | Pending |

**Coverage:**
- v1.0 requirements: 30 total (18 MUST, 5 SHOULD, 7 COULD)
- Mapped to phases: 0
- Unmapped: 30

---
*Requirements defined: 2026-02-18*
*Last updated: 2026-02-18 after initial definition*
