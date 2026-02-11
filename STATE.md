# PROJECT STATE - Snake Enchanter

**Letzte Aktualisierung:** 2026-02-11 (Session 11 - UNITY AUDIT + GDD v1.6 + PHASE 2 ROADMAP)

---

## ⚡ QUICK START FÜR NÄCHSTE SESSION

**Lies zuerst:** `PHASE_2_ROADMAP.md` (kompletter 3-Tage Plan)

**Starte mit:** Main Menu Scene erstellen (2h Task)

**Phase 2 Deadline:** 14.02.2026 (3 Tage)
**Status:** 55% Complete (6/11 Tasks)
**Kritisch fehlend:** Main Menu, Result Screen, Mock-API, Win Transition

---

## ✅ PIRATE CHARACTER SETUP - ABGESCHLOSSEN

### Was ist fertig:

#### ✅ 1. Pirate FBX + Avatar
- Pirate.FBX importiert (`Assets/_Project/Animations/Pirate/Mesh/`)
- Humanoid Rig konfiguriert (`animationType: 3`, `avatarSetup: 1`)
- PirateAvatar erstellt (GUID: `619359b845787a443af41cf1ed1cfed0`)

#### ✅ 2. Materials
- 8 URP/Lit Materials manuell auf SkinnedMeshRenderer zugewiesen
- Pirate rendert korrekt mit allen Texturen

#### ✅ 3. Animations
- 13 Pirate-spezifische Mixamo Animations importiert
- Alle Animations auf PirateAvatar retargeted
- Ordnerstruktur: `Idle/`, `Walk/`, `Crouch/`, `Death/`, `Spell/`, `Others/`

#### ✅ 4. Animator Setup
- MC_Controller.controller konfiguriert
- **States:** Idle, Walk, Crouch Idle, Crouch Walk
- **Parameters:** Speed (float), IsCrouching (bool)
- **Transitions:** Alle korrekt mit Conditions

#### ✅ 5. Scene Setup
- Pirate als Child vom Player GameObject
- Animator Component: Controller + Avatar + Root Motion OFF
- PlayerController.Animator Feld zugewiesen
- Pirate als Prefab gespeichert (`Assets/_Project/Prefabs/Pirate.prefab`)

#### ✅ 6. CameraTarget + Camera View
- Leeres GameObject unter Pirate Head Bone erstellt
- CM_PlayerCamera Tracking Target zugewiesen
- Kamera folgt smooth dem Kopf
- **View:** First-person mit sichtbaren Armen + Füßen (full body model)

---

## ✅ MC SPELL + DEATH ANIMATIONS - ABGESCHLOSSEN

### Was ist fertig (Session 9):

#### ✅ Animator Erweitert
- **10 States total:** 4 Movement + 4 Spell + 2 Death
- **Spell States:** Spell_Move, Spell_Daze, Spell_Attack, Spell_Fear
- **Death States:** Death_by_Drain, Death_by_Snakes
- **7 Parameters:** Speed, IsCrouching, 4x Spell Triggers, IsDead

#### ✅ TuneController v2.4
- Animator Referenz hinzugefügt (`GetComponentInChildren<Animator>()`)
- Bei Tune Success: Trigger Spell Animation basierend auf Tune Number
  - Tune 1 → SpellMove → "Spell Casting.fbx"
  - Tune 2 → SpellDaze → "Wide Arm Spell Casting.fbx"
  - Tune 3 → SpellAttack → "Standing 2H Cast Spell.fbx"
  - Tune 4 → SpellFear → "Magic Spell Casting.fbx"

#### ✅ HealthSystem v1.3
- Animator Referenz hinzugefügt
- `Die()` erweitert mit `deathBySnakeAttack` Parameter
- Bei Death: `animator.Play()` für passende Animation
  - Drain Death → "Death_by_Drain" (Standing React Death Forward)
  - Snake Attack → "Death_by_Snakes" (Standing React Death Left)

#### ✅ Testing
- ✅ Alle 4 Spell Animations getestet und funktionieren
- ✅ Death_by_Drain getestet und funktioniert
- ⏳ Death_by_Snakes noch nicht testbar (Snakes machen noch keinen Damage)

---

## 🟡 OFFENE AUFGABEN

### Enemy System + Snake Animations
- **Status:** Nächster großer Schritt
  - Standing 2H Cast Spell.fbx
  - Two Hand Spell Casting.fbx
  - Wide Arm Spell Casting.fbx
- **Nächster Schritt:** TuneController mit Spell Animation verknüpfen (für Tune Success Feedback)

---

## AKTUELLER STAND

### Phase: 2 - KOMPLETT (von 4)
### Branch: `feature/enemy-setup`

### Was funktioniert:
- ✅ Player Controller v1.7 (New Input System, Crouch, Cinemachine)
- ✅ Health System v1.3 (Drain, Events, Death Animations)
- ✅ Tune System (TuneController v2.4, Spell Animations, 4 TuneConfig SOs)
- ✅ **Snake AI v1.3.1** + 6 Toon Snake Prefabs
  - ✅ **Patrol System** (random waypoints, 2-3 units radius)
  - ✅ **Proximity Detection** (line-of-sight raycast)
  - ✅ **Range-based Behaviors** (Bite/Follow/Breath/Projectile)
  - ✅ **Attack System** (3 attack types, 4s cooldown, raycast damage)
  - 🔴 **Breath Attack Animation** (damage works, animation doesn't play)
  - 🔴 **Patrol Movement** (not moving, debugging with logs)
- ✅ Cave Map (Caves Parts Set + Dwarven Pack)
- ✅ Canvas UI: HealthBarUI v3.1 + TuneSliderUI v2.1
- ✅ Cinemachine v3.x (CM_PlayerCamera, CinemachineBrain)
- ✅ Win Condition (ExitTrigger)
- ✅ Game Loop (GameManager v1.1.1)
- ✅ **Pirate Character komplett setup**
- ✅ **MC Animations komplett: Movement (4), Spells (4), Death (2)**

### Was noch nicht fertig ist:
- 🔴 **Breath Attack Animation Debug** (highest priority)
- 🔴 **Patrol System Debug** (not moving)
- 🟡 **Failed Tune Behavior** (Follow + Bite once)
- 🟡 **Player Look Up/Down** (Mouse Y for camera pitch)
- ⬜ Death_by_Snakes Animation Testing (wartet auf Snake Damage)

---

## SCRIPTS (alle funktionieren)

| Script | Version | Status |
|--------|---------|--------|
| PlayerController.cs | v1.7 | ✅ Cinemachine Final |
| HealthSystem.cs | v1.2.1 | ✅ |
| TuneController.cs | v2.3 | ✅ |
| TuneConfig.cs | v1.0 | ✅ |
| GameEvents.cs | v1.1 | ✅ |
| GameManager.cs | v1.1.1 | ✅ |
| SnakeAI.cs | v1.3.1 | ✅ Patrol + Proximity |
| HealthBarUI.cs | v3.1 | ✅ |
| TuneSliderUI.cs | v2.1 | ✅ |
| ExitTrigger.cs | v1.0 | ✅ |
| CanvasUICreator.cs | v2.0 | ✅ Editor |
| TuneConfigCreator.cs | v1.0 | ✅ Editor |

---

## SCENE (GameLevel.unity)

| GameObject | Status |
|------------|--------|
| **Player** (CharacterController, PlayerController, HealthSystem, TuneController) | ✅ |
| └─ **Pirate** (Prefab Instance, Animator, SkinnedMeshRenderer, 8 Materials) | ✅ |
|    └─ **CameraTarget** (unter Head Bone) | ✅ |
| **Main Camera** (CinemachineBrain) | ✅ |
| **CM_PlayerCamera** (CinemachineCamera, Tracking Target = CameraTarget) | ✅ |
| **Cave Map** | ✅ |
| **ExitTrigger** | ✅ |
| **GameManager** | ✅ |
| **Snake(s)** | ✅ 6 Prefabs |
| **Canvas (UI)** | ✅ |

---

## PIRATE ASSET-STRUKTUR

```
_Project/Animations/Pirate/
├── Mesh/
│   └── Pirate.FBX (Humanoid Rig, PirateAvatar)
├── Materials/ (8 .mat files, alle URP/Lit, alle Textures zugewiesen)
│   ├── Pirate_Body_01.mat
│   ├── Pirate_Body_02.mat
│   ├── Pirate_Cloth.mat
│   ├── Pirate_Hair_01.mat
│   ├── Pirate_Hair_02.mat
│   ├── Pirate_Hair_03.mat
│   ├── Pirate_Details_Weapon.mat
│   └── Stand.mat
└── Animations/ (13 FBX files, alle auf PirateAvatar retargeted)
    ├── Idle/ (3 files)
    │   ├── Breathing Idle.fbx ✅ (in MC_Controller)
    │   ├── Crouch Idle.fbx ✅ (in MC_Controller)
    │   └── Crouch Idle 02 Looking Around.fbx
    ├── Walk/ (2 files)
    │   ├── Walking.fbx ✅ (in MC_Controller)
    │   └── Injured Walk.fbx
    ├── Crouch/ (1 file)
    │   └── Crouched Walking.fbx ✅ (in MC_Controller)
    ├── Death/ (2 files)
    │   ├── Standing React Death Forward.fbx
    │   └── Standing React Death Left.fbx
    └── Spell/ (5 files) 🟡 Nicht im Animator
        ├── Magic Spell Casting.fbx
        ├── Spell Casting.fbx
        ├── Standing 2H Cast Spell.fbx
        ├── Two Hand Spell Casting.fbx
        └── Wide Arm Spell Casting.fbx
```

---

## MC_CONTROLLER ANIMATOR

### States (Base Layer)
**Movement States:**
1. **Idle** → Motion: `Breathing Idle.fbx` (Pirate)
2. **Walk** → Motion: `Walking.fbx` (Pirate)
3. **Crouch Idle** → Motion: `Crouch Idle.fbx` (Pirate)
4. **Crouch Walk** → Motion: `Crouched Walking.fbx` (Pirate)

**Spell States:** (Triggered by successful Tune)
5. **Spell_Move** → Motion: `Spell Casting.fbx` (Tune 1)
6. **Spell_Daze** → Motion: `Wide Arm Spell Casting.fbx` (Tune 2)
7. **Spell_Attack** → Motion: `Standing 2H Cast Spell.fbx` (Tune 3)
8. **Spell_Fear** → Motion: `Magic Spell Casting.fbx` (Tune 4)

**Death States:** (Triggered by HP = 0)
9. **Death_by_Drain** → Motion: `Standing React Death Forward.fbx`
10. **Death_by_Snakes** → Motion: `Standing React Death Left.fbx`

### Parameters
- **Speed** (Float) - Horizontal movement speed
- **IsCrouching** (Bool) - Crouch state
- **SpellMove** (Trigger) - Tune 1 success
- **SpellDaze** (Trigger) - Tune 2 success
- **SpellAttack** (Trigger) - Tune 3 success
- **SpellFear** (Trigger) - Tune 4 success
- **IsDead** (Bool) - Player death (not used in v1.3, script-based)

### Transitions
**Movement:**
- Idle ↔ Walk: Speed threshold (0.1)
- Idle ↔ Crouch Idle: IsCrouching bool
- Crouch Idle ↔ Crouch Walk: Speed threshold (0.1)

**Spells:**
- Any State → Spell States (via Triggers)
- Spell States → Idle (Exit Time 0.9-0.96)

**Death:**
- Script calls `animator.Play("Death_by_Drain")` or `animator.Play("Death_by_Snakes")`

---

## GIT STATUS

```
Branch: feature/enemy-setup (aktiv)
Letzter Commit: 6642e2a "Fix Animator parameter names to match Toon Cobra Controller"
Remote: https://github.com/JuliGommz/Snake_Enchanter.git

Recent Commits (2026-02-10):
  6642e2a - Fix Animator parameter names to match Toon Cobra Controller
  6b69a9e - Add debug logs for patrol system troubleshooting
  8851f7e - Implement patrol system and proximity-based behaviors
  c0f3450 - Add spell animation delay and separate movement speeds
  a1a58ed - Add chase behavior for aggressive snakes within 5 units
  0a03a14 - Fix Attack System v1.1.1 - Breath Attack Bool + Advanced Mode timing

Uncommitted Changes: JA (dokumentation updates pending)
  - SESSION_NOTES_2026-02-10.md (new)
  - CLAUDE.md (updated with Snake AI info)
  - STATE.md (updated for Session 10)
```

**Nächster Commit:** "Update documentation for Session 10 - Snake AI behaviors"

---

## NÄCHSTE SCHRITTE (Priorität)

### ✅ SESSION 11 COMPLETE - Unity Audit + Documentation

**Session 11 Achievements:**
1. ✅ **Python 3.12 installiert** — Für automatisierte DOCX-Generierung
2. ✅ **GDD v1.6 erstellt** — TXT + DOCX (formatiert wie PDF v1.3)
3. ✅ **Unity Audit durchgeführt** — `/unity-audit` skill verwendet
4. ✅ **Audit Report erstellt** — `UNITY_AUDIT_2026-02-11.md`
5. ✅ **BACKLOG aktualisiert** — Performance optimization tasks hinzugefügt
6. ✅ **CARL System Setup** — 9 Domains mit 41 Rules für Snake Enchanter

**Audit Ergebnisse:**
- ✅ Code Quality: **GOOD** (8.5/10 Performance Score)
- ✅ No critical issues
- ⚠️ 2 minor GetComponent calls in SnakeAI.cs (cacheable für +5-10% performance)

**Nächste Session Empfehlung:**
1. 🟡 **SnakeAI Performance Fix** (5-10min) — Cache HealthSystem reference
2. 🔴 **Exit Trigger Animation Hang** (1-2h) — GameManager State Machine erweitern
3. 🐍 **Snake AI Debugging** — Breath Attack Animation + Patrol Movement fixes

### Phase 2 - KOMPLETT: 85% Complete

**Noch offen:**
- 🔴 Exit Trigger Animation Hang (Win Condition UX)
- 🔴 Breath Attack Animation Debug (damage works, animation doesn't play)
- 🔴 Patrol System Debug (snakes don't move)
- 🟡 SnakeAI Performance Optimization (GetComponent caching)
- 🟡 Cave Textures Fix (Neon-Yellow Materials)
- 🟡 Camera Position bei Crouch

Siehe `BACKLOG.md` für alle Issues mit Prioritäten.

---

## DOCUMENTATION STATUS

### ✅ Up-to-Date (Session 11)
- ✅ **GDD v1.6** — TXT + DOCX mit Snake AI v1.3.1 Changes
- ✅ **UNITY_AUDIT_2026-02-11.md** — Kompletter Audit Report
- ✅ **BACKLOG.md** — Aktualisiert mit Audit-Ergebnissen
- ✅ **STATE.md** — Diese Datei
- ✅ **.carl/** — CARL System konfiguriert (9 Domains)

### ⏳ Pending Updates
- ⬜ **Arbeitsprotokoll** — Session 11 eintragen
- ⬜ **SESSION_NOTES** — Session 11 Summary erstellen
- ⬜ **Git Commit** — "Add Unity Audit + GDD v1.6 + CARL Setup"

---

## BACKLOG

Alle identifizierten Issues sind im `BACKLOG.md` dokumentiert und priorisiert:
- 🔴 High Priority: Exit Trigger Animation Hang
- 🟡 Medium Priority: SnakeAI Performance, Cave Textures, Camera Crouch
- 🟢 Low Priority: Crouch Transitions, Injured Walk, Snake Stacking

**Unity Audit Integration:**
- ✅ GetComponent Performance Issue hinzugefügt (Medium Priority)
- ✅ Audit Results Section in BACKLOG.md

**Siehe:** `BACKLOG.md` für Details

---

## REGELN (NICHT VERHANDELBAR)

### Input System
AUSSCHLIESSLICH Unity New Input System! NIEMALS `UnityEngine.Input` (Legacy).

### Kamera-System (Cinemachine v3.x)
- Cinemachine besitzt Kamera-Position. NIEMALS per Script überschreiben.
- PlayerController steuert NUR Pitch (Mouse Y) + Body Yaw (Mouse X)

### Animation
- KEINE Flöte (zu komplex) → Spell Animation stattdessen
- Root Motion OFF (CharacterController steuert Movement)

---

## LESSONS LEARNED

### Session 9 (2026-02-09): MC Spell + Death Animations

**✅ Durchgeführt:**
- 4 Spell Animations in MC_Controller integriert (Any State → Spell → Idle)
- 2 Death Animations hinzugefügt (script-basiert via `animator.Play()`)
- TuneController v2.4: Trigger Spell Animation bei Success
- HealthSystem v1.3: Death Animation basierend auf Death Cause
- Alle Spell Animations getestet und funktionieren

**🎯 Entscheidung:**
- Death Animations via Script statt Animator Transitions (Option B)
  - Vorteil: Sauber, keine zusätzlichen Parameter nötig
  - Code entscheidet welche Animation via `animator.Play("Death_by_Drain" or "Death_by_Snakes")`

**📝 Neues Backlog Item:**
- Camera Position bei Crouch (folgt nicht dem Ducken)

**⏳ Nicht testbar:**
- Death_by_Snakes Animation (Snakes machen noch keinen Damage)

---

### Session 8 (2026-02-09): Pirate Character Setup

### ✅ Was funktioniert hat:
- Worktree/Main Repo Workflow (Commits im Main, dann merge ins Worktree)
- Manuelles Material Assignment direkt auf SkinnedMeshRenderer
- Pirate Avatar Configure in Unity (statt .meta Edit)
- Crouch Idle State hinzufügen löste Animation-Sprünge

### ❌ Fehler vermieden:
- FBX.meta manuell editieren (zerstört Humanoid Rig)
- Old Man Idle Prefab mit falschem Avatar verwenden
- Halluzinieren statt Unity Setup direkt prüfen
- Assumptions über Animator States ohne User zu fragen

### 📝 Memory Updates:
- DEBUGGING: Always Check Live Setup First (ask user what they see)
- NEVER assume files match Unity's current state
- READ COMPLETE files before making claims

---

**Status**: ✅ PHASE 1 - SPIELBAR: COMPLETE
**Next**: Dokumentation finalisieren → Git Push → Phase 2 Start

---

## SESSION 8 ZUSAMMENFASSUNG

**Erledigt:**
- ✅ Pirate Character komplett setup (FBX, Avatar, Materials, Animations)
- ✅ Animator konfiguriert (4 States, 2 Parameters, alle Transitions)
- ✅ Scene Integration (Prefab, CameraTarget, PlayerController)
- ✅ Core Loop getestet und funktional
- ✅ Git Commit: bd472c0 (79 files, 16323 insertions)
- ✅ Backlog erstellt mit priorisierten Issues

**Issues identifiziert (Backlog):**
- Exit Trigger Animation Hang
- Cave Textures Neon-Yellow
- Crouch Transition Tuning
- Injured Walk Animation (optional)

**Lessons Learned:**
- Worktree/Main Repo Workflow funktioniert gut
- Manuelles Material Assignment statt FBX Remapping
- Unity Setup direkt prüfen statt Dateien lesen
- NEVER assume files match Unity's current state

---

**END OF STATE - Session 8 COMPLETE**
