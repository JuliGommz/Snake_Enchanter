/*
====================================================================
* GameManager - Central game state and loop management
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-05
* Version: 1.4 - Code quality: M1 magic number + D3 placeholder comment
*
* ⚠️ WICHTIG: KOMMENTIERUNG NICHT LÖSCHEN! ⚠️
* Diese detaillierte Authorship-Dokumentation ist für die akademische
* Bewertung erforderlich und darf nicht entfernt werden!
*
* AUTHORSHIP CLASSIFICATION:
* [AI-ASSISTED]
* - Singleton pattern and game state management
* - Mode switching (Simple/Advanced)
* - Session tracking for backend API
* - Refactored to eliminate SerializeField duplication (v1.1)
* - Human reviewed and will modify as needed
*
* DEPENDENCIES:
* - GameEvents.cs (SnakeEnchanter.Core)
* - HealthSystem.cs (SnakeEnchanter.Player)
* - PlayerController.cs (SnakeEnchanter.Player)
* - SnakeAI.cs (SnakeEnchanter.Snakes)
*
* DESIGN RATIONALE:
* - GDD Section 3: Full game loop management
* - Singleton ensures single instance
* - Tracks session data for backend API (GDD Section 7.2)
* - Mode switching delegates to component-specific ApplyModeSettings()
* - No duplicated configuration data (Single Source of Truth)
*
* NOTES:
* - Phase 1 implementation — core loop only
* - Backend API integration in Phase 2
* - Pause system in Phase 2
* - Result screen in Phase 2
*
* VERSION HISTORY:
* - v1.0: Initial — game states, mode switching, session tracking
* - v1.1: Refactored to eliminate drain rate SerializeFields
* - v1.1.1: Fixed namespace references + Unity 2023 API
* - v1.2: Start() reads GameModePrefs (PlayerPrefs) from MainMenu
* - v1.3: Backend API integration (POST session on game end)
* - v1.4: M1 — replaced magic 33.3f with HeartsPerHealthUnit constant; D3 — fourthTuneUnlocked comment added
====================================================================
*/

using UnityEngine;

// Enums defined OUTSIDE namespace to prevent circular dependencies
namespace SnakeEnchanter.Core
{
    /// <summary>
    /// Game states for the core loop.
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        Won,
        Lost
    }

    /// <summary>
    /// Game modes as defined in GDD Section 3.3.
    /// </summary>
    public enum GameMode
    {
        Simple,
        Advanced
    }
}

namespace SnakeEnchanter.Core
{
    /// <summary>
    /// Central game manager — handles game state, mode, and session tracking.
    /// Singleton pattern ensures single instance across scenes.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        private static GameManager _instance;

        /// <summary>Global GameManager instance.</summary>
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<GameManager>();
                    if (_instance == null)
                    {
                        Debug.LogError("GameManager: No instance found in scene!");
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region Configuration
        [Header("Game Settings")]
        [SerializeField] private GameMode _gameMode = GameMode.Simple;

        [Header("References")]
        [SerializeField] private Player.HealthSystem _healthSystem;
        [SerializeField] private Player.PlayerController _playerController;
        [SerializeField] private Tunes.TuneController _tuneController;

        [Header("Intro")]
        [Tooltip("Show story intro panel before gameplay starts. Disable for quick testing.")]
        [SerializeField] private bool _showIntroOnStart = true;
        #endregion

        #region Private Fields
        // 100 HP / 3 hearts = 33.3 HP per heart (GDD Section 4.1)
        private const float HeartsPerHealthUnit = 33.3f;

        private GameState _currentState = GameState.MainMenu;
        private float _sessionStartTime;
        private float _sessionEndTime;

        // Player spawn position — captured once at Start(), restored on RestartGame()
        private Vector3    _playerSpawnPosition;
        private Quaternion _playerSpawnRotation;

        // Session tracking (GDD Section 7.2)
        private int _successfulTuneCasts = 0;
        private int _failedTuneCasts = 0;
        private int _tooEarlyCount = 0;
        private int _tooLateCount = 0;
        private int _snakeAttackCount = 0;
        private int _totalDamageTaken = 0;
        private int _totalHPRestored = 0;
        private int _startingHP;
        #endregion

        #region Properties
        /// <summary>Current game state.</summary>
        public GameState CurrentState => _currentState;

        /// <summary>Current game mode.</summary>
        public GameMode CurrentMode => _gameMode;

        /// <summary>Is the game currently in play?</summary>
        public bool IsPlaying => _currentState == GameState.Playing;

        /// <summary>Session elapsed time in seconds.</summary>
        public float SessionTime => IsPlaying ? Time.time - _sessionStartTime : _sessionEndTime - _sessionStartTime;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Singleton enforcement
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            // Auto-find references if not assigned (Unity 2023+ API)
            if (_healthSystem == null)
                _healthSystem = FindFirstObjectByType<Player.HealthSystem>();
            if (_playerController == null)
                _playerController = FindFirstObjectByType<Player.PlayerController>();
            if (_tuneController == null)
                _tuneController = FindFirstObjectByType<Tunes.TuneController>();
        }

        private void Start()
        {
            // Capture spawn position BEFORE StartGame moves anything
            if (_playerController != null)
            {
                _playerSpawnPosition = _playerController.transform.position;
                _playerSpawnRotation = _playerController.transform.rotation;
            }

            // Read mode chosen in MainMenu via PlayerPrefs (set by MainMenuController)
            // Key: "SnakeEnchanter_GameMode" — 0 = Simple, 1 = Advanced, default = Simple
            int modeValue = PlayerPrefs.GetInt("SnakeEnchanter_GameMode", 0);
            GameMode selectedMode = (modeValue == 1) ? GameMode.Advanced : GameMode.Simple;

            if (_showIntroOnStart)
            {
                // Store mode — StoryIntroController will call StartGameFromIntro() on dismiss
                _gameMode = selectedMode;
                if (_healthSystem != null) _healthSystem.SetPassiveDrainEnabled(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                StartGame(selectedMode);
            }
        }

        private void OnEnable()
        {
            // Subscribe to game events for session tracking
            GameEvents.OnGameWin += OnGameWin;
            GameEvents.OnGameOver += OnGameLost;
            GameEvents.OnTuneSuccess += OnTuneSuccessTracking;
            GameEvents.OnTuneFailed += OnTuneFailedTracking;
            GameEvents.OnPlayerDamaged += OnPlayerDamagedTracking;
            GameEvents.OnPlayerHealed += OnPlayerHealedTracking;
        }

        private void OnDisable()
        {
            GameEvents.OnGameWin -= OnGameWin;
            GameEvents.OnGameOver -= OnGameLost;
            GameEvents.OnTuneSuccess -= OnTuneSuccessTracking;
            GameEvents.OnTuneFailed -= OnTuneFailedTracking;
            GameEvents.OnPlayerDamaged -= OnPlayerDamagedTracking;
            GameEvents.OnPlayerHealed -= OnPlayerHealedTracking;
        }
        #endregion

        #region Game Flow
        /// <summary>Starts a new game session with the specified mode.</summary>
        public void StartGame(GameMode mode)
        {
            _gameMode = mode;
            _currentState = GameState.Playing;

            ResetSessionData();
            _sessionStartTime = Time.time;

            // Apply mode settings to all systems
            ApplyModeSettings(mode);

            // Store starting HP
            if (_healthSystem != null)
            {
                _startingHP = Mathf.RoundToInt(_healthSystem.CurrentHealth);
            }

            // Enable player systems
            if (_playerController != null) _playerController.SetMovementEnabled(true);
            if (_tuneController != null) _tuneController.SetTuneInputEnabled(true);

            // Lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Debug.Log($"GameManager: Game started — Mode: {mode}");
        }

        /// <summary>Called by StoryIntroController when the intro is dismissed.</summary>
        public void StartGameFromIntro()
        {
            StartGame(_gameMode);
        }

        /// <summary>Handles win condition.</summary>
        private void OnGameWin()
        {
            if (_currentState != GameState.Playing) return;

            _currentState = GameState.Won;
            _sessionEndTime = Time.time;
            EndGame(true);
        }

        /// <summary>Handles lose condition (HP = 0).</summary>
        private void OnGameLost()
        {
            if (_currentState != GameState.Playing) return;

            _currentState = GameState.Lost;
            _sessionEndTime = Time.time;
            EndGame(false);
        }

        /// <summary>Common end-game logic for both win and lose.</summary>
        private void EndGame(bool success)
        {
            // Disable player input
            if (_playerController != null) _playerController.SetMovementEnabled(false);
            if (_tuneController != null) _tuneController.SetTuneInputEnabled(false);

            // Unlock cursor for UI
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            LogSessionSummary(success);
            SendSessionToBackend(success);

            Debug.Log($"GameManager: Game ended — {(success ? "VICTORY!" : "DEFEAT")} | Time: {SessionTime:F1}s");
        }

        /// <summary>Restarts the current game with same mode.</summary>
        public void RestartGame()
        {
            // Reset health
            if (_healthSystem != null) _healthSystem.ResetHealth();

            // Reset player position to spawn point
            ResetPlayerPosition();

            // Reset all snakes
            Snakes.SnakeAI[] allSnakes = FindObjectsByType<Snakes.SnakeAI>(FindObjectsSortMode.None);
            foreach (var snake in allSnakes)
            {
                snake.ResetSnake();
            }

            // Reset all exit triggers so win condition can fire again
            Level.ExitTrigger[] exitTriggers = FindObjectsByType<Level.ExitTrigger>(FindObjectsSortMode.None);
            foreach (var trigger in exitTriggers)
            {
                trigger.ResetTrigger();
            }

            StartGame(_gameMode);
        }

        /// <summary>
        /// Teleports player back to spawn position.
        /// Disables CharacterController during move to avoid position fighting.
        /// </summary>
        private void ResetPlayerPosition()
        {
            if (_playerController == null) return;

            // CharacterController fights transform.position changes — disable, move, re-enable
            CharacterController cc = _playerController.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            _playerController.transform.SetPositionAndRotation(_playerSpawnPosition, _playerSpawnRotation);

            if (cc != null) cc.enabled = true;
        }
        #endregion

        #region Mode Settings
        /// <summary>
        /// Applies mode-specific settings to all systems.
        /// GDD Section 3.3: Simple vs Advanced differences.
        /// Delegates to component-specific ApplyModeSettings() methods.
        /// </summary>
        private void ApplyModeSettings(GameMode mode)
        {
            // Delegate to HealthSystem (owns drain rate configuration)
            if (_healthSystem != null)
            {
                _healthSystem.ApplyModeSettings(mode);
            }

            // Configure TuneController
            if (_tuneController != null)
            {
                _tuneController.SetSimpleMode(mode == GameMode.Simple);
            }

            Debug.Log($"GameManager: Mode settings applied — {mode}");
        }
        #endregion

        #region Session Tracking (GDD Section 7.2)
        private void ResetSessionData()
        {
            _successfulTuneCasts = 0;
            _failedTuneCasts = 0;
            _tooEarlyCount = 0;
            _tooLateCount = 0;
            _snakeAttackCount = 0;
            _totalDamageTaken = 0;
            _totalHPRestored = 0;
        }

        private void OnTuneSuccessTracking()
        {
            _successfulTuneCasts++;
        }

        private void OnTuneFailedTracking(bool snakeAttacks)
        {
            _failedTuneCasts++;

            if (snakeAttacks)
            {
                _tooLateCount++;
                _snakeAttackCount++;
            }
            else
            {
                _tooEarlyCount++;
            }
        }

        private void OnPlayerDamagedTracking(int damage)
        {
            _totalDamageTaken += damage;
        }

        private void OnPlayerHealedTracking(int amount)
        {
            _totalHPRestored += amount;
        }

        /// <summary>Logs session summary to Unity console.</summary>
        private void LogSessionSummary(bool success)
        {
            int endingHP = _healthSystem != null ? Mathf.RoundToInt(_healthSystem.CurrentHealth) : 0;

            Debug.Log("========== SESSION SUMMARY ==========");
            Debug.Log($"Mode: {_gameMode}");
            Debug.Log($"Result: {(success ? "WIN" : "LOSE")}");
            Debug.Log($"Time: {SessionTime:F1}s");
            Debug.Log($"Starting HP: {_startingHP} | Ending HP: {endingHP}");
            Debug.Log($"Successful Tunes: {_successfulTuneCasts}");
            Debug.Log($"Failed Tunes: {_failedTuneCasts} (Early: {_tooEarlyCount}, Late: {_tooLateCount})");
            Debug.Log($"Snake Attacks: {_snakeAttackCount}");
            Debug.Log($"Total Damage: {_totalDamageTaken} | Total Healed: {_totalHPRestored}");
            Debug.Log("=====================================");
        }

        /// <summary>
        /// Sends session data to backend via ApiManager.
        /// Fail-silent: if ApiManager is not found, logs warning only.
        /// </summary>
        private void SendSessionToBackend(bool success)
        {
            if (Data.ApiManager.Instance == null)
            {
                Debug.LogWarning("GameManager: ApiManager not found — session not sent to backend.");
                return;
            }

            float endingHp = _healthSystem != null ? _healthSystem.CurrentHealth : 0f;

            var sessionData = new Data.ApiManager.SessionData
            {
                sessionId             = System.Guid.NewGuid().ToString(),
                modeType              = _gameMode == GameMode.Advanced ? "advanced" : "simple",
                success               = success,
                completionTime        = Mathf.RoundToInt(SessionTime),
                startingHp            = _startingHP,
                endingHp              = endingHp,
                totalDamageTaken      = _totalDamageTaken,
                totalHpRestored       = _totalHPRestored,
                successfulTuneCasts   = _successfulTuneCasts,
                failedTuneCasts       = _failedTuneCasts,
                tooEarlyCount         = _tooEarlyCount,
                tooLateCount          = _tooLateCount,
                snakeBiteCount        = _snakeAttackCount,
                // PLACEHOLDER: Tune 4 (Freeze) was cut from scope. Field kept for potential post-submission expansion.
                fourthTuneUnlocked    = false,
                heartsRemaining       = Mathf.RoundToInt(endingHp / HeartsPerHealthUnit)
            };

            Data.ApiManager.Instance.PostSession(sessionData);
        }

        #endregion

        #region Debug Helpers
#if UNITY_EDITOR
        [Header("Debug - Editor Only")]
        [SerializeField] private bool _showDebugInfo = true;

        private void OnGUI()
        {
            if (!_showDebugInfo) return;

            GUILayout.BeginArea(new Rect(Screen.width - 310, 10, 300, 220));
            GUIStyle headerStyle = new GUIStyle(GUI.skin.label) { richText = true };
            GUI.color = Color.white;

            GUILayout.Label($"<b>GameManager Debug</b>", headerStyle);
            GUILayout.Label($"State: {_currentState} | Mode: {_gameMode}");
            GUILayout.Label($"Session Time: {SessionTime:F1}s");
            GUILayout.Label($"Tunes OK: {_successfulTuneCasts} | Fail: {_failedTuneCasts}");
            GUILayout.Label($"Damage: {_totalDamageTaken} | Healed: {_totalHPRestored}");

            GUILayout.Space(5);

            // Display current drain rates from HealthSystem
            if (_healthSystem != null)
            {
                GUILayout.Label($"<color=cyan>Drain Rates (from HealthSystem):</color>", headerStyle);
                GUILayout.Label($"Simple: {_healthSystem.SimpleDrainRate}/sec");
                GUILayout.Label($"Advanced: {_healthSystem.AdvancedDrainRate}/sec");
            }

            GUILayout.Space(5);

            if (GUILayout.Button("Restart Game"))
            {
                RestartGame();
            }

            if (GUILayout.Button("Toggle Mode"))
            {
                var newMode = _gameMode == GameMode.Simple ? GameMode.Advanced : GameMode.Simple;
                ApplyModeSettings(newMode);
                _gameMode = newMode;
            }

            GUILayout.EndArea();
        }
#endif
        #endregion
    }
}
