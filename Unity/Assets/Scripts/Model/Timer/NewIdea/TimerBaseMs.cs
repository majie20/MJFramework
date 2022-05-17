using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class TimerBaseMs : TimerBaseOriginal
    {

        public float curTime = 0.0f;
        public float planTime;

        public Action onComplete;

        public TimerComponent timerComponent;
        public override void Update(float tick)
        {
            if (!isRun || loop == 0)
            {
                return;
            }

            if (isStart)
            {
                onComplete();
                curLoop++;
                isStart = false;
            }
        }

        public override void Start()
        {
            isRun = true;
        }

        public override void Pause()
        {
            isRun = false;
        }

        public override void Resume()
        {
            curTime = 0.0f;
            curLoop = loop;
        }
    }
}
