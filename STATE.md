# PROJECT STATE - Snake Enchanter

**Letzte Aktualisierung:** 2026-03-01

---

## QUICK START

**Branch:** `feature/general-improvements`
**Letzter Commit:** e57cac3 (feat: restore MusicManager + add assets and screenshots)
**Working Tree:** Dirty — Script-Fixes + Scenes uncommitted

**Milestone:** v1.0 Submission Ready (Phases 7-13)
**Aktuelle Phase:** General Improvements / Phase 11 Polish
**Fortschritt:** ~85% von v1.0

---

## WAS GERADE PASSIERT

Phase 10 (Audio) DONE. General Improvements Session abgeschlossen.

### IN DIESEM BRANCH (feature/general-improvements) gemacht:
- ✅ Breath Attack Fix (CancelInvoke, _canSeePlayer)
- ✅ Advanced Mode Difficulty (+15% Damage, Drain 0.25f)
- ✅ Charges System entfernt (TuneController v3.3)
- ✅ Snake Behavior Fix (Basic Snake FollowPlayer in Breath-Range)
- ✅ Shield Bypass Fix (TakeSnakeAttackDamage in HealthSystem)
- ✅ HealthBarUI Cleanup (_debuffText/_debuffMessage entfernt)

### NÄCHSTER SCHRITT: Main Story Text
- [ ] Intro-Text / Story vor Game Start einbauen
- [ ] Noch im Branch `feature/general-improvements`

### Offene Items (deferred):
- CooldownOverlay FillOrigin → wenn Spell Icons erstellt werden
- SpellScrollPickup + SpellUnlockSystem in Scene (`_unlockAllOnStart = true` überbrückt)
- 3D Scrolls in Cave platzieren

---

## ABGESCHLOSSENE MILESTONES

| Milestone | Status | Shipped |
|-----------|--------|---------|
| v0.1 SPIELBAR | DONE | 2026-02-09 |
| v0.2 KOMPLETT | DONE | 2026-02-15 |
| v0.3 Bug Fixes & Stability | DONE | 2026-02-18 |

## v1.0 PHASEN-ÜBERSICHT

| Phase | Beschreibung | Status |
|-------|-------------|--------|
| 7 | Spell System (3 Tunes: Move, Daze, Shield) | ✅ DONE |
| 8 | Menu & UI | ✅ DONE |
| 9 | Backend & Stats | ✅ DONE |
| 10 | Audio & Music | ✅ DONE |
| 11 | Polish & Juice | 🔄 in progress |
| 12 | Testing & QA | pending |
| 13 | Build & Submission | pending |

---

## WAS FUNKTIONIERT

- Player Controller v1.9 (New Input System, Crouch, Cinemachine v3.x, walk-on-spawn fix)
- Health System v1.5 (Drain, Events, Death Animations, heal-on-charm, shield intercept)
- Tune System (TuneController v3.3, 3-Tune Array, Debug Unlock, Cooldown — Charges entfernt MVP)
- Snake AI v2.1 (NavMesh, Entranced→Dazed two-phase, CancelInvoke safety, Dead state removed)
- SpellScrollPickup + SpellUnlockSystem (code ready, NOT in scene)
- SpellHUDController v1.1 (dynamic HUD, cooldown overlay, range indicator)
- ShieldComponent v1.1 (duration from TuneConfig, blocks next attack)
- HealthSystem v1.5 + TakeSnakeAttackDamage() — Shield-Routing korrekt für alle Snake-Damage-Pfade
- MusicManager v1.0 (scene-based, gameplay alternation)
- Controls Overlay (Keycap Images in-game)
- Cave Map (Caves Parts Set + Dwarven Pack + ProBuilder/Polybrush)
- Canvas UI: HealthBarUI v3.1 + TuneSliderUI v2.1
- Cinemachine v3.x (CM_PlayerCamera, CinemachineBrain)
- Win Condition (ExitTrigger, ResetTrigger on Retry)
- Full Game Loop (GameManager v1.3, MainMenuController, ResultScreenController)
- Backend REST API (Node.js + Express + MySQL) — localhost:3000
- ApiManager.cs v1.0 (Unity → Backend, fail-silent)
- START_SERVER.bat (Dozenten-Deployment)
- MainMenu Scene (Simple/Advanced/Quit)
- Pirate Character: Humanoid Avatar, 10 Animations, Bake Into Pose

---

## FOLDER STRUCTURE

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
  ScriptableObjects/     # TuneConfigs: Tune1_Move, Tune2_Daze, Tune3_Shield
  Scenes/                # MainMenu, GameLevel
  Data/                  # Input Actions
  Design/
  Media/                 # Audio/Music, Audio/SFX, Audio/Tunes
```

---

## GIT STATUS

```
Branch: feature/phase9-backend
Letzter Commit: eb1bf69 (feat: START_SERVER.bat + README_DOZENTEN)
Remote: https://github.com/JuliGommz/Snake_Enchanter.git
Working Tree: Clean (Unity scenes auto-modified — nicht committed)
Merge-Bereit: Nein — erst End-to-End Test
```

---

## REGELN (NICHT VERHANDELBAR)

### Input System
AUSSCHLIESSLICH Unity New Input System! NIEMALS `UnityEngine.Input` (Legacy).

### Kamera-System (Cinemachine v3.x)
- Cinemachine besitzt Kamera-Position. NIEMALS per Script überschreiben.
- PlayerController steuert NUR Pitch (Mouse Y) + Body Yaw (Mouse X)

### Animation
- Humanoid Rig (Pirate.FBX + alle Animations)
- Bake Into Pose: Rotation (Body Orientation), Y (Feet), XZ (Center of Mass)
- Root Motion OFF (CharacterController steuert Movement)

### Git Workflow
- Feature Branches: `feature/<name>` from main
- Ein Feature = Ein Branch
- Nach Merge: Branch löschen

### Spell System (Phase 7)
- 3 Tunes: Move (1), Daze (2), Shield (3)
- Daze = zweiphasig: Entranced (3s, amber) → Dazed (8s, blau, Die-Anim)
- Melody spielt nur bei Success (nicht während Hold)
- Shield Duration = Melody Section Length (TuneConfig = Single Source)
- Debug: `_unlockAllOnStart = true` (für Testing, vor Release auf false setzen)
- HP heals only on successful charm (Move/Daze), not Shield
- Cooldown + Charges (Advanced mode)

### Wichtige Dateien für Spell-System
| Datei | Beschreibung |
|-------|-------------|
| `Scripts/TuneSystem/TuneController.cs` | v3.2 — Slider, Melody, Shield-Kopplung |
| `Scripts/Snakes/SnakeAI.cs` | v2.1 — Entranced State, CancelInvoke, Dead removed |
| `Scripts/Player/ShieldComponent.cs` | v1.1 — Duration + ShieldText TMP |
| `Scripts/TuneSystem/TuneConfig.cs` | ScriptableObject Definition |
| `ScriptableObjects/TuneConfigs/Tune1_Move.asset` | Move Config (melody 0-12s) |
| `ScriptableObjects/TuneConfigs/Tune2_Daze.asset` | Daze Config |
| `ScriptableObjects/TuneConfigs/Tune3_Shield.asset` | Shield Config (melody 10-25s = 15s shield) |
