# PROJECT STATE - Snake Enchanter

**Letzte Aktualisierung:** 2026-02-09 (Session 8)

---

## 🔴 DEIN AUFTRAG FÜR DIESE SESSION

### Was ist das Problem?
Die Pirate-FBX (`Assets/_Project/Animations/Pirate/Mesh/Pirate.FBX`) hat `materialImportMode: 2` (embedded) und `externalObjects: {}`. Das heißt: Unity erzeugt eigene interne Materials beim Import und ignoriert unsere 8 externen `.mat` Dateien in `Assets/_Project/Animations/Pirate/Materials/`. Der Pirate wird deshalb mit falschen/grauen Materials gerendert.

### Was ist bereits fertig?
- ✅ FBX importiert, Humanoid Rig konfiguriert (`avatarSetup: 1`, `animationType: 3`)
- ✅ 8 `.mat` Dateien im Ordner `Materials/` — alle URP/Lit Shader, alle Texture-GUIDs korrekt
- ✅ 15 Texturen im Ordner `Textures/` importiert
- ✅ 14 Mixamo-Animation FBX Dateien im Ordner `Animations/`

### Was musst du tun?
Die FBX muss die 8 externen `.mat` Dateien statt der embedded Materials nutzen. Entweder über Unity FBX Import Settings (Materials Tab → Search and Remap) oder durch manuelles Zuweisen auf dem SkinnedMeshRenderer in der Scene.

### Was darfst du NICHT tun?
- **NIEMALS `Pirate.FBX.meta` per Text-Editor editieren** — das hat in Session 7 den Humanoid Rig zerstört
- **NICHT nochmal versuchen** was schon gescheitert ist (6 Versuche dokumentiert unten)
- **NICHT nach Kontext fragen** — alles steht in dieser Datei

---

## AKTUELLER STAND

### Pirate Character Setup — 3 Probleme, 1 gelöst, 2 offen

Die FBX (`_Project/Animations/Pirate/Mesh/Pirate.FBX`) ist importiert mit Humanoid Rig. 8 Material-Dateien existieren im Ordner `Materials/`. 15 Texturen in `Textures/`. 14 Mixamo-Animations in `Animations/`. Pirate ist **NOCH NICHT in der Scene** — erst nach Material-Fix.

#### ~~Problem 1: FBX Rig~~ ✅ GELÖST
- `avatarSetup: 1`, `animationType: 3` — Humanoid Rig ist gesetzt
- FBX GUID: `acd21bb244ba21b4cb8435a26823d8d0`

#### Problem 2: Materials werden nicht vom FBX genutzt
- 8 externe `.mat` Dateien existieren und sind **korrekt** (URP/Lit Shader, richtige Texture-GUIDs)
- FBX ignoriert sie: `externalObjects: {}`, `materialImportMode: 2` (embedded)
- **Muss gelöst werden** — entweder über FBX Import Settings oder manuell auf SkinnedMeshRenderer

**Was NICHT funktioniert hat (6 Versuche in Session 7):**
1. Texture GUIDs in .mat fixen → ❌ FBX nutzte embedded Materials
2. FBX.meta externalObjects manuell editieren → ❌ **Zerstört Humanoid Rig!** NIEMALS machen
3. Frischer Reimport → Rig + Materials Reset (aktueller Stand)
4. URP Conversion via Unity Menu → ❌ Shader blieb Standard
5. .mat Dateien komplett neu geschrieben → ✅ Materials jetzt korrekt
6. materialImportMode auf Legacy → ❌ Search and Remap fand nichts (materialSearch war "Local" statt "Recursive-Up" oder "Project Wide")

**Empfohlene Lösungsansätze:**
- **A:** FBX Inspector → Materials → Search: "Project Wide" oder "Recursive-Up" → "Search and Remap" → Apply
- **B:** Pirate in Scene draggen → SkinnedMeshRenderer → Materials manuell per Drag&Drop zuweisen (nur Instance, nicht FBX-weit)

#### Problem 3: Animations referenzieren alten Avatar
- 14 Mixamo-Animations haben `avatarSetup: 2` (Copy From Other Avatar)
- Referenzieren GUID `e885ce14dfad3a642bd300e6c2cfe68f` — das ist der **alte** Pirate Avatar (vor Reimport)
- **Müssen nach Rig-Setup (Problem 1) auf den NEUEN PirateAvatar umgestellt werden**

#### Was FUNKTIONIERT:
- ✅ 8 `.mat` Dateien: URP/Lit Shader (`933532a4fcc9baf4fa0491de14d08ed7`), korrekte Texture-GUIDs
- ✅ 15 Texturen importiert in `Pirate/Textures/`
- ✅ 14 Mixamo Animation FBX Dateien vorhanden

---

## NÄCHSTE SCHRITTE (in Reihenfolge)

1. ✅ **Pirate.FBX Rig → Humanoid** — erledigt (`avatarSetup: 1`)
2. 🔴 **Material Remapping** lösen (Search and Remap mit "Project Wide", oder manuell)
3. 🔴 **14 Animations** auf neuen PirateAvatar umstellen (Copy From → neuer Avatar)
4. ⬜ **MC_Controller Motions** ersetzen — aktuell referenziert alte Cowboy-Clips:
   - Idle → `Old Man Idle.fbx` (`df1d5f44737c766479c0d441f4970acf`) → **Breathing Idle.fbx** (`8da9643668d27504a8573470828cfa46`)
   - Walk → `Orc Walk.fbx` (`21d25341ad143a942b5981ca014d0cee`) → **Walking.fbx** (`97f286d10c335e74eaf08b4278baae1b`)
   - Crouch Idle → `Crouch Idle.fbx` (`43d77b93cf99fab4d97b3cea8358eabe`) → **Crouch Idle.fbx** (Pirate)
   - Crouch Walk → `Crouch Walk Forward.fbx` (`646013ad6e5f857459594adfeaf02225`) → **Crouched Walking.fbx** (Pirate)
5. ⬜ **Pirate in Scene** — als Player Child, Animator (MC_Controller, PirateAvatar, Root Motion OFF)
6. ⬜ **CameraTarget** — leeres GameObject unter Pirate Head Bone → CM_PlayerCamera Tracking Target
7. ⬜ **Cowboy Cleanup** — MC_Mixamo/ Ordner + alte Cowboy FBX entfernen
8. ⬜ **Play-Test Core Loop**

---

## PROJEKT-ÜBERBLICK

### Phase: 1 - SPIELBAR (von 4)
### Branch: `feature/animations-complete`

### Was fertig ist:
- ✅ Player Controller v1.8 (New Input System, Crouch, Cinemachine Pitch-only)
- ✅ Health System v1.2.1 (Drain, Events, Namespace-Fix)
- ✅ Tune System (TuneController v2.3, 4 TuneConfig SOs)
- ✅ Snake AI v1.1 + 6 Toon Snake Prefabs in Scene
- ✅ Cave Map (Caves Parts Set + Dwarven Pack)
- ✅ Canvas UI: HealthBarUI v3.1 + TuneSliderUI v2.1 (Steampunk Theme)
- ✅ Cinemachine v3.x (CM_PlayerCamera, CinemachineBrain)
- ✅ Win Condition (ExitTrigger)
- ✅ Game Loop (GameManager v1.1.1)

### Was NICHT fertig ist:
- ❌ Pirate Character Setup (Material + Rig + Animations — siehe oben!)
- ❌ Pirate noch nicht in Scene
- ❌ MC_Controller referenziert noch Cowboy-Clips
- ❌ CameraTarget noch nicht erstellt
- ❌ Play-Test Core Loop

### Scripts (alle funktionieren):
| Script | Version | Status |
|--------|---------|--------|
| PlayerController.cs | v1.8 | ✅ |
| HealthSystem.cs | v1.2.1 | ✅ |
| TuneController.cs | v2.3 | ✅ |
| TuneConfig.cs | v1.0 | ✅ |
| GameEvents.cs | v1.1 | ✅ |
| GameManager.cs | v1.1.1 | ✅ |
| SnakeAI.cs | v1.1 | ✅ |
| HealthBarUI.cs | v3.1 | ✅ |
| TuneSliderUI.cs | v2.1 | ✅ |
| ExitTrigger.cs | v1.0 | ✅ |
| CanvasUICreator.cs | v2.0 | ✅ Editor |
| TuneConfigCreator.cs | v1.0 | ✅ Editor |

### Scene (GameLevel.unity):
| GameObject | Status |
|------------|--------|
| Player (CharacterController, PlayerController, HealthSystem, TuneController) | ✅ |
| Main Camera (CinemachineBrain) | ✅ |
| CM_PlayerCamera (CinemachineCamera) | ⏳ Target muss zugewiesen werden |
| Cave Map | ✅ |
| ExitTrigger | ✅ |
| GameManager | ✅ |
| Snake(s) | ✅ |
| Canvas (UI) | ✅ |
| **Pirate (Player Child)** | ❌ **Noch nicht in Scene** |
| **CameraTarget (unter Head Bone)** | ❌ **Noch nicht erstellt** |

---

## PIRATE ASSET-STRUKTUR

```
_Project/Animations/Pirate/
├── Mesh/Pirate.FBX          ← Rig: Humanoid ✅ (avatarSetup: 1)
├── Materials/                ← 8 .mat Dateien (URP/Lit ✅, Textur-GUIDs ✅)
│   ├── Pirate_Body_01.mat   (guid: 566f2752e6db9b9469b563c6ceeef514)
│   ├── Pirate_Body_02.mat   (guid: e9d4cce31875e084eb2eb72a25ce0ad2)
│   ├── Pirate_Cloth.mat     (guid: 1f3da8825332d264699bc01860394e8f)
│   ├── Pirate_Hair_01.mat   (guid: d7b3562339e6a034a95a0a81d53ece9b)
│   ├── Pirate_Hair_02.mat   (guid: f71f285f86caee547ad5d4269ef36080)
│   ├── Pirate_Hair_03.mat   (guid: a7291cf1865f4654d8753bb178e8c7e1)
│   ├── Pirate_Details_Weapon.mat (guid: a372295a356185a44a49e36afb662e19)
│   └── Stand.mat            (guid: 13651f48f0f43864192d0edcfff21268)
├── Textures/                 ← 15 .tga Dateien (Albedo, Normals, Metallic, AO)
│   ├── Pirate_Body/          (5 Texturen)
│   ├── Pirate_Cloth/         (4 Texturen)
│   └── Pirate_Hair/          (6 Texturen)
└── Animations/               ← 14 Mixamo FBX (avatarSetup: 2, alter Avatar!)
    ├── Idle/Breathing Idle.fbx
    ├── Walk/Walking.fbx, Injured Walk.fbx
    ├── Idle/Crouch Idle.fbx, Crouch Idle_1.fbx, Crouch Idle 02 Looking Around.fbx
    ├── Crouch/Crouched Walking.fbx
    ├── Spell/Magic Spell Casting.fbx + 4 weitere
    └── Death/Standing React Death Forward.fbx + 1 weitere
```

---

## GIT STATUS

```
Branch: feature/animations-complete (aktiv)
Letzter Commit: b47d810 Code review cleanup
Remote: https://github.com/JuliGommz/Snake_Enchanter.git
Uncommitted Changes: JA (viele — Pirate Assets, Material-Edits, gelöschte Cowboy-Dateien)
```

---

## OFFENE NEBENPROBLEME

### Snake MoveAwayTarget
- Beide Snakes laufen zum gleichen Punkt (stacken sich)
- Jede Snake braucht individuelles MoveAwayTarget
- Niedrige Priorität — kann für Phase 1 deaktiviert werden

---

## REGELN (NICHT VERHANDELBAR)

### Input System
AUSSCHLIESSLICH Unity New Input System! NIEMALS `UnityEngine.Input` (Legacy).

### Kamera-System (Cinemachine v3.x)
- Cinemachine besitzt Kamera-Position. NIEMALS per Script überschreiben.
- PlayerController steuert NUR Pitch (Mouse Y) + Body Yaw (Mouse X)
- CameraHeadTracker.cs wurde GELÖSCHT — war redundant

### Animation
- KEINE Flöte (zu komplex) → Spell Animation stattdessen

### Lessons Learned (Session 7)
- ❌ **NIEMALS FBX.meta manuell editieren** — Unity überschreibt Humanoid Rig-Daten
- ✅ `.mat` Dateien per Text-Editor schreiben ist sicher
- ❌ `materialSearch: 1` (Local) findet Materials in Unterordnern nicht → "Project Wide" verwenden
