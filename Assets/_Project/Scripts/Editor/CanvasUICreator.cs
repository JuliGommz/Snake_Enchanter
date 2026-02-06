/*
====================================================================
* CanvasUICreator - Editor utility to generate Canvas UI hierarchy
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-06
* Version: 2.0
*
* EDITOR ONLY - This script only runs in Unity Editor
*
* USAGE: Unity Menu -> SnakeEnchanter -> Create Canvas UI
* Creates a complete Canvas with HealthBar and TuneSlider UI.
* All elements are fully adjustable via Inspector after creation.
*
* AUTHORSHIP CLASSIFICATION:
* [AI-ASSISTED]
* - Canvas hierarchy structure
* - Auto-wiring of SerializeField references
* - GDD Section 6.2 compliant layout
* - v2.0: Segment-based TuneSlider, Gradient HealthBar
* - Human reviewed and will modify as needed
*
* CREATES:
* Canvas (ScreenSpace-Overlay, 1920x1080 Scaler)
* +-- HealthBar (top-center, 500x50)
* |   +-- Background (dark bar)
* |   +-- Fill Area -> Fill (gradient-colored)
* |   +-- DebuffText ("Giftiger Nebel - HP sinkt")
* +-- TuneSliderPanel (center-bottom, hidden by default)
*     +-- TuneLabel ("[1] Move")
*     +-- SliderArea
*     |   +-- SliderFrame (Image for curved sprite)
*     |   +-- SegmentContainer (segments built at runtime)
*     |   +-- Marker (Image for music note sprite)
*     +-- ResultText ("SUCCESS!" / "TOO LATE!")
*
* IMPORTANT: After creation, all positions, sizes, colors, sprites,
* and references can be freely adjusted in the Inspector!
*
* VERSION HISTORY:
* - v1.0: Initial - Slider-based HealthBar + TuneSlider
* - v2.0: Gradient HealthBar, Segmented TuneSlider, Marker, Frame
====================================================================
*/

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

namespace SnakeEnchanter.Editor
{
    public static class CanvasUICreator
    {
        [MenuItem("SnakeEnchanter/Create Canvas UI")]
        public static void CreateCanvasUI()
        {
            // Check if Canvas already exists
            var existingCanvas = GameObject.Find("GameCanvas");
            if (existingCanvas != null)
            {
                if (!EditorUtility.DisplayDialog("Canvas Exists",
                    "A 'GameCanvas' already exists in the scene.\nDo you want to delete it and create a new one?",
                    "Replace", "Cancel"))
                {
                    return;
                }
                Undo.DestroyObjectImmediate(existingCanvas);
            }

            // === CANVAS ===
            GameObject canvasGO = new GameObject("GameCanvas");
            Undo.RegisterCreatedObjectUndo(canvasGO, "Create Canvas UI");

            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasGO.AddComponent<GraphicRaycaster>();

            // === HEALTH BAR (top-center) ===
            GameObject healthBarGO = CreateHealthBar(canvasGO.transform);

            // === TUNE SLIDER PANEL (center-bottom) ===
            GameObject tuneSliderGO = CreateTuneSliderPanel(canvasGO.transform);

            // === WIRE UP COMPONENTS ===
            WireHealthBarUI(healthBarGO);
            WireTuneSliderUI(canvasGO, tuneSliderGO);

            // Select the canvas in hierarchy
            Selection.activeGameObject = canvasGO;

            Debug.Log("CanvasUICreator v2.0: Canvas UI created successfully! Check Inspector to adjust values.");
            EditorUtility.DisplayDialog("Canvas UI Created (v2.0)",
                "GameCanvas created with:\n\n" +
                "HealthBar (top-center, 500x50)\n" +
                "  - Gradient color (Red->Yellow->Green)\n" +
                "  - Pulsing effect at low HP\n" +
                "  - Debuff text (always visible)\n\n" +
                "TuneSliderPanel (center-bottom)\n" +
                "  - Segmented blocks (15 segments)\n" +
                "  - Marker sprite (assign in Inspector)\n" +
                "  - Frame sprite (assign in Inspector)\n" +
                "  - 6 zone colors (all configurable)\n\n" +
                "All elements adjustable in Inspector!",
                "OK");
        }

        // ================================================================
        // HEALTH BAR (top-center, 500x50)
        // ================================================================
        private static GameObject CreateHealthBar(Transform parent)
        {
            // Root object with HealthBarUI component
            GameObject root = new GameObject("HealthBar");
            root.transform.SetParent(parent, false);
            RectTransform rootRect = root.AddComponent<RectTransform>();

            // Anchor top-center (GDD Section 6.2 v2.0)
            rootRect.anchorMin = new Vector2(0.5f, 1f);
            rootRect.anchorMax = new Vector2(0.5f, 1f);
            rootRect.pivot = new Vector2(0.5f, 1f);
            rootRect.anchoredPosition = new Vector2(0f, -30f);
            rootRect.sizeDelta = new Vector2(500f, 50f);

            // Background (dark bar)
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(root.transform, false);
            RectTransform bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            Image bgImage = bg.AddComponent<Image>();
            bgImage.color = new Color(0.15f, 0.15f, 0.15f, 0.85f);

            // Slider component on root
            Slider slider = root.AddComponent<Slider>();
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0.3f; // Starting health = 30/100
            slider.interactable = false;

            // Fill Area
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(root.transform, false);
            RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0f, 0f);
            fillAreaRect.anchorMax = new Vector2(1f, 1f);
            fillAreaRect.offsetMin = new Vector2(5f, 5f);
            fillAreaRect.offsetMax = new Vector2(-5f, -5f);

            // Fill image
            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            RectTransform fillRect = fill.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.sizeDelta = Vector2.zero;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            Image fillImage = fill.AddComponent<Image>();
            fillImage.color = new Color(0.9f, 0.5f, 0.15f); // Orange-ish for 30% HP

            // Wire slider fill
            slider.fillRect = fillRect;
            slider.targetGraphic = fillImage;

            // Debuff Text (below the bar)
            GameObject debuffGO = new GameObject("DebuffText");
            debuffGO.transform.SetParent(root.transform, false);
            RectTransform debuffRect = debuffGO.AddComponent<RectTransform>();
            debuffRect.anchorMin = new Vector2(0f, 0f);
            debuffRect.anchorMax = new Vector2(1f, 0f);
            debuffRect.pivot = new Vector2(0.5f, 1f);
            debuffRect.anchoredPosition = new Vector2(0f, -2f);
            debuffRect.sizeDelta = new Vector2(0f, 22f);
            TextMeshProUGUI debuffText = debuffGO.AddComponent<TextMeshProUGUI>();
            debuffText.text = "\u2620 Giftiger Nebel \u2014 HP sinkt";
            debuffText.fontSize = 14;
            debuffText.fontStyle = FontStyles.Italic;
            debuffText.alignment = TextAlignmentOptions.Center;
            debuffText.color = new Color(0.8f, 0.3f, 0.3f, 0.9f);

            return root;
        }

        // ================================================================
        // TUNE SLIDER PANEL (center-bottom, segmented)
        // ================================================================
        private static GameObject CreateTuneSliderPanel(Transform parent)
        {
            // Root panel (center-bottom, hidden by default)
            GameObject panel = new GameObject("TuneSliderPanel");
            panel.transform.SetParent(parent, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();

            // Anchor bottom-center (GDD Section 6.2)
            panelRect.anchorMin = new Vector2(0.5f, 0f);
            panelRect.anchorMax = new Vector2(0.5f, 0f);
            panelRect.pivot = new Vector2(0.5f, 0f);
            panelRect.anchoredPosition = new Vector2(0f, 50f);
            panelRect.sizeDelta = new Vector2(500f, 80f);

            // Panel background
            Image panelBg = panel.AddComponent<Image>();
            panelBg.color = new Color(0.1f, 0.1f, 0.1f, 0.7f);

            // Tune Label (top of panel)
            GameObject labelGO = new GameObject("TuneLabel");
            labelGO.transform.SetParent(panel.transform, false);
            RectTransform labelRect = labelGO.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0f, 1f);
            labelRect.anchorMax = new Vector2(1f, 1f);
            labelRect.pivot = new Vector2(0.5f, 1f);
            labelRect.anchoredPosition = new Vector2(0f, -2f);
            labelRect.sizeDelta = new Vector2(0f, 25f);
            TextMeshProUGUI tuneLabel = labelGO.AddComponent<TextMeshProUGUI>();
            tuneLabel.text = "[1] Move";
            tuneLabel.fontSize = 16;
            tuneLabel.alignment = TextAlignmentOptions.Center;
            tuneLabel.color = Color.white;

            // Slider area container
            GameObject sliderArea = new GameObject("SliderArea");
            sliderArea.transform.SetParent(panel.transform, false);
            RectTransform sliderAreaRect = sliderArea.AddComponent<RectTransform>();
            sliderAreaRect.anchorMin = new Vector2(0f, 0f);
            sliderAreaRect.anchorMax = new Vector2(1f, 1f);
            sliderAreaRect.offsetMin = new Vector2(20f, 25f);
            sliderAreaRect.offsetMax = new Vector2(-20f, -28f);

            // Slider Frame (Image for curved sprite, behind segments)
            GameObject frameGO = new GameObject("SliderFrame");
            frameGO.transform.SetParent(sliderArea.transform, false);
            RectTransform frameRect = frameGO.AddComponent<RectTransform>();
            frameRect.anchorMin = Vector2.zero;
            frameRect.anchorMax = Vector2.one;
            frameRect.sizeDelta = Vector2.zero;
            frameRect.offsetMin = Vector2.zero;
            frameRect.offsetMax = Vector2.zero;
            Image frameImage = frameGO.AddComponent<Image>();
            frameImage.color = new Color(0.3f, 0.25f, 0.2f, 1f);
            frameImage.raycastTarget = false;
            // Sprite assigned via Inspector after creation

            // Segment Container (empty — segments built at runtime by TuneSliderUI)
            GameObject containerGO = new GameObject("SegmentContainer");
            containerGO.transform.SetParent(sliderArea.transform, false);
            RectTransform containerRect = containerGO.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0f, 0f);
            containerRect.anchorMax = new Vector2(1f, 1f);
            containerRect.offsetMin = new Vector2(4f, 4f);
            containerRect.offsetMax = new Vector2(-4f, -4f);

            // Marker (Image for music note/flute sprite, moves along slider)
            GameObject markerGO = new GameObject("Marker");
            markerGO.transform.SetParent(sliderArea.transform, false);
            RectTransform markerRect = markerGO.AddComponent<RectTransform>();
            markerRect.anchorMin = new Vector2(0f, 0.5f);
            markerRect.anchorMax = new Vector2(0f, 0.5f);
            markerRect.pivot = new Vector2(0.5f, 0f);
            markerRect.anchoredPosition = new Vector2(0f, 0f);
            markerRect.sizeDelta = new Vector2(24f, 32f);
            Image markerImage = markerGO.AddComponent<Image>();
            markerImage.color = Color.white;
            markerImage.raycastTarget = false;
            markerGO.SetActive(false); // Hidden by default
            // Sprite assigned via Inspector after creation

            // Result Text (below panel)
            GameObject resultGO = new GameObject("ResultText");
            resultGO.transform.SetParent(panel.transform, false);
            RectTransform resultRect = resultGO.AddComponent<RectTransform>();
            resultRect.anchorMin = new Vector2(0f, 0f);
            resultRect.anchorMax = new Vector2(1f, 0f);
            resultRect.pivot = new Vector2(0.5f, 1f);
            resultRect.anchoredPosition = new Vector2(0f, -5f);
            resultRect.sizeDelta = new Vector2(0f, 30f);
            TextMeshProUGUI resultText = resultGO.AddComponent<TextMeshProUGUI>();
            resultText.text = "";
            resultText.fontSize = 22;
            resultText.fontStyle = FontStyles.Bold;
            resultText.alignment = TextAlignmentOptions.Center;
            resultText.color = Color.green;
            resultGO.SetActive(false); // Hidden by default

            // Hide panel by default (TuneSliderUI shows it on tune cast)
            panel.SetActive(false);

            return panel;
        }

        // ================================================================
        // WIRE UP SERIALIZED FIELDS
        // ================================================================
        private static void WireHealthBarUI(GameObject healthBarGO)
        {
            // Add HealthBarUI component
            var healthBarUI = healthBarGO.AddComponent<UI.HealthBarUI>();

            // Use SerializedObject to set private [SerializeField] fields
            SerializedObject so = new SerializedObject(healthBarUI);

            Slider slider = healthBarGO.GetComponent<Slider>();
            Image fillImage = healthBarGO.transform.Find("Fill Area/Fill").GetComponent<Image>();
            TextMeshProUGUI debuffText = healthBarGO.transform.Find("DebuffText").GetComponent<TextMeshProUGUI>();

            so.FindProperty("_healthSlider").objectReferenceValue = slider;
            so.FindProperty("_fillImage").objectReferenceValue = fillImage;
            so.FindProperty("_debuffText").objectReferenceValue = debuffText;

            so.ApplyModifiedPropertiesWithoutUndo();

            Debug.Log("CanvasUICreator: HealthBarUI v2.0 wired - _healthSlider, _fillImage, _debuffText");
        }

        private static void WireTuneSliderUI(GameObject canvasGO, GameObject tuneSliderPanelGO)
        {
            // Add TuneSliderUI to canvas root (not the panel, since panel gets hidden)
            var tuneSliderUI = canvasGO.AddComponent<UI.TuneSliderUI>();

            SerializedObject so = new SerializedObject(tuneSliderUI);

            // Find all elements
            Transform panel = tuneSliderPanelGO.transform;
            Transform sliderArea = panel.Find("SliderArea");
            RectTransform segmentContainer = sliderArea.Find("SegmentContainer").GetComponent<RectTransform>();
            Image frameImage = sliderArea.Find("SliderFrame").GetComponent<Image>();
            Image markerImage = sliderArea.Find("Marker").GetComponent<Image>();
            TextMeshProUGUI tuneLabel = panel.Find("TuneLabel").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI resultText = panel.Find("ResultText").GetComponent<TextMeshProUGUI>();

            so.FindProperty("_sliderPanel").objectReferenceValue = tuneSliderPanelGO;
            so.FindProperty("_segmentContainer").objectReferenceValue = segmentContainer;
            so.FindProperty("_frameImageRef").objectReferenceValue = frameImage;
            so.FindProperty("_markerImage").objectReferenceValue = markerImage;
            so.FindProperty("_tuneLabel").objectReferenceValue = tuneLabel;
            so.FindProperty("_resultText").objectReferenceValue = resultText;

            // TuneController reference - try to find it in scene
            var tuneController = Object.FindFirstObjectByType<Tunes.TuneController>();
            if (tuneController != null)
            {
                so.FindProperty("_tuneController").objectReferenceValue = tuneController;
                Debug.Log("CanvasUICreator: TuneController auto-found and assigned!");
            }
            else
            {
                Debug.LogWarning("CanvasUICreator: TuneController not found in scene - assign manually in Inspector!");
            }

            so.ApplyModifiedPropertiesWithoutUndo();

            Debug.Log("CanvasUICreator: TuneSliderUI v2.0 wired - panel, segments, frame, marker, labels");
        }
    }
}
#endif
