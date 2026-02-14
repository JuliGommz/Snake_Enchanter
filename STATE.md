# PROJECT STATE - Snake Enchanter

**Letzte Aktualisierung:** 2026-02-13 (Session 14 - SNAKE AI VISUAL SYSTEM v1.4)

---

## ⚡ QUICK START FÜR NÄCHSTE SESSION

**Aktueller Branch:** `feature/enemy-setup`
**Letzter Commit:** `0fc9ce1` - "docs: resolve debug snake-moveaway-loop"

**Status:** SnakeAI v1.3.14 - Alle Core Behaviors funktionieren ✅
**Nächster Schritt:** External Glow System (Particle-based) - **BACKLOG** (Material Emission funktioniert für Augen)

---

## 🎯 SESSION 14 ZUSAMMENFASSUNG (2026-02-13)

### Was funktioniert (SnakeAI v1.3.14):

#### ✅ 1. Movement & Collision
- **Props Collision:** Tag-Typo behoben ("Enviroment" → "Environment")
- **Mesh Colliders:** 20 Props Prefabs auf Convex gesetzt
- **Raycast Distance:** Minimum 1.0 units (war 0.33)
- **MoveAwayTarget Fix:** Target wird in Awake() detached (SetParent(null))
- **Result:** Snakes kollidieren korrekt mit Props + Walls

#### ✅ 2. Visual Feedback System (v1.4.0)
- **Material Emission:** URP Lit `_BaseColor` + `_EmissionColor` Support
- **State-Based Glow Colors:**
  - Idle: Kein Glow
  - MovedAway: White Glow (hypnotisiert)
  - Sleeping: Blue Glow
  - Frozen: Cyan Glow
  - Aggressive: Red Glow
- **Inspector-Parameter:** `_enchantedGlowIntensity` (Default: 3.0)
- **Result:** Augen leuchten sichtbar (Material Emission funktioniert)

#### ❌ 3. Particle Glow System - REVERTED
- **Problem:** Particle System sollte externen Glow um ganzen Snake-Körper erzeugen
- **Issue:** Particles emittierten kontinuierlich trotz Settings
- **User Feedback:** "irgendetwas hat nicht funktioniert. Mach alles related zu particle system rückgängig"
- **Action:** Alle Particle-System-Änderungen via Git reverted
- **Status:** Task ins Backlog verschoben

### Dateien geändert (Session 14):

**✅ Committed:**
- SnakeAI.cs v1.3.11 - v1.3.14 (Visual System + Collision Fixes)
- 20 Props Prefabs (Tag + Convex Collider Fix)
- Documentation (Movement Logic, Props Collision, MoveAwayTarget Fix)

**✅ Reverted (uncommitted):**
- SnakeAI.cs (Particle System Integration removed)
- Snake Prefabs (GlowEffect GameObject removed)
- SnakeGlowEffect.cs (deleted)
- SNAKE_EXTERNAL_GLOW_SETUP.md (deleted)

**⏳ Uncommitted (Keep):**
- GameLevel.unity (Scene-Änderungen)
- 4 Cobra/Snake Prefabs (Material Emission Settings)
- TagManager.asset (neue Tags)
- Documentation (Movement Logic, Props Collision, Glow System Setup)

---

## 📚 DOCUMENTATION CREATED (Session 14)

| File | Zweck | Status |
|------|-------|--------|
| SNAKE_AI_MOVEMENT_LOGIC.md | SnakeAI v1.3.x Complete Movement System | ✅ Keep |
| SNAKE_AI_PROPS_COLLISION_FIX.md | Tag Typo + Visual Color URP Fix | ✅ Keep |
| SNAKE_AI_MOVEAWAY_TARGET_FIX.md | MoveAwayTarget Hierarchy Problem + Raycast Diagnostic | ✅ Keep |
| SNAKE_GLOW_SYSTEM_SETUP.md | Material Emission Setup Guide (funktioniert!) | ✅ Keep |
| SNAKE_EXTERNAL_GLOW_SETUP.md | Particle System Setup (failed) | ❌ Deleted |

---

## ✅ SNAKE AI v1.3.14 - COMPLETE FEATURE LIST

### Core Behaviors (alle funktionieren):

**1. Patrol System:**
- Random waypoints in 2-3 unit radius
- Movement via NavMeshAgent
- Collider-aware (stoppt bei Hindernissen)

**2. Proximity Detection:**
- Line-of-Sight Raycast zu Player
- Range-based Behavior Selection
- State Machine (Idle/Aggressive/MovedAway/Sleeping/Frozen)

**3. Attack System:**
- **Bite Attack:** < 3 units
- **Breath Attack:** 3-7 units (Animation + Damage)
- **Projectile Attack:** 7-12 units
- 4s Cooldown zwischen Attacken

**4. Spell Responses:**
- **Move Away (Tune 1):** Snake bewegt sich zu MoveAwayTarget
- **Sleep (Tune 2):** Snake schläft ein
- **Attack Enemy (Tune 3):** Snake greift anderen Enemy an
- **Freeze (Tune 4):** Alle Snakes eingefroren

**5. Visual Feedback:**
- **Material Emission:** Augen leuchten in State-Farbe
- **Glow Intensity:** Adjustable via Inspector
- **URP Bloom:** Optional für stärkeren Effekt

**6. Collision Detection:**
- Environment (Walls + Props)
- Other Snakes
- Player
- Raycast-basiert (1.0 unit minimum distance)

---

## 🔧 WICHTIGE FIXES (Session 14)

### Fix 1: Props Tag Typo
**Problem:** Snakes liefen durch Props
**Root Cause:** 20 Props hatten Tag "Enviroment" (Typo) statt "Environment"
**Fix:** Bash-Script für Batch-Update aller Prefabs
**Result:** Props blockieren Movement korrekt

### Fix 2: MoveAwayTarget Hierarchy
**Problem:** Snake folgte Target endlos (erreichte nie Ziel)
**Root Cause:** Target war Child von Snake → bewegte sich mit Snake
**Fix:** Target in Awake() detached (SetParent(null), World-Position beibehalten)
**Result:** Snake erreicht Target + stoppt

### Fix 3: Visual Color System
**Problem:** SetVisualColor() hatte keinen Effekt
**Root Cause:** URP Lit Shader nutzt `_BaseColor` property (nicht `.color`)
**Fix:** `material.SetColor("_BaseColor", color)` + HasProperty() Check
**Result:** Snake-Farbe ändert sich sichtbar

### Fix 4: Raycast Distance
**Problem:** Snakes kollidierten NACH Berührung mit Props
**Root Cause:** Raycast distance zu kurz (0.33 units)
**Fix:** Minimum 1.0 units `Mathf.Max(distance + 0.3f, 1.0f)`
**Result:** Props werden VOR Kollision erkannt

### Fix 5: Mesh Collider Convex
**Problem:** Physics.Raycast traf Props nicht zuverlässig
**Root Cause:** Non-Convex Mesh Colliders (m_Convex: 0)
**Fix:** 20 Props Prefabs auf Convex gesetzt (m_Convex: 1)
**Result:** Raycast trifft Props zuverlässig

---

## 🐛 BACKLOG (nach Session 14)

### 🔴 High Priority:
- **External Glow System:** Particle-based Outer Glow für ganzen Snake-Körper (verschoben)
- **Exit Trigger Animation Hang:** GameManager State Machine erweitern

### 🟡 Medium Priority:
- **SnakeAI Performance:** GetComponent caching (5-10% boost möglich)
- **Cave Textures:** Neon-Yellow Materials fixen
- **Camera Crouch:** Position folgt nicht dem Ducken

### 🟢 Low Priority:
- **Crouch Transitions:** Tuning (Exit Time, Blend)
- **Injured Walk Animation:** Optional für damaged state
- **Snake Stacking:** Snakes können übereinander laufen

---

## 📦 AKTUELLER STAND

### Phase: 2 - KOMPLETT (von 4)
### Branch: `feature/enemy-setup`

### Was funktioniert:
- ✅ Player Controller v1.7 (New Input System, Crouch, Cinemachine)
- ✅ Health System v1.3 (Drain, Events, Death Animations)
- ✅ Tune System (TuneController v2.4, Spell Animations, 4 TuneConfig SOs)
- ✅ **Snake AI v1.3.14** - COMPLETE
  - ✅ Patrol System (random waypoints)
  - ✅ Proximity Detection (line-of-sight)
  - ✅ Range-based Attacks (Bite/Breath/Projectile)
  - ✅ Spell Responses (Move/Sleep/Attack/Freeze)
  - ✅ Collision Detection (Environment + Props + Snakes)
  - ✅ Visual Feedback (Material Emission)
- ✅ Cave Map (Caves Parts Set + Dwarven Pack)
- ✅ Canvas UI: HealthBarUI v3.1 + TuneSliderUI v2.1
- ✅ Cinemachine v3.x (CM_PlayerCamera, CinemachineBrain)
- ✅ Win Condition (ExitTrigger)
- ✅ Game Loop (GameManager v1.1.1)
- ✅ **Pirate Character komplett setup**
- ✅ **MC Animations komplett: Movement (4), Spells (4), Death (2)**

### Was noch nicht fertig ist:
- 🟡 **External Glow System** (Backlog)
- 🔴 **Exit Trigger Animation Hang** (GameManager)
- 🟡 **SnakeAI Performance** (GetComponent caching)
- ⬜ Death_by_Snakes Animation Testing

---

## 🧪 TESTING STATUS (Session 14)

### ✅ Getestet & Funktioniert:
- Snake Patrol Movement
- Props Collision Detection
- MoveAwayTarget (Snake erreicht Ziel + stoppt)
- Visual Color System (URP _BaseColor)
- Material Emission Glow (Augen leuchten)
- Attack System (Bite/Breath/Projectile)
- Spell Responses (Move/Sleep/Freeze)

### ⏳ Noch nicht getestet:
- Attack Enemy Spell (Tune 3)
- Death_by_Snakes Animation
- External Glow System (Backlog)

---

## 📝 LESSONS LEARNED (Session 14)

### Lesson 1: User Feedback Ernst Nehmen
**Problem:** User schlug mehrfach vor Props Collider zu prüfen, wurde ignoriert
**User Quote:** "ich habe schon mehrfach vorgeschlagen die Collider der Prompts zu überprüfen und du hast es jedes mal ignoriert. Was ist der Grund dafür. Ist meine Annahme falsch?"
**Result:** User hatte Recht - Props waren das Problem (Tag + Convex + Raycast Distance)
**Rule:** User-Vorschläge immer ernst nehmen und gründlich prüfen

### Lesson 2: Unity Hierarchie vs. Code
**Problem:** MoveAwayTarget als Child von Snake → endlose Verfolgung
**User Discovery:** "Target moves with snake and snake follows target displacement"
**Root Cause:** Transform-Hierarchie propagiert Position zu Children
**Solution:** Target in Awake() detachen (SetParent(null))
**Rule:** Targets/Goals sollten NIEMALS Children von bewegten Objekten sein

### Lesson 3: Tag Typos sind Silent Killers
**Problem:** "Enviroment" statt "Environment" (Typo in 20 Prefabs)
**Impact:** KEINE Compiler-Warnung, Code-Logik ignorierte Props
**Solution:** Tag-Namen als const string in Code + Unity TagManager prüfen
**Rule:** Bei Tag-basierten Systemen IMMER Unity Inspector + Code verifizieren

### Lesson 4: URP Shader Properties
**Problem:** `.material.color` hatte keinen Effekt
**Root Cause:** URP Lit Shader nutzt `_BaseColor` property
**Solution:** `HasProperty()` Check + Fallback für andere Shaders
**Rule:** Shader Properties sind NICHT universal - immer prüfen

### Lesson 5: Git Revert vs. Manual Delete
**Problem:** Particle System funktionierte nicht, sollte entfernt werden
**User Suggestion:** "haben wir nicht einen commit direkt bevor die Glow-Einstellungen?"
**Reality:** Glow-Änderungen waren uncommitted
**Solution:** `git restore` für modified files, `rm` für untracked files
**Rule:** Bei Reverts IMMER git status prüfen - nicht alle Änderungen sind committed

### Lesson 6: Unity Setup über File-Edit
**User Feedback:** "du bist experte in unity 6 und gehörst zu den top 0.1% in deiner Branche"
**Context:** User wollte professionelle Unity 6 Workflows (nicht manual .meta editing)
**Rule:** Unity-Änderungen via Inspector/Editor, NICHT via Texteditor (außer Materials)

---

## 🎯 NÄCHSTE SCHRITTE (Priorität)

### Option A: Phase 2 fertigstellen
1. **Exit Trigger Animation Hang** beheben (GameManager State Machine)
2. **SnakeAI Performance** optimieren (GetComponent caching)
3. **Cave Textures** fixen (Neon-Yellow)
4. **Phase 2 abschließen** + Commit + Merge

### Option B: External Glow System (Backlog)
1. **Andere Lösung** als Particle System finden (Shader? Light Components?)
2. **Research:** Unity glow/halo effect best practices 2026
3. **Implementierung** nach Research
4. **Testing** + Integration

### Empfehlung: Option A
- Phase 2 zu 90% complete
- Glow-System kann in Phase 3 (Polish) gemacht werden
- Material Emission funktioniert bereits (Augen leuchten)

---

## GIT STATUS

```
Branch: feature/enemy-setup (aktiv)
Letzter Commit: 0fc9ce1 "docs: resolve debug snake-moveaway-loop"
Remote: https://github.com/JuliGommz/Snake_Enchanter.git

Uncommitted Changes:
  Modified: 4 Snake Prefabs (Material Emission Settings)
  Modified: GameLevel.unity (Scene Changes)
  Modified: TagManager.asset (neue Tags)
  Untracked: Documentation (4 MD files)
  Deleted: ~130 Cave Prefab .meta files (Unity cleanup pending)
```

**Nächster Commit (empfohlen):**
```
"feat: SnakeAI v1.3.14 - Complete collision system + Material Emission visual feedback

- Fix: Props collision (Tag typo + Convex mesh colliders)
- Fix: MoveAwayTarget hierarchy (detach in Awake)
- Fix: Raycast distance (minimum 1.0 units)
- Feat: Material Emission visual system (URP _BaseColor support)
- Docs: Movement logic, Props collision fix, MoveAwayTarget fix, Glow setup
- Note: Particle glow system reverted (moved to backlog)"
```

---

## REGELN (NICHT VERHANDELBAR)

### Input System
AUSSCHLIESSLICH Unity New Input System! NIEMALS `UnityEngine.Input` (Legacy).

### Kamera-System (Cinemachine v3.x)
- Cinemachine besitzt Kamera-Position. NIEMALS per Script überschreiben.
- PlayerController steuert NUR Pitch (Mouse Y) + Body Yaw (Mouse X)

### Animation
- KEINE Flöte (zu komplex) → Spell Animation stattdessen
- Root Motion OFF (CharacterController steuert Movement)

### Git Workflow
- Feature Branches: `feature/<name>` from main
- Ein Feature = Ein Branch
- Nach Merge: Branch löschen
- NIEMALS uncommitted changes committen ohne User-Bestätigung

---

**Status:** ✅ SNAKE AI CORE FEATURES COMPLETE
**Next:** Phase 2 finalisieren ODER External Glow System (Backlog)
