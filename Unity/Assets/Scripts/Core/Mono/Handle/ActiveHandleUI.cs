using UnityEngine;

namespace Model
{
    public class ActiveHandleUI : MonoBehaviour
    {
        private static Vector3 VEC_10000 = new Vector3(10000, 10000, 0);

        private CanvasGroup canvasGroup;

        private Vector3 curLocalPosition;
        private Vector3 curLocalScale;

        private float alpha;
        private bool blocksRaycasts;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void SetActiveByLocalPosition(bool isActive)
        {
            if (isActive)
            {
                if (gameObject.activeSelf)
                {
                    transform.localPosition = curLocalPosition;
                }
                else
                {
                    gameObject.SetActive(true);
                }
            }
            else
            {
                if (gameObject.activeSelf)
                {
                    curLocalPosition = transform.localPosition;
                    transform.localPosition = VEC_10000;
                }
            }
        }

        public void SetActiveByLocalScale(bool isActive)
        {
            if (isActive)
            {
                if (gameObject.activeSelf)
                {
                    transform.localScale = curLocalScale;
                }
                else
                {
                    gameObject.SetActive(true);
                }
            }
            else
            {
                if (gameObject.activeSelf)
                {
                    curLocalScale = transform.localScale;
                    transform.localScale = Vector3.zero;
                }
            }
        }

        public void SetActiveByCanvasGroup(bool isActive)
        {
            if (canvasGroup != null)
            {
                if (isActive)
                {
                    if (gameObject.activeSelf)
                    {
                        canvasGroup.alpha = alpha;
                        canvasGroup.blocksRaycasts = blocksRaycasts;
                    }
                    else
                    {
                        gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (gameObject.activeSelf)
                    {
                        alpha = canvasGroup.alpha;
                        blocksRaycasts = canvasGroup.blocksRaycasts;

                        canvasGroup.alpha = 0;
                        canvasGroup.blocksRaycasts = false;
                    }
                }
            }
            else
            {
                gameObject.SetActive(isActive);
            }
        }
    }
}