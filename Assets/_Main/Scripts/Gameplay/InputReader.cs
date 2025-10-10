using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay
{
    public class InputReader : ManagedBehavior, IUpdatable
    {
        public float MovementDirection { get; private set; }
        public bool HasUsedAbility { get; private set; }
        private bool _areInputEnabled;

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Inputs;
        
        public UnityAction<int> OnMovementDirectionChanged;
        public UnityAction OnStopMovement;
        private void Awake()
        {
            GameManager.Instance.EventManager.Subscribe<InputsEvents.SetEnable>(EventBus_Inputs_SetEnable);
        }

        public void ManagedUpdate()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if(_areInputEnabled == false) return;
            
            if (GetIsLeftPressed())
            {
                MovementDirection = 1;
                OnMovementDirectionChanged?.Invoke(1);
            }
            else if (GetIsRightPressed())
            {
                MovementDirection = -1;
                OnMovementDirectionChanged?.Invoke(-1);
            }
            else if (GetIsLeftUp() || GetIsRightUp())
            {
                MovementDirection = 0;
                OnStopMovement?.Invoke();
            }

            HasUsedAbility = GetIsDownPressed();

            if (Input.GetKeyDown(KeyCode.P))
            {
                GameManager.Instance.EventManager.Publish(
                    new GameModeEvents.SetPause{IsPaused = !GameManager.Instance.IsPaused});
            }
        }

        private bool GetIsLeftPressed()
        {
            return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        }

        private bool GetIsRightPressed()
        {
            return Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        }

        private bool GetIsDownPressed()
        {
            return Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
        }

        private bool GetIsLeftUp()
        {
            return Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow);
        }
        
        private bool GetIsRightUp()
        {
            return Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow);
        }

        private void EventBus_Inputs_SetEnable(InputsEvents.SetEnable input)
        {
            _areInputEnabled = input.IsEnable;
        }


    }
}