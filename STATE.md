# PROJECT STATE - Snake Enchanter
> **WICHTIG FÜR NEUE SESSIONS:** Diese Datei enthält den aktuellen Projektstand.
> Lies diese Datei ZUERST bevor du mit der Arbeit beginnst.

**Letzte Aktualisierung:** 2026-02-04 (Session 2)
**Letzte Session:** PlayerController v1.5, Crouch, Camera-Fix, Cave-Vorbereitung

---

## AKTUELLER STAND

### Phase: 1 - SPIELBAR (von 4)
### Status: Player Controller FERTIG, Cave Map IN ARBEIT (User baut in Unity)

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
- [ ] 1.10 Cave Map aufbauen (Caves Parts Set + Dwarven Pack) ← **IN ARBEIT**
- [ ] 1.11 Player Sprite einbinden (statisch, First-Person)
- [ ] 1.12 Toon Snakes Pack importieren (Asset Store)
- [ ] 1.13 Snake AI (Basic)
- [ ] 1.14 TuneConfig ScriptableObjects anlegen
- [ ] 1.15 UI: HealthBar + TuneSlider (Minimal)
- [ ] 1.16 Play-Test Core Loop

### Scripts Status:
| Script | Version | Namespace | Status |
|--------|---------|-----------|--------|
| PlayerController.cs | **v1.5** | SnakeEnchanter.Player | ✅ Hierarchy Camera, Crouch, Pitch Limits |
| HealthSystem.cs | v1.1 | SnakeEnchanter.Player | ✅ Done |
| TuneController.cs | **v2.1** | SnakeEnchanter.Tunes | ✅ **New Input System** |
| TuneConfig.cs | v1.0 | SnakeEnchanter.Tunes | ✅ ScriptableObject |
| ExitTrigger.cs | v1.0 | SnakeEnchanter.Level | ✅ Done |
| GameEvents.cs | v1.0 | SnakeEnchanter.Core | ✅ Done |

### Unity Scene Status (GameLevel.unity):
| GameObject | Components | Status |
|------------|------------|--------|
| Player | CharacterController, PlayerController v1.5, HealthSystem, TuneController | ✅ Komplett |
| Main Camera | Camera, AudioListener — **CHILD of Player** | ✅ Position via Hierarchy |
| ExitTrigger | BoxCollider (IsTrigger), ExitTrigger.cs | ✅ Platziert |
| Cave Map | Caves Parts Set + Dwarven Pack Prefabs | 🔨 In Arbeit |
| Input Actions | SnakeEnchanter.inputactions (inkl. Crouch) | ✅ Funktioniert |

---

## GIT STATUS

```
Branch: main
Letzter Commit: eeae128 Phase 1 Core Implementation
Remote: https://github.com/JuliGommz/Snake_Enchanter.git
Uncommitted Changes: JA - PlayerController v1.5, Crouch, inputactions, Config update
```

---

## NÄCHSTE AKTION

**Was:** Cave Map fertig bauen + Player Sprite einbinden
**Wo:** Unity Scene (GameLevel.unity)
**Details:**
1. Cave Map mit Caves Parts Set aufbauen (User macht das in Unity)
2. Player Sprite einbinden (statisch, kein Animation jetzt)
3. Toon Snakes Pack aus Asset Store importieren
4. TuneConfig ScriptableObjects erstellen (4 Stück)
5. UI: HealthBar + TuneSlider minimal
6. Snake AI + Core Loop Test

---

## ⚠️ WICHTIGE ÄNDERUNGEN DIESE SESSION (04.02)

### PlayerController v1.2 → v1.5:
- **v1.3:** Crouch-System (Hold LeftCtrl), reduced speed
- **v1.4:** Camera offset via SerializeField (ENTFERNT — überschrieb Inspector-Werte)
- **v1.5:** Kamera-Position NUR via Hierarchy-Transform, Script kontrolliert nur Rotation
  - Kamera muss CHILD des Players sein
  - Position im Scene View / Transform einstellen
  - Script liest Position bei Awake(), schreibt sie nur beim Crouchen

### Input Actions Update:
- Neue Action: **Crouch** (LeftCtrl)

### Camera Best Practices:
- Min Pitch: -70° (nach unten)
- Max Pitch: 70° (nach oben)
- Crouch Speed: 2.5 (50% von Move Speed 5.0)

### Config Update:
- Snake_EnchanterConfig.txt: Third-Person → First-Person (3 Stellen)

### Cave Assets inventarisiert:
- Caves Parts Set: 7 modulare Teile (I, L1, L2, E, U, X1, X2) — alle mit MeshCollider
- Dwarven Expedition Pack: Pillars, Arches, Gates, Wall Props, Window Props
- WeaponsAndPropsAssetPack: PanFlute, Barrel, Chest (Deko)

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

## OFFENE FRAGEN / BLOCKER

1. **Toon Snakes Pack** — Noch nicht importiert, muss aus Asset Store
2. **Cave Map** — User baut aktiv in Unity

---

## KONTEXT FÜR NEUE SESSION

### Projektstruktur:
```
Snake_Enchanter/
├── Assets/
│   ├── _Project/
│   │   ├── Scripts/
│   │   │   ├── Core/GameEvents.cs
│   │   │   ├── Player/{PlayerController v1.5, HealthSystem}.cs
│   │   │   ├── TuneSystem/{TuneController,TuneConfig}.cs
│   │   │   └── Level/ExitTrigger.cs
│   │   ├── Animations/MC_Mixamo/ (Walk, Cast_Spell, Death)
│   │   ├── Data/SnakeEnchanter.inputactions (inkl. Crouch)
│   │   └── Scenes/{GameLevel,MainMenu}.unity
│   ├── Documentation/GDD/GDD_v1.4_SnakeEnchanter.txt
│   ├── External_Assets/Caves Parts Set/ (7 modulare Cave-Teile)
│   ├── Toby Fredson/Dwarven Expedition Pack/ (Pillars, Arches, etc.)
│   └── WeaponsAndPropsAssetPack_NAS/ (Props)
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
| **04.02.2026** | **PlayerController v1.3→v1.5**: Crouch, Camera-Fix, Pitch Limits | ✅ v1.5 stabil |
| **04.02.2026** | **Crouch Action** in InputActions + Binding (LeftCtrl) | ✅ Funktioniert |
| **04.02.2026** | **Config Update**: Third-Person → First-Person | ✅ Konsistent |
| **04.02.2026** | **Cave Assets** inventarisiert, Collider geprüft (alle vorhanden) | ✅ Bereit |
| **04.02.2026** | **Cave Map**: User baut aktiv in Unity | 🔨 In Arbeit |
