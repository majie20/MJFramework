using UnityEngine;

namespace Model
{
    public static class GameObjectHelper
    {
        public static void SetActiveByScale(this GameObject obj, bool isActive)
        {
            if (isActive)
            {
                if (obj.activeSelf)
                {
                    obj.transform.localScale = Vector3.one;
                }
                else
                {
                    obj.SetActive(true);
                }
            }
            else
            {
                if (obj.activeSelf)
                {
                    obj.transform.localScale = Vector3.zero;
                }
            }
        }

        public static void SetActiveByPosition(this GameObject obj, bool isActive)
        {
            if (isActive)
            {
                if (obj.activeSelf)
                {
                    obj.transform.localPosition = Vector3.one;
                }
                else
                {
                    obj.SetActive(true);
                }
            }
            else
            {
                if (obj.activeSelf)
                {
                    obj.transform.localPosition = Vector3.zero;
                }
            }
        }

        public static void SetCanvasGroupToActive(this CanvasGroup cg, bool isActive)
        {
            if (isActive)
            {
                if (cg.gameObject.activeSelf)
                {
                    cg.alpha = 1;
                    cg.blocksRaycasts = true;
                }
                else
                {
                    cg.gameObject.SetActive(true);
                }
            }
            else
            {
                if (cg.gameObject.activeSelf)
                {
                    cg.alpha = 0;
                    cg.blocksRaycasts = false;
                }
            }
        }

        #region 3D

        public static void SetActiveByLocalPosition3D(this GameObject obj, bool isActive)
        {
            ActiveHandle3D component = obj.GetComponent<ActiveHandle3D>();
            if (component != null)
            {
                component.SetActiveByLocalPosition(isActive);
            }
            else
            {
                obj.SetActive(isActive);
            }
        }

        public static void SetActiveByLocalScale3D(this GameObject obj, bool isActive)
        {
            ActiveHandle3D component = obj.GetComponent<ActiveHandle3D>();
            if (component != null)
            {
                component.SetActiveByLocalScale(isActive);
            }
            else
            {
                obj.SetActive(isActive);
            }
        }

        #endregion 3D

        #region 2D

        public static void SetActiveByLocalPosition2D(this GameObject obj, bool isActive)
        {
            ActiveHandle3D component = obj.GetComponent<ActiveHandle3D>();
            if (component != null)
            {
                component.SetActiveByLocalPosition(isActive);
            }
            else
            {
                obj.SetActive(isActive);
            }
        }

        public static void SetActiveByLocalScale2D(this GameObject obj, bool isActive)
        {
            ActiveHandle2D component = obj.GetComponent<ActiveHandle2D>();
            if (component != null)
            {
                component.SetActiveByLocalScale(isActive);
            }
            else
            {
                obj.SetActive(isActive);
            }
        }

        #endregion 2D

        #region UI

        public static void SetActiveByLocalPositionUI(this GameObject obj, bool isActive)
        {
            ActiveHandleUI component = obj.GetComponent<ActiveHandleUI>();
            if (component != null)
            {
                component.SetActiveByLocalPosition(isActive);
            }
            else
            {
                obj.SetActive(isActive);
            }
        }

        public static void SetActiveByLocalScaleUI(this GameObject obj, bool isActive)
        {
            ActiveHandleUI component = obj.GetComponent<ActiveHandleUI>();
            if (component != null)
            {
                component.SetActiveByLocalScale(isActive);
            }
            else
            {
                obj.SetActive(isActive);
            }
        }

        public static void SetActiveByCanvasGroupUI(this GameObject obj, bool isActive)
        {
            ActiveHandleUI component = obj.GetComponent<ActiveHandleUI>();
            if (component != null)
            {
                component.SetActiveByCanvasGroup(isActive);
            }
            else
            {
                obj.SetActive(isActive);
            }
        }

        #endregion UI
    }
}