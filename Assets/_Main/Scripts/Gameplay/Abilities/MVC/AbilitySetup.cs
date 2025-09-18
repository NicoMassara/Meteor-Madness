using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilies
{
    [RequireComponent(typeof(AbilityView))]
    [RequireComponent(typeof(AbilityUIView))]
    public class AbilitySetup : ManagedBehavior, IUpdatable
    {
        [SerializeField] private InputReader inputReader;
        private AbilityMotor _motor;
        private AbilityController _controller;
        
        private AbilityView _view;
        private AbilityUIView _ui;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Ability;

        private void Awake()
        {
            _motor = new AbilityMotor();
            _controller = new AbilityController(_motor);
            
            _view = GetComponent<AbilityView>();
            _ui = GetComponent<AbilityUIView>();
            
            _motor.Subscribe(_view);
            _motor.Subscribe(_ui);
            
            _view.SetController(_controller);
            
            EventBusSetup();
        }

        private void Start()
        {
            _controller.Initialize();
        }

        public void ManagedUpdate()
        {
            if (inputReader.HasUsedAbility)
            {
                _controller.SelectAbility(AbilityType.SuperShield);
            }
        }

        #region Event Bus

        private void EventBusSetup()
        {
            var eventBus = GameManager.Instance.EventManager;
            
            eventBus.Subscribe<SetEnableAbility>(EventBus_OnSetEnableAbility);
        }

        private void EventBus_OnSetEnableAbility(SetEnableAbility input)
        {
            
        }

        #endregion
        
    }
}