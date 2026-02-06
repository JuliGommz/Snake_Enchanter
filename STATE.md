# PROJECT STATE - Snake Enchanter
> **WICHTIG FÜR NEUE SESSIONS:** Diese Datei enthält den aktuellen Projektstand.
> Lies diese Datei ZUERST bevor du mit der Arbeit beginnst.

**Letzte Aktualisierung:** 2026-02-05 (Session 4)
**Letzte Session:** Toon Snakes integriert, TuneConfigs erstellt, HealthDrain gefixt, Animationen importiert

---

## AKTUELLER STAND

### Phase: 1 - SPIELBAR (von 4)
### Status: Player + Animationen funktionieren! Canvas UI fehlt noch.

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
- [ ] 1.16 UI: HealthBar + TuneSlider ← **NOCH OFFEN** (Scripts fertig, Canvas fehlt)
- [ ] 1.17 Play-Test Core Loop

### Scripts Status:
| Script | Version | Namespace | Status |
|--------|---------|-----------|--------|
| PlayerController.cs | **v1.5** | SnakeEnchanter.Player | ✅ Hierarchy Camera, Crouch, Pitch Limits |
| HealthSystem.cs | **v1.2.1** | SnakeEnchanter.Player | ✅ Drain 0.1 HP/sec, deaktiviert für Dev |
| TuneController.cs | **v2.2** | SnakeEnchanter.Tunes | ✅ + TuneSuccessWithId Event |
| TuneConfig.cs | v1.0 | SnakeEnchanter.Tunes | ✅ ScriptableObject |
| ExitTrigger.cs | v1.0 | SnakeEnchanter.Level | ✅ Done |
| GameEvents.cs | **v1.1** | SnakeEnchanter.Core | ✅ + OnTuneSuccessWithId |
| SnakeAI.cs | **v1.0** | SnakeEnchanter.Snakes | ✅ State Machine, Tune Reaction |
| GameManager.cs | **v1.1.1** | SnakeEnchanter.Core | ✅ Game Loop, Mode, Session Tracking |
| HealthBarUI.cs | **v1.0** | SnakeEnchanter.UI | ✅ Script fertig, Canvas fehlt |
| TuneSliderUI.cs | **v1.0** | SnakeEnchanter.UI | ✅ Script fertig, Canvas fehlt |
| TuneConfigCreator.cs | **v1.0** | SnakeEnchanter.Editor | ✅ Editor Menu Tool |

### Unity Scene Status (GameLevel.unity):
| GameObject | Components | Status |
|------------|------------|--------|
| Player | CharacterController, PlayerController v1.5, HealthSystem v1.2.1, TuneController v2.2 | ✅ Komplett |
| Main Camera | Camera, AudioListener — **CHILD of Player** | ✅ Position via Hierarchy |
| ExitTrigger | BoxCollider (IsTrigger), ExitTrigger.cs | ✅ Platziert |
| Cave Map | Caves Parts Set + Dwarven Pack Prefabs | ✅ Fertig |
| Input Actions | SnakeEnchanter.inputactions (inkl. Crouch) | ✅ Funktioniert |
| GameManager | GameManager.cs | ✅ Angelegt |
| Snake(s) | Toon Cobra/Snake Prefabs + SnakeAI + BoxCollider | ✅ Platziert |
| Canvas (UI) | **NOCH ANLEGEN** — HealthBarUI + TuneSliderUI | ⏳ Ausstehend |

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
Branch: main
Letzter Commit: 1962557 Session 2: PlayerController v1.5, Crouch, Cave Map, First-Person fixes
Remote: https://github.com/JuliGommz/Snake_Enchanter.git
Uncommitted Changes: JA — Sehr viele (Scripts, Assets, Animationen, Scene, TuneConfigs)
```

---

## ⚠️ OFFENE PROBLEME

### 1. ~~Player/Animation Setup~~ ✅ GELÖST
- **Ursache gefunden:** Humanoid Animator Root Transform Position `heightFromFeet: 0` (Original) statt `1` (Feet)
- **Fixes angewendet:**
  - Animator von Player (Parent) auf Cowboy (Child) verschoben — Standard Unity Pattern
  - heightFromFeet: 1 für alle Animations-FBX
  - CharacterController: Height=1.84, Center Y=0.9
  - Injured Idle ersetzt durch Old Man Idle (defekte Root-Orientation)
- **Ergebnis:** Idle, Walk, Crouch funktionieren!

### 2. Snake MoveAwayTarget
- Beide Snakes liefen zum gleichen Punkt (übereinander)
- Jede Snake braucht ein individuelles MoveAwayTarget (Empty GameObject)
- Alternativ: Feature für Phase 1 Boceto deaktivieren

### 3. Canvas UI noch nicht aufgebaut
- Scripts (HealthBarUI, TuneSliderUI) sind fertig
- Canvas muss in Unity erstellt werden mit Slider, Images, TextMeshPro

---

## NÄCHSTE AKTION

**Ziel:** Player/Animation-Problem lösen, dann Core Loop spielbar machen

1. ⬜ **Player/Animation debuggen** — Spezifisches Problem identifizieren
2. ⬜ **Snake MoveAwayTargets** individuell setzen (oder deaktivieren für Boceto)
3. ⬜ **Canvas UI aufbauen** — HealthBar (top-left) + TuneSlider (center-bottom)
4. ⬜ **Play-Test Core Loop** — Bewegen → Schlange → Tune → Effekt → Win/Lose
5. ⬜ **Commit!**

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

## ⚠️ WICHTIGE REGELN (NICHT VERHANDELBAR)

### Input System (ADR-006):
```
AUSSCHLIESSLICH Unity New Input System!
- NIEMALS UnityEngine.Input (Legacy)
- IMMER UnityEngine.InputSystem
```

### Kamera-Position:
```
Kamera = Child des Players in Hierarchy.
Position über Transform im Scene View.
Script kontrolliert NUR Rotation + Crouch-Transition.
NIEMALS Kamera-Position per SerializeField überschreiben.
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
│   │   │   ├── Player/{PlayerController v1.5, HealthSystem v1.2.1}.cs
│   │   │   ├── TuneSystem/{TuneController v2.2, TuneConfig}.cs
│   │   │   ├── Snakes/SnakeAI.cs
│   │   │   ├── UI/{HealthBarUI, TuneSliderUI}.cs
│   │   │   ├── Level/ExitTrigger.cs
│   │   │   └── Editor/TuneConfigCreator.cs
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
