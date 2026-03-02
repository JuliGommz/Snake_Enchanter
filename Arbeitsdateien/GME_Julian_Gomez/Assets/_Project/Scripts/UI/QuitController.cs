/*
====================================================================
* QuitController - Escape-to-Quit + Cursor Release
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-03-03
* Version: v1.0
*
* AUTHORSHIP CLASSIFICATION:
* [HUMAN] — Minimal quit handler, no AI assistance needed.
*
* DEPENDENCIES:
* - UnityEngine.InputSystem (Keyboard)
* - UnityEngine.SceneManagement
*
* USAGE:
* Attach to any persistent GameObject in MainMenu AND GameLevel.
* Escape key quits the application at any time.
* In-Editor: Escape stops play mode instead of quitting.
*
* DESIGN RATIONALE:
* - Single responsibility: only handles application exit.
* - Releases cursor before quitting so OS regains control cleanly.
* - Works in both scenes without depending on GameManager state.
====================================================================
*/

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using SnakeEnchanter.Core;

namespace SnakeEnchanter.UI
{
    /// <summary>
    /// Listens for Escape key press and quits the application.
    /// Attach to any persistent GameObject in every scene.
    /// </summary>
    public class QuitController : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("If true, Escape returns to Main Menu instead of quitting (use in GameLevel).")]
        [SerializeField] private bool _escapeToMainMenu = false;

        [SerializeField] private string _mainMenuSceneName = "MainMenu";

        private void Update()
        {
            if (Keyboard.current == null) return;
            if (!Keyboard.current.escapeKey.wasPressedThisFrame) return;

            if (_escapeToMainMenu)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible   = true;
                GameEvents.ClearAllEvents();
                SceneManager.LoadScene(_mainMenuSceneName);
            }
            else
            {
                Quit();
            }
        }

        private static void Quit()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
