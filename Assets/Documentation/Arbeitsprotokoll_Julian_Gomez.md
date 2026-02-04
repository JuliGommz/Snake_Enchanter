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
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-05_.png`

**Notizen:**


---

### 06.02.2026 (Donnerstag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-06_.png`

**Notizen:**


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
