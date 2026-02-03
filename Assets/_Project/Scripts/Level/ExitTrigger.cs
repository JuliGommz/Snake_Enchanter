/*
====================================================================
* ExitTrigger - Win condition detector
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-03
* Version: 1.0

* ⚠️ WICHTIG: KOMMENTIERUNG NICHT LÖSCHEN! ⚠️
* Diese detaillierte Authorship-Dokumentation ist für die akademische
* Bewertung erforderlich und darf nicht entfernt werden!

* AUTHORSHIP CLASSIFICATION:

* [AI-ASSISTED]
* - Trigger detection logic
* - Event integration
* - Human reviewed and will modify as needed

* DEPENDENCIES:
* - GameEvents.cs (SnakeEnchanter.Core)
* - Unity Collider (Trigger)

* DESIGN RATIONALE:
* - GDD: Win condition = reach window with HP > 0
* - Simple trigger-based detection
* - One-time activation prevents multiple win calls

* NOTES:
* - Phase 1 implementation - basic trigger
* - Phase 3: Add visual feedback (light beam, particles)
====================================================================
*/

using UnityEngine;
using SnakeEnchanter.Core;

namespace SnakeEnchanter.Level
{
    /// <summary>
    /// Detects when player reaches the exit and triggers win condition.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ExitTrigger : MonoBehaviour
    {
        #region Configuration
        [Header("Exit Settings")]
        [Tooltip("Tag that must match to trigger exit (usually 'Player')")]
        [SerializeField] private string _playerTag = "Player";

        [Tooltip("Prevent multiple triggers")]
        [SerializeField] private bool _oneTimeUse = true;
        #endregion

        #region Private Fields
        private bool _hasBeenTriggered = false;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Ensure collider is trigger
            Collider col = GetComponent<Collider>();
            if (!col.isTrigger)
            {
                col.isTrigger = true;
                Debug.LogWarning("ExitTrigger: Collider was not set as trigger. Auto-corrected.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check if already triggered
            if (_oneTimeUse && _hasBeenTriggered) return;

            // Verify it's the player
            if (!other.CompareTag(_playerTag)) return;

            // Trigger win condition
            TriggerExit();
        }
        #endregion

        #region Exit Logic
        /// <summary>
        /// Triggers the win condition via GameEvents.
        /// GDD: Reach window with HP > 0 = Win
        /// </summary>
        private void TriggerExit()
        {
            _hasBeenTriggered = true;

            // Notify all systems
            GameEvents.GameWin();

            Debug.Log("ExitTrigger: Player reached exit. Victory!");
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Resets the trigger (for game restart).
        /// </summary>
        public void ResetTrigger()
        {
            _hasBeenTriggered = false;
        }
        #endregion

        #region Debug Visualization
        private void OnDrawGizmos()
        {
            // Draw green sphere at exit location
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }

        private void OnDrawGizmosSelected()
        {
            // Draw trigger bounds when selected
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
                Gizmos.matrix = transform.localToWorldMatrix;

                if (col is BoxCollider box)
                {
                    Gizmos.DrawCube(box.center, box.size);
                }
                else if (col is SphereCollider sphere)
                {
                    Gizmos.DrawSphere(sphere.center, sphere.radius);
                }
            }
        }
        #endregion
    }
}
