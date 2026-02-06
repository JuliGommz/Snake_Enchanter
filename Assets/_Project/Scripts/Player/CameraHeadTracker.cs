/*
====================================================================
* CameraHeadTracker - Tracks animated head position only, not rotation
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-06
* Version: 1.0 - POSITION ONLY TRACKING
* 
* ⚠️ WICHTIG: KOMMENTIERUNG NICHT LÖSCHEN! ⚠️
* Diese detaillierte Authorship-Dokumentation ist für die akademische
* Bewertung erforderlich und darf nicht entfernt werden!
* 
* AUTHORSHIP CLASSIFICATION:
* [AI-ASSISTED]
* - Position-only tracking of animated head bone
* - Does NOT touch rotation (PlayerController handles that)
* - LateUpdate execution for post-animation tracking
* - Configurable offset for camera positioning
* - OnValidate for live Inspector preview
* - Human reviewed and will modify as needed
* 
* DEPENDENCIES:
* - PlayerController.cs (handles camera rotation via mouse)
* - Animated Head bone (provides position)
* - Unity Core (UnityEngine)
* 
* DESIGN RATIONALE:
* - Problem: PlayerController expects Main Camera as Player child (mouse look)
* - Problem: Head animation position must be tracked
* - Solution: Track ONLY position from Head, leave rotation to PlayerController
* - Architecture: Separation of Concerns (Position vs Rotation)
* 
* COMPATIBILITY:
* - Works WITH PlayerController.HandleCameraLook()
* - Does NOT conflict with localRotation control
* - Position from Head, Rotation from Mouse Input
* 
* SETUP:
* 1. Main Camera stays as child of Player (PlayerController requirement)
* 2. Attach this script to Main Camera
* 3. Assign Head bone as Head Target
* 4. PlayerController continues to handle rotation normally
* 5. Adjust Position Offset in Inspector (live preview available)
* 
* VERSION HISTORY:
* - v1.0: Position-only tracking, rotation-agnostic, OnValidate live preview
====================================================================
*/

using UnityEngine;

namespace SnakeEnchanter.Player
{
    /// <summary>
    /// Verfolgt nur die Position des animierten Head-Bones.
    /// Rotation wird NICHT berührt (PlayerController hat Ownership).
    /// </summary>
    public class CameraHeadTracker : MonoBehaviour
    {
        #region Configuration

        [Header("Head Tracking")]
        [Tooltip("Das animierte Head-Bone dessen Position getrackt werden soll")]
        [SerializeField] private Transform _headTarget;

        [Header("Position Offset")]
        [Tooltip("Offset relativ zur Head-Position (local space des Players)")]
        [SerializeField] private Vector3 _positionOffset = new Vector3(0f, 0f, 0f);

        [Header("Smoothing")]
        [Tooltip("Position-Smoothing (0 = instant, höher = smoother)")]
        [SerializeField] private float _positionSmoothSpeed = 8f;

        [Tooltip("Instant Follow (kein Smoothing)")]
        [SerializeField] private bool _instantFollow = false;

        #endregion

        #region Private Fields

        // Cached parent transform (Player root)
        private Transform _playerRoot;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Find Player root (should be parent or grandparent)
            _playerRoot = transform.parent;

            if (_headTarget == null)
            {
                Debug.LogError("[CameraHeadTracker] Head Target nicht zugewiesen!", this);
            }
        }

        private void LateUpdate()
        {
            // LateUpdate: NACH Animation, NACH PlayerController.Update (Camera Rotation)
            // Wir ändern nur Position, Rotation bleibt unangetastet

            if (_headTarget == null || _playerRoot == null) return;

            UpdatePositionOnly();
        }

        #endregion

        #region Position Tracking

        /// <summary>
        /// Aktualisiert NUR die Position der Kamera basierend auf Head-Bone.
        /// Rotation wird NICHT berührt (PlayerController Ownership).
        /// </summary>
        private void UpdatePositionOnly()
        {
            // Calculate target position: Head world position + offset in Player local space
            Vector3 targetWorldPos = _headTarget.position;
            Vector3 offsetWorldPos = _playerRoot.TransformDirection(_positionOffset);
            Vector3 finalTargetPos = targetWorldPos + offsetWorldPos;

            if (_instantFollow || _positionSmoothSpeed <= 0f)
            {
                // Instant: Nur Position setzen, Rotation bleibt
                transform.position = finalTargetPos;
            }
            else
            {
                // Smooth: Nur Position lerpen, Rotation bleibt
                transform.position = Vector3.Lerp(
                    transform.position,
                    finalTargetPos,
                    _positionSmoothSpeed * Time.deltaTime
                );
            }

            // WICHTIG: transform.rotation wird NICHT berührt!
            // PlayerController.HandleCameraLook() hat Rotation-Ownership
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Ändert Head Target zur Laufzeit (z.B. für Cutscenes).
        /// </summary>
        public void SetHeadTarget(Transform newTarget)
        {
            _headTarget = newTarget;
        }

        /// <summary>
        /// Ändert Position Offset zur Laufzeit.
        /// </summary>
        public void SetPositionOffset(Vector3 newOffset)
        {
            _positionOffset = newOffset;
        }

        /// <summary>
        /// Aktiviert/Deaktiviert Head Tracking (z.B. für fixed camera angles).
        /// </summary>
        public void SetTrackingEnabled(bool enabled)
        {
            this.enabled = enabled;
        }

        #endregion

        #region Debug Helpers

#if UNITY_EDITOR
        /// <summary>
        /// Live-Update im Editor (ohne Play Mode).
        /// </summary>
        private void OnValidate()
        {
            // Nur ausführen wenn nicht im Play Mode
            if (Application.isPlaying) return;

            // Delay für Unity-Warnings vermeiden
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this == null) return;
                UpdatePositionInEditor();
            };
        }

        /// <summary>
        /// Editor-only position update für Inspector-Preview.
        /// </summary>
        private void UpdatePositionInEditor()
        {
            if (_headTarget == null) return;

            // Find player root if not cached
            if (_playerRoot == null)
            {
                _playerRoot = transform.parent;
                if (_playerRoot == null) return;
            }

            // Calculate and apply position directly (no smoothing in editor)
            Vector3 targetWorldPos = _headTarget.position;
            Vector3 offsetWorldPos = _playerRoot.TransformDirection(_positionOffset);
            Vector3 finalTargetPos = targetWorldPos + offsetWorldPos;

            transform.position = finalTargetPos;
        }

        /// <summary>
        /// Visualisiert Head Tracking im Scene View (nur wenn Camera ausgewählt).
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (_headTarget == null) return;

            Transform playerRoot = _playerRoot != null ? _playerRoot : transform.parent;
            if (playerRoot == null) return;

            // Target position visualization
            Vector3 targetPos = _headTarget.position;
            Vector3 offsetWorldPos = playerRoot.TransformDirection(_positionOffset);
            Vector3 finalPos = targetPos + offsetWorldPos;

            // Green line: Head → Camera target position
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_headTarget.position, finalPos);
            Gizmos.DrawWireSphere(finalPos, 0.2f);

            // Yellow line: Current camera → Target
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, finalPos);

            // Cyan sphere: Head bone position
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_headTarget.position, 0.15f);
        }
#endif

        #endregion
    }
}
