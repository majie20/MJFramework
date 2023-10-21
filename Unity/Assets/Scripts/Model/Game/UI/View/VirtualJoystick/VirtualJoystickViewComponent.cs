using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    [UIBaseData(UIViewType = (int)UIViewType.Pop, PrefabPath = "Assets/Res/UI/Prefab/VirtualJoystick/VirtualJoystickView.prefab", UIMaskMode = (int)UIMaskMode.BlackTransparent,
        UILayer = (int)UIViewLayer.Low, IsOperateMask = false)]
    public class VirtualJoystickViewComponent : UIBaseComponent, IOpen
    {
        private CanvasGroup _buttons;
        private CanvasGroup _joystick;
        private CanvasGroup _arrows;

        private InputManager InputManager;

        private float _initialJoystickAlpha;
        private float _initialButtonsAlpha;

        public override void Awake()
        {
            base.Awake();
            ReferenceCollector rc = this.Entity.GameObject.GetComponent<ReferenceCollector>();
            _buttons = rc.Get<GameObject>("Buttons").GetComponent<CanvasGroup>();
            _joystick = rc.Get<GameObject>("Joystick").GetComponent<CanvasGroup>();
            _arrows = rc.Get<GameObject>("Arrows").GetComponent<CanvasGroup>();

            InputManager = this.Entity.Transform.GetComponent<InputManager>();

            if (_joystick != null)
            {
                _initialJoystickAlpha = _joystick.alpha;
            }

            if (_buttons != null)
            {
                _initialButtonsAlpha = _buttons.alpha;
            }

            var movementControl = InputManager.MovementControl;
            var inputForcedMode = InputManager.InputForcedMode;
            SetMobileControlsActive(false);

            if (InputManager.AutoMobileDetection)
            {
#if UNITY_ANDROID || UNITY_IPHONE
                SetMobileControlsActive(true, movementControl);
#endif
            }

            if (inputForcedMode == InputManager.InputForcedModes.Mobile)
            {
                SetMobileControlsActive(true, movementControl);
            }
            else if (inputForcedMode == InputManager.InputForcedModes.Desktop)
            {
                SetMobileControlsActive(false);
            }
#if UNITY_EDITOR
            if (InputManager.HideMobileControlsInEditor)
            {
                SetMobileControlsActive(false);
            }
#endif
        }

        public override void Dispose()
        {
            _buttons = null;
            _joystick = null;
            _arrows = null;
            InputManager = null;
            base.Dispose();
        }

        public override async UniTaskVoid OnLoadComplete()
        {
        }

        public void Open()
        {
            Game.Instance.GAddComponent(this);
        }

        protected override void OnClose()
        {
            Game.Instance.GRemoveComponent(this);
        }

        public void SetMobileControlsActive(bool state, InputManager.MovementControls movementControl = InputManager.MovementControls.Joystick)
        {
            if (_joystick != null)
            {
                _joystick.gameObject.SetActive(state);

                if (state && movementControl == InputManager.MovementControls.Joystick)
                {
                    _joystick.alpha = _initialJoystickAlpha;
                }
                else
                {
                    _joystick.alpha = 0;
                    _joystick.gameObject.SetActive(false);
                }
            }

            if (_arrows != null)
            {
                _arrows.gameObject.SetActive(state);

                if (state && movementControl == InputManager.MovementControls.Arrows)
                {
                    _arrows.alpha = _initialJoystickAlpha;
                }
                else
                {
                    _arrows.alpha = 0;
                    _arrows.gameObject.SetActive(false);
                }
            }

            if (_buttons != null)
            {
                _buttons.gameObject.SetActive(state);

                if (state)
                {
                    _buttons.alpha = _initialButtonsAlpha;
                }
                else
                {
                    _buttons.alpha = 0;
                    _buttons.gameObject.SetActive(false);
                }
            }
        }
    }
}