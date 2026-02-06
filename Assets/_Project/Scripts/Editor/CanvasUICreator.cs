/*
====================================================================
* CanvasUICreator - Editor utility to generate Canvas UI hierarchy
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-06
* Version: 1.0
*
* EDITOR ONLY — This script only runs in Unity Editor
*
* USAGE: Unity Menu → SnakeEnchanter → Create Canvas UI
* Creates a complete Canvas with HealthBar and TuneSlider UI.
* All elements are fully adjustable via Inspector after creation.
*
* AUTHORSHIP CLASSIFICATION:
* [AI-ASSISTED]
* - Canvas hierarchy structure
* - Auto-wiring of SerializeField references
* - GDD Section 6.2 compliant layout
* - Human reviewed and will modify as needed
*
* CREATES:
* Canvas (ScreenSpace-Overlay, 1920x1080 Scaler)
* ├── HealthBar (top-left)
* │   ├── Background (dark bar)
* │   ├── Fill Area → Fill (green, color-coded)
* │   └── HealthText ("30 / 100")
* └── TuneSliderPanel (center-bottom, hidden by default)
*     ├── TuneLabel ("[1] Move")
*     ├── SliderBackground (dark bar)
*     ├── TuneSlider (slider component)
*     │   └── Fill Area → Fill
*     ├── ZoneOverlay (cyan transparent)
*     └── ResultText ("SUCCESS!" / "TOO LATE!")
*
* IMPORTANT: After creation, all positions, sizes, colors, and
* references can be freely adjusted in the Inspector!
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

            // === HEALTH BAR (top-left) ===
            GameObject healthBarGO = CreateHealthBar(canvasGO.transform);

            // === TUNE SLIDER PANEL (center-bottom) ===
            GameObject tuneSliderGO = CreateTuneSliderPanel(canvasGO.transform);

            // === WIRE UP COMPONENTS ===
            WireHealthBarUI(healthBarGO);
            WireTuneSliderUI(tuneSliderGO);

            // Select the canvas in hierarchy
            Selection.activeGameObject = canvasGO;

            Debug.Log("CanvasUICreator: Canvas UI created successfully! Check Inspector to adjust values.");
            EditorUtility.DisplayDialog("Canvas UI Created",
                "GameCanvas created with:\n\n" +
                "• HealthBar (top-left)\n" +
                "  - Slider + Fill + HealthText\n" +
                "  - HealthBarUI script auto-wired\n\n" +
                "• TuneSliderPanel (center-bottom)\n" +
                "  - Slider + Fill + ZoneOverlay\n" +
                "  - TuneLabel + ResultText\n" +
                "  - TuneSliderUI script auto-wired\n" +
                "  - Hidden by default (shows on tune cast)\n\n" +
                "All elements are adjustable in Inspector!",
                "OK");
        }

        // ================================================================
        // HEALTH BAR
        // ================================================================
        private static GameObject CreateHealthBar(Transform parent)
        {
            // Root object with HealthBarUI component
            GameObject root = new GameObject("HealthBar");
            root.transform.SetParent(parent, false);
            RectTransform rootRect = root.AddComponent<RectTransform>();

            // Anchor top-left (GDD Section 6.2)
            rootRect.anchorMin = new Vector2(0f, 1f);
            rootRect.anchorMax = new Vector2(0f, 1f);
            rootRect.pivot = new Vector2(0f, 1f);
            rootRect.anchoredPosition = new Vector2(30f, -30f);
            rootRect.sizeDelta = new Vector2(300f, 40f);

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
            slider.value = 1f;
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
            fillImage.color = new Color(0.2f, 0.8f, 0.2f); // Green

            // Wire slider fill
            slider.fillRect = fillRect;
            slider.targetGraphic = fillImage;

            // Health Text
            GameObject textGO = new GameObject("HealthText");
            textGO.transform.SetParent(root.transform, false);
            RectTransform textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            TextMeshProUGUI healthText = textGO.AddComponent<TextMeshProUGUI>();
            healthText.text = "30 / 100";
            healthText.fontSize = 18;
            healthText.alignment = TextAlignmentOptions.Center;
            healthText.color = Color.white;

            return root;
        }

        // ================================================================
        // TUNE SLIDER PANEL
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

            // Slider Background
            GameObject sliderBg = new GameObject("SliderBackground");
            sliderBg.transform.SetParent(sliderArea.transform, false);
            RectTransform sliderBgRect = sliderBg.AddComponent<RectTransform>();
            sliderBgRect.anchorMin = Vector2.zero;
            sliderBgRect.anchorMax = Vector2.one;
            sliderBgRect.sizeDelta = Vector2.zero;
            sliderBgRect.offsetMin = Vector2.zero;
            sliderBgRect.offsetMax = Vector2.zero;
            Image sliderBgImage = sliderBg.AddComponent<Image>();
            sliderBgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);

            // Tune Slider
            GameObject sliderGO = new GameObject("TuneSlider");
            sliderGO.transform.SetParent(sliderArea.transform, false);
            RectTransform sliderRect = sliderGO.AddComponent<RectTransform>();
            sliderRect.anchorMin = Vector2.zero;
            sliderRect.anchorMax = Vector2.one;
            sliderRect.sizeDelta = Vector2.zero;
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = Vector2.zero;

            Slider tuneSlider = sliderGO.AddComponent<Slider>();
            tuneSlider.direction = Slider.Direction.LeftToRight;
            tuneSlider.minValue = 0f;
            tuneSlider.maxValue = 1f;
            tuneSlider.value = 0f;
            tuneSlider.interactable = false;

            // Slider Fill Area
            GameObject tuneFillArea = new GameObject("Fill Area");
            tuneFillArea.transform.SetParent(sliderGO.transform, false);
            RectTransform tuneFillAreaRect = tuneFillArea.AddComponent<RectTransform>();
            tuneFillAreaRect.anchorMin = Vector2.zero;
            tuneFillAreaRect.anchorMax = Vector2.one;
            tuneFillAreaRect.offsetMin = new Vector2(2f, 2f);
            tuneFillAreaRect.offsetMax = new Vector2(-2f, -2f);

            // Slider Fill
            GameObject tuneFill = new GameObject("Fill");
            tuneFill.transform.SetParent(tuneFillArea.transform, false);
            RectTransform tuneFillRect = tuneFill.AddComponent<RectTransform>();
            tuneFillRect.anchorMin = Vector2.zero;
            tuneFillRect.anchorMax = Vector2.one;
            tuneFillRect.sizeDelta = Vector2.zero;
            tuneFillRect.offsetMin = Vector2.zero;
            tuneFillRect.offsetMax = Vector2.zero;
            Image tuneFillImage = tuneFill.AddComponent<Image>();
            tuneFillImage.color = new Color(0.5f, 0.5f, 0.5f); // Gray (before zone)

            tuneSlider.fillRect = tuneFillRect;

            // Zone Overlay (cyan transparent rectangle)
            GameObject zoneOverlay = new GameObject("ZoneOverlay");
            zoneOverlay.transform.SetParent(sliderArea.transform, false);
            RectTransform zoneRect = zoneOverlay.AddComponent<RectTransform>();
            zoneRect.anchorMin = new Vector2(0.4f, 0f);
            zoneRect.anchorMax = new Vector2(0.65f, 1f);
            zoneRect.offsetMin = new Vector2(0f, 2f);
            zoneRect.offsetMax = new Vector2(0f, -2f);
            Image zoneImage = zoneOverlay.AddComponent<Image>();
            zoneImage.color = new Color(0.2f, 0.8f, 0.8f, 0.4f); // Cyan transparent
            zoneImage.raycastTarget = false;

            // Result Text (below slider)
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
            TextMeshProUGUI healthText = healthBarGO.transform.Find("HealthText").GetComponent<TextMeshProUGUI>();

            so.FindProperty("_healthSlider").objectReferenceValue = slider;
            so.FindProperty("_fillImage").objectReferenceValue = fillImage;
            so.FindProperty("_healthText").objectReferenceValue = healthText;

            so.ApplyModifiedPropertiesWithoutUndo();

            Debug.Log("CanvasUICreator: HealthBarUI wired — _healthSlider, _fillImage, _healthText");
        }

        private static void WireTuneSliderUI(GameObject tuneSliderPanelGO)
        {
            // Add TuneSliderUI to canvas root (not the panel itself, since panel gets hidden)
            Transform canvasTransform = tuneSliderPanelGO.transform.parent;
            var tuneSliderUI = canvasTransform.gameObject.AddComponent<UI.TuneSliderUI>();

            SerializedObject so = new SerializedObject(tuneSliderUI);

            // Find all elements
            Transform panel = tuneSliderPanelGO.transform;
            Slider slider = panel.Find("SliderArea/TuneSlider").GetComponent<Slider>();
            Image fillImage = panel.Find("SliderArea/TuneSlider/Fill Area/Fill").GetComponent<Image>();
            RectTransform zoneOverlay = panel.Find("SliderArea/ZoneOverlay").GetComponent<RectTransform>();
            Image zoneImage = panel.Find("SliderArea/ZoneOverlay").GetComponent<Image>();
            TextMeshProUGUI tuneLabel = panel.Find("TuneLabel").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI resultText = panel.Find("ResultText").GetComponent<TextMeshProUGUI>();

            so.FindProperty("_sliderPanel").objectReferenceValue = tuneSliderPanelGO;
            so.FindProperty("_slider").objectReferenceValue = slider;
            so.FindProperty("_fillImage").objectReferenceValue = fillImage;
            so.FindProperty("_zoneOverlay").objectReferenceValue = zoneOverlay;
            so.FindProperty("_zoneImage").objectReferenceValue = zoneImage;
            so.FindProperty("_tuneLabel").objectReferenceValue = tuneLabel;
            so.FindProperty("_resultText").objectReferenceValue = resultText;

            // TuneController reference — try to find it in scene
            var tuneController = Object.FindFirstObjectByType<Tunes.TuneController>();
            if (tuneController != null)
            {
                so.FindProperty("_tuneController").objectReferenceValue = tuneController;
                Debug.Log("CanvasUICreator: TuneController auto-found and assigned!");
            }
            else
            {
                Debug.LogWarning("CanvasUICreator: TuneController not found in scene — assign manually in Inspector!");
            }

            so.ApplyModifiedPropertiesWithoutUndo();

            Debug.Log("CanvasUICreator: TuneSliderUI wired — all 7 references set");
        }
    }
}
#endif
