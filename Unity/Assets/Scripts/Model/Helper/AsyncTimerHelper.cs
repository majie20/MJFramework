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

        public static CancellationTokenSource TimeHandle(Action call, int milliseconds, int count, bool isInvoke = false)
        {
            var cts = new CancellationTokenSource();
            UniTask.Void(async () =>
            {
                var task = UniTask.Delay(milliseconds, cancellationToken: cts.Token).Preserve();
                if (count < 0)
                {
                    if (isInvoke)
                    {
                        call();
                    }

                    while (true)
                    {
                        await task;
                        call();
                    }
                }
                else
                {
                    int i = 0;
                    if (isInvoke)
                    {
                        call();
                        i++;
                    }

                    for (; i < count; i++)
                    {
                        await task;
                        call();
                    }
                }
            });

            return cts;
        }

        public static void TimeHandle(this UnityEngine.GameObject obj, Action call, int milliseconds, int count, bool isInvoke = false)
        {
            UniTask.Void(async () =>
            {
                var task = UniTask.Delay(milliseconds, cancellationToken: obj.GetCancellationTokenOnDestroy()).Preserve();
                if (count < 0)
                {
                    if (isInvoke)
                    {
                        call();
                    }
                    while (true)
                    {
                        await task;
                        call();
                    }
                }
                else
                {
                    int i = 0;
                    if (isInvoke)
                    {
                        call();
                        i++;
                    }

                    for (; i < count; i++)
                    {
                        await task;
                        call();
                    }
                }
            });
        }

        public static void TimeHandle(this Component component, Action call, int milliseconds, int count, bool isInvoke = false)
        {
            UniTask.Void(async () =>
            {
                var task = UniTask.Delay(milliseconds, cancellationToken: component.CancellationToken).Preserve();
                if (count < 0)
                {
                    if (isInvoke)
                    {
                        call();
                    }
                    while (true)
                    {
                        await task;
                        call();
                    }
                }
                else
                {
                    int i = 0;
                    if (isInvoke)
                    {
                        call();
                        i++;
                    }

                    for (; i < count; i++)
                    {
                        await task;
                        call();
                    }
                }
            });
        }

        #endregion 计时器-以毫秒为单位

        #region 计时器-以帧为单位

        public static CancellationTokenSource TimeFrameHandle(Action call, int time, int count, bool isInvoke = false)
        {
            var cts = new CancellationTokenSource();
            UniTask.Void(async () =>
            {
                var task = UniTask.DelayFrame(time, cancellationToken: cts.Token).Preserve();
                if (count < 0)
                {
                    if (isInvoke)
                    {
                        call();
                    }
                    while (true)
                    {
                        await task;
                        call();
                    }
                }
                else
                {
                    int i = 0;
                    if (isInvoke)
                    {
                        call();
                        i++;
                    }

                    for (; i < count; i++)
                    {
                        await task;
                        call();
                    }
                }
            });
            return cts;
        }

        public static void TimeFrameHandle(this Component component, Action call, int time, int count, bool isInvoke = false)
        {
            UniTask.Void(async () =>
            {
                var task = UniTask.DelayFrame(time, cancellationToken: component.CancellationToken).Preserve();
                if (count < 0)
                {
                    if (isInvoke)
                    {
                        call();
                    }
                    while (true)
                    {
                        await task;
                        call();
                    }
                }
                else
                {
                    int i = 0;
                    if (isInvoke)
                    {
                        call();
                        i++;
                    }

                    for (; i < count; i++)
                    {
                        await task;
                        call();
                    }
                }
            });
        }

        public static void TimeFrameHandle(this UnityEngine.GameObject obj, Action call, int time, int count, bool isInvoke = false)
        {
            UniTask.Void(async () =>
            {
                var task = UniTask.DelayFrame(time, cancellationToken: obj.GetCancellationTokenOnDestroy()).Preserve();
                if (count < 0)
                {
                    if (isInvoke)
                    {
                        call();
                    }
                    while (true)
                    {
                        await task;
                        call();
                    }
                }
                else
                {
                    int i = 0;
                    if (isInvoke)
                    {
                        call();
                        i++;
                    }

                    for (; i < count; i++)
                    {
                        await task;
                        call();
                    }
                }
            });
        }

        #endregion 计时器-以帧为单位
    }
}