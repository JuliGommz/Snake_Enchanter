# Technology Stack

**Analysis Date:** 2026-02-13

## Languages

**Primary:**
- C# - Used for all game logic, AI systems, UI, and core mechanics

**Secondary:**
- JSON - Configuration for Input Actions and project data serialization

## Runtime

**Environment:**
- Unity Editor 6000.0.62f1 (2022 LTS equivalent)
- .NET Framework (Standard 2.1 via Unity's managed backend)

**Package Manager:**
- Unity Package Manager (UPM)
- Lockfile: `Packages/packages-lock.json` (present)

## Frameworks

**Core:**
- Universal Render Pipeline (URP) v17.0.4 - Modern rendering pipeline configured in `ProjectSettings/GraphicsSettings.asset` with custom render pipeline GUID
- Cinemachine v3.1.5 - Advanced camera system for first-person follow cam with split control (position via Cinemachine, pitch via script)
- Timeline v1.8.9 - Animation sequencing framework (referenced for potential Phase 2 cutscenes)
- Visual Scripting v1.9.7 - Logic composition tool (included but not actively used in codebase)

**Input:**
- New Input System v1.14.2 (required, never Legacy Input)
  - Asset: `Assets/_Project/Data/SnakeEnchanter.inputactions`
  - Actions: Player, Tune1-4, Move, Look, Jump, Crouch, Pause
  - Enforced via project rule in CLAUDE.md

**Animation:**
- Animator Controller system (Unity built-in)
- Animation parameters with spaces (Toon Cobra only): `"Bite Attack"`, `"Breath Attack"`, `"Projectile Attack"`
- Animator reference in `TuneController.cs` for spell animations (v2.4+)

**Physics:**
- Unity Physics 3D (built-in module)
- CharacterController for player movement (`PlayerController.cs`)
- Collider-based snake interaction (TriggerEnter for contact damage)
- Raycast-based line-of-sight detection for snake AI perception

**Testing:**
- Test Framework v1.6.0 - Unit test infrastructure (included, not actively used in Phase 1)

**Build/Dev:**
- Rider IDE Integration v3.0.38 - C# debugger support
- Visual Studio Integration v2.0.25 - Alternative IDE support
- Collaboration Proxy v2.10.0 - Unity Cloud Save integration (not used)

## Key Dependencies

**Critical:**
- `com.unity.inputsystem` v1.14.2 - Mandatory for all input (keyboard/mouse capture)
- `com.unity.cinemachine` v3.1.5 - Camera follow system (first-person head tracking)
- `com.unity.render-pipelines.universal` v17.0.4 - Rendering backbone
- `com.unity.animation` v1.0.0 - Animator and state machine for snakes/player

**Infrastructure:**
- `com.unity.2d.sprite` v1.0.0 - UI sprite rendering (Genshin-style slider, health bar)
- `com.unity.ui` v2.0.0 - Canvas, Image, Button components for UI
- `com.unity.textmeshpro` - TextMeshPro for UI text rendering
- `com.unity.ai.navigation` v2.0.9 - NavMesh system (Phase 2: snake patrol)

## Configuration

**Environment:**
- Active Input Handling: "Input System Package (New)" (enforced in ProjectSettings/PlayerSettings.asset)
- Color Space: Linear (m_ActiveColorSpace: 1)
- Graphics API: Default (Windows uses DirectX 11/12)
- Target Resolution: 1920x1080 (primary), Ultrawide supported
- Frame Rate: 60 FPS (standard target)

**Build:**
- Player Settings: `Assets/ProjectSettings/PlayerSettings.asset`
  - Product Name: Snake_Enchanter
  - Bundle Version: 0.1.0
  - Company: DefaultCompany
- Graphics Settings: URP render pipeline with custom settings
- Quality Settings: Mobile profile (2 pixel lights, shadow distance 40)

**Rendering:**
- Render Pipeline: URP (guid: `4b83569d67af61e458304325a23e5dfd`)
- Shader: URP/Lit (GUID: `933532a4fcc9baf4fa0491de14d08ed7`, fileID: 4800000)
- Multi-threaded Rendering: Enabled (m_MTRendering: 1)
- GPU Skinning: Enabled (for skeletal animation on Pirate avatar)

## Platform Requirements

**Development:**
- Unity Editor v6000.0.62f1 or compatible
- C# 9.0+ language features support
- New Input System package enabled in project settings
- 1920x1080 minimum display (tested) or ultrawide compatible

**Production:**
- Windows PC (primary deployment target)
- DirectX 11 or later graphics support
- .NET runtime via Unity standalone player
- Resolution: 1920x1080 native, ultrawide scaling
- Frame rate: Stable 60 FPS

## Asset Pipeline

**Skeletal Animation:**
- Player Avatar: Pirate character (FBX rigged model)
  - Location: `Assets/_Project/Animations/Pirate/` (paths in CLAUDE.md)
  - 14 Mixamo animations imported
  - Humanoid rig for IK-compatible animation
  - Material Import Mode: Embedded (mode: 2)

**Snake Models:**
- Toon Snakes Pack (Meshtint Studio)
  - Basic: Toon Snake (Bite only)
  - Advanced: Toon Cobra (Bite + Breath + Projectile)
  - No Plugins directory (assets imported to main Assets folder)

**UI Assets:**
- Steampunk UI Pack (Gentleland)
  - Shader: URP/Lit
  - Font: Arvo SDE (custom license-free font)

**Audio:**
- Lizenzfreie Flötenmelodien (license-free flute melodies)
- Duration: 5-12 seconds per tune
- References in TuneConfig assets

---

*Stack analysis: 2026-02-13*
