using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    public abstract class LoopScrollRectMulti : LoopScrollRectBase
    {
        [HideInInspector]
        [NonSerialized]
        public Action<Transform, int> ProvideDataCall = null;

        protected override void ProvideData(Transform transform, int index)
        {
            ProvideDataCall?.Invoke(transform, index);
        }

        // Multi Data Source cannot support TempPool
        protected override RectTransform GetFromTempPool(int itemIdx)
        {
            RectTransform nextItem = GetObject?.Invoke(itemIdx).transform as RectTransform;
            nextItem.transform.SetParent(m_Content, false);
            nextItem.gameObject.SetActive(true);

            ProvideData(nextItem, itemIdx);
            return nextItem;
        }

        protected override void ReturnToTempPool(bool fromStart, int count)
        {
            Debug.Assert(m_Content.childCount >= count);

            if (ReturnObject!=null)
            {
                if (fromStart)
                {
                    for (int i = count - 1; i >= 0; i--)
                    {
                        ReturnObject(m_Content.GetChild(i));
                    }
                }
                else
                {
                    int t = m_Content.childCount - count;
                    for (int i = m_Content.childCount - 1; i >= t; i--)
                    {
                        ReturnObject(m_Content.GetChild(i));
                    }
                }
            }
        }

        protected override void ClearTempPool()
        {
        }
    }
}