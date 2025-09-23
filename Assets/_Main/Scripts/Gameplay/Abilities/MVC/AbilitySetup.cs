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
            _motor = new AbilityMotor(GameValues.MaxAbilityCount);
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
                _controller.SelectAbility();
            }
        }

        #region Event Bus

        private void EventBusSetup()
        {
            var eventBus = GameManager.Instance.EventManager;
            
            eventBus.Subscribe<SetEnableAbility>(EventBus_OnSetEnableAbility);
            eventBus.Subscribe<AddAbility>(EventBus_OnAddAbility);
            eventBus.Subscribe<GameFinished>(EventBus_OnGameFinished);
        }

        private void EventBus_OnGameFinished(GameFinished input)
        {
            _controller.RestartAbilities();
        }

        private void EventBus_OnAddAbility(AddAbility input)
        {
            _controller.TryAddAbility(input.AbilityType);
        }

        private void EventBus_OnSetEnableAbility(SetEnableAbility input)
        {
            if (input.IsEnable)
            {
                _controller.TransitionToEnable();
            }
            else
            {
                _controller.TransitionToDisable();
            }
        }

        #endregion
        
    }
}