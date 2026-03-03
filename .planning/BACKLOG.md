# BACKLOG - Snake Enchanter

**Letzte Aktualisierung:** 2026-02-13 (Session 14)

---

## 🔴 HIGH PRIORITY

### Player Spell Animation Timing
- **Problem:** Spell Animations werden NACH dem Loslassen der Taste getriggert (zu spät)
- **Erwartet:** Animation startet WÄHREND die Taste gedrückt wird
- **Aktuell:** Animation triggert bei Key Release statt Key Press
- **Betroffen:** Alle 4 Spell Animations (Tune 1-4)
- **Benötigt:** TuneController Event-Timing anpassen
  - Animation sollte bei Slider-Start triggern (Key Down)
  - Nicht bei Slider-Ende (Key Release)
- **Phase:** 2 (Komplett) - Animation Polish
- **Aufwand:** 20-30min (TuneController.cs Zeile ~150 anpassen)

---

## 🔴 HIGH PRIORITY (Continued)

### Exit Trigger Animation Hang
- **Problem:** Animation bleibt beim Erreichen des Exit Triggers hängen
- **Aktuell:** Console logs funktionieren (ExitTrigger triggert)
- **Benötigt:** Durchdachte Game State Logic
  - Fade to black
  - Disable Player Input
  - Transition zu Win Screen/Next Level
- **Phase:** 2 (Komplett)
- **Aufwand:** 1-2h (GameManager State Machine erweitern)

---

## 🟡 MEDIUM PRIORITY

### Crouch Animation Transitions
- **Problem:** Ducken/Aufstehen ist an manchen Stellen ruckelig
- **Aktuell:** Technisch funktioniert alles
- **Benötigt:** Tuning für Player Experience
  - Transition Duration anpassen
  - Blend-Kurven optimieren
  - Eventuell Crouch-Transition Animation hinzufügen
- **Phase:** 3 (Schön - Polish)
- **Aufwand:** 30-45min (Animator Transitions optimieren)

### Camera Position bei Crouch
- **Problem:** Kamera bleibt auf gleicher Höhe wenn MC sich duckt
- **Aktuell:** CameraTarget ist unter Head Bone (folgt nicht Crouch)
- **Benötigt:** Camera Offset bei Crouch State anpassen
  - Option A: CameraTarget per Script nach unten bewegen bei IsCrouching
  - Option B: Cinemachine Offset dynamisch anpassen
  - Option C: Separate CameraTarget States im Animator
- **Phase:** 3 (Schön - Polish)
- **Aufwand:** 20-30min (Cinemachine Body Offset tweaking)

### Cave Textures Neon-Yellow
- **Problem:** Manche Cave-Texturen werden neon-gelb dargestellt
- **Vermutung:** Material/Shader Issue (Built-in statt URP?)
- **Benötigt:** Cave Materials auf URP konvertieren
  - Prüfen welche Materials betroffen sind
  - URP Conversion durchführen
  - Texture Re-Import falls nötig
- **Phase:** 3 (Schön - Polish)
- **Aufwand:** 30-60min (Material Debugging + Fix)

---

## 🟢 LOW PRIORITY / NICE TO HAVE

### Injured Walk Animation
- **Problem:** Injured Walk bringt sehr viele Bewegung mit sich, optisch verwirrend
- **Idee:** Drei Walk Animation States health-basiert
  - 100-66% HP: Normal Walk
  - 65-33% HP: Slightly Injured Walk (neue Animation finden)
  - 32-1% HP: Heavy Injured Walk (oder aktuelle entfernen)
- **Phase:** 3-4 (Polish/Fertig)
- **Aufwand:** 1-2h (Animation suchen, Animator erweitern, HealthSystem Integration)
- **Status:** Idee, muss noch entschieden werden ob umgesetzt

### Death_by_Snakes Animation Testing
- **Problem:** Death_by_Snakes Animation noch nicht getestet
- **Grund:** Snakes machen aktuell noch keinen Damage
- **Benötigt:** Snake Attack Integration
  - SnakeAI → Player Collision/Attack
  - HealthSystem.TakeSnakeAttack() triggern
  - Dann Death Animation testen
- **Phase:** 2 (Komplett)
- **Aufwand:** Teil von Enemy System Integration
- **Status:** Warten auf Snake Damage Implementation

### Snake MoveAwayTarget Stacking
- **Problem:** Beide Snakes laufen zum gleichen Punkt (stacken sich)
- **Benötigt:** Jede Snake braucht individuelles MoveAwayTarget
- **Phase:** 2-3 (Komplett/Schön)
- **Aufwand:** 30min (MoveAwayTarget pro Snake generieren)
- **Status:** Niedrige Priorität, kann für Phase 1 deaktiviert werden

---

## ✅ COMPLETED (aus Backlog entfernt)

### Pirate Character Setup
- ~~FBX Humanoid Rig konfigurieren~~
- ~~Materials auf SkinnedMeshRenderer zuweisen~~
- ~~Animations auf PirateAvatar retargeten~~
- ~~Animator Setup (Idle, Walk, Crouch States)~~
- ~~CameraTarget für Cinemachine~~
- **Completed:** 2026-02-09 (Session 8)

### Crouch Idle State
- ~~Crouch Idle Animation springt direkt zu Crouch Walk~~
- ~~Neuer State "Crouch Idle" hinzufügen~~
- ~~Transitions mit Speed Condition~~
- **Completed:** 2026-02-09 (Session 8)

### MC Spell + Death Animations
- ~~4 Spell Animations in Animator integrieren~~
- ~~TuneController Integration (Trigger bei Success)~~
- ~~2 Death Animations (Death_by_Drain, Death_by_Snakes)~~
- ~~HealthSystem Integration (Play bei HP=0)~~
- ~~Spell Animations getestet und funktionieren~~
- ~~Death_by_Drain getestet und funktioniert~~
- **Completed:** 2026-02-09 (Session 9)

---

## PRIORISIERUNG FÜR PHASE 2

**Empfohlene Reihenfolge:**
1. **Exit Trigger Animation Hang** (High Priority, blockiert Win Condition UX)
2. **Spell Animation Integration** (Medium, erhöht Juice signifikant)
3. **Cave Textures Fix** (Medium, visuell störend)
4. **Crouch Transition Polish** (Low, funktioniert bereits)
5. **Injured Walk / Snake Stacking** (Nice to Have, nicht kritisch)

---

**END OF BACKLOG**
