#if WX
using System;
using Cysharp.Threading.Tasks;
using WeChatWASM;


//[RequireComponent(typeof(BaseInputModule))]
//public class WXTouchInputOverride : BaseInput
//{
//    private          bool            _isInitWechatSDK;
//    private readonly List<TouchData> _touches               = new List<TouchData>();
//    private          BaseInputModule _standaloneInputModule = null;

//    protected override void Awake()
//    {
//        base.Awake();
//        _standaloneInputModule = GetComponent<BaseInputModule>();
//    }

//    protected override void OnEnable()
//    {
//        base.OnEnable();
//        if (string.IsNullOrEmpty(WeChatWASM.WX.GetSystemInfoSync().platform)) return;
//        InitWechatTouchEvents();
//        if (_standaloneInputModule)
//        {
//            _standaloneInputModule.inputOverride = this;
//        }
//    }

//    protected override void OnDisable()
//    {
//        base.OnDisable();
//        UnregisterWechatTouchEvents();
//        if (_standaloneInputModule)
//        {
//            _standaloneInputModule.inputOverride = null;
//        }
//    }

namespace Model
{
    [LifeCycle]
    public class WXFileSystemManagerComponent : Component, IAwake
    {
        private WeChatWASM.WXFileSystemManager _fileSystemManager;

        public void Awake()
        {
            _fileSystemManager = WeChatWASM.WXBase.GetFileSystemManager();
        }

        public override void Dispose()
        {
            _fileSystemManager = null;
            base.Dispose();
        }

        private void ReadFile(string path, Action<byte[]> call)
        {
            var param = new ReadFileParam
            {
                filePath = path,
                success = response => { call(response.binData); },
                fail = error =>
                {
                    NLog.Log.Error($"ReadFile fail! {error.errCode}==>{error.errMsg}");
                    call(null);
                }
            };
            _fileSystemManager.ReadFile(param);
        }

        public async UniTask<byte[]> ReadFile(string path)
        {
            var isComplete = false;
            byte[] result = null;

            ReadFile(path, bytes =>
            {
                result = bytes;
                isComplete = true;
            });
            await UniTask.WaitUntil(() => isComplete);

            return result;
        }

        private void WriteFile(string path, byte[] buffer, Action<bool> call)
        {
            var param = new WriteFileParam
            {
                //param.encoding = "utf-8";
                filePath = path,
                data = buffer,
                success = response => { call(true); },
                fail = error =>
                {
                    NLog.Log.Error($"WriteFile fail! {error.errCode}==>{error.errMsg}");
                    call(false);
                }
            };
            _fileSystemManager.WriteFile(param);
        }

        public async UniTask<bool> WriteFile(string path, byte[] buffer)
        {
            var isComplete = false;
            bool result = false;

            WriteFile(path, buffer, b =>
            {
                result = b;
                isComplete = true;
            });
            await UniTask.WaitUntil(() => isComplete);

            return result;
        }

        private void FileExists(string path, Action<bool> call)
        {
            var param = new AccessParam
            {
                path = path,
                success = response => { call(true); },
                fail = error =>
                {
                    NLog.Log.Debug($"FileExists fail! {error.errCode}==>{error.errMsg}");
                    call(false);
                }
            };
            _fileSystemManager.Access(param);
        }

        public async UniTask<bool> FileExists(string path)
        {
            var isComplete = false;
            var result = false;

            FileExists(path, b =>
            {
                result = b;
                isComplete = true;
            });
            await UniTask.WaitUntil(() => isComplete);

            return result;
        }

        private void CopyFile(string srcPath, string destPath, Action<bool> call)
        {
            var param = new CopyFileParam
            {
                srcPath = srcPath,
                destPath = destPath,
                success = response => { call(true); },
                fail = error =>
                {
                    NLog.Log.Error($"CopyFile fail! {error.errCode}==>{error.errMsg}");
                    call(false);
                }
            };
            _fileSystemManager.CopyFile(param);
        }

        public async UniTask<bool> CopyFile(string srcPath, string destPath)
        {
            var isComplete = false;
            var result = false;

            CopyFile(srcPath, destPath, b =>
            {
                result = b;
                isComplete = true;
            });
            await UniTask.WaitUntil(() => isComplete);

            return result;
        }

        private void Mkdir(string path, Action<bool> call)
        {
            var param = new MkdirParam
            {
                dirPath = path,
                recursive = true,
                success = response => { call(true); },
                fail = error =>
                {
                    NLog.Log.Error($"Mkdir fail! {error.errCode}==>{error.errMsg}");
                    call(false);
                }
            };
            _fileSystemManager.Mkdir(param);
        }

        public async UniTask<bool> Mkdir(string path)
        {
            var isComplete = false;
            var result = false;

            Mkdir(path, b =>
            {
                result = b;
                isComplete = true;
            });
            await UniTask.WaitUntil(() => isComplete);

            return result;
        }
    }
}
#endif