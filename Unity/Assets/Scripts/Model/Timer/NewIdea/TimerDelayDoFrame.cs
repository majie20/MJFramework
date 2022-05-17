using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class TimerDelayDoFrame : TimerBaseFrame
    {
        public TimerDelayDoFrame(int planFrame, Action onComplete, bool isStart = false, int loop = 0)
        {
            this.isStart = isStart;
            this.loop = loop;
            curFrame = Time.frameCount;
            this.planFrame = planFrame;
            this.onComplete = onComplete;
            timerComponent = Game.Instance.Scene.GetComponent<TimerComponent>();
        }

        public override void Update(float tick)
        {
            base.Update(tick);
            if (Time.frameCount >= planFrame + curFrame + pauseFrame)
            {
                curLoop++;
                onComplete();
                if (loop == -1 || curLoop < loop)
                {
                    pauseFrame = 0;
                    curFrame = Time.frameCount;
                }
                else
                {
                    //timerComponent.RemoveTimer(this);
                }
            }
        }
    }
}