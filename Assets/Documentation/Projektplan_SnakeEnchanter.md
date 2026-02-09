# Projektplan - Snake Enchanter
**Projekt:** PIP-3 Theme B - Snake Enchanter
**Teilnehmer:** Julian Gomez
**Zeitraum:** 03.02.2026 – ~03.03.2026

---

## Meilensteine

| Meilenstein | Ziel-Datum | Status |
|-------------|------------|--------|
| GDD v1 Abgabe | 27.01.2026 | ✅ Erledigt |
| **Phase 1: Spielbar** | ~07.02 | ⏳ |
| Fortschritts-Präsentation 1 | ~08.02 | ⏳ |
| **Phase 2: Komplett** | ~14.02 | ⏳ |
| GDD v2 + Making-of | ~17.02 | ⏳ |
| **Phase 3: Schön** | ~21.02 | ⏳ |
| Fortschritts-Präsentation 2 | ~24.02 | ⏳ |
| **Phase 4: Fertig** | ~03.03 | ⏳ |
| Finale Abgabe | ~03.03 | ⏳ |

---

## Phase 1: SPIELBAR (~3-4 Tage)
> "Ich kann durch einen Raum laufen und eine Schlange wegcharmen"

| Nr | Aufgabe | Status |
|----|---------|--------|
| 1.1 | Unity Projekt Setup | ✅ |
| 1.2 | Git/GitHub Setup | ✅ |
| 1.3 | Dokumentation & Struktur | ✅ |
| 1.4 | Player Controller (WASD + Kamera) | ✅ v1.7 Cinemachine |
| 1.5 | Greybox Level (1 Raum) | ✅ Cave Map |
| 1.6 | Tune Input (1 Taste halten → Timer) | ✅ ADR-008 Slider |
| 1.7 | Timing Window (Erfolg/Fail) | ✅ Triggerzone |
| 1.8 | Health System (HP, Drain, Damage) | ✅ v1.2.1 |
| 1.9 | Win/Lose Conditions | ✅ ExitTrigger |

**Done when:** Spielbarer Loop mit Cubes/Capsules

---

## Phase 2: KOMPLETT (~4-5 Tage)
> "Alle Features drin, sieht noch rough aus"

| Nr | Aufgabe | Status |
|----|---------|--------|
| 2.1 | Alle 3 Tunes (Move, Sleep, Attack) | ⏳ |
| 2.2 | Snake State Machine | ⏳ |
| 2.3 | Toon Snakes importieren | ⏳ |
| 2.4 | Snake Animationen einbinden | ⏳ |
| 2.5 | 3 Areas (Tutorial → Mitte → Finale) | ⏳ |
| 2.6 | Game States (Menu, Playing, Paused, End) | ⏳ |
| 2.7 | Main Menu + Result Screen | ⏳ |
| 2.8 | Simple + Advanced Mode | ⏳ |
| 2.9 | Tune 4 (Freeze) für Advanced | ⏳ |
| 2.10 | Backend Setup + API | ⏳ |
| 2.11 | Session-Stats senden | ⏳ |

**Done when:** Feature-Complete, alle Anforderungen erfüllt

---

## Phase 3: SCHÖN (~3-4 Tage)
> "Fühlt sich gut an, sieht anständig aus"

| Nr | Aufgabe | Status |
|----|---------|--------|
| 3.1 | Flöten-Melodien einbinden | ⏳ |
| 3.2 | Snake SFX (Hiss, Bite, Sleep) | ⏳ |
| 3.3 | UI Sounds | ⏳ |
| 3.4 | Ambient Music | ⏳ |
| 3.5 | Visual Feedback (Particles, Shake) | ⏳ |
| 3.6 | Health Bar Polish | ⏳ |
| 3.7 | Timing Meter Polish | ⏳ |
| 3.8 | Level Polish (Lighting, Props) | ⏳ |
| 3.9 | Low HP Feedback (Vignette, Heartbeat) | ⏳ |

**Done when:** Juice & Polish fertig

---

## Phase 4: FERTIG (~2-3 Tage)
> "Abgabe-Ready"

| Nr | Aufgabe | Status |
|----|---------|--------|
| 4.1 | Bug Fixing | ⏳ |
| 4.2 | Balancing (HP, Timing, Drain) | ⏳ |
| 4.3 | Test auf Schul-Laptops | ⏳ |
| 4.4 | Trailer produzieren | ⏳ |
| 4.5 | GDD finalisieren | ⏳ |
| 4.6 | Final Build | ⏳ |
| 4.7 | ZIP packen | ⏳ |
| 4.8 | Präsentation vorbereiten | ⏳ |

**Done when:** Alles abgabebereit

---

## Risiken & Mitigationen

| Risiko | Mitigation |
|--------|------------|
| Timing-System komplex | Phase 1 mit simplem Timer, später verfeinern |
| Snake AI Probleme | Einfache State Machine, keine Pathfinding |
| Zeitdruck | Jede Phase ist abgebbar - Scope reduzieren wenn nötig |
| Backend-Integration | Mock-API während Entwicklung, echtes Backend später |

---

## Regeln

1. **Keine Phase anfangen bevor vorherige DONE ist**
2. **Täglich:** Arbeitsprotokoll + Screenshot + Commit
3. **Bei Problemen:** Scope reduzieren, nicht Zeit verlängern
