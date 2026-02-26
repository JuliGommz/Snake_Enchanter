/*
====================================================================
* ShieldComponent - Player shield state machine with activate, timer, absorb, deactivate
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-18
* Version: v1.0
*
* AUTHORSHIP CLASSIFICATION:
* [AI-ASSISTED]
* - Shield state machine architecture
* - Coroutine-based timer with WaitForSeconds (pauses with game)
* - Event-driven communication via GameEvents static hub
* - Human reviewed
*
* DEPENDENCIES:
* - GameEvents.cs (SnakeEnchanter.Core)
* - Unity UI (UnityEngine.UI.Image for border glow)
* - Unity MonoBehaviour
*
* DESIGN RATIONALE:
* - Shield blocks exactly one snake attack (absorb-then-deactivate)
* - 8-second timer expires shield if no attack occurs
* - Screen edge glow provides passive visual feedback during shield active
* - White flash on absorb confirms the block to the player
* - No recast while active (user decision: prevents spam)
* - ShieldComponent is a passive component — ActivateShield() called by TuneController (07-04)
*
* BORDER GLOW SPRITE REQUIREMENT:
* Requires a UI Image with a border/vignette sprite (transparent center, colored edges)
* on a Screen Space - Overlay Canvas. Assign the Image reference in the Inspector.
* The Image's GameObject will be shown/hidden by this script.
*
* VERSION HISTORY:
* - v1.0: Initial implementation — full shield lifecycle
====================================================================
*/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SnakeEnchanter.Core;

namespace SnakeEnchanter.Player
{
    /// <summary>
    /// Manages the player's shield spell lifecycle: activate, 8-second timer, absorb attack, and expire.
    /// Provides screen-edge glow feedback via a UI Image (border/vignette sprite).
    /// Activated by TuneController when Tune 3 (Shield) is cast successfully.
    /// </summary>
    public class ShieldComponent : MonoBehaviour
    {
        #region Configuration
        [Header("Shield Settings")]
        [Tooltip("Fallback duration if no TuneConfig provides one (overridden by melody section length)")]
        [SerializeField] private float _shieldDuration = 8f;

        [Header("Screen Edge Glow")]
        [Tooltip("Full-screen UI Image with a border/vignette sprite (transparent center, colored edges). Assign in Inspector.")]
        [SerializeField] private Image _borderGlowImage;

        [Tooltip("Color of the screen edge glow while shield is active")]
        [SerializeField] private Color _shieldActiveColor = new Color(0.3f, 0.6f, 1f, 0.5f);

        [Tooltip("Flash color on shield absorb (briefly shows before hiding)")]
        [SerializeField] private Color _shieldAbsorbFlashColor = Color.white;
        #endregion

        #region Private Fields
        private bool _isShieldActive = false;
        private Coroutine _shieldTimerCoroutine = null;
        #endregion

        #region Properties
        /// <summary>True while shield is active and able to absorb the next attack.</summary>
        public bool IsShieldActive => _isShieldActive;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Shield glow hidden by default — only visible while shield is active
            if (_borderGlowImage != null)
            {
                _borderGlowImage.gameObject.SetActive(false);
            }
        }
        #endregion

        #region Public API
        /// <summary>
        /// Activates the shield. Guard: does nothing if shield is already active (no recast).
        /// Called by TuneController when Tune 3 (Shield) is cast successfully.
        /// </summary>
        /// <param name="duration">Shield duration in seconds. If 0 or negative, uses fallback _shieldDuration.</param>
        public void ActivateShield(float duration = 0f)
        {
            // Guard: cannot recast while shield is already active
            if (_isShieldActive) return;

            // Use provided duration (from TuneConfig melody section) or fallback
            if (duration > 0f)
            {
                _shieldDuration = duration;
            }

            _isShieldActive = true;

            // Fire event — UI and audio subscribe to this
            GameEvents.ShieldActivated();

            // Show screen edge glow
            if (_borderGlowImage != null)
            {
                _borderGlowImage.color = _shieldActiveColor;
                _borderGlowImage.gameObject.SetActive(true);
            }

            // Start expiry timer
            _shieldTimerCoroutine = StartCoroutine(ShieldTimerCoroutine());
        }

        /// <summary>
        /// Called by HealthSystem.TakeSnakeAttack() before applying damage.
        /// Returns true if attack was absorbed (damage blocked), false if no shield active.
        /// </summary>
        public bool TryAbsorbAttack()
        {
            // No shield — damage passes through
            if (!_isShieldActive) return false;

            // Shield absorbs the attack — deactivate it
            DeactivateShield(absorbed: true);

            // Fire absorb event (screen flash + shatter feedback ready for subscribers)
            GameEvents.ShieldAbsorbedAttack();

            // Play white flash then hide glow
            StartCoroutine(AbsorbFlashCoroutine());

            return true; // Attack absorbed
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Waits for the shield duration then expires the shield naturally.
        /// Uses WaitForSeconds (correct — shield timer pauses with the game when timeScale = 0).
        /// </summary>
        private IEnumerator ShieldTimerCoroutine()
        {
            yield return new WaitForSeconds(_shieldDuration);

            // Only expire if still active (not already absorbed by an attack)
            if (_isShieldActive)
            {
                DeactivateShield(absorbed: false);
            }
        }

        /// <summary>
        /// Deactivates the shield and fires the corresponding event.
        /// Stops the timer coroutine to prevent double-deactivation.
        /// </summary>
        /// <param name="absorbed">True if shield broke by absorbing an attack, false if it expired naturally.</param>
        private void DeactivateShield(bool absorbed)
        {
            _isShieldActive = false;

            // Stop the timer coroutine (prevents double-expiry when absorbed)
            if (_shieldTimerCoroutine != null)
            {
                StopCoroutine(_shieldTimerCoroutine);
                _shieldTimerCoroutine = null;
            }

            // Fire event — subscribers know whether it absorbed or expired
            GameEvents.ShieldDeactivated(absorbed);

            // Hide glow on natural expiry (absorbed case handled by AbsorbFlashCoroutine)
            if (!absorbed && _borderGlowImage != null)
            {
                _borderGlowImage.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Brief white flash confirming the shield absorbed the attack, then hides the glow.
        /// </summary>
        private IEnumerator AbsorbFlashCoroutine()
        {
            if (_borderGlowImage != null)
            {
                // Flash white to signal absorption
                _borderGlowImage.color = _shieldAbsorbFlashColor;
                yield return new WaitForSeconds(0.15f);

                // Hide glow after flash
                _borderGlowImage.gameObject.SetActive(false);
            }
        }
        #endregion
    }
}
