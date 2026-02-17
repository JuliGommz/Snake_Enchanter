# External Integrations

**Analysis Date:** 2026-02-13

## APIs & External Services

**Backend API (Phase 2):**
- POST `/api/game-session` - Send session stats after game ends
  - Implementation: Planned in GameManager.cs (line 257: "Phase 2: Send data to backend API")
  - Payload structure: SessionData class (tracking mode, duration, result, snakes charmed)
  - Status: Not implemented in Phase 1

- GET `/api/leaderboard?mode=simple/advanced` - Fetch global leaderboard
  - Parameters: `mode` (query string)
  - Expected response: JSON array of top scores
  - Status: Not implemented in Phase 1

- GET `/api/player-stats` - Fetch aggregated player statistics
  - Expected response: Win/loss rate, total snakes charmed, playtime
  - Status: Not implemented in Phase 1

**Notes:**
- Backend integration deferred to Phase 2+
- No third-party API SDK currently in use
- Session data structure defined but not sent: `GameManager.cs` has `SessionData` class for local tracking only

## Data Storage

**Databases:**
- None currently integrated
- Backend API (when implemented) will persist session data
- Phase 2 will likely use REST API with server-side storage

**File Storage:**
- Local filesystem only (no cloud)
- ScriptableObjects for configuration stored in `Assets/_Project/ScriptableObjects/TuneConfigs/`
  - `Tune1_Move.asset`
  - `Tune2_Sleep.asset`
  - `Tune3_Attack.asset`
  - `Tune4_Freeze.asset`

**Caching:**
- None currently implemented
- Tune configurations loaded once at startup via `TuneController.cs` SerializeFields

## Authentication & Identity

**Auth Provider:**
- None currently implemented
- Single-player game (no user accounts in Phase 1)
- Phase 2+ may add optional cloud sync requiring authentication
- No session tokens or user IDs in current system

## Monitoring & Observability

**Error Tracking:**
- None (no Sentry, Bugsnag, or similar)
- Local console logging only via `Debug.Log()` in all gameplay systems

**Logs:**
- Local console output (Development only)
- Key logging points in:
  - `SnakeAI.cs`: State transitions, tune reactions, damage dealt
  - `GameManager.cs`: Game state changes, mode switching
  - `PlayerController.cs`: Movement state changes
  - `TuneController.cs`: Slider state, timing results
  - `HealthSystem.cs`: Damage, healing, death events

**Telemetry:**
- None beyond local session tracking
- Backend API (Phase 2) will collect aggregated stats only

## CI/CD & Deployment

**Hosting:**
- Windows PC (standalone executable) - primary target
- No cloud hosting or server deployment needed for Phase 1

**CI Pipeline:**
- None currently configured
- Git branch strategy implemented (feature branches), manual merge workflow
- No automated build/test pipeline

**Build System:**
- Unity Editor build pipeline (File > Build and Run)
- Target platform: Windows Standalone
- Build output: .exe executable + Data folder

## Environment Configuration

**Required env vars:**
- None currently required
- All configuration via Inspector or ScriptableObjects
- Input bindings in `SnakeEnchanter.inputactions` asset (no external configuration)

**Secrets location:**
- No secrets management system in place
- Backend API credentials (when added Phase 2) should use environment variables
- `.env` file support not yet integrated (planned for Phase 2 backend integration)

## Webhooks & Callbacks

**Incoming:**
- None (client-side only, no server receiving webhooks)

**Outgoing:**
- None in Phase 1
- Phase 2 may implement POST to `/api/game-session` webhook-style

## Event System (Internal)

**Event-Driven Architecture:**
- Central hub: `GameEvents.cs` (SnakeEnchanter.Core namespace)
- Loose coupling via static events (no external API, internal only)

**Key Events:**
- Health Events:
  - `OnHealthChanged(int newHealth)` - Player HP changed
  - `OnPlayerDamaged(int damage)` - Damage taken
  - `OnPlayerHealed(int heal)` - HP restored

- Tune Events:
  - `OnTuneSuccess()` - Any tune succeeded (deprecated, use OnTuneSuccessWithId)
  - `OnTuneSuccessWithId(int tuneNumber)` - Tune 1-4 succeeded (primary event for snake targeting)
  - `OnTuneFailed(bool snakeAttacks)` - Tune failed (bool indicates attack type)
  - `OnTuneStarted(int tuneNumber)` - Player started holding key
  - `OnTuneReleased()` - Player released key

- Game State Events:
  - `OnGameStateChanged(GameState newState)` - Game state transition
  - `OnGameModeChanged(GameMode newMode)` - Mode switched (Simple/Advanced)

## Data Format & Serialization

**JSON Serialization:**
- `JsonUtility` (built-in Unity) for ScriptableObject serialization
- Input Actions Asset: JSON format (`SnakeEnchanter.inputactions`)
- No third-party serialization libraries (e.g., Newtonsoft.JSON) currently used

**ScriptableObject Serialization:**
- YAML binary format (Unity proprietary) for .asset files
- TuneConfig instances serialized via Unity's native format
- No external serialization needed

## Physics Simulation & Raycasting

**Physics System:**
- Built-in Unity Physics 3D
- Collider-based detection:
  - OnTriggerEnter for contact damage (SnakeAI.cs)
  - Collider.enabled toggled by snake states (Sleeping, MovedAway disable collision)

**Raycasting:**
- Line-of-sight detection for player visibility (SnakeAI will use in v1.3.1+)
- Damage raycast check for snake attacks (Phase 2+ for ranged attacks)
- No external physics libraries

## Third-Party Asset Integrations

**Imported Asset Packs:**
- Pirate Character Pack (Mixamo animations + rig)
- Toon Snakes Pack (Meshtint Studio - models + materials)
- Caves Parts Set (environment props)
- Dwarven Pack (environment props)
- Steampunk UI Pack (UI components and shaders)
- Gentleland Graphics (custom UI styling)

**No Third-Party SDKs:**
- No analytics (Amplitude, Mixpanel)
- No ads (AdMob, Unity Ads)
- No social (Steam, Discord)
- No payment processing (IAP, Stripe)

---

*Integration audit: 2026-02-13*
