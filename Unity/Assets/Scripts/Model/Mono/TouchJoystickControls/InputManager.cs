using UnityEngine;

namespace Model
{
    public class ButtonPressedFirstTimeJump : EventBase { }

    public class ButtonReleasedJump : EventBase { }

    public class ButtonPressedJump : EventBase { }

    public class ButtonPressedFirstTimeDash : EventBase { }

    public class ButtonReleasedDash : EventBase { }

    public class ButtonPressedDash : EventBase { }

    public class ButtonPressedFirstTimeCrouch : EventBase { }

    public class ButtonReleasedCrouch : EventBase { }

    public class ButtonPressedCrouch : EventBase { }

    public class ButtonPressedFirstTimeRun : EventBase { }

    public class ButtonReleasedRun : EventBase { }

    public class ButtonPressedRun : EventBase { }

    public class ButtonPressedFirstTimeInteract : EventBase { }

    public class ButtonReleasedInteract : EventBase { }

    public class ButtonPressedInteract : EventBase { }

    public class InputManager : MonoBehaviour
    {
        public enum InputForcedModes { None, Mobile, Desktop }

        /// the possible kinds of control used for movement
        public enum MovementControls { Joystick, Arrows }

        public bool InputDetectionActive = true;

        public string PlayerID = "Player1";

        public bool AutoMobileDetection = true;

        public InputForcedModes InputForcedMode;

        public bool HideMobileControlsInEditor = false;

        public MovementControls MovementControl = MovementControls.Joystick;

        public bool IsMobile { get; set; }

        public bool SmoothMovement = true;

        public Vector2 Threshold = new Vector2(0.1f, 0.4f);

        /// the primary movement value (used to move the character around)
        public Vector2 PrimaryMovement { get { return _primaryMovement; } }

        /// the secondary movement (usually the right stick on a gamepad), used to aim
        public Vector2 SecondaryMovement { get { return _secondaryMovement; } }

        /// the primary movement value (used to move the character around)
        public Vector2 LastNonNullPrimaryMovement { get; set; }

        /// the secondary movement (usually the right stick on a gamepad), used to aim
        public Vector2 LastNonNullSecondaryMovement { get; set; }

        /// the camera rotation axis input value
        public float CameraRotationInput { get { return _cameraRotationInput; } }

        //private Camera _targetCamera;
        //private bool _camera3D;
        private Vector2 _primaryMovement = Vector2.zero;
        private Vector2 _secondaryMovement = Vector2.zero;
        private float _cameraRotationInput = 0f;

        private string _axisHorizontal;
        private string _axisVertical;
        private string _axisSecondaryHorizontal;
        private string _axisSecondaryVertical;
        //private string _axisShoot;
        //private string _axisShootSecondary;
        private string _axisCamera;

        private string _buttonJump;
        private string _buttonRun;
        private string _buttonInteract;
        private string _buttonDash;
        private string _buttonCrouch;

        private bool _isButtonDownJump;
        private bool _isButtonDownRun;
        private bool _isButtonDownInteract;
        private bool _isButtonDownDash;
        private bool _isButtonDownCrouch;


        /// <summary>
        /// Initializes the axis strings.
        /// </summary>
        public virtual void InitializeAxis()
        {
            _axisHorizontal = PlayerID + "_Horizontal";
            _axisVertical = PlayerID + "_Vertical";
            _axisSecondaryHorizontal = PlayerID + "_SecondaryHorizontal";
            _axisSecondaryVertical = PlayerID + "_SecondaryVertical";
            //_axisShoot = PlayerID + "_ShootAxis";
            //_axisShootSecondary = PlayerID + "_SecondaryShootAxis";
            _axisCamera = PlayerID + "_CameraRotationAxis";

            _buttonJump = PlayerID + "_Jump";
            _buttonRun = PlayerID + "_Run";
            _buttonInteract = PlayerID + "_Interact";
            _buttonDash = PlayerID + "_Dash";
            _buttonCrouch = PlayerID + "_Crouch";
        }

        ///// <summary>
        ///// Sets an associated camera, used to rotate input based on camera position
        ///// </summary>
        ///// <param name="targetCamera"></param>
        ///// <param name="rotationAxis"></param>
        //public virtual void SetCamera(Camera targetCamera, bool camera3D)
        //{
        //    _targetCamera = targetCamera;
        //    _camera3D = camera3D;
        //}

        /// <summary>
        /// At update, we check the various commands and update our values and states accordingly.
        /// </summary>
        public virtual void Refresh()
        {
            if (!IsMobile && InputDetectionActive)
            {
                SetMovement();
                SetSecondaryMovement();
                SetCameraRotationAxis();
                GetInputButtons();
                GetLastNonNullValues();
            }
        }

        /// <summary>
        /// On LateUpdate, we process our button states
        /// </summary>
        public virtual void LateRefresh()
        {
            ProcessButtonStates();
        }

        /// <summary>
        /// Gets the last non null values for both primary and secondary axis
        /// </summary>
        public virtual void GetLastNonNullValues()
        {
            if (_primaryMovement.magnitude > Threshold.x)
            {
                LastNonNullPrimaryMovement = _primaryMovement;
            }
            if (_secondaryMovement.magnitude > Threshold.x)
            {
                LastNonNullSecondaryMovement = _secondaryMovement;
            }
        }

        /// <summary>
        /// If we're not on mobile, watches for input changes, and updates our buttons states accordingly
        /// </summary>
        public virtual void GetInputButtons()
        {
            if (Input.GetButton(_buttonJump))
            {
                Game.Instance.EventSystem.Invoke<ButtonPressedJump>();
            }
            if (Input.GetButtonDown(_buttonJump))
            {
                Game.Instance.EventSystem.Invoke<ButtonPressedFirstTimeJump>();
            }
            if (Input.GetButtonUp(_buttonJump))
            {
                Game.Instance.EventSystem.Invoke<ButtonReleasedJump>();
            }

            if (Input.GetButton(_buttonDash))
            {
                Game.Instance.EventSystem.Invoke<ButtonPressedDash>();
            }
            if (Input.GetButtonDown(_buttonDash))
            {
                Game.Instance.EventSystem.Invoke<ButtonPressedFirstTimeDash>();
            }
            if (Input.GetButtonUp(_buttonDash))
            {
                Game.Instance.EventSystem.Invoke<ButtonReleasedDash>();
            }

            if (Input.GetButton(_buttonCrouch))
            {
                Game.Instance.EventSystem.Invoke<ButtonPressedCrouch>();
            }
            if (Input.GetButtonDown(_buttonCrouch))
            {
                Game.Instance.EventSystem.Invoke<ButtonPressedFirstTimeCrouch>();
            }
            if (Input.GetButtonUp(_buttonCrouch))
            {
                Game.Instance.EventSystem.Invoke<ButtonReleasedCrouch>();
            }

            if (Input.GetButton(_buttonRun))
            {
                Game.Instance.EventSystem.Invoke<ButtonPressedRun>();
            }
            if (Input.GetButtonDown(_buttonRun))
            {
                Game.Instance.EventSystem.Invoke<ButtonPressedFirstTimeRun>();
            }
            if (Input.GetButtonUp(_buttonRun))
            {
                Game.Instance.EventSystem.Invoke<ButtonReleasedRun>();
            }

            if (Input.GetButton(_buttonInteract))
            {
                Game.Instance.EventSystem.Invoke<ButtonPressedInteract>();
            }
            if (Input.GetButtonDown(_buttonInteract))
            {
                Game.Instance.EventSystem.Invoke<ButtonPressedFirstTimeInteract>();
            }
            if (Input.GetButtonUp(_buttonInteract))
            {
                Game.Instance.EventSystem.Invoke<ButtonReleasedInteract>();
            }
        }

        /// <summary>
        /// Called at LateUpdate(), this method processes the button states of all registered buttons
        /// </summary>
        public virtual void ProcessButtonStates()
        {
            // for each button, if we were at ButtonDown this frame, we go to ButtonPressed. If we were at ButtonUp, we're now Off
            if (_isButtonDownJump)
            {
                Game.Instance.EventSystem.Invoke<ButtonPressedJump>();
                _isButtonDownJump = false;
            }
            if (_isButtonDownDash)
            {
                Game.Instance.EventSystem.Invoke<ButtonPressedDash>();
                _isButtonDownDash = false;
            }
            if (_isButtonDownCrouch)
            {
                Game.Instance.EventSystem.Invoke<ButtonPressedCrouch>();
                _isButtonDownCrouch = false;
            }
            if (_isButtonDownRun)
            {
                Game.Instance.EventSystem.Invoke<ButtonPressedRun>();
                _isButtonDownRun = false;
            }
            if (_isButtonDownInteract)
            {
                Game.Instance.EventSystem.Invoke<ButtonPressedInteract>();
                _isButtonDownInteract = false;
            }
        }

        #region 直接调用

        /// <summary>
        /// Called every frame, if not on mobile, gets primary movement values from input
        /// </summary>
        public virtual void SetMovement()
        {
            if (!IsMobile && InputDetectionActive)
            {
                if (SmoothMovement)
                {
                    _primaryMovement.x = Input.GetAxis(_axisHorizontal);
                    _primaryMovement.y = Input.GetAxis(_axisVertical);
                }
                else
                {
                    _primaryMovement.x = Input.GetAxisRaw(_axisHorizontal);
                    _primaryMovement.y = Input.GetAxisRaw(_axisVertical);
                }
            }
        }

        /// <summary>
        /// Called every frame, if not on mobile, gets secondary movement values from input
        /// </summary>
        public virtual void SetSecondaryMovement()
        {
            if (!IsMobile && InputDetectionActive)
            {
                if (SmoothMovement)
                {
                    _secondaryMovement.x = Input.GetAxis(_axisSecondaryHorizontal);
                    _secondaryMovement.y = Input.GetAxis(_axisSecondaryVertical);
                }
                else
                {
                    _secondaryMovement.x = Input.GetAxisRaw(_axisSecondaryHorizontal);
                    _secondaryMovement.y = Input.GetAxisRaw(_axisSecondaryVertical);
                }
            }
        }

        /// <summary>
        /// Grabs camera rotation input and stores it
        /// </summary>
        public virtual void SetCameraRotationAxis()
        {
            _cameraRotationInput = Input.GetAxis(_axisCamera);
        }

        #endregion 直接调用

        #region 事件调用

        /// <summary>
        /// If you're using a touch joystick, bind your main joystick to this method
        /// </summary>
        /// <param name="movement">Movement.</param>
        public virtual void SetMovement(Vector2 movement)
        {
            if (IsMobile && InputDetectionActive)
            {
                _primaryMovement.x = movement.x;
                _primaryMovement.y = movement.y;
            }
        }

        /// <summary>
        /// If you're using a touch joystick, bind your secondary joystick to this method
        /// </summary>
        /// <param name="movement">Movement.</param>
        public virtual void SetSecondaryMovement(Vector2 movement)
        {
            if (IsMobile && InputDetectionActive)
            {
                _secondaryMovement.x = movement.x;
                _secondaryMovement.y = movement.y;
            }
        }

        /// <summary>
        /// If you're using touch arrows, bind your left/right arrows to this method
        /// </summary>
        /// <param name="">.</param>
        public virtual void SetHorizontalMovement(float horizontalInput)
        {
            if (IsMobile && InputDetectionActive)
            {
                _primaryMovement.x = horizontalInput;
            }
        }

        /// <summary>
        /// If you're using touch arrows, bind your secondary down/up arrows to this method
        /// </summary>
        /// <param name="">.</param>
        public virtual void SetVerticalMovement(float verticalInput)
        {
            if (IsMobile && InputDetectionActive)
            {
                _primaryMovement.y = verticalInput;
            }
        }

        /// <summary>
        /// If you're using touch arrows, bind your secondary left/right arrows to this method
        /// </summary>
        /// <param name="">.</param>
        public virtual void SetSecondaryHorizontalMovement(float horizontalInput)
        {
            if (IsMobile && InputDetectionActive)
            {
                _secondaryMovement.x = horizontalInput;
            }
        }

        /// <summary>
        /// If you're using touch arrows, bind your down/up arrows to this method
        /// </summary>
        /// <param name="">.</param>
        public virtual void SetSecondaryVerticalMovement(float verticalInput)
        {
            if (IsMobile && InputDetectionActive)
            {
                _secondaryMovement.y = verticalInput;
            }
        }

        #endregion 事件调用

        #region 按键调用

        public virtual void JumpButtonDown()
        {
            Game.Instance.EventSystem.Invoke<ButtonPressedFirstTimeJump>();
            _isButtonDownJump = true;
        }

        public virtual void JumpButtonPressed()
        { Game.Instance.EventSystem.Invoke<ButtonPressedJump>(); }

        public virtual void JumpButtonUp()
        {
            Game.Instance.EventSystem.Invoke<ButtonReleasedJump>();
            _isButtonDownJump = false;
        }

        public virtual void DashButtonDown()
        {
            Game.Instance.EventSystem.Invoke<ButtonPressedFirstTimeDash>();
            _isButtonDownDash = true;
        }

        public virtual void DashButtonPressed()
        { Game.Instance.EventSystem.Invoke<ButtonPressedDash>(); }

        public virtual void DashButtonUp()
        {
            Game.Instance.EventSystem.Invoke<ButtonReleasedDash>();
            _isButtonDownDash = false;
        }

        public virtual void CrouchButtonDown()
        {
            Game.Instance.EventSystem.Invoke<ButtonPressedFirstTimeCrouch>();
            _isButtonDownCrouch = true;
        }

        public virtual void CrouchButtonPressed()
        { Game.Instance.EventSystem.Invoke<ButtonPressedCrouch>(); }

        public virtual void CrouchButtonUp()
        {
            Game.Instance.EventSystem.Invoke<ButtonReleasedCrouch>();
            _isButtonDownCrouch = false;
        }

        public virtual void RunButtonDown()
        {
            Game.Instance.EventSystem.Invoke<ButtonPressedFirstTimeRun>();
            _isButtonDownRun = true;
        }

        public virtual void RunButtonPressed()
        { Game.Instance.EventSystem.Invoke<ButtonPressedRun>(); }

        public virtual void RunButtonUp()
        {
            Game.Instance.EventSystem.Invoke<ButtonReleasedRun>();
            _isButtonDownRun = false;
        }

        public virtual void InteractButtonDown()
        {
            Game.Instance.EventSystem.Invoke<ButtonPressedFirstTimeInteract>();
            _isButtonDownInteract = true;
        }

        public virtual void InteractButtonPressed()
        { Game.Instance.EventSystem.Invoke<ButtonPressedInteract>(); }

        public virtual void InteractButtonUp()
        {
            Game.Instance.EventSystem.Invoke<ButtonReleasedInteract>();
            _isButtonDownInteract = false;
        }

        #endregion 按键调用
    }
}