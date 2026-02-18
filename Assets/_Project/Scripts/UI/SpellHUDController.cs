/*
====================================================================
* SpellHUDController - Dynamic Spell Slot HUD
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-18
* Version: 1.0

* ⚠️ WICHTIG: KOMMENTIERUNG NICHT LÖSCHEN! ⚠️
* Diese detaillierte Authorship-Dokumentation ist für die akademische
* Bewertung erforderlich und darf nicht entfernt werden!

* AUTHORSHIP CLASSIFICATION:

* [AI-ASSISTED]
* - Dynamic slot instantiation via GameEvents.OnTuneUnlocked
* - CanvasGroup fade-in reveal coroutine with unscaledDeltaTime
* - Human reviewed and will modify as needed

* DEPENDENCIES:
* - GameEvents.cs (SnakeEnchanter.Core) — OnTuneUnlocked event
* - TMPro (TextMeshProUGUI) — slot label and spell name text
* - Unity UI (CanvasGroup, Image, HorizontalLayoutGroup)

* DESIGN NOTES:
* - HUD starts completely empty — no slots visible at game start
* - Each scroll collection fires OnTuneUnlocked → adds one slot
* - Slots are instantiated at runtime into _slotsContainer
* - _slotsContainer should have HorizontalLayoutGroup for auto-layout
* - RevealSlot uses Time.unscaledDeltaTime to handle timeScale=0 edge cases

* SLOT PREFAB STRUCTURE (build in Unity Editor):
* SlotPrefab (RectTransform, CanvasGroup)
*   ├── Background (Image — colored background per tune)
*   ├── KeyIcon (Image — physical key shape sprite, one per tune slot)
*   │   └── KeyLabel (TextMeshProUGUI — "1", "2", "3" — child of KeyIcon, overlaid on key)
*   └── SpellName (TextMeshProUGUI — "Move", "Daze", "Shield")
*
* NOTE: _slotsContainer should have a HorizontalLayoutGroup component
*       for automatic left-to-right slot positioning.

* VERSION HISTORY:
* - v1.0: Initial — dynamic slot creation, fade-in reveal, color/name/key icon per tune
====================================================================
*/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SnakeEnchanter.Core;

namespace SnakeEnchanter.UI
{
    /// <summary>
    /// Manages the dynamic spell HUD that starts empty and grows as scrolls are collected.
    /// Subscribes to GameEvents.OnTuneUnlocked to create and reveal one slot per tune.
    /// Each slot shows a key icon with number, spell name, and tune color.
    /// </summary>
    public class SpellHUDController : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Layout")]
        [Tooltip("Parent Transform for slot GameObjects. Use HorizontalLayoutGroup for auto-positioning.")]
        [SerializeField] private Transform _slotsContainer;

        [Tooltip("Prefab for one tune slot. See slot structure in header comment.")]
        [SerializeField] private GameObject _slotPrefab;

        [Header("Tune Appearance")]
        [Tooltip("Color per tune slot: [0]=Move (green), [1]=Daze (blue), [2]=Shield (gold)")]
        [SerializeField] private Color[] _tuneColors = new Color[3]
        {
            Color.green,
            Color.blue,
            new Color(1f, 0.85f, 0f)   // Gold for Shield
        };

        [Tooltip("Name per tune slot: [0]=Move, [1]=Daze, [2]=Shield")]
        [SerializeField] private string[] _tuneNames = new string[3]
        {
            "Move",
            "Daze",
            "Shield"
        };

        [Header("Key Icons")]
        [Tooltip("Physical key shape sprites for each tune slot. " +
                 "Index 0=Move, 1=Daze, 2=Shield. " +
                 "Per user decision #8: each slot shows a key shape with the tune number on it. " +
                 "Assign placeholder sprites in Inspector until final art is ready.")]
        [SerializeField] private Sprite[] _keyIconSprites = new Sprite[3];
        #endregion

        #region Private Fields
        // Instantiated slot GameObjects — null until that tune is unlocked
        private GameObject[] _slots = new GameObject[3];

        // CanvasGroups for fade-in reveal — one per slot
        private CanvasGroup[] _slotCanvasGroups = new CanvasGroup[3];
        #endregion

        #region Unity Lifecycle
        private void OnEnable()
        {
            GameEvents.OnTuneUnlocked += OnTuneUnlocked;
        }

        private void OnDisable()
        {
            GameEvents.OnTuneUnlocked -= OnTuneUnlocked;
        }
        #endregion

        #region Slot Creation
        /// <summary>
        /// Called when a tune is unlocked via scroll collection.
        /// Creates and reveals the corresponding HUD slot.
        /// </summary>
        private void OnTuneUnlocked(int tuneNumber)
        {
            int idx = tuneNumber - 1;

            // Guard: invalid index or slot already created
            if (idx < 0 || idx >= 3 || _slots[idx] != null) return;

            // Guard: missing prefab or container
            if (_slotPrefab == null)
            {
                Debug.LogError("SpellHUDController: _slotPrefab is not assigned in Inspector!");
                return;
            }
            if (_slotsContainer == null)
            {
                Debug.LogError("SpellHUDController: _slotsContainer is not assigned in Inspector!");
                return;
            }

            // Instantiate slot as child of container
            GameObject slot = Instantiate(_slotPrefab, _slotsContainer);
            _slots[idx] = slot;

            // Configure slot appearance
            ConfigureSlot(slot, idx, tuneNumber);

            // Set up CanvasGroup for fade-in
            CanvasGroup cg = slot.GetComponent<CanvasGroup>();
            if (cg == null)
            {
                cg = slot.AddComponent<CanvasGroup>();
            }
            cg.alpha = 0f;
            _slotCanvasGroups[idx] = cg;

            // Start fade-in coroutine
            StartCoroutine(RevealSlot(idx));

            Debug.Log($"SpellHUDController: Created slot for Tune {tuneNumber} ({_tuneNames[idx]})");
        }

        /// <summary>
        /// Configures a slot's visual elements (key icon, label, spell name, background color).
        /// </summary>
        private void ConfigureSlot(GameObject slot, int idx, int tuneNumber)
        {
            // Background color
            Transform bgTransform = slot.transform.Find("Background");
            if (bgTransform != null)
            {
                Image bg = bgTransform.GetComponent<Image>();
                if (bg != null)
                    bg.color = _tuneColors[idx];
            }
            else
            {
                Debug.LogWarning($"SpellHUDController: Slot prefab missing 'Background' child Image.");
            }

            // Key icon sprite (physical key shape — user decision #8)
            Transform keyIconTransform = slot.transform.Find("KeyIcon");
            if (keyIconTransform != null)
            {
                Image keyIcon = keyIconTransform.GetComponent<Image>();
                if (keyIcon != null && _keyIconSprites != null && idx < _keyIconSprites.Length)
                    keyIcon.sprite = _keyIconSprites[idx];

                // Key label (number overlaid on key shape — child of KeyIcon)
                Transform keyLabelTransform = keyIconTransform.Find("KeyLabel");
                if (keyLabelTransform != null)
                {
                    TextMeshProUGUI keyLabel = keyLabelTransform.GetComponent<TextMeshProUGUI>();
                    if (keyLabel != null)
                        keyLabel.text = tuneNumber.ToString();
                }
                else
                {
                    Debug.LogWarning($"SpellHUDController: KeyIcon missing 'KeyLabel' child TMPro.");
                }
            }
            else
            {
                Debug.LogWarning($"SpellHUDController: Slot prefab missing 'KeyIcon' child Image.");
            }

            // Spell name label
            Transform spellNameTransform = slot.transform.Find("SpellName");
            if (spellNameTransform != null)
            {
                TextMeshProUGUI spellName = spellNameTransform.GetComponent<TextMeshProUGUI>();
                if (spellName != null)
                    spellName.text = _tuneNames[idx];
            }
            else
            {
                Debug.LogWarning($"SpellHUDController: Slot prefab missing 'SpellName' child TMPro.");
            }
        }
        #endregion

        #region Reveal Animation
        /// <summary>
        /// Fades in the slot CanvasGroup from alpha 0 to 1 over 0.5 seconds.
        /// Uses Time.unscaledDeltaTime because this may be called immediately after
        /// the scroll unlock panel dismiss, where timeScale might still be transitioning.
        /// </summary>
        private IEnumerator RevealSlot(int idx)
        {
            float duration = 0.5f;
            float elapsed = 0f;
            CanvasGroup cg = _slotCanvasGroups[idx];

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                cg.alpha = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }

            cg.alpha = 1f;
        }
        #endregion
    }
}
