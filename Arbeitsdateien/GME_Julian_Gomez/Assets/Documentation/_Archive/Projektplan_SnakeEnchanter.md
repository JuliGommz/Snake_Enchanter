# Projektplan - Snake Enchanter
**Projekt:** PIP-3 Theme B - Snake Enchanter
**Teilnehmer:** Julian Gomez
**Zeitraum:** 03.02.2026 – ~03.03.2026

---

## Meilensteine

| Meilenstein | Ziel-Datum | Ist-Datum | Status |
|-------------|------------|-----------|--------|
| GDD v1 Abgabe | 27.01.2026 | 27.01.2026 | ✅ Erledigt |
| **Phase 1: Spielbar** | 07.02 | 09.02.2026 | ✅ Erledigt |
| Fortschritts-Präsentation 1 | 08.02 | 08.02.2026 | ✅ Erledigt |
| **Phase 2: Komplett** | 14.02 | 15.02.2026 | ✅ Erledigt |
| **v0.3: Bug Fixes & Stability** | 18.02 | 19.02.2026 | ✅ Erledigt |
| GDD v2 + Making-of | 17.02 | 21.02.2026 | ✅ GDD v1.6 |
| **Phase 3: Schön** | 21.02 | 27.02.2026 | ✅ Erledigt |
| Fortschritts-Präsentation 2 | 24.02 | 24.02.2026 | ✅ Erledigt |
| **Phase 4: Fertig** | 03.03 | 03.03.2026 | ✅ Erledigt |
| Finale Abgabe | 03.03.2026 | — | ⏳ Ausstehend |

---

## Phase 1: SPIELBAR (~3-4 Tage) ✅ DONE
> "Ich kann durch einen Raum laufen und eine Schlange wegcharmen"

| Nr | Aufgabe | Status |
|----|---------|--------|
| 1.1 | Unity Projekt Setup | ✅ |
| 1.2 | Git/GitHub Setup | ✅ |
| 1.3 | Dokumentation & Struktur | ✅ |
| 1.4 | Player Controller (WASD + Kamera) | ✅ v1.9 Cinemachine |
| 1.5 | Greybox Level (1 Raum) | ✅ Cave Map |
| 1.6 | Tune Input (1 Taste halten → Slider) | ✅ ADR-008 Genshin-Style |
| 1.7 | Timing Window (Erfolg/Fail) | ✅ Triggerzone |
| 1.8 | Health System (HP, Drain, Damage) | ✅ v1.5 |
| 1.9 | Win/Lose Conditions | ✅ ExitTrigger |

**Done when:** Spielbarer Loop mit Cubes/Capsules ✅
**Completion Date:** 09.02.2026

---

## Phase 2: KOMPLETT (~4-5 Tage) ✅ DONE
> "Alle Features drin, sieht noch rough aus"

| Nr | Aufgabe | Status |
|----|---------|--------|
| 2.1 | Alle Tunes (Move, Daze, Shield) | ✅ v3.3 — 3 Tunes (Freeze/Attack gestrichen, Shield neu) |
| 2.2 | Snake State Machine | ✅ v2.1 NavMesh + Entranced/Dazed |
| 2.3 | Toon Snakes importieren | ✅ 6 Prefabs |
| 2.4 | Snake Animationen einbinden | ✅ Slither 3-dir + Attack + Die |
| 2.5 | Cave Level aufbauen | ✅ Caves Parts Set + Dwarven Pack |
| 2.6 | Game States (Menu, Playing, End) | ✅ GameManager v1.3 |
| 2.7 | Main Menu + Result Screen | ✅ Vollständig |
| 2.8 | Simple + Advanced Mode | ✅ Wählbar, differenzierte Schwierigkeit |
| 2.9 | Tune 4 (Freeze) | ⚠️ Gestrichen — Shield als Ersatz implementiert |
| 2.10 | Backend Setup + API | ✅ Node.js + MySQL (Phase 9) |
| 2.11 | Session-Stats senden | ✅ ApiManager.cs v1.1 — POST + PUT + DELETE + 2× GET |

**Done when:** Core Features Complete ✅
**Completion Date:** 15.02.2026

---

## v0.3: Bug Fixes & Stability ✅ DONE
> "NavMesh migration + Feature verification before polish"

| Nr | Aufgabe | Status |
|----|---------|--------|
| 0.3.1 | Player Ground Detection Fix | ✅ v1.9 |
| 0.3.2 | NavMesh Research | ✅ |
| 0.3.3 | NavMesh Scene Setup (Baking) | ✅ 17.02 |
| 0.3.4 | NavMeshAgent auf Snake-Prefabs | ✅ 17.02 |
| 0.3.5 | Pirate Avatar / FBX Recovery | ✅ 21.02 |
| 0.3.6 | Breath Attack Fix (CancelInvoke) | ✅ 01.03 |
| 0.3.7 | Shield Bypass Fix | ✅ 01.03 |
| 0.3.8 | Advanced Mode Difficulty kalibriert | ✅ 01.03 |

**Completion Date:** 19.02.2026 (NavMesh), 01.03.2026 (Bug Fixes)

---

## Phase 3: SCHÖN (~3-4 Tage) ✅ DONE
> "Fühlt sich gut an, sieht anständig aus"

| Nr | Aufgabe | Status |
|----|---------|--------|
| 3.1 | Flöten-Melodien einbinden (Tune-Audio) | ✅ TuneConfig, AudioClips zugewiesen |
| 3.2 | Snake SFX (Hiss, Bite) | ⚠️ Minimal |
| 3.3 | UI Sounds | ⚠️ Minimal |
| 3.4 | Hintergrundmusik (MainMenu + Gameplay) | ✅ MusicManager v1.1 |
| 3.5 | Visual Feedback — Story Fade In/Out | ✅ StoryIntro + EndingStory |
| 3.6 | Health Bar Polish (Gradient, Puls) | ✅ HealthBarUI v3.1 |
| 3.7 | Timing Meter Polish | ✅ TuneSliderUI v2.2 |
| 3.8 | Level Polish (Lighting, Props, Cave) | ✅ Caves Parts Set + Dwarven Pack |
| 3.9 | Spell HUD (dynamisch, Cooldown, Range) | ✅ SpellHUDController v1.1 |
| 3.10 | Active Effects Anzeige (MOVE/DAZE/SHIELD) | ✅ ActiveEffectsController v1.1 |
| 3.11 | Story Intro + Ending (CanvasGroup Fade) | ✅ StoryIntroController v1.2 + EndingStoryController v1.1 |
| 3.12 | Pirate Character + 10 Animationen | ✅ Humanoid Rig, Mixamo |

**Done when:** Juice & Polish fertig ✅
**Completion Date:** ~01.03.2026

---

## Phase 4: FERTIG (~2-3 Tage) 🔄 IN ARBEIT
> "Abgabe-Ready"

| Nr | Aufgabe | Status |
|----|---------|--------|
| 4.1 | Bug Fixing | ✅ Breath Attack, Shield, NavMesh, Code Review (Phase 12) |
| 4.2 | Balancing (HP, Timing, Drain) | ✅ Simple/Advanced kalibriert |
| 4.3 | Test auf Schul-Laptops | ⏳ |
| 4.4 | **Trailer produzieren** (1920×1080, MP4) | ⏳ |
| 4.5 | GDD finalisieren → PDF | ✅ GDD v1.8 fertig |
| 4.6 | Final Build (Windows x64) | ✅ Builds/Snake_Enchanter.exe |
| 4.7 | API — alle 4 HTTP-Methoden | ✅ GET + POST + PUT + DELETE implementiert |
| 4.8 | Repo-Struktur nach Abgabevorgabe | 🔄 In Arbeit |
| 4.9 | Arbeitsprotokoll + Projektplan als PDF | 🔄 In Arbeit |
| 4.10 | Präsentation vorbereiten (15 min) | ⏳ |

**Done when:** Trailer + Präsentation abgeschlossen

---

## Scope-Änderungen (dokumentierte Abweichungen)

| Original | Änderung | Begründung |
|----------|----------|------------|
| Tune 3: Attack (Schlange greift Feind an) | → Shield (Spieler blockt nächsten Angriff) | Attack benötigt vollständiges Creature-System; Shield bietet mehr taktischen Mehrwert |
| Tune 4: Freeze (Advanced) | Gestrichen | Überlappt funktional mit Daze; Shield ersetzt strategische Tiefe |
| Spell Scroll Unlock-System im Level | `_unlockAllOnStart = false` — Code fertig, Pickup nicht platziert | Scrolls in Cave platzieren war nicht MVP-kritisch |
| 3 Areas (Tutorial → Mitte → Finale) | 1 offenes Level | Zeitdruck; Kernmechanik funktioniert ohne strikte Zonen-Trennung |
| API: GET + POST + PUT + DELETE | Alle 4 implementiert | Zwei-Phasen-Lifecycle: POST (Start) → PUT (Ende) → DELETE (Result Screen) |

---

## Risiken & Mitigationen

| Risiko | Mitigation | Status |
|--------|------------|--------|
| Timing-System komplex | Slider-System (ADR-008) — klarer als Timer | ✅ Gelöst |
| Snake AI Probleme | NavMeshAgent + State Machine v2.1 | ✅ Gelöst |
| Zeitdruck | Scope reduziert (Freeze/Attack → Shield) | ✅ Gelöst |
| Backend-Integration | Node.js + MySQL, fail-silent API | ✅ Gelöst |
| Trailer Produktion | Unity Recorder eingerichtet | ⏳ Offen |

---

## Regeln

1. **Keine Phase anfangen bevor vorherige DONE ist**
2. **Täglich:** Arbeitsprotokoll + Screenshot + Commit
3. **Bei Problemen:** Scope reduzieren, nicht Zeit verlängern
