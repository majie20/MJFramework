using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cs : VerticalScroll
{
    public ScrollRect Scrolla;
    public GameObject boxa;
    private int level = 50;
    // Start is called before the first frame update
    void Start()
    {
        AddScrollListener(Scrolla, boxa, 100);
    }

    public override void SetBoxInfo(int idx, GameObject box)
    {
        box.GetComponentInChildren<Text>().text = (idx + 1).ToString();
        box.transform.Find("imgLock").gameObject.SetActive(level < idx);
    }

}
