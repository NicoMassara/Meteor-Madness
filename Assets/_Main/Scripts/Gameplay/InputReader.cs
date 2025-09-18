using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Managers.UpdateManager.Interfaces;
using _Main.Scripts.MyCustoms;
using UnityEngine;

namespace _Main.Scripts.Gameplay
{
    public class InputReader : ManagedBehavior, IUpdatable
    {
        public float MovementDirection { get; private set; }
        public bool HasUsedAbility { get; private set; }
        private bool _areInputEnabled;

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Inputs;
        private void Awake()
        {
            GameManager.Instance.EventManager.Subscribe<SetEnableInputs>(EventBus_OnSetEnableInputs);
        }
        
        public void ManagedUpdate()
        {
            if(_areInputEnabled == false) return;
            
            if (GetIsLeftPressed())
            {
                MovementDirection = 1;
            }
            else if (GetIsRightPressed())
            {
                MovementDirection = -1;
            }
            else
            {
                MovementDirection = 0;
            }

            HasUsedAbility = GetIsDownPressed();

            if (Input.GetKeyDown(KeyCode.P))
            {
                UpdateManager.Instance.IsPaused = !UpdateManager.Instance.IsPaused;
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
        
        private void EventBus_OnSetEnableInputs(SetEnableInputs input)
        {
            _areInputEnabled = input.IsEnabled;
        }


    }
}