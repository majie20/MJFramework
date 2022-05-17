using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    public string CharacterColliderEventSign { set; get; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!string.IsNullOrEmpty(CharacterColliderEventSign))
        {
            Game.Instance.EventSystem.Invoke(CharacterColliderEventSign);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!string.IsNullOrEmpty(CharacterColliderEventSign))
        {
            Game.Instance.EventSystem.Invoke(CharacterColliderEventSign, GetTargetType(collision.gameObject), collision.name);
        }
    }

    private TargetType GetTargetType(GameObject target)
    {
        if (target.tag == "Item")
        {
            Destroy(target);
            return TargetType.Item;
        }
        else if(target.tag == "Wall")
        {
            return TargetType.Wall;
        }
        return TargetType.Enemy;
    }
}
