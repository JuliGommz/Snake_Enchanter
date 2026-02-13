# Unity Tag Setup Guide - Environment Tag System

**Session 14 - SnakeAI v1.3.6**
**Date:** 2026-02-13

---

## 🎯 ZIEL

Snake Movement soll nur durch Environment-GameObjects (Wände, Böden) blockiert werden.
**Lösung:** Alle Environment-Objects mit Tag `"Environment"` versehen.

---

## ⚙️ SCHRITT 1: Tag erstellen (falls nicht vorhanden)

1. Unity öffnen → **Edit** → **Project Settings** → **Tags and Layers**
2. Unter **Tags** → **+** klicken
3. Neuen Tag erstellen: `Environment`
4. **Save** (schließen)

---

## 🏷️ SCHRITT 2: Cave Environment taggen

### Cave Walls (alle Wall-Objekte)
**Hierarchie suchen:** `Caves_X2_wall`, `Caves_E_wall`, `Caves_L_wall`, `Caves_L2_wall`, etc.

**Für jedes Wall-Objekt:**
1. GameObject im Hierarchy auswählen
2. Inspector → **Tag** Dropdown (oben)
3. Tag auf `Environment` setzen

**Shortcut:** Multi-Select (Shift/Ctrl + Click auf mehrere Walls) → Tag auf alle gleichzeitig anwenden

---

### Cave Floors (alle Floor-Objekte)
**Hierarchie suchen:** `Caves_X2_floor`, `Caves_E_floor`, `Caves_L_floor`, `Caves_L2_floor`, etc.

**Für jedes Floor-Objekt:**
1. GameObject im Hierarchy auswählen
2. Inspector → **Tag** = `Environment`

---

### Andere Obstacles/Props
**Falls vorhanden (z.B. Felsen, Säulen, Kisten):**
- Alle statischen Objekte die Snakes blockieren sollen → Tag `Environment`

---

## 🐍 SCHRITT 3: Snake Prefabs verifizieren

**WICHTIG:** Snakes brauchen Tag `"Snake"` (sollte bereits gesetzt sein)

**Prefabs zu prüfen:**
1. `Assets/_Project/Art-Visuals/3D_Assets/Snakes/Prefabs/Toon Snake - Green.prefab`
2. `Assets/_Project/Art-Visuals/3D_Assets/Snakes/Prefabs/Toon Snake - Magenta.prefab`
3. `Assets/_Project/Art-Visuals/3D_Assets/Snakes/Prefabs/Toon Snake - Purple.prefab`
4. `Assets/_Project/Art-Visuals/3D_Assets/Snakes/Prefabs/Toon Cobra - Green.prefab`
5. `Assets/_Project/Art-Visuals/3D_Assets/Snakes/Prefabs/Toon Cobra - Magenta.prefab`
6. `Assets/_Project/Art-Visuals/3D_Assets/Snakes/Prefabs/Toon Cobra - Purple.prefab`

**Für jedes Prefab:**
1. Prefab im Project-Window auswählen
2. Inspector → **Tag** = `Snake` (sollte bereits so sein)
3. Falls nicht: Tag auf `Snake` setzen und **Apply** klicken

**Alternative (schneller):**
Öffne jedes Prefab → Überprüfe Root GameObject → Tag sollte `Snake` sein

---

## 👤 SCHRITT 4: Player GameObject verifizieren

**GameObject:** `Player` (im GameLevel Scene)

1. Player im Hierarchy auswählen
2. Inspector → **Tag** sollte `Player` sein
3. Falls nicht: Tag auf `Player` setzen

---

## 🎮 SCHRITT 5: MoveAwayTargets (optional)

**Falls du separate Targets für Move Away Spell hast:**

**GameObject:** Leere GameObjects wo Snakes hin-charmed werden

**Tag:** `MoveAwayTarget` (optional, nicht zwingend)
- Snakes passieren durch diese Targets (kein Block)
- Keine spezielle Behandlung nötig

---

## ✅ VERIFICATION CHECKLIST

Nach dem Tagging folgendes prüfen:

### In Unity Hierarchy:
- [ ] Alle `Caves_*_wall` haben Tag `Environment`
- [ ] Alle `Caves_*_floor` haben Tag `Environment`
- [ ] Alle Snake instances haben Tag `Snake`
- [ ] Player GameObject hat Tag `Player`

### In Unity Console (beim Spielen):
- [ ] **Keine "Stuck" Messages** mehr
- [ ] **Logs zeigen:** `"Movement blocked by [Wall/Floor Name]"` wenn Snake an Wand trifft
- [ ] **Snakes stoppen** an Wänden (keine Durchgang)
- [ ] **Snakes können durch andere Snakes** (passthrough, kein Block)

---

## 🧪 TESTING

**Test 1: Wall Collision**
1. Play Mode starten
2. Warte bis Snake patroulliert oder Move Away castest
3. Snake sollte an Wand stoppen
4. Console: `"Movement blocked by Caves_X2_wall at distance X"`

**Test 2: Snake Passthrough**
1. Zwei Snakes nah zusammen platzieren
2. Move Away auf beide casten
3. Snakes sollten durch einander gleiten (kein Block)

**Test 3: Player Passthrough**
1. Player neben Snake stellen
2. Snake sollte NICHT durch Player blockiert werden (Follow sollte funktionieren)

---

## 📊 TAG ÜBERSICHT

| Tag | GameObject Typ | Zweck |
|-----|---------------|-------|
| `Environment` | Walls, Floors, Props | **Blockiert** Snake Movement |
| `Snake` | Toon Snake/Cobra Prefabs | Passthrough (Snakes ignorieren Snakes) |
| `Player` | Player GameObject | Passthrough (Snakes können Player folgen) |
| `Untagged` | Andere Objects | Passthrough (Default) |

---

## 🔧 TROUBLESHOOTING

**Problem:** Snakes gehen immer noch durch Wände
**Lösung:** Verifiziere dass Cave Walls wirklich Tag `Environment` haben (nicht `Untagged`)

**Problem:** Snakes blockieren sich gegenseitig
**Lösung:** Snake Prefabs müssen Tag `Snake` haben (nicht `Environment` oder `Untagged`)

**Problem:** Console zeigt "Movement blocked" aber Snake bewegt sich trotzdem
**Lösung:** Code-Bug, bitte melden (sollte nicht passieren nach v1.3.6)

---

## 📝 HINWEISE

- **Performance:** Tag-Checks sind sehr schnell (optimiert in Unity)
- **Erweiterbar:** Neue Passthrough-Tags einfach hinzufügen ohne Code-Änderung
- **Best Practice:** Tags für Gameplay-relevante Kategorien (nicht für visuelles Grouping)

---

**Setup abgeschlossen?** → Starte Play Mode und teste Movement!

**Bei Problemen:** Console Logs lesen, Tag-Assignments in Inspector überprüfen.
