/*
====================================================================
* TuneSliderUI - Minimal tune slider display
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
* - Slider UI visualization for tune casting
* - Triggerzone visual feedback
* - GDD Section 6.2 compliant layout
* - Human reviewed and will modify as needed

* DEPENDENCIES:
* - TuneController.cs (SnakeEnchanter.Tunes) — reads slider state
* - GameEvents.cs (SnakeEnchanter.Core)
* - Unity UI (Slider, Image, Text)

* DESIGN RATIONALE (GDD Section 6.2):
* - Location: Center-bottom of screen
* - Horizontal bar: 0% left, 100% right
* - Moving marker shows current slider position
* - Triggerzone highlighted in green/cyan
* - Before zone: Gray (safe, no effect)
* - After zone: Red (danger)
* - Appears on key press, disappears on release

* SETUP:
* 1. Create Canvas → Panel (center-bottom, anchored)
* 2. Inside panel: Slider + zone overlay images
* 3. Assign references in Inspector
* 4. Set TuneController reference

* NOTES:
* - Phase 1: Minimal — shows slider position + zone
* - Phase 3: Polish with animations, particles, glow

* VERSION HISTORY:
* - v1.0: Initial — slider, zone visualization, show/hide
====================================================================
*/

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SnakeEnchanter.Core;
using SnakeEnchanter.Tunes;

namespace SnakeEnchanter.UI
{
    /// <summary>
    /// Displays the tune slider during casting.
    /// Shows slider position, triggerzone, and timing feedback.
    /// GDD: Center-bottom, appears on hold, disappears on release.
    /// </summary>
    public class TuneSliderUI : MonoBehaviour
    {
        #region Configuration
        [Header("References")]
        [Tooltip("The TuneController to read state from")]
        [SerializeField] private TuneController _tuneController;

        [Header("UI Elements")]
        [Tooltip("Root panel — shown/hidden during casting")]
        [SerializeField] private GameObject _sliderPanel;

        [Tooltip("The main Slider component")]
        [SerializeField] private Slider _slider;

        [Tooltip("Fill image of the slider (for position color)")]
        [SerializeField] private Image _fillImage;

        [Tooltip("Triggerzone overlay image (RectTransform positioned dynamically)")]
        [SerializeField] private RectTransform _zoneOverlay;

        [Tooltip("Image component of zone overlay (for color)")]
        [SerializeField] private Image _zoneImage;

        [Tooltip("Tune name/number label")]
        [SerializeField] private TextMeshProUGUI _tuneLabel;

        [Tooltip("Result feedback text (SUCCESS / FAIL)")]
        [SerializeField] private TextMeshProUGUI _resultText;

        [Header("Colors")]
        [SerializeField] private Color _beforeZoneColor = new Color(0.5f, 0.5f, 0.5f);  // Gray
        [SerializeField] private Color _inZoneColor = new Color(0.2f, 0.9f, 0.4f);      // Green
        [SerializeField] private Color _afterZoneColor = new Color(0.9f, 0.2f, 0.2f);   // Red
        [SerializeField] private Color _zoneHighlightColor = new Color(0.2f, 0.8f, 0.8f, 0.4f); // Cyan transparent

        [Header("Result Feedback")]
        [SerializeField] private float _resultDisplayDuration = 1.5f;
        [SerializeField] private Color _successColor = new Color(0.2f, 0.9f, 0.2f);
        [SerializeField] private Color _failEarlyColor = new Color(0.8f, 0.8f, 0.4f);   // Yellow-ish
        [SerializeField] private Color _failLateColor = new Color(0.9f, 0.2f, 0.2f);     // Red
        #endregion

        #region Private Fields
        private bool _isShowing = false;
        private float _resultTimer = 0f;
        private RectTransform _sliderRect;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (_tuneController == null)
            {
                _tuneController = FindObjectOfType<TuneController>();
            }

            if (_slider != null)
            {
                _sliderRect = _slider.GetComponent<RectTransform>();
                _slider.minValue = 0f;
                _slider.maxValue = 1f;
                _slider.interactable = false;
            }

            HideSlider();
        }

        private void OnEnable()
        {
            GameEvents.OnTuneStarted += OnTuneStarted;
            GameEvents.OnTuneReleased += OnTuneReleased;
            GameEvents.OnTuneSuccess += OnTuneSuccess;
            GameEvents.OnTuneFailed += OnTuneFailed;
        }

        private void OnDisable()
        {
            GameEvents.OnTuneStarted -= OnTuneStarted;
            GameEvents.OnTuneReleased -= OnTuneReleased;
            GameEvents.OnTuneSuccess -= OnTuneSuccess;
            GameEvents.OnTuneFailed -= OnTuneFailed;
        }

        private void Update()
        {
            if (_isShowing && _tuneController != null && _tuneController.IsHolding)
            {
                UpdateSliderPosition();
                UpdateSliderColor();
            }

            // Result text fade timer
            if (_resultTimer > 0f)
            {
                _resultTimer -= Time.deltaTime;
                if (_resultTimer <= 0f)
                {
                    HideResultText();
                }
            }
        }
        #endregion

        #region Event Handlers
        private void OnTuneStarted(int tuneNumber)
        {
            ShowSlider(tuneNumber);
        }

        private void OnTuneReleased()
        {
            // Small delay before hiding to show result
            Invoke(nameof(HideSlider), 0.3f);
        }

        private void OnTuneSuccess()
        {
            ShowResult("SUCCESS!", _successColor);
        }

        private void OnTuneFailed(bool snakeAttacks)
        {
            if (snakeAttacks)
            {
                ShowResult("TOO LATE!", _failLateColor);
            }
            else
            {
                ShowResult("Too Early", _failEarlyColor);
            }
        }
        #endregion

        #region Display Logic
        /// <summary>
        /// Shows the slider panel and configures for the current tune.
        /// </summary>
        private void ShowSlider(int tuneNumber)
        {
            _isShowing = true;

            if (_sliderPanel != null)
                _sliderPanel.SetActive(true);

            if (_slider != null)
                _slider.value = 0f;

            // Set tune label
            if (_tuneLabel != null)
            {
                string tuneName = tuneNumber switch
                {
                    1 => "Move",
                    2 => "Sleep",
                    3 => "Attack",
                    4 => "Freeze",
                    _ => $"Tune {tuneNumber}"
                };
                _tuneLabel.text = $"[{tuneNumber}] {tuneName}";
            }

            // Position zone overlay
            UpdateZoneOverlay();

            // Hide any previous result
            HideResultText();
        }

        /// <summary>
        /// Hides the slider panel.
        /// </summary>
        private void HideSlider()
        {
            _isShowing = false;

            if (_sliderPanel != null)
                _sliderPanel.SetActive(false);
        }

        /// <summary>
        /// Updates slider value from TuneController.
        /// </summary>
        private void UpdateSliderPosition()
        {
            if (_slider == null || _tuneController == null) return;
            _slider.value = _tuneController.SliderPosition;
        }

        /// <summary>
        /// Updates slider fill color based on position relative to zone.
        /// GDD: Gray before, Green in zone, Red after.
        /// </summary>
        private void UpdateSliderColor()
        {
            if (_fillImage == null || _tuneController == null) return;

            float position = _tuneController.SliderPosition;
            float zoneStart = _tuneController.ZoneStart;
            float zoneEnd = _tuneController.ZoneEnd;

            if (position < zoneStart)
            {
                _fillImage.color = _beforeZoneColor;
            }
            else if (position <= zoneEnd)
            {
                _fillImage.color = _inZoneColor;
            }
            else
            {
                _fillImage.color = _afterZoneColor;
            }
        }

        /// <summary>
        /// Positions the zone overlay on the slider to match triggerzone.
        /// </summary>
        private void UpdateZoneOverlay()
        {
            if (_zoneOverlay == null || _sliderRect == null || _tuneController == null) return;

            float zoneStart = _tuneController.ZoneStart;
            float zoneEnd = _tuneController.ZoneEnd;
            float sliderWidth = _sliderRect.rect.width;

            // Position zone overlay as anchored rect
            float startX = zoneStart * sliderWidth;
            float zoneWidth = (zoneEnd - zoneStart) * sliderWidth;

            _zoneOverlay.anchoredPosition = new Vector2(
                startX - sliderWidth / 2f + zoneWidth / 2f, 0f);
            _zoneOverlay.sizeDelta = new Vector2(zoneWidth, _zoneOverlay.sizeDelta.y);

            // Set zone color
            if (_zoneImage != null)
            {
                _zoneImage.color = _zoneHighlightColor;
            }
        }

        /// <summary>
        /// Shows result feedback text.
        /// </summary>
        private void ShowResult(string text, Color color)
        {
            if (_resultText == null) return;

            _resultText.text = text;
            _resultText.color = color;
            _resultText.gameObject.SetActive(true);
            _resultTimer = _resultDisplayDuration;
        }

        /// <summary>
        /// Hides result text.
        /// </summary>
        private void HideResultText()
        {
            if (_resultText != null)
            {
                _resultText.gameObject.SetActive(false);
            }
        }
        #endregion
    }
}
