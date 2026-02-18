# PROJECT STATE - Snake Enchanter

**Letzte Aktualisierung:** 2026-02-18

---

## QUICK START

**Branch:** `main`
**Letzter Commit:** `d6eaec0` - "refactor: restructure _Project folder"

**Milestone:** v1.0 Submission Ready (Phases 7-13)
**Aktuelle Phase:** 7 - Spell System (Plans 07-01 bis 07-03 done, 07-04 auto tasks done - checkpoint pending)
**Fortschritt:** ~30% von v1.0

**GSD Tracking:** `.planning/STATE.md` (detaillierter Stand)

---

## WAS GERADE PASSIERT

Phase 7 (Spell System) ist fast fertig. Plans 07-01 bis 07-03 sind komplett implementiert. Plan 07-04 (Spell Casting Rules) auto tasks sind done, wartet auf Human Verification im Unity Editor.

Parallel dazu: Folder Restructuring wurde durchgefuhrt und committed.

---

## ABGESCHLOSSENE MILESTONES

| Milestone | Status | Shipped |
|-----------|--------|---------|
| v0.1 SPIELBAR | DONE | 2026-02-09 |
| v0.2 KOMPLETT | DONE | 2026-02-15 |
| v0.3 Bug Fixes & Stability | DONE | 2026-02-18 |

## v1.0 PHASEN-UBERSICHT

| Phase | Beschreibung | Status |
|-------|-------------|--------|
| 7 | Spell System (3 Tunes: Move, Daze, Shield) | ~90% (checkpoint pending) |
| 8 | Menu & UI | pending |
| 9 | Backend & Stats | pending |
| 10 | Audio & Music | pending |
| 11 | Polish & Juice | pending |
| 12 | Testing & QA | pending |
| 13 | Build & Submission | pending |

---

## WAS FUNKTIONIERT

- Player Controller v1.8 (New Input System, Crouch, Cinemachine v3.x)
- Health System v1.5 (Drain, Events, Death Animations, heal-on-charm, shield intercept)
- Tune System (TuneController v3.1, 3-Tune Array, Unlock Gate, Cooldown, Charges)
- Snake AI v1.9 (NavMesh, SnakeCharmed event, 7-state machine)
- SpellScrollPickup + SpellUnlockSystem (scroll-based unlock)
- SpellHUDController v1.1 (dynamic HUD, cooldown overlay, range indicator)
- ShieldComponent (8s shield, blocks next attack)
- Cave Map (Caves Parts Set + Dwarven Pack + ProBuilder/Polybrush)
- Canvas UI: HealthBarUI v3.1 + TuneSliderUI v2.1
- Cinemachine v3.x (CM_PlayerCamera, CinemachineBrain)
- Win Condition (ExitTrigger)
- Game Loop (GameManager)
- Pirate Character komplett setup

---

## FOLDER STRUCTURE (nach Restructuring 2026-02-18)

```
Assets/_Project/
  Art-Visuals/
    Animations/          # MC_Controller, Pirate Anims + FBX
    Images/              # UI Textures
    Prefabs-FBX-Materials/
      Cave/              # Cave FBX, Materials, Prefabs, Textures
      Pirate/            # Pirate Prefab + Original Assets
      Props/             # Dungeon Props
      Snakes/            # Snake Prefabs, FBX, Controllers, Materials
  Scripts/               # Core, Player, Snakes, TuneSystem, UI, Data, Level, Editor
  ScriptableObjects/
  Scenes/                # MainMenu, GameLevel
  Data/                  # Input Actions
  Design/
  Media/
```

---

## GIT STATUS

```
Branch: main
Letzter Commit: d6eaec0 "refactor: restructure _Project folder"
Remote: https://github.com/JuliGommz/Snake_Enchanter.git
```

---

## REGELN (NICHT VERHANDELBAR)

### Input System
AUSSCHLIESSLICH Unity New Input System! NIEMALS `UnityEngine.Input` (Legacy).

### Kamera-System (Cinemachine v3.x)
- Cinemachine besitzt Kamera-Position. NIEMALS per Script uberschreiben.
- PlayerController steuert NUR Pitch (Mouse Y) + Body Yaw (Mouse X)

### Animation
- Spell Animation (keine Flote)
- Root Motion OFF (CharacterController steuert Movement)

### Git Workflow
- Feature Branches: `feature/<name>` from main
- Ein Feature = Ein Branch
- Nach Merge: Branch loschen

### Spell System (Phase 7)
- 3 Tunes: Move, Daze, Shield (Attack + Freeze removed)
- Scroll-based unlock (Zelda-style)
- HP heals only on successful charm (Move/Daze), not Shield
- Cooldown + Charges (Advanced mode)
