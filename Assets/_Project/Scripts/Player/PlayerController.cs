/*
====================================================================
* PlayerController - Handles player movement and camera control
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-06
* Version: 1.8 - SINGLE MOVE FIX
* 
* ⚠️ WICHTIG: KOMMENTIERUNG NICHT LÖSCHEN! ⚠️
* Diese detaillierte Authorship-Dokumentation ist für die akademische
* Bewertung erforderlich und darf nicht entfernt werden!
* 
* AUTHORSHIP CLASSIFICATION:
* [AI-ASSISTED]
* - Initial implementation structure
* - CharacterController movement logic
* - First-person camera system (v1.1)
* - New Input System migration (v1.2)
* - Cinemachine compatibility - auto Camera.main (v1.6)
* - Cinemachine final integration - pitch control only (v1.7)
* - Human reviewed and will modify as needed
* 
* DEPENDENCIES:
* - Unity CharacterController component
* - Unity New Input System (InputSystem package)
* - SnakeEnchanter.inputactions asset
* - Cinemachine 3.x (Position: Cinemachine, Rotation: Split Control)
* 
* DESIGN RATIONALE:
* - Cinemachine handles camera position (follows CameraTarget under Head)
* - Cinemachine "Rotate With Follow Target" handles yaw (follows Player body)
* - PlayerController handles pitch (vertical look) via direct camera rotation
* - Player body rotates on Y-axis (mouse X), camera pitch on X-axis (mouse Y)
* 
* SETUP:
* 1. Main Camera with Cinemachine Brain (auto-managed)
* 2. CM_PlayerCamera with "Rotate With Follow Target"
* 3. CameraTarget under animated Head bone
* 4. This script on Player GameObject
* 5. Camera reference auto-found (Camera.main) or manually assigned
* 
* VERSION HISTORY:
* - v1.0: Initial with Legacy Input
* - v1.1: First-Person camera
* - v1.2: New Input System only
* - v1.3: Crouch system (hold LeftCtrl)
* - v1.4: Live camera offset (removed — caused override issues)
* - v1.5: Camera position via Hierarchy only, script never overrides
* - v1.6: Cinemachine compatibility - auto-finds Camera.main
* - v1.7: Cinemachine final - pitch-only control, yaw via Cinemachine
====================================================================
*/

using UnityEngine;
using UnityEngine.InputSystem;

namespace SnakeEnchanter.Player
{
    /// <summary>
    /// Controls player movement using CharacterController with Cinemachine camera.
    /// Cinemachine handles position + yaw, PlayerController handles pitch (vertical look).
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        #region Movement Settings

        [Header("Movement")]
        [Tooltip("Bewegungsgeschwindigkeit (m/s)")]
        [SerializeField] private float _moveSpeed = 0.1f;

        [Tooltip("Gravitationskraft (negativ = nach unten)")]
        [SerializeField] private float _gravity = -9.81f;

        [Header("Crouch")]
        [Tooltip("CharacterController Höhe beim Ducken")]
        [SerializeField] private float _crouchHeight = 1.0f;

        [Tooltip("Bewegungsgeschwindigkeit beim Ducken")]
        [SerializeField] private float _crouchSpeed = 0.5f;

        [Tooltip("Geschwindigkeit der Crouch-Transition (höher = schneller)")]
        [SerializeField] private float _crouchTransitionSpeed = 8f;

        #endregion

        #region Camera Settings

        [Header("Camera - Cinemachine Compatible")]
        [Tooltip("Main Camera (optional - wird automatisch als Camera.main gefunden)")]
        [SerializeField] private Camera _playerCamera;

        [Tooltip("Maus-Empfindlichkeit (höher = schneller)")]
        [SerializeField] private float _mouseSensitivity = 0.1f;

        [Tooltip("Minimaler Pitch-Winkel (nach unten schauen, negativ)")]
        [SerializeField] private float _minPitch = -70f;

        [Tooltip("Maximaler Pitch-Winkel (nach oben schauen, positiv)")]
        [SerializeField] private float _maxPitch = 70f;

        #endregion

        #region Animation

        [Header("Animation")]
        [Tooltip("Animator component auf dem Player-Modell")]
        [SerializeField] private Animator _animator;

        // Animation parameter hashes (performance optimization)
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int IsCrouchingHash = Animator.StringToHash("IsCrouching");

        #endregion

        #region Input Settings

        [Header("Input")]
        [Tooltip("Input Action Asset (SnakeEnchanter)")]
        [SerializeField] private InputActionAsset _inputActions;

        #endregion

        #region Private Fields

        // CharacterController
        private CharacterController _controller;
        private Vector3 _velocity;
        private bool _isGrounded;

        // Camera control
        private float _cameraPitch = 0f;

        // Input System
        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _crouchAction;
        private Vector2 _moveInput;
        private Vector2 _lookInput;

        // Crouch state
        private bool _isCrouching;
        private float _standingHeight;
        private Vector3 _standingCenter;
        private Vector3 _standingCamLocalPos;
        private Vector3 _crouchCamLocalPos;
        private bool _cameraIsChild; // Cache: Ist Camera Child dieses GameObjects?

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();

            // Auto-find Animator if not assigned
            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>();
            }

            // Auto-find Camera if not assigned (Cinemachine mode)
            if (_playerCamera == null)
            {
                _playerCamera = Camera.main;

                if (_playerCamera == null)
                {
                    Debug.LogWarning("PlayerController: Camera.main nicht gefunden! Mouse Look wird nicht funktionieren.", this);
                }
                else
                {
                    Debug.Log("PlayerController: Camera.main automatisch gefunden (Cinemachine Mode).", this);
                }
            }

            // Check if camera is child (affects crouch camera position handling)
            _cameraIsChild = _playerCamera != null && _playerCamera.transform.IsChildOf(transform);

            // Lock cursor for gameplay
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Store standing dimensions from Inspector/Hierarchy
            _standingHeight = _controller.height;
            _standingCenter = _controller.center;

            // Camera position handling (only if camera is child)
            if (_cameraIsChild)
            {
                _standingCamLocalPos = _playerCamera.transform.localPosition;

                // Crouch camera: same X/Z, Y drops by collider height difference
                float heightDiff = _standingHeight - _crouchHeight;
                _crouchCamLocalPos = _standingCamLocalPos - new Vector3(0f, heightDiff, 0f);
            }
            else
            {
                // Cinemachine mode: no local position handling needed
                _standingCamLocalPos = Vector3.zero;
                _crouchCamLocalPos = Vector3.zero;
            }

            SetupInputActions();
        }

        private void OnEnable()
        {
            EnableInput();
        }

        private void OnDisable()
        {
            DisableInput();
        }

        private void Update()
        {
            HandleGroundCheck();
            HandleCrouch();
            HandleMovementAndGravity();
            UpdateAnimations();
        }

        private void LateUpdate()
        {
            // Camera Look MUSS in LateUpdate sein, NACH Cinemachine!
            // Cinemachine setzt Yaw (horizontal rotation), wir setzen Pitch (vertical)
            HandleCameraLook();
        }

        #endregion

        #region Input System Setup

        /// <summary>
        /// Lädt Input Action Asset und findet Player Action Map.
        /// </summary>
        private void SetupInputActions()
        {
            if (_inputActions == null)
            {
                _inputActions = Resources.Load<InputActionAsset>("SnakeEnchanter");
            }

            if (_inputActions != null)
            {
                var playerMap = _inputActions.FindActionMap("Player");
                if (playerMap != null)
                {
                    _moveAction = playerMap.FindAction("Move");
                    _lookAction = playerMap.FindAction("Look");
                    _crouchAction = playerMap.FindAction("Crouch");
                }
                else
                {
                    Debug.LogError("PlayerController: 'Player' action map not found in InputActionAsset!", this);
                }
            }
            else
            {
                Debug.LogError("PlayerController: InputActionAsset 'SnakeEnchanter' nicht gefunden!", this);
            }
        }

        private void EnableInput()
        {
            if (_moveAction != null)
            {
                _moveAction.Enable();
                _moveAction.performed += OnMove;
                _moveAction.canceled += OnMove;
            }

            if (_lookAction != null)
            {
                _lookAction.Enable();
                _lookAction.performed += OnLook;
                _lookAction.canceled += OnLook;
            }

            if (_crouchAction != null)
            {
                _crouchAction.Enable();
                _crouchAction.started += OnCrouchStarted;
                _crouchAction.canceled += OnCrouchCanceled;
            }
        }

        private void DisableInput()
        {
            if (_moveAction != null)
            {
                _moveAction.performed -= OnMove;
                _moveAction.canceled -= OnMove;
                _moveAction.Disable();
            }

            if (_lookAction != null)
            {
                _lookAction.performed -= OnLook;
                _lookAction.canceled -= OnLook;
                _lookAction.Disable();
            }

            if (_crouchAction != null)
            {
                _crouchAction.started -= OnCrouchStarted;
                _crouchAction.canceled -= OnCrouchCanceled;
                _crouchAction.Disable();
            }
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            _lookInput = context.ReadValue<Vector2>();
        }

        private void OnCrouchStarted(InputAction.CallbackContext context)
        {
            _isCrouching = true;
        }

        private void OnCrouchCanceled(InputAction.CallbackContext context)
        {
            _isCrouching = false;
        }

        #endregion

        #region Crouch

        /// <summary>
        /// Smooth transition zwischen Stehen und Ducken.
        /// CharacterController Höhe + Center werden angepasst.
        /// Camera localPosition nur wenn Camera Child ist (nicht Cinemachine).
        /// </summary>
        private void HandleCrouch()
        {
            float targetHeight = _isCrouching ? _crouchHeight : _standingHeight;
            float heightDiff = _standingHeight - targetHeight;
            Vector3 targetCenter = new Vector3(
                _standingCenter.x,
                _standingCenter.y - heightDiff / 2f,
                _standingCenter.z);
            Vector3 targetCamPos = _isCrouching ? _crouchCamLocalPos : _standingCamLocalPos;

            // Skip if already at target
            if (Mathf.Abs(_controller.height - targetHeight) < 0.001f)
            {
                _controller.height = targetHeight;
                _controller.center = targetCenter;

                // Only update camera position if camera is child (not Cinemachine)
                if (_cameraIsChild)
                {
                    _playerCamera.transform.localPosition = targetCamPos;
                }
                return;
            }

            // Smooth transition
            float t = _crouchTransitionSpeed * Time.deltaTime;
            _controller.height = Mathf.Lerp(_controller.height, targetHeight, t);
            _controller.center = Vector3.Lerp(_controller.center, targetCenter, t);

            // Only update camera position if camera is child (not Cinemachine)
            if (_cameraIsChild)
            {
                _playerCamera.transform.localPosition = Vector3.Lerp(
                    _playerCamera.transform.localPosition, targetCamPos, t);
            }
        }

        #endregion

        #region Movement

        /// <summary>
        /// Prüft ob der Player auf dem Boden steht.
        /// </summary>
        private void HandleGroundCheck()
        {
            _isGrounded = _controller.isGrounded;

            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }
        }

        /// <summary>
        /// Kombiniert horizontale Bewegung (WASD relativ zur Kamera) mit Gravity
        /// in einem einzigen CharacterController.Move() Aufruf.
        /// WICHTIG: Nur EIN Move() pro Frame, damit _controller.velocity korrekt ist!
        /// </summary>
        private void HandleMovementAndGravity()
        {
            // --- Horizontal Movement ---
            Vector3 horizontalMove = Vector3.zero;

            if (_playerCamera != null)
            {
                Vector3 forward = _playerCamera.transform.forward;
                Vector3 right = _playerCamera.transform.right;

                forward.y = 0f;
                right.y = 0f;
                forward.Normalize();
                right.Normalize();

                Vector3 moveDirection = right * _moveInput.x + forward * _moveInput.y;

                if (moveDirection.sqrMagnitude > 0.01f)
                {
                    moveDirection.Normalize();
                    float currentSpeed = _isCrouching ? _crouchSpeed : _moveSpeed;
                    horizontalMove = moveDirection * currentSpeed * Time.deltaTime;
                }
            }

            // --- Gravity ---
            _velocity.y += _gravity * Time.deltaTime;
            _velocity.y = Mathf.Max(_velocity.y, -20f);

            // --- Single Move() call: horizontal + vertical combined ---
            Vector3 finalMove = horizontalMove + _velocity * Time.deltaTime;
            _controller.Move(finalMove);
        }

        #endregion

        #region Camera

        /// <summary>
        /// Mouse Look - Cinemachine-kompatibel.
        /// Verantwortlichkeiten:
        /// - Player Body Rotation (Yaw/Y-Achse): Dieser Code (Mouse X)
        /// - Camera Yaw: Cinemachine "Rotate With Follow Target" folgt Player automatisch
        /// - Camera Pitch (X-Achse): Dieser Code (Mouse Y)
        /// - Camera Position: Cinemachine Follow (folgt CameraTarget)
        /// </summary>
        private void HandleCameraLook()
        {
            if (_playerCamera == null) return;

            float mouseX = _lookInput.x * _mouseSensitivity;
            float mouseY = _lookInput.y * _mouseSensitivity;

            // Rotate player body around Y-axis (horizontal mouse = yaw)
            // Cinemachine "Rotate With Follow Target" folgt diesem automatisch
            transform.Rotate(Vector3.up * mouseX);

            // Calculate vertical look angle (pitch)
            _cameraPitch -= mouseY;
            _cameraPitch = Mathf.Clamp(_cameraPitch, _minPitch, _maxPitch);

            // Apply rotation to Main Camera (world space)
            // X-axis: Pitch (wir setzen direkt)
            // Y-axis: Yaw (Cinemachine steuert via "Rotate With Follow Target")
            // Z-axis: Roll (immer 0)
            _playerCamera.transform.rotation = Quaternion.Euler(
                _cameraPitch,              // X: Pitch (vertical look)
                transform.eulerAngles.y,   // Y: Match player yaw (Cinemachine folgt dem)
                0f                         // Z: No roll
            );
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Aktiviert/Deaktiviert Bewegung und Input.
        /// </summary>
        public void SetMovementEnabled(bool enabled)
        {
            this.enabled = enabled;
        }

        /// <summary>
        /// Entsperrt Cursor (z.B. für Menüs).
        /// </summary>
        public void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        /// <summary>
        /// Sperrt Cursor für Gameplay.
        /// </summary>
        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        #endregion

        #region Animation

        /// <summary>
        /// Aktualisiert Animator-Parameter basierend auf Bewegungs-State.
        /// </summary>
        private void UpdateAnimations()
        {
            if (_animator == null) return;

            // Calculate horizontal speed (ignore Y axis)
            Vector3 horizontalVelocity = _controller.velocity;
            horizontalVelocity.y = 0f;
            float speed = horizontalVelocity.magnitude;

            // Update animator parameters
            _animator.SetFloat(SpeedHash, speed);
            _animator.SetBool(IsCrouchingHash, _isCrouching);
        }

        #endregion
    }
}
