using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public abstract class TimerBaseOriginal
    {
        /// <summary>
        /// 循环次数
        /// </summary>
        public int loop = 1;
        /// <summary>
        /// 当前循环次数
        /// </summary>
        protected int curLoop = 0;
        /// <summary>
        /// 是否立即执行一次
        /// </summary>
        public bool isStart = false;
        /// <summary>
        /// 是否运行
        /// </summary>
        public bool isRun = true;

        /// <summary>
        /// 执行
        /// </summary>
        public abstract void Update(float tick);
        /// <summary>
        /// 开始执行
        /// </summary>
        public abstract void Start();
        /// <summary>
        /// 暂停执行
        /// </summary>
        public abstract void Pause();
        /// <summary>
        /// 重新执行
        /// </summary>
        public abstract void Resume();
    }
}
