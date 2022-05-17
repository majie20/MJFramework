using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class TimerDelayDoMs : TimerBaseMs
    {
        public TimerDelayDoMs(float planTime, Action onComplete, bool isStart = false, int loop = 0)
        {
            this.isStart = isStart;
            this.loop = loop;
            curTime = 0.0f;
            this.planTime = planTime;
            this.onComplete = onComplete;
            timerComponent = Game.Instance.Scene.GetComponent<TimerComponent>();
        }

        public override void Update(float tick)
        {
            base.Update(tick);
            curTime += tick;
            if (curTime >= planTime)
            {
                curLoop++;
                onComplete();
                if (loop == -1 || curLoop < loop)
                {
                    curTime = 0.0f;
                }
                else
                {
                    //timerComponent.RemoveTimer(this);
                }
            }
        }
    }

}