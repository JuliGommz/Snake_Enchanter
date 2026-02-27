@echo off
title Snake Enchanter - Backend Server
color 0A

echo ========================================
echo  Snake Enchanter Backend Server
echo  PIP-3 Theme B - SRH Fachschulen
echo  Entwickler: Julian Gomez
echo ========================================
echo.

:: In das Backend-Verzeichnis wechseln (relativ zur BAT-Datei)
cd /d "%~dp0"

:: Node.js pruefen
where node >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    color 0C
    echo FEHLER: Node.js wurde nicht gefunden!
    echo.
    echo Bitte installieren Sie Node.js von: https://nodejs.org
    echo Empfohlen: Version 18 oder hoeher
    echo.
    pause
    exit /b 1
)

echo Node.js gefunden:
node --version
echo.

:: Abhaengigkeiten installieren (nur falls noetig, idempotent)
echo Pruefe Abhaengigkeiten (npm install)...
call npm install --silent
if %ERRORLEVEL% NEQ 0 (
    color 0C
    echo.
    echo FEHLER: npm install fehlgeschlagen.
    echo Bitte pruefen Sie Ihre Internetverbindung.
    echo.
    pause
    exit /b 1
)

echo Abhaengigkeiten OK.
echo.
echo ========================================
echo  Server startet auf: http://localhost:3000
echo  Health-Check:        http://localhost:3000/api/health
echo.
echo  WICHTIG: Dieses Fenster offen lassen!
echo           Zum Beenden: STRG + C
echo ========================================
echo.

:: Server starten
node server.js

:: Nur erreicht wenn server.js beendet
echo.
echo Server wurde beendet.
pause
