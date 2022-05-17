using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Profiling;

namespace Model
{
    [LifeCycle]
    public class TimerComponent : Component, IUpdateSystem,IAwake
    {
        public List<TimerBase> TimerBases;

        public Action<float> OnTimerUpdate;

        public Dictionary<string, List<TimerBase>> TimerDictionary;
        public T GetCache<T>(string name) where T : TimerBase, new()
        {
            var timerComponent = Game.Instance.Scene.GetComponent<TimerComponent>();
            List<TimerBase> list;
            T timer;
            if (TimerDictionary.ContainsKey(name)&& TimerDictionary.TryGetValue(name, out list))
            {
                if (list.Count > 0)
                {
                    timer = list[list.Count - 1] as T;
                    list.RemoveAt(list.Count - 1);
                    return timer;
                }
            }
            timer = new T();
            return timer;
        }

        public void AddTimer(TimerBase timerBase)
        {
            TimerBases.Add(timerBase);

            if (timerBase.isFrame)
            {
                OnTimerUpdate+=timerBase.UpdateFrame;
            }
            else
            {
                OnTimerUpdate += timerBase.UpdateLogin;
            }
        }

        public void RemoveTimer(TimerBase timerBase)
        {
            for (int i = TimerBases.Count - 1; i >= 0; i--)
            {
                if (TimerBases[i] == timerBase)
                {
                    TimerBases.RemoveAt(i);

                    if (timerBase.isFrame)
                    {
                        OnTimerUpdate -= timerBase.UpdateFrame;
                    }
                    else
                    {
                        OnTimerUpdate -= timerBase.UpdateLogin;
                    }

                    string name = timerBase.GetType().Name;
                    if (TimerDictionary.ContainsKey(name))
                    {
                        TimerDictionary[name].Add(timerBase);
                    }
                    else
                    {
                        var list = new List<TimerBase>();
                        list.Add(timerBase);
                        TimerDictionary.Add(name, list);
                    }
                }
            }
        }

        public void Awake()
        {
            TimerBases = new List<TimerBase>();
            TimerDictionary=new Dictionary<string, List<TimerBase>>();
        }

        public override void Dispose()
        {
            TimerBases = null;
            TimerDictionary = null;
            base.Dispose();
        }
        public void OnUpdate(float tick)
        {
            OnTimerUpdate?.Invoke(tick);
        }
    }
}