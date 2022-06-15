using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public enum ETextColor
{
    C1 = 0xd20607,
}

public enum ETextSize
{
    S1 = 45,
}

public enum ETextOutLine
{
    None = 0,
    O1 = 0x644523,
}

public enum ETextShadow
{
    None = 0,
    D1 = 0x644523,
}
public class TextPicConifg : MonoBehaviour
{
    public ETextColor textColor = ETextColor.C1;
    public ETextSize textSize = ETextSize.S1;

    [Header("互斥选项，必须有一个为None")]
    public ETextOutLine outLine = ETextOutLine.None;
    public ETextShadow shadow = ETextShadow.None;

    private TextPic pic;

    public void Awake()
    {
        pic = GetComponent<TextPic>();
    }

    public void Start()
    {
        Apply();
    }

    [ContextMenu("Apply")]
    public void Apply()
    {
        pic = GetComponent<TextPic>();
        pic.fontSize = (int)textSize;
        Color tColor = new Color();
        string c = "#" + Convert.ToString((int)textColor, 16);
        ColorUtility.TryParseHtmlString(c, out tColor);
        pic.color = tColor;

        Outline lint = gameObject.GetComponent<Outline>();
        if (outLine == ETextOutLine.None)
        {
            if (lint)
            {
                DestroyImmediate(lint);
            }
        }
        else
        {
            if (lint == null)
            {
                lint = gameObject.AddComponent<Outline>();
            }
            Color olColor = new Color();
            c = "#" + Convert.ToString((int)outLine, 16);
            ColorUtility.TryParseHtmlString(c, out olColor);
            lint.effectColor = olColor;
        }

        Shadow sha = gameObject.GetComponent<Shadow>();
        if (shadow == ETextShadow.None)
        {
            if (sha)
            {
                DestroyImmediate(sha);
            }
        }
        else
        {
            if (sha == null)
            {
                sha = gameObject.AddComponent<Shadow>();
            }
            Color shaColor = new Color();
            c = "#" + Convert.ToString((int)shadow, 16);
            ColorUtility.TryParseHtmlString(c, out shaColor);
            sha.effectColor = shaColor;
        }
    }
}
