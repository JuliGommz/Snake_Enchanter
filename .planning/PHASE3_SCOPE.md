# Phase 3 Scope - SCHÖN (Polish + Audio + Visual)

**Target Duration:** 3-4 days
**Goal:** "Das Spiel fühlt sich gut an und sieht anständig aus"
**Abgabe-Wert:** Solides Indie-Game ⭐⭐⭐⭐

---

## 🎯 Phase 3 Core Features (MVP_Phasen.md)

### Audio System
- [ ] **Flöten-Melodien** (5-12s per Tune)
  - Tune 1 (Move): Sanfte Flöte
  - Tune 2 (Daze): Langsame, beruhigende Melodie
  - Tune 3 (Attack): Aggressive, schnelle Töne
  - Tune 4 (Freeze): Eisige, hohe Töne
- [ ] **Snake SFX:**
  - Hiss (idle ambient)
  - Bite attack sound
  - Breath attack (poison spray)
  - Daze/collapse sound
- [ ] **UI Sounds:**
  - Slider movement
  - Success/Fail feedback
  - Button clicks (Menu)
- [ ] **Ambient Music:**
  - Cave atmosphere
  - Low HP tension music

### Visual Feedback
- [ ] **Particle Effects:**
  - Spell cast particles (replace Material Emission?)
  - Attack impact particles (Bite/Breath/Projectile)
  - HP restoration glow
  - Exit portal glow/shimmer
- [ ] **Screen Effects:**
  - Screen shake on Fail/Damage
  - Vignette on low HP (<30%)
  - Flash on spell success
- [ ] **Animation Polish:**
  - Smooth transitions
  - Hit reactions
  - Death polish

### UI Polish
- [ ] **Health Bar:**
  - Animated drain/fill
  - Color gradient (Green→Yellow→Red)
  - Pulse on low HP
- [ ] **Timing Meter:**
  - Visual polish (not just slider)
  - Zone highlight animation
  - Success/Fail VFX
- [ ] **Menus:**
  - Animated transitions
  - Button hover effects
  - Mode selection polish

### Level Polish
- [ ] **Lighting:**
  - Atmospheric cave lighting
  - Dynamic shadows
  - Exit glow/beacon
- [ ] **Props:**
  - Add detail meshes
  - Break up long corridors
  - Add visual interest
- [ ] **Exit Portal:**
  - Glowing effect
  - Particle system
  - "Win" feeling

---

## 🔴 Phase 2 Carryover Items (BACKLOG)

### Critical (Must-Fix)
- [ ] **Tune 4 (Freeze) Debugging**
  - Current: Code exists, unlocked, but non-functional
  - Symptom: No UI slider visible (fixed?), spell doesn't freeze snakes
  - Priority: HIGH - core feature not working

### Important (Phase 2 Incomplete)
- [ ] **3 Areas Implementation**
  - Current: 1 GameLevel scene
  - Need: Tutorial → Main → Finale progression
  - OR: Scope down to 1 polished area

- [ ] **Backend API Integration**
  - POST `/api/game-session` - Session stats
  - GET `/api/leaderboard` - Bestenliste
  - GET `/api/player-stats` - Aggregated stats

- [ ] **Main Menu Polish**
  - Current: Basic functional
  - Need: Mode selection, settings, quit

- [ ] **Result Screen Polish**
  - Current: Basic Win/Lose
  - Need: Stats display, retry button, leaderboard

---

## 📊 BACKLOG from Session 16 (Optional Enhancements)

### Spell System (Phase 3/4)
- [ ] Two-Level Success System (Player Timing + Enemy Enchantment)
- [ ] Player Spell Cooldown (Inspector-konfigurierbar)
- [ ] Player Success Rate (50-90% based on Health)
- [ ] Spell Range System (Inspector-definierbar)
- [ ] Dynamic Slider Balancing (Speed/Zone Variation)
- [ ] Particle Glow System (replace Material Color Change)

### Creature Combat (Phase 4?)
- [ ] Kampf-System: Snake vs Creature mit HP
- [ ] Creature kann Snake angreifen
- [ ] Snake überlebt/stirbt basierend auf HP-Interaktion
- [ ] Current: Both die (Phase 1 simplified) - works for now

---

## 🎮 Testing Checklist (Phase 3 Complete)

### Audio
- [ ] All 4 Tune melodies play on spell cast
- [ ] Snake SFX trigger correctly (attack, daze, etc.)
- [ ] UI sounds responsive
- [ ] Ambient music loops smoothly
- [ ] Volume levels balanced

### Visual
- [ ] Particles look good at 60 FPS
- [ ] Screen shake feels impactful not nauseating
- [ ] Vignette visible on low HP
- [ ] Health bar animations smooth

### Polish
- [ ] Game feels "juicy"
- [ ] No jarring transitions
- [ ] Exit portal looks inviting
- [ ] Cave atmosphere immersive

---

## 📁 Key Assets Needed

### Audio
- **Flute Melodies:** 4 tracks (5-12s each) - lizenzfrei
- **SFX Pack:** Snake sounds + UI sounds
- **Ambient:** Cave atmosphere loop

### Visual
- **Particle Textures:** Glow, smoke, sparkle
- **Shaders:** URP-compatible particle shaders
- **VFX Prefabs:** Exit portal, spell cast, attack impact

---

## ⚙️ Technical Considerations

### Performance
- Particle system pooling (if many effects)
- Audio source management (don't spam sources)
- Post-processing performance (URP Volume)

### URP Compatibility
- All effects must work with URP (no Built-in RP shaders)
- Post-processing via Volume component
- Particle shaders: URP/Particles/Lit or Unlit

### Scene Structure
- Organize particles in hierarchy (FX group)
- Audio sources on appropriate GameObjects
- Lighting baked where possible

---

## 🚀 Phase 3 → Phase 4 Handoff

**Done Criteria:**
- All audio implemented and playing
- Visual feedback feels good
- UI polished and animated
- Level has atmosphere
- Game feels like an indie game (not a prototype)

**Phase 4 Focus:**
- Bug fixing + stability
- Balancing (HP, Timing, Drain rates)
- Trailer production
- Final build + documentation
- Abgabe preparation

---

**Branch:** New branch from `main` → `feature/phase3-polish`
**Estimated Duration:** 3-4 days (2026-02-17 → 2026-02-21)
**Success Metric:** Game looks and feels polished, ready for final balancing
