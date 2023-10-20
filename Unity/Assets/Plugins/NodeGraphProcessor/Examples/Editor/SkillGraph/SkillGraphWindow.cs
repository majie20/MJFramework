using System;
using System.Collections;
using System.Collections.Generic;
using Examples.Editor._05_All;
using UnityEngine;
using UnityEditor;
using GraphProcessor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Profiling;
using UnityEngine.UIElements;

public class SkillGraphWindow : UniversalGraphWindow
{
    protected override void InitializeWindow(BaseGraph graph)
    {
        graphView = new UniversalGraphView(this);

        m_MiniMap = new MiniMap() {anchored = true};
        graphView.Add(m_MiniMap);

        m_ToolbarView = new SkillToolbarView(this, graphView, m_MiniMap, graph);
        graphView.Add(m_ToolbarView);
    }

    public bool ShowFlowPoint;

    private float m_FlowPointGap = 60f;

    private float m_FlowPointMoveSpeed = 0.003f;

    public void ShowOrHideEdgeFlowPoint()
    {
        ShowFlowPoint = !ShowFlowPoint;
    }

    protected override void Update()
    {
        base.Update();
        if (ShowFlowPoint)
        {
            ShowEdgeFlowPoint();
        }
        else
        {
            HideEdgeFlowPoint();
        }
    }

    /// <summary>
    /// 展示FlowPoint，自己计算位置信息
    ///
    /// ---------------------------------------------------------------------------------------
    /// 
    /// 也可以继承Manipulator实现一个控制器，对EdgeView添加AddManipulator，Manipulator即可自动同步位置
    /// 只需要设置Manipulator的left和top间距即可，可参见https://github.com/HalfLobsterMan/3.0_GraphProcessor/blob/56c2928a1790994df4a1f8a9c2a30477bbe6e21d/Editor/Views/BaseEdgeView.cs
    /// </summary>
    private void ShowEdgeFlowPoint()
    {
        foreach (var edgeView in graphView.edgeViews)
        {
            float edgeLength = 0;
            for (int i = 0; i < edgeView.GetPointsAndTangents.Length - 1; i++)
            {
                edgeLength += Vector2.Distance(edgeView.GetPointsAndTangents[i],
                    edgeView.GetPointsAndTangents[i + 1]);
            }

            float eachChunkContainsPercentage = m_FlowPointGap / edgeLength;
            int flowPointCount = (int) (1 / eachChunkContainsPercentage);

            if (flowPointCount % 2 == 0)
            {
                flowPointCount++;
            }

            if (edgeView.EdgeFlowPointVisualElements != null && edgeView.EdgeFlowPointVisualElements.Count > 0 &&
                //如果长度发生变化就需要重新计算
                edgeView.EdgeFlowPointVisualElements.Count == flowPointCount)
            {
                for (int i = 0; i < flowPointCount; i++)
                {
                    edgeView.FlowPointProgress[i] += Time.deltaTime * m_FlowPointMoveSpeed;

                    edgeView.EdgeFlowPointVisualElements[i].transform.position =
                        EdgeFlowPointCaculator.GetFlowPointPosByPercentage(
                            Mathf.Repeat(edgeView.FlowPointProgress[i], 1),
                            edgeView.GetPointsAndTangents, edgeLength) -
                        new Vector2(8 * i, 0);
                }
            }
            else
            {
                if (edgeView.EdgeFlowPointVisualElements != null)
                {
                    foreach (var oldFlowPoint in edgeView.EdgeFlowPointVisualElements)
                    {
                        edgeView.Remove(oldFlowPoint);
                    }
                }

                edgeView.EdgeFlowPointVisualElements = new List<VisualElement>();
                edgeView.FlowPointProgress.Clear();

                for (int i = 0; i < flowPointCount; i++)
                {
                    float initalPercentage = eachChunkContainsPercentage * i;

                    VisualElement visualElement = new VisualElement()
                    {
                        name = "EdgeFlowPoint", transform =
                        {
                            position = EdgeFlowPointCaculator.GetFlowPointPosByPercentage(
                                           initalPercentage, edgeView.GetPointsAndTangents, edgeLength) -
                                       new Vector2(8 * i, 0),
                        }
                    };
                    
                    //可以自定义流点颜色，但注意将其alpha通道设置为1
                    //visualElement.style.unityBackgroundImageTintColor = edgeView.serializedEdge.outputNode.color;
                    edgeView.FlowPointProgress.Add(initalPercentage);
                    edgeView.EdgeFlowPointVisualElements.Add(visualElement);
                    edgeView.Add(visualElement);
                }
            }
        }
    }

    private void HideEdgeFlowPoint()
    {
        foreach (var edgeView in graphView.edgeViews)
        {
            if (edgeView.EdgeFlowPointVisualElements == null) return;
            foreach (var edgeFlowPoint in edgeView.EdgeFlowPointVisualElements)
            {
                edgeView.Remove(edgeFlowPoint);
            }

            edgeView.EdgeFlowPointVisualElements.Clear();
        }
    }
}