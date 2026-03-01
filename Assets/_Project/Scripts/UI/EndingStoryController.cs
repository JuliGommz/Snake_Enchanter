/*
====================================================================
* EndingStoryController - Ending story panel shown on Win before Result Screen
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-03-01
* Version: v1.0
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
* - CanvasGroup component on EndingStoryPanel
*
* UI HIERARCHY REQUIRED (GameLevel Scene):
*   Canvas
*     └── EndingStoryPanel         (Image — full-screen dark bg + CanvasGroup component!)
*           ├── EndingText         (TextMeshProUGUI — ending story body)
*           └── ContinueHint       (TextMeshProUGUI — "Drücke eine Taste")
*
* SETUP:
* 1. Create EndingStoryPanel with a CanvasGroup component attached
* 2. Attach this script to a GameObject in the GameLevel scene
* 3. Assign all references in Inspector
* 4. ResultScreenController._waitForEndingStory must be true
*
* FLOW:
* OnGameWin → FadeIn panel → player reads → any key → FadeOut → ResultScreen.ShowWin()
*
* VERSION HISTORY:
* - v1.0: Initial — fade-in, any-key dismiss, fade-out, handoff to ResultScreenController
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
        [Tooltip("Root panel with CanvasGroup component — REQUIRED for fade effect.")]
        [SerializeField] private CanvasGroup _endingPanelGroup;

        [Tooltip("ResultScreenController to call ShowWin() on after dismiss.")]
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
        private bool _inputReady   = false;
        private bool _dismissed    = false;
        private bool _isActive     = false;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Panel starts invisible and hidden
            if (_endingPanelGroup != null)
            {
                _endingPanelGroup.alpha          = 0f;
                _endingPanelGroup.gameObject.SetActive(false);
            }
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
            if (_endingPanelGroup == null) yield break;

            _endingPanelGroup.gameObject.SetActive(true);
            _endingPanelGroup.alpha = 0f;

            float elapsed = 0f;
            while (elapsed < _fadeInDuration)
            {
                elapsed += Time.deltaTime;
                _endingPanelGroup.alpha = Mathf.Clamp01(elapsed / _fadeInDuration);
                yield return null;
            }
            _endingPanelGroup.alpha = 1f;

            // Brief cooldown so the last keypress from gameplay doesn't skip immediately
            yield return new WaitForSeconds(_inputCooldown);
            _inputReady = true;
        }

        /// <summary>Fades the ending panel out, then shows the result screen.</summary>
        private IEnumerator FadeOutAndShowResult()
        {
            if (_endingPanelGroup == null) yield break;

            float startAlpha = _endingPanelGroup.alpha;
            float elapsed    = 0f;

            while (elapsed < _fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                _endingPanelGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / _fadeOutDuration);
                yield return null;
            }
            _endingPanelGroup.alpha = 0f;
            _endingPanelGroup.gameObject.SetActive(false);

            // Hand off to ResultScreenController
            if (_resultScreen != null)
                _resultScreen.ShowWin();
            else
                Debug.LogError("EndingStoryController: ResultScreenController reference is missing!");
        }
        #endregion
    }
}
