# Abgabe-Export Anleitung

Letzter Stand: 02.03.2026

## Status-Übersicht

| Schritt | Status | Notiz |
|---------|--------|-------|
| GDD v1.8 (PDF) | ⚠️ Ausstehend | .txt ist v1.8 — .docx öffnen, anpassen, PDF exportieren |
| Projektplan (PDF) | ⚠️ Ausstehend | Projektplan_SnakeEnchanter.md → Word öffnen → PDF speichern |
| Arbeitsprotokoll (PDF) | ⚠️ Ausstehend | Arbeitsprotokoll_Julian_Gomez.docx → Als PDF speichern |
| Build | ✓ Fertig | Builds/ enthält Snake_Enchanter.exe + Data |
| ReadMe.txt | ✓ Fertig | Liegt in Builds/ReadMe.txt |
| Trailer | ❌ Fehlt | Muss noch aufgenommen/geschnitten werden (MP4, 1920x1080) |
| Backend | ✓ Fertig | Alle 4 HTTP-Methoden implementiert (GET/POST/PUT/DELETE) |

---

## Schritt-für-Schritt am Abgabetag

### 1. PDFs erstellen (manuell in Word / LibreOffice)

**GDD (v1.8):**
- `Assets/Documentation/GDD/GDD_v1.7_SnakeEnchanter.docx` öffnen
- Inhalt mit `GDD_v1.7_SnakeEnchanter.txt` (v1.8) abgleichen und aktualisieren
- Als PDF speichern → `GDD_v1.8_SnakeEnchanter.pdf`

**Projektplan:**
- `Assets/Documentation/Projektplan_SnakeEnchanter.md` in Word/LibreOffice öffnen
- Als PDF speichern → `Projektplan.pdf`

**Arbeitsprotokoll:**
- `Assets/Documentation/Arbeitsprotokoll_Julian_Gomez.docx` öffnen
- Als PDF speichern → `Arbeitsprotokoll_Julian_Gomez.pdf`

### 2. Build prüfen
- `Builds/Snake_Enchanter.exe` starten und Spielstart bestätigen
- Build ist bereits vorhanden — ggf. neuen Build machen:
  Unity → File → Build Settings → Windows → Build

### 3. Trailer aufnehmen
- MP4, mind. 1920x1080
- Dateiname: `SnakeEnchanter_Trailer.mp4`

### 4. ZIP-Paket erstellen

Ordnerstruktur des ZIP:

```
GruppenNr_Gomez/
├── Konzeption/
│   └── GDD_v1.8_SnakeEnchanter.pdf
├── Arbeitsdateien/
│   └── GME_Julian_Gomez/
│       └── [Gesamtes Assets/ Verzeichnis]
├── Anwendung/
│   ├── Snake_Enchanter.exe
│   ├── Snake_Enchanter_Data/
│   ├── UnityPlayer.dll
│   ├── MonoBleedingEdge/
│   ├── D3D12/
│   └── ReadMe.txt          ← liegt in Builds/ReadMe.txt
├── Trailer/
│   └── SnakeEnchanter_Trailer.mp4
├── Projektplan.pdf
└── Arbeitsprotokoll_Julian_Gomez.pdf
```

ZIP-Dateiname: `GruppenNr_Gomez.zip`
→ GruppenNr durch tatsächliche Gruppennummer ersetzen!

---

### 5. Checkliste vor Abgabe

- [ ] GDD v1.8 als PDF vorhanden und illustriert
- [ ] Projektplan als PDF vollständig ausgefüllt
- [ ] Arbeitsprotokoll als PDF — alle Tage dokumentiert
- [ ] Build startet fehlerfrei (Snake_Enchanter.exe testen)
- [ ] ReadMe.txt mit Steuerung und Backend-Setup enthalten
- [ ] Trailer vorhanden (MP4, mind. 1920x1080)
- [ ] backend/START_SERVER.bat im Arbeitsdateien-Ordner enthalten
- [ ] ZIP-Dateiname korrekt: `GruppenNr_Gomez.zip`
