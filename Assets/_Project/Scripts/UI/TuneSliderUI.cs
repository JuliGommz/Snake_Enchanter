/*
====================================================================
* TuneSliderUI - Segmented tune slider with marker and frame
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-06
* Version: 2.1

* WICHTIG: KOMMENTIERUNG NICHT LOSCHEN!
* Diese detaillierte Authorship-Dokumentation ist fuer die akademische
* Bewertung erforderlich und darf nicht entfernt werden!

* AUTHORSHIP CLASSIFICATION:

* [AI-ASSISTED]
* - Segment-based slider visualization (Genshin-style)
* - Zone coloring system (Gelb=Safe, Orange=Success, Grau=Danger)
* - Marker sprite movement along segments
* - Customizable frame image for curved appearance
* - All colors and dimensions Inspector-configurable
* - Event-driven via GameEvents
* - Human reviewed and will modify as needed

* DEPENDENCIES:
* - TuneController.cs (SnakeEnchanter.Tunes) - reads slider state
* - GameEvents.cs (SnakeEnchanter.Core)
* - Unity UI (Image, TextMeshPro)

* DESIGN RATIONALE (GDD Section 6.2 + Genshin Cooking Reference):
* - Location: Center-bottom of screen
* - Segmented blocks instead of solid fill
* - 3 color zones: Gelb (safe) -> Orange (success) -> Grau (danger)
* - Zone size = difficulty (wider = easier, narrower = harder)
* - Marker sprite (music note/flute) moves along segments
* - Frame sprite assignable for curved visual appearance
* - Appears on key press, disappears on release

* SETUP:
* 1. Use CanvasUICreator (Menu -> SnakeEnchanter -> Create Canvas UI)
* 2. Segments are built at runtime in Awake()
* 3. Assign marker and frame sprites via Inspector
* 4. All colors, dimensions, and segment count adjustable in Inspector

* VERSION HISTORY:
* - v1.0: Initial - Unity Slider, solid fill, zone overlay
* - v2.0: Segmented blocks, marker sprite, frame image, zone colors
* - v2.1: Fix marker size apply, frame sprite Sliced, OnValidate live update, keep aspect ratio
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
    /// Displays the tune slider as segmented colored blocks with a moving marker.
    /// Genshin-style: Gelb=Safe, Orange=Success zone, Grau=Danger.
    /// </summary>
    public class TuneSliderUI : MonoBehaviour
    {
        #region Configuration
        [Header("References")]
        [Tooltip("The TuneController to read state from")]
        [SerializeField] private TuneController _tuneController;

        [Header("UI Elements")]
        [Tooltip("Root panel - shown/hidden during casting")]
        [SerializeField] private GameObject _sliderPanel;

        [Tooltip("Tune name/number label")]
        [SerializeField] private TextMeshProUGUI _tuneLabel;

        [Tooltip("Result feedback text (SUCCESS / FAIL)")]
        [SerializeField] private TextMeshProUGUI _resultText;

        [Header("Segment Container")]
        [Tooltip("Parent RectTransform that holds all segment blocks")]
        [SerializeField] private RectTransform _segmentContainer;

        [Header("Slider Dimensions")]
        [Tooltip("Total width of the slider area in pixels")]
        [SerializeField] private float _sliderWidth = 460f;

        [Tooltip("Total height of the slider area in pixels")]
        [SerializeField] private float _sliderHeight = 30f;

        [Tooltip("Number of visual segments")]
        [SerializeField, Range(10, 20)] private int _segmentCount = 15;

        [Tooltip("Gap between segments in pixels")]
        [SerializeField] private float _segmentGap = 2f;

        [Header("Frame")]
        [Tooltip("Background frame sprite (for curved appearance)")]
        [SerializeField] private Sprite _sliderFrameSprite;

        [Tooltip("Frame tint color")]
        [SerializeField] private Color _frameColor = new Color(0.3f, 0.25f, 0.2f, 1f);

        [Tooltip("Reference to the frame Image component")]
        [SerializeField] private Image _frameImageRef;

        [Header("Marker")]
        [Tooltip("Marker Image that moves along the slider")]
        [SerializeField] private Image _markerImage;

        [Tooltip("Marker sprite (music note, flute, etc.)")]
        [SerializeField] private Sprite _markerSprite;

        [Tooltip("Marker size in pixels")]
        [SerializeField] private Vector2 _markerSize = new Vector2(24f, 32f);

        [Tooltip("Keep marker sprite aspect ratio (adjusts height from width)")]
        [SerializeField] private bool _markerKeepAspect = true;

        [Header("Segment Colors - Safe Zone (before trigger)")]
        [Tooltip("Gelb: Inactive segments before the trigger zone")]
        [SerializeField] private Color _segmentSafeColor = new Color(0.85f, 0.75f, 0.3f, 0.4f);

        [Tooltip("Gelb leuchtend: Active (lit) segments before zone")]
        [SerializeField] private Color _segmentSafeActiveColor = new Color(0.95f, 0.85f, 0.3f, 1f);

        [Header("Segment Colors - Trigger Zone (success)")]
        [Tooltip("Orange: Inactive segments in the trigger zone")]
        [SerializeField] private Color _segmentZoneColor = new Color(0.9f, 0.55f, 0.1f, 0.5f);

        [Tooltip("Orange leuchtend: Active segments in zone (success!)")]
        [SerializeField] private Color _segmentZoneActiveColor = new Color(1f, 0.65f, 0.1f, 1f);

        [Header("Segment Colors - Danger Zone (after trigger)")]
        [Tooltip("Grau/durchsichtig: Inactive segments past the zone")]
        [SerializeField] private Color _segmentDangerColor = new Color(0.3f, 0.3f, 0.3f, 0.25f);

        [Tooltip("Rot: Active segments past zone (snake attacks!)")]
        [SerializeField] private Color _segmentDangerActiveColor = new Color(0.9f, 0.2f, 0.2f, 1f);

        [Header("Result Feedback")]
        [SerializeField] private float _resultDisplayDuration = 1.5f;
        [SerializeField] private Color _successColor = new Color(0.2f, 0.9f, 0.2f);
        [SerializeField] private Color _failEarlyColor = new Color(0.8f, 0.8f, 0.4f);
        [SerializeField] private Color _failLateColor = new Color(0.9f, 0.2f, 0.2f);
        #endregion

        #region Private Fields
        private bool _isShowing = false;
        private float _resultTimer = 0f;
        private Image[] _segments;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (_tuneController == null)
            {
                _tuneController = FindFirstObjectByType<TuneController>();
            }

            // Apply frame sprite and color
            ApplyFrameSettings();

            // Apply marker sprite and size
            ApplyMarkerSettings();
            if (_markerImage != null)
            {
                _markerImage.gameObject.SetActive(false);
            }

            // Build segments at runtime
            BuildSegments();

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
                UpdateSegments();
                UpdateMarker();
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

        #region Inspector Live Update
#if UNITY_EDITOR
        /// <summary>
        /// Called by Unity when Inspector values change (Editor only).
        /// Applies frame color, frame sprite, marker sprite, and marker size live.
        /// </summary>
        private void OnValidate()
        {
            // Delay to next frame to avoid Unity warnings about SendMessage
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this == null) return;
                ApplyFrameSettings();
                ApplyMarkerSettings();
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

            if (_sliderFrameSprite != null)
            {
                _frameImageRef.sprite = _sliderFrameSprite;
                _frameImageRef.type = Image.Type.Sliced;
                _frameImageRef.preserveAspect = false;
            }
        }

        /// <summary>
        /// Applies marker sprite and _markerSize to the marker Image.
        /// If _markerKeepAspect is true, height is calculated from width using sprite aspect ratio.
        /// </summary>
        private void ApplyMarkerSettings()
        {
            if (_markerImage == null) return;

            if (_markerSprite != null)
            {
                _markerImage.sprite = _markerSprite;
            }

            // Apply _markerSize with optional aspect ratio lock
            Vector2 finalSize = _markerSize;
            if (_markerKeepAspect && _markerSprite != null && _markerSprite.rect.width > 0f)
            {
                float aspect = _markerSprite.rect.height / _markerSprite.rect.width;
                finalSize.y = finalSize.x * aspect;
            }

            _markerImage.rectTransform.sizeDelta = finalSize;
        }
        #endregion

        #region Segment System
        /// <summary>
        /// Creates segment Image blocks at runtime inside the container.
        /// Manual positioning for full control over layout.
        /// </summary>
        private void BuildSegments()
        {
            if (_segmentContainer == null) return;

            // Clear any existing children
            for (int i = _segmentContainer.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(_segmentContainer.GetChild(i).gameObject);
            }

            _segments = new Image[_segmentCount];
            float totalGaps = _segmentGap * (_segmentCount - 1);
            float segWidth = (_sliderWidth - totalGaps) / _segmentCount;

            for (int i = 0; i < _segmentCount; i++)
            {
                GameObject segGO = new GameObject($"Segment_{i}");
                segGO.transform.SetParent(_segmentContainer, false);

                RectTransform segRect = segGO.AddComponent<RectTransform>();
                segRect.anchorMin = new Vector2(0f, 0f);
                segRect.anchorMax = new Vector2(0f, 1f);
                segRect.pivot = new Vector2(0f, 0.5f);

                float xPos = i * (segWidth + _segmentGap);
                segRect.anchoredPosition = new Vector2(xPos, 0f);
                segRect.sizeDelta = new Vector2(segWidth, 0f); // Height stretches via anchors

                Image segImage = segGO.AddComponent<Image>();
                segImage.color = _segmentSafeColor;
                segImage.raycastTarget = false;

                _segments[i] = segImage;
            }
        }

        /// <summary>
        /// Updates segment colors based on slider position and zone.
        /// Gelb = safe (before zone), Orange = success (in zone), Grau/Rot = danger (after zone).
        /// </summary>
        private void UpdateSegments()
        {
            if (_segments == null || _tuneController == null) return;

            float position = _tuneController.SliderPosition;
            float zoneStart = _tuneController.ZoneStart;
            float zoneEnd = _tuneController.ZoneEnd;

            int activeSegmentIndex = Mathf.FloorToInt(position * _segmentCount);
            activeSegmentIndex = Mathf.Clamp(activeSegmentIndex, 0, _segmentCount - 1);

            for (int i = 0; i < _segmentCount; i++)
            {
                if (_segments[i] == null) continue;

                float segStart = (float)i / _segmentCount;
                float segEnd = (float)(i + 1) / _segmentCount;
                bool isInZone = (segEnd > zoneStart && segStart < zoneEnd);
                bool isPastZone = (segStart >= zoneEnd);
                bool isLit = (i <= activeSegmentIndex);

                if (isLit)
                {
                    // Active/lit segment
                    if (isInZone)
                        _segments[i].color = _segmentZoneActiveColor;     // Orange leuchtend
                    else if (isPastZone)
                        _segments[i].color = _segmentDangerActiveColor;   // Rot
                    else
                        _segments[i].color = _segmentSafeActiveColor;     // Gelb leuchtend
                }
                else
                {
                    // Inactive/unlit segment
                    if (isInZone)
                        _segments[i].color = _segmentZoneColor;           // Orange gedimmt
                    else if (isPastZone)
                        _segments[i].color = _segmentDangerColor;         // Grau/durchsichtig
                    else
                        _segments[i].color = _segmentSafeColor;           // Gelb gedimmt
                }
            }
        }

        /// <summary>
        /// Moves the marker image along the segment positions.
        /// </summary>
        private void UpdateMarker()
        {
            if (_markerImage == null || _tuneController == null || _segmentContainer == null) return;

            float position = _tuneController.SliderPosition;
            float xPos = position * _sliderWidth;

            RectTransform markerRect = _markerImage.rectTransform;
            markerRect.anchoredPosition = new Vector2(xPos, markerRect.anchoredPosition.y);
        }

        /// <summary>
        /// Resets all segments to their inactive state.
        /// </summary>
        private void ResetSegments()
        {
            if (_segments == null) return;
            for (int i = 0; i < _segments.Length; i++)
            {
                if (_segments[i] != null)
                    _segments[i].color = _segmentSafeColor;
            }
        }

        /// <summary>
        /// Pre-colors zone segments so player can see the target zone before filling.
        /// Called when a tune starts — shows Gelb/Orange/Grau layout.
        /// </summary>
        private void PreColorZoneSegments()
        {
            if (_segments == null || _tuneController == null) return;

            float zoneStart = _tuneController.ZoneStart;
            float zoneEnd = _tuneController.ZoneEnd;

            for (int i = 0; i < _segments.Length; i++)
            {
                if (_segments[i] == null) continue;

                float segStart = (float)i / _segmentCount;
                float segEnd = (float)(i + 1) / _segmentCount;
                bool isInZone = (segEnd > zoneStart && segStart < zoneEnd);
                bool isPastZone = (segStart >= zoneEnd);

                if (isInZone)
                    _segments[i].color = _segmentZoneColor;       // Orange gedimmt
                else if (isPastZone)
                    _segments[i].color = _segmentDangerColor;     // Grau/durchsichtig
                else
                    _segments[i].color = _segmentSafeColor;       // Gelb gedimmt
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
            // Hide marker immediately
            if (_markerImage != null)
                _markerImage.gameObject.SetActive(false);

            // Small delay before hiding panel to show result
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

            // Reset and pre-color segments
            ResetSegments();
            PreColorZoneSegments();

            // Show and reset marker
            if (_markerImage != null)
            {
                _markerImage.gameObject.SetActive(true);
                RectTransform markerRect = _markerImage.rectTransform;
                markerRect.anchoredPosition = new Vector2(0f, markerRect.anchoredPosition.y);
            }

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
