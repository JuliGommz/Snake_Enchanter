# Roadmap: Snake Enchanter

## Milestones

- ✅ **v0.1 SPIELBAR** — Phase 1 (shipped 2026-02-09)
- ✅ **v0.2 KOMPLETT** — Phase 2 (shipped 2026-02-15)
- ✅ **v0.3 Bug Fixes & Stability** — Phases 3-6 (shipped 2026-02-18)
- 🚧 **v1.0 MVP Submission** — Phases 7-11 (in progress, deadline ~2026-03-03)

## Phases

<details>
<summary>✅ v0.1 SPIELBAR (Phase 1) — SHIPPED 2026-02-09</summary>

- [x] Phase 1: Core Implementation — PlayerController, HealthSystem, TuneController, GameManager, ExitTrigger, Pirate character, Cave environment, Canvas UI

</details>

<details>
<summary>✅ v0.2 KOMPLETT (Phase 2) — SHIPPED 2026-02-15</summary>

- [x] Phase 2: Enemy Setup — SnakeAI v1.7.2, 7-state machine, 4 Tunes, 3 attack types, directional slither, visual feedback

</details>

<details>
<summary>✅ v0.3 Bug Fixes & Stability (Phases 3-6) — SHIPPED 2026-02-18</summary>

- [x] Phase 3: NavMesh Scene Setup (1/1 plans) — completed 2026-02-17
- [x] Phase 4: Component Integration (2/2 plans) — completed 2026-02-17
- [x] Phase 5: Movement Migration (3/3 plans executed) — completed 2026-02-17
- [x] Phase 6: Cleanup & Polish (1/1 plans) — completed 2026-02-17

See: `.planning/milestones/v0.3-ROADMAP.md` for full details

</details>

---

### ✅ v1.0 MVP Submission (Phases 7-11)

**Milestone Goal:** Complete the game for PIP-3 Theme B academic submission — new cave layout, all spell systems wired in-editor, menu UI, backend stats, audio, and a stable Windows build by ~2026-03-03.

**Note:** Phases 7-11 implemented outside GSD tracking. Marked complete 2026-03-02.

---

### Phase 7: Cave Rebuild ✅
**Goal**: Players can walk a coherent path through a playable cave with a clear start, 3 distinct rooms, and a reachable exit
**Status**: Complete (implemented outside GSD)

### Phase 8: Scene Integration ✅
**Goal**: The cave is fully populated — snakes patrol, scrolls exist and work, every spell system component is wired and functional in-editor
**Status**: Complete (implemented outside GSD)

### Phase 9: Menu & Win Screen ✅
**Goal**: Players start from a proper menu with mode selection and see their session stats on the Win screen
**Status**: Complete (implemented outside GSD)

### Phase 10: Backend API ✅
**Goal**: Session data posts automatically on game end and leaderboard/player stats are retrievable without manual action
**Status**: Complete (implemented outside GSD)

### Phase 11: Balancing & Build ✅
**Goal**: Both game modes feel correctly tuned and a stable 60 FPS Windows .exe is packaged and ready for submission
**Status**: Complete (implemented outside GSD)

### Phase 12: Code Review Fixes
**Goal**: All critical bugs and code-quality issues from the expert review are resolved before academic submission
**Depends on**: All code complete (standalone phase — independent of Phases 7-11)
**Success Criteria** (what must be TRUE):
  1. TuneSliderUI shows "Shield" (not "Attack") for Tune 3 — the wrong label is corrected
  2. TuneController._unlockAllOnStart is false — scroll unlock system is not bypassed in production
  3. No bare magic numbers 33.3f or 0.15f — named constants used instead
  4. GameEvents.cs and HealthSystem.cs have WHY comments on key design decisions
  5. TuneSliderUI.BuildSegments() uses Destroy() not DestroyImmediate() at runtime
**Plans**: 2 plans

Plans:
- [x] 12-01-PLAN.md — Critical bugs: K1 (wrong tune label "Attack"→"Shield") + K2 (debug flag _unlockAllOnStart false)
- [x] 12-02-PLAN.md — Code quality: M1/M2 (magic number constants), D3/D4 (dead code comments), C1/C2 (WHY comments), P2 (Destroy fix)

---

### Phase 13: Repo-Abgabe-Struktur
**Goal**: Repo matches the PIP-3 submission layout exactly — Konzeption/, Arbeitsdateien/GME_Julian_Gomez/, Anwendung/, Trailer/, root PDFs
**Depends on**: Phase 12 complete
**Success Criteria** (what must be TRUE):
  1. `Konzeption/GDD_v1.8_SnakeEnchanter.pdf` exists and is git-tracked
  2. `Projektplan.pdf` exists at repo root and is git-tracked
  3. `Arbeitsprotokoll_Julian_Gomez.pdf` exists at repo root and is git-tracked
  4. `Arbeitsdateien/GME_Julian_Gomez/` exists with ASSETS_AT_ROOT.md pointer
  5. `Anwendung/ReadMe.txt` exists (build binaries placed manually, gitignored)
  6. `Trailer/` folder exists (MP4 added last)
  7. Unity project at repo root is completely unaffected
**Plans**: 2 plans

Plans:
- [ ] 13-01-PLAN.md — Konzeption/ + root PDFs (GDD, Projektplan, Arbeitsprotokoll)
- [ ] 13-02-PLAN.md — Arbeitsdateien/ + Anwendung/ + Trailer/ + ABGABE_EXPORT update

---

## Progress

**Execution Order:** 7 → 8 → 9 → 10 → 11 | 12 (standalone, any time)

| Phase | Complete    | 2026-03-02 | Status | Completed |
|-------|-----------|----------------|--------|-----------|
| 1. Core Implementation | v0.1 | 1/1 | Complete | 2026-02-09 |
| 2. Enemy Setup | v0.2 | 1/1 | Complete | 2026-02-15 |
| 3. NavMesh Scene Setup | v0.3 | 1/1 | Complete | 2026-02-17 |
| 4. Component Integration | v0.3 | 2/2 | Complete | 2026-02-17 |
| 5. Movement Migration | v0.3 | 4/3 | Complete | 2026-02-17 |
| 6. Cleanup & Polish | v0.3 | 1/1 | Complete | 2026-02-17 |
| 7. Cave Rebuild | v1.0 | - | Complete (outside GSD) | 2026-03-02 |
| 8. Scene Integration | v1.0 | - | Complete (outside GSD) | 2026-03-02 |
| 9. Menu & Win Screen | v1.0 | - | Complete (outside GSD) | 2026-03-02 |
| 10. Backend API | v1.0 | - | Complete (outside GSD) | 2026-03-02 |
| 11. Balancing & Build | v1.0 | - | Complete (outside GSD) | 2026-03-02 |
| 12. Code Review Fixes | v1.0 | 2/2 | Complete | 2026-03-02 |
| 13. Repo-Abgabe-Struktur | v1.0 | 0/2 | Pending | — |
