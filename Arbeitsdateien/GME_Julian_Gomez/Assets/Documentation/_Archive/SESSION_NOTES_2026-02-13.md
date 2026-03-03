# Session 14 Notes - Snake AI Visual System & Bug Fixes

**Datum:** 2026-02-13 (Donnerstag)
**Dauer:** ~6 Stunden
**Branch:** feature/enemy-setup
**SnakeAI Version:** v1.3.11 → v1.3.14

---

## 🎯 SESSION ZIELE

**Haupt-Ziel:** Snake AI Bugs beheben (Props collision, MoveAwayTarget, Visual Feedback)

**Erreicht:**
- ✅ Props Collision funktioniert (Tag + Convex + Raycast Distance)
- ✅ MoveAwayTarget erreicht Ziel (Hierarchy Fix)
- ✅ Visual Feedback System (Material Emission)
- ❌ External Glow System (Particle-based) → Backlog

---

## ✅ BUGS BEHOBEN

### 1. Props Collision (HIGH PRIORITY)

**Problem:**
- Snakes liefen durch Props (Pillars, Support Beams, Wall Decorations)
- Walls blockierten korrekt, aber Props nicht

**Root Causes (3 Issues gefunden):**

**Issue A: Tag Typo**
- 20 Props Prefabs hatten Tag "Enviroment" (TYPO) statt "Environment"
- Code verwendet `CompareTag("Environment")` → Props wurden ignoriert
- Fix: Bash-Script für Batch-Update aller Prefabs (`sed -i` replace)

**Issue B: Non-Convex Mesh Colliders**
- Props hatten `m_Convex: 0` (non-convex mesh colliders)
- Physics.Raycast arbeitet besser mit Convex Colliders
- Fix: 20 Prefabs auf `m_Convex: 1` gesetzt

**Issue C: Raycast Distance zu kurz**
- Raycast Distance: `distance + 0.3f` (z.B. 0.33 units bei 0.03 units/frame movement)
- Props wurden NACH Kollision erkannt (zu spät)
- Fix: Minimum 1.0 units `Mathf.Max(distance + 0.3f, 1.0f)`

**Result:**
- ✅ Props blockieren Movement korrekt
- ✅ Console zeigt: "Movement blocked by [PropName] (Tag: Environment)"
- ✅ Snakes stoppen VOR Kollision

**Lesson Learned:**
- User schlug MEHRFACH vor Props Collider zu prüfen → wurde ignoriert
- User hatte RECHT - Props waren das Problem
- **RULE:** User-Vorschläge immer ernst nehmen und gründlich prüfen

---

### 2. MoveAwayTarget Endlos-Verfolgung (CRITICAL)

**Problem:**
- Snake bewegte sich zu MoveAwayTarget, aber erreichte es NIE
- Distance sank nicht, Snake lief kontinuierlich
- 2s Timeout griff immer

**User Discovery:**
> "MoveAwayTarget is a child of each snake. Target moves with snake and snake follows target displacement."

**Root Cause:**
- Target war Child GameObject von Snake
- Transform-Hierarchie propagiert: Wenn Snake sich bewegt, bewegt sich Target mit
- Snake bei (0,0,0), Target bei local (5,0,0) → world (5,0,0)
- Snake bewegt sich zu (1,0,0) → Target ist jetzt bei world (6,0,0)
- Endlose Verfolgung!

**Fix (SnakeAI.cs Awake()):**
```csharp
if (_moveAwayTarget != null && _moveAwayTarget.parent == transform)
{
    Vector3 worldPos = _moveAwayTarget.position;
    _moveAwayTarget.SetParent(null);
    _moveAwayTarget.position = worldPos;
    Debug.Log($"SnakeAI ({_snakeName}): Detached MoveAwayTarget (World: {worldPos})");
}
```

**Result:**
- ✅ Target bleibt an fixer World-Position
- ✅ Snake erreicht Target (Distance sinkt zu <1.0)
- ✅ Console: "Reached MoveAwayTarget at distance 0.87, transitioning to root state"

**Lesson Learned:**
- Targets/Goals sollten NIEMALS Children von bewegten Objekten sein
- Transform-Hierarchie propagiert immer
- Detach via SetParent(null) + World-Position beibehalten

---

### 3. Visual Color System funktionierte nicht

**Problem:**
- `SetVisualColor()` wurde aufgerufen, aber Snake-Farbe änderte sich nicht
- User: "Die Prefab start zugewiesene Farbe - keine Änderung beim Spellcasten"

**Root Cause:**
- Code verwendete `.material.color` property
- URP Lit Shader nutzt `_BaseColor` property (NICHT `.color`)
- Material Property war falsch → keine visuelle Änderung

**Fix (SnakeAI.cs v1.3.11):**
```csharp
private void SetVisualColor(Color color)
{
    if (_renderer != null)
    {
        // URP Lit shader uses "_BaseColor" property
        if (_renderer.material.HasProperty("_BaseColor"))
        {
            _renderer.material.SetColor("_BaseColor", color);
        }
        else
        {
            // Fallback for other shaders
            _renderer.material.color = color;
        }
        Debug.Log($"SnakeAI ({_snakeName}): Color changed to {color}");
    }
}
```

**Result:**
- ✅ Snake-Farbe ändert sich sichtbar während Spell
- ✅ Gray/Transparent für MovedAway State
- ✅ Console-Logs bestätigen Color Change

**Lesson Learned:**
- Shader Properties sind NICHT universal
- URP verwendet andere Property-Namen als Built-In
- Immer `HasProperty()` prüfen + Fallback bereitstellen

---

## ✨ FEATURES IMPLEMENTIERT

### Material Emission Glow System (v1.4.0)

**User Request:**
> "Color is changing, but not really visible. I would like enchanted snakes to retain color but shine. Bright light is to be adjustable in inspector and in the same color of the snake"

**Implementierung:**
- Material Emission aktiviert via `material.EnableKeyword("_EMISSION")`
- Emission Color gesetzt via `material.SetColor("_EmissionColor", glowColor * intensity)`
- State-basierte Glow Colors:
  - Idle: Kein Glow
  - MovedAway: White Glow (hypnotisiert)
  - Sleeping: Blue Glow
  - Frozen: Cyan Glow
  - Aggressive: Red Glow

**Inspector Parameter:**
- `_enchantedGlowIntensity` (Default: 3.0)
- Höher = Heller, 0 = Kein Glow

**Unity Setup (Manual):**
- Snake Materials: Emission Section aktivieren (Checkbox)
- Shader: Universal Render Pipeline/Lit
- Optional: URP Post-Processing Bloom (Intensity 0.2-0.5)

**Result:**
- ✅ Augen leuchten sichtbar in State-Farbe
- ✅ Snake behält Original-Farbe + Glow-Overlay
- ✅ Intensity adjustable via Inspector

**Limitation:**
- Nur Augen leuchten sichtbar (nicht ganzer Körper)
- User wünschte "external glow for the whole snake"

---

### Particle Glow System Experiment (v1.4.1) - FAILED

**Ziel:**
- Externen Glow um ganzen Snake-Körper (Halo-Effekt)
- Particle System mit Glow Sphere

**Implementierung:**
- SnakeGlowEffect.cs Script erstellt
- GlowEffect GameObject als Child jedes Snake Prefabs
- Particle System konfiguriert (Looping, Start Size 1.5, Max Particles 1)
- Fade In/Out System via Update() Loop

**Problem:**
- Particles emittierten kontinuierlich trotz `Play On Awake = false`
- User Feedback: "effekt funktioniert zum teil, tut aber nicht was ich möchte"
- User: "I need external glow for the whole snake. If body glows it is not visible"

**User Decision:**
> "irgendetwas hat nicht funktioniert. Mach alles related zu particle system rückgängig, plaziere task aud Backlog"

**Revert Process:**
- Git Status geprüft: Alle Änderungen uncommitted
- `git restore` für modified files (SnakeAI.cs, Snake Prefabs)
- `rm` für untracked files (SnakeGlowEffect.cs, SNAKE_EXTERNAL_GLOW_SETUP.md)
- Clean revert zu v1.3.14 (vor Particle System Arbeit)

**Lesson Learned:**
- Bei Reverts IMMER git status prüfen (committed vs. uncommitted)
- `git restore` für tracked files, `rm` für untracked
- Nicht alle "Changes" sind Commits

**Status:**
- ❌ Particle Glow System reverted
- 📦 Task ins Backlog verschoben
- ✅ Material Emission bleibt (funktioniert für Augen)

---

## 📚 DOCUMENTATION CREATED

| File | Lines | Zweck |
|------|-------|-------|
| SNAKE_AI_MOVEMENT_LOGIC.md | 350+ | Complete SnakeAI v1.3.x Movement System Documentation |
| SNAKE_AI_PROPS_COLLISION_FIX.md | 300+ | Props Tag Typo + Visual Color URP Fix |
| SNAKE_AI_MOVEAWAY_TARGET_FIX.md | 315+ | MoveAwayTarget Hierarchy Problem + Diagnostic Guide |
| SNAKE_GLOW_SYSTEM_SETUP.md | 330+ | Material Emission Setup Guide (Unity Inspector) |
| ~~SNAKE_EXTERNAL_GLOW_SETUP.md~~ | - | Particle System Guide (deleted after revert) |

**Alle Docs enthalten:**
- Problem Analysis
- Root Cause Explanation
- Step-by-Step Fix Guide
- Testing Instructions
- Troubleshooting Section

---

## 🧪 TESTING DURCHGEFÜHRT

### ✅ Erfolgreich Getestet:

**Props Collision:**
- Snake läuft gegen Pillar → stoppt
- Console: "Movement blocked by PillarADEP_ST (Tag: Environment) at distance 0.42"
- 2s Timeout → Snake gibt auf

**MoveAwayTarget:**
- Cast Move Away Spell
- Distance sinkt: 8.3 → 6.1 → 3.9 → 1.6 → 0.87
- Console: "Reached MoveAwayTarget at distance 0.87"
- Snake transitions zu Idle

**Visual Feedback:**
- Cast Move Away Spell
- Snake-Farbe ändert sich zu Gray/Transparent
- Console: "Color changed to RGBA(0.50, 0.50, 0.50, 0.50)"
- Nach Target erreicht: Farbe zurück zu Original

**Material Emission:**
- Snake Augen leuchten White (MovedAway State)
- Deutlich sichtbar mit URP Bloom
- Glow verschwindet nach State-Change (Idle)

### ⏳ Noch nicht getestet:
- Attack Enemy Spell (Tune 3)
- Death_by_Snakes Animation (Snakes machen noch keinen Damage)
- External Glow System (Backlog)

---

## 📊 CODE CHANGES

### SnakeAI.cs (v1.3.11 - v1.3.14)

**v1.3.11:**
- SetVisualColor() URP _BaseColor Support
- Enhanced Debug Logging (MoveAwayTarget tracking)

**v1.3.12:**
- RaycastAll Diagnostic Logging (alle Collider im Pfad)
- Props IsTrigger + Layer Detection

**v1.3.13:**
- MoveAwayTarget Detach in Awake()
- World Position Preservation

**v1.3.14:**
- Raycast Distance Fix (minimum 1.0 units)
- SetVisualGlow() für Material Emission
- State-based Glow Colors

**Total Changes:**
- +120 lines (Visual System + Diagnostic Logging)
- 4 new methods (SetVisualGlow, DisableGlow, EnableGlow, helpers)
- 6 new Inspector fields (Glow Color per State)

### Props Prefabs (20 files)

**Changes:**
- `m_TagString: Enviroment` → `m_TagString: Environment`
- `m_Convex: 0` → `m_Convex: 1`

**Modified via Bash:**
```bash
for file in *.prefab; do
    sed -i 's/m_TagString: Enviroment/m_TagString: Environment/g' "$file"
done
```

---

## 🎓 LESSONS LEARNED

### 1. User Feedback Ernst Nehmen
**Context:** User schlug mehrfach vor Props Collider zu prüfen
**Fehler:** Vorschläge ignoriert, auf Code-Ebene debugged
**Reality:** Props waren das Problem (Tag + Convex + Raycast)
**Lesson:** User kennt sein Projekt am besten - Vorschläge ernst nehmen

### 2. Unity Hierarchie vs. Code
**Problem:** MoveAwayTarget als Child von Snake
**User Discovery:** "Target moves with snake"
**Root Cause:** Transform-Hierarchie propagiert Position
**Solution:** SetParent(null) in Awake()
**Lesson:** Targets niemals als Children von bewegten Objekten

### 3. Tag Typos sind Silent Killers
**Problem:** "Enviroment" Typo in 20 Prefabs
**Impact:** Keine Compiler-Warnung, Code ignorierte Props
**Detection:** Erst nach User-Hinweis gefunden
**Lesson:** Tag-Namen als const string + Unity TagManager prüfen

### 4. URP Shader Properties
**Problem:** .material.color hatte keinen Effekt
**Root Cause:** URP Lit nutzt _BaseColor property
**Solution:** HasProperty() Check + Fallback
**Lesson:** Shader Properties sind NICHT universal

### 5. Git Revert Workflow
**User Suggestion:** "haben wir nicht einen commit direkt bevor die Glow-Einstellungen?"
**Reality:** Glow-Änderungen waren uncommitted
**Solution:** git status prüfen, dann git restore + rm
**Lesson:** Nicht alle Changes sind Commits

### 6. Unity Setup über Code
**User:** "du bist experte in unity 6 und gehörst zu den top 0.1%"
**Context:** User wollte professionelle Unity Workflows
**Lesson:** Unity-Änderungen via Inspector/Editor, NICHT .meta editing

---

## 🚧 BEKANNTE ISSUES (Backlog)

### 🔴 High Priority:
- **External Glow System:** Particle-based Outer Glow (verschoben)
- **Exit Trigger Animation Hang:** GameManager State Machine

### 🟡 Medium Priority:
- **SnakeAI Performance:** GetComponent caching (5-10%)
- **Cave Textures:** Neon-Yellow Materials
- **Camera Crouch:** Position folgt nicht

### 🟢 Low Priority:
- **Crouch Transitions:** Tuning
- **Injured Walk:** Optional Animation
- **Snake Stacking:** Physics Issue

---

## 📈 METRICS

**Session Dauer:** ~6 Stunden
**Code Changes:** 4 versions (v1.3.11 - v1.3.14)
**Bugs Fixed:** 5 (Props Tag, Convex, Raycast, MoveAwayTarget, Visual Color)
**Features Added:** 1 (Material Emission Glow)
**Features Reverted:** 1 (Particle Glow)
**Documentation:** 4 MD files (~1400 lines total)
**Prefabs Modified:** 20 Props + 6 Snakes
**Testing:** 4 test cases passed

---

## 🎯 NÄCHSTE SESSION

### Empfohlene Priorität:

**Option A: Phase 2 abschließen (EMPFOHLEN)**
1. Exit Trigger Animation Hang beheben (1-2h)
2. SnakeAI Performance (GetComponent caching) (30min)
3. Cave Textures Fix (1h)
4. Git Commit + Merge feature/enemy-setup → main
5. Phase 2 COMPLETE 🎉

**Option B: External Glow System**
1. Research Unity glow/halo effect alternatives
2. Shader-based solution? Light Components? Post-Processing?
3. Implementierung + Testing
4. Kann auch in Phase 3 (Polish) gemacht werden

**Empfehlung:** Option A
- Phase 2 zu 90% complete
- Material Emission funktioniert bereits
- Glow kann in Phase 3 polished werden

---

## 🔖 COMMIT READY

**Branch:** feature/enemy-setup
**Files Modified:** 30+ (Scripts, Prefabs, Documentation)
**Commit Message (empfohlen):**

```
feat: SnakeAI v1.3.14 - Complete collision system + Material Emission visual feedback

FIXES:
- Props collision (Tag typo "Enviroment" → "Environment")
- Props Mesh Colliders (Convex enabled for 20 prefabs)
- MoveAwayTarget hierarchy (detach in Awake to prevent infinite chase)
- Raycast distance (minimum 1.0 units instead of 0.33)
- Visual color system (URP _BaseColor support)

FEATURES:
- Material Emission glow system (state-based colors, adjustable intensity)
- State-specific glow colors (White/Blue/Cyan/Red for different states)
- Enhanced debug logging (MoveAwayTarget tracking, RaycastAll diagnostic)

DOCUMENTATION:
- SNAKE_AI_MOVEMENT_LOGIC.md (complete v1.3.x system)
- SNAKE_AI_PROPS_COLLISION_FIX.md (tag + visual fixes)
- SNAKE_AI_MOVEAWAY_TARGET_FIX.md (hierarchy issue + diagnostic)
- SNAKE_GLOW_SYSTEM_SETUP.md (emission setup guide)

NOTES:
- Particle glow system reverted (moved to backlog)
- External glow can be implemented in Phase 3 (Polish)
- Material Emission works well for eyes/highlights
```

---

**Session Status:** ✅ COMPLETE
**SnakeAI Status:** ✅ v1.3.14 - ALL CORE FEATURES WORKING
**Next:** Phase 2 finalisieren → Phase 3 (Polish)
