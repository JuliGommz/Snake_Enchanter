# Arbeitsprotokoll - Snake Enchanter
**Teilnehmer:** Julian Gomez
**Projekt:** Snake Enchanter - PIP-3 Theme B
**Zeitraum:** 03.02.2026 – ~03.03.2026

---

## Phase 1: SPIELBAR

### 03.02.2026 (Montag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| Unity Projekt anlegen | x | x | x |
| Ordnerstruktur erstellen (_Project, Scripts, etc.) | x | x | x |
| Git Repository initialisieren | x | x | x |
| GitHub Remote verbinden + Push | x | x | x |
| Documentation migrieren (GDD, Config) | x | x | x |
| .gitignore konfigurieren | x | x | x |
| Projektplan erstellen (4-Phasen-Modell) | x | x | x |
| Arbeitsprotokoll-Template erstellen | x | x | x |
| MVP-Struktur definieren | x | x | x |
| Core Scripts erstellen (6 Scripts) | x | x | x |
| GDD v1.4 mit Slider-System aktualisieren | x | x | x |
| New Input System Migration (v1.2/v2.1) | x | x | x |
| Unity Integration (Player, ExitTrigger) | x | x | x |
| Input Actions Asset konfigurieren | x | x | x |
| Greybox Level aufbauen | x | x | x |
| ADR-006 als Projekt-Regel definieren | x | x | x |

**Screenshot:** `Media/Screenshots/2026-02-03_ProjectSetup.png`

**Notizen:**
- Unity 2022 LTS mit URP Template
- 4-Phasen-Entwicklungsmodell: Spielbar → Komplett → Schön → Fertig
- Jede Phase ist abgebbar - Qualität konstant, Umfang wächst
- Initial Commit + Dokumentation Commit
- **WICHTIG:** Nur New Input System verwenden (ADR-006 Projekt-Regel)
- Animation-Entscheidung: Spell statt Flute (einfacher)
- Scripts: PlayerController v1.2, TuneController v2.1, HealthSystem, GameEvents, TuneConfig, ExitTrigger
- Unity Scene: Player komplett konfiguriert, ExitTrigger platziert, Input Actions zugewiesen
- **OFFEN:** Toon Snakes Pack muss noch aus Asset Store importiert werden

---

### 04.02.2026 (Dienstag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| PlayerController v1.3: Crouch-System (LeftCtrl) | x | x | x |
| PlayerController v1.4: Camera Offset (entfernt) | x | x | x |
| PlayerController v1.5: Hierarchy Camera | x | x | x |
| Crouch Action in InputActions + Binding | x | x | x |
| Camera Pitch Limits (Best Practice -70/+70) | x | x | x |
| Crouch Speed 50% (2.5 von 5.0) | x | x | x |
| Config Update: Third-Person → First-Person | x | x | x |
| Cave Assets inventarisieren + Collider prüfen | x | x | x |
| Cave System aufbauen (Caves Parts Set) | x | x | x |

**Screenshot:** `Media/Screenshots/2026-02-04_CaveMap.png`

**Notizen:**
- PlayerController durchlief 3 Iterationen (v1.3→v1.5) wegen Camera-Override-Problem
- Lektion: Script sollte Kamera-Position NICHT besitzen, sondern aus Hierarchy lesen
- Cave Prefabs haben alle MeshCollider (IsTrigger=false, Convex=false)
- Dwarven Pack für Deko (Pillars, Arches, Window Props für Exit)
- Cave System fertig gebaut in Unity
- **OFFEN:** TuneConfig SOs, UI (HealthBar/Slider), Snake AI, Toon Snakes Pack


---

### 05.02.2026 (Mittwoch)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| Toon Snakes Pack importieren + in Scene platzieren | x | x | x |
| Snake-Sichtbarkeit in Game View debuggen (mit Dozent gelöst) | | x | x |
| TuneConfig ScriptableObjects erstellen (Editor-Tool) | x | x | x |
| GameManager GameObject anlegen + konfigurieren | x | x | x |
| SnakeAI auf Snakes (BoxCollider + SnakeAI Component) | x | x | x |
| TuneConfigs im TuneController zuweisen | x | x | x |
| HealthSystem Drain-Rate kalibrieren (0.1 HP/sec, deaktiviert für Dev) | | x | x |
| HealthSystem Event-Flood Fix (_lastReportedHealth) | | x | x |
| Snake Tune-Targeting testen (Proximity-basiert, Command Range) | x | x | x |
| 16 neue Mixamo-Animationen importiert (Crouch, Spell, etc.) | x | x | x |
| MC_Controller Animator neu aufgebaut (Idle, Walk, Crouch) | x | x | x |
| Player/Animation Setup untersuchen + debuggen | x | x | x |
| Player Einsinken Fix (heightFromFeet + keepOriginalOrientation) | | x | x |
| Animator von Player auf Cowboy (child) verschoben | | x | x |
| CharacterController Capsule korrekt konfiguriert | | x | x |
| Canvas UI aufbauen (HealthBar + TuneSlider) | x | | |

**Screenshot:** `Media/Screenshots/2026-02-05_.png`

**Notizen:**
- Toon Snakes Pack (Meshtint Studio) importiert: 6 Prefabs (Cobra/Snake x Green/Purple/Magenta)
- Snake-Sichtbarkeit: Snakes in Scene View sichtbar aber Game View unsichtbar — mit Dozent gelöst
- TuneConfigs: 4 ScriptableObjects per Editor-Tool erstellt (Move 3s, Sleep 4s, Attack 5s, Freeze 6s)
- HealthDrain Bug: Rate war 2.5 HP/sec (viel zu schnell), korrigiert auf 0.1 HP/sec, deaktiviert für Dev
- HealthChanged Event Flood: Feuerte 60x/sec, Fix durch _lastReportedHealth Check
- Snake-Verhalten getestet: Proximity-basiertes Targeting funktioniert (nächste Snake reagiert)
- Snake MoveAwayTarget: Beide Snakes liefen zum gleichen Punkt — individuell pro Snake nötig
- 16 neue Mixamo-Animationen importiert (Crouch Walk, Spell Casting, Jump, etc.)
- MC_Controller neu aufgebaut: Idle → Walk (Speed > 0.1), Crouch Forward/Back (IsCrouching)
- **Player Einsinken debuggt:** Ursache war Humanoid Animator Root Transform Position — heightFromFeet war 0 (Original) statt 1 (Feet). Alle 4 Animations-FBX gefixt.
- **Animator verschoben:** Von Player (Parent) auf Cowboy (Child mit Bone-Hierarchy) — Standard Unity Pattern
- **CharacterController:** Height=1.84, Center Y=0.9, Radius=0.3 — Capsule korrekt um Model
- **Animationen funktionieren!** Walk, Crouch — Fortschritt!
- **Injured Idle ersetzt:** FBX hatte defekte Root-Orientation. Ersetzt durch "Old Man Idle" — funktioniert!
- **PLAYER ANIMATION KOMPLETT GELÖST!** Idle steht, Walk funktioniert, Crouch funktioniert
- **OFFEN:** Canvas UI (HealthBar + TuneSlider) noch nicht aufgebaut


---

### 06.02.2026 (Donnerstag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| Git Branch-Strategie einrichten + dokumentieren | x | x | x |
| CanvasUICreator v1.0 Editor-Tool erstellen | x | x | x |
| HealthBarUI v2.0→v3.1: Gradient, Pulse, Debuff, Frame | x | x | x |
| TuneSliderUI v2.0→v2.1: Segmente, Marker, Frame, Zonen | x | x | x |
| CanvasUICreator v2.0: Neue Hierarchie + Auto-Wiring | x | x | x |
| Canvas UI im Inspector anpassen (Steampunk-Theme, Arvo SDE Font) | x | x | x |
| Cinemachine einbauen (CM_PlayerCamera, CameraHeadTracker) | x | x | x |
| PlayerController v1.5→v1.7: Cinemachine-kompatibel (Pitch-only) | x | x | x |
| CameraHeadTracker.cs erstellen (Position-only Tracking) | x | x | x |
| Steampunk UI Pack (Gentleland) importieren | x | x | x |
| 3D_Assets Ordnerstruktur bereinigen | x | x | x |
| Gradient-Bug fixen (UpdateBarColor nur bei Event statt kontinuierlich) | | x | x |
| TuneSliderUI Fixes: MarkerSize, Frame Sliced, OnValidate | | x | x |
| Setup-Review aller 12 Scripts (Qualitaet, Standards, Redundanz) | x | x | x |
| Backlog erstellen (B-001 Lambda-Leak, B-002 deprecated API) | | x | x |
| Player Avatar geaendert + Cinemachine Kamera repariert | x | x | x |
| Animationen nach Cinemachine-Umbau testen | x | x | |

**Screenshot:** `Media/Screenshots/2026-02-06_.png`

**Notizen:**
- Git Branch-Strategie eingefuehrt: `feature/<name>` von main, Claude erinnert bei Start/Ende
- Canvas UI komplett ueberarbeitet: Genshin-Style Segmented Slider mit 3 Farbzonen (Gelb=Safe, Orange=Success, Grau=Danger)
- HealthBar: Gradient (Rot→Gelb→Gruen), Puls-Effekt unter 30% HP, Debuff-Text, kein HP-Text
- Steampunk UI Pack fuer Rahmen/Frame-Sprites, Font: Arvo SDE
- Cinemachine v3.x integriert: Kamera folgt Head-Bone, PlayerController steuert nur Pitch
- CameraHeadTracker.cs: Verfolgt nur Position des Head-Bones, Rotation bleibt bei PlayerController
- PlayerController v1.7: Cinemachine Final — Camera.main Auto-Find, Pitch-only Steuerung
- **Gradient-Bug gefixt:** UpdateBarColor wurde nur bei OnHealthChanged aufgerufen, nicht kontinuierlich
- **Setup-Review:** Alle 12 Scripts validiert — kein ueberflussiges Script, alle Standards eingehalten
- **Backlog:** B-001 TuneController Lambda-Leak, B-002 SnakeAI deprecated FindObjectsOfType
- **PROBLEM:** Nach Cinemachine-Umbau und Avatar-Aenderung funktionieren Animationen nicht mehr
- **OFFEN:** Animations-Problem debuggen (Avatar-Zuweisung, Animator-Platzierung pruefen)


---

### 07.02.2026 (Freitag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-07_.png`

**Notizen:**


---

## Phase 2: KOMPLETT

### 10.02.2026 (Montag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-10_.png`

**Notizen:**


---

### 11.02.2026 (Dienstag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-11_.png`

**Notizen:**


---

### 12.02.2026 (Mittwoch)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-12_.png`

**Notizen:**


---

### 13.02.2026 (Donnerstag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-13_.png`

**Notizen:**


---

### 14.02.2026 (Freitag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-14_.png`

**Notizen:**


---

## Phase 3: SCHÖN

### 17.02.2026 (Montag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-17_.png`

**Notizen:**


---

### 18.02.2026 (Dienstag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-18_.png`

**Notizen:**


---

### 19.02.2026 (Mittwoch)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-19_.png`

**Notizen:**


---

### 20.02.2026 (Donnerstag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-20_.png`

**Notizen:**


---

### 21.02.2026 (Freitag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-21_.png`

**Notizen:**


---

## Phase 4: FERTIG

### 24.02.2026 (Montag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-24_.png`

**Notizen:**


---

### 25.02.2026 (Dienstag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-25_.png`

**Notizen:**


---

### 26.02.2026 (Mittwoch)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-26_.png`

**Notizen:**


---

### 27.02.2026 (Donnerstag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-27_.png`

**Notizen:**


---

### 02.03.2026 (Montag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-03-02_.png`

**Notizen:**


---

### 03.03.2026 (Dienstag) - ABGABE
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| Finale Abgabe vorbereiten |  |  |  |
| ZIP erstellen |  |  |  |
| Präsentation halten |  |  |  |

**Screenshot:** `Media/Screenshots/2026-03-03_Final.png`

**Notizen:**


---

## Zusammenfassung

| Phase | Ziel | Ergebnis |
|-------|------|----------|
| 1 - Spielbar | Kern-Loop funktioniert | |
| 2 - Komplett | Alle Features | |
| 3 - Schön | Polish & Juice | |
| 4 - Fertig | Abgabe-Ready | |
