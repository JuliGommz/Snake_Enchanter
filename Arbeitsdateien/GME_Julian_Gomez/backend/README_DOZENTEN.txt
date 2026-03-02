====================================================================
 Snake Enchanter - Backend Server
 PIP-3 Theme B | SRH Fachschulen | Entwickler: Julian Gomez
====================================================================

TECHNOLOGIE-STACK
------------------
- Runtime:   Node.js
- Framework: Express v5
- Datenbank: MySQL (via XAMPP)
- Protokoll: HTTP/REST, JSON

ENDPUNKTE
----------
POST /api/game-session         - Session-Daten nach jedem Spiel speichern
GET  /api/leaderboard          - Bestenliste (?mode=simple oder advanced)
GET  /api/player-stats         - Gesamtstatistiken aller Sessions
GET  /api/health               - Server-Status

====================================================================
 SCHRITT-FÜR-SCHRITT: Backend starten
====================================================================

SCHRITT 1 — XAMPP starten
--------------------------
1. XAMPP Control Panel öffnen
2. "Apache" starten (grün)
3. "MySQL" starten (grün)

SCHRITT 2 — Datenbank einrichten (einmalig)
-------------------------------------------
1. Browser öffnen: http://localhost/phpmyadmin
2. Oben auf "SQL" klicken
3. Inhalt der Datei "schema.sql" (im gleichen Ordner) einfügen
4. "OK" klicken
→ Datenbank "snake_enchanter" und Tabelle werden erstellt

SCHRITT 3 — Node.js installieren (einmalig, falls nicht vorhanden)
------------------------------------------------------------------
Download: https://nodejs.org (LTS-Version)
Prüfen: PowerShell öffnen → node --version

SCHRITT 4 — Server starten
---------------------------
PowerShell öffnen und eingeben:

  cd "C:\...\Snake_Enchanter\backend"
  npm install
  node server.js

Erfolg: "Snake Enchanter API running at http://localhost:3000"

SCHRITT 5 — Unity starten
--------------------------
Server-Fenster OFFEN lassen → Unity öffnen → Play drücken

====================================================================
 DATEN ÜBERPRÜFEN (phpMyAdmin)
====================================================================

1. http://localhost/phpmyadmin öffnen
2. Links: "snake_enchanter" → "game_sessions"
3. Oben: "Anzeigen" → alle gespeicherten Sessions sichtbar

Oder direkt im Browser:
  http://localhost:3000/api/player-stats   → Gesamtstatistik (JSON)
  http://localhost:3000/api/leaderboard    → Bestenliste (JSON)
  http://localhost:3000/api/health         → Server-Status

====================================================================
 SERVER BEENDEN
====================================================================
Im PowerShell-Fenster: STRG + C

====================================================================
