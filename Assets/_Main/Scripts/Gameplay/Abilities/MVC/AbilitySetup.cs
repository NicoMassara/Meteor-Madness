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
            
            eventBus.Subscribe<AbilitiesEvents.SetEnable>(EventBus_Ability_SetEnable);
            eventBus.Subscribe<AbilitiesEvents.Add>(EventBus_Ability_Add);
            eventBus.Subscribe<GameModeEvents.Start>(EventBus_GameMode_Start);
            eventBus.Subscribe<GameModeEvents.Finish>(EventBus_GameMode_Finish);
            eventBus.Subscribe<GameModeEvents.Disable>(EventBus_GameMode_Disable);
            eventBus.Subscribe<MeteorEvents.RingActive>(EventBus_Meteor_RingActive);
        }

        private void EventBus_Meteor_RingActive(MeteorEvents.RingActive input)
        {
            if (input.IsActive == false)
            {
                _controller.RunActiveTimer();
            }
        }

        private void EventBus_GameMode_Start(GameModeEvents.Start input)
        {
            _controller.TransitionToEnable();
        }

        private void EventBus_GameMode_Disable(GameModeEvents.Disable input)
        {
            _controller.TransitionToRestart();
            _controller.TransitionToDisable();
        }
        
        private void EventBus_GameMode_Finish(GameModeEvents.Finish input)
        {
            _controller.TransitionToRestart();
            _controller.TransitionToDisable();
        }

        private void EventBus_Ability_Add(AbilitiesEvents.Add input)
        {
            _controller.TryAddAbility((int)input.AbilityType);
        }

        private void EventBus_Ability_SetEnable(AbilitiesEvents.SetEnable input)
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