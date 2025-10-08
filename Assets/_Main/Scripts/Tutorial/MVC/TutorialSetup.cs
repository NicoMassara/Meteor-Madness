using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using UnityEngine;

namespace _Main.Scripts.Tutorial.MVC
{
    [RequireComponent(typeof(TutorialView))]
    [RequireComponent(typeof(TutorialUIView))]
    public class TutorialSetup : ManagedBehavior, IUpdatable
    {
        private TutorialMotor _motor;
        private TutorialController _controller;
        private TutorialView _view;
        private TutorialUIView _ui;

        private bool _isEnable;

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Always;
        
        private void Awake()
        {
            _view = GetComponent<TutorialView>();
            _ui = GetComponent<TutorialUIView>();
            
            _motor = new TutorialMotor();
            _controller = new TutorialController(_motor);
            
            _motor.Subscribe(_view);
            _motor.Subscribe(_ui);
            
            SetViewHandlers();
            SubscribeEventBus();
            
            GameEventCaller.Subscribe<GameScreenEvents.SetGameScreen>(EventBus_GameScreen_SetGameScreen);
        }

        private void Start()
        {
            _controller.Initialize();
        }
        
        public void ManagedUpdate()
        {
            if (_isEnable)
            {
                _controller.Execute(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
            }
        }

        private void TutorialEnable()
        {
            _isEnable = true;
            SubscribeEventBus();
            _controller.TransitionToStart();
        }

        private void TutorialDisable()
        {
            _isEnable = false;
            UnsubscribeEventBus();
            _controller.TransitionToDisable();
        }

        #region Event Handlers

        private void SetViewHandlers()
        {
            _ui.OnNext += UI_OnNextHandler;

            _view.OnTutorialEnable += ViewOnTutorialEnable;
        }

        private void ViewOnTutorialEnable()
        {
            TutorialEnable();
        }

        private void UI_OnNextHandler()
        {
            _controller.TransitionToMovement();
        }

        #endregion
        
        #region Event Bus

        private void SubscribeEventBus()
        {
            GameEventCaller.Subscribe<ProjectileEvents.Deflected>(EventBus_Meteor_Deflected);
            GameEventCaller.Subscribe<AbilitiesEvents.SetActive>(EventBus_Abilities_Active);
            GameEventCaller.Subscribe<MeteorEvents.RingActive>(EventBus_Meteor_RingActive);
        }

        private void UnsubscribeEventBus()
        {
            GameEventCaller.Unsubscribe<ProjectileEvents.Deflected>(EventBus_Meteor_Deflected);
            GameEventCaller.Unsubscribe<AbilitiesEvents.SetActive>(EventBus_Abilities_Active);
            GameEventCaller.Unsubscribe<MeteorEvents.RingActive>(EventBus_Meteor_RingActive);
        }
        
        private void EventBus_Meteor_RingActive(MeteorEvents.RingActive input)
        {
            if (input.IsActive == false)
            {
                _controller.SpawnExtraMeteors();
            }
        }

        private void EventBus_GameScreen_SetGameScreen(GameScreenEvents.SetGameScreen input)
        {
            if (input.ScreenType == ScreenType.Tutorial && 
                input.IsEnable)
            {
                _controller.TransitionToEnable();
            }
            else
            {
                TutorialDisable();
            }
        }
        
        private void EventBus_Meteor_Deflected(ProjectileEvents.Deflected input)
        {
            if (input.Type == ProjectileType.Meteor)
            {
                _controller.TransitionToAbility();
            }
        }
        
        private void EventBus_Abilities_Active(AbilitiesEvents.SetActive input)
        {
            if (input.IsActive == false)
            {
                _controller.TransitionToFinish();
            }
        }
        
        #endregion
    }
}