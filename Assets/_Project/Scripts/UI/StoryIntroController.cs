/*
====================================================================
* StoryIntroController - Story intro panel before gameplay starts
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-03-01
* Version: v1.2
*
* AUTHORSHIP CLASSIFICATION:
* [AI-ASSISTED]
* - Panel show/hide and any-key detection pattern
* - New Input System integration for any-key
* - Smooth sine-pulse blink effect
* - CanvasGroup FadeIn/FadeOut coroutines (mirrored from EndingStoryController)
* - Human reviewed
*
* DEPENDENCIES:
* - GameManager.cs (SnakeEnchanter.Core)
* - UnityEngine.InputSystem
* - TMPro (optional — for blinking hint text)
* - CanvasGroup component on StoryIntroPanel (added automatically if missing)
*
* UI HIERARCHY REQUIRED (GameLevel Scene):
*   Canvas
*     └── StoryIntroPanel          (Image — full-screen, dark bg, CanvasGroup auto-added)
*           ├── StoryText          (TextMeshProUGUI — story body)
*           └── ContinueHint       (TextMeshProUGUI — "Drücke eine Taste")
*
* SETUP:
* 1. Attach this script to a GameObject in the GameLevel scene
* 2. Assign _introPanel (the full-screen panel GameObject)
* 3. Optionally assign _continueHintText for blinking effect
* 4. GameManager._showIntroOnStart must be true
*
* FLOW:
* Start → FadeIn panel → player reads (hint blinks) → any key → FadeOut → GameManager.StartGameFromIntro()
*
* VERSION HISTORY:
* - v1.0: Initial — show panel, any-key dismiss, call StartGameFromIntro()
* - v1.1: Blink fix — smooth sine-pulse via Time.time, min-alpha floor, removed blinkTimer
* - v1.2: Fade In/Out — CanvasGroup fade coroutines (mirrors EndingStoryController pattern)
*         Input blocked during fade + cooldown via _inputReady flag
====================================================================
*/

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using SnakeEnchanter.Core;

namespace SnakeEnchanter.UI
{
    /// <summary>
    /// Shows the story intro panel before gameplay starts with fade in/out.
    /// Waits for any key/click after fade completes, then fades out and calls GameManager.StartGameFromIntro().
    /// </summary>
    public class StoryIntroController : MonoBehaviour
    {
        #region Configuration
        [Header("UI References")]
        [Tooltip("The full-screen panel containing story text. CanvasGroup is added automatically.")]
        [SerializeField] private GameObject _introPanel;

        [Tooltip("Optional — 'Press any key' hint text. Will blink after fade-in completes.")]
        [SerializeField] private TextMeshProUGUI _continueHintText;

        [Header("Fade Settings")]
        [Tooltip("Seconds to fade the panel in.")]
        [SerializeField] private float _fadeInDuration = 1.2f;

        [Tooltip("Seconds to fade the panel out before handing to GameManager.")]
        [SerializeField] private float _fadeOutDuration = 0.6f;

        [Header("Input Cooldown")]
        [Tooltip("Seconds after fade-in before key input is accepted. Prevents accidental skip from previous keypress.")]
        [SerializeField] private float _inputCooldown = 0.8f;

        [Header("Blink Settings")]
        [Tooltip("Pulse speed for the continue hint. Lower = slower. Default 1.5 ≈ one breath every ~2s.")]
        [SerializeField] private float _blinkSpeed = 1.5f;

        [Tooltip("Minimum alpha the hint fades to (0 = fully invisible, 0.2 = always readable).")]
        [SerializeField] private float _blinkMinAlpha = 0.2f;
        #endregion

        #region Private Fields
        private CanvasGroup _canvasGroup;
        private bool _inputReady = false;
        private bool _dismissed  = false;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (_introPanel == null) return;

            // Get or add CanvasGroup — no manual component setup needed in Inspector
            _canvasGroup = _introPanel.GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = _introPanel.AddComponent<CanvasGroup>();

            // Start invisible — FadeIn coroutine will reveal it
            _canvasGroup.alpha = 0f;
            _introPanel.SetActive(false);

            // Hide hint text until fade completes
            if (_continueHintText != null)
                _continueHintText.color = new Color(
                    _continueHintText.color.r,
                    _continueHintText.color.g,
                    _continueHintText.color.b,
                    0f);
        }

        private void Start()
        {
            // Cursor visible so nothing feels broken before game starts
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;

            StartCoroutine(FadeIn());
        }

        private void Update()
        {
            // Block input during fade-in + cooldown
            if (!_inputReady || _dismissed) return;

            BlinkHintText();

            // Any keyboard key or left mouse click dismisses the intro
            bool keyPressed   = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
            bool mouseClicked = Mouse.current    != null && Mouse.current.leftButton.wasPressedThisFrame;

            if (keyPressed || mouseClicked)
            {
                _dismissed = true;
                StartCoroutine(FadeOutAndStart());
            }
        }
        #endregion

        #region Fade Coroutines
        /// <summary>Fades the intro panel in, then enables key input after cooldown.</summary>
        private IEnumerator FadeIn()
        {
            if (_canvasGroup == null) yield break;

            _introPanel.SetActive(true);
            _canvasGroup.alpha = 0f;

            float elapsed = 0f;
            while (elapsed < _fadeInDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Clamp01(elapsed / _fadeInDuration);
                yield return null;
            }
            _canvasGroup.alpha = 1f;

            // Brief cooldown prevents last keypress from MainMenu/previous scene from skipping
            yield return new WaitForSeconds(_inputCooldown);
            _inputReady = true;
        }

        /// <summary>Fades the intro panel out, then calls GameManager.StartGameFromIntro().</summary>
        private IEnumerator FadeOutAndStart()
        {
            if (_canvasGroup == null) yield break;

            float startAlpha = _canvasGroup.alpha;
            float elapsed    = 0f;

            while (elapsed < _fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / _fadeOutDuration);
                yield return null;
            }
            _canvasGroup.alpha = 0f;
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
        /// Only called after _inputReady — blink starts when panel is fully visible.
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
