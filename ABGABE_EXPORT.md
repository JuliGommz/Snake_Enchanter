# Abgabe-Export Anleitung

Letzter Stand: 03.03.2026

## Repo-Struktur (entspricht Projektauftrag Seite 18)

```
Snake_Enchanter/                    ← Repo-Root = Abgabe-Verzeichnis
├── Konzeption/
│   └── GDD_v1.8_SnakeEnchanter.pdf        ✓ in git
├── Arbeitsdateien/
│   └── GME_Julian_Gomez/
│       ├── Assets/                         ✓ Unity-Projekt (Scripts, Scenes, Art)
│       ├── ProjectSettings/               ✓
│       ├── Packages/                      ✓
│       └── backend/                       ✓ Node.js REST API
├── Anwendung/
│   ├── ReadMe.txt                         ✓ in git
│   ├── Snake_Enchanter.exe                ⏳ manuell einfügen (aus Builds/)
│   ├── Snake_Enchanter_Data/              ⏳ manuell einfügen
│   ├── UnityPlayer.dll                    ⏳ manuell einfügen
│   ├── MonoBleedingEdge/                  ⏳ manuell einfügen
│   └── D3D12/                             ⏳ manuell einfügen
├── Trailer/
│   └── SnakeEnchanter_Trailer.mp4         ⏳ noch aufnehmen
├── Projektplan.pdf                        ✓ in git
└── Arbeitsprotokoll_Julian_Gomez.pdf      ✓ in git
```

---

## Status-Übersicht

| Element | Status | Pfad im Repo |
|---------|--------|--------------|
| GDD v1.8 (PDF) | ✓ Fertig | `Konzeption/GDD_v1.8_SnakeEnchanter.pdf` |
| Projektplan (PDF) | ✓ Fertig | `Projektplan.pdf` |
| Arbeitsprotokoll (PDF) | ✓ Fertig | `Arbeitsprotokoll_Julian_Gomez.pdf` |
| Unity-Projekt (Assets) | ✓ Fertig | `Arbeitsdateien/GME_Julian_Gomez/Assets/` |
| Backend | ✓ Fertig | `Arbeitsdateien/GME_Julian_Gomez/backend/` |
| ReadMe.txt | ✓ Fertig | `Anwendung/ReadMe.txt` |
| Build (.exe + Data) | ⏳ Manuell | Aus `Builds/` in `Anwendung/` kopieren |
| Trailer | ❌ Fehlt | `Trailer/SnakeEnchanter_Trailer.mp4` — noch aufnehmen |

---

## Ausstehende Schritte

### 1. Build-Dateien nach Anwendung/ kopieren
```
Builds/Snake_Enchanter.exe        → Anwendung/Snake_Enchanter.exe
Builds/Snake_Enchanter_Data/      → Anwendung/Snake_Enchanter_Data/
Builds/UnityPlayer.dll            → Anwendung/UnityPlayer.dll
Builds/MonoBleedingEdge/          → Anwendung/MonoBleedingEdge/
Builds/D3D12/                     → Anwendung/D3D12/
```
⚠️ Build-Binaries sind gitignored (zu groß) — müssen MANUELL kopiert werden.

### 2. Unity Hub neu konfigurieren
Nach Repo-Umstrukturierung muss Unity Hub neu zeigen:
- Projekt entfernen → Re-Add von: `Arbeitsdateien/GME_Julian_Gomez/`
- Unity öffnet dann aus dem neuen Pfad (Assets/ ist dort)

### 3. Trailer produzieren
- MP4, mind. 1920×1080
- Dateiname: `SnakeEnchanter_Trailer.mp4`
- Ablegen in: `Trailer/`
- Hinweis: `*.mp4` ist normalerweise gitignored → Exception für `Trailer/SnakeEnchanter_Trailer.mp4` ist bereits in `.gitignore`

### 4. GDD auf Illustrationen prüfen
- Projektauftrag fordert "vollständige, **illustrierte** Konzepte"
- `GDD_v1.8_SnakeEnchanter.pdf` prüfen: enthält Bilder/Screenshots?
- Falls nicht: Screenshots manuell ins DOCX einfügen → PDF neu exportieren

---

## Checkliste vor Abgabe

- [x] GDD v1.8 als PDF vorhanden (`Konzeption/`)
- [ ] GDD v1.8 illustriert (Screenshots/Bilder drin?)
- [x] Projektplan als PDF (`Projektplan.pdf`)
- [x] Arbeitsprotokoll als PDF (`Arbeitsprotokoll_Julian_Gomez.pdf`)
- [x] Unity-Projekt in `Arbeitsdateien/GME_Julian_Gomez/`
- [x] Backend in `Arbeitsdateien/GME_Julian_Gomez/backend/`
- [x] ReadMe.txt in `Anwendung/`
- [ ] Build-Dateien in `Anwendung/` (manuell kopieren)
- [ ] Build startet fehlerfrei
- [ ] Trailer `Trailer/SnakeEnchanter_Trailer.mp4` vorhanden
- [x] Alle 4 HTTP-Methoden implementiert (GET/POST/PUT/DELETE)
- [x] Git-Historie nachvollziehbar (Commits täglich)
