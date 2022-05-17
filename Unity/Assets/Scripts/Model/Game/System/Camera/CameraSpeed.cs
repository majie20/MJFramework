using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpeed : MonoBehaviour
{
    public Transform pos;
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private void Start()
    {
        //pos = GameObject.Find("player").transform;
    }

    //void Update()
    //{
    //    Vector3 v = transform.position;
    //    v.x = pos.position.x;
    //    v.y = pos.position.y;
    //    if (v.x > maxX)
    //    {
    //        v.x = maxX;
    //    }
    //    else if (v.x < minX)
    //    {
    //        v.x = minX;
    //    }
    //    if (v.y > maxY)
    //    {
    //        v.y = maxY;
    //    }
    //    else if (v.y < minY)
    //    {
    //        v.y = minY;
    //    }
    //    transform.position = v;
    //}
}
