# Snake Enchanter – README

**Entwickler:** Julian Gomez
**Kurs:** PIP-3 Theme B – SRH Fachschulen
**Technologie:** Unity 6000.0.62f1 (URP) + Node.js REST API
**Version:** v1.0
**Datum:** 03.03.2026

---

## Kurzbeschreibung

Snake Enchanter ist ein First-Person-Präzisionsspiel. Der Spieler ist ein verwundeter Krieger in einer verfallenen Ruine und muss den einzigen Ausweg – ein Fenster am Ende des Levels – erreichen. Da er zu geschwächt zum Kämpfen ist, nutzt er magische Melodien, um Schlangen zu kommandieren.

Kernmechanik: Taste halten → Slider aufladen → zum richtigen Zeitpunkt loslassen. Zu früh = sicheres Fail. In der Triggerzone = Erfolg. Zu spät = Schlange greift an.

---

## Anleitung zum Starten

### Schritt 1: Backend starten (PFLICHT für Datenbankbewertung)

Das Spiel speichert Spielsessions automatisch in einer MySQL-Datenbank über eine Node.js REST API. **Ohne laufendes Backend läuft das Spiel weiter, aber alle API-Endpunkte werden nicht ausgeführt und können nicht bewertet werden.**

**Voraussetzungen (einmalige Installation):**
- XAMPP: https://www.apachefriends.org (Apache + MySQL)
- Node.js v18+: https://nodejs.org

**Backend starten (vor jedem Spielstart):**
1. XAMPP Control Panel öffnen → **MySQL starten**
2. Doppelklick: `Arbeitsdateien\GME_Julian_Gomez\backend\START_SERVER.bat`
3. Fenster offen lassen während das Spiel läuft
4. Verbindung prüfen: http://localhost:3000/api/health → `{"status":"ok"}`

> **Datenbank wird automatisch angelegt** — kein phpMyAdmin, kein manueller Import nötig.

---

### Schritt 2: Spiel starten

> **Hinweis:** Der Build (`Snake_Enchanter.exe`) ist nicht im Git-Repository enthalten, da die Dateigröße (~500 MB) das GitHub-Limit überschreitet. Der Build wird auf Anfrage separat eingereicht (USB / Download-Link) und befindet sich dann im Ordner `Anwendung\`.

```
Anwendung\Snake_Enchanter.exe  ← Doppelklick
```

Kein Installer nötig. Das Spiel startet direkt im Hauptmenü.

**Systemanforderungen:** Windows 10/11 (64-bit), DirectX 12-kompatible GPU, Tastatur + Maus, 1920×1080

---

### Schritt 3: Beide Spielmodi testen

Im Hauptmenü stehen zwei Modi zur Auswahl:

| Modus | Timing-Fenster | HP-Drain | Schlangenschaden |
|-------|---------------|----------|-----------------|
| **Simple Escape** | Breiter (leichter) | ~0.115 HP/s | Standard |
| **Advanced Escape** | Enger (schwieriger) | ~0.25 HP/s (×2.17) | +15% |

→ **Beide Modi separat starten**, um alle Schwierigkeitsunterschiede zu bewerten.

---

### Schritt 4: HTTP-Methoden beobachten

Alle 4 HTTP-Methoden feuern automatisch – kein manueller API-Aufruf nötig:

| Methode | Endpunkt | Wann |
|---------|----------|------|
| `POST` | `/api/game-session` | Beim Spielstart (Modus gewählt → Spielfeld geladen) |
| `PUT` | `/api/game-session/:id` | Bei Spielende (Sieg oder Niederlage) |
| `DELETE` | `/api/game-session/:id` | Ergebnis-Screen verlassen (Retry oder Hauptmenü) |
| `GET` | `/api/leaderboard` | Bestenliste auf dem Ergebnis-Screen |
| `GET` | `/api/player-stats` | Spielerstatistiken auf dem Ergebnis-Screen |

**Überprüfung:** Im `START_SERVER.bat`-Fenster werden alle eingehenden Requests live geloggt.

---

## Steuerung

| Aktion | Taste |
|--------|-------|
| Bewegung | WASD |
| Kamera | Maus |
| Ducken | Linke Strg (gehalten) |
| Tune 1 – Move | `1` halten → loslassen in Triggerzone → Schlange weicht aus, +HP |
| Tune 2 – Daze | `2` halten → loslassen in Triggerzone → Schlange benommen (11s), +HP |
| Tune 3 – Shield | `3` halten → loslassen in Triggerzone → Schutzschild (8s, blockt Angriff) |
| Screens weiter | Beliebige Taste (Story-Intro, Ending) |
| Beenden | Escape |

---

## Spielprinzip – Slider-Mechanik

```
[Gelb: Safe Zone] → [Orange: Triggerzone] → [Grau: Danger Zone]
        ↑                    ↑                       ↑
   Zu früh loslassen    Erfolg! +HP           Zu spät → Angriff
```

1. Taste **halten** → Slider bewegt sich von 0% → 100%
2. Taste **loslassen** → Position wird gegen Triggerzone geprüft
   - Vor Zone: Kein Effekt, kein Schaden (sicheres Fail)
   - In Zone: Erfolg – Schlange gehorcht, +HP (Move/Daze)
   - Nach Zone: Schlange greift an → HP-Verlust

**Gesundheit:** Start 30/100 HP (verwundeter Krieger). Passiver Drain läuft kontinuierlich. Heilung nur bei erfolgreichem Move/Daze.

---

## Technischer Überblick

### Architektur

```
MainMenu.unity → (Modus-Auswahl via PlayerPrefs) → GameLevel.unity
                                                         ├── GameManager (State Machine)
                                                         ├── PlayerController + HealthSystem
                                                         ├── TuneController (Slider-Logik)
                                                         ├── SnakeAI (7-State Machine)
                                                         └── ApiManager (REST API, fail-silent)
```

### Schlüssel-Scripts

| Script | Beschreibung |
|--------|-------------|
| `TuneController.cs` | Slider-System: Hold/Release, Zone-Check, 3-Tune-Array |
| `SnakeAI.cs` | 7-State Machine: Idle → Patrol → Chase → Attack → Entranced → Dazed → Dead |
| `HealthSystem.cs` | HP-Drain, Events, Shield-Routing, Death |
| `GameManager.cs` | Game-State: MainMenu → Playing → Won/Lost |
| `ApiManager.cs` | POST/PUT/DELETE/GET gegen Node.js Backend (fail-silent) |
| `TuneSliderUI.cs` | Genshin-style segmentierter Slider (Safe/Zone/Danger Zonen) |

### Backend (Node.js REST API)

```
backend/
├── server.js          – Express API, alle Endpunkte
├── schema.sql         – Datenbankschema (snake_enchanter DB)
└── START_SERVER.bat   – Startskript für Windows
```

---

## Repo-Struktur

```
Snake_Enchanter/                          ← Repo-Root (= Abgabe-Verzeichnis)
├── Konzeption/
│   └── GDD_v1.8_SnakeEnchanter.pdf      ← Game Design Document
├── Arbeitsdateien/GME_Julian_Gomez/
│   ├── Assets/_Project/                 ← Unity Scripts, Scenes, Art
│   └── backend/                         ← Node.js REST API
├── Anwendung/
│   ├── Snake_Enchanter.exe              ← Spielstart hier
│   └── ReadMe.txt                       ← Spieler-Manual
├── Trailer/
│   └── SnakeEnchanter_Trailer.mp4
├── Projektplan.pdf
└── Arbeitsprotokoll_Julian_Gomez.pdf
```

---

## Entwicklungs-Attribution

**[HUMAN-AUTHORED]**
- Spielkonzept, Mechanik-Design, Balancing, Szenen-Aufbau
- Cave-Layout (ProBuilder/Polybrush), alle Inspector-Konfigurationen
- Backend-Architektur (REST API, Schema, Endpunkte)
- Tägliche Commit-Historie, Arbeitsprotokoll

**[AI-ASSISTED]**
- Segment-basierter Slider (TuneSliderUI) – menschlich reviewed und angepasst
- SnakeAI State Machine – menschlich reviewed und angepasst
- Dokumentationsstruktur (README, GDD-Abschnitte)

Jedes Script enthält im Header ein detailliertes Authorship-Tracking.

---

## Third-Party Assets

| Asset | Lizenz |
|-------|--------|
| Toon Snakes Pack (Meshtint Studio) | Asset Store Commercial |
| Caves Parts Set | Asset Store Commercial |
| Dwarven Expedition Pack | Asset Store Commercial |
| Steampunk UI Pack (Gentleland) | Asset Store Commercial |
| Pirate Pack | Asset Store Commercial |
| Unity Input System | Unity Companion License |
| TextMeshPro | Unity Package |
| Cinemachine | Unity Package |

---

## Kontakt

**Entwickler:** Julian Gomez
**Repository:** https://github.com/JuliGommz/Snake_Enchanter
**Kurs:** PIP-3 Theme B – SRH Fachschulen 2026
