# PROJECT STATE - Snake Enchanter

**Letzte Aktualisierung:** 2026-02-24

---

## QUICK START

**Branch:** `feature/cave-rebuild`
**Letzter Commit:** `c69ed43` - "fix(pirate): convert animation system to Humanoid + Bake Into Pose"

**Milestone:** v1.0 Submission Ready (Phases 7-13)
**Aktuelle Phase:** 7 - Spell System (Plans 07-01 bis 07-03 done, 07-04 auto tasks done - checkpoint pending)
**Fortschritt:** ~30% von v1.0

**GSD Tracking:** `.planning/STATE.md` (detaillierter Stand)

---

## WAS GERADE PASSIERT

Cave-Rebuild Branch: Asset-Konsolidierung + Pirate Animation Fix abgeschlossen.
- Alle Art-Assets in einheitliche Struktur unter Art-Visuals/ konsolidiert
- Pirate Animation System auf Humanoid umgestellt (war Generic ohne Avatar)
- Bake Into Pose konfiguriert (Rotation/Y/XZ) — Player sinkt nicht mehr ein
- 3 unbenutzte Animations + orphaned Cave Prefabs geloscht
- Cave Materials auf URP Lit konvertiert, NavMesh gebacken

**Nachster Schritt:** feature/cave-rebuild in main mergen, dann weiter mit Phase 7 Checkpoint oder Phase 8.

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
- Pirate Character: Humanoid Avatar, 10 Animations, Bake Into Pose

---

## FOLDER STRUCTURE (nach Konsolidierung 2026-02-24)

```
Assets/_Project/
  Art-Visuals/
    Images/              # UI Textures (teastain)
    Prefabs-FBX-Materials-Animations/
      Cave/              # FBX/, Materials/, Prefabs/ (Sample.prefab), Textures/
      Pirate/            # Mesh/, Animations/, Materials/, Prefabs/, Textures/, MC_Controller
      Props/             # Dungeon Props (Dwarven Pack)
      Snakes/            # Controllers/, FBX/, Materials/, Prefabs/, Textures/
  Scripts/               # Core, Player, Snakes, TuneSystem, UI, Data, Level, Editor
  ScriptableObjects/
  Scenes/                # MainMenu, GameLevel
  Data/                  # Input Actions
  Design/
  Media/                 # Audio/Music, Audio/SFX, Audio/Tunes
```

---

## GIT STATUS

```
Branch: feature/cave-rebuild
Letzter Commit: c69ed43 "fix(pirate): convert animation system to Humanoid + Bake Into Pose"
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
- Humanoid Rig (Pirate.FBX + alle Animations)
- Bake Into Pose: Rotation (Body Orientation), Y (Feet), XZ (Center of Mass)
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
