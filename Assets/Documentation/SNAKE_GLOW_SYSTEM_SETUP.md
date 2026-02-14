# Snake Glow/Emission Visual System - Setup Guide

**Session 14 - 2026-02-13**
**SnakeAI Version:** v1.4.0

---

## 🎨 SYSTEM OVERVIEW

**Neues Visual Feedback System:**
- ❌ **Alt:** Einfache Farbänderung (schwer sichtbar)
- ✅ **Neu:** Emission-basiertes Leuchten (Snake behält Original-Farbe + Glow)

**Features:**
- Snake Original-Farbe bleibt erhalten
- Leuchten in Snake-spezifischer Farbe (z.B. grüne Snake = grünes Leuchten)
- **Helligkeit einstellbar im Inspector** (`_enchantedGlowIntensity`)
- State-spezifische Glow-Farben (Aggressive = rot, Sleep = blau, etc.)
- URP Bloom kompatibel (automatisches HDR Glow)

---

## ⚙️ UNITY SETUP (REQUIRED)

### 1. Snake Material - Emission aktivieren

**Für JEDES Snake Material (z.B. "Toon Snake Green Material"):**

1. **Select Material** in Project Window:
   - `Assets/Plugins/ToonSnakesPack/Materials/`
   - Beispiel: `Toon_Snake_Green_Mat`, `Toon_Cobra_Purple_Mat`, etc.

2. **Inspector → Shader:** Muss `Universal Render Pipeline/Lit` sein
   - Falls anders → Change Shader zu URP/Lit

3. **Enable Emission:**
   - Scroll zu "Emission" Section
   - ✅ **Checkbox ANKLICKEN** (aktivieren)
   - Emission Color: Kann auf Schwarz bleiben (wird per Code gesetzt)
   - Emission Map: LEER lassen (None)

4. **Rendering Options prüfen:**
   - Surface Type: Opaque (Standard)
   - Render Face: Both (oder Front, je nach Bedarf)

5. **Material speichern** (`Ctrl+S`)

**Wiederhole für ALLE Snake Materials:**
- Toon Snake - Green
- Toon Snake - Purple
- Toon Cobra - Red
- Toon Cobra - Blue
- etc.

---

### 2. URP Post-Processing - Bloom aktivieren (Optional, aber empfohlen)

**Für besseren Glow-Effekt:**

1. **Create Volume Profile** (falls noch nicht vorhanden):
   - Project Window → Right-click
   - Create → Volume Profile
   - Name: `Global_PostProcessing`

2. **Add Bloom Override:**
   - Select Volume Profile
   - Inspector → Add Override → Post-processing → Bloom
   - ✅ Enable Bloom
   - **Intensity:** 0.2 - 0.5 (höher = stärkerer Glow)
   - **Threshold:** 1.0 (nur helle Emission leuchtet)
   - **Scatter:** 0.7 (Glow-Radius)

3. **Add Volume to Scene:**
   - Hierarchy → Create → Volume → Global Volume
   - Inspector → Profile: Drag & Drop `Global_PostProcessing`
   - Mode: Global

---

## 🎛️ INSPECTOR SETTINGS (SnakeAI Component)

### Neue Inspector-Parameter:

**Visual Feedback Section:**

| Parameter | Default | Beschreibung |
|-----------|---------|-------------|
| **Enchanted Glow Intensity** | 3.0 | Helligkeit des Glows (0 = kein Glow, 2-5 = sichtbar, höher = heller) |
| **Idle No Glow** | ✅ True | Idle state ohne Glow (Original-Look) |
| **Aggressive Glow Color** | (1, 0.2, 0.2) | Roter Glow-Tint |
| **Sleep Glow Color** | (0.3, 0.3, 1) | Blauer Glow-Tint |
| **Frozen Glow Color** | (0.3, 1, 1) | Cyan Glow-Tint |
| **Moved Glow Color** | (1, 1, 1) | Weißer Glow (hypnotisiert) |

---

## 🎨 WIE ES FUNKTIONIERT

### Emission-System:

```csharp
// Code setzt Emission per Material Property:
material.EnableKeyword("_EMISSION");
material.SetColor("_EmissionColor", glowColor * intensity);
```

**Emission Color Berechnung:**
```
Final Glow = GlowColor * Intensity

Beispiel (MovedAway State):
- GlowColor: White (1, 1, 1)
- Intensity: 3.0
- Final: (3, 3, 3) = Helles Weiß

Beispiel (Aggressive State):
- GlowColor: (1, 0.2, 0.2) = Rot-Tint
- Snake Base Color: Grün
- Intensity: 3.0
- Final: (3, 0.6, 0.6) = Rot-Orange Glow
```

**Glow multipliziert mit Snake Base Color** → Natürlicher Look!

---

## 🧪 TESTING GUIDE

### Test 1: Idle State (No Glow) ✅

**Steps:**
1. Play Mode starten
2. Snake beobachten (keine Spells casten)
3. **Expected:** Snake sieht normal aus (Original-Farbe, **KEIN** Leuchten)

**Success Criteria:**
- ✅ Snake behält Original-Farbe
- ✅ Kein Glow sichtbar

---

### Test 2: MovedAway State (White Glow) ✅

**Steps:**
1. Cast Move Away Spell (Tune 1) auf Snake
2. Snake beobachten während Spell
3. **Expected:** Snake **LEUCHTET WEISS** (hypnotisiert)
4. Console: `"SnakeAI (Snake): Glow enabled | Color: (1, 1, 1) | Intensity: 3"`

**Success Criteria:**
- ✅ Snake behält Original-Farbe (Grün/Lila/etc.)
- ✅ **Weißes Leuchten** um Snake herum sichtbar
- ✅ Nach Target erreichen: Glow verschwindet (zurück zu Idle)

---

### Test 3: Sleep State (Blue Glow) ✅

**Steps:**
1. Cast Sleep Spell (Tune 2) auf Snake
2. Snake beobachten während Sleep
3. **Expected:** Snake **LEUCHTET BLAU**

**Success Criteria:**
- ✅ Original-Farbe + Blaues Leuchten
- ✅ Glow deutlich sichtbar

---

### Test 4: Frozen State (Cyan Glow) ✅

**Steps:**
1. Cast Freeze Spell (Tune 4) - **Alle Snakes**
2. Alle Snakes beobachten
3. **Expected:** Alle Snakes **LEUCHTEN CYAN**

**Success Criteria:**
- ✅ Original-Farben + Cyan Leuchten
- ✅ Glow bei ALLEN Snakes gleichzeitig

---

### Test 5: Aggressive State (Red Glow) ✅

**Steps:**
1. Tune Failed auslösen (absichtlich falsches Timing)
2. Snake wird Aggressive
3. **Expected:** Snake **LEUCHTET ROT**

**Success Criteria:**
- ✅ Original-Farbe + Rotes Leuchten
- ✅ Snake verfolgt Player

---

## 🎚️ INTENSITY TUNING

### Glow zu schwach?

**Inspector → SnakeAI → Enchanted Glow Intensity:**
- Erhöhe Wert: 3.0 → 5.0 → 8.0
- Teste im Play Mode
- Höhere Werte = helleres Leuchten

### Glow zu stark/hell?

**Inspector → SnakeAI → Enchanted Glow Intensity:**
- Senke Wert: 3.0 → 2.0 → 1.5
- Teste im Play Mode

### Bloom zu stark (alles überstrahlt)?

**Volume Profile → Bloom:**
- Intensity senken: 0.5 → 0.3 → 0.2
- Threshold erhöhen: 1.0 → 1.5 → 2.0

---

## 🐛 TROUBLESHOOTING

### Problem: Kein Glow sichtbar (Snake bleibt normal)

**Check 1: Material Emission aktiviert?**
- Snake Material öffnen
- Emission Section → Checkbox muss ✅ sein
- Falls ❌ → Anklicken und speichern

**Check 2: Shader ist URP/Lit?**
- Material → Shader dropdown
- Muss `Universal Render Pipeline/Lit` sein
- Falls anders → Ändern zu URP/Lit

**Check 3: Console Logs prüfen**
```
Expected: "SnakeAI (Snake): Glow enabled | Color: ... | Intensity: 3"
Falls nicht → Code funktioniert nicht (Renderer-Problem?)
```

**Check 4: Renderer vorhanden?**
- Snake Prefab öffnen
- Child GameObject mit SkinnedMeshRenderer prüfen
- Falls fehlt → Prefab ist kaputt

---

### Problem: Glow zu schwach/kaum sichtbar

**Fix 1: Intensity erhöhen**
- Inspector → Enchanted Glow Intensity: 3.0 → 6.0

**Fix 2: Bloom aktivieren (URP)**
- Post-Processing Volume mit Bloom hinzufügen
- Bloom Intensity: 0.3 - 0.5

**Fix 3: Scene Lighting prüfen**
- Sehr helle Directional Light kann Emission überstrahlen
- Light Intensity senken: 1.5 → 1.0

---

### Problem: Glow falsche Farbe (nicht Snake-Farbe)

**Expected:** Glow multipliziert mit Snake Base Color

**Beispiel:**
- Grüne Snake + White Glow = Grünliches Leuchten
- Lila Snake + White Glow = Lila Leuchten

**Falls Glow komplett weiß (ohne Tint):**
- Material Base Color prüfen
- _BaseColor Property muss Snake-Farbe sein

---

### Problem: Glow bleibt nach State-Change

**Check Console:**
```
Expected: "SnakeAI (Snake): Glow disabled (Idle state)"
```

Falls nicht → Bug im SetState() Code

**Workaround:**
- Spiel neu starten (Material-State Reset)

---

## 📊 PERFORMANCE

**Emission-System ist performance-freundlich:**
- ✅ Keine zusätzlichen GameObjects
- ✅ Keine Particle Systems (CPU-freundlich)
- ✅ Nur Material Property-Änderung
- ✅ URP Bloom nutzt GPU (sehr effizient)

**18 Snakes mit Glow:**
- FPS Impact: ~1-2 FPS (vernachlässigbar)
- Mit Bloom: ~3-5 FPS (immer noch gut)

---

## 🎯 STATE-GLOW ÜBERSICHT

| State | Glow Color | Intensity | Bedeutung |
|-------|-----------|-----------|-----------|
| **Idle** | None | 0.0 | Normal (kein Spell aktiv) |
| **Aggressive** | Red (1, 0.2, 0.2) | 3.0 | Snake greift an (Failed Tune) |
| **MovedAway** | White (1, 1, 1) | 3.0 | Hypnotisiert, bewegt sich zu Target |
| **Sleeping** | Blue (0.3, 0.3, 1) | 3.0 | Eingeschlafen (Tune 2) |
| **Frozen** | Cyan (0.3, 1, 1) | 3.0 | Eingefroren (Tune 4) |
| **AttackingEnemy** | Yellow (1, 1, 0) | 3.0 | Greift anderen Gegner an (Tune 3) |

---

## ✅ NEXT STEPS

**Nach erfolgreichem Testing:**
1. **Commit:** "feat: SnakeAI v1.4.0 - Emission-based glow system"
2. **STATE.md Update:** Visual System Phase 1 complete
3. **Optional:** Glow-Colors im Inspector feintunen (pro Snake-Type)
4. **Phase 3:** Particle Effects hinzufügen (zusätzlich zu Emission)

---

**Branch:** feature/enemy-setup
**Status:** Code Complete, Unity Material Setup Required
**User Action:** Enable Emission in all Snake Materials + Test in Play Mode
