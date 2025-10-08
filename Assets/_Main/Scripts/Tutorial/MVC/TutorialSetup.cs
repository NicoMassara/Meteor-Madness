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
            SetEventBus();
        }

        private void Start()
        {
            _controller.Initialize();
        }
        
        public void ManagedUpdate()
        {
            _controller.Execute(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
        }

        #region Event Handlers

        private void SetViewHandlers()
        {
            _ui.OnFinish += UI_OnFinishHandler;
            _ui.OnNext += UI_OnNextHandler;
        }

        private void UI_OnNextHandler()
        {
            _controller.TransitionToMovement();
        }

        private void UI_OnFinishHandler()
        {
            _controller.TransitionToDisable();
        }

        #endregion
        
        #region Event Bus

        private void SetEventBus()
        {
            //Screen
            GameEventCaller.Subscribe<GameScreenEvents.TutorialEnable>(EventBus_GameScreen_Tutorial);
            GameEventCaller.Subscribe<GameScreenEvents.MainMenuEnable>(EventBus_GameScreen_MainMenu);
            
            //Projectile
            GameEventCaller.Subscribe<ProjectileEvents.Deflected>(EventBus_Meteor_Deflected);
            GameEventCaller.Subscribe<AbilitiesEvents.SetActive>(EventBus_Abilities_Active);
        }

        private void EventBus_GameScreen_MainMenu(GameScreenEvents.MainMenuEnable input)
        {
            _controller.TransitionToDisable();
        }

        private void EventBus_GameScreen_Tutorial(GameScreenEvents.TutorialEnable input)
        {
            _controller.TransitionToStart();
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