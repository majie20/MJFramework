using UnityEngine;

namespace Model
{
    [LifeCycle]
    [ComponentOf(typeof(MainCharacterComponent))]
    public class MainCharacterCtrlComponent : Component, IAwake, IStartSystem, IUpdateSystem
    {
        private bool isMove;

        private Player_InputControl _inputControl;

        public Vector2 PrimaryMovement => _inputControl.Player.Move.ReadValue<Vector2>();

        public void Awake()
        {
            isMove = false;

            _inputControl = new Player_InputControl();
            _inputControl.Enable();

            var aa = new Player_InputControl();
            aa.Enable();

            _inputControl.Player.Attack.performed += context => { this.Entity.EventSystem.Invoke<E_CharacterAttack>(); };

            _inputControl.Player.Jump.performed += context =>
            {
                CharacterComponent characterComponent = Entity.GetComponent<CharacterComponent>();

                if (!characterComponent.IsFloating)
                {
                    this.Entity.EventSystem.Invoke<E_CharacterJump>();
                }
            };

            //Game.Instance.EventSystem.AddListener<E_ButtonPressedFirstTimeRun>(this, OnButtonPressedFirstTimeRun);
            //Game.Instance.EventSystem.AddListener<E_ButtonPressedFirstTimeJump>(this, OnButtonPressedFirstTimeJump);
        }

        public void Start()
        {
        }

        public override void Dispose()
        {
            _inputControl.Disable();
            _inputControl.Dispose();
            _inputControl = null;
            base.Dispose();
        }

        [FunctionSort(10000)]
        public void OnUpdate(float tick)
        {
            CharacterComponent characterComponent = this.Entity.GetComponent<CharacterComponent>();

            if (!characterComponent.IsCanMove)
            {
                if (!isMove) return;
                isMove = false;
                this.Entity.EventSystem.Invoke<E_CharacterMove, MoveType, Vector2>(MoveType.StopMove, Vector2.zero);

                return;
            }

            var move = PrimaryMovement;

            if (Mathf.Abs(move.x) > 0.99f)
            {
                isMove = true;
                this.Entity.EventSystem.Invoke<E_CharacterMove, MoveType, Vector2>(MoveType.Walking, move.x > 0 ? Vector2.right : Vector2.left);
            }
            else
            {
                if (!isMove) return;
                isMove = false;
                this.Entity.EventSystem.Invoke<E_CharacterMove, MoveType, Vector2>(MoveType.StopMove, Vector2.zero);
            }
        }

        //private void OnButtonPressedFirstTimeRun()
        //{
        //    this.Entity.EventSystem.Invoke<E_CharacterAttack>();
        //}

        //private void OnButtonPressedFirstTimeJump()
        //{
        //    CharacterComponent characterComponent = Entity.GetComponent<CharacterComponent>();

        //    if (!characterComponent.IsFloating)
        //    {
        //        this.Entity.EventSystem.Invoke<E_CharacterJump>();
        //    }
        //}
    }
}