/*
====================================================================
* HealthBarUI - Minimal health bar display
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-04
* Version: 1.0

* ⚠️ WICHTIG: KOMMENTIERUNG NICHT LÖSCHEN! ⚠️
* Diese detaillierte Authorship-Dokumentation ist für die akademische
* Bewertung erforderlich und darf nicht entfernt werden!

* AUTHORSHIP CLASSIFICATION:

* [AI-ASSISTED]
* - UI Slider-based health bar
* - Color gradient system (GDD Section 6.2)
* - Event-driven update via GameEvents
* - Human reviewed and will modify as needed

* DEPENDENCIES:
* - GameEvents.cs (SnakeEnchanter.Core)
* - Unity UI (UnityEngine.UI)
* - Canvas with Slider element in scene

* DESIGN RATIONALE (GDD Section 6.2):
* - Location: Top-left
* - Gradient: Green (100-50%) → Yellow (50-25%) → Red (25-0%)
* - Numeric display: "Current / Maximum"
* - Smooth continuous depletion animation

* SETUP:
* 1. Create Canvas → UI → Slider
* 2. Remove "Handle Slide Area" child
* 3. Assign Fill Image and Text references
* 4. Position top-left (anchor top-left)

* NOTES:
* - Phase 1: Minimal — slider + color + text
* - Phase 3: Pulsing at low HP, vignette, damage flash

* VERSION HISTORY:
* - v1.0: Initial — slider, gradient, text
====================================================================
*/

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SnakeEnchanter.Core;

namespace SnakeEnchanter.UI
{
    /// <summary>
    /// Displays player health as a color-coded slider with numeric text.
    /// GDD: Top-left, Green→Yellow→Red gradient.
    /// </summary>
    public class HealthBarUI : MonoBehaviour
    {
        #region Configuration
        [Header("UI References")]
        [Tooltip("The Slider component for health bar")]
        [SerializeField] private Slider _healthSlider;

        [Tooltip("The fill image of the slider (for color change)")]
        [SerializeField] private Image _fillImage;

        [Tooltip("Text showing 'Current / Max' HP")]
        [SerializeField] private TextMeshProUGUI _healthText;

        [Header("Color Gradient (GDD Section 6.2)")]
        [SerializeField] private Color _highColor = new Color(0.2f, 0.8f, 0.2f);    // Green
        [SerializeField] private Color _mediumColor = new Color(0.9f, 0.8f, 0.1f);  // Yellow
        [SerializeField] private Color _lowColor = new Color(0.9f, 0.2f, 0.2f);     // Red

        [Tooltip("HP% threshold for yellow (GDD: 50%)")]
        [SerializeField] private float _mediumThreshold = 0.5f;

        [Tooltip("HP% threshold for red (GDD: 25%)")]
        [SerializeField] private float _lowThreshold = 0.25f;

        [Header("Animation")]
        [Tooltip("Smooth speed for slider movement")]
        [SerializeField] private float _smoothSpeed = 5f;
        #endregion

        #region Private Fields
        private float _targetValue = 1f;
        private int _maxHealth = 100;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            // Get max health from HealthSystem
            var healthSystem = FindObjectOfType<Player.HealthSystem>();
            if (healthSystem != null)
            {
                _maxHealth = healthSystem.MaxHealth;
            }

            if (_healthSlider != null)
            {
                _healthSlider.minValue = 0f;
                _healthSlider.maxValue = 1f;
                _healthSlider.interactable = false; // Display only
            }
        }

        private void OnEnable()
        {
            GameEvents.OnHealthChanged += OnHealthChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnHealthChanged -= OnHealthChanged;
        }

        private void Update()
        {
            // Smooth slider animation
            if (_healthSlider != null)
            {
                _healthSlider.value = Mathf.Lerp(
                    _healthSlider.value, _targetValue, _smoothSpeed * Time.deltaTime);
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Updates health bar when health changes.
        /// </summary>
        private void OnHealthChanged(int newHealth)
        {
            float percentage = (float)newHealth / _maxHealth;
            _targetValue = Mathf.Clamp01(percentage);

            // Update text
            if (_healthText != null)
            {
                _healthText.text = $"{newHealth} / {_maxHealth}";
            }

            // Update color based on thresholds (GDD Section 6.2)
            UpdateBarColor(percentage);
        }
        #endregion

        #region Color Management
        /// <summary>
        /// Updates bar color based on HP percentage.
        /// GDD: Green (100-50%) → Yellow (50-25%) → Red (25-0%)
        /// </summary>
        private void UpdateBarColor(float percentage)
        {
            if (_fillImage == null) return;

            Color targetColor;

            if (percentage > _mediumThreshold)
            {
                // Green zone: 100% - 50%
                targetColor = _highColor;
            }
            else if (percentage > _lowThreshold)
            {
                // Yellow zone: 50% - 25% — lerp from yellow to orange
                float t = (percentage - _lowThreshold) / (_mediumThreshold - _lowThreshold);
                targetColor = Color.Lerp(_mediumColor, _highColor, t);
            }
            else
            {
                // Red zone: 25% - 0% — lerp from red to yellow
                float t = percentage / _lowThreshold;
                targetColor = Color.Lerp(_lowColor, _mediumColor, t);
            }

            _fillImage.color = targetColor;
        }
        #endregion
    }
}
