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
            
            SetViewHandlers();
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

        #region View Handlers

        private void SetViewHandlers()
        {
            _view.OnAbilityFinished += View_OnAbilityFinishedHandler;
            _view.OnAbilitySelected += View_OnAbilitySelectedHandler;
        }

        private void View_OnAbilitySelectedHandler()
        {
            _controller.TransitionToRunning();
        }

        private void View_OnAbilityFinishedHandler()
        {
            _controller.TransitionToEnable();
        }

        #endregion

        #region Event Bus

        private void EventBusSetup()
        {
            var eventBus = GameManager.Instance.EventManager;
            
            eventBus.Subscribe<AbilitiesEvents.SetEnable>(EventBus_OnSetEnableAbility);
            eventBus.Subscribe<AbilitiesEvents.Add>(EventBus_OnAddAbility);
            eventBus.Subscribe<GameModeEvents.Finish>(EventBus_OnGameFinished);
            eventBus.Subscribe<GameModeEvents.Start>(EventBus_OnGameStart);
            eventBus.Subscribe<GameModeEvents.SetEnable>(EventBus_OnGameModeEnable);
        }

        private void EventBus_OnGameModeEnable(GameModeEvents.SetEnable input)
        {
            if (input.IsEnabled)
            {
                
            }
            else
            {
                _controller.TransitionToRestart();
                _controller.TransitionToDisable();
            }
        }

        private void EventBus_OnGameStart(GameModeEvents.Start input)
        {
            _controller.TransitionToEnable();
        }

        private void EventBus_OnGameFinished(GameModeEvents.Finish input)
        {
            _controller.TransitionToRestart();
            _controller.TransitionToDisable();
        }

        private void EventBus_OnAddAbility(AbilitiesEvents.Add input)
        {
            _controller.TryAddAbility(input.AbilityType);
        }

        private void EventBus_OnSetEnableAbility(AbilitiesEvents.SetEnable input)
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