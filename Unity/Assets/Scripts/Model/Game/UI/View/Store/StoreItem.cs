using System.Collections;
using System.Collections.Generic;
using cfg;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

/// <summary>
/// 商店界面列表
/// </summary>
public class StoreItem
{
    public StoreItem(GameObject item, role data)
    {
        this.data = data;
        this.item = item;
    }

    private GameObject _item;

    private GameObject item
    {
        get { return _item; }
        set
        {
            _item = value;
            if (_item)
            {
                Init();
            }
        }
    }

    private role data;

    /// <summary>
    /// 状态
    /// </summary>
    private Text stage;

    /// <summary>
    /// 名称和介绍信息
    /// </summary>
    private TextPic name;

    /// <summary>
    /// 全身像
    /// </summary>
    private Image Icon;

    /// <summary>
    /// 点击
    /// </summary>
    private Button btn;

    void Init()
    {
        ReferenceCollector rc = item.GetComponent<ReferenceCollector>();
        stage = rc.Get<GameObject>("stage").GetComponent<Text>();
        stage.text = "未拥有";
        name = rc.Get<GameObject>("name").GetComponent<TextPic>();
        name.text = data.Name + "\n" + data.Info;
        Icon = rc.Get<GameObject>("Icon").GetComponent<Image>();
        Icon.sprite = null;

        btn = item.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            NLog.Log.Debug("——点击了" + data.Name);
        });
    }

}
