using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class UnitValue
    {
        public static Dictionary<UnitType, Type[]> UnitTypeMap = new Dictionary<UnitType, Type[]>()
        {
            {
                UnitType.Player,
                new[]
                {
                    typeof(StateDataComponent), typeof(NumericComponent), typeof(AnimatorComponent), typeof(Rigidbody2DComponent), typeof(CharacterMovementComponent),
                    typeof(CharacterJumpComponent), typeof(CharacterAttackComponent), typeof(MainCharacterCtrlComponent),
                }
            },
            {
                UnitType.Monster,
                new[]
                {
                    typeof(StateDataComponent), typeof(NumericComponent), typeof(AnimatorComponent), typeof(Rigidbody2DComponent), typeof(CharacterMovementComponent),
                    typeof(CharacterJumpComponent), typeof(CharacterAttackComponent), typeof(MonsterComponent), typeof(MonsterAIComponent)
                }
            }
        };

        public static Dictionary<UnitType, StateInfo[]> StateMachineTypeMap = new Dictionary<UnitType, StateInfo[]>()
        {
            {
                UnitType.Player,
                new[]
                {
                    new StateInfo(StateMachineType.Idle, StateMachineType.Idle | StateMachineType.Walk | StateMachineType.Run | StateMachineType.Die),
                    new StateInfo(StateMachineType.Walk, StateMachineType.Idle | StateMachineType.Run),
                    new StateInfo(StateMachineType.Run, StateMachineType.Idle | StateMachineType.Walk),
                    new StateInfo(StateMachineType.Jump, StateMachineType.Idle | StateMachineType.Walk | StateMachineType.Run | StateMachineType.Jump | StateMachineType.Climb),
                    new StateInfo(StateMachineType.Attack, StateMachineType.Idle | StateMachineType.Walk | StateMachineType.Run | StateMachineType.Jump),
                    new StateInfo(StateMachineType.Skill, StateMachineType.Idle | StateMachineType.Walk | StateMachineType.Run | StateMachineType.Attack),
                    new StateInfo(StateMachineType.Die,
                        StateMachineType.Idle | StateMachineType.Walk | StateMachineType.Run | StateMachineType.Jump | StateMachineType.Climb | StateMachineType.Attack
                      | StateMachineType.Skill | StateMachineType.Hurt),
                    new StateInfo(StateMachineType.Hurt,
                        StateMachineType.Idle | StateMachineType.Walk | StateMachineType.Run | StateMachineType.Jump | StateMachineType.Climb | StateMachineType.Attack
                      | StateMachineType.Skill),
                    new StateInfo(StateMachineType.Climb, StateMachineType.Idle | StateMachineType.Walk | StateMachineType.Run | StateMachineType.Jump),
                }
            },
            {
                UnitType.Monster,
                new[]
                {
                    new StateInfo(StateMachineType.Idle, StateMachineType.Idle | StateMachineType.Walk | StateMachineType.Run | StateMachineType.Die),
                    new StateInfo(StateMachineType.Walk, StateMachineType.Idle | StateMachineType.Run),
                    new StateInfo(StateMachineType.Run, StateMachineType.Idle | StateMachineType.Walk),
                    new StateInfo(StateMachineType.Jump, StateMachineType.Idle | StateMachineType.Walk | StateMachineType.Run | StateMachineType.Jump | StateMachineType.Climb),
                    new StateInfo(StateMachineType.Attack, StateMachineType.Idle | StateMachineType.Walk | StateMachineType.Run | StateMachineType.Jump),
                    new StateInfo(StateMachineType.Skill, StateMachineType.Idle | StateMachineType.Walk | StateMachineType.Run | StateMachineType.Attack),
                    new StateInfo(StateMachineType.Die,
                        StateMachineType.Idle | StateMachineType.Walk | StateMachineType.Run | StateMachineType.Jump | StateMachineType.Climb | StateMachineType.Attack
                      | StateMachineType.Skill | StateMachineType.Hurt),
                    new StateInfo(StateMachineType.Hurt,
                        StateMachineType.Idle | StateMachineType.Walk | StateMachineType.Run | StateMachineType.Jump | StateMachineType.Climb | StateMachineType.Attack
                      | StateMachineType.Skill),
                    new StateInfo(StateMachineType.Climb, StateMachineType.Idle | StateMachineType.Walk | StateMachineType.Run | StateMachineType.Jump),
                }
            }
        };

        public const float MIN_VERTICAL_VELOCITY = 0.01f;
        public const float MIN_MOVE_DISTANCE     = 0.001f;
        public const float DETECT_DISTANCE_FLOOR = 0.3f;
    }

    public class StateInfo
    {
        public StateMachineType Type;
        public StateMachineType InterruptType;

        public StateInfo(StateMachineType type, StateMachineType InterruptType)
        {
            this.Type = type;
            this.InterruptType = InterruptType;
        }
    }

    public struct StateMachineToggleInfo
    {
        public StateMachineType Type;
        public int              Index;

        public StateMachineToggleInfo(StateMachineType type)
        {
            this.Type = type;
            this.Index = 0;
        }

        public StateMachineToggleInfo(StateMachineType type, int index)
        {
            this.Type = type;
            this.Index = index;
        }

        //public static bool operator ==(StateMachineToggleInfo left, StateMachineToggleInfo right)
        //{
        //    return left.Type == right.Type && left.Index == right.Index;
        //}

        //public static bool operator !=(StateMachineToggleInfo left, StateMachineToggleInfo right)
        //{
        //    return left.Type != right.Type || left.Index != right.Index;
        //}
    }

    public enum MoveType : byte
    {
        Walking,
        Running,
        StopMove,
    }

    public enum MoveDir : byte
    {
        None,
        Left,
        Right,
        Up,
        Down,
    }

    public enum VelocityType : byte
    {
        Lasting,
        Disposable,
    }

    public enum VelocitySource : byte
    {
        Move,
        Jump,
        Hurt,
    }

    public struct VelocityInfo
    {
        public Vector2        Vec;
        public VelocityType   Type;
        public VelocitySource Source;

        public VelocityInfo(Vector2 vec, VelocityType type, VelocitySource source)
        {
            Vec = vec;
            Type = type;
            Source = source;
        }
    }
}