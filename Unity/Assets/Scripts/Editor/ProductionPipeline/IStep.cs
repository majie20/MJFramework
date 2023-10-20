using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M.ProductionPipeline
{
    public interface IStep
    {
        /// <summary>
        /// 执行
        /// </summary>
        void Run();
        /// <summary>
        /// 开始执行打印
        /// </summary>
        /// <returns></returns>
        string EnterText();
        /// <summary>
        /// 结束执行打印
        /// </summary>
        /// <returns></returns>
        string ExitText();
        /// <summary>
        /// 是否会触发编译
        /// </summary>
        /// <returns></returns>
        bool IsTriggerCompile();
    }
}