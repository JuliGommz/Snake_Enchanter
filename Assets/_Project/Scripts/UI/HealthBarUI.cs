/*
====================================================================
* HealthBarUI - Visual health bar with gradient, pulse, and debuff
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-06
* Version: 2.0

* WICHTIG: KOMMENTIERUNG NICHT LOSCHEN!
* Diese detaillierte Authorship-Dokumentation ist fuer die akademische
* Bewertung erforderlich und darf nicht entfernt werden!

* AUTHORSHIP CLASSIFICATION:

* [AI-ASSISTED]
* - UI Slider-based health bar
* - Gradient color system (replaces threshold-based v1.0)
* - Pulsing effect at low HP
* - Debuff indicator text
* - Event-driven update via GameEvents
* - Human reviewed and will modify as needed

* DEPENDENCIES:
* - GameEvents.cs (SnakeEnchanter.Core)
* - Unity UI (UnityEngine.UI)
* - TextMeshPro (TMPro)
* - Canvas with Slider element in scene

* DESIGN RATIONALE (GDD Section 6.2):
* - Location: Top-center, 500x50
* - Gradient: Red (0%) -> Yellow (50%) -> Green (100%)
* - Visual only: No numeric display
* - Debuff text: Always visible (constant poison drain)
* - Pulsing: Subtle alpha oscillation, faster at low HP

* SETUP:
* 1. Use CanvasUICreator (Menu -> SnakeEnchanter -> Create Canvas UI)
* 2. Or manually: Canvas -> Slider, assign Fill Image
* 3. All values adjustable in Inspector after creation

* VERSION HISTORY:
* - v1.0: Initial - slider, 3-color thresholds, numeric text
* - v2.0: Gradient, pulse effect, debuff text, no numbers, bigger bar
====================================================================
*/

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SnakeEnchanter.Core;

namespace SnakeEnchanter.UI
{
    /// <summary>
    /// Displays player health as a gradient-colored slider with pulsing effect.
    /// GDD: Top-center, Red->Yellow->Green gradient, visual only.
    /// </summary>
    public class HealthBarUI : MonoBehaviour
    {
        #region Configuration
        [Header("UI References")]
        [Tooltip("The Slider component for health bar")]
        [SerializeField] private Slider _healthSlider;

        [Tooltip("The fill image of the slider (for color + pulse)")]
        [SerializeField] private Image _fillImage;

        [Header("Color Gradient")]
        [Tooltip("Maps health % to color. Key 0=red(dead), 0.5=yellow, 1.0=green(full)")]
        [SerializeField] private Gradient _healthGradient;

        [Header("Debuff Indicator")]
        [Tooltip("TextMeshPro element showing debuff status")]
        [SerializeField] private TextMeshProUGUI _debuffText;

        [Tooltip("Debuff message text (configurable in Inspector)")]
        [SerializeField] private string _debuffMessage = "\u2620 Giftiger Nebel \u2014 HP sinkt";

        [Header("Pulsing Effect")]
        [Tooltip("Base pulse speed (oscillations per second)")]
        [SerializeField] private float _pulseSpeed = 2f;

        [Tooltip("Pulse intensity - max alpha deviation (0-0.3 recommended)")]
        [SerializeField] private float _pulseIntensity = 0.15f;

        [Tooltip("HP threshold below which pulsing accelerates (0-1)")]
        [SerializeField] private float _pulseAccelerateThreshold = 0.3f;

        [Tooltip("Maximum pulse speed multiplier at 0 HP")]
        [SerializeField] private float _pulseMaxSpeedMultiplier = 3f;

        [Header("Animation")]
        [Tooltip("Smooth speed for slider movement")]
        [SerializeField] private float _smoothSpeed = 5f;
        #endregion

        #region Private Fields
        private float _targetValue = 1f;
        private int _maxHealth = 100;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Initialize default gradient if not configured in Inspector
            if (_healthGradient == null || _healthGradient.colorKeys.Length < 2)
            {
                _healthGradient = new Gradient();
                GradientColorKey[] colorKeys = new GradientColorKey[3];
                colorKeys[0] = new GradientColorKey(new Color(0.9f, 0.2f, 0.2f), 0.0f);  // Red at 0%
                colorKeys[1] = new GradientColorKey(new Color(0.9f, 0.8f, 0.1f), 0.5f);  // Yellow at 50%
                colorKeys[2] = new GradientColorKey(new Color(0.2f, 0.8f, 0.2f), 1.0f);  // Green at 100%

                GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
                alphaKeys[0] = new GradientAlphaKey(1f, 0f);
                alphaKeys[1] = new GradientAlphaKey(1f, 1f);

                _healthGradient.SetKeys(colorKeys, alphaKeys);
            }
        }

        private void Start()
        {
            // Get max health from HealthSystem
            var healthSystem = FindFirstObjectByType<Player.HealthSystem>();
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

            // Initialize debuff text
            if (_debuffText != null)
            {
                _debuffText.text = _debuffMessage;
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

            // Pulsing effect on fill
            ApplyPulseEffect();
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

            // Update color via gradient
            UpdateBarColor(percentage);
        }
        #endregion

        #region Color Management
        /// <summary>
        /// Updates bar color using gradient evaluation.
        /// Gradient maps 0.0 (dead/red) to 1.0 (full/green).
        /// </summary>
        private void UpdateBarColor(float percentage)
        {
            if (_fillImage == null) return;
            _fillImage.color = _healthGradient.Evaluate(percentage);
        }
        #endregion

        #region Pulse Effect
        /// <summary>
        /// Applies subtle alpha oscillation to the fill image.
        /// Pulse accelerates as HP drops below threshold for urgency feedback.
        /// </summary>
        private void ApplyPulseEffect()
        {
            if (_fillImage == null) return;

            // Calculate pulse speed based on current health
            float healthPercent = _healthSlider != null ? _healthSlider.value : 1f;
            float speedMultiplier = 1f;

            if (healthPercent < _pulseAccelerateThreshold && _pulseAccelerateThreshold > 0f)
            {
                // Accelerate pulse as HP drops below threshold
                float t = 1f - (healthPercent / _pulseAccelerateThreshold);
                speedMultiplier = Mathf.Lerp(1f, _pulseMaxSpeedMultiplier, t);
            }

            // Sine wave oscillation for smooth pulsing
            float pulse = Mathf.Sin(Time.time * _pulseSpeed * speedMultiplier * Mathf.PI * 2f);
            float alphaOffset = pulse * _pulseIntensity;

            Color current = _fillImage.color;
            current.a = Mathf.Clamp01(1f + alphaOffset);
            _fillImage.color = current;
        }
        #endregion
    }
}
