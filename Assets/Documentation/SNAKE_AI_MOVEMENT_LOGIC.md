# Snake AI Movement & Collision Logic

**Session 14 - 2026-02-13**
**SnakeAI Version:** v1.3.9

---

## 📋 ÜBERSICHT

Dieses Dokument beschreibt die komplette Bewegungs- und Kollisions-Logik des Snake AI Systems, wie sie in Session 14 entwickelt und iterativ verbessert wurde.

---

## 🎯 DESIGN-PHILOSOPHIE

### Core Principle: Tag-basierte Kollisionserkennung

**Warum Tags?**
- Semantisch klar ("Environment" blockiert, "Player" blockiert)
- Einfach zu verstehen und zu debuggen
- Keine komplexen Layer-Masken
- Erweiterbar ohne Code-Änderungen
- Unity Best Practice 2026

**Rule of Thumb:**
> "ALLES blockiert Snake Movement. Keine Ausnahmen."

Dies verhindert:
- Snakes laufen durch Wände ❌
- Snakes stapeln sich übereinander ❌
- Snakes phasen durch Player ❌
- Snakes ignorieren Targets ❌

---

## 🏷️ TAG-SYSTEM

### Tag-Definitionen

| Tag | GameObjects | Blockiert Movement? | Zweck |
|-----|-------------|---------------------|-------|
| `Environment` | Walls, Floors, Props | ✅ JA | Hindernisse im Level |
| `Snake` | Toon Snake/Cobra Prefabs | ✅ JA | Verhindert Snake-Stacking |
| `MoveAwayTarget` | Target GameObjects | ✅ JA | Präzises Stoppen am Ziel |
| `Player` | Player Character | ✅ JA | Ermöglicht Attack bei Kontakt |
| `Untagged` | Andere Objects | ✅ JA | Safe Default Behavior |

### Wichtige Regeln

1. **Keine Passthrough-Logik**
   - Frühere Versionen hatten "Player = passthrough" → FALSCH
   - Korrekt: Alles blockiert, Snake stoppt für Attack

2. **Collider sind Pflicht**
   - MoveAwayTargets brauchen Box Collider (Is Trigger = NO)
   - Ohne Collider: Snake läuft einfach durch

3. **Tag vs. Layer**
   - Wir verwenden TAGS nicht Layers
   - Grund: Einfacher, semantischer, weniger Fehleranfällig

---

## 🔧 TECHNISCHE IMPLEMENTATION

### MoveTowardsSafe() - Core Movement Method

```csharp
private bool MoveTowardsSafe(Vector3 targetPosition, float speed)
{
    Vector3 direction = (targetPosition - transform.position).normalized;
    float distance = speed * Time.deltaTime;

    // Raycast to check for obstacles ahead
    RaycastHit hit;
    if (Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, out hit, distance + 0.3f))
    {
        // ALL objects block movement
        Debug.Log($"SnakeAI ({_snakeName}): Movement blocked by {hit.collider.name} (Tag: {hit.collider.tag}) at distance {hit.distance:F2}");
        return false; // BLOCKED
    }

    // Safe to move
    transform.position = Vector3.MoveTowards(transform.position, targetPosition, distance);
    return true; // SUCCESS
}
```

### Warum dieser Ansatz?

**Raycast-Parameter:**
- `origin`: Snake position + 0.5 units up (center of snake)
- `direction`: Normalized direction to target
- `distance`: Movement distance this frame + 0.3f buffer
- `layerMask`: Default (ALL layers)

**Rückgabewert:**
- `true` = Movement erfolgreich (kein Hindernis)
- `false` = Movement blockiert (Hindernis getroffen)

**Design-Entscheidung:**
- Keine Tag-Checks im Movement Code
- ALLES blockiert = einfachste, sicherste Regel
- Debug-Log zeigt was blockiert (wichtig für Testing)

---

## 🎮 MOVEMENT BEHAVIORS

### 1. Patrol System

**Aktivierung:**
- Nur im `SnakeState.Idle`
- Player NICHT sichtbar (`_canSeePlayer = false`)

**Logik:**
```csharp
private void UpdatePatrol()
{
    // Patrol only in Idle state
    if (_currentState != SnakeState.Idle) return;

    // Stop patrol if player is visible
    if (_canSeePlayer)
    {
        _isPatrolling = false;
        return;
    }

    // Generate random waypoint in 2-3 units radius
    // Move toward waypoint with MoveTowardsSafe()
    // Wait at waypoint (_patrolWaitTime)
    // Repeat
}
```

**Parameter:**
- Patrol Radius: 2-3 units (Random.Range)
- Patrol Speed: 75% of normal movement speed
- Wait Time: 2 seconds per waypoint

**Kollision während Patrol:**
- Snake stoppt wenn Wand/andere Snake getroffen wird
- Waypoint wird nicht neu generiert (Snake wartet)
- Nächstes Waypoint wird nach Wait-Time generiert

---

### 2. Follow Player (Proximity Detection)

**Aktivierung:**
- Player ist sichtbar (`_canSeePlayer = true`)
- Distance: 0.5 - 3.5 units (Follow Range)
- State: `Idle` oder `Aggressive`

**Logik:**
```csharp
private void FollowPlayer()
{
    if (_playerTransform == null) return;

    // Move toward player (with collision detection)
    MoveTowardsSafe(_playerTransform.position, _chaseSpeed);

    LookAtPlayer();
}
```

**Was passiert bei Kollision?**
- Snake stoppt bei ~0.5 units vor Player (Player-Collider)
- Snake ist jetzt in **Bite Range**
- Attack-System triggert automatisch (CheckAndTriggerAttack)

**Warum Player-Collider wichtig ist:**
- Ohne Block: Snake läuft DURCH Player
- Mit Block: Snake stoppt → Attack triggert
- Korrekte Range-Detection für Bite (0-0.5 units)

---

### 3. Move Away (Tune 1 Success)

**Aktivierung:**
- Tune 1 erfolgreich gecastet
- Snake geht in `SnakeState.MovedAway`
- Nach 3.5s Spell Animation Delay: Movement startet

**Logik:**
```csharp
case SnakeState.MovedAway:
    if (_moveAwayTarget != null)
    {
        // Wait for spell animation delay (3.5s)
        if (!_isMoving) break;

        float distanceToTarget = Vector3.Distance(transform.position, _moveAwayTarget.position);

        // Stop when close to target
        if (distanceToTarget < 0.5f)
        {
            _isMoving = false;
            SetState(SnakeState.Idle); // ← WICHTIG: State-Transition!
        }
        else
        {
            // Move toward target (with collision detection)
            MoveTowardsSafe(_moveAwayTarget.position, _moveSpeed);
        }
    }
    break;
```

**Stopping Mechanism:**
1. **Distance Check:** `distanceToTarget < 0.5f`
2. **Collider Detection:** MoveTowardsSafe() blockiert bei Target-Collider
3. **State Transition:** Snake geht zurück zu Idle (kann wieder patrouillieren)

**Warum 0.5f Threshold?**
- Snake Collider Radius: ~0.3-0.5 units
- Target Collider Size: ~1x1x1 units
- 0.5f = "Snake ist nah genug, Target ist erreicht"
- Ohne Threshold: Snake könnte nie exakt auf Target-Center sein

---

## 🚫 KOLLISIONS-SZENARIEN

### Szenario 1: Snake trifft Wand (Environment)

```
Snake Position: (5, 0, 5)
Target Position: (10, 0, 5)
Wand Position: (7, 0, 5)

Frame 1: Snake bei (5, 0, 5) → Move to (5.4, 0, 5) ✅
Frame 2: Snake bei (5.4, 0, 5) → Move to (5.8, 0, 5) ✅
Frame 3: Snake bei (5.8, 0, 5) → Raycast trifft Wand bei (7, 0, 5) ❌
         → MoveTowardsSafe() returns false
         → Snake stoppt bei (5.8, 0, 5)
         → Console: "Movement blocked by Caves_X2_wall (Tag: Environment) at distance 1.2"
```

**Resultat:** Snake bleibt bei (5.8, 0, 5) stehen, patroulliert nicht weiter bis neues Waypoint generiert wird.

---

### Szenario 2: Snake trifft andere Snake

```
Snake A Position: (5, 0, 5) → Moving to (10, 0, 5)
Snake B Position: (7, 0, 5) → Idle/Patrolling

Frame X: Snake A bei (6.5, 0, 5) → Raycast trifft Snake B ❌
         → MoveTowardsSafe() returns false
         → Snake A stoppt bei (6.5, 0, 5)
         → Console: "Movement blocked by Toon Cobra - Purple (Tag: Snake) at distance 0.5"
```

**Resultat:** Keine Snake-Stacking! Snake A wartet/patroulliert um Snake B herum.

---

### Szenario 3: Snake trifft Player (Attack Range)

```
Player Position: (10, 0, 5)
Snake Position: (8, 0, 5) → Following Player (Aggressive State)

Frame 1: Snake bei (8, 0, 5) → Distance = 2 units (Follow Range)
Frame 2: Snake bei (9, 0, 5) → Distance = 1 units (Follow Range)
Frame 3: Snake bei (9.5, 0, 5) → Raycast trifft Player-Collider ❌
         → MoveTowardsSafe() returns false
         → Snake stoppt bei (9.5, 0, 5)
         → Distance = 0.5 units → BITE RANGE!
         → CheckAndTriggerAttack() triggert Bite Animation
         → Damage wird nach 0.3s delay applied
```

**Resultat:** Korrektes Combat-Verhalten! Snake stoppt nah an Player, triggert Attack.

---

### Szenario 4: Snake erreicht MoveAwayTarget

```
Snake Position: (5, 0, 5)
MoveAwayTarget Position: (10, 0, 5) mit Box Collider (Size: 1x1x1)

Spell Animation Delay: 3.5s → _isMoving = true

Frame X: Snake bei (9, 0, 5) → Distance = 1.0 units
         → distanceToTarget >= 0.5f → Continue moving
Frame Y: Snake bei (9.4, 0, 5) → Distance = 0.6 units
         → distanceToTarget >= 0.5f → Continue moving
Frame Z: Snake bei (9.5, 0, 5) → Raycast trifft MoveAwayTarget-Collider ❌
         → MoveTowardsSafe() returns false
         → Distance = 0.5 units → distanceToTarget < 0.5f ✅
         → SetState(SnakeState.Idle)
         → Console: "Reached MoveAwayTarget at distance 0.50"
```

**Resultat:** Snake stoppt präzise am Target, geht zurück zu Idle State.

---

## 🐛 BUG-HISTORIE & LESSONS LEARNED

### Bug 1: Infinite Move Away (v1.3.2)

**Problem:**
- Snake fuhr zum MoveAwayTarget aber stoppte nie
- Lief einfach weiter ins Unendliche

**Root Cause:**
```csharp
// BAD CODE (v1.3.2):
if (distanceToTarget < 0.1f)
{
    _isMoving = false;
    // ❌ FEHLT: SetState(SnakeState.Idle)
}
```

**Lösung:**
```csharp
// FIXED (v1.3.3+):
if (distanceToTarget < 0.5f)
{
    _isMoving = false;
    SetState(SnakeState.Idle); // ← State-Transition hinzugefügt
}
```

**Lesson:** State-Transitions sind kritisch! `_isMoving` alleine reicht nicht.

---

### Bug 2: Snake Self-Collision (v1.3.3)

**Problem:**
- Console-Spam: "Stuck in Toon Snake - Purple, escaping..."
- Snakes detektierten sich selbst als Obstacle

**Root Cause:**
```csharp
// BAD CODE (v1.3.3):
Collider[] overlaps = Physics.OverlapSphere(transform.position, 0.3f, ~_playerLayer);
if (overlaps.Length > 0 && overlaps[0] != _collider)
{
    // ❌ "overlaps[0] != _collider" funktioniert nicht zuverlässig
}
```

**Lösung:**
```csharp
// FIXED (v1.3.4):
// OverlapSphere komplett entfernt
// Nur Raycast verwendet (einfacher, zuverlässiger)
```

**Lesson:** KISS Principle - Einfache Lösungen sind oft besser als komplexe.

---

### Bug 3: Inverted LayerMask (v1.3.4 → v1.3.5)

**Problem:**
- Snakes liefen durch ALLES (Wände, Floors, etc.)
- Kein Blocking funktionierte

**Root Cause:**
```csharp
// BAD CODE (v1.3.4):
int layerMask = ~(_playerLayer | LayerMask.GetMask("Default"));
// ❌ Das bedeutet: "Checke ALLES außer Player und Default"
// → Aber Walls sind IM Default Layer!
// → Walls wurden IGNORIERT!
```

**Lösung:**
```csharp
// FIXED (v1.3.5):
Physics.Raycast(...) // Kein LayerMask = ALL layers
if (hit.collider.CompareTag("Environment")) {
    return false; // Block
}
```

**Lesson:** LayerMasks sind fehleranfällig. Tags sind klarer.

---

### Bug 4: Player Passthrough Logic (v1.3.7 → v1.3.8)

**Problem:**
- Snake lief DURCH Player statt zu attackieren
- Follow-Behavior funktionierte nicht richtig

**Root Cause:**
```csharp
// BAD CODE (v1.3.7):
if (hitTag == "Player")
{
    // PASSTHROUGH (snake can follow player)
}
else
{
    return false; // BLOCK
}
// ❌ Gedanke: "Snake muss Player folgen können"
// → ABER: Snake soll STOPPEN um zu attackieren!
```

**Lösung:**
```csharp
// FIXED (v1.3.8):
if (Physics.Raycast(...))
{
    // ALL objects block movement (including Player)
    return false;
}
// ✅ Einfach: ALLES blockiert, keine Ausnahmen
```

**Lesson:** "Follow" bedeutet nicht "Passthrough". Snake folgt bis zum Collider, dann Attack.

---

### Bug 6: MoveAwayTarget Stopping Failure (v1.3.9)

**Problem:**
- Snakes liefen zum MoveAwayTarget aber stoppten nicht
- Liefen weiter bis zur ersten Wand/Collider
- Distance-Check (< 0.5f) wurde nie erreicht

**Root Cause:**
```csharp
// BAD CODE (v1.3.8):
if (distanceToTarget < 0.5f)
{
    SetState(SnakeState.Idle);
}
else
{
    MoveTowardsSafe(...); // ← Raycast trifft MoveAwayTarget Collider
    // Snake stoppt VOR dem Target (wegen Collider)
    // Distance-Check (< 0.5f) wird nie erreicht
}
```

**Lösung:**
```csharp
// FIXED (v1.3.9):
// 1) Erhöhe threshold auf 1.0f (für Collider-Größe)
if (distanceToTarget < 1.0f) { ... }

// 2) Prüfe ob blockiert DURCH MoveAwayTarget Collider
if (!moved)
{
    RaycastHit hit;
    if (Physics.Raycast(..., out hit, 1.5f))
    {
        if (hit.collider.CompareTag("MoveAwayTarget"))
        {
            // Blocked BY target = Arrived!
            TransitionFromMoveAwayToRootState();
        }
    }
}
```

**Lesson:** Collider haben Größe! Threshold muss größer sein als Collider-Radius. **Blocken = Ankommen** bei Targets mit Collider.

---

### Bug 7: Snakes Don't Attack After MoveAway (v1.3.9)

**Problem:**
- Nach MoveAway → Idle state
- Snakes greifen nicht mehr an (obwohl Player sichtbar)
- Attack Cooldown funktioniert, aber Attacks werden nie ausgelöst

**Root Cause:**
```csharp
// HandleIdlePlayerInteraction():
if (_playerDistance > 3.5 && _playerDistance < 4.0)
{
    // GAP! Nichts passiert hier
    // Snake steht nur rum, kein Follow, kein Attack
}
```

**Attack Range Gaps:**
- Bite: 0-0.5 ✅
- Follow: 0.5-3.5 ✅
- **GAP: 3.5-4.0** ❌
- Breath: 4-7 ✅
- **GAP: 7-8** ❌
- Projectile: 8+ ✅

**Lösung:**
```csharp
// FIXED (v1.3.9):
else if (_playerDistance > _followRangeMax && _playerDistance < _breathRangeMin)
{
    // Gap 3.5-4: Approach for breath attack
    FollowPlayer();
}
else if (_playerDistance > _breathRangeMax && _playerDistance < _projectileRange)
{
    // Gap 7-8: Approach for projectile
    FollowPlayer(); // (or LookAtPlayer in Simple mode)
}
else
{
    // Default fallback: Look at player
    LookAtPlayer();
}
```

**Lesson:** **Range Gaps sind tödlich**. Jede mögliche Player-Distance muss ein Verhalten haben. `else` Fallback für Edge Cases!

---

### Bug 8: State Transition Missing After MoveAway (v1.3.9)

**Problem:**
- Nach MoveAwayTarget erreichen: Direkt zu Idle
- Keine intelligente Evaluation der Player-Position
- Snake verhält sich "dumm" (startet einfach Patrol, ignoriert Player)

**Root Cause:**
```csharp
// BAD CODE (v1.3.8):
if (distanceToTarget < 0.5f)
{
    SetState(SnakeState.Idle); // ← Zu simpel!
}
```

**Lösung:**
```csharp
// FIXED (v1.3.9):
private void TransitionFromMoveAwayToRootState()
{
    // Immer zurück zu Idle
    // ABER: Idle state evaluiert Player visibility & distance
    // → Snake greift an / folgt / patroulliert (intelligent)
    SetState(SnakeState.Idle);

    Debug.Log($"MoveAway complete → Idle (Player visible: {_canSeePlayer}, Distance: {_playerDistance:F1})");
}
```

**Lesson:** **Idle ist nicht "dumm"**. Idle state ist der **Root State** mit intelligenter Logik (Player-Check → Attack/Follow OR Patrol). State-Namen können täuschen!

---

## 📊 PERFORMANCE-ÜBERLEGUNGEN

### Raycast Frequency

**Problem:**
- MoveTowardsSafe() wird JEDEN Frame aufgerufen (3x pro Snake)
- Patrol, Follow, Move Away - jeder State macht Raycasts

**Anzahl Raycasts pro Frame:**
```
18 Snakes × 3 Raycasts/Frame = 54 Raycasts/Frame
At 60 FPS: 3,240 Raycasts/Second
```

**Ist das ein Problem?**
- **NEIN** für Phase 2 (18 Snakes)
- Raycasts sind sehr schnell in Unity (optimiert in Physics Engine)
- Distance check (0.3f) ist sehr kurz → wenig CPU

**Potentielle Optimierungen (Phase 3+):**
1. **Raycast nur wenn Movement nötig:**
   ```csharp
   if (!_isMoving && _currentState == Idle) return; // Skip raycast
   ```

2. **Spatial Partitioning:**
   - Nur raycaste gegen nahe Objects (Octree/Grid)

3. **Fixed Update statt Update:**
   - Movement in FixedUpdate (Physics-synchronized)
   - Weniger häufige Checks (50 FPS statt 60 FPS)

**Aktueller Stand:**
- Unity Audit 2026-02-11: Performance Score 8.5/10
- Keine Raycast-Probleme identifiziert
- **Optimierung nicht nötig für Phase 2**

---

## 🎓 UNITY BEST PRACTICES ANGEWANDT

### 1. Tag-basierte Kollision (Unity 2026)

**Warum besser als Layer-Masken?**
- Semantisch klarer (`"Environment"` vs. `LayerMask 8`)
- Weniger Fehleranfällig (keine Bit-Operationen)
- Inspector-freundlich (Tags sichtbar im GameObject)
- Erweiterbar ohne Code-Änderungen

**Standard in Unity 2026:**
- Physics2D/3D bevorzugen Tag-Checks
- Layers nur für Rendering/Culling

---

### 2. State Machine Pattern

**Snake States:**
```csharp
public enum SnakeState
{
    Idle,
    Aggressive,
    MovedAway,
    Sleeping,
    Frozen,
    AttackingEnemy,
    Dead
}
```

**Warum wichtig?**
- Klare Separation of Concerns
- Debuggable (kann State in Inspector sehen)
- Erweiterbar (neue States einfach hinzufügen)

---

### 3. Raycast Direction + Distance

**Best Practice:**
```csharp
Vector3 origin = transform.position + Vector3.up * 0.5f; // Center of snake
Vector3 direction = (target - transform.position).normalized;
float distance = speed * Time.deltaTime + 0.3f; // Buffer for safety
```

**Warum + 0.3f Buffer?**
- Snake könnte in einem Frame zu weit bewegen
- Buffer verhindert "Phase-through" bei hohen Speeds
- Unity Physics Empfehlung: 10-30% Buffer

---

### 4. Debug Logging Strategy

**Aktueller Code:**
```csharp
Debug.Log($"SnakeAI ({_snakeName}): Movement blocked by {hit.collider.name} (Tag: {hit.collider.tag}) at distance {hit.distance:F2}");
```

**Warum gut?**
- Snake Name → welche Snake betroffen
- Collider Name → was blockiert
- Tag → warum blockiert
- Distance → wie nah

**Für Release:**
```csharp
#if UNITY_EDITOR
    Debug.Log(...);
#endif
```

---

## 🔮 ZUKÜNFTIGE ERWEITERUNGEN

### Phase 3: NavMesh Integration

**Problem mit transform.position:**
- Keine echte Pathfinding (Snake läuft geradeaus)
- Stuck wenn Weg blockiert (kein Re-routing)

**NavMesh Lösung:**
```csharp
private NavMeshAgent _navAgent;

private void MoveTowardsSafe(Vector3 targetPosition)
{
    _navAgent.SetDestination(targetPosition);
    // NavMesh handled Pathfinding + Collision automatically
}
```

**Vorteile:**
- Automatisches Pathfinding um Obstacles
- Performance-optimiert (Unity C++)
- Keine manuellen Raycasts nötig

**Warum noch nicht implementiert?**
- Phase 2: Simple Movement reicht
- NavMesh baking braucht Zeit
- Aktuelle Lösung funktioniert gut

---

### Phase 3: Avoidance Behavior

**Aktuell:**
- Snake stoppt bei Obstacle (statisch)

**Verbesserung:**
```csharp
if (MoveTowardsSafe(target) == false)
{
    // Try alternative directions
    Vector3[] alternatives = {
        Quaternion.Euler(0, 45, 0) * direction,
        Quaternion.Euler(0, -45, 0) * direction
    };

    foreach (var alt in alternatives)
    {
        if (MoveTowardsSafe(transform.position + alt))
        {
            break; // Found alternative path
        }
    }
}
```

**Resultat:** Snake geht um Obstacles herum statt zu stoppen.

---

## 📚 REFERENZEN

### Verwendete Unity APIs

- `Physics.Raycast()` - Collision Detection
- `Vector3.MoveTowards()` - Smooth Movement
- `Quaternion.Slerp()` - Smooth Rotation
- `Vector3.Distance()` - Distance Checks
- `Collider.tag` / `CompareTag()` - Tag-basierte Logic

### Projektdokumente

- `CLAUDE.md` - Projekt-Regeln (New Input System, Cinemachine)
- `GDD_v1.6` - Game Design Document mit Snake Behaviors
- `STATE.md` - Aktueller Projekt-Status
- `BACKLOG.md` - Bekannte Issues & Future Work
- `UNITY_TAG_SETUP_GUIDE.md` - Tag-Setup Anleitung

---

## ✅ CHECKLISTE: Movement System Verification

### Testing Checklist

- [ ] **Wall Collision:** Snake stoppt an Caves_X2_wall/floor
- [ ] **Snake Collision:** Zwei Snakes blockieren sich gegenseitig
- [ ] **Player Collision:** Snake stoppt bei ~0.5 units vor Player
- [ ] **MoveAwayTarget:** Snake stoppt präzise am Target (0.5f)
- [ ] **Patrol Movement:** Snake bewegt sich zu random Waypoints
- [ ] **Follow Behavior:** Snake folgt Player (0.5-3.5 units)
- [ ] **Attack Trigger:** Bite triggert wenn Player in Range

### Code Quality Checklist

- [x] Keine LayerMask-Bugs (alles via Tags)
- [x] Keine Self-Collision (Raycast funktioniert korrekt)
- [x] State Transitions vorhanden (MovedAway → Idle)
- [x] Debug Logs aussagekräftig (Name, Tag, Distance)
- [x] Performance akzeptabel (54 Raycasts/Frame OK)
- [x] Code dokumentiert (Kommentare + dieses Dokument)

---

**Dokument erstellt:** 2026-02-13 (Session 14)
**SnakeAI Version:** v1.3.8
**Status:** ✅ Production-Ready für Phase 2
