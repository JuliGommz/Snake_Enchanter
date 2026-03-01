/*
====================================================================
* StoryIntroController - Story intro panel before gameplay starts
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-03-01
* Version: v1.1
*
* AUTHORSHIP CLASSIFICATION:
* [AI-ASSISTED]
* - Panel show/hide and any-key detection pattern
* - New Input System integration for any-key
* - Smooth sine-pulse blink effect
* - Human reviewed
*
* DEPENDENCIES:
* - GameManager.cs (SnakeEnchanter.Core)
* - UnityEngine.InputSystem
* - TMPro (optional — for blinking hint text)
*
* UI HIERARCHY REQUIRED (GameLevel Scene):
*   Canvas
*     └── StoryIntroPanel          (Image — full-screen, dark bg)
*           ├── StoryText          (TextMeshProUGUI — story body)
*           └── ContinueHint       (TextMeshProUGUI — "Drücke eine Taste")
*
* SETUP:
* 1. Attach this script to a GameObject in the GameLevel scene
* 2. Assign _introPanel (the full-screen panel GameObject)
* 3. Optionally assign _continueHintText for blinking effect
* 4. GameManager._showIntroOnStart must be true
*
* VERSION HISTORY:
* - v1.0: Initial — show panel, any-key dismiss, call StartGameFromIntro()
* - v1.1: Blink fix — smooth sine-pulse via Time.time, min-alpha floor, removed blinkTimer
====================================================================
*/

using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using SnakeEnchanter.Core;

namespace SnakeEnchanter.UI
{
    /// <summary>
    /// Shows the story intro panel before gameplay starts.
    /// Waits for any key/click, then calls GameManager.StartGameFromIntro().
    /// </summary>
    public class StoryIntroController : MonoBehaviour
    {
        #region Configuration
        [Header("UI References")]
        [Tooltip("The full-screen panel containing story text. Will be shown on Start, hidden on dismiss.")]
        [SerializeField] private GameObject _introPanel;

        [Tooltip("Optional — 'Press any key' hint text. Will blink if assigned.")]
        [SerializeField] private TextMeshProUGUI _continueHintText;

        [Header("Blink Settings")]
        [Tooltip("Pulse speed for the continue hint. Lower = slower. Default 1.5 ≈ one breath every ~2s.")]
        [SerializeField] private float _blinkSpeed = 1.5f;

        [Tooltip("Minimum alpha the hint fades to (0 = fully invisible, 0.2 = always readable).")]
        [SerializeField] private float _blinkMinAlpha = 0.2f;
        #endregion

        #region Private Fields
        private bool _dismissed = false;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            if (_introPanel != null)
                _introPanel.SetActive(true);

            // Cursor visible so nothing feels broken before game starts
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }

        private void Update()
        {
            if (_dismissed) return;

            BlinkHintText();

            // Any keyboard key or left mouse click dismisses the intro
            bool keyPressed   = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
            bool mouseClicked = Mouse.current    != null && Mouse.current.leftButton.wasPressedThisFrame;

            if (keyPressed || mouseClicked)
            {
                DismissIntro();
            }
        }
        #endregion

        #region Intro Flow
        private void DismissIntro()
        {
            _dismissed = true;

            if (_introPanel != null)
                _introPanel.SetActive(false);

            // Hand control to GameManager — it will call StartGame() with the stored mode
            if (GameManager.Instance != null)
                GameManager.Instance.StartGameFromIntro();
            else
                Debug.LogError("StoryIntroController: GameManager.Instance is null — cannot start game!");
        }
        #endregion

        #region Blink Effect
        /// <summary>
        /// Smooth sine-wave pulse using Time.time directly.
        /// (Sin + 1) * 0.5 maps sine's -1..1 range to a clean 0..1 curve.
        /// Lerp prevents fully fading out so text stays readable.
        /// </summary>
        private void BlinkHintText()
        {
            if (_continueHintText == null) return;

            float raw   = (Mathf.Sin(Time.time * _blinkSpeed) + 1f) * 0.5f;   // 0 → 1
            float alpha = Mathf.Lerp(_blinkMinAlpha, 1f, raw);                  // never fully gone
            Color c = _continueHintText.color;
            _continueHintText.color = new Color(c.r, c.g, c.b, alpha);
        }
        #endregion
    }
}
