/*
====================================================================
* TuneConfig - ScriptableObject for tune configuration
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-03
* Version: 1.0

* AUTHORSHIP CLASSIFICATION:

* [AI-ASSISTED]
* - ScriptableObject structure based on ADR-002
* - Timing parameters for ADR-008 Slider system

* DEPENDENCIES:
* - None (data container only)

* DESIGN RATIONALE (ADR-002 & ADR-008):
* - ScriptableObject allows Inspector-based balancing
* - Triggerzone position-based (0-1) for Slider system
* - Simple Mode bonus expands zone for easier gameplay
====================================================================
*/

using UnityEngine;

namespace SnakeEnchanter.Tunes
{
    /// <summary>
    /// Configuration data for a single tune.
    /// Create instances via: Create > SnakeEnchanter > TuneConfig
    /// </summary>
    [CreateAssetMenu(fileName = "NewTune", menuName = "SnakeEnchanter/TuneConfig", order = 1)]
    public class TuneConfig : ScriptableObject
    {
        #region Basic Info
        [Header("Basic Info")]
        [Tooltip("Display name of the tune")]
        public string tuneName = "New Tune";

        [Tooltip("Key number (1-4)")]
        [Range(1, 4)]
        public int keyNumber = 1;

        [Tooltip("Effect on snake when successful")]
        public SnakeEffect resultEffect = SnakeEffect.Move;

        [TextArea(2, 4)]
        [Tooltip("Description for UI/tooltip")]
        public string description = "Hold and release at the right moment.";
        #endregion

        #region Timing Configuration (ADR-008)
        [Header("Timing - Slider System (ADR-008)")]
        [Tooltip("Total time for slider to reach end (seconds)")]
        [Range(1f, 10f)]
        public float duration = 3.0f;

        [Tooltip("Triggerzone start position (0 = left, 1 = right)")]
        [Range(0f, 1f)]
        public float triggerZoneStart = 0.4f;

        [Tooltip("Triggerzone end position (0 = left, 1 = right)")]
        [Range(0f, 1f)]
        public float triggerZoneEnd = 0.65f;

        [Tooltip("Zone bonus for Simple Mode (added to both sides)")]
        [Range(0f, 0.3f)]
        public float simpleModeZoneBonus = 0.1f;
        #endregion

        #region Audio
        [Header("Audio")]
        [Tooltip("Melody that plays while holding")]
        public AudioClip melody;

        [Tooltip("Success sound effect")]
        public AudioClip successSound;

        [Tooltip("Fail sound effect")]
        public AudioClip failSound;
        #endregion

        #region Visual
        [Header("Visual")]
        [Tooltip("Color for triggerzone in UI")]
        public Color zoneColor = new Color(0.2f, 0.8f, 0.2f, 1f); // Green

        [Tooltip("Icon for tune selection")]
        public Sprite icon;
        #endregion

        #region Calculated Properties
        /// <summary>
        /// Size of the triggerzone (0-1).
        /// </summary>
        public float ZoneSize => triggerZoneEnd - triggerZoneStart;

        /// <summary>
        /// Center of the triggerzone (0-1).
        /// </summary>
        public float ZoneCenter => (triggerZoneStart + triggerZoneEnd) / 2f;

        /// <summary>
        /// Validates zone configuration.
        /// </summary>
        public bool IsValid => triggerZoneEnd > triggerZoneStart && duration > 0f;
        #endregion

        #region Editor Validation
        private void OnValidate()
        {
            // Ensure zone end is after zone start
            if (triggerZoneEnd <= triggerZoneStart)
            {
                triggerZoneEnd = triggerZoneStart + 0.1f;
            }

            // Clamp to valid range
            triggerZoneStart = Mathf.Clamp(triggerZoneStart, 0f, 0.9f);
            triggerZoneEnd = Mathf.Clamp(triggerZoneEnd, triggerZoneStart + 0.05f, 1f);
        }
        #endregion
    }

    /// <summary>
    /// Effect types for successful tune casts.
    /// </summary>
    public enum SnakeEffect
    {
        Move,       // Snake moves out of the way
        Sleep,      // Snake falls asleep
        Attack,     // Snake attacks enemies
        Freeze      // All creatures freeze (Advanced Mode)
    }
}
