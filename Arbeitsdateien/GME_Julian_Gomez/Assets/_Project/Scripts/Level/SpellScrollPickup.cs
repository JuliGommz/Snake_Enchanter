/*
====================================================================
* SpellScrollPickup - Collectable scroll that unlocks a spell/tune
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-18
* Version: 1.0
*
* AUTHORSHIP CLASSIFICATION:
*
* [AI-ASSISTED]
* - Walk-over trigger collection (OnTriggerEnter)
* - Interact() raycast hook for New Input System compatibility
* - Proximity emission glow via Shader.PropertyToID
* - Fires GameEvents.ScrollCollected on collection
*
* NOTES:
* - Attach to each scroll prefab in the cave
* - Requires Collider with IsTrigger = true for walk-over detection
* - Player must be tagged "Player"
* - Set up MeshRenderer or child Renderer for glow effect
* - Interact() is called by PlayerController's raycast (New Input System)
*   DO NOT use OnMouseDown — legacy Unity callback, violates project rules
*
* VERSION HISTORY:
* - v1.0: Initial implementation (Phase 7 spell system)
====================================================================
*/

using UnityEngine;
using SnakeEnchanter.Core;

namespace SnakeEnchanter.Level
{
    /// <summary>
    /// Placed on each scroll prefab. Handles walk-over collection,
    /// click-to-collect via PlayerController raycast, and proximity glow.
    /// </summary>
    public class SpellScrollPickup : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Scroll Data")]
        [Tooltip("Which tune this scroll unlocks (1 = Move, 2 = Daze, 3 = Shield)")]
        [Range(1, 3)]
        [SerializeField] private int _tuneNumberToUnlock = 1;

        [Tooltip("Display name shown in the unlock panel (e.g. 'Scroll of Movement')")]
        [SerializeField] private string _scrollName = "Scroll of Movement";

        [TextArea(2, 4)]
        [Tooltip("Description shown in the unlock panel")]
        [SerializeField] private string _scrollDescription = "Hold [1] and release in the glowing zone to charm snakes away.";

        [Header("Proximity Glow")]
        [Tooltip("Distance at which glow starts (full glow = distance 0)")]
        [Range(1f, 20f)]
        [SerializeField] private float _glowMaxDistance = 8f;

        [Tooltip("Maximum emission intensity at point-blank distance")]
        [Range(0.5f, 10f)]
        [SerializeField] private float _glowMaxIntensity = 3f;
        #endregion

        #region Private Fields
        private bool _collected = false;
        private Transform _playerTransform;
        private Renderer _renderer;
        private Material _material;

        // Cached shader property ID — static to avoid per-instance allocation
        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Cache player transform for distance calculations
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
            else
            {
                Debug.LogWarning($"[SpellScrollPickup] No GameObject tagged 'Player' found. Glow will not work.", this);
            }

            // Cache renderer — check self first, then children (scroll mesh may be a child)
            _renderer = GetComponent<Renderer>();
            if (_renderer == null)
            {
                _renderer = GetComponentInChildren<Renderer>();
            }

            if (_renderer != null)
            {
                // Use instance material so we don't modify the shared asset
                _material = _renderer.material;
                _material.EnableKeyword("_EMISSION");
            }
            else
            {
                Debug.LogWarning($"[SpellScrollPickup] No Renderer found on '{name}' or its children. Glow disabled.", this);
            }
        }

        private void Update()
        {
            if (_collected) return;
            UpdateProximityGlow();
        }
        #endregion

        #region Collection Methods
        /// <summary>
        /// Walk-over collection. Requires a Collider with IsTrigger = true on this GameObject.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (_collected) return;
            if (other.CompareTag("Player"))
            {
                Collect();
            }
        }

        /// <summary>
        /// Click collection hook. Called by PlayerController's raycast (New Input System).
        /// Do NOT use OnMouseDown — it is a legacy Unity callback.
        /// </summary>
        public void Interact()
        {
            if (_collected) return;
            Collect();
        }
        #endregion

        #region Core Logic
        /// <summary>
        /// Marks scroll as collected, hides the GameObject, and fires the collection event.
        /// </summary>
        private void Collect()
        {
            _collected = true;
            gameObject.SetActive(false);
            GameEvents.ScrollCollected(_tuneNumberToUnlock, _scrollName, _scrollDescription);
        }

        /// <summary>
        /// Updates emission color based on distance to player.
        /// Full glow at distance 0, no glow at _glowMaxDistance.
        /// </summary>
        private void UpdateProximityGlow()
        {
            if (_renderer == null || _playerTransform == null || _material == null) return;

            float dist = Vector3.Distance(transform.position, _playerTransform.position);
            float t = 1f - Mathf.Clamp01(dist / _glowMaxDistance);
            Color glowColor = Color.yellow * (t * _glowMaxIntensity);
            _material.SetColor(EmissionColorId, glowColor);
        }
        #endregion
    }
}
