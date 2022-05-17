using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class TimerHelper
    {
        /// <summary>
        /// 根据帧执行
        /// </summary>
        /// <param name="frame">延迟多少帧</param>
        /// <param name="onComplete">完成函数</param>
        /// <returns></returns>
        public static TimerBase DelayDoFrame(int frame, Action onComplete, bool isStart = false, int loop = 0)
        {
            var timerComponent = Game.Instance.Scene.GetComponent<TimerComponent>();
            var timerDelayDo = timerComponent.GetCache<TimerDelayDo>("TimerDelayDo");
            timerDelayDo.InitFrame(frame, onComplete, isStart, loop);
            timerComponent.AddTimer(timerDelayDo);
            return timerDelayDo;
        }

        /// <summary>
        /// 根据时间执行
        /// </summary>
        /// <param name="frame">延迟多少时间</param>
        /// <param name="onComplete">完成函数</param>
        /// <returns></returns>
        public static TimerBase DelayDo(float time, Action onComplete, bool isStart = false, int loop = 0)
        {
            var timerComponent = Game.Instance.Scene.GetComponent<TimerComponent>();
            var timerDelayDo = timerComponent.GetCache<TimerDelayDo>("TimerDelayDo");
            timerDelayDo.Init(time, onComplete, isStart, loop);
            timerComponent.AddTimer(timerDelayDo);
            return timerDelayDo;
        }
    }
}
