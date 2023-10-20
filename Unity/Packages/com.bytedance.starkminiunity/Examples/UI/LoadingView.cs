// created by StarkMini

using System;
using UnityEngine;
using UnityEngine.UI;

namespace StarkMini.Examples.UI {
    /// <summary>
    /// 通用加载界面。 通常调用 LoadingManager 的相关接口，以相应显示。
    /// LoadingManager 相关接口例如：  StartShowLoading, StopShowLoading, ShowLoadingTask, ShowLoadingHandle.
    /// </summary>
    public class LoadingView : StarkMiniBaseView {
        public static LoadingView Instance;

        public GameObject spinner;

        /// <summary>
        /// make it as LoadingManager default loading ui
        /// </summary>
        public bool useForLoadingManager = true;

        /// <summary>
        /// enable auto rotate spinner
        /// </summary>
        public bool autoRotateSpinner = true;

        /// <summary>
        /// rotate speed, angle of degree / second
        /// </summary>
        public float autoRotateSpinnerSpeed = 90.0f;

        /// <summary>
        /// enable auto set self transform.SetAsLastSibling() to make it top
        /// </summary>
        public Action OnUpdate { get; set; }

        /// <summary>
        /// enable auto set self transform.SetAsLastSibling() to make it top
        /// </summary>
        public bool autoMakeItTopSibling { get; set; }

        private void SetInstance() {
            if (Instance == null) {
                Instance = this;
            }
        }
        private void DestroyInstance() {
            if (Instance == this) {
                Instance = null;
            }
        }

        public override void Awake() {
            SetInstance();
            if (useForLoadingManager) {
                _usedLoadingManager = LoadingManager.Instance;
                _usedLoadingManager.OnLoadingUIUpdate += UpdateShowLoading;
            }
            base.Awake();
        }

        private void OnDestroy() {
            DestroyInstance();
            if (_usedLoadingManager != null) {
                _usedLoadingManager.OnLoadingUIUpdate -= UpdateShowLoading;
            }
        }

        private void Start() {
            Setup();
        }

        void OnEnable() {
        }

        void OnDisable() {
        }

        private void Update() {
            UpdateSpinner();
            if (autoMakeItTopSibling) {
                MakeItTop();
            }
            OnUpdate?.Invoke();
        }

        private void UpdateSpinner() {
            if (autoRotateSpinner && this.spinner.activeSelf) {
                var deltaTime = Math.Max(Time.deltaTime, 0.05f);
                this.spinner.transform.Rotate(0, 0, -deltaTime * autoRotateSpinnerSpeed);
            }
        }

        public override void Hide() {
            base.Hide();
            if (autoMakeItTopSibling) {
                MakeItTop();
            }
        }

        public override void Show() {
            base.Show();
            if (autoMakeItTopSibling) {
                MakeItTop();
            }
        }

        private void MakeItTop() {
            this.transform.SetAsLastSibling();
        }

        public void Setup() {
        }


        public void StartSpinner() {
            spinner.SetActive(true);
        }

        public void StopSpinner() {
            spinner.SetActive(false);
        }

        public void UpdateShowLoading(bool loading) {
            _UpdateShowLoading(loading);
        }

        private void _UpdateShowLoading(bool loading) {
            if (loading) {
                LoadingView.Instance.Show();
            } else {
                LoadingView.Instance.Hide();
            }
        }

        private LoadingManager _usedLoadingManager;
    }

}