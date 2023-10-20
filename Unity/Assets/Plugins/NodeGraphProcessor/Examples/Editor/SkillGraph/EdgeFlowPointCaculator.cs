//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2021年7月12日 18:07:21
//------------------------------------------------------------

using UnityEngine;

namespace Examples.Editor._05_All
{
    /// <summary>
    /// 用于计算连线流动点的使用函数集
    /// </summary>
    public class EdgeFlowPointCaculator
    {
        /// <summary>
        /// 根据给定百分比和edge的所有端点返回一个位置
        /// </summary>
        /// <param name="percentage"></param>
        /// <param name="edgePoints"></param>
        /// <param name="totalEdgeLength"></param>
        /// <returns></returns>
        public static Vector2 GetFlowPointPosByPercentage(float percentage, Vector2[] edgePoints, float totalEdgeLength)
        {
            float firstFragmentLength = Vector2.Distance(edgePoints[1], edgePoints[0]);
            float firstFragmentPercentage = firstFragmentLength / totalEdgeLength;

            Vector2 resultPos;
            //如果处于第一段
            if (percentage <= firstFragmentPercentage)
            {
                resultPos.x = percentage * totalEdgeLength + edgePoints[0].x;
                resultPos.y = edgePoints[0].y;
            }
            //如果处于第二段
            else if (percentage > firstFragmentPercentage &&
                     percentage <= firstFragmentPercentage + 1 - 2 * firstFragmentPercentage)
            {
                float percentageInSecondFragment =
                    (percentage - firstFragmentPercentage) / (1 - 2 * firstFragmentPercentage);
                resultPos.x = percentageInSecondFragment * (edgePoints[2].x - edgePoints[1].x) + edgePoints[1].x;
                resultPos.y = percentageInSecondFragment * (edgePoints[2].y - edgePoints[1].y) + edgePoints[1].y;
            }
            //如果处于第三段
            else
            {
                float reversePercentageInThirdFragment = 1 - percentage;
                resultPos.x = edgePoints[3].x - reversePercentageInThirdFragment * totalEdgeLength;
                resultPos.y = edgePoints[3].y;
            }

            return resultPos;
        }
    }
}