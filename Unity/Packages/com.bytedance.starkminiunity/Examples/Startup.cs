using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StarkMini.Examples
{
    public class Startup : MonoBehaviour
    {
        /// <summary>
        /// 原首场景AA化后的Address
        /// </summary>
        public string firstSceneAddress;

        /// <summary>
        /// 首场景载入的进度显示
        /// </summary>
        public Slider progressSlider;

        /// <summary>
        /// 首场景载入的百分比文字显示
        /// </summary>
        public Text progressText;

        // Start is called before the first frame update
        async Task Start()
        {
            var handle = ResLoader.LoadSceneAsync(firstSceneAddress,LoadSceneMode.Single, false);
            while (!handle.IsDone)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    break;
#endif
                var percent = handle.GetDownloadStatus().Percent;
                Debug.Log("Startup Progress:"+ percent.ToString());
                progressSlider.value = percent;
                progressText.text = $"{percent * 100:N0}%";
                await Task.Yield();
            }

            progressSlider.value = 1f;
            progressText.text ="100%";

            await Task.Yield();
            handle.Result.ActivateAsync();
        }
    }
}

