using DG.Tweening;
using System;

namespace MGame
{
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down,
        LeftAndUp,
        RightAndUp,
        LeftAndDown,
        RightAndDown
    }

    public enum UseWay
    {
        Open,
        Close
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class BaseTweenAttribute : Attribute
    {
        public UseWay useWay;
        public Ease ease;
        public float duration;

        protected BaseTweenAttribute(UseWay way, Ease ease, float duration)
        {
            this.useWay = way;
            this.ease = ease;
            this.duration = duration;
        }
    }

    public class AlphaTweenAttribute : BaseTweenAttribute
    {
        public float alpha;

        public AlphaTweenAttribute(UseWay way, float alpha, Ease ease, float duration) : base(way, ease, duration)
        {
            this.alpha = alpha;
        }
    }

    public class RotateTweenAttribute : BaseTweenAttribute
    {
        public float x;
        public float y;
        public float z;

        public RotateTweenAttribute(UseWay way, float x, float y, float z, Ease ease, float duration) : base(way, ease, duration)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public class ScaleTweenAttribute : BaseTweenAttribute
    {
        public float x;
        public float y;
        public float z;

        public ScaleTweenAttribute(UseWay way, float x, float y, float z, Ease ease, float duration) : base(way, ease, duration)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public class TranslateTweenAttribute : BaseTweenAttribute
    {
        public Direction direction;

        public TranslateTweenAttribute(UseWay way, Direction dic, Ease ease, float duration) : base(way, ease, duration)
        {
            this.direction = dic;
        }
    }
}