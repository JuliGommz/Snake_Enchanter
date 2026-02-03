# Abgabe-Export Anleitung

## Am Abgabetag ausführen:

### 1. PDFs erstellen
- `Assets/Documentation/Projektplan_SnakeEnchanter.md` → PDF exportieren
- `Assets/Documentation/Arbeitsprotokoll_Julian_Gomez.md` → PDF exportieren
- `Assets/Documentation/GDD/GDD_v1.3_SnakeEnchanter.txt` → Finale Version als PDF

### 2. Build erstellen
- Unity → File → Build Settings → Windows
- Output: `Builds/SnakeEnchanter/`

### 3. ZIP-Struktur erstellen

```
GruppenNr_Gomez/
├── Konzeption/
│   └── GDD_SnakeEnchanter_Final.pdf
├── Arbeitsdateien/
│   └── GME_Julian_Gomez/
│       └── [Gesamtes Assets/ Verzeichnis]
├── Anwendung/
│   ├── SnakeEnchanter.exe
│   ├── SnakeEnchanter_Data/
│   ├── UnityPlayer.dll
│   └── ReadMe.txt
├── Trailer/
│   └── SnakeEnchanter_Trailer.mp4
├── Projektplan.pdf
└── Arbeitsprotokoll_Julian_Gomez.pdf
```

### 4. Checkliste vor Abgabe
- [ ] GDD ist aktuell und illustriert
- [ ] Projektplan vollständig ausgefüllt
- [ ] Arbeitsprotokoll alle Tage dokumentiert
- [ ] Build startet und läuft fehlerfrei
- [ ] Trailer vorhanden (mind. 1920x1080, MP4)
- [ ] ReadMe.txt mit Steuerung enthalten
- [ ] ZIP-Dateiname korrekt: `GruppenNr_Gomez.zip`
