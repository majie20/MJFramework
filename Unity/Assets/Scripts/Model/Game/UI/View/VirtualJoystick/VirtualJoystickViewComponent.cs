using UnityEngine;
using UnityEngine.UI;

namespace Model
{
    [LifeCycle]
    [UIBaseData(UIViewType = (int)UIViewType.Pop, PrefabPath = "Assets/Res/NoBuildAB/Prefabs/UI/VirtualJoystick/VirtualJoystickView", UIMaskMode = (int)UIMaskMode.TransparentPenetrate)]
    public class VirtualJoystickViewComponent : UIBaseComponent, IOpen, IAwake, IUpdateSystem, ILateUpdateSystem
    {
        private CanvasGroup Buttons;
        private CanvasGroup Joystick;
        private CanvasGroup Arrows;
        private CanvasGroup JoystickRepositionable;

        private InputManager InputManager;

        private float _initialJoystickAlpha;
        private float _initialButtonsAlpha;
        public override void Awake()
        {
            base.Awake();
            ReferenceCollector rc = this.Entity.GameObject.GetComponent<ReferenceCollector>();
            Buttons = rc.Get<GameObject>("Buttons").GetComponent<CanvasGroup>();
            Joystick = rc.Get<GameObject>("Joystick").GetComponent<CanvasGroup>();
            Arrows = rc.Get<GameObject>("Arrows").GetComponent<CanvasGroup>();
            JoystickRepositionable = rc.Get<GameObject>("JoystickRepositionable").GetComponent<CanvasGroup>();

            InputManager = this.Entity.Transform.GetComponent<InputManager>();

            if (Joystick != null)
            {
                _initialJoystickAlpha = Joystick.alpha;
            }
            if (Buttons != null)
            {
                _initialButtonsAlpha = Buttons.alpha;
            }

            var isMobile = false;
            var movementControl = InputManager.MovementControl;
            var inputForcedMode = InputManager.InputForcedMode;
            SetMobileControlsActive(false);
            if (InputManager.AutoMobileDetection)
            {
#if UNITY_ANDROID || UNITY_IPHONE
                SetMobileControlsActive(true, movementControl);
                isMobile = true;
#endif
            }
            if (inputForcedMode == InputManager.InputForcedModes.Mobile)
            {
                SetMobileControlsActive(true, movementControl);
                isMobile = true;
            }
            else if (inputForcedMode == InputManager.InputForcedModes.Desktop)
            {
                SetMobileControlsActive(false);
                isMobile = false;
            }
#if UNITY_EDITOR
            if (InputManager.HideMobileControlsInEditor)
            {
                SetMobileControlsActive(false);
                isMobile = false;
            }
#endif
            InputManager.IsMobile = isMobile;

            InputManager.InitializeAxis();
        }

        public override void Dispose()
        {
            base.Dispose();
            Buttons = null;
            Joystick = null;
            Arrows = null;
            InputManager = null;
        }

        public void OnUpdate(float tick)
        {
            InputManager.Refresh();
        }

        public void OnLateUpdate()
        {
            InputManager.LateRefresh();
        }

        public void Open()
        {
            OnOpen();

        }

        protected override void OnOpen()
        {
            base.OnOpen();
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        public void SetMobileControlsActive(bool state, InputManager.MovementControls movementControl = InputManager.MovementControls.Joystick)
        {
            if (Joystick != null)
            {
                Joystick.gameObject.SetActive(state);
                if (state && movementControl == InputManager.MovementControls.Joystick)
                {
                    Joystick.alpha = _initialJoystickAlpha;
                }
                else
                {
                    Joystick.alpha = 0;
                    Joystick.gameObject.SetActive(false);
                }
            }

            if (Arrows != null)
            {
                Arrows.gameObject.SetActive(state);
                if (state && movementControl == InputManager.MovementControls.Arrows)
                {
                    Arrows.alpha = _initialJoystickAlpha;
                }
                else
                {
                    Arrows.alpha = 0;
                    Arrows.gameObject.SetActive(false);
                }
            }

            if (Buttons != null)
            {
                Buttons.gameObject.SetActive(state);
                if (state)
                {
                    Buttons.alpha = _initialButtonsAlpha;
                }
                else
                {
                    Buttons.alpha = 0;
                    Buttons.gameObject.SetActive(false);
                }
            }
        }
    }
}