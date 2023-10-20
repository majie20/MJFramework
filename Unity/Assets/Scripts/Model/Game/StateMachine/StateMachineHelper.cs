using Model;
using UnityEngine;

public static class StateMachineHelper
{
    public static void Move(this BaseState state)
    {
        CharacterComponent characterComponent = state.Manager.Entity.GetComponent<CharacterComponent>();
        var vec = characterComponent.MoveVec;

        if (state.Vec != vec)
        {
            if (vec.x < 0)
            {
                state.Manager.Entity.EventSystem.Invoke<E_SwitchCharacterDir, MoveDir>(MoveDir.Left);
            }
            else if (vec.x > 0)
            {
                state.Manager.Entity.EventSystem.Invoke<E_SwitchCharacterDir, MoveDir>(MoveDir.Right);
            }

            state.Vec = vec;
            NumericComponent numericComponent = state.Manager.Entity.GetComponent<NumericComponent>();
            var speed = numericComponent.GetAsInt(NumericType.Speed);
            state.Manager.Entity.EventSystem.Invoke<E_VelocityChange, VelocityInfo>(new VelocityInfo(Vector2.right * vec * speed, VelocityType.Lasting, VelocitySource.Move));
        }
    }
}