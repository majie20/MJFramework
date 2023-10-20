// created by StarkMini

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace StarkMini.Examples.UI {

    public class StarkMiniBaseView : MonoBehaviour {
        public GameObject container;
        public LayoutGroup layout;
        public Image fade;

        public virtual void Awake() {
            Hide();
        }

        public virtual void Hide() {
            container.SetActive(false);
            if (layout != null) layout.enabled = false;
        }

        public virtual void Show() {
            container.SetActive(true);
            if (layout != null) layout.enabled = true;
            //if (fade!=null) StartCoroutine("ShowFade");
        }

        IEnumerator ShowFade() {
            float elapsedTime = 0f;
            //       float waitTime = 0.5f;
            float duration = 1f;

            float startAlpha = 0f;
            float endAlpha = 0.5f;
            Color c = fade.color;
            c.a = startAlpha;
            fade.color = c;
            //       yield return new WaitForSeconds(waitTime);

            while (elapsedTime < duration) {
                float perc = elapsedTime / duration;
                float alpha = Mathf.Lerp(startAlpha, endAlpha, perc);
                c.a = alpha;
                fade.color = c;
                yield return new WaitForEndOfFrame();
                elapsedTime = Mathf.Min(duration, elapsedTime + Time.unscaledDeltaTime);
            }

            c.a = endAlpha;
            fade.color = c;
        }

    }
}