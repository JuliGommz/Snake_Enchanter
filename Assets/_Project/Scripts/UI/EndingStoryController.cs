/*
====================================================================
* EndingStoryController - Ending story panel shown on Win before Result Screen
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-03-01
* Version: v1.1
*
* AUTHORSHIP CLASSIFICATION:
* [AI-ASSISTED]
* - CanvasGroup fade-in/out coroutine pattern
* - GameEvents subscription for Win trigger
* - Input cooldown to prevent accidental skip
* - Human reviewed
*
* DEPENDENCIES:
* - GameEvents.cs (SnakeEnchanter.Core)
* - ResultScreenController.cs (SnakeEnchanter.UI) — ShowWin()
* - UnityEngine.InputSystem
* - CanvasGroup component on EndingStoryPanel (added automatically if missing)
*
* UI HIERARCHY REQUIRED (GameLevel Scene):
*   GameCanvas
*     └── EndingStoryPanel     (Image — full-screen dark bg, CanvasGroup auto-added)
*           ├── EndingText     (TextMeshProUGUI — ending story body)
*           └── ContinueHint  (TextMeshProUGUI — "Press any key")
*
* INSPECTOR SETUP:
* - Ending Panel  → drag EndingStoryPanel from Hierarchy
* - Result Screen → drag GameCanvas from Hierarchy (ResultScreenController lives there)
*
* FLOW:
* OnGameWin → FadeIn panel → player reads → any key → FadeOut → ResultScreen.ShowWin()
*
* VERSION HISTORY:
* - v1.0: Initial — fade-in, any-key dismiss, fade-out, handoff to ResultScreenController
* - v1.1: _endingPanel is now GameObject (consistent with _introPanel in StoryIntroController)
*         CanvasGroup fetched internally via GetComponent — no manual component assignment needed
====================================================================
*/

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using SnakeEnchanter.Core;

namespace SnakeEnchanter.UI
{
    /// <summary>
    /// Shows an ending story panel on Win with a fade-in effect.
    /// After player dismisses it, hands off to ResultScreenController.ShowWin().
    /// </summary>
    public class EndingStoryController : MonoBehaviour
    {
        #region Configuration
        [Header("UI References")]
        [Tooltip("Drag EndingStoryPanel from the Hierarchy here.")]
        [SerializeField] private GameObject _endingPanel;

        [Tooltip("Drag GameCanvas here — ResultScreenController is attached to it.")]
        [SerializeField] private ResultScreenController _resultScreen;

        [Header("Fade Settings")]
        [Tooltip("Seconds to fade the panel in.")]
        [SerializeField] private float _fadeInDuration  = 1.2f;

        [Tooltip("Seconds to fade the panel out before showing result screen.")]
        [SerializeField] private float _fadeOutDuration = 0.6f;

        [Header("Input Cooldown")]
        [Tooltip("Seconds after fade-in before key input is accepted. Prevents accidental skip.")]
        [SerializeField] private float _inputCooldown = 0.8f;
        #endregion

        #region Private Fields
        private CanvasGroup _canvasGroup;
        private bool _inputReady = false;
        private bool _dismissed  = false;
        private bool _isActive   = false;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (_endingPanel == null) return;

            // Get or add CanvasGroup — no manual component setup needed in Inspector
            _canvasGroup = _endingPanel.GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = _endingPanel.AddComponent<CanvasGroup>();

            _canvasGroup.alpha = 0f;
            _endingPanel.SetActive(false);
        }

        private void OnEnable()
        {
            GameEvents.OnGameWin += HandleWin;
        }

        private void OnDisable()
        {
            GameEvents.OnGameWin -= HandleWin;
        }

        private void Update()
        {
            if (!_isActive || !_inputReady || _dismissed) return;

            bool keyPressed   = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
            bool mouseClicked = Mouse.current    != null && Mouse.current.leftButton.wasPressedThisFrame;

            if (keyPressed || mouseClicked)
            {
                _dismissed = true;
                StartCoroutine(FadeOutAndShowResult());
            }
        }
        #endregion

        #region Win Flow
        private void HandleWin()
        {
            _isActive = true;
            StartCoroutine(FadeIn());
        }
        #endregion

        #region Fade Coroutines
        /// <summary>Fades the ending panel in, then enables key input after cooldown.</summary>
        private IEnumerator FadeIn()
        {
            if (_canvasGroup == null) yield break;

            _endingPanel.SetActive(true);
            _canvasGroup.alpha = 0f;

            float elapsed = 0f;
            while (elapsed < _fadeInDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Clamp01(elapsed / _fadeInDuration);
                yield return null;
            }
            _canvasGroup.alpha = 1f;

            // Brief cooldown so the last keypress from gameplay doesn't skip immediately
            yield return new WaitForSeconds(_inputCooldown);
            _inputReady = true;
        }

        /// <summary>Fades the ending panel out, then shows the result screen.</summary>
        private IEnumerator FadeOutAndShowResult()
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
            _endingPanel.SetActive(false);

            // Hand off to ResultScreenController
            if (_resultScreen != null)
                _resultScreen.ShowWin();
            else
                Debug.LogError("EndingStoryController: ResultScreenController reference is missing!");
        }
        #endregion
    }
}
