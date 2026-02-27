# PROJECT STATE - Snake Enchanter

**Letzte Aktualisierung:** 2026-02-27

---

## QUICK START

**Branch:** `feature/phase9-backend`
**Letzter Commit:** eb1bf69 (feat: START_SERVER.bat + README_DOZENTEN)
**Working Tree:** Clean (scene files auto-modified by Unity, nicht committed)

**Milestone:** v1.0 Submission Ready (Phases 7-13)
**Aktuelle Phase:** 9 - Backend & Stats (IN PROGRESS — bereit für Test + Merge)
**Fortschritt:** ~70% von v1.0

---

## WAS GERADE PASSIERT

Phase 9 Backend vollständig implementiert. Bereit für End-to-End Test.

### Was Phase 9 gemacht hat:
1. **backend/server.js v1.0** — Node.js + Express REST API (localhost:3000)
   - POST `/api/game-session` — Session-Stats nach jedem Run speichern
   - GET `/api/leaderboard?mode=simple|advanced` — Top 10 Wins (schnellste Zeit)
   - GET `/api/player-stats` — Aggregierte Stats (COUNT, AVG, MIN, SUM)
   - GET `/api/health` — Server Health Check
2. **backend/database.js v1.0** — better-sqlite3, WAL-Mode, game_sessions Tabelle
3. **backend/package.json** — express ^5, better-sqlite3 ^12
4. **backend/.gitignore** — node_modules/, *.db, *.db-shm, *.db-wal
5. **ApiManager.cs v1.0** — Unity Singleton, UnityWebRequest + Coroutines, fail-silent
6. **GameManager.cs v1.3** — `SendSessionToBackend()` in EndGame() integriert
7. **START_SERVER.bat** — Auto npm install + node server.js starten (für Dozenten)
8. **README_DOZENTEN.txt** — Anleitung für Dozenten (Deutsch)

### Evaluation-Flow (Dozenten bewerten vom Repository):
```
1. Repo klonen / öffnen
2. backend/START_SERVER.bat doppelklicken → npm install + server startet
3. Unity Projekt öffnen → Play drücken
4. Spiel spielen → Session-Daten gehen automatisch an localhost:3000
5. http://localhost:3000/api/player-stats zum Verifizieren
```

### NÄCHSTER SCHRITT:
- [ ] **Backend End-to-End Test:**
  1. `backend/START_SERVER.bat` ausführen
  2. Unity Play → Spiel spielen → Win oder Lose
  3. Browser: `http://localhost:3000/api/player-stats` — Session muss erscheinen
  4. Wenn OK: `feature/phase9-backend` → `main` mergen
- [ ] **ApiManager-GameObject** in GameLevel Scene hinzufügen (falls noch nicht da)
- [ ] **Phase 10: Audio** beginnen

### Offene Items (deferred):
- CooldownOverlay FillOrigin → wenn Spell Icons erstellt werden
- SpellScrollPickup + SpellUnlockSystem in Scene (`_unlockAllOnStart = true` überbrückt bis Phase 11)
- 3D Scrolls in Cave platzieren (Phase 11 Polish)

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
| 9 | Backend & Stats | 🔄 IN PROGRESS (bereit für Test+Merge) |
| 10 | Audio & Music | pending |
| 11 | Polish & Juice | pending |
| 12 | Testing & QA | pending |
| 13 | Build & Submission | pending |

---

## WAS FUNKTIONIERT

- Player Controller v1.9 (New Input System, Crouch, Cinemachine v3.x, walk-on-spawn fix)
- Health System v1.5 (Drain, Events, Death Animations, heal-on-charm, shield intercept)
- Tune System (TuneController v3.2, 3-Tune Array, Debug Unlock, Cooldown, Charges)
- Snake AI v2.1 (NavMesh, Entranced→Dazed two-phase, CancelInvoke safety, Dead state removed)
- SpellScrollPickup + SpellUnlockSystem (code ready, NOT in scene)
- SpellHUDController v1.1 (dynamic HUD, cooldown overlay, range indicator)
- ShieldComponent v1.1 (duration from TuneConfig, blocks next attack)
- Cave Map (Caves Parts Set + Dwarven Pack + ProBuilder/Polybrush)
- Canvas UI: HealthBarUI v3.1 + TuneSliderUI v2.1
- Cinemachine v3.x (CM_PlayerCamera, CinemachineBrain)
- Win Condition (ExitTrigger, ResetTrigger on Retry)
- Full Game Loop (GameManager v1.3, MainMenuController, ResultScreenController)
- Backend REST API (Node.js + Express + SQLite) — localhost:3000
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
