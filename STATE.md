# PROJECT STATE - Snake Enchanter

**Letzte Aktualisierung:** 2026-02-25

---

## QUICK START

**Branch:** `feature/cave-rebuild`
**Letzter Commit:** `fd68d8d` - "feat(spells): fix spell system — TuneConfigs, Entranced state, Shield coupling"
**Working Tree:** Clean (nur Roboto-Bold SDF.asset auto-change)

**Milestone:** v1.0 Submission Ready (Phases 7-13)
**Aktuelle Phase:** 7 - Spell System
**Fortschritt:** ~35% von v1.0

---

## WAS GERADE PASSIERT

Spell-System wurde repariert. **Alles committed + pushed.** Nächster Schritt: Testing.

### Was diese Session gemacht wurde:
1. **TuneConfig ScriptableObjects korrigiert:**
   - Tune2_Sleep → Tune2_Daze (umbenannt)
   - Tune3_Shield neu erstellt (key=3, effect=Shield, duration=3s, zone=40-65%)
   - Tune4_Freeze gelöscht
   - Alle 3 Configs: Melody + SuccessSFX + FailSFX zugewiesen (im Unity Inspector)

2. **TuneController v3.2:**
   - `_unlockAllOnStart = true` — alle Spells sofort freigeschaltet (Debug, für Testing)
   - Melody spielt NUR bei Success (nicht während Hold)
   - Shield-Dauer = TuneConfig Melody Section Length (Single Source of Truth)
   - Subscribes auf `OnShieldDeactivated` → Melody stoppt mit Shield

3. **ShieldComponent v1.1:**
   - `ActivateShield(float duration)` — Duration von TuneConfig, Fallback auf Inspector-Wert

4. **SnakeAI v2.0 — Entranced-System:**
   - Neuer State `Entranced` (3s, amber) → dann `Dazed` (8s, blau, Die-Animation)
   - `CancelInvoke()` bei jedem Zustandswechsel
   - `SetTrigger("Die")` + `SetBool("IsDazed", true)` im Dazed-Entry

### NÄCHSTER SCHRITT — Testen (Play Mode):
- [ ] **Daze (Taste 2):** Entranced (amber, 3s, Snake schaut Player an) → Die-Animation → Dazed (blau, 8s) → Idle
- [ ] **Shield (Taste 3):** Dauer = 15s (endPoint 25 - startPoint 10), Melody stoppt bei Absorb/Expire
- [ ] **Move (Taste 1):** Melody spielt bei Success, Snake bewegt sich zum MoveAwayTarget
- [ ] **Daze-Fix testen:** `SetTrigger("Die")` war der letzte Fix — Snake sollte jetzt umfallen
- [ ] Wenn Spells funktionieren → `feature/cave-rebuild` in `main` mergen

### Bekannte Issues:
- SpellScrollPickup + SpellUnlockSystem existieren als Scripts, sind aber NICHT in der Scene platziert
  (Debug-Flag `_unlockAllOnStart` überbrückt das für Testing)
- TuneSliderUI muss getestet werden (zeigt Slider-Fortschritt während Hold?)
- Debug-Logs in SnakeAI sind noch aktiv (Entranced/Dazed transitions) — nach Testing entfernen

---

## ABGESCHLOSSENE MILESTONES

| Milestone | Status | Shipped |
|-----------|--------|---------|
| v0.1 SPIELBAR | DONE | 2026-02-09 |
| v0.2 KOMPLETT | DONE | 2026-02-15 |
| v0.3 Bug Fixes & Stability | DONE | 2026-02-18 |

## v1.0 PHASEN-ÜBERSICHT

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
- SpellScrollPickup + SpellUnlockSystem (code ready, NOT in scene)
- SpellHUDController v1.1 (dynamic HUD, cooldown overlay, range indicator)
- ShieldComponent v1.1 (duration from TuneConfig, blocks next attack)
- Cave Map (Caves Parts Set + Dwarven Pack + ProBuilder/Polybrush)
- Canvas UI: HealthBarUI v3.1 + TuneSliderUI v2.1
- Cinemachine v3.x (CM_PlayerCamera, CinemachineBrain)
- Win Condition (ExitTrigger)
- Game Loop (GameManager)
- Pirate Character: Humanoid Avatar, 10 Animations, Bake Into Pose

---

## FOLDER STRUCTURE

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
Branch: feature/cave-rebuild (pushed to origin)
Letzter Commit: fd68d8d "feat(spells): fix spell system — TuneConfigs, Entranced state, Shield coupling"
Remote: https://github.com/JuliGommz/Snake_Enchanter.git
Working Tree: Clean
```

---

## REGELN (NICHT VERHANDELBAR)

### Input System
AUSSCHLIESSLICH Unity New Input System! NIEMALS `UnityEngine.Input` (Legacy).

### Kamera-System (Cinemachine v3.x)
- Cinemachine besitzt Kamera-Position. NIEMALS per Script überschreiben.
- PlayerController steuert NUR Pitch (Mouse Y) + Body Yaw (Mouse X)

### Animation
- Humanoid Rig (Pirate.FBX + alle Animations)
- Bake Into Pose: Rotation (Body Orientation), Y (Feet), XZ (Center of Mass)
- Root Motion OFF (CharacterController steuert Movement)

### Git Workflow
- Feature Branches: `feature/<name>` from main
- Ein Feature = Ein Branch
- Nach Merge: Branch löschen

### Spell System (Phase 7)
- 3 Tunes: Move (1), Daze (2), Shield (3)
- Daze = zweiphasig: Entranced (3s, amber) → Dazed (8s, blau, Die-Anim)
- Melody spielt nur bei Success (nicht während Hold)
- Shield Duration = Melody Section Length (TuneConfig = Single Source)
- Debug: `_unlockAllOnStart = true` (für Testing, vor Release auf false setzen)
- HP heals only on successful charm (Move/Daze), not Shield
- Cooldown + Charges (Advanced mode)

### Wichtige Dateien für Spell-System
| Datei | Beschreibung |
|-------|-------------|
| `Scripts/TuneSystem/TuneController.cs` | v3.2 — Slider, Melody, Shield-Kopplung |
| `Scripts/Snakes/SnakeAI.cs` | v2.0 — Entranced State, CancelInvoke |
| `Scripts/Player/ShieldComponent.cs` | v1.1 — Duration Parameter |
| `Scripts/TuneSystem/TuneConfig.cs` | ScriptableObject Definition |
| `ScriptableObjects/TuneConfigs/Tune1_Move.asset` | Move Config (melody 0-12s) |
| `ScriptableObjects/TuneConfigs/Tune2_Daze.asset` | Daze Config |
| `ScriptableObjects/TuneConfigs/Tune3_Shield.asset` | Shield Config (melody 10-25s = 15s shield) |
