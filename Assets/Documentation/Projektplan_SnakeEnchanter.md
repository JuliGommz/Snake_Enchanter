# Projektplan - Snake Enchanter
**Projekt:** PIP-3 Theme B - Snake Enchanter
**Teilnehmer:** Julian Gomez
**Zeitraum:** 03.02.2026 – ~03.03.2026

---

## Meilensteine

| Meilenstein | Ziel-Datum | Status |
|-------------|------------|--------|
| GDD v1 Abgabe | 27.01.2026 | ✅ Erledigt |
| **Phase 1: Spielbar** | ~07.02 | ✅ Erledigt |
| Fortschritts-Präsentation 1 | ~08.02 | ✅ Erledigt |
| **Phase 2: Komplett** | ~14.02 | ✅ Erledigt (15.02) |
| **v0.3: Bug Fixes & Stability** | ~16.02 | 🔄 In Arbeit (NavMesh) |
| GDD v2 + Making-of | ~17.02 | ⏳ |
| **Phase 3: Schön** | ~21.02 | ⏳ Nach v0.3 |
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

## Phase 2: KOMPLETT (~4-5 Tage) ✅ **COMPLETE**
> "Alle Features drin, sieht noch rough aus"

| Nr | Aufgabe | Status |
|----|---------|--------|
| 2.1 | Alle 4 Tunes (Move, Daze, Attack, Freeze) | ✅ v2.5 |
| 2.2 | Snake State Machine | ✅ v1.7.2 |
| 2.3 | Toon Snakes importieren | ✅ 6 Prefabs |
| 2.4 | Snake Animationen einbinden | ✅ Slither 3-dir |
| 2.5 | 3 Areas (Tutorial → Mitte → Finale) | 🔄 Phase 3 |
| 2.6 | Game States (Menu, Playing, Paused, End) | ✅ Basic |
| 2.7 | Main Menu + Result Screen | ✅ Basic |
| 2.8 | Simple + Advanced Mode | ✅ Wählbar |
| 2.9 | Tune 4 (Freeze) für Advanced | ⚠️ Phase 3 |
| 2.10 | Backend Setup + API | 🔄 Phase 3 |
| 2.11 | Session-Stats senden | 🔄 Phase 3 |

**Done when:** Core Features Complete ✅ (Scoped items moved to Phase 3)

**Completion Date:** 2026-02-15 (Session 17)
**Branch:** `feature/enemy-setup` (9 commits, 4 bugs fixed)

---

## v0.3: Bug Fixes & Stability (~2-3 Tage) 🔄 **IN ARBEIT**
> "NavMesh migration + Feature verification before polish"

| Nr | Aufgabe | Status |
|----|---------|--------|
| 0.3.1 | Player Ground Detection Fix | ✅ v1.9 |
| 0.3.2 | GSD Milestone Initialization | ✅ Complete |
| 0.3.3 | NavMesh Research (4 files) | ✅ Complete |
| 0.3.4 | Requirements & Roadmap | ✅ Complete |
| 0.3.5 | NavMesh Scene Setup (Baking) | ⏳ Phase 3 |
| 0.3.6 | Component Integration (Prefabs) | ⏳ Phase 4 |
| 0.3.7 | Movement Migration (Code) | ⏳ Phase 5 |
| 0.3.8 | Cleanup & Polish | ⏳ Phase 6 |
| 0.3.9 | Testing & Verification | ⏳ Phase 7 |

**Done when:** Snake patrol animations fixed, all Phase 2 features verified

**Current Status:** Requirements phase complete (2026-02-16)
**Branch:** `feature/enemy-setup` (12 commits, player ground fix + GSD init)
**Next:** Plan Phase 3 (NavMesh Scene Setup) via `/gsd:plan-phase 3`

**Research Findings:**
- NavMeshAgent integration: HIGH confidence, 5 hours estimated
- Zero blockers identified
- 10-step incremental migration plan
- Animation fix: Use `agent.velocity` instead of `_isPatrolling` bool

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
