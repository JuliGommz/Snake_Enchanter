====================================================================
 Snake Enchanter - Backend Server
 PIP-3 Theme B | SRH Fachschulen | Entwickler: Julian Gomez
====================================================================

VORAUSSETZUNGEN
---------------
- Node.js v18 oder hoeher muss installiert sein
  Download: https://nodejs.org (LTS-Version empfohlen)

- Das Unity-Projekt muss in Unity 6 (6000.0.62f1) geoeffnet sein
  ODER der fertige Build muss verfuegbar sein


BACKEND STARTEN (Schritt-fuer-Schritt)
---------------------------------------
1. Diesen Ordner (backend/) im Explorer oeffnen

2. START_SERVER.bat doppelklicken
   -> Installiert automatisch alle Abhaengigkeiten (npm install)
   -> Startet den Server auf http://localhost:3000

3. Warten bis die Meldung erscheint:
   "Snake Enchanter API running at http://localhost:3000"

4. Server-Fenster OFFEN lassen (minimieren ist OK)

5. Spiel in Unity starten (Play-Button) oder fertigen Build ausfuehren


SERVER TESTEN (optional)
--------------------------
Im Browser oeffnen:
  http://localhost:3000/api/health
  -> Antwort: {"status":"ok","message":"Snake Enchanter API running"}

  http://localhost:3000/api/player-stats
  -> Aggregierte Statistiken aller gespielten Sessions

  http://localhost:3000/api/leaderboard?mode=simple
  -> Top 10 Bestzeiten (Simple-Modus)


ENDPUNKTE
----------
POST /api/game-session         - Session-Daten nach jedem Spiel
GET  /api/leaderboard          - Bestenliste (?mode=simple oder advanced)
GET  /api/player-stats         - Gesamtstatistiken
GET  /api/health               - Server-Status


DATENBANK
----------
- SQLite-Datenbank wird automatisch erstellt: backend/snake_enchanter.db
- Alle Spielsessions werden gespeichert
- Datenbank kann mit jedem SQLite-Viewer geoeffnet werden


TECHNOLOGIE-STACK
------------------
- Runtime:   Node.js
- Framework: Express v5
- Datenbank: SQLite (better-sqlite3)
- Protokoll: HTTP/REST, JSON


SERVER BEENDEN
---------------
Im Server-Fenster: STRG + C druecken


====================================================================
