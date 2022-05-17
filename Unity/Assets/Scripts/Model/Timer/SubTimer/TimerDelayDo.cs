using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class TimerDelayDo : TimerBase
    {
        public float curTime = 0.0f;
        public float planTime;

        public int pauseFrame = 0;
        public int curFrame = 0;
        public int planFrame;

        public Action onComplete;

        public TimerComponent timerComponent;

        public void InitFrame(int planFrame, Action onComplete,bool isStart=false, int loop = 0)
        {
            isRun = true;
            curLoop = 0;
            pauseFrame = 0;
            this.isStart = isStart;
            this.loop = loop;
            curFrame = Time.frameCount;
            this.planFrame = planFrame;
            this.onComplete = onComplete;
            this.isFrame = true;
            timerComponent = Game.Instance.Scene.GetComponent<TimerComponent>();
        }

        public void Init(float planTime, Action onComplete, bool isStart = false, int loop = 0)
        {
            isRun = true;
            curLoop = 0;
            pauseFrame = 0;
            this.isStart = isStart;
            this.loop = loop;
            curTime = 0.0f;
            this.planTime = planTime;
            this.onComplete = onComplete;
            this.isFrame = false;
            timerComponent = Game.Instance.Scene.GetComponent<TimerComponent>();
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

            curTime = 0.0f;

            curLoop = loop;
        }

        public override void UpdateFrame(float tick)
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


            if (loop == -1 || curLoop < loop)
            {
                if (Time.frameCount >= planFrame + curFrame + pauseFrame)
                {
                    curLoop++;
                    onComplete();
                    pauseFrame = 0;
                    curFrame = Time.frameCount;
                }
            }
            else
            {
                timerComponent.RemoveTimer(this);
            }
        }

        public override void UpdateLogin(float tick)
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

            curTime += tick;
            if (loop == -1 || curLoop < loop)
            {
                if (curTime >= planTime)
                {
                    curLoop++;
                    onComplete();
                    curTime = 0.0f;
                }
            }
            else
            {
                timerComponent.RemoveTimer(this);
            }
        }
    }
}