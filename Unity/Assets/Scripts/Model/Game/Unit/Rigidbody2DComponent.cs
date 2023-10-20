using System;
using System.Collections.Generic;
using M.Algorithm;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    [ComponentOf(typeof(UnitComponent))]
    public class Rigidbody2DComponent : Component, IAwake, IFixedUpdateSystem, IUpdateSystem
    {
        [Flags]
        private enum State
        {
            None = 1,
            Move = 2,
            Jump = 4,
            Hurt = 8,
        }

        public static PhysicsMaterial2D PM2D_FullFriction;
        public static PhysicsMaterial2D PM2D_NoFriction;

        private Rigidbody2D _rigidbody2D;
        private Collider2D  _collider2D;

        private StaticLinkedListDictionary<VelocitySource, VelocityInfo> _velocityInfoMap;
        private List<VelocitySource>                                     _velocitySourceList;
        private List<VelocityInfo>                                       _addVelocityInfoList;

        private RaycastHit2D[]  _raycastHit2DList;
        private ContactFilter2D _contactFilter2D;

        private PhysicsMaterial2D _sharedMaterial;

        private bool  _isUphill;
        private int   _raycastHit2DCount;
        private float _gravityScale;
        private bool  _isInUpdate;
        private State _state;

        public void Awake()
        {
            _isInUpdate = false;
            _isUphill = false;
            _state = State.None;

            _raycastHit2DCount = 0;
            _rigidbody2D = this.Entity.Transform.GetComponent<Rigidbody2D>();
            _raycastHit2DList = new RaycastHit2D[5];
            _velocityInfoMap = new StaticLinkedListDictionary<VelocitySource, VelocityInfo>(0, 5);
            _velocitySourceList = new List<VelocitySource>();
            _addVelocityInfoList = new List<VelocityInfo>();
            _gravityScale = _rigidbody2D.gravityScale;

            this.Entity.EventSystem.AddListener<E_VelocityChange, VelocityInfo>(this, OnVelocityChange);

            var rc = this.Entity.GameObject.GetComponent<ReferenceCollector>();
            _collider2D = rc.Get<GameObject>("Body").GetComponent<Collider2D>();

            _contactFilter2D = new ContactFilter2D {useLayerMask = true, useTriggers = true, layerMask = LayerMask.GetMask("Environment")};

            //Entity.Transform.GetComponent<BoxCollider2D>().offset
        }

        public override void Dispose()
        {
            _velocityInfoMap = null;
            _velocitySourceList = null;
            _addVelocityInfoList = null;
            _rigidbody2D = null;
            _collider2D = null;
            _raycastHit2DList = null;
            _sharedMaterial = null;
            base.Dispose();
        }

        public void OnUpdate(float tick)
        {
            _raycastHit2DCount = _collider2D.Cast(Vector2.down, _contactFilter2D, _raycastHit2DList, UnitValue.DETECT_DISTANCE_FLOOR);
            var result = _raycastHit2DCount == 0;

            CharacterComponent characterComponent = Entity.GetComponent<CharacterComponent>();

            if (characterComponent is not null)
            {
                characterComponent.IsFloating = result;
            }
        }

        public void OnFixedUpdate(float tick)
        {
            //_raycastHit2DCount = _collider2D.Cast(Vector2.down, _contactFilter2D, _raycastHit2DList, UnitValue.DETECT_DISTANCE_FLOOR);
            var result = Entity.GetComponent<CharacterComponent>().IsFloating;

            //CharacterComponent characterComponent = Entity.GetComponent<CharacterComponent>();

            //if (!(characterComponent is null))
            //{
            //    characterComponent.IsFloating = result;
            //}

            _isInUpdate = true;
            _state = State.None;
            _isUphill = false;

            if (_velocityInfoMap.Length > 0)
            {
                var velocity = Vector2.zero;
                var isControl = false;

                if (_velocityInfoMap.ContainsKey(VelocitySource.Hurt))
                {
                    _state = State.Hurt;
                }

                var data = this._velocityInfoMap.GetElement(1);

                while (data.right != 0)
                {
                    data = this._velocityInfoMap.GetElement(data.right);

                    var velocityInfo = data.element;
                    var vec = velocityInfo.Vec;

                    if (_velocitySourceList.Contains(velocityInfo.Source))
                    {
                        continue;
                    }

                    if (velocityInfo.Type == VelocityType.Disposable)
                    {
                        _velocitySourceList.Add(velocityInfo.Source);
                    }
                    else if (vec.sqrMagnitude == 0)
                    {
                        _velocitySourceList.Add(velocityInfo.Source);
                    }

                    if (velocityInfo.Source == VelocitySource.Jump)
                    {
                        if ((_state & State.Hurt) == 0)
                        {
                            _state |= State.Jump;
                        }
                        else
                        {
                            vec = Vector2.zero;
                        }
                    }
                    else if (velocityInfo.Source == VelocitySource.Move)
                    {
                        if ((_state & State.Hurt) == 0)
                        {
                            isControl = true;
                            _state = vec.sqrMagnitude > 0 ? _state | State.Move : _state;
                        }
                        else
                        {
                            vec = Vector2.zero;
                        }

                        if (_raycastHit2DCount > 0)
                        {
                            var normal = Vector2.zero;

                            for (int i = 0; i < _raycastHit2DCount; i++)
                            {
                                normal += _raycastHit2DList[i].normal;
                            }

                            normal = normal.normalized;

                            var distance = vec.magnitude;
                            var direction = vec.normalized;
                            var tangentDirection = new Vector2(normal.y, -normal.x);
                            var tangentDot = Vector2.Dot(tangentDirection, direction);

                            if (tangentDot < 0)
                            {
                                tangentDirection = -tangentDirection;
                                //tangentDot = -tangentDot;
                            }

                            var dot = Mathf.Abs(Vector2.Dot(tangentDirection, Vector2.up));

                            if (dot > 0.05f && dot < 0.9f)
                            {
                                var v = tangentDirection * distance;
                                velocity += (_state & State.Jump) != 0 ? new Vector2(v.x, 0) : v;
                                _isUphill = true;

                                continue;
                            }
                        }
                    }

                    velocity += vec;
                }

                if (isControl)
                {
                    if (_isUphill && (_state & State.Move) != 0)
                    {
                        _rigidbody2D.gravityScale = 0;
                    }
                    else
                    {
                        _rigidbody2D.gravityScale = _gravityScale;
                        velocity += new Vector2(0, (_state & State.Jump) != 0 ? 0 : (_isUphill ? 0 : _rigidbody2D.velocity.y));
                    }
                }
                else
                {
                    _rigidbody2D.gravityScale = _gravityScale;
                    velocity += new Vector2(_rigidbody2D.velocity.x, (_state & State.Jump) != 0 ? 0 : (_isUphill ? 0 : _rigidbody2D.velocity.y));
                }

                _rigidbody2D.velocity = velocity;
            }

            _isInUpdate = false;

            if (_velocitySourceList.Count > 0)
            {
                for (int i = _velocitySourceList.Count - 1; i >= 0; i--)
                {
                    _velocityInfoMap.Remove(_velocitySourceList[i]);
                }

                _velocitySourceList.Clear();
            }

            if (_addVelocityInfoList.Count > 0)
            {
                for (int i = _addVelocityInfoList.Count - 1; i >= 0; i--)
                {
                    _velocityInfoMap.Add(_addVelocityInfoList[i].Source, _addVelocityInfoList[i]);
                }

                _addVelocityInfoList.Clear();
            }

            PhysicsMaterial2D pm2d;

            if ((_state & State.Move) != 0 || (_state & State.Hurt) != 0)
            {
                pm2d = PM2D_NoFriction;
            }
            else
            {
                if (result)
                {
                    pm2d = PM2D_NoFriction;
                }
                else
                {
                    pm2d = PM2D_FullFriction;
                }
            }

            _sharedMaterial = pm2d;

            if (_rigidbody2D.sharedMaterial != _sharedMaterial)
            {
                _rigidbody2D.sharedMaterial = _sharedMaterial;
            }
        }

        public void OnVelocityChange(VelocityInfo info)
        {
            if (info.Vec.sqrMagnitude > 0)
            {
                if (_isInUpdate)
                {
                    if (!_addVelocityInfoList.Contains(info))
                    {
                        _addVelocityInfoList.Add(info);
                    }
                }
                else
                {
                    if (_velocityInfoMap.ContainsKey(info.Source))
                    {
                        _velocityInfoMap[info.Source] = info;
                    }
                    else
                    {
                        _velocityInfoMap.Add(info.Source, info);
                    }
                }
            }
            else
            {
                if (_velocityInfoMap.ContainsKey(info.Source))
                {
                    _velocityInfoMap[info.Source] = info;
                }
            }
        }
    }
}