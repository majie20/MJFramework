// created by StarkMini

using UnityEngine;

namespace StarkMini.Examples.UI {
    public class StarkMiniUICanvas : SingletonMonoBehaviour<StarkMiniUICanvas> {
        void Awake() {
            if (CheckDuplicated()) {
                return;
            }
            DontDestroyOnLoad(this);
        }
    }
}