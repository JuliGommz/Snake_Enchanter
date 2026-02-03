# Snake Enchanter - MVP Phasen
**Prinzip:** 4 Phasen, jede ist abgebbar. Mach fertig, dann weiter.

---

## Phase 1: SPIELBAR (3-4 Tage)
> "Ich kann durch einen Raum laufen und eine Schlange wegcharmen"

**Done when:**
- Player bewegt sich (WASD + Kamera)
- 1 Taste halten → Timer läuft → Loslassen = Erfolg/Fail
- Erfolg = Cube verschwindet, HP +10
- Fail = HP -15
- HP = 0 → Game Over
- Exit erreichen → Win

**Assets:** Alles Cubes/Capsules. Kein Polish.

**Abgabe-Wert:** Kern-Mechanik funktioniert. ⭐⭐

---

## Phase 2: KOMPLETT (4-5 Tage)
> "Das Spiel hat alle Features, sieht aber noch rough aus"

**Done when:**
- 3 Tunes funktionieren (Move, Sleep, Attack)
- 3 Areas (Tutorial → Mitte → Finale)
- Echte Snake-Models + Animationen
- Main Menu → Game → Result Screen
- Simple + Advanced Mode wählbar
- Backend sendet Session-Daten

**Assets:** Toon Snakes, Blockout-Level. Minimal UI.

**Abgabe-Wert:** Feature-Complete Prototyp. ⭐⭐⭐

---

## Phase 3: SCHÖN (3-4 Tage)
> "Das Spiel fühlt sich gut an und sieht anständig aus"

**Done when:**
- Audio: Flöten-Melodien, Snake-Sounds, Ambient
- Visual Feedback: Particles, Screen Shake, Vignette
- UI Polish: Animierte Health Bar, schöne Menus
- Level Polish: Lighting, Props, Exit-Fenster mit Glow
- Timing Meter sieht gut aus

**Abgabe-Wert:** Solides Indie-Game. ⭐⭐⭐⭐

---

## Phase 4: FERTIG (2-3 Tage)
> "Abgabe-Ready mit Trailer"

**Done when:**
- Bugs gefixt
- Balancing getestet (HP Drain, Timing Windows)
- Trailer produziert
- Build getestet auf Schul-Laptops
- Dokumentation finalisiert
- ZIP gepackt

**Abgabe-Wert:** Polished Game. ⭐⭐⭐⭐⭐

---

## Dein Rhythmus

```
Phase 1: Diese Woche     → Spielbar
Phase 2: Nächste Woche   → Komplett
Phase 3: Woche 3         → Schön
Phase 4: Letzte Tage     → Fertig
```

**Regel:** Keine Phase anfangen bevor die vorherige DONE ist.

---

## Heute starten mit Phase 1

Nächste Aktion:
1. [ ] PlayerController.cs - WASD + Mouse Look
2. [ ] Greybox Scene - Boden, Wände, 1 Snake-Cube, 1 Exit-Cube
3. [ ] Spielen, iterieren, fertig machen
