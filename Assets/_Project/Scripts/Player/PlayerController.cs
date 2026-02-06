/*
====================================================================
* PlayerController - Handles player movement and camera control
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-03
* Version: 1.5 - HIERARCHY CAMERA

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

* SETUP:
* 1. Camera must be a CHILD of the Player GameObject in Hierarchy
* 2. Position the Camera in Scene View (Transform) — no code needed
* 3. Assign the Camera reference in Inspector
* 4. Script handles: rotation, movement, crouch — NOT camera position

* VERSION HISTORY:
* - v1.0: Initial with Legacy Input
* - v1.1: First-Person camera
* - v1.2: New Input System only
* - v1.3: Crouch system (hold LeftCtrl)
* - v1.4: Live camera offset (removed — caused override issues)
* - v1.5: Camera position via Hierarchy only, script never overrides
====================================================================
*/

using UnityEngine;
using UnityEngine.InputSystem;

namespace SnakeEnchanter.Player
{
    /// <summary>
    /// Controls player movement using CharacterController with first-person camera.
    /// Camera must be a child of the Player in the Hierarchy.
    /// Position the camera freely via Transform — this script only handles rotation.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        #region Movement Settings
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _gravity = -9.81f;

        [Header("Crouch")]
        [SerializeField] private float _crouchHeight = 1.0f;
        [SerializeField] private float _crouchSpeed = 2.5f;
        [SerializeField] private float _crouchTransitionSpeed = 8f;
        #endregion

        #region Camera Settings
        [Header("Camera - First Person")]
        [Tooltip("Assign the Camera that is a CHILD of this Player in the Hierarchy.")]
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private float _mouseSensitivity = 0.1f;
        [Tooltip("How far the player can look down (negative = down). Best practice: -70 to -80.")]
        [SerializeField] private float _minPitch = -70f;
        [Tooltip("How far the player can look up (positive = up). Best practice: 70 to 80.")]
        [SerializeField] private float _maxPitch = 70f;
        #endregion

        #region Animation
        [Header("Animation")]
        [Tooltip("Animator component on the player model")]
        [SerializeField] private Animator _animator;

        // Animation parameter hashes (performance optimization)
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int IsCrouchingHash = Animator.StringToHash("IsCrouching");
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
        private InputAction _crouchAction;
        private Vector2 _moveInput;
        private Vector2 _lookInput;

        // Crouch — stored at startup from Hierarchy/Inspector values
        private bool _isCrouching;
        private float _standingHeight;
        private Vector3 _standingCenter;
        private Vector3 _standingCamLocalPos;
        private Vector3 _crouchCamLocalPos;
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

            // Lock cursor for gameplay
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Store standing dimensions exactly as set in Inspector/Hierarchy
            _standingHeight = _controller.height;
            _standingCenter = _controller.center;

            // Camera position comes from Hierarchy — read it, never invent it
            if (_playerCamera != null)
            {
                _standingCamLocalPos = _playerCamera.transform.localPosition;

                // Crouch camera: same X/Z, Y drops by the same amount as the collider
                float heightDiff = _standingHeight - _crouchHeight;
                _crouchCamLocalPos = _standingCamLocalPos - new Vector3(0f, heightDiff, 0f);
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
            HandleMovement();
            HandleCameraLook();
            ApplyGravity();
            UpdateAnimations();
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
                    Debug.LogError("PlayerController: 'Player' action map not found!");
                }
            }
            else
            {
                Debug.LogError("PlayerController: InputActionAsset not assigned!");
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
        /// Smoothly transitions CharacterController height and camera between standing/crouching.
        /// Standing values come from Hierarchy at startup — never from SerializeField offsets.
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
                if (_playerCamera != null)
                {
                    _playerCamera.transform.localPosition = targetCamPos;
                }
                return;
            }

            // Smooth transition
            float t = _crouchTransitionSpeed * Time.deltaTime;
            _controller.height = Mathf.Lerp(_controller.height, targetHeight, t);
            _controller.center = Vector3.Lerp(_controller.center, targetCenter, t);

            if (_playerCamera != null)
            {
                _playerCamera.transform.localPosition = Vector3.Lerp(
                    _playerCamera.transform.localPosition, targetCamPos, t);
            }
        }
        #endregion

        #region Movement
        /// <summary>
        /// Handles movement input relative to camera facing direction.
        /// </summary>
        private void HandleMovement()
        {
            if (_playerCamera == null) return;

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
                _controller.Move(moveDirection * currentSpeed * Time.deltaTime);
            }
        }

        private void HandleGroundCheck()
        {
            _isGrounded = _controller.isGrounded;
            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }
        }

        private void ApplyGravity()
        {
            _velocity.y += _gravity * Time.deltaTime;

            // Clamp terminal velocity to prevent falling through floor
            _velocity.y = Mathf.Max(_velocity.y, -20f);

            _controller.Move(_velocity * Time.deltaTime);
        }
        #endregion

        #region Camera
        /// <summary>
        /// Mouse look: rotates player body (yaw) and camera (pitch).
        /// Never touches camera local position — only rotation.
        /// </summary>
        private void HandleCameraLook()
        {
            if (_playerCamera == null) return;

            float mouseX = _lookInput.x * _mouseSensitivity;
            float mouseY = _lookInput.y * _mouseSensitivity;

            transform.Rotate(Vector3.up * mouseX);

            _cameraPitch -= mouseY;
            _cameraPitch = Mathf.Clamp(_cameraPitch, _minPitch, _maxPitch);
            _playerCamera.transform.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);
        }
        #endregion

        #region Public Methods
        public void SetMovementEnabled(bool enabled)
        {
            this.enabled = enabled;
        }

        public void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        #endregion

        #region Animation
        /// <summary>
        /// Updates animator parameters based on current movement state.
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

