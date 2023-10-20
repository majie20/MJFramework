using Coffee.UIEffects;
using UnityEditor;
using UnityEngine;

//自定义ReferenceCollector类在界面中的显示与功能
[CustomEditor(typeof(CanvasRenderer))]
public class BaseMeshEffectEditor : Editor
{
    private CanvasRenderer _canvasRenderer;

    private void OnEnable()
    {
        _canvasRenderer = target as CanvasRenderer;

        var component = _canvasRenderer.GetComponent<BaseMeshEffect>();

        if (component != null)
        {
            OnComponentWasAdded(component);
        }

        ObjectFactory.componentWasAdded += OnComponentWasAdded;
    }

    private void OnDisable()
    {
        ObjectFactory.componentWasAdded += OnComponentWasAdded;
    }

    private void OnComponentWasAdded(Component component)
    {
        if (_canvasRenderer != null && component.gameObject == _canvasRenderer.gameObject && component is BaseMeshEffect effect)
        {
            var components = _canvasRenderer.GetComponentsInChildren<UnityEngine.UI.Graphic>();

            for (int i = components.Length - 1; i > 0; i--)
            {
                var e = components[i].GetComponent<BaseMeshEffect>();

                if (e == null)
                {
                    var uiSub = components[i].gameObject.AddComponent<UISub>();
                    uiSub.parent = effect;
                    uiSub.DirtyMaterialCallback();
                    uiSub.DirtyVerticesCallback();
                }
            }
        }
    }

    [MenuItem("CONTEXT/BaseMeshEffect/Remove Component")]
    public static void RemoveImage(MenuCommand menuCommand)
    {
        var effect = menuCommand.context as BaseMeshEffect;
        var components = effect.GetComponentsInChildren<UnityEngine.UI.Graphic>();

        for (int i = components.Length - 1; i > 0; i--)
        {
            var e = components[i].GetComponent<BaseMeshEffect>();

            if (e != null && e is UISub uiSub && uiSub.parent == effect)
            {
                UnityEngine.Object.DestroyImmediate(e);
                components[i].SetMaterialDirty();
                components[i].SetVerticesDirty();
            }
        }

        UnityEngine.Object.DestroyImmediate(effect);
    }
}