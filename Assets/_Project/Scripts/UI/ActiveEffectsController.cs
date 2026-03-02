/*
====================================================================
* ActiveEffectsController - Shows active spell effects in ActiveEffectsWindow
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-03-01
* Version: v1.1
*
* AUTHORSHIP CLASSIFICATION:
* [AI-ASSISTED]
* - OnSnakeCharmed subscription + timer-based show/hide pattern
* - Coroutine per effect with Stop+Restart on recast
* - Human reviewed
*
* DEPENDENCIES:
* - GameEvents.cs (SnakeEnchanter.Core) — OnSnakeCharmed
* - TMPro (TextMeshProUGUI) — MoveText, DazeText in ActiveEffectsWindow
*
* UI HIERARCHY REQUIRED (GameLevel Scene):
*   GameCanvas
*     └── ActiveEffectsWindow     (existing panel — already has ShieldText)
*           ├── MoveText           (TextMeshProUGUI — assign in Inspector)
*           └── DazeText           (TextMeshProUGUI — assign in Inspector)
*
* SETUP:
* 1. Attach this script to any persistent GameObject in GameLevel (e.g. GameCanvas)
* 2. Assign _moveText → MoveText TMP in ActiveEffectsWindow
* 3. Assign _dazeText → DazeText TMP in ActiveEffectsWindow
* 4. Adjust durations to match SnakeAI state durations if needed
*
* NOTE: ShieldText is handled directly by ShieldComponent — not here.
*
* VERSION HISTORY:
* - v1.0: Initial — Move + Daze text shown/hidden via OnSnakeCharmed
* - v1.1: D4 — unused Coroutine parameter documented with comment
====================================================================
*/

using System.Collections;
using UnityEngine;
using TMPro;
using SnakeEnchanter.Core;

namespace SnakeEnchanter.UI
{
    /// <summary>
    /// Listens for OnSnakeCharmed events and shows Move/Daze effect labels
    /// in the ActiveEffectsWindow for the duration of each spell effect.
    /// Shield is handled by ShieldComponent directly — not managed here.
    /// </summary>
    public class ActiveEffectsController : MonoBehaviour
    {
        #region Configuration
        [Header("Effect Text References")]
        [Tooltip("TextMeshPro label for Move effect — shown when Tune 1 (Move) is successfully cast.")]
        [SerializeField] private TextMeshProUGUI _moveText;

        [Tooltip("TextMeshPro label for Daze effect — shown when Tune 2 (Daze) is successfully cast.")]
        [SerializeField] private TextMeshProUGUI _dazeText;

        [Header("Effect Durations")]
        [Tooltip("Seconds the MOVE label stays visible. " +
                 "Should match snake Entranced state duration in SnakeAI (default: 3s).")]
        [SerializeField] private float _moveDuration = 3f;

        [Tooltip("Seconds the DAZE label stays visible. " +
                 "Should match Entranced (3s) + Dazed (8s) state total in SnakeAI (default: 11s).")]
        [SerializeField] private float _dazeDuration = 11f;
        #endregion

        #region Private Fields
        // Track active coroutines to stop them on recast (prevents stacking timers)
        private Coroutine _moveCoroutine;
        private Coroutine _dazeCoroutine;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Both effect labels hidden by default — only visible while effect is active
            if (_moveText != null) _moveText.gameObject.SetActive(false);
            if (_dazeText != null) _dazeText.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            GameEvents.OnSnakeCharmed += OnSnakeCharmed;
        }

        private void OnDisable()
        {
            GameEvents.OnSnakeCharmed -= OnSnakeCharmed;
        }
        #endregion

        #region Event Handler
        /// <summary>
        /// Called when a snake is successfully charmed.
        /// Tune 1 = Move, Tune 2 = Daze. Tune 3 (Shield) is handled by ShieldComponent.
        /// </summary>
        private void OnSnakeCharmed(int tuneNumber)
        {
            switch (tuneNumber)
            {
                case 1:
                    ShowEffect(ref _moveCoroutine, _moveText, _moveDuration);
                    break;
                case 2:
                    ShowEffect(ref _dazeCoroutine, _dazeText, _dazeDuration);
                    break;
                // Tune 3 (Shield) intentionally not handled here — ShieldComponent owns ShieldText
            }
        }
        #endregion

        #region Effect Display
        /// <summary>
        /// Shows a text label for the given duration, then hides it.
        /// If already running (recast), stops the old timer and starts fresh.
        /// </summary>
        private void ShowEffect(ref Coroutine coroutine, TextMeshProUGUI label, float duration)
        {
            if (label == null) return;

            // Recast: stop existing timer, start fresh
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            label.gameObject.SetActive(true);
            coroutine = StartCoroutine(HideAfterDuration(coroutine, label, duration));
        }

        /// <summary>Waits for the effect duration, then hides the label.</summary>
        private IEnumerator HideAfterDuration(Coroutine _ /* unused, kept for call-site clarity */, TextMeshProUGUI label, float duration)
        {
            yield return new WaitForSeconds(duration);
            if (label != null)
                label.gameObject.SetActive(false);
        }
        #endregion
    }
}
