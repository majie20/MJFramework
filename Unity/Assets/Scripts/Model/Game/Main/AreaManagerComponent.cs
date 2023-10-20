using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class AreaManagerComponent : Component, IAwake, IStartSystem, IUpdateSystem
    {
        private AreaEventTriggerHandle _handle;

        private int         _count;
        private float       _time;
        private float       _delayTime;
        private bool        _isOpen;
        private float       _interval;
        private int         _loop;
        private WrapMode    _wrapMode;
        private TriggerMode _triggerMode;

        public void Awake()
        {
            _handle = Entity.Transform.GetComponent<AreaEventTriggerHandle>();
            _interval = _handle.Interval;
            _loop = _handle.Loop;
            _wrapMode = _handle.WrapMode;
            _triggerMode = _handle.TriggerMode;
            _isOpen = _wrapMode == WrapMode.Loop;
            _time = 0f;
            _delayTime = _time + _interval;
            _count = 1;

            if (_triggerMode == TriggerMode.Trigger)
            {
                this.Entity.EventSystem.AddListener<E_TriggerEnter2D, UnitColliderType, Collider2D>(this, OnTriggerEnter2D);
                this.Entity.EventSystem.AddListener<E_TriggerStay2D, UnitColliderType, Collider2D>(this, OnTriggerStay2D);
            }
            else if (_triggerMode == TriggerMode.Event)
            {
            }
        }

        public override void Dispose()
        {
            _handle = null;
            base.Dispose();
        }

        public void Start()
        {
            if (_triggerMode == TriggerMode.PlayOnStart)
            {
                if (_wrapMode == WrapMode.Once)
                {
                    Run();
                    Finish();
                }
                else
                {
                    Entity.TimeHandle(Run, (int) _interval * 1000, _loop, true, Finish);
                }
            }
        }

        [FunctionSort(FunctionLayer.Low)]
        public void OnUpdate(float tick)
        {
            if (_wrapMode == WrapMode.Loop && !_isOpen)
            {
                if (_time >= _delayTime)
                {
                    _isOpen = true;
                    _delayTime = _time + _interval;
                    _count++;
                }

                _time += tick;
            }
        }

        public void OnTriggerEnter2D(UnitColliderType type, Collider2D collider2D)
        {
            if (collider2D.gameObject.CompareTag(_handle.TrgetTag))
            {
                RunTrigger();
            }
        }

        public void OnTriggerStay2D(UnitColliderType type, Collider2D collider2D)
        {
            if (collider2D.gameObject.CompareTag(_handle.TrgetTag))
            {
                RunTrigger();
            }
        }

        private void RunTrigger()
        {
            if (_wrapMode == WrapMode.Loop)
            {
                if (_isOpen)
                {
                    _isOpen = false;
                    Run();

                    if (_loop > 0 && _count >= _loop)
                    {
                        Finish();
                    }
                }
            }
            else
            {
                Run();
                Finish();
            }
        }

        private void Run()
        {
            UniTask.Void(async () =>
            {
                var parent = Game.Instance.GGetComponent<GameRootComponent>().Entity;
                GameConfigDataComponent gameConfigDataComponent = Game.Instance.Scene.GetComponent<GameConfigDataComponent>();

                for (int i = _handle.UnitBornHandles.Length - 1; i >= 0; i--)
                {
                    var handle = _handle.UnitBornHandles[i];
                    var data = gameConfigDataComponent.JsonTables.TbMonster.Get(handle.UnitBaseDataSettings.UnitId);
                    var entity = Game.Instance.Scene.GetComponent<UnitDataComponent>().CreateUnit(data.TypeCode, parent, null, handle.transform);
                    entity.EventSystem.Invoke<E_UnitDataInitialize, int>(handle.UnitBaseDataSettings.UnitId);
                    entity.EventSystem.Invoke<E_UnitBehaviorInitialize, UnitBornHandle>(handle);
                }
            });
        }

        private void Finish()
        {
            Entity.GameObject.SetActive(false);
            Entity.Dispose();
        }
    }
}