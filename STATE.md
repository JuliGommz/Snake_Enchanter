# PROJECT STATE - Snake Enchanter
> **WICHTIG FÜR NEUE SESSIONS:** Diese Datei enthält den aktuellen Projektstand.
> Lies diese Datei ZUERST bevor du mit der Arbeit beginnst.

**Letzte Aktualisierung:** 2026-02-07 (Session 6)
**Letzte Session:** UI Polish (v2.1/v3.1), Cinemachine Integration, Setup Review, Doku-Update

---

## AKTUELLER STAND

### Phase: 1 - SPIELBAR (von 4)
### Status: UI Polish fertig (v2.1/v3.1), Cinemachine integriert, Animationen BROKEN nach Avatar-Wechsel

### Fortschritt Phase 1:
- [x] 1.1 Unity Projekt Setup
- [x] 1.2 Git/GitHub Setup
- [x] 1.3 Dokumentation & Struktur
- [x] 1.4 Player Controller (First-Person, New Input System, Crouch) ✅
- [x] 1.5 Greybox Level Setup ✅
- [x] 1.6 Tune Input (ADR-008 Slider, New Input System) ✅
- [x] 1.7 Timing Window (Triggerzone Evaluation) ✅
- [x] 1.8 Health System (HP, Drain, Damage) ✅
- [x] 1.9 Win Condition (ExitTrigger in Scene) ✅
- [x] 1.10 Cave Map aufbauen (Caves Parts Set + Dwarven Pack) ✅
- [x] 1.11 Player Sprite einbinden (statisch, First-Person) ✅
- [x] 1.12 Toon Snakes Pack importieren ✅ 6 Prefabs (Cobra/Snake x 3 Farben)
- [x] 1.13 Snake AI (Basic) ✅ Script + Unity-Integration fertig
- [x] 1.14 TuneConfig ScriptableObjects anlegen ✅ 4 SOs erstellt (Move/Sleep/Attack/Freeze)
- [x] 1.15 Player/Animation Setup debuggen ✅ GELÖST (heightFromFeet, Animator auf Child, Old Man Idle)
- [x] 1.16 UI: HealthBar + TuneSlider ✅ v3.1/v2.1 fertig + in Unity getestet + Steampunk Theme
- [x] 1.17 Cinemachine Integration ✅ CM_PlayerCamera + CameraHeadTracker
- [ ] 1.18 Animationen reparieren (broken nach Avatar/Cinemachine-Umbau)
- [ ] 1.19 Play-Test Core Loop

### Scripts Status:
| Script | Version | Namespace | Status |
|--------|---------|-----------|--------|
| PlayerController.cs | **v1.7** | SnakeEnchanter.Player | ✅ Cinemachine Final, Pitch-only, Auto Camera.main |
| CameraHeadTracker.cs | **v1.0** | SnakeEnchanter.Player | ✅ NEU — Position-only Head Bone Tracking |
| HealthSystem.cs | **v1.2.1** | SnakeEnchanter.Player | ✅ Drain 0.1 HP/sec, deaktiviert für Dev |
| TuneController.cs | **v2.3** | SnakeEnchanter.Tunes | ✅ B-001 Lambda-Leak fix + proper unsubscribe |
| TuneConfig.cs | v1.0 | SnakeEnchanter.Tunes | ✅ ScriptableObject |
| ExitTrigger.cs | v1.0 | SnakeEnchanter.Level | ✅ Done |
| GameEvents.cs | **v1.1** | SnakeEnchanter.Core | ✅ + OnTuneSuccessWithId |
| SnakeAI.cs | **v1.1** | SnakeEnchanter.Snakes | ✅ B-002 deprecated API fix |
| GameManager.cs | **v1.1.1** | SnakeEnchanter.Core | ✅ Game Loop, Mode, Session Tracking |
| HealthBarUI.cs | **v3.1** | SnakeEnchanter.UI | ✅ Gradient (continuous), Pulse, Debuff, Frame, Steampunk |
| TuneSliderUI.cs | **v2.1** | SnakeEnchanter.UI | ✅ Segmente, Marker, Frame, OnValidate, KeepAspect |
| CanvasUICreator.cs | **v2.0** | SnakeEnchanter.Editor | ✅ Neue Hierarchie + Auto-Wiring |
| TuneConfigCreator.cs | **v1.0** | SnakeEnchanter.Editor | ✅ Editor Menu Tool |

### Unity Scene Status (GameLevel.unity):
| GameObject | Components | Status |
|------------|------------|--------|
| Player | CharacterController, PlayerController v1.7, HealthSystem v1.2.1, TuneController v2.2 | ✅ Komplett |
| Cowboy (Child) | Animator (MC_Controller), Avatar | ⚠️ Animationen broken nach Avatar-Wechsel |
| CameraTarget | CameraHeadTracker v1.0 — unter Head Bone | ✅ Position-only Tracking |
| Main Camera | Camera, CinemachineBrain | ✅ Cinemachine-gesteuert |
| CM_PlayerCamera | CinemachineCamera, Follow=CameraTarget, Rotate With Follow Target | ✅ Cinemachine v3.x |
| ExitTrigger | BoxCollider (IsTrigger), ExitTrigger.cs | ✅ Platziert |
| Cave Map | Caves Parts Set + Dwarven Pack Prefabs | ✅ Fertig |
| Input Actions | SnakeEnchanter.inputactions (inkl. Crouch) | ✅ Funktioniert |
| GameManager | GameManager.cs | ✅ Angelegt |
| Snake(s) | Toon Cobra/Snake Prefabs + SnakeAI + BoxCollider | ✅ Platziert |
| Canvas (UI) | HealthBarUI v3.1, TuneSliderUI v2.1, Steampunk Theme, Arvo SDE Font | ✅ Fertig + getestet |

### TuneConfig ScriptableObjects:
| Asset | Key | Duration | Zone | Effect |
|-------|-----|----------|------|--------|
| Tune1_Move.asset | 1 | 3s | 40-65% | Move |
| Tune2_Sleep.asset | 2 | 4s | 35-60% | Sleep |
| Tune3_Attack.asset | 3 | 5s | 30-55% | Attack |
| Tune4_Freeze.asset | 4 | 6s | 25-50% | Freeze |

### Animator Controller (MC_Controller):
| Parameter | Type | Usage |
|-----------|------|-------|
| Speed | Float | Walk: >0.1, Idle: <=0.1 |
| IsCrouching | Bool | Crouch Walk Forward/Back |

**States:** Idle (default) → Walk → Crouch Walk Forward / Crouch Walk Back

---

## GIT STATUS

```
Branch: feature/animations-complete (aktiv)
Letzter Commit: dae0b75 Remove orphaned 3D_Assets.meta after folder restructure
Remote: https://github.com/JuliGommz/Snake_Enchanter.git
Uncommitted Changes: NEIN (clean state)
Main: dae0b75 (up-to-date, feature/canvas-ui wurde gemergt + gelöscht)
```

---

## ⚠️ OFFENE PROBLEME

### 1. ~~Player/Animation Setup~~ ✅ GELÖST (Session 4) → ⚠️ BROKEN (Session 5)
- Session 4: Gelöst (heightFromFeet, Animator auf Child, Old Man Idle)
- **Session 5: Animationen broken nach Cinemachine-Umbau + Avatar-Wechsel**
- User hat Player Avatar geändert und alles neu zugewiesen
- **Zu prüfen in Unity:**
  - Animator-Komponente auf Cowboy (Child), NICHT auf Player
  - Avatar = vom gleichen FBX wie das Mesh (z.B. Cowboy@Idle)
  - Apply Root Motion = **UNCHECKED**
  - PlayerController findet Animator via GetComponentInChildren
  - MC_Controller.controller zugewiesen mit Speed + IsCrouching Parametern

### 2. Snake MoveAwayTarget
- Beide Snakes liefen zum gleichen Punkt (übereinander)
- Jede Snake braucht ein individuelles MoveAwayTarget (Empty GameObject)
- Alternativ: Feature für Phase 1 Boceto deaktivieren

### 3. ~~Canvas UI~~ ✅ v3.1/v2.1 FERTIG + getestet
- Steampunk Theme mit Pergament-Rahmen, Arvo SDE Font
- HealthBarUI v3.1: Gradient (continuous), Pulse, Debuff, Frame
- TuneSliderUI v2.1: OnValidate, MarkerSize, FrameSliced, KeepAspect

---

## 📋 BACKLOG (Phase 2+)

### ~~B-001: TuneController Lambda-Leak~~ ✅ FIXED (v2.3)
- Cached delegates in Awake(), proper unsubscribe in DisableInput()

### ~~B-002: SnakeAI deprecated FindObjectsOfType~~ ✅ FIXED (v1.1)
- Replaced with FindObjectsByType<SnakeAI>(FindObjectsSortMode.None)

---

## NÄCHSTE AKTION

**Ziel:** Animationen reparieren, dann Core Loop testen

1. ✅ **Canvas UI v3.1/v2.1** — Fertig + getestet + Steampunk Theme (feature/canvas-ui → main gemergt)
2. ⬜ **Animationen reparieren** — Avatar/Cinemachine-Umbau hat Anims broken (feature/animations-complete)
3. ⬜ **Spell Cast + Death** zum Animator hinzufügen (IsCasting, IsDead Parameter)
4. ⬜ **Play-Test Core Loop** — Bewegen → Schlange → Tune → Effekt → Win/Lose
5. ⬜ **Phase 1 abschließen** — Alles spielbar?

---

## ⚠️ WICHTIGE ÄNDERUNGEN SESSION 4 (05.02)

### Toon Snakes Pack Integration:
- 6 Prefabs importiert: Toon Cobra/Snake x Green/Purple/Magenta
- 14 Cobra-Animationen: Idle, Slither(6), BiteAttack, BreathAttack, ProjectileAttack, CastSpell, TakeDamage, Die
- FX Prefabs: Poison Breath, Poison Projectile, Poison Projectile Impact
- Materials: URP/Lit Shader (korrekt)

### HealthSystem v1.2.1 Fixes:
- Drain Rate: 2.5 → 0.1 HP/sec (30HP für 5 Minuten)
- Passive Drain: Default deaktiviert (_enablePassiveDrain = false)
- Event Flood Fix: _lastReportedHealth verhindert 60x/sec Event-Spam
- Advanced Drain: 0.115 HP/sec (15% schneller als Simple)
- Namespace-Fix + Unity 2023 API (FindFirstObjectByType)

### GameManager v1.1.1:
- Drain Rates entfernt (delegiert an HealthSystem — Single Source of Truth)
- Namespace-Fix + Unity 2023 API

### Neue Mixamo-Animationen (16 Stück):
- Crouch: Walk Back, Walk Forward, Walk Left, Walk Right, Standing To Crouched, Crouched To Standing
- Spell: Magic Spell Casting, Spell Casting(1), Two Hand Spell Casting, Wide Arm Spell Casting
- Sonstige: Jump, Injured Idle, Sitting, Sitting Dazed, Standing Up, Taking Item

### MC_Controller (Animator Controller) neu aufgebaut:
- Verschoben von MC_Mixamo/ nach Animations/ (Root)
- 4 States: Idle, Walk, Crouch Walk Forward, Crouch Walk Back
- 2 Parameter: Speed (Float), IsCrouching (Bool)
- Transitions mit 0.25s Duration

### TuneConfig ScriptableObjects erstellt:
- 4 Assets via Editor-Tool (Menu → SnakeEnchanter → Create Tune Configs)
- Alle GDD-Werte korrekt konfiguriert
- Simple Mode Bonus: +10% Zone (außer Freeze: 0%)

### Snake-Integration:
- Snakes in Scene platziert mit SnakeAI + BoxCollider (IsTrigger)
- Proximity-basiertes Targeting funktioniert (_commandRange = 8f)
- MoveAwayTarget-Problem: Snakes stacken sich (individuell nötig)

### TagManager:
- "Enemy" Tag hinzugefügt

---

## ⚠️ WICHTIGE ÄNDERUNGEN SESSION 5 (06.02)

### Git Branch-Strategie:
- Feature-Branches: `feature/<kurzer-name>` von main
- Dokumentiert in CLAUDE.md unter "Arbeitsweise"
- Aktive Branches: `feature/canvas-ui`, `feature/animations-complete`

### Canvas UI v2.0 (Genshin-Style):
**HealthBarUI v2.0:**
- Gradient Farbsystem: Rot(0%) → Gelb(50%) → Grün(100%) via `Gradient.Evaluate()`
- Puls-Effekt: Alpha-Oszillation, beschleunigt unter 30% HP
- Debuff-Text immer sichtbar ("☠ Giftiger Nebel — HP sinkt")
- Keine Zahlen mehr (nur visuell, GDD 6.2)
- Position: Top-Center, 500x50

**TuneSliderUI v2.0:**
- Segmentierte Blöcke (15 Segmente, nicht solid fill)
- 3 Farbzonen: Gelb=Safe(nichts passiert), Orange=Success(Schlange gecharmt), Grau=Danger(Schlange greift an)
- Marker-Sprite (Musiknote/Flöte) bewegt sich entlang Segmenten
- Frame-Image für visuellen Rahmen
- Alle Farben, Dimensionen, Sprites per Inspector konfigurierbar

**CanvasUICreator v2.0:**
- Neue Hierarchie: SliderFrame, SegmentContainer, Marker
- Auto-Wiring aller neuen SerializeField-Referenzen
- DebuffText statt HealthText, Top-Center statt Top-Left

### Projektstruktur-Erweiterung:
```
Assets/_Project/Scripts/Editor/CanvasUICreator.cs (NEU v2.0)
Assets/Documentation/Media/Screenshots/Cooking-Slider-Example.png (Referenz)
```

---

## ⚠️ WICHTIGE ÄNDERUNGEN SESSION 5 NACHTRAG (06.02 spät)

### UI Polish:
- **TuneSliderUI v2.1:** OnValidate() für live Inspector-Updates, MarkerSize fix, Frame Image.Type=Sliced, KeepAspect
- **HealthBarUI v3.1:** Gradient continuous update (war nur bei Event), Steampunk Theme, Frame + Texture
- **CanvasUICreator:** SliderFrame extends beyond SliderArea (-6/-6 bis 6/6)
- **Steampunk UI Pack** (Gentleland) importiert für Rahmen-Sprites, Font: Arvo SDE

### Cinemachine v3.x Integration:
- **CM_PlayerCamera:** CinemachineCamera, Follow=CameraTarget, Rotate With Follow Target
- **CameraHeadTracker v1.0:** Position-only tracking des animierten Head Bones (LateUpdate)
- **PlayerController v1.7:** Cinemachine Final — Camera.main Auto-Find, Pitch-only Steuerung
- Body Rotation (Yaw) → PlayerController, Cinemachine folgt via "Rotate With Follow Target"
- Camera Position → Cinemachine Follow (folgt CameraTarget unter Head)

### Setup-Review:
- Alle 12 Scripts validiert — kein überflüssiges Script
- Backlog: B-001 Lambda-Leak, B-002 deprecated API

### Git:
- feature/canvas-ui → main gemergt (fast-forward) + Branch gelöscht
- feature/animations-complete erstellt + main gemergt

---

## ⚠️ WICHTIGE REGELN (NICHT VERHANDELBAR)

### Input System (ADR-006):
```
AUSSCHLIESSLICH Unity New Input System!
- NIEMALS UnityEngine.Input (Legacy)
- IMMER UnityEngine.InputSystem
```

### Kamera-System (Cinemachine v3.x):
```
Main Camera = Cinemachine Brain (auto-managed)
CM_PlayerCamera = CinemachineCamera mit:
  - Follow = CameraTarget (unter Head Bone, via CameraHeadTracker)
  - Rotation = "Rotate With Follow Target" (folgt Player Yaw)
PlayerController v1.7 steuert NUR:
  - Player Body Rotation (Yaw/Y-Achse, Mouse X)
  - Camera Pitch (X-Achse, Mouse Y, direkt auf Camera.main)
NIEMALS Kamera-Position per Script überschreiben!
```

### Animation-Entscheidung:
- **KEINE Flöte** (Animation zu komplex)
- **Spell Animation** stattdessen (Cast_Spell.anim vorhanden)

---

## KONTEXT FÜR NEUE SESSION

### Projektstruktur:
```
Snake_Enchanter/
├── Assets/
│   ├── _Project/
│   │   ├── Scripts/
│   │   │   ├── Core/{GameEvents v1.1, GameManager v1.1.1}.cs
│   │   │   ├── Player/{PlayerController v1.7, HealthSystem v1.2.1, CameraHeadTracker v1.0}.cs
│   │   │   ├── TuneSystem/{TuneController v2.3, TuneConfig}.cs
│   │   │   ├── Snakes/SnakeAI.cs
│   │   │   ├── UI/{HealthBarUI v3.1, TuneSliderUI v2.1}.cs
│   │   │   ├── Level/ExitTrigger.cs
│   │   │   └── Editor/{CanvasUICreator v2.0, TuneConfigCreator}.cs
│   │   ├── ScriptableObjects/TuneConfigs/ (4 TuneConfig SOs)
│   │   ├── 3D_Assets/
│   │   │   ├── Cave/ (7 modulare Cave-Teile)
│   │   │   └── Snakes/ (Toon Snakes Pack — 6 Prefabs, 14 Anims je Typ)
│   │   ├── Animations/
│   │   │   ├── MC_Controller.controller (Idle, Walk, Crouch)
│   │   │   └── MC_Mixamo/ (26 FBX + 2 .anim)
│   │   ├── Data/SnakeEnchanter.inputactions (inkl. Crouch)
│   │   └── Scenes/{GameLevel, MainMenu}.unity
│   ├── Documentation/GDD/GDD_v1.4_SnakeEnchanter.txt
│   ├── External_Assets/ (Caves Parts Set, Dwarven Pack, etc.)
│   └── Plugins/ (Toon Snakes Pack — Meshtint Studio)
├── CLAUDE.md (Projektkontext + REGELN)
└── STATE.md (diese Datei)
```

### Wichtige Dateien zum Einlesen:
1. `STATE.md` (diese Datei)
2. `CLAUDE.md` (Projektkontext + PROJEKT-REGELN)
3. `Assets/Documentation/ProjectStandards/01.Architecture_Decisions.txt` (ADRs)
4. `Assets/Documentation/GDD/GDD_v1.4_SnakeEnchanter.txt`

---

## SESSION HISTORY

| Datum | Was gemacht | Ergebnis |
|-------|-------------|----------|
| 03.02.2026 | Projekt-Setup, Git, Dokumentation, 4-Phasen-Modell | Bereit für Phase 1 Code |
| 03.02.2026 | ProjectStandards bereinigt (11→7), ADRs für Snake Enchanter | Struktur steht |
| 03.02.2026 | Perplexity Session: Core Scripts v1 erstellt | 5 Scripts done |
| 03.02.2026 | Expert Audit: TuneController auf ADR-008 Slider umgeschrieben | v2.0 compliant |
| 03.02.2026 | GDD v1.4: Slider-System vollständig dokumentiert | Doku aktuell |
| 03.02.2026 | **New Input System Migration** - PlayerController v1.2, TuneController v2.1 | ✅ Funktioniert |
| 03.02.2026 | Unity Integration: Player Setup, ExitTrigger, Input Actions zugewiesen | ✅ Scene ready |
| 03.02.2026 | Animation-Check: MC_Mixamo vorhanden, Spell statt Flute | Entscheidung |
| 04.02.2026 | **PlayerController v1.3→v1.5**: Crouch, Camera-Fix, Pitch Limits | ✅ v1.5 stabil |
| 04.02.2026 | **Crouch Action** in InputActions + Binding (LeftCtrl) | ✅ Funktioniert |
| 04.02.2026 | **Config Update**: Third-Person → First-Person | ✅ Konsistent |
| 04.02.2026 | **Cave Assets** inventarisiert, Collider geprüft (alle vorhanden) | ✅ Bereit |
| 04.02.2026 | **Cave Map + Player Sprite** fertig gebaut in Unity | ✅ Done |
| 04.02.2026 | **5 neue Scripts**: SnakeAI, GameManager, HealthBarUI, TuneSliderUI, TuneConfigCreator | ✅ Geschrieben |
| 04.02.2026 | **GameEvents v1.1**: OnTuneSuccessWithId hinzugefügt | ✅ Snake-Tune-Zuordnung |
| 04.02.2026 | **TuneController v2.2**: Feuert TuneSuccessWithId | ✅ Kompatibel |
| **05.02.2026** | **Toon Snakes Pack** importiert + in Scene platziert | ✅ 6 Prefabs |
| **05.02.2026** | **Snake-Sichtbarkeit** debuggt (mit Dozent gelöst) | ✅ Fixed |
| **05.02.2026** | **TuneConfigs** erstellt (4 ScriptableObjects via Editor-Tool) | ✅ Done |
| **05.02.2026** | **GameManager + SnakeAI** in Scene integriert | ✅ Funktioniert |
| **05.02.2026** | **HealthSystem v1.2.1**: Drain-Fix, Event-Flood-Fix, Namespace-Fix | ✅ Stabil |
| **05.02.2026** | **16 Mixamo-Animationen** importiert, MC_Controller neu aufgebaut | ✅ Importiert |
| **05.02.2026** | **Player/Animation Problem** gelöst — heightFromFeet, Animator auf Child, Old Man Idle | ✅ GELÖST |
| **06.02.2026** | **Git Branch-Strategie** eingeführt: feature/<name> Workflow | ✅ Dokumentiert in CLAUDE.md |
| **06.02.2026** | **CanvasUICreator v1.0** erstellt (Editor Menu Tool) | ✅ Commit efd06b9 |
| **06.02.2026** | **Canvas UI v2.0**: Genshin-Style Customization geplant + implementiert | ✅ 3 Dateien |
| **06.02.2026** | **HealthBarUI v2.0**: Gradient, Pulse, Debuff-Text, kein HP-Text | ✅ Fertig |
| **06.02.2026** | **TuneSliderUI v2.0**: Segmente, Marker, Frame, 3 Zonen-Farben | ✅ Fertig |
| **06.02.2026** | **CanvasUICreator v2.0**: Neue Hierarchie + Auto-Wiring | ✅ Commit 7e71b13 |
| **06.02.2026** | **TuneSliderUI v2.1**: MarkerSize, FrameSliced, OnValidate, KeepAspect | ✅ Commit b8c03e4 |
| **06.02.2026** | **CanvasUICreator**: SliderFrame extends beyond SliderArea | ✅ Commit 302c9cb |
| **06.02.2026** | **HealthBarUI v3.1**: Gradient continuous update fix | ✅ Steampunk Theme |
| **06.02.2026** | **Cinemachine v3.x** integriert: CM_PlayerCamera + CameraHeadTracker | ✅ PlayerController v1.7 |
| **06.02.2026** | **Steampunk UI Pack** importiert, Player Avatar geändert | ✅ Visuelles Update |
| **06.02.2026** | **Setup-Review** aller 12 Scripts: Keine Redundanz, Standards OK | ✅ B-001, B-002 geloggt |
| **06.02.2026** | **Commit Session 5**: UI polish, Cinemachine, asset restructure | ✅ Commit 01c0329 |
| **06.02.2026** | **feature/canvas-ui → main** gemergt + Branch gelöscht | ✅ Fast-forward |
| **06.02.2026** | **feature/animations-complete** erstellt, main gemergt | ⚠️ Anims broken |
