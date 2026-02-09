# Snake Enchanter - Projektkontext

> **⚠️ NEUE SESSION?** Lies `STATE.md` **KOMPLETT** bevor du eine einzige Frage stellst oder Vorschläge machst. Alles was du wissen musst steht dort. Frag NICHT nach Kontext — arbeite.

## Projekt-Übersicht
- **Name:** Snake Enchanter
- **Engine:** Unity 3D (2022 LTS, URP)
- **Plattform:** Windows, 1920x1080 + Ultrawide, 60 FPS
- **Zeitraum:** 03.02.2026 – ~03.03.2026
- **Typ:** Solo-Projekt, akademische Einreichung (PIP-3 Theme B)

## Kernmechanik (ADR-008: Genshin-Style Slider)
Timing-basierte Schlangen-Kontrolle durch Melodien mit Hold & Release System:
1. **Taste HALTEN** → Slider bewegt sich von 0 nach 1
2. **Taste LOSLASSEN** → Position wird gegen Triggerzone geprüft
3. **Ergebnisse:**
   - Zu früh (vor Zone) = Safe Fail, kein Schaden
   - In Zone = SUCCESS → Snake charmed, +HP
   - Zu spät (nach Zone) = FAIL → Snake greift an!

## Game Loop
1. Modus wählen (Simple/Advanced)
2. Durch Ruine navigieren, Schlangen charmen
3. HP verwalten (passiver Drain + Restoration bei Erfolg)
4. Fenster erreichen (Win) oder HP=0 (Lose)

## Tunes (Slider-System)
| Taste | Tune | Slider-Dauer | Triggerzone | Effekt |
|-------|------|--------------|-------------|--------|
| 1 | Move | 3s | 40-65% | Schlange weicht aus |
| 2 | Sleep | 4s | 35-60% | Schlange schläft ein |
| 3 | Attack | 5s | 30-55% | Schlange greift Feind an |
| 4 | Freeze | 6s | 25-50% | Alle einfrieren (Advanced) |

*Triggerzone = Position auf Slider wo Loslassen = Erfolg*

## Modi
- **Simple:** Längere Timing-Fenster, langsamerer HP-Drain, keine Timer
- **Advanced:** Striktere Timings, 15% schnellerer Drain, dynamischer Collapse-Timer

---

## Entwicklungsansatz: 4 Phasen

**Prinzip:** Jede Phase ist abgebbar. Qualität konstant, Umfang wächst.

| Phase | Ziel |
|-------|------|
| **1 - Spielbar** | Kern-Loop mit Greybox |
| **2 - Komplett** | Alle Features, rough |
| **3 - Schön** | Polish & Juice |
| **4 - Fertig** | Abgabe-Ready |

> **Aktueller Stand:** Siehe `STATE.md`

---

## Ordnerstruktur
```
Assets/
├── _Project/           # Eigene Assets
│   ├── Scripts/        # Core, Player, Snakes, TuneSystem, UI, Data
│   ├── Prefabs/        # Player, Snakes, Environment, UI
│   ├── ScriptableObjects/
│   └── Scenes/         # MainMenu, GameLevel
├── Documentation/      # GDD, Projektplan, Arbeitsprotokoll, Media
└── Plugins/            # Toon Snakes Pack
```

## Namespaces
- `SnakeEnchanter.Core` - GameManager, GameState, GameEvents
- `SnakeEnchanter.Player` - PlayerController, HealthSystem
- `SnakeEnchanter.Snakes` - SnakeAI, SnakeBehavior
- `SnakeEnchanter.Tunes` - TuneController, TuneConfig
- `SnakeEnchanter.Level` - ExitTrigger, LevelManager
- `SnakeEnchanter.UI` - HealthBar, ResultScreen
- `SnakeEnchanter.Data` - API, SessionData

## Coding Standards
- C# Naming: PascalCase für public, _camelCase für private
- Ein Script = Eine Verantwortung
- ScriptableObjects für Konfigurationsdaten
- Events für lose Kopplung

## ⚠️ PROJEKT-REGELN (NICHT VERHANDELBAR)

### Input System
**AUSSCHLIESSLICH Unity New Input System verwenden!**
- NIEMALS `UnityEngine.Input` (Legacy Input) verwenden
- NIEMALS `Input.GetKey()`, `Input.GetAxis()`, etc.
- IMMER `InputAction`, `InputActionAsset` aus `UnityEngine.InputSystem`
- Input Actions Asset: `Assets/_Project/Data/SnakeEnchanter.inputactions`
- Project Settings → Player → Active Input Handling = "Input System Package (New)"

### Kamera-System (Cinemachine v3.x)
**Cinemachine steuert Position + Yaw, PlayerController steuert Pitch!**
- **Main Camera:** CinemachineBrain (auto-managed)
- **CM_PlayerCamera:** CinemachineCamera
  - Tracking Target = CameraTarget (leeres GameObject unter Head Bone)
  - Kein eigenes Script nötig — Cinemachine übernimmt Follow, Offset, Damping
- **CameraTarget:** Leeres GameObject unter Head Bone im Skeleton (kein Script!)
- **PlayerController v1.8:** Nur Pitch (Mouse Y) + Body Yaw (Mouse X)
- NIEMALS Kamera-Position per Script überschreiben — Cinemachine besitzt die Position!

---

## Wichtige Dateien
| Datei | Zweck |
|-------|-------|
| `STATE.md` | **Aktueller Stand** - IMMER ZUERST LESEN |
| `CLAUDE.md` | Projektkontext (diese Datei) |
| `Assets/Documentation/Projektplan_SnakeEnchanter.md` | Aufgaben & Phasen |
| `Assets/Documentation/Arbeitsprotokoll_Julian_Gomez.md` | Tägliche Doku |
| `Assets/Documentation/GDD/GDD_v1.4_SnakeEnchanter.txt` | Game Design |
| `Assets/Documentation/MVP_Phasen.md` | Phasen-Details |

## Backend API
- POST `/api/game-session` - Session-Stats speichern
- GET `/api/leaderboard?mode=simple/advanced` - Bestenliste
- GET `/api/player-stats` - Aggregierte Stats

## Assets
- **Player Avatar:** Pirate Pack (`_Project/Animations/Pirate/`) — FBX + Materials + Texturen + 14 Mixamo Animations
- **Snakes:** Toon Snakes Pack (Meshtint Studio)
- **Environment:** Caves Parts Set + Dwarven Pack
- **UI:** Steampunk UI Pack (Gentleland), Font: Arvo SDE
- **Audio:** Lizenzfreie Flötenmelodien (5-12s pro Tune)

---

## Arbeitsweise

### Git Branch-Strategie (ab Session 5)
**Jedes Feature bekommt einen eigenen Branch!**

- **Branch-Naming:** `feature/<kurzer-name>` (z.B. `feature/canvas-ui`, `feature/snake-moveaway`)
- **Workflow:**
  1. Neues Feature → `git checkout -b feature/<name>` von `main`
  2. Arbeiten + Commits auf dem Feature-Branch
  3. Feature fertig → Merge in `main` (oder PR falls gewünscht)
  4. Branch löschen nach Merge
- **Claude-Pflicht:** Bei Feature-Start und Feature-Ende den Entwickler erinnern:
  - **Start:** "Neuen Branch erstellen: `feature/<name>`"
  - **Ende:** "Feature fertig — Branch in `main` mergen?"

### Für den Entwickler:
- **Täglich:** Arbeitsprotokoll + Screenshot + Commit
- **Phasen-Regel:** Keine neue Phase bevor vorherige DONE
- **Bei Problemen:** Scope reduzieren, nicht Zeit verlängern

### Für Claude (AI Assistant):
- **Session-Start:** `STATE.md` lesen für aktuellen Stand
- **Während Arbeit:** State-Updates nach jedem größeren Schritt
- **Session-Ende:** `STATE.md` aktualisieren mit:
  - Was wurde gemacht
  - Nächster Schritt
  - Offene Fragen/Blocker
  - Git-Status (Branch, letzter Commit)
