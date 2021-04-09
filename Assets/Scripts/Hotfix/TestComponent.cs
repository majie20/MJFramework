using UnityEngine;

namespace MGame.Hotfix
{
    public class TestComponent
    {
        public int value = 10101;
        public GameObject GameObject;
        public Transform Transform;
        public Vector3 Vector3;

        public TestComponent()
        {
            GameObject = new GameObject("aaa");
            Transform = GameObject.transform;
            Vector3 = Transform.position;
        }
    }
}