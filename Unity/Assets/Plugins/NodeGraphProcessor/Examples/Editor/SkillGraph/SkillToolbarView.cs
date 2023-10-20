//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2021年5月31日 19:15:32
//------------------------------------------------------------

using GraphProcessor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

namespace Examples.Editor._05_All
{
    public class SkillToolbarView : NPBehaveToolbarView
    {
        private SkillGraphWindow _skillGraphWindow;

        public SkillToolbarView(SkillGraphWindow skillGraphWindow, BaseGraphView graphView, MiniMap miniMap,
            BaseGraph baseGraph) : base(graphView,
            miniMap, baseGraph)
        {
            _skillGraphWindow = skillGraphWindow;
        }

        protected override void AddButtons()
        {
            base.AddButtons();
            AddButton(new GUIContent("Show Edge Flow", "展示连线流向"), _skillGraphWindow.ShowOrHideEdgeFlowPoint);
        }
    }
}