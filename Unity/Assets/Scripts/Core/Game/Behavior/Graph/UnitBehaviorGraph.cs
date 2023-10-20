using GraphProcessor;

public class UnitBehaviorGraph : BaseGraph
{
#if UNITY_EDITOR

    [UnityEditor.MenuItem("Assets/Create/UnitBehaviorGraph", false, 10)]
    public static void CreateGraph()
    {
        var graph = UnityEngine.ScriptableObject.CreateInstance<UnitBehaviorGraph>();
        UnityEditor.ProjectWindowUtil.CreateAsset(graph, "UnitBehaviorGraph.asset");
    }

#endif
}