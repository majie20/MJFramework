//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2021年6月16日 10:58:07
//------------------------------------------------------------

using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Examples.Editor._05_All
{
    [HideReferenceObjectPicker]
    [BoxGroup]
    public class NP_BlackBoard
    {
        public Dictionary<string, string> TestEvent = new Dictionary<string, string>();

        public Dictionary<long, long> TestId = new Dictionary<long, long>();
    }
}