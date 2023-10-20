// created by StarkMini

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StarkMini.Examples.UI {

    /// <summary>
    /// 通用加载失败弹窗界面。
    /// 调用 static 的 `Popup`, `PopupAsync` 接口，以弹框。
    /// 使用 `ErrorMsg`, `OKText`, `RetryText` 作为通用的文本。
    /// 可以配合使用 `LocaleManager` 类，以增加和使用自定义的多语言文本。
    /// </summary>
    public class LoadErrorPopupView : StarkMiniBaseView {
        public static LoadErrorPopupView Instance;

        public Text title;
        public Text body;
        public Button okButton;
        public Button acceptButton;
        public Button declineButton;
        public Text okButtonText;
        public Text acceptButtonLabel;
        public Text declineButtonLabel;
        private Action acceptAction;
        private Action declineAction;

        /// <summary>
        /// 弹窗
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="acceptText"></param>
        /// <param name="onAccept"></param>
        /// <exception cref="Exception"></exception>
        public static void Popup(string msg, string acceptText, Action onAccept = null) {
            var view = LoadErrorPopupView.Instance;
            if (!view) {
                throw new Exception("Error popup view invalid!");
            }

            string title = ErrorTitle;
            LoadErrorPopupView.Instance.Setup(title, msg, acceptText, onAccept);
        }

        /// <summary>
        /// 弹窗
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="acceptText"></param>
        /// <param name="declineText"></param>
        /// <param name="onAccept"></param>
        /// <param name="onDecline"></param>
        /// <exception cref="Exception"></exception>
        public static void Popup(string msg, string acceptText, string declineText = null, Action onAccept = null, Action onDecline = null) {
            var view = LoadErrorPopupView.Instance;
            if (!view) {
                throw new Exception("Error popup view invalid!");
            }

            var eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem == null) {
                Debug.LogWarning("EventSystem not found! Now auto created one with StandaloneInputModule.");
                var go = new GameObject(nameof(EventSystem), typeof(EventSystem), typeof(StandaloneInputModule));
            }

            string title = ErrorTitle;
            if (string.IsNullOrEmpty(declineText)) {
                LoadErrorPopupView.Instance.Setup(title, msg, acceptText, onAccept);
            } else {
                LoadErrorPopupView.Instance.SetupTwo(title, msg, acceptText, declineText, onAccept, onDecline);
            }
        }

        /// <summary>
        /// 弹窗, 异步返回 bool isAccept.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="acceptText"></param>
        /// <param name="declineText"></param>
        /// <returns>isAccept</returns>
        public static async Task<bool> PopupAsync(string msg, string acceptText, string declineText = null) {
            bool isAccept = false;

            void onAccept() {
                isAccept = true;
            }

            Popup(msg, acceptText, declineText, onAccept);
            while (!isAccept) {
                await Task.Yield();
            }

            return isAccept;
        }

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
            base.Awake();
            this.okButton.onClick.AddListener(AcceptButtonTapped);
            this.acceptButton.onClick.AddListener(AcceptButtonTapped);
            this.declineButton.onClick.AddListener(DeclineButtonTapped);
        }

        private void OnDestroy() {
            DestroyInstance();
        }

        public override void Show() {
            if (this.container.activeSelf) {
                Debug.LogWarning("LoadErrorPopupView Already Showing!");
            }

            base.Show();
            MakeItTop();
        }

        public override void Hide() {
            base.Hide();
        }

        public static string ErrorTitle {
            get { return LocaleManager.Get(LocaleManager.TextKey_Load_ErrorTitle); }
        }

        public static string ErrorMsg {
            get {
                return LocaleManager.Get(LocaleManager.TextKey_Load_ErrorMsg);
            }
        }

        public static string GameResErrorMsg {
            get { return LocaleManager.Get(LocaleManager.TextKey_Load_GameResErrorMsg); }
        }

        public static string AdErrorMsg {
            get {
                return LocaleManager.Get(LocaleManager.TextKey_SDK_AdLoadErrorMsg);
            }
        }
        public static string AdExceptionMsg {
            get {
                return LocaleManager.Get(LocaleManager.TextKey_SDK_AdExceptionMsg);
            }
        }

        public static string RetryText {
            get { return LocaleManager.Get(LocaleManager.TextKey_Retry); }
        }

        public static string OKText {
            get { return LocaleManager.Get(LocaleManager.TextKey_OK); }
        }

        public void Setup(string titleText, string bodyText, string acceptText, Action acceptAction) {
            this.title.text = titleText;
            this.body.text = bodyText;
            this.okButtonText.text = acceptText;
            this.acceptButtonLabel.text = acceptText;

            this.acceptAction = acceptAction; // save callback handler

            declineButton.gameObject.SetActive(false);
            acceptButton.gameObject.SetActive(false);
            okButton.gameObject.SetActive(true);
            this.Show();
        }

        public void SetupTwo(string titleText, string bodyText, string acceptText, string declineText,
            Action acceptAction, Action declineAction) {
            this.title.text = titleText;
            this.body.text = bodyText;
            this.okButtonText.text = acceptText;
            this.acceptButtonLabel.text = acceptText;

            this.acceptAction = acceptAction; // save callback handler

            if (declineText != null) {
                this.declineButtonLabel.text = declineText;
                this.declineAction = declineAction; // save callback handler
                declineButton.gameObject.SetActive(true);
                acceptButton.gameObject.SetActive(true);
                okButton.gameObject.SetActive(false);
            } else {
                declineButton.gameObject.SetActive(false);
                acceptButton.gameObject.SetActive(false);
                okButton.gameObject.SetActive(true);
            }

            this.Show();
        }

        public void AcceptButtonTapped() {
            // must call Hide first, in case if the callback trigger another popup
            this.Hide();
            // then callback
            if (acceptAction != null) acceptAction();
        }

        public void DeclineButtonTapped() {
            // must call Hide first, in case if the callback trigger another popup
            this.Hide();
            // then callback
            if (declineAction != null) declineAction();
        }

        private void MakeItTop() {
            this.transform.SetAsLastSibling();
        }
    }
}