/*
====================================================================
* SpellUnlockSystem - Manages scroll collection and Zelda-style pause panel
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-18
* Version: 1.0
*
* AUTHORSHIP CLASSIFICATION:
*
* [AI-ASSISTED]
* - Subscribes to GameEvents.OnScrollCollected
* - Pauses game (Time.timeScale = 0) and shows unlock panel
* - Waits for any key press using WaitForSecondsRealtime (works at timeScale 0)
* - Resumes game after player dismisses panel
*
* DEPENDENCIES:
* - GameEvents.cs (OnScrollCollected, TuneUnlocked, GamePaused)
* - TMPro (TextMeshProUGUI)
* - UnityEngine.InputSystem (Keyboard.current.anyKey)
*
* SETUP:
* - Attach to a dedicated GameObject in the scene (e.g. "SpellUnlockManager")
* - Assign _scrollPanel (the UI root, disabled by default in Inspector)
* - Assign _scrollNameLabel, _scrollDescriptionLabel, _scrollKeyLabel (TMPro Text)
*
* NOTES:
* - WaitForSecondsRealtime MUST be used instead of WaitForSeconds when
*   Time.timeScale = 0, otherwise coroutine never resumes
*
* VERSION HISTORY:
* - v1.0: Initial implementation (Phase 7 spell system)
====================================================================
*/

using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using SnakeEnchanter.Core;

namespace SnakeEnchanter.Tunes
{
    /// <summary>
    /// Listens for scroll collection events and shows a Zelda-style pause panel
    /// that the player dismisses by pressing any key.
    /// </summary>
    public class SpellUnlockSystem : MonoBehaviour
    {
        #region Serialized Fields
        [Header("UI References")]
        [Tooltip("Root panel GameObject to show/hide on scroll collection. Must be disabled by default.")]
        [SerializeField] private GameObject _scrollPanel;

        [Tooltip("Label that shows the scroll's display name")]
        [SerializeField] private TextMeshProUGUI _scrollNameLabel;

        [Tooltip("Label that shows the scroll's gameplay description")]
        [SerializeField] private TextMeshProUGUI _scrollDescriptionLabel;

        [Tooltip("Label that shows which key casts the unlocked spell")]
        [SerializeField] private TextMeshProUGUI _scrollKeyLabel;
        #endregion

        #region Unity Lifecycle
        private void OnEnable()
        {
            GameEvents.OnScrollCollected += OnScrollCollected;
        }

        private void OnDisable()
        {
            GameEvents.OnScrollCollected -= OnScrollCollected;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Called when any SpellScrollPickup fires GameEvents.ScrollCollected.
        /// Fires TuneUnlocked, shows the pause panel, and starts the any-key dismiss coroutine.
        /// </summary>
        private void OnScrollCollected(int tuneNumber, string scrollName, string description)
        {
            // Notify other systems (HUD, TuneController) that a tune is now available
            GameEvents.TuneUnlocked(tuneNumber);

            // Populate panel labels
            if (_scrollNameLabel != null)
                _scrollNameLabel.text = scrollName;

            if (_scrollDescriptionLabel != null)
                _scrollDescriptionLabel.text = description;

            if (_scrollKeyLabel != null)
                _scrollKeyLabel.text = $"Press [{tuneNumber}] to cast";

            // Show panel
            if (_scrollPanel != null)
                _scrollPanel.SetActive(true);
            else
                Debug.LogWarning("[SpellUnlockSystem] _scrollPanel is not assigned.", this);

            // Pause game
            Time.timeScale = 0f;
            GameEvents.GamePaused(true);

            // Wait for player to dismiss
            StartCoroutine(WaitForAnyKey());
        }
        #endregion

        #region Coroutines
        /// <summary>
        /// Waits a brief buffer then waits for any key press before resuming the game.
        /// Uses WaitForSecondsRealtime because Time.timeScale is 0 during the pause.
        /// </summary>
        private IEnumerator WaitForAnyKey()
        {
            // Brief buffer to prevent the key that triggered collection from instantly dismissing the panel
            yield return new WaitForSecondsRealtime(0.2f);

            // Wait until any keyboard key is pressed
            yield return new WaitUntil(() => Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame);

            // Hide panel
            if (_scrollPanel != null)
                _scrollPanel.SetActive(false);

            // Resume game
            Time.timeScale = 1f;
            GameEvents.GamePaused(false);
        }
        #endregion
    }
}
