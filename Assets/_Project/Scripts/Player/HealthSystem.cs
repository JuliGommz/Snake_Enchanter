/*
====================================================================
* HealthSystem - Manages player health with drain, damage, and healing
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-05
* Version: 1.3 - DEATH ANIMATION INTEGRATION
*
* ⚠️ WICHTIG: KOMMENTIERUNG NICHT LÖSCHEN! ⚠️
* Diese detaillierte Authorship-Dokumentation ist für die akademische
* Bewertung erforderlich und darf nicht entfernt werden!
*
* AUTHORSHIP CLASSIFICATION:
* [AI-ASSISTED]
* - Health system architecture
* - Passive drain implementation
* - Event-driven design pattern
* - Single Source of Truth for drain rates (v1.2)
* - Human reviewed and will modify as needed
*
* DEPENDENCIES:
* - GameEvents.cs (SnakeEnchanter.Core)
* - Unity MonoBehaviour
*
* DESIGN RATIONALE:
* - Continuous HP (0-100) instead of discrete hearts for smooth feedback
* - Passive drain creates constant pressure (GDD v1.4)
* - Event-driven communication prevents tight coupling
* - Mode-specific drain rates stored HERE (not duplicated elsewhere)
* - All values configurable for balancing Phase 2+
*
* NOTES:
* - Phase 1 implementation - core functionality
* - Drain rates: Simple 0.1 HP/sec, Advanced 0.115 HP/sec (GDD Section 10)
*
* VERSION HISTORY:
* - v1.0: Initial implementation
* - v1.1: Changed "Bite" terminology to "Attack" for consistency
* - v1.2: Refactored to Single Source of Truth pattern
* - v1.2.1: Fixed namespace references + Unity 2023 API
* - v1.3: Death animation integration (IsDead bool, death cause tracking)
====================================================================
*/

using UnityEngine;
using SnakeEnchanter.Core;

namespace SnakeEnchanter.Player
{
    /// <summary>
    /// Manages player health including passive drain, damage, healing, and death.
    /// Integrates with GameEvents for decoupled communication.
    /// Single source of truth for mode-specific drain rates.
    /// </summary>
    public class HealthSystem : MonoBehaviour
    {
        #region Health Configuration
        [Header("Health Pool")]
        [Tooltip("Maximum health (GDD: 100)")]
        [SerializeField] private int _maxHealth = 100;

        [Tooltip("Starting health (GDD: 30 - wounded warrior)")]
        [SerializeField] private int _startingHealth = 30;

        [Header("Passive Drain - Mode Settings (GDD Section 10)")]
        [Tooltip("Simple Mode drain rate (0.1 HP/sec = 5 min survival from 30 HP)")]
        [SerializeField] private float _simpleDrainRate = 0.1f;

        [Tooltip("Advanced Mode drain rate (15% faster than Simple)")]
        [SerializeField] private float _advancedDrainRate = 0.115f;

        [Tooltip("Enable passive drain (disable for testing/development)")]
        [SerializeField] private bool _enablePassiveDrain = false;

        [Header("Balancing Values - Phase 2 Tuning")]
        [Tooltip("HP restored per successful tune cast")]
        [SerializeField] private int _healPerTuneSuccess = 15;

        [Tooltip("Damage from snake attack")]
        [SerializeField] private int _snakeAttackDamage = 20;
        #endregion

        #region Private Fields
        private float _currentHealth;
        private float _activeDrainRate;
        private bool _isDead = false;
        private int _lastReportedHealth = -1;
        private Animator _animator;
        #endregion

        #region Properties
        /// <summary>Current health value (0-100 range).</summary>
        public float CurrentHealth => _currentHealth;

        /// <summary>Maximum health cap.</summary>
        public int MaxHealth => _maxHealth;

        /// <summary>Health as normalized percentage (0.0 - 1.0).</summary>
        public float HealthPercentage => _currentHealth / _maxHealth;

        /// <summary>Is the player dead (HP <= 0)?</summary>
        public bool IsDead => _isDead;

        /// <summary>Simple Mode drain rate (read-only).</summary>
        public float SimpleDrainRate => _simpleDrainRate;

        /// <summary>Advanced Mode drain rate (read-only).</summary>
        public float AdvancedDrainRate => _advancedDrainRate;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _currentHealth = _startingHealth;
            _activeDrainRate = _simpleDrainRate; // Default to Simple Mode

            // Get Animator component (looks in children for Pirate model)
            _animator = GetComponentInChildren<Animator>();
            if (_animator == null)
            {
                Debug.LogWarning("HealthSystem: No Animator found! Death animations will not play.");
            }
        }

        private void Start()
        {
            GameEvents.HealthChanged(Mathf.RoundToInt(_currentHealth));
        }

        private void OnEnable()
        {
            GameEvents.OnTuneSuccess += OnTuneSuccessHealing;
        }

        private void OnDisable()
        {
            GameEvents.OnTuneSuccess -= OnTuneSuccessHealing;
        }

        private void Update()
        {
            if (_isDead) return;

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
            float drainAmount = _activeDrainRate * Time.deltaTime;
            _currentHealth -= drainAmount;
            _currentHealth = Mathf.Max(_currentHealth, 0f);

            // Only notify when rounded value changes (prevents event flood)
            int rounded = Mathf.RoundToInt(_currentHealth);
            if (rounded != _lastReportedHealth)
            {
                _lastReportedHealth = rounded;
                GameEvents.HealthChanged(rounded);
            }

            if (_currentHealth <= 0f && !_isDead)
            {
                Die(deathBySnakeAttack: false); // Death by drain
            }
        }
        #endregion

        #region Damage
        /// <summary>Applies damage to the player (e.g., snake attack).</summary>
        public void TakeDamage(int amount)
        {
            if (_isDead) return;

            if (amount < 0)
            {
                Debug.LogWarning($"HealthSystem: TakeDamage called with negative value ({amount}). Ignoring.");
                return;
            }

            _currentHealth -= amount;
            _currentHealth = Mathf.Max(_currentHealth, 0f);

            GameEvents.PlayerDamaged(amount);
            GameEvents.HealthChanged(Mathf.RoundToInt(_currentHealth));

            if (_currentHealth <= 0f)
            {
                Die(deathBySnakeAttack: true); // Death by snake attack
            }
        }

        /// <summary>Convenience method for snake attack damage.</summary>
        public void TakeSnakeAttack()
        {
            TakeDamage(_snakeAttackDamage);
        }
        #endregion

        #region Healing
        /// <summary>Restores health to the player (e.g., successful tune cast).</summary>
        public void Heal(int amount)
        {
            if (_isDead) return;

            if (amount < 0)
            {
                Debug.LogWarning($"HealthSystem: Heal called with negative value ({amount}). Ignoring.");
                return;
            }

            _currentHealth += amount;
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);

            GameEvents.PlayerHealed(amount);
            GameEvents.HealthChanged(Mathf.RoundToInt(_currentHealth));
        }

        /// <summary>Event handler for successful tune casts.</summary>
        private void OnTuneSuccessHealing()
        {
            Heal(_healPerTuneSuccess);
        }
        #endregion

        #region Death
        /// <summary>Handles player death (HP <= 0).</summary>
        /// <param name="deathBySnakeAttack">True if killed by snake, false if by drain</param>
        private void Die(bool deathBySnakeAttack)
        {
            if (_isDead) return;

            _isDead = true;
            _currentHealth = 0f;

            // Play appropriate death animation directly
            if (_animator != null)
            {
                string stateName = deathBySnakeAttack ? "Death_by_Snakes" : "Death_by_Drain";
                _animator.Play(stateName);

                string deathCause = deathBySnakeAttack ? "by Snake Attack" : "by Drain";
                Debug.Log($"HealthSystem: Player died {deathCause}. Playing '{stateName}' animation.");
            }

            GameEvents.HealthChanged(0);
            GameEvents.GameOver();

            Debug.Log("HealthSystem: Player died. Game Over triggered.");
        }
        #endregion

        #region Public Utilities
        /// <summary>Resets health to starting value (for game restart).</summary>
        public void ResetHealth()
        {
            _isDead = false;
            _currentHealth = _startingHealth;
            GameEvents.HealthChanged(Mathf.RoundToInt(_currentHealth));
        }

        /// <summary>
        /// Applies drain rate for specified game mode.
        /// GameManager calls this with the appropriate mode.
        /// </summary>
        public void ApplyModeSettings(Core.GameMode mode)
        {
            switch (mode)
            {
                case Core.GameMode.Simple:
                    _activeDrainRate = _simpleDrainRate;
                    break;
                case Core.GameMode.Advanced:
                    _activeDrainRate = _advancedDrainRate;
                    break;
            }

            Debug.Log($"HealthSystem: Drain rate set to {_activeDrainRate}/sec ({mode} Mode)");
        }

        /// <summary>Enables or disables passive drain (for testing/cutscenes).</summary>
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
            GUILayout.BeginArea(new Rect(10, 10, 300, 170));

            GUILayout.Label($"<b>HealthSystem Debug</b>", new GUIStyle(GUI.skin.label) { richText = true });
            GUILayout.Label($"Current HP: {_currentHealth:F1} / {_maxHealth}");
            GUILayout.Label($"HP %: {HealthPercentage:P1}");
            GUILayout.Label($"Active Drain: {_activeDrainRate}/sec");
            GUILayout.Label($"Dead: {_isDead}");
            GUILayout.Space(5);

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
