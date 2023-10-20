using Sirenix.OdinInspector;
using UnityEngine;

namespace Model
{
    public class InputManager : MonoBehaviour
    {
        public enum InputForcedModes
        {
            None,
            Mobile,
            Desktop
        }

        /// the possible kinds of control used for movement
        public enum MovementControls
        {
            Joystick,
            Arrows
        }

        [Tooltip("如果您选中自动移动检测，当您的构建目标是 Android 或 iOS 时，引擎将自动切换到移动控件。 您还可以使用下面的下拉菜单强制移动或桌面（键盘、游戏手柄）控件。\n" + "请注意，如果您不需要移动控件和/或 GUI，此组件也可以单独工作，只需将其放在空的 GameObject 上 .")]
        public bool AutoMobileDetection = true;

        [Tooltip("使用它来强制桌面（键盘、键盘）或移动（触摸）模式")]
        public InputForcedModes InputForcedMode;

        [Tooltip("如果这是真的，移动控件将在编辑器模式下隐藏，无论当前构建目标还是强制模式")]
        public bool HideMobileControlsInEditor = false;

        [LabelText("摇杆样式")]
        public MovementControls MovementControl = MovementControls.Joystick;
    }
}