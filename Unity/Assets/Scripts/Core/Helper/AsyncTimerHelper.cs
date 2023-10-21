using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Model
{
    public static class AsyncTimerHelper
    {
        public static void StopTimer(CancellationTokenSource cts)
        {
            cts.Cancel();
            cts.Dispose();
        }

        #region 计时器-以毫秒为单位

        public static async UniTaskVoid TimeHandle(Action call, int milliseconds, int count, CancellationTokenSource cts = null, bool isInvoke = false, Action onComplete = null)
        {
            if (count < 0)
            {
                if (isInvoke)
                {
                    call();
                }

                while (true)
                {
                    var b = await UniTask.Delay(milliseconds, cancellationToken: cts?.Token ?? default(CancellationToken)).SuppressCancellationThrow();

                    if (b)
                    {
                        return;
                    }

                    call();
                }
            }
            else
            {
                if (count > 0)
                {
                    int i = 0;

                    if (isInvoke)
                    {
                        call();
                        i++;
                    }

                    for (; i < count; i++)
                    {
                        var b = await UniTask.Delay(milliseconds, cancellationToken: cts?.Token ?? default(CancellationToken)).SuppressCancellationThrow();

                        if (b)
                        {
                            return;
                        }

                        call();
                    }
                }
            }

            onComplete?.Invoke();
        }

        public static async UniTaskVoid TimeHandle(this UnityEngine.GameObject obj, Action call, int milliseconds, int count, bool isInvoke = false, Action onComplete = null)
        {
            if (count < 0)
            {
                if (isInvoke)
                {
                    call();
                }

                while (true)
                {
                    var b = await UniTask.Delay(milliseconds, cancellationToken: obj.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();

                    if (b)
                    {
                        return;
                    }

                    call();
                }
            }
            else
            {
                if (count > 0)
                {
                    int i = 0;

                    if (isInvoke)
                    {
                        call();
                        i++;
                    }

                    for (; i < count; i++)
                    {
                        var b = await UniTask.Delay(milliseconds, cancellationToken: obj.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();

                        if (b)
                        {
                            return;
                        }

                        call();
                    }
                }
            }

            onComplete?.Invoke();
        }

        public static async UniTaskVoid TimeHandle(this Entity entity, Action call, int milliseconds, int count, bool isInvoke = false, Action onComplete = null)
        {
            if (count < 0)
            {
                if (isInvoke)
                {
                    call();
                }

                while (true)
                {
                    var b = await UniTask.Delay(milliseconds, cancellationToken: entity.CancellationToken).SuppressCancellationThrow();

                    if (b)
                    {
                        return;
                    }

                    call();
                }
            }
            else
            {
                if (count > 0)
                {
                    int i = 0;

                    if (isInvoke)
                    {
                        call();
                        i++;
                    }

                    for (; i < count; i++)
                    {
                        var b = await UniTask.Delay(milliseconds, cancellationToken: entity.CancellationToken).SuppressCancellationThrow();

                        if (b)
                        {
                            return;
                        }

                        call();
                    }
                }
            }

            onComplete?.Invoke();
        }

        #endregion 计时器-以毫秒为单位

        #region 计时器-以帧为单位

        public static async UniTaskVoid TimeFrameHandle(Action call, int time, int count, CancellationTokenSource cts = null, bool isInvoke = false, Action onComplete = null)
        {
            if (count < 0)
            {
                if (isInvoke)
                {
                    call();
                }

                while (true)
                {
                    var b = await UniTask.DelayFrame(time, cancellationToken: cts?.Token ?? default(CancellationToken)).SuppressCancellationThrow();

                    if (b)
                    {
                        return;
                    }

                    call();
                }
            }
            else
            {
                if (count > 0)
                {
                    int i = 0;

                    if (isInvoke)
                    {
                        call();
                        i++;
                    }

                    for (; i < count; i++)
                    {
                        var b = await UniTask.DelayFrame(time, cancellationToken: cts?.Token ?? default(CancellationToken)).SuppressCancellationThrow();

                        if (b)
                        {
                            return;
                        }

                        call();
                    }
                }
            }

            onComplete?.Invoke();
        }

        public static async UniTaskVoid TimeFrameHandle(this Entity entity, Action call, int time, int count, bool isInvoke = false, Action onComplete = null)
        {
            if (count < 0)
            {
                if (isInvoke)
                {
                    call();
                }

                while (true)
                {
                    var b = await UniTask.DelayFrame(time, cancellationToken: entity.CancellationToken).SuppressCancellationThrow();

                    if (b)
                    {
                        return;
                    }

                    call();
                }
            }
            else
            {
                if (count > 0)
                {
                    int i = 0;

                    if (isInvoke)
                    {
                        call();
                        i++;
                    }

                    for (; i < count; i++)
                    {
                        var b = await UniTask.DelayFrame(time, cancellationToken: entity.CancellationToken).SuppressCancellationThrow();

                        if (b)
                        {
                            return;
                        }

                        call();
                    }
                }
            }

            onComplete?.Invoke();
        }

        public static async UniTaskVoid TimeFrameHandle(this UnityEngine.GameObject obj, Action call, int time, int count, bool isInvoke = false, Action onComplete = null)
        {
            if (count < 0)
            {
                if (isInvoke)
                {
                    call();
                }

                while (true)
                {
                    var b = await UniTask.DelayFrame(time, cancellationToken: obj.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();

                    if (b)
                    {
                        return;
                    }

                    call();
                }
            }
            else
            {
                if (count > 0)
                {
                    int i = 0;

                    if (isInvoke)
                    {
                        call();
                        i++;
                    }

                    for (; i < count; i++)
                    {
                        var b = await UniTask.DelayFrame(time, cancellationToken: obj.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();

                        if (b)
                        {
                            return;
                        }

                        call();
                    }
                }
            }

            onComplete?.Invoke();
        }

        #endregion 计时器-以帧为单位
    }
}