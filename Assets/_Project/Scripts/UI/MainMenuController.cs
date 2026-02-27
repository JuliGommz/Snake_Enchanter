/*
====================================================================
* MainMenuController - Main menu with Simple/Advanced mode selection
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-27
* Version: v1.0
*
* AUTHORSHIP CLASSIFICATION:
* [AI-ASSISTED]
* - PlayerPrefs-based mode transfer between scenes
* - SceneManager.LoadScene flow
* - Human reviewed
*
* DEPENDENCIES:
* - UnityEngine.SceneManagement
* - UnityEngine.UI (Button)
* - TMPro (TextMeshProUGUI)
*
* DESIGN RATIONALE:
* - GameManager lives in GameLevel only (no DontDestroyOnLoad)
* - PlayerPrefs key "GameMode" transfers mode selection across scenes
*   0 = Simple, 1 = Advanced
* - GameManager.Start() reads PlayerPrefs via GameModePrefs.Load()
*   and passes it to StartGame()
*
* UI HIERARCHY REQUIRED (MainMenu Scene):
*   Canvas
*     ├── TitleText        (TextMeshProUGUI — "SNAKE ENCHANTER")
*     ├── SimpleModeButton (Button — "Simple")
*     ├── AdvancedModeButton (Button — "Advanced")
*     └── QuitButton       (Button — "Beenden")
*
* HOW MODE IS READ IN GAMELEVEL:
* GameManager.Start() calls StartGame(GameModePrefs.Load())
* which reads the PlayerPrefs key set here.
*
* VERSION HISTORY:
* - v1.0: Initial — title, Simple/Advanced/Quit buttons, PlayerPrefs handoff
====================================================================
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace SnakeEnchanter.UI
{
    /// <summary>
    /// Controls the main menu — mode selection and scene loading.
    /// Saves chosen GameMode to PlayerPrefs before loading GameLevel.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        #region Configuration
        [Header("Buttons")]
        [Tooltip("Starts game in Simple mode (wider timing window, slower drain)")]
        [SerializeField] private Button _simpleModeButton;

        [Tooltip("Starts game in Advanced mode (strict timings, faster drain, charges)")]
        [SerializeField] private Button _advancedModeButton;

        [Tooltip("Quits the application")]
        [SerializeField] private Button _quitButton;

        [Header("Scene Names")]
        [SerializeField] private string _gameLevelSceneName = "GameLevel";
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            // Unlock cursor for menu navigation
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;

            // Wire buttons
            if (_simpleModeButton  != null) _simpleModeButton.onClick.AddListener(OnSimpleModeClicked);
            if (_advancedModeButton != null) _advancedModeButton.onClick.AddListener(OnAdvancedModeClicked);
            if (_quitButton        != null) _quitButton.onClick.AddListener(OnQuitClicked);
        }
        #endregion

        #region Button Callbacks
        /// <summary>Saves Simple mode to PlayerPrefs and loads GameLevel.</summary>
        private void OnSimpleModeClicked()
        {
            GameModePrefs.Save(isAdvanced: false);
            LoadGame();
        }

        /// <summary>Saves Advanced mode to PlayerPrefs and loads GameLevel.</summary>
        private void OnAdvancedModeClicked()
        {
            GameModePrefs.Save(isAdvanced: true);
            LoadGame();
        }

        private void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void LoadGame()
        {
            SceneManager.LoadScene(_gameLevelSceneName);
        }
        #endregion
    }

    /// <summary>
    /// Static helper — single source of truth for GameMode PlayerPrefs key.
    /// Used by MainMenuController (write) and GameManager (read).
    /// </summary>
    public static class GameModePrefs
    {
        private const string KEY = "SnakeEnchanter_GameMode";

        /// <summary>Saves mode choice. Call before LoadScene.</summary>
        public static void Save(bool isAdvanced)
        {
            PlayerPrefs.SetInt(KEY, isAdvanced ? 1 : 0);
            PlayerPrefs.Save();
        }

        /// <summary>Loads saved mode. Returns Simple if no pref saved yet.</summary>
        public static Core.GameMode Load()
        {
            int value = PlayerPrefs.GetInt(KEY, 0); // default = Simple
            return value == 1 ? Core.GameMode.Advanced : Core.GameMode.Simple;
        }
    }
}
