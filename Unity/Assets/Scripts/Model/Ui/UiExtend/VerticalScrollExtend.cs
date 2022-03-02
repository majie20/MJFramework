using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VerticalScrollExtend: MonoBehaviour
{
    public GameObject itemBox;
    public ScrollRect scroll;
    private int itemNum;
    private Dictionary<int, GameObject> boxDic = new Dictionary<int, GameObject>();
    private Queue<GameObject> poolQueue = new Queue<GameObject>();
    private RectTransform groupTransform;
    private float viewportHeight;
    private float itemHeight;
    private float rowNum;
    private int oldIndMin;
    private int oldIndMax;
    private System.Action<int, GameObject> SetBoxFunc;

    public void AddScrollListener(int num, System.Action<int, GameObject> SetBoxFunc)
    {
        this.SetBoxFunc = SetBoxFunc;
        this.itemNum = num;
        Init();
    }

    private void Init()
    {
        groupTransform = scroll.content.GetComponent<RectTransform>();
        viewportHeight = scroll.viewport.GetComponent<RectTransform>().rect.height;
        var boxTransfrom = itemBox.GetComponent<RectTransform>().rect;
        itemHeight = boxTransfrom.height + 20;
        rowNum = Mathf.FloorToInt(groupTransform.rect.width / (boxTransfrom.width + 20));
        groupTransform.sizeDelta = new Vector2(0, Mathf.CeilToInt(itemNum / rowNum) * itemHeight);
        Refresh(scroll.GetComponent<RectTransform>().anchoredPosition);
        scroll.onValueChanged.AddListener(Refresh);
    }

    public void Refresh(Vector2 pos)
    {
        var minIdx = (int)(Mathf.FloorToInt(groupTransform.anchoredPosition.y / itemHeight) * rowNum);
        var maxIdx = (int)Mathf.Min(itemNum - 1, minIdx + Mathf.CeilToInt(viewportHeight / itemHeight) * rowNum + (rowNum - 1));

        if(minIdx < 0)
        {
            return;
        }

        for(int i = oldIndMin; i < minIdx; i++)
        {
            if (boxDic.ContainsKey(i))
            {
                AddBox(boxDic[i]);
            }
            boxDic.Remove(i);
        }

        for(int i = maxIdx + 1; i<=oldIndMax; i++)
        {
            if (boxDic.ContainsKey(i))
            {
                AddBox(boxDic[i]);
            }
            boxDic.Remove(i);
        }

        oldIndMax = maxIdx;
        oldIndMin = minIdx;

        for (int i = minIdx; i <= maxIdx; i++)
        {
            if (boxDic.ContainsKey(i))
            {
                continue;
            }
            else
            {
                int idx = i;
                var curBox = GetBox();
                curBox.SetActive(true);
                curBox.transform.SetParent(groupTransform);
                curBox.transform.localPosition = new Vector2(idx % rowNum * itemHeight + itemHeight / 2, Mathf.CeilToInt(-idx / rowNum) * itemHeight - itemHeight / 2);
                SetBoxFunc(idx, curBox);
                boxDic.Add(i, curBox);
            }
        }
    }

    public void AddBox(GameObject box)
    {
        box.SetActive(false);
        poolQueue.Enqueue(box);
    }

    public GameObject GetBox()
    {
        if(poolQueue.Count > 0)
        {
            return poolQueue.Dequeue();
        }
        else
        {
           return GameObject.Instantiate(itemBox);
        }
    }
}
