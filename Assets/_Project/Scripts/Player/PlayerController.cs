/*
====================================================================
* PlayerController - Handles player movement and camera control
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-03
* Version: 1.2 - NEW INPUT SYSTEM

* ⚠️ WICHTIG: KOMMENTIERUNG NICHT LÖSCHEN! ⚠️
* Diese detaillierte Authorship-Dokumentation ist für die akademische
* Bewertung erforderlich und darf nicht entfernt werden!

* AUTHORSHIP CLASSIFICATION:

* [AI-ASSISTED]
* - Initial implementation structure
* - CharacterController movement logic
* - First-person camera system (v1.1)
* - New Input System migration (v1.2)
* - Human reviewed and will modify as needed

* DEPENDENCIES:
* - Unity CharacterController component
* - Unity New Input System (InputSystem package)
* - SnakeEnchanter.inputactions asset

* NOTES:
* - Phase 1 implementation - basic movement
* - First-person perspective (v1.1)
* - Camera offset adjustable for head height
* - v1.2: Migrated to New Input System (project rule)

* VERSION HISTORY:
* - v1.0: Initial with Legacy Input
* - v1.1: First-Person camera
* - v1.2: New Input System only
====================================================================
*/

using UnityEngine;
using UnityEngine.InputSystem;

namespace SnakeEnchanter.Player
{
    /// <summary>
    /// Controls player movement using CharacterController with first-person camera.
    /// Uses New Input System for WASD movement and mouse look.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        #region Movement Settings
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private float _groundCheckDistance = 0.1f;
        #endregion

        #region Camera Settings
        [Header("Camera - First Person")]
        [SerializeField] private Camera _playerCamera;
        [Tooltip("Height offset for camera (eye level)")]
        [SerializeField] private float _cameraHeight = 1.6f;
        [SerializeField] private float _mouseSensitivity = 0.1f;
        [SerializeField] private float _minPitch = -80f;
        [SerializeField] private float _maxPitch = 80f;
        #endregion

        #region Input Settings
        [Header("Input")]
        [SerializeField] private InputActionAsset _inputActions;
        #endregion

        #region Private Fields
        private CharacterController _controller;
        private Vector3 _velocity;
        private float _cameraPitch = 0f;
        private bool _isGrounded;

        // Input System
        private InputAction _moveAction;
        private InputAction _lookAction;
        private Vector2 _moveInput;
        private Vector2 _lookInput;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _controller = GetComponent<CharacterController>();

            // Lock cursor for gameplay
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Setup camera for first-person
            SetupFirstPersonCamera();

            // Setup Input System
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
            HandleMovement();
            HandleCameraLook();
            ApplyGravity();
        }
        #endregion

        #region Input System Setup
        /// <summary>
        /// Sets up Input System actions from the InputActionAsset.
        /// </summary>
        private void SetupInputActions()
        {
            if (_inputActions == null)
            {
                // Try to find the input actions asset
                _inputActions = Resources.Load<InputActionAsset>("SnakeEnchanter");
            }

            if (_inputActions != null)
            {
                var playerMap = _inputActions.FindActionMap("Player");
                if (playerMap != null)
                {
                    _moveAction = playerMap.FindAction("Move");
                    _lookAction = playerMap.FindAction("Look");
                }
                else
                {
                    Debug.LogError("PlayerController: 'Player' action map not found in InputActionAsset!");
                }
            }
            else
            {
                Debug.LogError("PlayerController: InputActionAsset not assigned! Assign SnakeEnchanter.inputactions in Inspector.");
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
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            _lookInput = context.ReadValue<Vector2>();
        }
        #endregion

        #region Camera Setup
        /// <summary>
        /// Configures camera for first-person view at eye height.
        /// </summary>
        private void SetupFirstPersonCamera()
        {
            // If no camera assigned, try to find main camera
            if (_playerCamera == null)
            {
                _playerCamera = Camera.main;
            }

            if (_playerCamera != null)
            {
                // Position camera at eye level
                _playerCamera.transform.SetParent(transform);
                _playerCamera.transform.localPosition = new Vector3(0f, _cameraHeight, 0f);
                _playerCamera.transform.localRotation = Quaternion.identity;
            }
            else
            {
                Debug.LogError("PlayerController: No camera found! Assign camera in Inspector.");
            }
        }
        #endregion

        #region Movement
        /// <summary>
        /// Handles movement input and moves the player.
        /// </summary>
        private void HandleMovement()
        {
            if (_playerCamera == null) return;

            // Calculate move direction relative to camera's forward direction
            Vector3 forward = _playerCamera.transform.forward;
            Vector3 right = _playerCamera.transform.right;

            // Flatten forward and right vectors (no vertical movement from looking up/down)
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 moveDirection = right * _moveInput.x + forward * _moveInput.y;

            // Only normalize if moving to prevent zero vector normalization
            if (moveDirection.sqrMagnitude > 0.01f)
            {
                moveDirection.Normalize();
                _controller.Move(moveDirection * _moveSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Checks if player is grounded using CharacterController.
        /// </summary>
        private void HandleGroundCheck()
        {
            _isGrounded = _controller.isGrounded;

            // Reset vertical velocity when grounded
            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f; // Small downward force to keep grounded
            }
        }

        /// <summary>
        /// Applies gravity to the player.
        /// </summary>
        private void ApplyGravity()
        {
            _velocity.y += _gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }
        #endregion

        #region Camera
        /// <summary>
        /// Handles mouse look for first-person camera rotation.
        /// Rotates entire player on Y-axis (yaw) and camera on X-axis (pitch).
        /// </summary>
        private void HandleCameraLook()
        {
            if (_playerCamera == null) return;

            // Use look input from New Input System
            float mouseX = _lookInput.x * _mouseSensitivity;
            float mouseY = _lookInput.y * _mouseSensitivity;

            // Rotate player body (yaw)
            transform.Rotate(Vector3.up * mouseX);

            // Rotate camera (pitch) with clamping
            _cameraPitch -= mouseY;
            _cameraPitch = Mathf.Clamp(_cameraPitch, _minPitch, _maxPitch);
            _playerCamera.transform.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Enables or disables player movement (for tune playing, cutscenes, etc.)
        /// </summary>
        public void SetMovementEnabled(bool enabled)
        {
            this.enabled = enabled;
        }

        /// <summary>
        /// Unlocks cursor (for menus, pause, etc.)
        /// </summary>
        public void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        /// <summary>
        /// Locks cursor (for gameplay)
        /// </summary>
        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        #endregion
    }
}
