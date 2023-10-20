using UnityEngine;

namespace Model
{
    public class BaseState
    {
        public StateMachineType Type;
        public StateMachineType InterruptType;

        public    Vector2             Vec;
        public    StateMachineManager Manager { protected set; get; }
        protected AnimationData[]     _datas;
        protected int                 _curIndex;

        public BaseState(StateMachineManager manager, StateMachineType interruptType, AnimationData[] datas)
        {
            _curIndex = -1;
            Manager = manager;
            _datas = datas;
            this.InterruptType = interruptType;
        }

        public bool InterruptJudge(BaseState state, int index)
        {
            if ((InterruptType & state.Type) != 0 || (state.Type == Type && _datas.Length > 1 && (_curIndex != index || Random.value < 0.7f)))
            {
                return ConditionJudge();
            }

            return false;
        }

        public virtual bool ConditionJudge()
        {
            return true;
        }

        public virtual void Enter(int index)
        {
            if (index == -1)
            {
                index = Random.Range(0, _datas.Length - 1);
            }
            else if (_curIndex == index)
            {
                if (index >= _datas.Length - 1)
                {
                    index = 0;
                }
                else
                {
                    index++;
                }
            }

            _curIndex = index;
            var data = _datas[_curIndex];

            if (data.Cutscene != null)
            {
                data.Cutscene.Play();
            }

            Manager.Play(data.Clip);
        }

        //public void Play()
        //{
        //    var data = _datas[_curIndex];

        //    if (data.Cutscene != null)
        //    {
        //        data.Cutscene.Play();
        //    }

        //    Manager.Play(data.Clip);
        //}

        public virtual void Exit()
        {
            var data = _datas[_curIndex];
            data.Cutscene.Stop();
        }

        public virtual void Update(float tick)
        {
        }
    }
}