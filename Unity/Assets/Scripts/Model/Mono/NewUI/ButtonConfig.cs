using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class ButtonData
{
    public EButtonType buttonType;
    public ETextColor txtColor;
    public ETextSize txtSize;

    [Header("互斥选项，必须有一个为None")]
    public ETextOutLine txtOnline;
    public ETextShadow textShadow;
    public Vector2 ButtonSize;
    public Sprite spr;
}

[CreateAssetMenu(fileName = "ButtonConfig", menuName = "ButtonConfig")]
public class ButtonConfig : SerializedScriptableObject
{
    public List<ButtonData> buttonList;

    public ButtonData GetButtonData(EButtonType buttonType)
    {
        ButtonData buttonData = null;

        if (buttonList != null)
        {
            buttonData = buttonList.Find((ele) =>
            {
                if (ele.buttonType == buttonType)
                    return true;
                else
                    return false;
            });
        }

        return buttonData;
    }
}
