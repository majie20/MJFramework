using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public enum EButtonType
{
    B1,
}
public class ButtonStyle : MonoBehaviour
{
    public EButtonType buttonType;
    public ButtonConfig buttonConfig;
    private Button btn;
    private TextPicConifg txt;
    private Image img;
    private void Start()
    {
        Apply();
    }

    [ContextMenu("Apply")]
    public void Apply()
    {
        btn = GetComponent<Button>();
        img = GetComponent<Image>();
        txt = GetComponentInChildren<TextPicConifg>();
        ButtonData buttonData = buttonConfig.GetButtonData(buttonType);
        btn.GetComponent<RectTransform>().sizeDelta = buttonData.ButtonSize;
        img.sprite = buttonData.spr;
        txt.textColor = buttonData.txtColor;
        txt.textSize = buttonData.txtSize;
        txt.outLine = buttonData.txtOnline;
        img.type = Image.Type.Sliced;
        txt.Apply();
    }
}
