using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace M.ProductionPipeline
{
    [CustomEditor(typeof(StepCollector))]
    public class StepCollectorInspectorEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Ö´ÐÐ", GUILayout.Height(40)))
            {
                var collector = target as StepCollector;

                StepEditor.RunStepGroup(StepCollector.GetSteps(collector));
            }

            GUILayout.EndHorizontal();

            base.OnInspectorGUI();
        }
    }
}