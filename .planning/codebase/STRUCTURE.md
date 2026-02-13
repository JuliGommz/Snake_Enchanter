# Codebase Structure

**Analysis Date:** 2026-02-13

## Directory Layout

```
Assets/
├── _Project/                           # Project root (all custom work)
│   ├── Scripts/                        # All C# game logic
│   │   ├── Core/                       # Game state and event system
│   │   │   ├── GameEvents.cs           # Static event hub
│   │   │   └── GameManager.cs          # Game state machine, mode switching
│   │   ├── Player/                     # Player systems
│   │   │   ├── PlayerController.cs     # Movement, camera, input (New Input System)
│   │   │   └── HealthSystem.cs         # HP management, drain, healing, death
│   │   ├── Snakes/                     # Snake AI
│   │   │   └── SnakeAI.cs              # State machine, tune interaction, attacks
│   │   ├── TuneSystem/                 # Spell/tune mechanics (ADR-008)
│   │   │   ├── TuneController.cs       # Hold-release slider, timing evaluation
│   │   │   └── TuneConfig.cs           # ScriptableObject tune parameters
│   │   ├── Level/                      # Level mechanics
│   │   │   └── ExitTrigger.cs          # Win condition detector
│   │   ├── UI/                         # User interface
│   │   │   ├── HealthBarUI.cs          # Gradient health bar with pulse
│   │   │   └── TuneSliderUI.cs         # Segmented tune slider with marker
│   │   ├── Data/                       # API/persistence (Phase 2)
│   │   └── Editor/                     # Editor tools
│   │       ├── CanvasUICreator.cs      # Auto-create Canvas UI structure
│   │       └── TuneConfigCreator.cs    # Create TuneConfig ScriptableObjects
│   ├── Animations/                     # Character and snake animations
│   │   ├── Pirate/                     # Player character (FBX + animations)
│   │   │   ├── Animations/             # Organized by type (Idle, Walk, Spell, etc.)
│   │   │   ├── Materials/              # Pirate textures/materials
│   │   │   ├── Mesh/                   # FBX model
│   │   │   └── Textures/               # Pirate body/cloth/hair textures
│   │   └── MC_Mixamo/                  # Alternative character (Mixamo)
│   ├── Art-Visuals/                    # 3D assets
│   │   ├── 3D_Assets/                  # Organized by source
│   │   │   ├── Cave/                   # Environment (FBX, materials, prefabs)
│   │   │   └── Snakes/                 # Snake models, controllers, prefabs
│   │   └── UI/                         # UI sprites, icons, panels
│   ├── Prefabs/                        # Reusable game object templates (Phase 1: empty)
│   ├── ScriptableObjects/              # Data-driven config assets
│   │   ├── ModeSettings/               # Game mode configurations
│   │   └── TuneConfigs/                # Tune timing and balance (Tune1.asset - Tune4.asset)
│   ├── Scenes/                         # Game scenes
│   │   ├── MainMenu.unity              # Menu scene (Phase 2)
│   │   └── GameLevel.unity             # Main play level
│   ├── Media/                          # Audio assets
│   │   ├── Audio/                      # Audio files
│   │   │   ├── Music/                  # Background music
│   │   │   ├── SFX/                    # Sound effects
│   │   │   └── Tunes/                  # Tune melodies (5-12s flute clips)
│   │   │       └── TestTunes/          # Temp test audio
│   │   └── [Sprites, Particles later]
│   └── Data/                           # Input actions and config data
│       └── SnakeEnchanter.inputactions # New Input System asset
├── Documentation/                      # Project documentation
│   ├── Projektplan_SnakeEnchanter.md   # Timeline and phases
│   ├── Arbeitsprotokoll_Julian_Gomez.md # Daily log
│   ├── GDD/
│   │   └── GDD_v1.4_SnakeEnchanter.txt # Game Design Document
│   └── MVP_Phasen.md                   # Phase breakdown
└── External_Assets/                    # Third-party packs
    ├── Gentleland/SteampunkUI/        # UI theme (Steampunk pack)
    ├── WeaponsAndPropsAssetPack_NAS/   # Environment props
    └── [Snakes plugin path]            # Toon Snakes Pack
```

## Directory Purposes

**Assets/_Project/Scripts/Core:**
- Purpose: Foundation layer - game state and event system
- Contains: GameManager singleton, GameEvents hub
- Key files: `GameManager.cs` (state machine), `GameEvents.cs` (event publish/subscribe)

**Assets/_Project/Scripts/Player:**
- Purpose: Player-controlled character logic
- Contains: Movement (CharacterController + New Input System), camera (Cinemachine integration), health (drain/damage/healing)
- Key files: `PlayerController.cs`, `HealthSystem.cs`

**Assets/_Project/Scripts/Snakes:**
- Purpose: Enemy AI behavior and tune interactions
- Contains: State machine (7 states), attack system, tune reaction logic
- Key files: `SnakeAI.cs` (contains SnakeState enum)

**Assets/_Project/Scripts/TuneSystem:**
- Purpose: Genshin-style hold-and-release spell mechanic
- Contains: Slider timing logic, position evaluation, tune configuration
- Key files: `TuneController.cs`, `TuneConfig.cs`

**Assets/_Project/Scripts/Level:**
- Purpose: Level mechanics and win conditions
- Contains: Trigger-based exit detection
- Key files: `ExitTrigger.cs`

**Assets/_Project/Scripts/UI:**
- Purpose: Visual feedback and HUD elements
- Contains: Health bar display (gradient + pulse), tune slider (segments + marker)
- Key files: `HealthBarUI.cs`, `TuneSliderUI.cs`

**Assets/_Project/ScriptableObjects/TuneConfigs:**
- Purpose: Tune balance and timing parameters
- Contains: Four TuneConfig assets (Tune1-4) with duration, zone, audio
- Pattern: Each tune is a separate .asset file for independent tweaking

**Assets/_Project/Scenes/:**
- Purpose: Game levels and flow
- Key files: `GameLevel.unity` (Phase 1 play scene)

**Assets/_Project/Media/Audio/Tunes/:**
- Purpose: Flute melody audio clips
- Usage: Assigned to TuneConfig assets, plays during tune hold

**Assets/_Project/Animations/Pirate/:**
- Purpose: Character animations
- Contains: FBX model + 14 Mixamo animations organized by type
- Animators: Humanoid rig, Spell animations use triggers (SpellMove, SpellDaze, SpellAttack, SpellFear)

## Key File Locations

**Entry Points:**
- `Assets/_Project/Scripts/Core/GameManager.cs`: Main game loop initialization
- `Assets/_Project/Scenes/GameLevel.unity`: Scene containing GameManager, Player, Snakes

**Configuration:**
- `Assets/_Project/Data/SnakeEnchanter.inputactions`: Input System action map (New Input System only)
- `Assets/_Project/ScriptableObjects/TuneConfigs/Tune*.asset`: Per-tune timing and audio
- `Assets/_Project/ScriptableObjects/ModeSettings/`: Mode-specific configurations (Phase 2)

**Core Logic:**
- `Assets/_Project/Scripts/Core/GameEvents.cs`: Central event hub (static)
- `Assets/_Project/Scripts/Snakes/SnakeAI.cs`: AI state machine (7 states)
- `Assets/_Project/Scripts/TuneSystem/TuneController.cs`: Slider evaluation (ADR-008)
- `Assets/_Project/Scripts/Player/HealthSystem.cs`: HP management

**Testing:**
- No formal test suite (Phase 1). Manual testing in editor via Debug GUI panels.
- Each system has Editor-only debug info (GameManager, HealthSystem, TuneController, SnakeAI)

## Naming Conventions

**Files:**
- PascalCase: `GameManager.cs`, `PlayerController.cs`, `SnakeAI.cs`
- ScriptableObjects: `Tune1.asset`, `Tune2.asset` (numbered by key)
- Scenes: `GameLevel.unity`, `MainMenu.unity`
- Animations: Descriptive + state: `Idle_Pirate.anim`, `Walk_Forward.anim`

**Directories:**
- lowercase: `scripts`, `prefabs`, `scenes`, `animations`
- descriptive: `TuneConfigs`, `ModeSettings`, `Pirate`, `Snakes`

**Namespaces:**
- `SnakeEnchanter.Core`: GameManager, GameEvents
- `SnakeEnchanter.Player`: PlayerController, HealthSystem
- `SnakeEnchanter.Snakes`: SnakeAI, SnakeState
- `SnakeEnchanter.Tunes`: TuneController, TuneConfig, TuneResult, SnakeEffect
- `SnakeEnchanter.Level`: ExitTrigger
- `SnakeEnchanter.UI`: HealthBarUI, TuneSliderUI
- `SnakeEnchanter.Data`: API classes (Phase 2)

**Code Style:**
- Private fields: `_camelCase` with underscore prefix
- Public methods: `PascalCase`
- Properties: `PascalCase`
- Enums: `PascalCase` (e.g., SnakeState, GameMode, TuneResult)
- Events: `OnEventName` pattern (e.g., OnHealthChanged, OnTuneSuccess)

## Where to Add New Code

**New Feature (e.g., Enemy Type):**
- Primary code: `Assets/_Project/Scripts/Snakes/NewEnemyType.cs`
- Inherit from base AI behavior or create new state machine
- Tests: `Assets/_Project/Tests/Snakes/NewEnemyTypeTests.cs` (Phase 2)
- Configuration: `Assets/_Project/ScriptableObjects/EnemyTypes/` (Phase 2)

**New Component/Module (e.g., Inventory):**
- Implementation: `Assets/_Project/Scripts/Systems/InventorySystem.cs`
- Configuration: `Assets/_Project/ScriptableObjects/InventoryConfigs/`
- Events: Add to `GameEvents.cs` (OnInventoryChanged, etc.)
- UI: `Assets/_Project/Scripts/UI/InventoryUI.cs`

**Utilities & Helpers:**
- Shared helpers: `Assets/_Project/Scripts/Utilities/` (if created)
- Extension methods: `Assets/_Project/Scripts/Extensions/`
- Math helpers: Inline in relevant systems or central Utilities

**UI Elements:**
- New HUD displays: `Assets/_Project/Scripts/UI/NewDisplay.cs`
- Canvas auto-creation: Editor tool in `Assets/_Project/Scripts/Editor/CanvasUICreator.cs`
- Sprites: `Assets/_Project/Art-Visuals/UI/`

**Testing:**
- Unit tests: `Assets/_Project/Tests/NamespaceFolder/ClassName.Tests.cs` (Phase 2)
- Integration tests: `Assets/_Project/Tests/Integration/FeatureName.Tests.cs` (Phase 2)
- Test runner: NUnit (Unity standard)

## Special Directories

**Assets/_Project/Scripts/Editor/:**
- Purpose: Editor-only tools
- Generated: No (hand-written)
- Committed: Yes
- Examples: `CanvasUICreator.cs` (auto-creates Canvas + UI structure), `TuneConfigCreator.cs`

**Assets/_Project/ScriptableObjects/:**
- Purpose: Data assets for balance and configuration
- Generated: Partially (created via Editor menu or Inspector)
- Committed: Yes (all production tune configs)
- Examples: `TuneConfigs/Tune1.asset`, `ModeSettings/SimpleMode.asset`

**Assets/External_Assets/:**
- Purpose: Third-party plugins and packs
- Generated: No
- Committed: Yes (part of repo)
- Examples: Gentleland Steampunk UI, Toon Snakes Pack, environment assets

**Assets/Documentation/:**
- Purpose: GDD, timeline, daily logs
- Generated: Partially (logs appended daily)
- Committed: Yes
- Examples: `GDD_v1.4_SnakeEnchanter.txt`, `Arbeitsprotokoll_*.md`

---

*Structure analysis: 2026-02-13*
