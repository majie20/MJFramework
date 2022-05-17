using Sirenix.OdinInspector;
using UnityEngine;

namespace Model
{
    public class InputManager : MonoBehaviour
    {
        public enum InputForcedModes
        { None, Mobile, Desktop }

        /// the possible kinds of control used for movement
        public enum MovementControls
        { Joystick, Arrows }

        [Tooltip("将此设置为 false 以防止 InputManager 读取输入")]
        public bool InputDetectionActive = true;

        [LabelText("玩家ID")]
        public string PlayerID = "Player1";

        [Tooltip("如果您选中自动移动检测，当您的构建目标是 Android 或 iOS 时，引擎将自动切换到移动控件。 您还可以使用下面的下拉菜单强制移动或桌面（键盘、游戏手柄）控件。\n" +
                 "请注意，如果您不需要移动控件和/或 GUI，此组件也可以单独工作，只需将其放在空的 GameObject 上 .")]
        public bool AutoMobileDetection = true;

        [Tooltip("使用它来强制桌面（键盘、键盘）或移动（触摸）模式")]
        public InputForcedModes InputForcedMode;

        [Tooltip("如果这是真的，移动控件将在编辑器模式下隐藏，无论当前构建目标还是强制模式")]
        public bool HideMobileControlsInEditor = false;

        [LabelText("摇杆样式")]
        public MovementControls MovementControl = MovementControls.Joystick;

        [Tooltip("停止摇杆输入时是平滑处理还是立即中断")]
        public bool SmoothMovement = true;

        [Tooltip("触发模拟控制器（例如操纵杆）上的运动所需达到的最小值（到摇杆圆心距离的平方）")]
        public float Threshold = 0.01f;

        public bool IsMobile { get; set; }

        /// the primary movement value (used to move the character around)
        public Vector2 PrimaryMovement => _primaryMovement;

        /// the primary movement value (used to move the character around)
        public Vector2 LastNonNullPrimaryMovement => _LastNonNullPrimaryMovement;

        /// the camera rotation axis input value
        public float CameraRotationInput => _cameraRotationInput;

        private Vector2 _primaryMovement = Vector2.zero;
        private Vector2 _LastNonNullPrimaryMovement = Vector2.zero;
        private float _cameraRotationInput = 0f;

        private string _axisHorizontal;
        private string _axisVertical;
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
            _axisCamera = PlayerID + "_CameraRotationAxis";

            _buttonJump = PlayerID + "_Jump";
            _buttonRun = PlayerID + "_Run";
            _buttonInteract = PlayerID + "_Interact";
            _buttonDash = PlayerID + "_Dash";
            _buttonCrouch = PlayerID + "_Crouch";
        }

        /// <summary>
        /// At update, we check the various commands and update our values and states accordingly.
        /// </summary>
        public virtual void Refresh()
        {
            if (!IsMobile && InputDetectionActive)
            {
                SetMovement();
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
        /// If we're not on mobile, watches for input changes, and updates our buttons states accordingly
        /// </summary>
        public virtual void GetInputButtons()
        {
            if (Input.GetButton(_buttonJump))
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedJump>();
            }
            if (Input.GetButtonDown(_buttonJump))
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedFirstTimeJump>();
            }
            if (Input.GetButtonUp(_buttonJump))
            {
                Game.Instance.EventSystem.Invoke<E_ButtonReleasedJump>();
            }

            if (Input.GetButton(_buttonDash))
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedDash>();
            }
            if (Input.GetButtonDown(_buttonDash))
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedFirstTimeDash>();
            }
            if (Input.GetButtonUp(_buttonDash))
            {
                Game.Instance.EventSystem.Invoke<E_ButtonReleasedDash>();
            }

            if (Input.GetButton(_buttonCrouch))
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedCrouch>();
            }
            if (Input.GetButtonDown(_buttonCrouch))
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedFirstTimeCrouch>();
            }
            if (Input.GetButtonUp(_buttonCrouch))
            {
                Game.Instance.EventSystem.Invoke<E_ButtonReleasedCrouch>();
            }

            if (Input.GetButton(_buttonRun))
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedRun>();
            }
            if (Input.GetButtonDown(_buttonRun))
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedFirstTimeRun>();
            }
            if (Input.GetButtonUp(_buttonRun))
            {
                Game.Instance.EventSystem.Invoke<E_ButtonReleasedRun>();
            }

            if (Input.GetButton(_buttonInteract))
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedInteract>();
            }
            if (Input.GetButtonDown(_buttonInteract))
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedFirstTimeInteract>();
            }
            if (Input.GetButtonUp(_buttonInteract))
            {
                Game.Instance.EventSystem.Invoke<E_ButtonReleasedInteract>();
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
                Game.Instance.EventSystem.Invoke<E_ButtonPressedJump>();
                _isButtonDownJump = false;
            }
            if (_isButtonDownDash)
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedDash>();
                _isButtonDownDash = false;
            }
            if (_isButtonDownCrouch)
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedCrouch>();
                _isButtonDownCrouch = false;
            }
            if (_isButtonDownRun)
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedRun>();
                _isButtonDownRun = false;
            }
            if (_isButtonDownInteract)
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedInteract>();
                _isButtonDownInteract = false;
            }
        }

        /// <summary>
        /// Gets the last non null values for both primary and secondary axis
        /// </summary>
        public virtual void GetLastNonNullValues()
        {
            if (_primaryMovement.sqrMagnitude > Threshold)
            {
                _LastNonNullPrimaryMovement = _primaryMovement;
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
                _primaryMovement = movement;
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

        #endregion 事件调用

        #region 按键调用

        #region 按键空格，摇杆按钮A

        public virtual void JumpButtonDown()
        {
            if (InputDetectionActive)
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedFirstTimeJump>();
                _isButtonDownJump = true;
            }
        }

        public virtual void JumpButtonPressed()
        {
            if (InputDetectionActive)
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedJump>();
            }
        }

        public virtual void JumpButtonUp()
        {
            if (InputDetectionActive)
            {
                Game.Instance.EventSystem.Invoke<E_ButtonReleasedJump>();
                _isButtonDownJump = false;
            }
        }

        #endregion 按键空格，摇杆按钮A

        #region 按键F，摇杆按钮RT

        public virtual void DashButtonDown()
        {
            if (InputDetectionActive)
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedFirstTimeDash>();
                _isButtonDownDash = true;
            }
        }

        public virtual void DashButtonPressed()
        {
            if (InputDetectionActive)
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedDash>();
            }
        }

        public virtual void DashButtonUp()
        {
            if (InputDetectionActive)
            {
                Game.Instance.EventSystem.Invoke<E_ButtonReleasedDash>();
                _isButtonDownDash = false;
            }
        }

        #endregion 按键F，摇杆按钮RT

        #region 按键C，摇杆按钮Y

        public virtual void CrouchButtonDown()
        {
            if (InputDetectionActive)
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedFirstTimeCrouch>();
                _isButtonDownCrouch = true;
            }
        }

        public virtual void CrouchButtonPressed()
        {
            if (InputDetectionActive)
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedCrouch>();
            }
        }

        public virtual void CrouchButtonUp()
        {
            if (InputDetectionActive)
            {
                Game.Instance.EventSystem.Invoke<E_ButtonReleasedCrouch>();
                _isButtonDownCrouch = false;
            }
        }

        #endregion 按键C，摇杆按钮Y

        #region 按键左Shift，摇杆按钮X

        public virtual void RunButtonDown()
        {
            if (InputDetectionActive)
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedFirstTimeRun>();
                _isButtonDownRun = true;
            }
        }

        public virtual void RunButtonPressed()
        {
            if (InputDetectionActive)
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedRun>();
            }
        }

        public virtual void RunButtonUp()
        {
            if (InputDetectionActive)
            {
                Game.Instance.EventSystem.Invoke<E_ButtonReleasedRun>();
                _isButtonDownRun = false;
            }
        }

        #endregion 按键左Shift，摇杆按钮X

        #region 按键E，摇杆按钮B

        public virtual void InteractButtonDown()
        {
            if (InputDetectionActive)
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedFirstTimeInteract>();
                _isButtonDownInteract = true;
            }
        }

        public virtual void InteractButtonPressed()
        {
            if (InputDetectionActive)
            {
                Game.Instance.EventSystem.Invoke<E_ButtonPressedInteract>();
            }
        }

        public virtual void InteractButtonUp()
        {
            if (InputDetectionActive)
            {
                Game.Instance.EventSystem.Invoke<E_ButtonReleasedInteract>();
                _isButtonDownInteract = false;
            }
        }

        #endregion 按键E，摇杆按钮B

        #endregion 按键调用
    }
}