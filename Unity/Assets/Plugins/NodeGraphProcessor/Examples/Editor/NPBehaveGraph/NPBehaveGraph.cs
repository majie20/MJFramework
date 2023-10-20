//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2021年6月15日 11:19:33
//------------------------------------------------------------

using GraphProcessor;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Examples.Editor._05_All
{
    public class NPBehaveGraph: BaseGraph
    {
        [HideInInspector]
        public NP_BlackBoard NpBlackBoard = new NP_BlackBoard();
        
        [Button("ExpoertNPBehaveData")]
        public void ExpoertNPBehaveData()
        {
            
        }
    }
}