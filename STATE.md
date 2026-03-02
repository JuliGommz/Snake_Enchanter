# PROJECT STATE - Snake Enchanter

**Letzte Aktualisierung:** 2026-03-03

---

## QUICK START

**Branch:** `main`
**Letzter Commit:** `b98d476` — chore: Unity reimport — new meta files + project settings update
**Remote:** https://github.com/JuliGommz/Snake_Enchanter.git — ✅ Pushed (up to date)
**Working Tree:** 1 modified (ProjectSettings.asset — Unity-intern, harmlos), build-Dateien untracked (gitignored)

**Milestone:** v1.0 Submission Ready
**Status:** Phase 12 ✅ + Phase 13 ✅ — ABGABE-STRUKTUR FERTIG

---

## WAS HEUTE GEMACHT WURDE (2026-03-03)

### Phase 13: Repo-Abgabe-Struktur — COMPLETE ✅
- Unity-Projekt nach `Arbeitsdateien/GME_Julian_Gomez/` verschoben (Python-Copy + git)
- `Konzeption/GDD_v1.8_SnakeEnchanter.pdf` → git-tracked ✅
- `Projektplan.pdf` → Root, git-tracked ✅
- `Arbeitsprotokoll_Julian_Gomez.pdf` → Root, git-tracked ✅
- `Anwendung/ReadMe.txt` → git-tracked ✅
- `Trailer/` → Placeholder erstellt ✅
- `.gitignore` angepasst (root-anchored `/Assets/` etc.)

### Bug Fixes (committed)
- `fix: Escape releases cursor in Editor, quits only in build` (90a75dc)
  - QuitController: `EditorApplication.isPlaying = false` entfernt
  - Editor: Escape = nur Cursor freigeben, Play Mode läuft weiter
  - Build: Escape = Application.Quit() wie gewollt
- `fix: _escapeToMainMenu false in GameLevel scene` (9ba4a50)
  - War auf `true` gesetzt → Escape ging zu Main Menu statt zu beenden
- `fix: CS0414 — _sliderHeight entfernt aus TuneSliderUI` (226e456)
  - Feld war [SerializeField] aber nie gelesen

### Post-Restructuring Fixes
- Große gitignorierte Assets (mp3, tif, psd, hdr) von root `Assets/` nach
  `Arbeitsdateien/GME_Julian_Gomez/Assets/` kopiert (59 Dateien)
- Gelöschte .meta Dateien aus git wiederhergestellt (GUIDs erhalten)
- PDFs in Arbeitsdateien/Documentation/ jetzt auch git-tracked

---

## NÄCHSTER SCHRITT (morgen direkt starten)

### 1. NEUEN BUILD ERSTELLEN ⚠️ PFLICHT
Der aktuelle Build in `Anwendung/` ist veraltet:
- Escape-Bug war noch aktiv (ging zu Main Menu)
- `_escapeToMainMenu` Fix ist nur im Quellcode, noch nicht im Build

**Vorgehen:**
1. Unity öffnen von `Arbeitsdateien/GME_Julian_Gomez/`
2. File → Build Settings → Build
3. Ziel: `Anwendung/Snake_Enchanter.exe` (direkt in Anwendung/ bauen)
4. Build testen: Starten → spielen → Escape → muss Fenster schließen

### 2. TRAILER ⏳
- `Trailer/Trailer.mlt` existiert → Kdenlive-Projekt angelegt
- MP4 exportieren: mind. 1920×1080, Dateiname `SnakeEnchanter_Trailer.mp4`
- In `Trailer/` ablegen (gitignore-Exception bereits vorhanden)

### 3. GDD Illustrationen prüfen
- `Konzeption/GDD_v1.8_SnakeEnchanter.pdf` öffnen
- Enthält es Screenshots/Bilder? (Projektauftrag fordert "illustrierte Konzepte")
- Falls nicht: Screenshots in DOCX einfügen → PDF neu exportieren

---

## ABGABE-CHECKLISTE

```
- [x] GDD v1.8 als PDF (Konzeption/)
- [ ] GDD v1.8 illustriert (Screenshots drin?)
- [x] Projektplan.pdf (Root)
- [x] Arbeitsprotokoll.pdf (Root)
- [x] Unity-Projekt in Arbeitsdateien/GME_Julian_Gomez/
- [x] Backend in Arbeitsdateien/GME_Julian_Gomez/backend/
- [x] ReadMe.txt in Anwendung/
- [x] Build in Anwendung/ (liegt lokal, gitignored)
- [ ] Neuer Build (Escape-Fix!)
- [ ] Build startet fehlerfrei + Escape beendet .exe
- [ ] Trailer (Trailer/SnakeEnchanter_Trailer.mp4)
- [x] Alle 4 HTTP-Methoden (GET/POST/PUT/DELETE)
- [x] GitHub up to date
```

---

## UNITY ÖFFNEN

**Projekt-Pfad (Unity Hub):**
`C:\Users\Teilnehmer\Desktop\Schule\PRG\Unity_Projects\Snake_Enchanter\Arbeitsdateien\GME_Julian_Gomez\`

Falls nicht in Unity Hub: Remove → Re-Add mit obigem Pfad.

---

## REPO-STRUKTUR (Abgabe-konform)

```
Snake_Enchanter/                        ← Repo-Root
├── Konzeption/
│   └── GDD_v1.8_SnakeEnchanter.pdf    ✅ git-tracked
├── Arbeitsdateien/GME_Julian_Gomez/
│   ├── Assets/                         ✅ Unity-Projekt
│   ├── ProjectSettings/               ✅
│   ├── Packages/                      ✅
│   └── backend/                       ✅ Node.js REST API
├── Anwendung/
│   ├── ReadMe.txt                     ✅ git-tracked
│   ├── Snake_Enchanter.exe            ⚠️  lokal vorhanden, VERALTET → neu bauen
│   └── [Build-Dateien]                gitignored, lokal vorhanden
├── Trailer/
│   ├── .gitkeep                       ✅
│   └── Trailer.mlt                    ⏳ Kdenlive-Projekt (MP4 noch exportieren)
├── Projektplan.pdf                    ✅ git-tracked
└── Arbeitsprotokoll_Julian_Gomez.pdf  ✅ git-tracked
```

---

## GIT STATUS (aktuell)

```
Branch: main
Letzter Commit: b98d476
Remote: GitHub — vollständig gepusht ✅
Unstaged: ProjectSettings.asset (Unity-intern, kein Handlungsbedarf)
Untracked: Anwendung/[Build-Dateien] (gitignored — korrekt)
           Trailer/Trailer.mlt (neues Kdenlive-Projekt)
           GDD_v1.7_SnakeEnchanter.txt.meta (altes Meta, unwichtig)
```

---

## BEKANNTE WARNINGS (harmlos)

- `Color primaries 0 unknown` — MP4-Aufnahmen in Documentation/Media/Recordings/
  → Nicht-Spiel-Assets, Unity-Import-Warning, kein Einfluss auf Build
- `UDP port 56906 VS/Unity messaging` — VS Integration Port-Konflikt
  → Harmlos, anderer Prozess hält den Port

---

## WAS FUNKTIONIERT

- Player Controller v1.9 (New Input System, Crouch, Cinemachine v3.x)
- Health System v1.5 (Drain, Events, Death Animations, heal-on-charm, shield intercept)
- Tune System (TuneController v3.3 — 3-Tune Array, Cooldown)
- Snake AI v2.1 (NavMesh, Entranced→Dazed, CancelInvoke)
- SpellHUDController v1.1 (dynamic HUD, cooldown overlay, range indicator)
- ShieldComponent v1.1 (duration from TuneConfig, blocks next attack)
- MusicManager v1.1 (scene-based, gameplay alternation)
- Canvas UI: HealthBarUI v3.1 + TuneSliderUI v2.2 + ActiveEffectsController v1.0
- Win Condition (ExitTrigger) + Full Game Loop (GameManager v1.5)
- Backend REST API (Node.js + Express + MySQL) — localhost:3000
- MainMenu Scene (Simple/Advanced/Quit) + ResultScreen
- QuitController: Escape = Cursor freigeben (Editor) / Quit (Build)

---

## REGELN (NICHT VERHANDELBAR)

- Input System: NUR New Input System (`UnityEngine.InputSystem`) — KEIN Legacy Input
- Kamera: Cinemachine besitzt Position — NIEMALS per Script überschreiben
- Git: Feature Branches `feature/<name>` — Ein Feature = Ein Branch — nach Merge löschen
