# Phase 7: Spell System - Context

**Gathered:** 2026-02-18
**Status:** Ready for planning

<domain>
## Phase Boundary

Players earn spells by finding scrolls in the cave. Tunes are locked until their scroll is collected. The system uses **3 tunes** (not 4 — Tune 3 Attack and Tune 4 Freeze are replaced by a single new Tune 3 Shield).

**SCOPE CHANGE from original roadmap:**
- Tune 3 (Attack Creature) → REMOVED (requires second creature + fight system, too much work)
- Tune 4 (Freeze) → REMOVED (overlaps with Daze, unnecessary)
- NEW Tune 3: Shield — defensive spell, blocks next snake attack for 8s

**Final tune lineup:**
| Key | Tune | Effect |
|-----|------|--------|
| 1 | Move | Snake moves away to MoveAwayTarget |
| 2 | Daze | Snake collapses for 8 seconds |
| 3 | Shield | Player gains a shield that blocks next snake attack (8s duration) |

</domain>

<decisions>
## Implementation Decisions

### Scroll Pickup Behavior
- Collection method: **walk-over trigger collider OR mouse click** — both should work
- Scrolls glow brighter as player approaches (proximity-based intensity)
- On collection: **game pauses**, center-screen panel appears showing scroll name, 2-line description, and assigned key number
- Player presses any key to dismiss the panel and resume gameplay
- Scroll disappears from world after collection (no VFX needed — instant removal is fine)
- Key rebinding is **nice-to-have** (not v1.0 scope) — keys stay hardcoded as 1, 2, 3

### Scroll Placement & Progression
- **Fixed unlock order**: Tune 1 (Move) → Tune 2 (Daze) → Tune 3 (Shield)
- **1 scroll per QuestRoom**: clean 1:1 mapping
- Scrolls are **not inside the QuestRooms** but in the cave system, placed strategically before each room's snake confrontation
- 2 scrolls on the main path, 1 in a side exploration area (the last one too, probably)
- Player starts with **zero tunes** — first scroll pickup is the tutorial moment

### HUD / Locked Tune UI
- HUD starts **completely empty** — no tune slots visible at all
- Each scroll pickup **adds a new slot** to the HUD
- Each slot shows: **key icon with number** (sketch/simplified key shape) + **spell name** + **color**
- Keys are **always visible** on each slot — no memorization needed
- Pressing an unassigned key does **nothing** (silently ignored)
- When a scroll is collected, the new HUD slot appears with a clear visual transition (color fill, noticeable)

### Tune 3: Shield Behavior
- Duration: **8 seconds** (`[SerializeField]` for tuning)
- Blocks the **next incoming attack** (bite, breath, or projectile), then breaks
- If no attack comes within 8s, shield expires naturally
- Visual: **screen edge glow** (blue/gold) while shield is active — first-person friendly, no 3D model work
- Block feedback: **screen flash + shatter sound** when shield absorbs an attack
- **Cannot recast while active** — Tune 3 key is locked/non-responsive while shield is up
- Shield state tracked on player (HealthSystem or new ShieldComponent)

### Claude's Discretion
- Scroll 3D model/visual design (can be a simple glowing scroll mesh or particle placeholder)
- Exact proximity glow curve (linear, ease-in, etc.)
- Pause panel layout and styling details
- Shield screen-edge glow implementation approach (post-processing, UI overlay, etc.)
- HUD slot layout/positioning (horizontal bar, vertical stack, etc.)
- Exact shield color palette

</decisions>

<specifics>
## Specific Ideas

- Zelda-style item pickup: game pauses, item shown center screen with description, press any key to continue
- Key icon on HUD should look like a physical key shape with the number on it — not just a plain number
- Shield is NOT stackable — strategic timing matters
- Scroll proximity glow = player can spot scrolls from a distance but they become more noticeable as you approach

</specifics>

<deferred>
## Deferred Ideas

- **Key rebinding** (let player choose which key per tune) — nice-to-have, not Phase 7
- **Tune 4 slot** — removed entirely. If a 4th tune is ever wanted, it would be a new phase
- **Attack Creature tune** — requires second creature + fight system, deferred to Phase 12 (EXT-02 RobotKyle)

</deferred>

---

*Phase: 07-spell-system*
*Context gathered: 2026-02-18*
