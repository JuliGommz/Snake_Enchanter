/*
====================================================================
* HealthBarUI - Visual health bar with gradient, pulse, and debuff
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-06
* Version: 3.1
* WICHTIG: KOMMENTIERUNG NICHT LOSCHEN!
* Diese detaillierte Authorship-Dokumentation ist fuer die akademische
* Bewertung erforderlich und darf nicht entfernt werden!
* AUTHORSHIP CLASSIFICATION:
* [AI-ASSISTED]
* - UI Slider-based health bar
* - Gradient color system (replaces threshold-based v1.0)
* - Pulsing effect at low HP
* - Debuff indicator text
* - Customizable frame sprite and colors
* - Customizable background image
* - Icon/marker sprite for visual enhancement
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
* - Frame/background fully customizable
* SETUP:
* 1. Use CanvasUICreator (Menu -> SnakeEnchanter -> Create Canvas UI)
* 2. Or manually: Canvas -> Slider, assign Fill Image
* 3. Assign frame sprite and health icon in Inspector
* 4. All values adjustable in Inspector after creation
* VERSION HISTORY:
* - v1.0: Initial - slider, 3-color thresholds, numeric text
* - v2.0: Gradient, pulse effect, debuff text, no numbers, bigger bar
* - v3.0: Frame sprite, background image, health icon, full visual customization
* - v3.1: Fix gradient not updating (continuous UpdateBarColor in Update)
====================================================================
*/

using SnakeEnchanter.Core;
using SnakeEnchanter.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SnakeEnchanter.UI
{
    /// <summary>
    /// Displays player health as a gradient-colored slider with pulsing effect.
    /// GDD: Top-center, Red->Yellow->Green gradient, visual only.
    /// v3.0: Added frame, background, and icon customization.
    /// </summary>
    public class HealthBarUI : MonoBehaviour
    {
        #region Configuration

        [Header("UI References")]
        [Tooltip("The Slider component for health bar")]
        [SerializeField] private Slider _healthSlider;

        [Tooltip("The fill image of the slider (for color + pulse)")]
        [SerializeField] private Image _fillImage;

        [Header("Frame & Background")]
        [Tooltip("Background frame sprite (decorative border/frame)")]
        [SerializeField] private Sprite _frameSprite;

        [Tooltip("Frame tint color")]
        [SerializeField] private Color _frameColor = new Color(0.3f, 0.25f, 0.2f, 1f);

        [Tooltip("Reference to the frame Image component")]
        [SerializeField] private Image _frameImageRef;

        [Tooltip("Background sprite behind the health bar (optional)")]
        [SerializeField] private Sprite _backgroundSprite;

        [Tooltip("Background tint color")]
        [SerializeField] private Color _backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.6f);

        [Tooltip("Reference to the background Image component")]
        [SerializeField] private Image _backgroundImageRef;

        [Header("Health Icon/Marker")]
        [Tooltip("Icon sprite (heart, cross, etc.) displayed with health bar")]
        [SerializeField] private Sprite _healthIconSprite;

        [Tooltip("Icon size in pixels")]
        [SerializeField] private Vector2 _iconSize = new Vector2(32f, 32f);

        [Tooltip("Keep icon sprite aspect ratio")]
        [SerializeField] private bool _iconKeepAspect = true;

        [Tooltip("Reference to the icon Image component")]
        [SerializeField] private Image _iconImageRef;

        [Header("Color Gradient")]
        [Tooltip("Maps health % to color. Key 0=red(dead), 0.5=yellow, 1.0=green(full)")]
        [SerializeField] private Gradient _healthGradient;

        [Header("Custom Fill Colors (Optional Override)")]
        [Tooltip("Enable to override gradient with custom colors")]
        [SerializeField] private bool _useCustomColors = false;

        [Tooltip("Fill color at critical health (0-25%)")]
        [SerializeField] private Color _criticalHealthColor = new Color(0.9f, 0.2f, 0.2f, 1f);

        [Tooltip("Fill color at low health (25-50%)")]
        [SerializeField] private Color _lowHealthColor = new Color(0.9f, 0.8f, 0.1f, 1f);

        [Tooltip("Fill color at medium health (50-75%)")]
        [SerializeField] private Color _mediumHealthColor = new Color(0.7f, 0.85f, 0.3f, 1f);

        [Tooltip("Fill color at high health (75-100%)")]
        [SerializeField] private Color _highHealthColor = new Color(0.2f, 0.8f, 0.2f, 1f);

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
                colorKeys[0] = new GradientColorKey(new Color(0.9f, 0.2f, 0.2f), 0.0f); // Red at 0%
                colorKeys[1] = new GradientColorKey(new Color(0.9f, 0.8f, 0.1f), 0.5f); // Yellow at 50%
                colorKeys[2] = new GradientColorKey(new Color(0.2f, 0.8f, 0.2f), 1.0f); // Green at 100%

                GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
                alphaKeys[0] = new GradientAlphaKey(1f, 0f);
                alphaKeys[1] = new GradientAlphaKey(1f, 1f);

                _healthGradient.SetKeys(colorKeys, alphaKeys);
            }

            // Apply visual settings
            ApplyFrameSettings();
            ApplyBackgroundSettings();
            ApplyIconSettings();
        }

        private void Start()
        {
            // Get max health from HealthSystem
            var healthSystem = FindFirstObjectByType<HealthSystem>();
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

                // Update gradient color continuously (follows smoothed slider value)
                UpdateBarColor(_healthSlider.value);
            }

            // Pulsing effect on fill (alpha only, preserves gradient color)
            ApplyPulseEffect();
        }

        #endregion

        #region Inspector Live Update

#if UNITY_EDITOR
        /// <summary>
        /// Called by Unity when Inspector values change (Editor only).
        /// Applies visual settings live without entering Play mode.
        /// </summary>
        private void OnValidate()
        {
            // Delay to next frame to avoid Unity warnings
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this == null) return;
                ApplyFrameSettings();
                ApplyBackgroundSettings();
                ApplyIconSettings();
            };
        }
#endif

        /// <summary>
        /// Applies frame sprite and color to the frame Image.
        /// </summary>
        private void ApplyFrameSettings()
        {
            if (_frameImageRef == null) return;

            _frameImageRef.color = _frameColor;

            if (_frameSprite != null)
            {
                _frameImageRef.sprite = _frameSprite;
                _frameImageRef.type = Image.Type.Sliced;
                _frameImageRef.preserveAspect = false;
            }
        }

        /// <summary>
        /// Applies background sprite and color to the background Image.
        /// </summary>
        private void ApplyBackgroundSettings()
        {
            if (_backgroundImageRef == null) return;

            _backgroundImageRef.color = _backgroundColor;

            if (_backgroundSprite != null)
            {
                _backgroundImageRef.sprite = _backgroundSprite;
                _backgroundImageRef.type = Image.Type.Sliced;
                _backgroundImageRef.preserveAspect = false;
            }
        }

        /// <summary>
        /// Applies icon sprite and size to the icon Image.
        /// If _iconKeepAspect is true, height is calculated from width using sprite aspect ratio.
        /// </summary>
        private void ApplyIconSettings()
        {
            if (_iconImageRef == null) return;

            if (_healthIconSprite != null)
            {
                _iconImageRef.sprite = _healthIconSprite;
            }

            // Apply icon size with optional aspect ratio lock
            Vector2 finalSize = _iconSize;
            if (_iconKeepAspect && _healthIconSprite != null && _healthIconSprite.rect.width > 0f)
            {
                float aspect = _healthIconSprite.rect.height / _healthIconSprite.rect.width;
                finalSize.y = finalSize.x * aspect;
            }

            _iconImageRef.rectTransform.sizeDelta = finalSize;
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
            // Color update happens continuously in Update() via smoothed slider value
        }

        #endregion

        #region Color Management

        /// <summary>
        /// Updates bar color using gradient evaluation or custom color zones.
        /// </summary>
        private void UpdateBarColor(float percentage)
        {
            if (_fillImage == null) return;

            if (_useCustomColors)
            {
                // Use custom color zones
                if (percentage <= 0.25f)
                    _fillImage.color = _criticalHealthColor;
                else if (percentage <= 0.5f)
                    _fillImage.color = _lowHealthColor;
                else if (percentage <= 0.75f)
                    _fillImage.color = _mediumHealthColor;
                else
                    _fillImage.color = _highHealthColor;
            }
            else
            {
                // Use gradient
                _fillImage.color = _healthGradient.Evaluate(percentage);
            }
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
