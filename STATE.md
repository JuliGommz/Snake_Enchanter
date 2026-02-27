# PROJECT STATE - Snake Enchanter

**Letzte Aktualisierung:** 2026-02-27

---

## QUICK START

**Branch:** `main` (feature/spell-editor-setup gemergt + gelöscht)
**Letzter Commit:** (nach Merge — siehe `git log --oneline -1`)
**Working Tree:** Clean

**Milestone:** v1.0 Submission Ready (Phases 7-13)
**Aktuelle Phase:** 7 - Spell System
**Fortschritt:** ~40% von v1.0

---

## WAS GERADE PASSIERT

feature/spell-editor-setup vollständig abgeschlossen und in main gemergt.

### Was Session 22+23 gemacht hat:
1. **cave-rebuild** in main gemergt + branch gelöscht
2. **feature/spell-editor-setup** neu von main erstellt (clean branch)
3. **Asset Recovery** (root cause: `.gitignore: *.fbx`):
   - Cave FBX: 7 binaries → `Cave/FBX/` (aus External_Assets/Caves Parts Set)
   - Snake FBX: 16 binaries → `Snakes/FBX/` (aus External_Assets/Toon Snakes Pack)
   - Pirate.FBX → `Pirate/Mesh/` (GUID: `619359b845787a443af41cf1ed1cfed0` ✓)
   - Pirate Animations: alle 10 .meta aus git HEAD restauriert, 10/10 FBX vorhanden
   - Cave Materials: von Standard-Shader auf URP/Lit (933532a4) zurückgesetzt
4. **MC_Controller Cleanup:**
   - `Spell_Fear` → `Spell_Shield` (Animation: Magic Spell Casting)
   - `Spell_Attack` → **gelöscht**
   - Parameter `SpellFear` → `SpellShield`, `SpellAttack` → **gelöscht**
   - Parameter `IsDead` → `IsDazed`
   - Alle 9 States haben Animationen zugewiesen
5. Alles committed + gepusht + in main gemergt

### NÄCHSTER SCHRITT:
- [ ] **Unity Play-Test** — 14-Punkt Checklist (Move/Daze/Shield Spells)
- [ ] **ShieldComponent._borderGlowImage** = NULL → Inspector Fix
- [ ] **CooldownOverlay FillOrigin** = Bottom → Top (Unity Inspector)
- [ ] **"SpellInfos" UI-Panels** aus Scene entfernen (orphaned)
- [ ] **3D Scrolls** in cave platzieren
- [ ] **SnakeAI-Cleanup** (Dead state, SnakeType enum)

### Bekannte Issues (aus Session 21):
- SpellScrollPickup + SpellUnlockSystem NICHT in Scene (`_unlockAllOnStart = true` überbrückt)
- ShieldComponent._borderGlowImage = NULL (Inspector Fix nötig)
- CooldownOverlay FillOrigin = Bottom → Top (Unity Inspector)
- "SpellInfos" orphaned UI-Panels aus Scene entfernen
- 3D Scrolls noch nicht platziert
- SnakeAI-Cleanup (Dead state, SnakeType enum) noch pending

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
| 7 | Spell System (3 Tunes: Move, Daze, Shield) | ~90% (testing pending) |
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
- Tune System (TuneController v3.2, 3-Tune Array, Debug Unlock, Cooldown, Charges)
- Snake AI v2.0 (NavMesh, Entranced→Dazed two-phase, CancelInvoke safety)
- SpellScrollPickup + SpellUnlockSystem (code ready, NOT in scene)
- SpellHUDController v1.1 (dynamic HUD, cooldown overlay, range indicator)
- ShieldComponent v1.1 (duration from TuneConfig, blocks next attack)
- Cave Map (Caves Parts Set + Dwarven Pack + ProBuilder/Polybrush)
- Canvas UI: HealthBarUI v3.1 + TuneSliderUI v2.1
- Cinemachine v3.x (CM_PlayerCamera, CinemachineBrain)
- Win Condition (ExitTrigger)
- Game Loop (GameManager)
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
Branch: main
Letzter Commit: (Merge von feature/spell-editor-setup)
Remote: https://github.com/JuliGommz/Snake_Enchanter.git
Working Tree: Clean
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
| `Scripts/Snakes/SnakeAI.cs` | v2.0 — Entranced State, CancelInvoke |
| `Scripts/Player/ShieldComponent.cs` | v1.1 — Duration Parameter |
| `Scripts/TuneSystem/TuneConfig.cs` | ScriptableObject Definition |
| `ScriptableObjects/TuneConfigs/Tune1_Move.asset` | Move Config (melody 0-12s) |
| `ScriptableObjects/TuneConfigs/Tune2_Daze.asset` | Daze Config |
| `ScriptableObjects/TuneConfigs/Tune3_Shield.asset` | Shield Config (melody 10-25s = 15s shield) |
