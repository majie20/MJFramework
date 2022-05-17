using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class TimerBaseFrame : TimerBaseOriginal
    {
        public int pauseFrame = 0;
        public int curFrame = 0;
        public int planFrame;

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
            pauseFrame = Time.frameCount - curFrame;
        }

        public override void Resume()
        {
            curFrame = Time.frameCount;
            curLoop = loop;
        }
    }
}
