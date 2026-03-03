# Snake AI - MoveAwayTarget Hierarchy Fix + Props Diagnostic

**Session 14 - 2026-02-13**
**SnakeAI Version:** v1.3.12

---

## 🚨 CRITICAL ISSUE DISCOVERED

### User Report:
> "MoveAwayTarget is a child of each snake (each individual snake needs a target and that was the viable solution at the moment). **Target moves with snake and snake follows target displacement.**"

---

## 🐛 ROOT CAUSE: Parent-Child Hierarchy Problem

### The Issue:

**Current Hierarchy (FALSCH):**
```
Toon Snake - Green (Root)
  ├─ Skeleton/Mesh
  └─ MoveAwayTarget (Child) ← PROBLEM!
```

**Was passiert:**
1. Snake ist bei Position `(0, 0, 0)`
2. MoveAwayTarget ist Child mit **lokaler** Position `(5, 0, 0)`
   - → **World Position:** `(5, 0, 0)` ✅
3. Snake bewegt sich zu `(1, 0, 0)`
4. **Transform-Hierarchie propagiert!**
   - MoveAwayTarget bleibt bei lokaler Position `(5, 0, 0)`
   - → **Neue World Position:** `(6, 0, 0)` ❌
5. Snake folgt dem **bewegenden Target** → **Endlos-Verfolgung!**

### Symptome:
- ✅ "Snakes laufen in andere (ähnliche) Richtung" → Target verschiebt sich kontinuierlich
- ✅ "Snakes stoppen nicht" → Target ist nie erreichbar (bewegt sich weg)
- ✅ "2s Timeout greift immer" → Snake gibt auf, weil Target unerreichbar
- ✅ "Snake follow target behavior resets after a couple of seconds" → Timeout-Mechanismus funktioniert

**User-Bestätigung:**
> "What works good: Snake follow target behavior resets after a couple of seconds as planned."

→ Das bedeutet der **Code ist korrekt**, nur die **Scene-Setup ist falsch**!

---

## ✅ LÖSUNG: MoveAwayTarget aus Hierarchie entfernen

### Option A: Scene-Level Targets (EMPFOHLEN für Phase 2)

**Korrekte Hierarchie:**
```
GameLevel Scene
├─ Environment
│   └─ QuestRoom_1
│       └─ Props (WallPropsBottomA_ST, etc.)
├─ Snakes
│   ├─ Toon Snake - Green
│   └─ Toon Snake - Purple
└─ MoveAwayTargets (Empty GameObject als Container)
    ├─ MoveAwayTarget_Snake1
    └─ MoveAwayTarget_Snake2
```

**Eigenschaften jedes Targets:**
- **Tag:** `MoveAwayTarget` ✅
- **Position:** Feste World-Koordinaten (z.B. `(10, 0, 10)`)
- **Collider:** BoxCollider mit `Is Trigger = false`
- **Parent:** Scene Root ODER MoveAwayTargets Container (NICHT Snake!)

---

### Manuelle Unity-Schritte:

#### 1. Targets aus Snake-Prefab entfernen

**Pro Snake-Prefab:**
1. Öffne Prefab "Toon Snake - Green" im Prefab-Editor
2. Wähle Child GameObject "MoveAwayTarget"
3. **Rechtsklick → Unpack Prefab Completely** (falls es ein Prefab ist)
4. **DELETE** das MoveAwayTarget GameObject
5. Speichere Prefab (`Ctrl+S`)
6. Wiederhole für "Toon Snake - Purple"

#### 2. Targets in Scene erstellen

**In GameLevel Scene:**
1. Erstelle Empty GameObject: `MoveAwayTargets` (Container)
2. Pro Snake: Erstelle Child GameObject unter MoveAwayTargets
   - Name: `MoveAwayTarget_Snake1`, `MoveAwayTarget_Snake2`
3. **Für jedes Target:**
   - **Transform Position:** Setze feste World-Koordinaten
     - Beispiel: Snake1 → `(10, 0, 10)`, Snake2 → `(-5, 0, 15)`
   - **Tag:** Wähle "MoveAwayTarget" im Inspector
   - **Add Component:** Box Collider
     - Size: `(1, 1, 1)` (anpassen nach Bedarf)
     - Is Trigger: **NO** (unchecked) ✅
   - **Layer:** Default (oder beliebig, Raycast filtert nicht nach Layer)

#### 3. Targets zu Snakes zuweisen

**Pro Snake in der Scene:**
1. Wähle Snake GameObject (z.B. "Toon Snake - Green")
2. Im Inspector → SnakeAI Component
3. **Move Away Target Field:**
   - Drag & Drop das entsprechende MoveAwayTarget aus der Hierarchy
   - Snake1 → MoveAwayTarget_Snake1
   - Snake2 → MoveAwayTarget_Snake2
4. Verifiziere dass Field NICHT leer ist (None)

---

### Option B: Dynamische Target-Erstellung (Phase 3 - Advanced)

**Code-basierte Lösung** (für später, wenn TuneController erweitert wird):

```csharp
// In TuneController.OnMoveAwaySuccess():
GameObject targetObj = new GameObject($"MoveAwayTarget_{snake.name}");
targetObj.transform.position = snake.position + snake.forward * 5f; // 5 units voraus
targetObj.transform.rotation = Quaternion.identity; // No rotation
targetObj.tag = "MoveAwayTarget";

BoxCollider collider = targetObj.AddComponent<BoxCollider>();
collider.size = new Vector3(1f, 1f, 1f);
collider.isTrigger = false;

snake.GetComponent<SnakeAI>().SetMoveAwayTarget(targetObj.transform);

// WICHTIG: Destroy nach Snake erreicht Target oder gibt auf!
Destroy(targetObj, 10f); // Cleanup nach 10 Sekunden
```

**Vorteil:** Keine manuelle Scene-Setup
**Nachteil:** Mehr Code, schwieriger zu debuggen

---

## 🔍 Props Collision - Diagnostic Logging

### Zweites Problem:
User berichtet: "snake still ignore Props collider" TROTZ Tag-Fix

### Neue Diagnostic-Features (v1.3.12):

**RaycastAll() Logging hinzugefügt:**
```csharp
RaycastHit[] allHits = Physics.RaycastAll(rayOrigin, direction, rayDistance);
if (allHits.Length > 0 && Time.frameCount % 60 == 0)
{
    Debug.Log($"SnakeAI ({_snakeName}): RaycastAll found {allHits.Length} colliders in path:");
    foreach (var h in allHits)
    {
        Debug.Log($"  - {h.collider.name} | Tag: {h.collider.tag} | IsTrigger: {h.collider.isTrigger} | Layer: {LayerMask.LayerToName(h.collider.gameObject.layer)}");
    }
}
```

**Was es zeigt:**
- **ALLE** Collider im Raycast-Pfad (nicht nur der erste)
- Collider Name
- Tag (zur Verifikation)
- **IsTrigger:** Könnte Props sein Trigger? (`true` = Raycast ignoriert)
- **Layer:** Vielleicht Props auf falschem Layer?

**Logs erscheinen:**
- Jede ~1 Sekunde (alle 60 Frames)
- Nur wenn Collider im Pfad sind
- Console zeigt ALLE Props die zwischen Snake und Ziel sind

---

## 🧪 TESTING GUIDE

### Test 1: MoveAwayTarget Hierarchy Fix ✅

**Voraussetzung:** Targets aus Snake-Prefab entfernt + in Scene platziert

**Steps:**
1. Play Mode starten
2. **Vor Spell:** Notiere Snake Position (z.B. `(2, 0, 3)`)
3. **Vor Spell:** Notiere Target Position (z.B. `(10, 0, 10)`)
4. Cast Move Away Spell
5. **Während Spell:** Console prüfen:
   ```
   SnakeAI (Snake): Moving to Target 'MoveAwayTarget_Snake1' | Distance: 9.5 | Direction: (0.85, 0.00, 0.74)
   ```
6. **Prüfe:** Distance sollte **SINKEN** (z.B. 9.5 → 7.3 → 5.1 → 2.9)
7. Snake sollte Target **ERREICHEN** (Distance < 1.0)
8. Console: `"Reached MoveAwayTarget at distance 0.87, transitioning to root state"`

**Success Criteria:**
- ✅ Distance sinkt kontinuierlich
- ✅ Direction bleibt konsistent
- ✅ Snake erreicht Target (NICHT 2s Timeout!)
- ✅ Visual: Snake zeigt Gray/Transparent während Spell

---

### Test 2: Props Collision Diagnostic 🔍

**Steps:**
1. Play Mode starten
2. Stelle Snake VOR einen Prop (z.B. Pillar, Support Beam)
3. Cast Move Away Spell → Target HINTER dem Prop
4. **Console beobachten:** RaycastAll Logs jede Sekunde
5. **Erwartete Ausgabe:**
   ```
   SnakeAI (Snake): RaycastAll found 2 colliders in path:
     - WallPropsBottomA_ST (2) | Tag: Environment | IsTrigger: False | Layer: Default
     - MoveAwayTarget_Snake1 | Tag: MoveAwayTarget | IsTrigger: False | Layer: Default
   SnakeAI (Snake): Movement blocked by WallPropsBottomA_ST (2) (Tag: Environment) at distance 0.30
   ```

**Diagnostic-Analyse:**

**Fall A: Props erscheinen in RaycastAll ABER Movement nicht blockiert**
- → Code-Bug: `return false` wird nicht ausgeführt
- → Unwahrscheinlich (Code sieht korrekt aus)

**Fall B: Props erscheinen NICHT in RaycastAll**
- → **IsTrigger: True** → Raycast ignoriert Trigger!
- → **Layer Problem:** Props auf Layer der nicht von Raycast getroffen wird
- → **Collider disabled:** Props Collider ist ausgeschalten

**Fall C: Props erscheinen in RaycastAll MIT IsTrigger: True**
- → **DAS ist das Problem!**
- → Fix: Props Prefabs öffnen, BoxCollider → `Is Trigger = false`

**Fall D: Raycast trifft Props, Movement passiert trotzdem**
- → Snake bypassed `return false` irgendwie
- → Debugging needed (sollte NICHT passieren)

---

## 📊 ERWARTETE CONSOLE OUTPUT (Nach Fix)

### Erfolgreicher MoveAway Spell:

```
SnakeAI (Snake): Color changed to RGBA(0.50, 0.50, 0.50, 0.50) (State visual feedback)

SnakeAI (Snake): Moving to Target 'MoveAwayTarget_Snake1' | Distance: 8.34 | Direction: (0.87, 0.00, 0.49)
SnakeAI (Snake): RaycastAll found 1 colliders in path:
  - MoveAwayTarget_Snake1 | Tag: MoveAwayTarget | IsTrigger: False | Layer: Default

SnakeAI (Snake): Moving to Target 'MoveAwayTarget_Snake1' | Distance: 6.12 | Direction: (0.87, 0.00, 0.49)
SnakeAI (Snake): Moving to Target 'MoveAwayTarget_Snake1' | Distance: 3.89 | Direction: (0.87, 0.00, 0.49)
SnakeAI (Snake): Moving to Target 'MoveAwayTarget_Snake1' | Distance: 1.65 | Direction: (0.87, 0.00, 0.49)

SnakeAI (Snake): Reached MoveAwayTarget at distance 0.87, transitioning to root state
SnakeAI (Snake): MoveAway complete → Idle (Player visible: True, Distance: 4.2)
SnakeAI (Snake): Color changed to RGBA(0.00, 1.00, 0.00, 1.00) (State visual feedback)
```

### Props Blocking Movement (Erwünscht):

```
SnakeAI (Snake): Moving to Target 'MoveAwayTarget_Snake1' | Distance: 5.23 | Direction: (0.92, 0.00, 0.39)
SnakeAI (Snake): RaycastAll found 2 colliders in path:
  - PillarADEP_ST | Tag: Environment | IsTrigger: False | Layer: Default
  - MoveAwayTarget_Snake1 | Tag: MoveAwayTarget | IsTrigger: False | Layer: Default
SnakeAI (Snake): Movement blocked by PillarADEP_ST (Tag: Environment) at distance 0.42

[2 Sekunden später...]

SnakeAI (Snake): Blocked by obstacle for 2s, giving up on MoveAwayTarget
SnakeAI (Snake): MoveAway complete → Idle (Player visible: False, Distance: 3.1)
```

---

## ✅ ZUSAMMENFASSUNG - WAS MUSS DER USER TUN?

### Manuelle Unity-Arbeit (REQUIRED):

1. **MoveAwayTarget Hierarchy Fix:**
   - [ ] Targets aus Snake-Prefabs löschen
   - [ ] Neue Targets in Scene erstellen (Scene-Level, NICHT Child!)
   - [ ] Targets zu Snake Inspector-Fields zuweisen
   - [ ] Targets mit BoxCollider + Tag "MoveAwayTarget" ausstatten

2. **Props Collision Diagnostic:**
   - [ ] Play Mode starten
   - [ ] RaycastAll Logs in Console beobachten
   - [ ] Props erscheinen in Logs? → Check IsTrigger value
   - [ ] Props erscheinen NICHT? → Check Layer oder Collider enabled

3. **Test & Report Back:**
   - [ ] Test Case 1: MoveAwayTarget erreicht (Distance sinkt zu <1.0)
   - [ ] Test Case 2: Props blockieren Movement (Console zeigt blocked)
   - [ ] Sende Console-Logs wenn Props immer noch durchlaufen werden

---

## 🎯 NEXT STEPS

**Nach erfolgreicher Fix-Verification:**
1. Commit Changes: "fix: SnakeAI v1.3.12 - MoveAwayTarget hierarchy + Props diagnostic"
2. Update STATE.md mit Test-Ergebnissen
3. **Falls Props immer noch nicht blockieren:**
   - User sendet RaycastAll Console-Logs
   - Wir analysieren IsTrigger/Layer Issues
   - Weitere Code-Fixes wenn nötig

---

**Branch:** feature/enemy-setup
**Status:** Diagnostic Code Ready, Manual Unity Work Required
**User Action Needed:** Scene-Setup gemäß Anleitung durchführen
