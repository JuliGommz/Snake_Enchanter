# Arbeitsprotokoll - Snake Enchanter
**Teilnehmer:** Julian Gomez
**Projekt:** Snake Enchanter - PIP-3 Theme B
**Zeitraum:** 03.02.2026 – ~03.03.2026

---

## Phase 1: SPIELBAR

### 03.02.2026 (Montag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| Unity Projekt anlegen | x | x | x |
| Ordnerstruktur erstellen (_Project, Scripts, etc.) | x | x | x |
| Git Repository initialisieren | x | x | x |
| GitHub Remote verbinden + Push | x | x | x |
| Documentation migrieren (GDD, Config) | x | x | x |
| .gitignore konfigurieren | x | x | x |
| Projektplan erstellen (4-Phasen-Modell) | x | x | x |
| Arbeitsprotokoll-Template erstellen | x | x | x |
| MVP-Struktur definieren | x | x | x |
| Core Scripts erstellen (6 Scripts) | x | x | x |
| GDD v1.4 mit Slider-System aktualisieren | x | x | x |
| New Input System Migration (v1.2/v2.1) | x | x | x |
| Unity Integration (Player, ExitTrigger) | x | x | x |
| Input Actions Asset konfigurieren | x | x | x |
| Greybox Level aufbauen | x | x | x |
| ADR-006 als Projekt-Regel definieren | x | x | x |

**Screenshot:** `Media/Screenshots/2026-02-03_ProjectSetup.png`

**Notizen:**
- Unity 2022 LTS mit URP Template
- 4-Phasen-Entwicklungsmodell: Spielbar → Komplett → Schön → Fertig
- Jede Phase ist abgebbar - Qualität konstant, Umfang wächst
- Initial Commit + Dokumentation Commit
- **WICHTIG:** Nur New Input System verwenden (ADR-006 Projekt-Regel)
- Animation-Entscheidung: Spell statt Flute (einfacher)
- Scripts: PlayerController v1.2, TuneController v2.1, HealthSystem, GameEvents, TuneConfig, ExitTrigger
- Unity Scene: Player komplett konfiguriert, ExitTrigger platziert, Input Actions zugewiesen
- **OFFEN:** Toon Snakes Pack muss noch aus Asset Store importiert werden

---

### 04.02.2026 (Dienstag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| PlayerController v1.3: Crouch-System (LeftCtrl) | x | x | x |
| PlayerController v1.4: Camera Offset (entfernt) | x | x | x |
| PlayerController v1.5: Hierarchy Camera | x | x | x |
| Crouch Action in InputActions + Binding | x | x | x |
| Camera Pitch Limits (Best Practice -70/+70) | x | x | x |
| Crouch Speed 50% (2.5 von 5.0) | x | x | x |
| Config Update: Third-Person → First-Person | x | x | x |
| Cave Assets inventarisieren + Collider prüfen | x | x | x |
| Cave System aufbauen (Caves Parts Set) | x | x | x |

**Screenshot:** `Media/Screenshots/2026-02-04_CaveMap.png`

**Notizen:**
- PlayerController durchlief 3 Iterationen (v1.3→v1.5) wegen Camera-Override-Problem
- Lektion: Script sollte Kamera-Position NICHT besitzen, sondern aus Hierarchy lesen
- Cave Prefabs haben alle MeshCollider (IsTrigger=false, Convex=false)
- Dwarven Pack für Deko (Pillars, Arches, Window Props für Exit)
- Cave System fertig gebaut in Unity
- **OFFEN:** TuneConfig SOs, UI (HealthBar/Slider), Snake AI, Toon Snakes Pack


---

### 05.02.2026 (Mittwoch)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| Toon Snakes Pack importieren + in Scene platzieren | x | x | x |
| Snake-Sichtbarkeit in Game View debuggen (mit Dozent gelöst) | | x | x |
| TuneConfig ScriptableObjects erstellen (Editor-Tool) | x | x | x |
| GameManager GameObject anlegen + konfigurieren | x | x | x |
| SnakeAI auf Snakes (BoxCollider + SnakeAI Component) | x | x | x |
| TuneConfigs im TuneController zuweisen | x | x | x |
| HealthSystem Drain-Rate kalibrieren (0.1 HP/sec, deaktiviert für Dev) | | x | x |
| HealthSystem Event-Flood Fix (_lastReportedHealth) | | x | x |
| Snake Tune-Targeting testen (Proximity-basiert, Command Range) | x | x | x |
| 16 neue Mixamo-Animationen importiert (Crouch, Spell, etc.) | x | x | x |
| MC_Controller Animator neu aufgebaut (Idle, Walk, Crouch) | x | x | x |
| Player/Animation Setup untersuchen + debuggen | x | x | x |
| Player Einsinken Fix (heightFromFeet + keepOriginalOrientation) | | x | x |
| Animator von Player auf Cowboy (child) verschoben | | x | x |
| CharacterController Capsule korrekt konfiguriert | | x | x |
| Canvas UI aufbauen (HealthBar + TuneSlider) | x | | |

**Screenshot:** `Media/Screenshots/2026-02-05_.png`

**Notizen:**
- Toon Snakes Pack (Meshtint Studio) importiert: 6 Prefabs (Cobra/Snake x Green/Purple/Magenta)
- Snake-Sichtbarkeit: Snakes in Scene View sichtbar aber Game View unsichtbar — mit Dozent gelöst
- TuneConfigs: 4 ScriptableObjects per Editor-Tool erstellt (Move 3s, Sleep 4s, Attack 5s, Freeze 6s)
- HealthDrain Bug: Rate war 2.5 HP/sec (viel zu schnell), korrigiert auf 0.1 HP/sec, deaktiviert für Dev
- HealthChanged Event Flood: Feuerte 60x/sec, Fix durch _lastReportedHealth Check
- Snake-Verhalten getestet: Proximity-basiertes Targeting funktioniert (nächste Snake reagiert)
- Snake MoveAwayTarget: Beide Snakes liefen zum gleichen Punkt — individuell pro Snake nötig
- 16 neue Mixamo-Animationen importiert (Crouch Walk, Spell Casting, Jump, etc.)
- MC_Controller neu aufgebaut: Idle → Walk (Speed > 0.1), Crouch Forward/Back (IsCrouching)
- **Player Einsinken debuggt:** Ursache war Humanoid Animator Root Transform Position — heightFromFeet war 0 (Original) statt 1 (Feet). Alle 4 Animations-FBX gefixt.
- **Animator verschoben:** Von Player (Parent) auf Cowboy (Child mit Bone-Hierarchy) — Standard Unity Pattern
- **CharacterController:** Height=1.84, Center Y=0.9, Radius=0.3 — Capsule korrekt um Model
- **Animationen funktionieren!** Walk, Crouch — Fortschritt!
- **Injured Idle ersetzt:** FBX hatte defekte Root-Orientation. Ersetzt durch "Old Man Idle" — funktioniert!
- **PLAYER ANIMATION KOMPLETT GELÖST!** Idle steht, Walk funktioniert, Crouch funktioniert
- **OFFEN:** Canvas UI (HealthBar + TuneSlider) noch nicht aufgebaut


---

### 06.02.2026 (Donnerstag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| Git Branch-Strategie einrichten + dokumentieren | x | x | x |
| CanvasUICreator v1.0 Editor-Tool erstellen | x | x | x |
| HealthBarUI v2.0→v3.1: Gradient, Pulse, Debuff, Frame | x | x | x |
| TuneSliderUI v2.0→v2.1: Segmente, Marker, Frame, Zonen | x | x | x |
| CanvasUICreator v2.0: Neue Hierarchie + Auto-Wiring | x | x | x |
| Canvas UI im Inspector anpassen (Steampunk-Theme, Arvo SDE Font) | x | x | x |
| Cinemachine einbauen (CM_PlayerCamera, CameraHeadTracker) | x | x | x |
| PlayerController v1.5→v1.7: Cinemachine-kompatibel (Pitch-only) | x | x | x |
| CameraHeadTracker.cs erstellen (Position-only Tracking) | x | x | x |
| Steampunk UI Pack (Gentleland) importieren | x | x | x |
| 3D_Assets Ordnerstruktur bereinigen | x | x | x |
| Gradient-Bug fixen (UpdateBarColor nur bei Event statt kontinuierlich) | | x | x |
| TuneSliderUI Fixes: MarkerSize, Frame Sliced, OnValidate | | x | x |
| Setup-Review aller 12 Scripts (Qualitaet, Standards, Redundanz) | x | x | x |
| Backlog erstellen (B-001 Lambda-Leak, B-002 deprecated API) | | x | x |
| Player Avatar geaendert + Cinemachine Kamera repariert | x | x | x |
| Animationen nach Cinemachine-Umbau testen | x | x | |

**Screenshot:** `Media/Screenshots/2026-02-06_.png`

**Notizen:**
- Git Branch-Strategie eingefuehrt: `feature/<name>` von main, Claude erinnert bei Start/Ende
- Canvas UI komplett ueberarbeitet: Genshin-Style Segmented Slider mit 3 Farbzonen (Gelb=Safe, Orange=Success, Grau=Danger)
- HealthBar: Gradient (Rot→Gelb→Gruen), Puls-Effekt unter 30% HP, Debuff-Text, kein HP-Text
- Steampunk UI Pack fuer Rahmen/Frame-Sprites, Font: Arvo SDE
- Cinemachine v3.x integriert: Kamera folgt Head-Bone, PlayerController steuert nur Pitch
- CameraHeadTracker.cs: Verfolgt nur Position des Head-Bones, Rotation bleibt bei PlayerController
- PlayerController v1.7: Cinemachine Final — Camera.main Auto-Find, Pitch-only Steuerung
- **Gradient-Bug gefixt:** UpdateBarColor wurde nur bei OnHealthChanged aufgerufen, nicht kontinuierlich
- **Setup-Review:** Alle 12 Scripts validiert — kein ueberflussiges Script, alle Standards eingehalten
- **Backlog:** B-001 TuneController Lambda-Leak, B-002 SnakeAI deprecated FindObjectsOfType
- **PROBLEM:** Nach Cinemachine-Umbau und Avatar-Aenderung funktionieren Animationen nicht mehr
- **OFFEN:** Animations-Problem debuggen (Avatar-Zuweisung, Animator-Platzierung pruefen)


---

### 08.02.2026 (Samstag) - Session 8
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| Pirate FBX importieren + Humanoid Rig konfigurieren | x | x | x |
| PirateAvatar erstellen (Configure in Unity) | x | x | x |
| 8 URP/Lit Materials manuell auf SkinnedMeshRenderer zuweisen | x | x | x |
| 13 Pirate Animations importieren + auf PirateAvatar retargeten | x | x | x |
| MC_Controller.controller: 4 States (Idle, Walk, Crouch Idle, Crouch Walk) | x | x | x |
| Pirate als Child von Player im GameLevel.unity | x | x | x |
| CameraTarget unter Head Bone für Cinemachine erstellen | x | x | x |
| CM_PlayerCamera Tracking Target auf CameraTarget setzen | x | x | x |
| Full Core Loop Play-Test durchführen | x | x | x |
| Backlog erstellen aus Test-Feedback | x | x | x |
| Git Commit: "Complete Pirate character setup" | x | x | x |

**Screenshot:** `Media/Screenshots/2026-02-08_PirateSetupComplete.png`

**Notizen:**
- Pirate Character komplett integriert: FBX, Avatar, Materials, Animations
- Material Remapping schlug fehl → Manuelles Assignment auf SkinnedMeshRenderer
- PirateAvatar GUID: 619359b845787a443af41cf1ed1cfed0
- 13 Animations organisiert: Idle/, Walk/, Crouch/, Death/, Spell/, Others/
- Animator: 4 Movement States mit Speed/IsCrouching Parameters
- CameraTarget: Leeres GameObject unter Head Bone (Pirate_Skeleton/Spine/Spine1/Spine2/Neck/Head)
- **Core Loop Test erfolgreich:** Movement, Crouch, Camera, Animations funktionieren
- **Backlog Items:** Exit Trigger Hang, Cave Textures Neon-Yellow, Crouch Transitions ruckelig
- **LESSONS LEARNED:** DEBUGGING: Always Check Live Setup First (ask user, don't read old files)
- Git Commit: bd472c0 "Complete Pirate character setup - Phase 1 animations working"

---

### 09.02.2026 (Sonntag) - Session 9
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| MC_Controller: 4 Spell States hinzufügen (Move, Daze, Attack, Fear) | x | x | x |
| MC_Controller: 2 Death States hinzufügen (Drain, Snakes) | x | x | x |
| Animator: 7 Parameter (Speed, IsCrouching, 4 Spell Triggers, IsDead) | x | x | x |
| TuneController v2.3→v2.4: Spell Animation Integration | x | x | x |
| HealthSystem v1.2.1→v1.3: Death Animation Integration | x | x | x |
| Spell Animations testen (Tune 1-4) | x | x | x |
| Death Animation testen (Death_by_Drain) | x | x | x |
| GDD v1.4→v1.5: Player Character + Animations dokumentieren | x | x | x |
| STATE.md + BACKLOG.md aktualisieren | x | x | x |
| Arbeitsprotokoll Session 8 + 9 nachtragen | x | x | x |
| Alle Dokumente auf "First-Person" Konsistenz prüfen | x | x | x |

**Screenshot:** `Media/Screenshots/2026-02-09_MCAnimationsComplete.png`

**Notizen:**
- **MC Animations komplett:** 10 States total (4 Movement, 4 Spell, 2 Death)
- **Spell Animations:** Triggered by TuneController v2.4 bei Success
  - Tune 1 (Move): Spell Casting
  - Tune 2 (Daze): Wide Arm Spell Casting
  - Tune 3 (Attack): Standing 2H Cast Spell
  - Tune 4 (Fear): Magic Spell Casting
- **Death Animations:** Script-basiert via `animator.Play()` (Option B)
  - Death_by_Drain: Standing React Death Forward
  - Death_by_Snakes: Standing React Death Left (noch nicht testbar, Snakes machen kein Damage)
- **Testing:** Alle 4 Spell Animations funktionieren, Death_by_Drain funktioniert
- **Camera View:** First-person mit sichtbaren Armen + Füßen (full body model)
- **GDD v1.5:** Player Character Section erweitert mit allen Animations
- **Neue Backlog Items:** Camera Position bei Crouch, Death_by_Snakes Testing
- **Phase 1 Status:** KOMPLETT → Übergang zu Phase 2 (Enemy System + Snake Animations)

---

## Phase 2: KOMPLETT

### 10.02.2026 (Montag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-10_.png`

**Notizen:**


---

### 11.02.2026 (Dienstag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-11_.png`

**Notizen:**


---

### 12.02.2026 (Mittwoch)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-12_.png`

**Notizen:**


---

### 13.02.2026 (Donnerstag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| SnakeAI Props Collision Fix (Tag Typo "Enviroment") | | x | x |
| Props Mesh Colliders auf Convex setzen (20 Prefabs) | | x | x |
| SnakeAI Raycast Distance Fix (0.33 → 1.0 min) | | x | x |
| MoveAwayTarget Hierarchy Fix (SetParent null) | | x | x |
| Visual Color System Fix (URP _BaseColor) | | x | x |
| Material Emission Glow System (v1.4.0) | x | x | x |
| Particle Glow System Experiment (v1.4.1) | | x | ❌ |
| Particle System Revert (git restore) | | x | x |
| Documentation erstellen (4 MD files) | | x | x |
| SnakeAI v1.3.11 - v1.3.14 Testing | x | x | x |

**Screenshot:** `Media/Screenshots/2026-02-13_.png`

**Notizen:**
- **SnakeAI v1.3.14 COMPLETE** - Alle Core Behaviors funktionieren
- **Props Collision:** Tag-Typo "Enviroment" in 20 Prefabs behoben (Bash-Script)
- **MoveAwayTarget:** War Child von Snake → Endlos-Verfolgung. Fix: SetParent(null) in Awake()
- **Visual Feedback:** Material Emission funktioniert (Augen leuchten in State-Farbe)
- **Particle Glow:** Experiment fehlgeschlagen (kontinuierliche Emission trotz Settings)
- **Git Revert:** Particle System Changes sauber entfernt (git restore + rm)
- **Lessons Learned:**
  - User-Vorschläge ernst nehmen (Props Collider waren das Problem)
  - Targets niemals als Child von bewegten Objekten
  - Tag Typos sind silent killers (keine Compiler-Warnung)
  - URP Shader Properties sind nicht universal (_BaseColor statt .color)
- **Documentation:** Movement Logic, Props Collision Fix, MoveAwayTarget Fix, Glow Setup
- **Backlog:** External Glow System verschoben (Material Emission reicht erstmal)
- **Status:** Phase 2 zu 90% complete, SnakeAI Core Features DONE


---

### 15.02.2026 (Samstag) - Session 17
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| Scene + Prefabs Setup (6 Snake Prefabs, MoveAwayTargets) | | x | x |
| Unity Packages Update (URP, ShaderGraph) | | x | x |
| SnakeAI v1.7.0: Line-of-Sight + Dead Code Cleanup (GSD Debug) | | x | x |
| SnakeAI v1.7.1: Controller Fix + Attack Cooldown Reset | | x | x |
| SnakeAI v1.7.2: Die Animation Loop Fixed | | x | x |
| Tune 4 (Freeze) Unlock für Testing | | x | x |
| Bug Testing Session (IsDazed, Attack Cooldown, Die Animation) | x | x | x |
| GSD Debug Documentation | | x | x |
| Projektplan Phase 2 Status Update | | x | x |
| MERGE_CHECKLIST.md + PHASE3_SCOPE.md erstellen | | x | x |

**Screenshot:** `Media/Screenshots/2026-02-15_Phase2Complete.png`

**Notizen:**
- **SnakeAI v1.7.0 → v1.7.2:** 4 Critical Bugs Fixed in 3 Commits
- **Bug 1 (IsDazed Parameter ERROR):**
  - Prefabs nutzten External_Assets controller ohne IsDazed parameter
  - Fix: Alle 6 Snake Prefabs auf _Project controller umgestellt
  - Result: "Parameter 'IsDazed' does not exist" ERROR behoben
- **Bug 2 (Attack Cooldown):**
  - Snake konnte 4s nicht angreifen nach Dazed → Idle transition
  - Fix: `_lastAttackTime = 0f` beim Verlassen von Dazed state
- **Bug 3 (Die Animation Loop):**
  - Root Cause: Dead state setzte IsDazed NICHT → Die → Idle Transition
  - Fix: `IsDazed=true` in Dead state setzen
  - Result: Snake bleibt in Die Animation (collapsed)
- **Bug 4 (Tune 4 UI Missing):**
  - `_tune4Unlocked = false` by default
  - Fix: Changed to `true` for Phase 2 testing
- **Testing Results:**
  - ✅ Tune 1 (Move): Works perfectly
  - ✅ Tune 2 (Daze): 8s timer, Blue glow, Die animation
  - ✅ Tune 3 (Attack): Snake attacks RobotKyle, both die (Phase 1 design)
  - ⏳ Tune 4 (Freeze): Unlocked but not functional → Phase 3 backlog
  - ⏳ Slither Left/Right: Untested
- **Phase 2 Declaration:** User declared Phase 2 feature-complete
  - Tune 4 Freeze: Moved to Phase 3 backlog (code exists, not working)
  - Manual work: User placing Snake prefabs + MoveAwayTargets in scene
- **Documentation:**
  - GSD Debug Session: 3 files in `.planning/debug/`
  - MERGE_CHECKLIST.md: Branch merge workflow
  - PHASE3_SCOPE.md: Audio, Visual, UI Polish scope
- **Status:** Phase 2 COMPLETE (95%), awaiting scene placement → Branch merge
- **Git:** 7 commits on feature/enemy-setup, 4 critical bugs fixed

---

### 16.02.2026 (Samstag) - Session 18
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| PlayerController Ground Detection Fix | | x | x |
| GSD Milestone v0.3 Initialization | | x | x |
| NavMesh Research (4 research files) | | x | x |
| Requirements Definition (REQUIREMENTS.md) | | x | x |
| Roadmap Creation (ROADMAP.md - 5 phases) | | x | x |
| Documentation Updates (STATE.md, PROJECT.md, MILESTONES.md) | | x | x |

**Screenshot:** `Media/Screenshots/2026-02-16_v0.3Initialized.png`

**Notizen:**
- **PlayerController v1.8 → v1.9:** Ground Detection Fix (Commit aad1aac)
  - Added Start() method with `_velocity.y = -5f`
  - Player no longer floats above ground on spawn
  - Teacher request addressed: "Player should find ground at start"
- **GSD Milestone v0.3 Initialized:** Bug Fixes & Stability
  - PROJECT.md: Project context, validated requirements, constraints
  - STATE.md: Current position, active issues, accumulated context
  - MILESTONES.md: v0.1 (Phase 1) and v0.2 (Phase 2) completion history
  - Total: 3 initialization commits (b1f1aac, 3a6dd07, bb689bb)
- **Research Phase Complete:** (HIGH confidence, 2500+ lines total)
  - STACK.md: com.unity.ai.navigation v2.0.9 installed, baking workflow
  - FEATURES.md: NavMesh feature mapping to SnakeAI behaviors
  - ARCHITECTURE.md: 10-step migration plan, integration points
  - PITFALLS.md: 7 critical pitfalls with prevention strategies
  - SUMMARY.md: Executive summary, 2.5-3 hours estimated
- **Requirements Defined:** (REQUIREMENTS.md)
  - REQ-1: Player ground detection ✅ COMPLETE
  - REQ-2: Snake patrol animation fix (NavMesh migration)
  - REQ-3: State machine preservation (must not break)
  - REQ-4: Full feature verification (all Phase 2 features)
- **Roadmap Created:** (ROADMAP.md - 5 phases, 5 hours total)
  - Phase 3: NavMesh Scene Setup (1 hour, LOW risk)
  - Phase 4: Component Integration (1 hour, LOW risk)
  - Phase 5: Movement Migration (1.5 hours, MEDIUM risk)
  - Phase 6: Cleanup & Polish (30 min, LOW risk)
  - Phase 7: Testing & Verification (1 hour, ZERO risk)
- **Key Findings:**
  - NavMeshAgent integration is architecturally feasible with minimal risk
  - Use `agent.isStopped` to control movement (NOT `agent.enabled`)
  - Animation trigger: `agent.velocity.magnitude > 0.1f` (replaces `_isPatrolling` bool)
  - 10-step incremental migration plan ensures safety
  - Zero blockers identified
- **Status:** Requirements phase complete, ready for Phase 3 planning
- **Next Action:** `/gsd:plan-phase 3` to begin NavMesh Scene Setup implementation
- **Git:** 4 commits on feature/enemy-setup (aad1aac, b1f1aac, 3a6dd07, bb689bb)

---

### 14.02.2026 (Freitag) - Session 16
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| Tune 2: Sleep → Daze Rename (alle Files) | | x | x |
| Tune 2 Behavior: 8s Timer, Die Animation, Blue Glow | x | x | x |
| Tune 3 Behavior: Attack Non-Snake Creatures (tag-based) | x | x | x |
| Directional Slither Animations (Forward/Left/Right) | x | x | x |
| Debug Logging System (Spells, Attacks, States) | | x | x |
| SnakeAI v1.5.0 → v1.6.0 Implementation | x | x | x |
| DESIGN_CHANGES.md Backlog Section erstellen | | x | x |

**Screenshot:** `Media/Screenshots/2026-02-14_Session16.png`

**Notizen:**
- **SnakeAI v1.6.0** - Directional Slither + Debug Logging
- **Sleep → Daze:** Komplettes Rename (SnakeState, SnakeEffect, UI, Editor Scripts)
- **Tune 2 (Daze):** Code funktioniert (IsDazed Bool, 8s Timer, Blue Glow, Collision OFF)
- **Tune 3 (Attack):** FindNearestCreature() skips ALL snakes (nur non-snake creatures)
- **Directional Slither:** UpdateMovementAnimation() setzt Forward/Left/Right basierend auf InverseTransformDirection
- **Debug Logging:** Alle Spell States, Attack Types, Daze Transitions vollständig geloggt
- **Testing:** Slither funktioniert, Tune 1-3 Spells loggen korrekt
- **BACKLOG Items dokumentiert:**
  - Two-Level Success System (Spell Cast + Enemy Enchanted)
  - Player Spell Cooldown (Inspector-konfigurierbar)
  - Player Success Rate (50-90% basierend auf Health)
  - Spell Range System (Inspector-definierbar)
  - Dynamic Slider Balancing (Speed/Zone Variation)
  - Particle Glow System (ersetzt Material Color Change)
  - Enemy Attack System Completion
- **Status:** Phase 2 Core Features COMPLETE, Backlog für Phase 3 definiert


---

## Phase 3: SCHÖN

### 17.02.2026 (Montag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| v0.3 Milestone: NavMesh Migration (Phase 3–5) | x | x | x |
| NavMeshSurface baken (GameLevel Scene) | x | x | x |
| NavMeshAgent auf 6 Snake-Prefabs konfigurieren | x | x | x |
| SnakeAI v1.8.1: NavMeshAgent Aktivierung (updatePosition=true) | x | x | x |
| SnakeAI v1.8.2: UpdatePatrol() → SetDestination + SamplePosition | x | x | x |
| SnakeAI v1.8.3: FollowPlayer + MoveAway → SetDestination, MoveTowardsSafe() gelöscht | x | x | x |
| Bug-Fix: Patrol-Animation-Sprung (Root Motion → In Place Clips) | x | x | x |
| Animator Controller: W Root → In Place für alle 3 Slither-States | x | x | x |
| LateUpdate() Sync: transform.position = agent.nextPosition | x | x | x |
| applyRootMotion = false im Script | x | x | x |

**Screenshot:** `Media/Screenshots/2026-02-17_NavMeshMigration.png`

**Notizen:**
- **Kern-Bug behoben:** Schlangen-Patrol-Animation sprang zu Frame 0 zurück wenn Collider getroffen
- **Root Cause (Dozent bestätigt):** MoveTowardsSafe() blockiert → _isPatrolling bool blieb true → Animation-Reset
- **Lösung Teil 1:** NavMeshAgent.SetDestination() ersetzt MoveTowardsSafe() — NavMesh navigiert um Hindernisse
- **Lösung Teil 2:** Animator-Clips getauscht: "Slither Forward/Left/Right W Root" → "In Place" Versionen
  - "W Root" Clips enthielten Root-Motion-Positions-Daten die gegen den NavMeshAgent kämpften
  - "In Place" Clips: nur Pose-Animation, keine Positions-Daten
- **Zusatz:** applyRootMotion = false im Script + LateUpdate() für manuellen Position-Sync
- NavMeshAgent: updatePosition=false, manuelle Sync via agent.nextPosition in LateUpdate()
- Commits: 355a6be (v1.8.1), 5d8ac55 (v1.8.2), 7ef80c6 (v1.8.3)


---

### 18.02.2026 (Dienstag) - Session 20: Phase 7 Spell System
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| GameEvents.cs: 9 neue Spell System Events + Invokers | x | x | x |
| TuneConfig.cs: SnakeEffect Enum auf Move/Daze/Shield reduziert | x | x | x |
| SpellScrollPickup.cs: Walk-over + Raycast Collection + Proximity Glow | x | x | x |
| SpellUnlockSystem.cs: Zelda-Style Pause Panel (TMPro + WaitForSecondsRealtime) | x | x | x |
| TuneController v3.0: 4 Einzelfelder → 3-Element Array + Unlock Gate | x | x | x |
| SpellHUDController.cs: Dynamische Slots (startet leer, wächst mit Scrolls) | x | x | x |
| ShieldComponent.cs: 8s Shield, Absorb/Expire Lifecycle, Screen-Edge-Glow | x | x | x |
| HealthSystem v1.4→v1.5: Shield Intercept + Heal-on-Charm (OnSnakeCharmed statt OnTuneSuccess) | x | x | x |
| SnakeAI v1.9: SnakeCharmed Event + Attack/Freeze Dead Code entfernt | x | x | x |
| TuneController v3.1: Range Check, Cooldown, Charges, Shield Wiring | x | x | x |
| SpellHUDController v1.1: Cooldown Overlay + Range Indicator | x | x | x |
| fix: UnlockTune4 aus GameManager + TuneConfigCreator auf 3 Tunes | x | x | x |
| Unity Editor: TuneConfigs Array, ShieldComponent, ScrollUnlockPanel, HUD Setup | x | x | |
| Unity Editor: 3 Scroll-GameObjects in Cave platzieren | x | | |
| 14-Punkte Play-Test Verification | x | | |

**Screenshot:** `Media/Screenshots/2026-02-18_.png`

**Notizen:**
- **Phase 7 Spell System — Code komplett, Editor Setup begonnen**
- **Kern-Änderung:** Von 4 Tunes (Move/Sleep/Attack/Freeze) auf 3 (Move/Daze/Shield)
  - Attack entfernt (braucht Creature System), Freeze entfernt (überlappt mit Daze)
  - Shield ist neue defensive Mechanik (8s, blockt nächsten Snake-Angriff)
- **Scroll Collection System:** Zelda-style — Scroll aufheben → Spiel pausiert → Panel zeigt Name/Beschreibung/Taste → Any Key → weiter
- **Unlock Progression:** Spieler startet mit 0 Tunes. Jeder Scroll schaltet 1 Tune frei. HUD wächst dynamisch.
- **Heal-on-Charm:** HP heilt NUR wenn Move/Daze tatsächlich eine Schlange charmt. Shield heilt nicht.
- **Casting Rules:** Range Check (Move/Daze brauchen Snake in Nähe), Cooldown (alle), Charges (Advanced Mode)
- **Dead Code Cleanup:** ~50 Zeilen Attack/Freeze Code aus SnakeAI entfernt (States, Methoden, Felder)
- **Bug Fix:** GameManager.cs rief noch UnlockTune4() auf → Compile Error → behoben
- **Editor Setup Status:** TuneConfigs zugewiesen, ShieldComponent auf Player, ScrollUnlockPanel + HUD teilweise gebaut
- **OFFEN:** SpellSlotPrefab als Asset speichern, SpellHUDManager erstellen, 3 Scrolls in Cave platzieren, Play-Test
- **Git:** 14 Commits auf main (b40d554 → df665e1), davon 12 Phase 7 Code + 1 Bug Fix + 1 Audio Feature


---

### 19.02.2026 (Mittwoch) - Session 21: Cave-Rebuild & Art-Restructure
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| v1.0 MVP Roadmap umstrukturiert (5 Phasen, 21 Requirements) | x | x | x |
| Cave Materialien: Standard-Shader → URP/Lit (4 Materials) | x | x | x |
| Pirate Assets in eigenen Ordner konsolidiert | x | x | x |
| Art-Visuals Ordnerstruktur vereinheitlicht (Cave, Pirate, Snakes, Props) | x | x | x |
| Pirate Animation System: Humanoid Rig + Bake Into Pose konfiguriert | x | x | x |
| Water Material + NavMesh nach Restructure wiederhergestellt | x | x | x |
| Spell System gefixt: TuneConfigs, Entranced State, Shield-Kopplung | x | x | x |
| GameManager Bindings + Scene-Referenzen repariert | x | x | x |

**Screenshot:** `Media/Screenshots/2026-02-19_.png`

**Notizen:**
- **Art-Visuals Restructure:** Alle Assets in einheitliche Struktur unter `_Project/Art-Visuals/Prefabs-FBX-Materials-Animations/` (Cave, Pirate, Snakes, Props)
- **Cave Materialien:** 4 Materials hatten Standard-Shader (erscheinen pink in URP) → auf URP/Lit Shader umgestellt. `materialSearch: Recursive-Up` damit Unity Materials in Unterordnern findet
- **Pirate Humanoid:** Animation System auf Humanoid Rig umgestellt + "Bake Into Pose" für alle Clips (Root Motion OFF — CharacterController steuert Bewegung)
- **Spell System Fix:** TuneConfigs korrekt zugewiesen, Entranced-State für Daze (zweiphasig: Entranced 3s → Dazed 8s), Shield-Kopplung repariert
- **Lesson Learned:** materialSearch Wert `1` (Local) findet keine Materialien in Unterordnern → `2` (Recursive-Up) nötig
- **Git:** feature/cave-rebuild Branch — 8 Commits (6b9dd85 → fd68d8d)


---

### 20.02.2026 (Donnerstag) - Session 22: Dokumentation & Merge
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| STATE.md aktualisiert — Spell System Session Handoff dokumentiert | x | x | x |
| feature/cave-rebuild in main gemergt (--no-ff) | x | x | x |
| Feature-Branch lokal + remote gelöscht | x | x | x |

**Screenshot:** `Media/Screenshots/2026-02-20_.png`

**Notizen:**
- **Merge:** feature/cave-rebuild → main (Commit e0c85cc) — enthält Cave-Rebuild, Spell System, Pirate Humanoid
- **Git Workflow:** Branch nach Merge lokal + remote gelöscht (Projekt-Regel: Ein Feature = Ein Branch, nach Merge löschen)
- **Status nach Merge:** main ist clean, alle cave-rebuild Änderungen integriert


---

### 21.02.2026 (Freitag) - Session 23: FBX Asset Recovery & Animator Cleanup
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| Root Cause identifiziert: `.gitignore: *.fbx` schließt alle FBX-Binaries aus | | x | x |
| Cave FBX: 7 Binaries aus External_Assets wiederhergestellt | x | x | x |
| Snake FBX: 16 Binaries (Toon Cobra + Animationen) wiederhergestellt | x | x | x |
| Pirate.FBX: GUID-korrekt wiederhergestellt (619359b8...) | x | x | x |
| Pirate Animations: 10/10 .meta aus git HEAD + FBX-Binaries platziert | x | x | x |
| Cave Materialien: URP/Lit Shader nach Stash-Operation zurückgesetzt | x | x | x |
| MC_Controller: Spell_Fear → Spell_Shield (Magic Spell Casting Anim) | x | x | x |
| MC_Controller: Spell_Attack State + SpellAttack Parameter gelöscht | x | x | x |
| MC_Controller: IsDead Parameter → IsDazed umbenannt | x | x | x |
| Alle 9 Animator-States mit Animationsclips verknüpft | x | x | x |
| Alte Platzhalter-Avatare entfernt (Malbers/Cowboy, SpaceRobotKyle) | x | x | x |
| feature/spell-editor-setup committed + gepusht + in main gemergt | x | x | x |

**Screenshot:** `Media/Screenshots/2026-02-21_.png`

**Notizen:**
- **Root Cause FBX fehlen:** `.gitignore: *.fbx` tracked keine FBX-Binaries. Nur `.meta`-Dateien im Repo. FBX-Dateien müssen manuell aus External_Assets-Quellen bereitgestellt werden
- **GUID-System:** Unity referenziert Assets per GUID (in .meta-Datei). Beim Wiederherstellen von FBX darf die `.meta`-Datei NICHT überschrieben werden — sonst brechen alle Scene/Prefab/Controller-Referenzen
- **Pirate.FBX GUID:** `619359b845787a443af41cf1ed1cfed0` — muss exakt stimmen, da Pirate-Prefab + MC_Controller diese GUID referenzieren
- **MC_Controller finales Setup:** 3 Spell States (Move, Daze, Shield) + 2 Death States + 4 Movement States. Parameter: Speed, IsCrouching, SpellMove, SpellDaze, SpellShield, IsDazed
- **Scripts bereits korrekt:** SnakeAI.cs nutzt `IsDazed`, TuneController.cs nutzt `SpellShield` — keine Code-Änderungen nötig nach Rename
- **Lesson Learned:** Mixamo FBX-Animationsclips im Animator zuweisen: FBX aufklappen (▶), den `mixamo.com`-Clip in Motion-Feld ziehen — nicht die FBX-Datei selbst
- **Git:** 2 Feature-Commits (de09e49, eb6c9d3) + Merge (f6777f8) auf main


---

## Phase 4: FERTIG

### 24.02.2026 (Montag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-24_.png`

**Notizen:**


---

### 25.02.2026 (Dienstag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-25_.png`

**Notizen:**


---

### 26.02.2026 (Mittwoch)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-02-26_.png`

**Notizen:**


---

### 27.02.2026 (Donnerstag) - Session: Phase 10 Audio & Music
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| MusicManager.cs v1.0 erstellen (scene-based, gameplay alternation) | x | x | x |
| MusicManager in MainMenu + GameLevel Scenes einbinden | x | x | x |
| Controls Overlay mit Keycap Images (UI) | x | x | x |
| Screenshots für Dokumentation erstellen (4 Screenshots) | x | x | x |
| feature/phase10-audio → main gemergt | x | x | x |

**Screenshot:** `Media/Screenshots/Screenshot 2026-02-27 211040.png`

**Notizen:**
- **MusicManager v1.0:** Scene-basiertes Musik-System — MainMenu-Musik und Gameplay-Musik getrennt
- **Gameplay Music Alternation:** Musik wechselt dynamisch während des Spiels
- **Controls Overlay:** Keycap-Images zeigen Tastaturbelegung im Spiel
- **Phase 10 COMPLETE:** Audio & Music vollständig integriert
- **Git:** feature/phase10-audio Branch gemergt (Commits: 7b8addc, d8b187e, 20855d1, 386c937, e57cac3)


---

### 01.03.2026 (Sonntag) - Session: General Improvements & Bug Fixes
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| Debug: Breath Attack (CancelInvoke Fix, _canSeePlayer Fix) | x | x | x |
| Advanced Mode Difficulty (+15% Snake-Damage, Drain-Rate 0.25f) | x | x | x |
| Charges-System aus TuneController entfernt (MVP Scope Reduction) | x | x | x |
| Snake Behavior Fix: Basic Snake FollowPlayer in Breath-Range | x | x | x |
| Shield Damage Bypass Fix: TakeSnakeAttackDamage() in HealthSystem | x | x | x |
| HealthBarUI: _debuffText / _debuffMessage entfernt (TMP = Single Source) | x | x | x |
| Gelbes Licht (Skybox außerhalb Cave) | x | x | x |

**Screenshot:** `Media/Screenshots/2026-03-01_.png`

**Notizen:**
- **Breath Attack Fix:** `CancelInvoke(nameof(ResetBreathBool))` vor Invoke verhindert doppelten Reset. `_canSeePlayer = true` Bugfix in else-Branch von UpdateProximityDetection
- **Advanced Difficulty:** Snake-Schaden ×1.15 wenn `_isAdvancedMode`, Drain von 0.115 auf 0.25 HP/sec
- **Charges System entfernt:** TuneController v3.3 — kein Charge-Counter mehr. Im GDD als Future Feature dokumentiert
- **Snake Behavior:** Basic Snakes haben kein Breath-Attack → verfolgen Spieler in Breath-Range statt zu warten. SnakeType-Check in `HandleIdlePlayerInteraction()`
- **Shield Fix:** SnakeAI rief `TakeDamage()` direkt auf (bypassed Shield-Check). Neues `TakeSnakeAttackDamage(int amount)` in HealthSystem — prüft Shield vor jedem Snake-Angriff. Beide Damage-Pfade (OnTriggerEnter + DealScheduledDamage) aktualisiert
- **HealthBarUI:** `_debuffText` + `_debuffMessage` SerializedFields entfernt. TMP-Child von ActiveEffectsWindow ist die einzige Text-Quelle
- **Branch:** `feature/general-improvements`

### 02.03.2026 (Montag)
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
|  |  |  |  |

**Screenshot:** `Media/Screenshots/2026-03-02_.png`

**Notizen:**


---

### 03.03.2026 (Dienstag) - ABGABE
| Aufgabe | geplant | in Bearbeitung | erledigt |
|---------|:-------:|:--------------:|:--------:|
| Finale Abgabe vorbereiten |  |  |  |
| ZIP erstellen |  |  |  |
| Präsentation halten |  |  |  |

**Screenshot:** `Media/Screenshots/2026-03-03_Final.png`

**Notizen:**


---

## Zusammenfassung

| Phase | Ziel | Ergebnis |
|-------|------|----------|
| 1 - Spielbar | Kern-Loop funktioniert | |
| 2 - Komplett | Alle Features | |
| 3 - Schön | Polish & Juice | |
| 4 - Fertig | Abgabe-Ready | |
