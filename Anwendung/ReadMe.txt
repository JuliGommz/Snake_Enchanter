====================================================================
  Snake Enchanter
  PIP-3 Theme B – SRH Fachschulen
  Entwickler: Julian Gomez
  Version: 1.0 | Datum: 03.03.2026
====================================================================

SPIELBESCHREIBUNG
-----------------
Snake Enchanter ist ein First-Person-Präzisionsspiel. Der Spieler
ist ein verwundeter Krieger in einer verfallenen Ruine und muss
den einzigen Ausweg – ein Fenster am Ende des Levels – erreichen.
Da er zu geschwächt zum Kämpfen ist, nutzt er magische Melodien,
um Schlangen zu kommandieren. Die Kernmechanik: Taste halten,
Slider aufladen, zum richtigen Zeitpunkt loslassen.

SYSTEMANFORDERUNGEN
-------------------
Betriebssystem: Windows 10 / 11 (64-bit)
Grafik:         DirectX 12-kompatible GPU
Eingabe:        Tastatur + Maus
Auflösung:      1920x1080 (Ultrawide unterstützt)

BACKEND-SETUP (PFLICHT für Datenbankfunktion)
----------------------------------------------
Das Spiel speichert Spielstatistiken in einer MySQL-Datenbank.
Ohne laufendes Backend funktioniert das Spiel weiterhin –
Datenbankfunktionen werden stillschweigend übersprungen.

Voraussetzungen:
  1. XAMPP installiert (https://www.apachefriends.org)
  2. Node.js installiert (https://nodejs.org, Version 18+)

Backend starten:
  1. XAMPP Control Panel öffnen → MySQL starten
  2. Doppelklick auf:  Arbeitsdateien\GME_Julian_Gomez\backend\START_SERVER.bat
  3. Fenster offen lassen während das Spiel läuft
  4. Prüfen: http://localhost:3000/api/health → {"status":"ok"}

  Hinweis: Datenbank wird beim ersten Start automatisch angelegt.
           Kein phpMyAdmin, kein manueller Import nötig.

API-Endpunkte (implementiert):
  POST   /api/game-session         – Session anlegen (Spielstart)
  PUT    /api/game-session/:id     – Session aktualisieren (Spielende)
  DELETE /api/game-session/:id     – Session löschen (Ergebnis-Screen)
  GET    /api/leaderboard          – Bestenliste abrufen
  GET    /api/player-stats         – Gesamtstatistiken abrufen

SPIEL STARTEN
-------------
  Snake_Enchanter.exe starten (kein Installer nötig)
  Das Spiel startet direkt im Hauptmenü.

STEUERUNG
---------
  Bewegung:       WASD
  Kamera:         Maus
  Ducken:         Linke Strg (gehalten)
  Tune 1 – Move:  Taste 1 halten, in Triggerzone loslassen
                  → Schlange weicht aus | bei Erfolg: +HP
  Tune 2 – Daze:  Taste 2 halten, in Triggerzone loslassen
                  → Schlange benommen (11 Sek.) | bei Erfolg: +HP
  Tune 3 – Shield: Taste 3 halten, in Triggerzone loslassen
                  → Schutzschild (8 Sek., blockt nächsten Angriff)
  Screens weiter: Beliebige Taste (Story-Intro, Ending Story)
  Beenden:        Escape

SPIELMODI
---------
  Simple Escape:   Längere Timing-Fenster, langsamerer HP-Drain
  Advanced Escape: Schnellerer Drain (×2.17), höherer Schlangenschaden

SPIELPRINZIP – SLIDER-MECHANIK
--------------------------------
  1. Taste HALTEN  → Slider bewegt sich von 0% → 100%
  2. Taste LOSLASSEN → Position wird gegen Triggerzone geprüft
     - Zu früh (vor Zone): Kein Effekt, kein Schaden (sicheres Fail)
     - In Zone:            Erfolg! Schlange gehorcht, +HP (Move/Daze)
     - Zu spät (nach Zone): Schlange greift an → HP-Verlust

GESUNDHEIT
----------
  Startleben:         30/100 HP (verwundeter Krieger)
  Passiver Drain:     Simple ~0.115 HP/s | Advanced ~0.25 HP/s
  Heilung:            Nur bei erfolgreichen Move / Daze Casts
  Tod:                HP ≤ 0 → Sofortiges Game Over

SIEGBEDINGUNG:  Fenster am Ende des Levels mit HP > 0 erreichen
NIEDERLAGE:     HP fällt auf 0

====================================================================
  Kontakt: Julian Gomez | PIP-3 Theme B | SRH Fachschulen 2026
====================================================================
