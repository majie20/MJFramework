using UnityEngine;

namespace MGame.Hotfix
{
    public static class Init
    {
        public static void Start()
        {
            TestComponent testComponent = new TestComponent();
            Debug.Log($"------{testComponent.value}------"); // MDEBUG:
            Debug.Log($"------{testComponent.Vector3}------"); // MDEBUG:
            Debug.Log($"------{testComponent.GameObject}------"); // MDEBUG:
            Debug.Log($"------{testComponent.Transform}------"); // MDEBUG:
        }
    }
}