# PROJECT STATE - Snake Enchanter

**Letzte Aktualisierung:** 2026-02-24 (Session-Ende)

---

## QUICK START

**Branch:** `feature/cave-rebuild`
**Letzter Commit:** `abaa8eb` - "fix(scene): restore water material refs + NavMesh + GameManager bindings"
**Uncommitted:** Spell-System Fixes (siehe unten)

**Milestone:** v1.0 Submission Ready (Phases 7-13)
**Aktuelle Phase:** 7 - Spell System (Plans 07-01 bis 07-03 done, 07-04 auto tasks done)
**Fortschritt:** ~35% von v1.0

**GSD Tracking:** `.planning/STATE.md` (detaillierter Stand)

---

## WAS GERADE PASSIERT

Spell-System wird repariert und erweitert. **Uncommitted changes vorhanden!**

### Fertig (uncommitted):
1. **TuneConfig ScriptableObjects korrigiert:**
   - Tune2_Sleep → **Tune2_Daze** (umbenannt, tuneName=Daze)
   - **Tune3_Shield** neu erstellt (key=3, effect=Shield, duration=3s, zone=40-65%)
   - Tune4_Freeze gelöscht (nicht mehr im System)
   - Alle 3 Configs mit Melody + SFX Referenzen versehen

2. **TuneController v3.2 Änderungen:**
   - Debug-Flag `_unlockAllOnStart = true` (bypassed scroll collection für Testing)
   - Melody spielt jetzt nur bei **Success** (nicht während Hold)
   - `_activeMelodyConfig` trackt Config für post-success Melodie
   - Shield-Dauer gekoppelt an Melody Section Length (TuneConfig = Single Source of Truth)
   - Subscribes auf `OnShieldDeactivated` → stoppt Melody wenn Shield endet

3. **ShieldComponent v1.1:**
   - `ActivateShield(float duration)` — Duration wird von TuneConfig übergeben
   - Fallback auf Inspector-Wert wenn keine Duration gegeben

4. **SnakeAI v2.0 — Entranced-System:**
   - Neuer State `Entranced` im SnakeState Enum
   - Daze-Spell Flow: Entranced (3s, amber) → Dazed (8s, blau)
   - `_entrancedDuration` SerializeField (3s default)
   - `_entrancedColor` (amber/gold)
   - `CancelInvoke()` bei jedem Zustandswechsel (verhindert Invoke-Leaks)
   - **LETZTER FIX:** `SetTrigger("Die")` zum Dazed-Entry hinzugefügt (fehlte!)

### Noch zu testen (nächste Session):
- [ ] Daze: Entranced (amber, 3s) → Die-Animation → Dazed (blau, 8s) → Idle
- [ ] Shield: Dauer = Melody Section Length (startPoint: 10 → endPoint: 25 = 15s)
- [ ] Shield: Melody stoppt bei Shield-Deaktivierung (absorb oder expire)
- [ ] Move: Melody spielt bei Success (nicht während Hold)
- [ ] Alle 3 Spells: TuneConfigs korrekt zugewiesen im Inspector

---

## ABGESCHLOSSENE MILESTONES

| Milestone | Status | Shipped |
|-----------|--------|---------|
| v0.1 SPIELBAR | DONE | 2026-02-09 |
| v0.2 KOMPLETT | DONE | 2026-02-15 |
| v0.3 Bug Fixes & Stability | DONE | 2026-02-18 |

## v1.0 PHASEN-UBERSICHT

| Phase | Beschreibung | Status |
|-------|-------------|--------|
| 7 | Spell System (3 Tunes: Move, Daze, Shield) | ~90% (testing pending) |
| 8 | Menu & UI | pending |
| 9 | Backend & Stats | pending |
| 10 | Audio & Music | pending |
| 11 | Polish & Juice | pending |
| 12 | Testing & QA | pending |
| 13 | Build & Submission | pending |

---

## WAS FUNKTIONIERT

- Player Controller v1.8 (New Input System, Crouch, Cinemachine v3.x)
- Health System v1.5 (Drain, Events, Death Animations, heal-on-charm, shield intercept)
- Tune System (TuneController v3.2, 3-Tune Array, Debug Unlock, Cooldown, Charges)
- Snake AI v2.0 (NavMesh, Entranced→Dazed two-phase, CancelInvoke safety)
- SpellScrollPickup + SpellUnlockSystem (scroll-based unlock — NOT in scene yet)
- SpellHUDController v1.1 (dynamic HUD, cooldown overlay, range indicator)
- ShieldComponent v1.1 (duration from TuneConfig, blocks next attack)
- Cave Map (Caves Parts Set + Dwarven Pack + ProBuilder/Polybrush)
- Canvas UI: HealthBarUI v3.1 + TuneSliderUI v2.1
- Cinemachine v3.x (CM_PlayerCamera, CinemachineBrain)
- Win Condition (ExitTrigger)
- Game Loop (GameManager)
- Pirate Character: Humanoid Avatar, 10 Animations, Bake Into Pose

---

## FOLDER STRUCTURE (nach Konsolidierung 2026-02-24)

```
Assets/_Project/
  Art-Visuals/
    Images/              # UI Textures (teastain)
    Prefabs-FBX-Materials-Animations/
      Cave/              # FBX/, Materials/, Prefabs/ (Sample.prefab), Textures/
      Pirate/            # Mesh/, Animations/, Materials/, Prefabs/, Textures/, MC_Controller
      Props/             # Dungeon Props (Dwarven Pack)
      Snakes/            # Controllers/, FBX/, Materials/, Prefabs/, Textures/
  Scripts/               # Core, Player, Snakes, TuneSystem, UI, Data, Level, Editor
  ScriptableObjects/     # TuneConfigs: Tune1_Move, Tune2_Daze, Tune3_Shield
  Scenes/                # MainMenu, GameLevel
  Data/                  # Input Actions
  Design/
  Media/                 # Audio/Music, Audio/SFX, Audio/Tunes
```

---

## GIT STATUS

```
Branch: feature/cave-rebuild
Letzter Commit: abaa8eb "fix(scene): restore water material refs + NavMesh + GameManager bindings"
Remote: https://github.com/JuliGommz/Snake_Enchanter.git (pushed)
Uncommitted: Spell-System fixes (TuneController, ShieldComponent, SnakeAI, TuneConfigs)
```

---

## REGELN (NICHT VERHANDELBAR)

### Input System
AUSSCHLIESSLICH Unity New Input System! NIEMALS `UnityEngine.Input` (Legacy).

### Kamera-System (Cinemachine v3.x)
- Cinemachine besitzt Kamera-Position. NIEMALS per Script uberschreiben.
- PlayerController steuert NUR Pitch (Mouse Y) + Body Yaw (Mouse X)

### Animation
- Humanoid Rig (Pirate.FBX + alle Animations)
- Bake Into Pose: Rotation (Body Orientation), Y (Feet), XZ (Center of Mass)
- Spell Animation (keine Flote)
- Root Motion OFF (CharacterController steuert Movement)

### Git Workflow
- Feature Branches: `feature/<name>` from main
- Ein Feature = Ein Branch
- Nach Merge: Branch loschen

### Spell System (Phase 7)
- 3 Tunes: Move, Daze, Shield (Attack + Freeze removed)
- Daze = zweiphasig: Entranced (3s) → Dazed (8s)
- Melody spielt nur bei Success (nicht während Hold)
- Shield Duration = Melody Section Length (TuneConfig = Single Source)
- Debug: `_unlockAllOnStart = true` (scroll collection bypassed für Testing)
- Scroll-based unlock ready (SpellScrollPickup + SpellUnlockSystem existieren, aber NICHT in Scene)
- HP heals only on successful charm (Move/Daze), not Shield
- Cooldown + Charges (Advanced mode)
