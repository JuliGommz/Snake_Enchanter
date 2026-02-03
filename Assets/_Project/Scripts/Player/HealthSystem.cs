/*
====================================================================
* HealthSystem - Manages player health with drain, damage, and healing
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-03
* Version: 1.1 - Terminology Update (Bite → Attack)

* ⚠️ WICHTIG: KOMMENTIERUNG NICHT LÖSCHEN! ⚠️
* Diese detaillierte Authorship-Dokumentation ist für die akademische
* Bewertung erforderlich und darf nicht entfernt werden!

* AUTHORSHIP CLASSIFICATION:

* [AI-ASSISTED]
* - Health system architecture
* - Passive drain implementation
* - Event-driven design pattern
* - Human reviewed and will modify as needed

* DEPENDENCIES:
* - GameEvents.cs (SnakeEnchanter.Core)
* - Unity MonoBehaviour

* DESIGN RATIONALE:
* - Continuous HP (0-100) instead of discrete hearts for smooth feedback
* - Passive drain creates constant pressure (matches GDD v1.3)
* - Event-driven communication prevents tight coupling
* - All values configurable for balancing Phase 2+

* NOTES:
* - Phase 1 implementation - core functionality
* - Balancing values are placeholders (see GDD Section 10)
* - Drain rate calibrated for ~6min survival (Simple Mode)

* VERSION HISTORY:
* - v1.0: Initial implementation
* - v1.1: Changed "Bite" terminology to "Attack" for consistency
====================================================================
*/

using UnityEngine;
using SnakeEnchanter.Core;

namespace SnakeEnchanter.Player
{
    /// <summary>
    /// Manages player health including passive drain, damage, healing, and death.
    /// Integrates with GameEvents for decoupled communication.
    /// </summary>
    public class HealthSystem : MonoBehaviour
    {
        #region Health Configuration
        [Header("Health Pool")]
        [Tooltip("Maximum health (GDD: 100)")]
        [SerializeField] private int _maxHealth = 100;

        [Tooltip("Starting health (GDD: 30 - wounded warrior)")]
        [SerializeField] private int _startingHealth = 30;

        [Header("Passive Drain")]
        [Tooltip("HP lost per second (Simple Mode baseline)")]
        [SerializeField] private float _drainRatePerSecond = 2.5f;

        [Tooltip("Enable passive drain (disable for testing)")]
        [SerializeField] private bool _enablePassiveDrain = true;

        [Header("Balancing Values - Phase 2 Tuning")]
        [Tooltip("HP restored per successful tune cast")]
        [SerializeField] private int _healPerTuneSuccess = 15;

        [Tooltip("Damage from snake attack")]
        [SerializeField] private int _snakeAttackDamage = 20;
        #endregion

        #region Private Fields
        private float _currentHealth;
        private bool _isDead = false;
        #endregion

        #region Properties
        /// <summary>
        /// Current health value (0-100 range).
        /// </summary>
        public float CurrentHealth => _currentHealth;

        /// <summary>
        /// Maximum health cap.
        /// </summary>
        public int MaxHealth => _maxHealth;

        /// <summary>
        /// Health as normalized percentage (0.0 - 1.0).
        /// </summary>
        public float HealthPercentage => _currentHealth / _maxHealth;

        /// <summary>
        /// Is the player dead (HP <= 0)?
        /// </summary>
        public bool IsDead => _isDead;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Initialize health
            _currentHealth = _startingHealth;
        }

        private void Start()
        {
            // Notify systems of initial health
            GameEvents.HealthChanged(Mathf.RoundToInt(_currentHealth));
        }

        private void OnEnable()
        {
            // Subscribe to tune success for healing
            GameEvents.OnTuneSuccess += OnTuneSuccessHealing;
        }

        private void OnDisable()
        {
            // Unsubscribe to prevent memory leaks
            GameEvents.OnTuneSuccess -= OnTuneSuccessHealing;
        }

        private void Update()
        {
            if (_isDead) return;

            // Apply passive health drain
            if (_enablePassiveDrain)
            {
                ApplyPassiveDrain();
            }
        }
        #endregion

        #region Passive Drain
        /// <summary>
        /// Applies continuous health drain over time (wounded warrior mechanic).
        /// GDD: Calibrated for ~6 minutes survival with competent play.
        /// </summary>
        private void ApplyPassiveDrain()
        {
            float drainAmount = _drainRatePerSecond * Time.deltaTime;
            _currentHealth -= drainAmount;

            // Clamp to prevent negative health
            _currentHealth = Mathf.Max(_currentHealth, 0f);

            // Notify UI of continuous change
            GameEvents.HealthChanged(Mathf.RoundToInt(_currentHealth));

            // Check death condition
            if (_currentHealth <= 0f && !_isDead)
            {
                Die();
            }
        }
        #endregion

        #region Damage
        /// <summary>
        /// Applies damage to the player (e.g., snake attack, timing failure).
        /// </summary>
        /// <param name="amount">Damage amount to subtract from health.</param>
        public void TakeDamage(int amount)
        {
            if (_isDead) return;

            // Validate input
            if (amount < 0)
            {
                Debug.LogWarning($"HealthSystem: TakeDamage called with negative value ({amount}). Ignoring.");
                return;
            }

            _currentHealth -= amount;
            _currentHealth = Mathf.Max(_currentHealth, 0f);

            // Notify systems
            GameEvents.PlayerDamaged(amount);
            GameEvents.HealthChanged(Mathf.RoundToInt(_currentHealth));

            // Check death
            if (_currentHealth <= 0f)
            {
                Die();
            }
        }

        /// <summary>
        /// Convenience method for snake attack damage.
        /// </summary>
        public void TakeSnakeAttack()
        {
            TakeDamage(_snakeAttackDamage);
        }
        #endregion

        #region Healing
        /// <summary>
        /// Restores health to the player (e.g., successful tune cast).
        /// </summary>
        /// <param name="amount">Heal amount to add to health.</param>
        public void Heal(int amount)
        {
            if (_isDead) return;

            // Validate input
            if (amount < 0)
            {
                Debug.LogWarning($"HealthSystem: Heal called with negative value ({amount}). Ignoring.");
                return;
            }

            _currentHealth += amount;

            // Cap at max health
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);

            // Notify systems
            GameEvents.PlayerHealed(amount);
            GameEvents.HealthChanged(Mathf.RoundToInt(_currentHealth));
        }

        /// <summary>
        /// Event handler for successful tune casts - restores health.
        /// GDD: "Each successful melody restores health"
        /// </summary>
        private void OnTuneSuccessHealing()
        {
            Heal(_healPerTuneSuccess);
        }
        #endregion

        #region Death
        /// <summary>
        /// Handles player death (HP <= 0).
        /// GDD: Immediate game over, lose screen, session data sent to backend.
        /// </summary>
        private void Die()
        {
            if (_isDead) return; // Prevent multiple death triggers

            _isDead = true;
            _currentHealth = 0f;

            // Notify all systems
            GameEvents.HealthChanged(0);
            GameEvents.GameOver();

            Debug.Log("HealthSystem: Player died. Game Over triggered.");
        }
        #endregion

        #region Public Utilities
        /// <summary>
        /// Resets health to starting value (for game restart).
        /// </summary>
        public void ResetHealth()
        {
            _isDead = false;
            _currentHealth = _startingHealth;
            GameEvents.HealthChanged(Mathf.RoundToInt(_currentHealth));
        }

        /// <summary>
        /// Sets drain rate (for mode switching: Simple vs Advanced).
        /// Advanced Mode = 15% faster than Simple.
        /// </summary>
        /// <param name="ratePerSecond">New drain rate per second.</param>
        public void SetDrainRate(float ratePerSecond)
        {
            _drainRatePerSecond = ratePerSecond;
            Debug.Log($"HealthSystem: Drain rate set to {ratePerSecond}/sec");
        }

        /// <summary>
        /// Enables or disables passive drain (for testing/cutscenes).
        /// </summary>
        public void SetPassiveDrainEnabled(bool enabled)
        {
            _enablePassiveDrain = enabled;
        }
        #endregion

        #region Debug Helpers
#if UNITY_EDITOR
        [Header("Debug - Editor Only")]
        [SerializeField] private bool _showDebugInfo = true;

        private void OnGUI()
        {
            if (!_showDebugInfo) return;

            GUI.color = Color.white;
            GUILayout.BeginArea(new Rect(10, 10, 300, 150));
            GUILayout.Label($"<b>HealthSystem Debug</b>", new GUIStyle(GUI.skin.label) { richText = true });
            GUILayout.Label($"Current HP: {_currentHealth:F1} / {_maxHealth}");
            GUILayout.Label($"HP %: {HealthPercentage:P1}");
            GUILayout.Label($"Drain Rate: {_drainRatePerSecond}/sec");
            GUILayout.Label($"Dead: {_isDead}");

            if (GUILayout.Button("Test Damage (20)"))
            {
                TakeDamage(20);
            }
            if (GUILayout.Button("Test Heal (15)"))
            {
                Heal(15);
            }
            GUILayout.EndArea();
        }
#endif
        #endregion
    }
}
