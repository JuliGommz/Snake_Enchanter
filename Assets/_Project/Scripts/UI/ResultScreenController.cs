/*
====================================================================
* ResultScreenController - Win/Lose result screen
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-27
* Version: v1.2
*
* AUTHORSHIP CLASSIFICATION:
* [AI-ASSISTED]
* - Event subscription pattern (OnGameWin / OnGameOver)
* - Scene management for Retry and MainMenu flow
* - Human reviewed
*
* DEPENDENCIES:
* - GameEvents.cs (SnakeEnchanter.Core)
* - GameManager.cs (SnakeEnchanter.Core) — RestartGame()
* - UnityEngine.SceneManagement
* - TMPro (TextMeshProUGUI)
*
* DESIGN RATIONALE:
* - ResultPanel hidden by default — shown only on Win or Lose
* - Retry: resets all systems in-scene (no reload) via GameManager.RestartGame()
* - Main Menu: loads MainMenu scene + clears all events
* - Cursor unlocked by GameManager.EndGame() before this panel appears
*
* UI HIERARCHY REQUIRED (GameCanvas):
*   ResultPanel (GameObject — hidden by default)
*     ├── ResultText       (TextMeshProUGUI — "VICTORY!" / "GAME OVER")
*     ├── SubtitleText     (TextMeshProUGUI — session time, optional)
*     ├── RetryButton      (Button)
*     ├── MainMenuButton   (Button)
*     └── DeleteRunButton  (Button — optional, calls DELETE /api/game-session/:id)
*
* VERSION HISTORY:
* - v1.0: Initial — Win/Lose panel, Retry, Main Menu
* - v1.1: Added _waitForEndingStory flag + public ShowWin() for EndingStoryController handoff
* - v1.2: Added Delete Run button — calls ApiManager.DeleteSession() to remove last session
====================================================================
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using SnakeEnchanter.Core;
using SnakeEnchanter.Data;

namespace SnakeEnchanter.UI
{
    /// <summary>
    /// Shows a result panel when the game ends (Win or Lose).
    /// Provides Retry and Main Menu buttons.
    /// Subscribes to GameEvents.OnGameWin and GameEvents.OnGameOver.
    /// </summary>
    public class ResultScreenController : MonoBehaviour
    {
        #region Configuration
        [Header("Panel")]
        [Tooltip("Root panel GameObject — hidden by default, shown on game end.")]
        [SerializeField] private GameObject _resultPanel;

        [Header("Texts")]
        [Tooltip("Main result label — set to 'VICTORY!' or 'GAME OVER'")]
        [SerializeField] private TextMeshProUGUI _resultText;

        [Tooltip("Optional subtitle — shows session time. Leave empty to skip.")]
        [SerializeField] private TextMeshProUGUI _subtitleText;

        [Header("Buttons")]
        [Tooltip("Retry button — restarts the current session in-scene.")]
        [SerializeField] private Button _retryButton;

        [Tooltip("Main Menu button — loads the MainMenu scene.")]
        [SerializeField] private Button _mainMenuButton;

        [Tooltip("Delete Run button — removes this session from the backend. Optional.")]
        [SerializeField] private Button _deleteRunButton;

        [Header("Text Content")]
        [SerializeField] private string _winText    = "VICTORY!";
        [SerializeField] private string _loseText   = "GAME OVER";
        [SerializeField] private Color  _winColor   = new Color(0.2f, 1f, 0.4f);
        [SerializeField] private Color  _loseColor  = new Color(1f, 0.25f, 0.25f);

        [Header("Scene Names")]
        [SerializeField] private string _mainMenuSceneName = "MainMenu";

        [Header("Ending Story")]
        [Tooltip("If true, Win result waits for EndingStoryController to call ShowWin(). Lose always shows immediately.")]
        [SerializeField] private bool _waitForEndingStory = true;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Panel hidden at game start
            if (_resultPanel != null)
                _resultPanel.SetActive(false);
        }

        private void OnEnable()
        {
            GameEvents.OnGameWin  += HandleWin;
            GameEvents.OnGameOver += HandleLose;
        }

        private void OnDisable()
        {
            GameEvents.OnGameWin  -= HandleWin;
            GameEvents.OnGameOver -= HandleLose;
        }

        private void Start()
        {
            // Wire buttons
            if (_retryButton != null)
                _retryButton.onClick.AddListener(OnRetryClicked);

            if (_mainMenuButton != null)
                _mainMenuButton.onClick.AddListener(OnMainMenuClicked);

            if (_deleteRunButton != null)
                _deleteRunButton.onClick.AddListener(OnDeleteRunClicked);
        }
        #endregion

        #region Event Handlers
        private void HandleWin()
        {
            // If EndingStoryController is active, it will call ShowWin() after the story panel.
            // Lose always shows immediately — only Win is delayed.
            if (_waitForEndingStory) return;
            ShowResult(win: true);
        }

        /// <summary>
        /// Called by EndingStoryController after the ending story is dismissed.
        /// Shows the Win result panel directly.
        /// </summary>
        public void ShowWin()
        {
            ShowResult(win: true);
        }

        private void HandleLose()
        {
            ShowResult(win: false);
        }
        #endregion

        #region Result Display
        /// <summary>
        /// Shows the result panel with appropriate text and color.
        /// Called by GameManager.EndGame() indirectly via GameEvents.
        /// </summary>
        private void ShowResult(bool win)
        {
            if (_resultPanel != null)
                _resultPanel.SetActive(true);

            if (_resultText != null)
            {
                _resultText.text  = win ? _winText  : _loseText;
                _resultText.color = win ? _winColor : _loseColor;
            }

            // Optional subtitle: show session time
            if (_subtitleText != null && GameManager.Instance != null)
            {
                float time = GameManager.Instance.SessionTime;
                int   min  = Mathf.FloorToInt(time / 60f);
                int   sec  = Mathf.FloorToInt(time % 60f);
                _subtitleText.text = $"Time: {min:00}:{sec:00}";
            }
        }
        #endregion

        #region Button Callbacks
        /// <summary>
        /// Retry — resets health, snakes and restarts in the same scene.
        /// GameManager.RestartGame() handles all reset logic.
        /// </summary>
        private void OnRetryClicked()
        {
            if (_resultPanel != null)
                _resultPanel.SetActive(false);

            if (GameManager.Instance != null)
                GameManager.Instance.RestartGame();
        }

        /// <summary>
        /// Main Menu — clears all events and loads the MainMenu scene.
        /// </summary>
        private void OnMainMenuClicked()
        {
            GameEvents.ClearAllEvents();
            SceneManager.LoadScene(_mainMenuSceneName);
        }

        /// <summary>
        /// Delete Run — sends DELETE /api/game-session/:id to remove this session.
        /// Button is disabled after click to prevent double-delete.
        /// </summary>
        private void OnDeleteRunClicked()
        {
            if (ApiManager.Instance == null) return;

            // Disable button immediately to prevent double-click
            if (_deleteRunButton != null) _deleteRunButton.interactable = false;

            ApiManager.Instance.DeleteSession(success =>
            {
                if (_deleteRunButton != null)
                    _deleteRunButton.interactable = !success; // re-enable only if failed
            });
        }
        #endregion
    }
}
