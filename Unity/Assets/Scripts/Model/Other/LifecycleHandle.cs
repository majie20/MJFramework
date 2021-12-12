using UnityEngine;

public class LifecycleHandle : MonoBehaviour
{
    public string EnableEventSign { set; get; }
    public string DisableEventSign { set; get; }
    public string DestroyEventSign { set; get; }

    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(EnableEventSign))
        {
            Model.Game.Instance.EventSystem.Invoke(EnableEventSign);
        }
    }

    private void OnDisable()
    {
        if (!string.IsNullOrEmpty(DisableEventSign))
        {
            Model.Game.Instance.EventSystem.Invoke(DisableEventSign);
        }
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(DestroyEventSign))
        {
            Model.Game.Instance.EventSystem.Invoke(DestroyEventSign);
        }
    }
}