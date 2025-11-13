using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.Tutorial.MVC
{
    [RequireComponent(typeof(TutorialView))]
    [RequireComponent(typeof(TutorialUIView))]
    public class TutorialSetup : ManagedBehavior
    {
        private TutorialMotor _motor;
        private TutorialController _controller;
        private TutorialView _view;
        private TutorialUIView _ui;
        
        private void Awake()
        {
            _view = GetComponent<TutorialView>();
            _ui = GetComponent<TutorialUIView>();
            
            _motor = new TutorialMotor();
            _controller = new TutorialController(_motor);
            
            _motor.Subscribe(_view);
            _motor.Subscribe(_ui);
            
            SetViewHandlers();
            
            GameEventCaller.Subscribe<GameScreenEvents.EnableScreen>(EventBus_GameScreen_Enable);
            GameEventCaller.Subscribe<GameScreenEvents.DisableScreen>(EventBus_GameScreen_Disable);
        }

        private void Start()
        {
            _controller.Initialize();
        }

        private void EnableTutorial()
        {
            SubscribeEventBus();
            _controller.TransitionToEnable();
        }

        private void DisableTutorial()
        {
            UnsubscribeEventBus();
            _controller.TransitionToDisable();
        }

        #region Event Handlers

        private void SetViewHandlers()
        {
            _ui.OnStartButtonPressed += UIOnStartTutorialButtonPressedHandler;

            _view.OnTutorialEnable += ViewOnTutorialEnable;
            _view.OnTutorialFinished += _controller.TransitionToMultiPage;
        }

        private void ViewOnTutorialEnable()
        {
            _controller.TransitionToStart();
        }

        private void UIOnStartTutorialButtonPressedHandler()
        {
            _controller.TransitionToMultiPage();
        }

        #endregion
        
        #region Event Bus

        private void SubscribeEventBus()
        {
            GameEventCaller.Subscribe<ProjectileEvents.Deflected>(EventBus_Meteor_Deflected);
            GameEventCaller.Subscribe<ProjectileEvents.Collision>(EventBus_Projectile_Collision);
            //
            GameEventCaller.Subscribe<AbilitiesEvents.NotifyIsActive>(EventBus_Abilities_Active);
            //
            GameEventCaller.Subscribe<MeteorEvents.RingActive>(EventBus_Meteor_RingActive);
            //
            GameEventCaller.Subscribe<MultiPageUIEvents.Finished>(EventBus_MultiPage_Finished);
        }

        private void UnsubscribeEventBus()
        {
            GameEventCaller.Unsubscribe<ProjectileEvents.Collision>(EventBus_Projectile_Collision);
            GameEventCaller.Unsubscribe<ProjectileEvents.Deflected>(EventBus_Meteor_Deflected);
            //
            GameEventCaller.Unsubscribe<AbilitiesEvents.NotifyIsActive>(EventBus_Abilities_Active);
            //
            GameEventCaller.Unsubscribe<MeteorEvents.RingActive>(EventBus_Meteor_RingActive);
            //
            GameEventCaller.Unsubscribe<MultiPageUIEvents.Finished>(EventBus_MultiPage_Finished);
        }
        
        private void EventBus_MultiPage_Finished(MultiPageUIEvents.Finished input)
        {
            switch (input.CreateId)
            {
                case 0:
                    _controller.TransitionToMovement();
                    break;
                case 1:
                    _controller.TransitionToAbility();
                    break;
                case 2: 
                    GameManager.Instance.LoadMainMenu();
                    break;
                default:
                    Debug.Log("MultiPage_Finished - Finish Action Not Found");
                    break;
            }
        }
        
        private void EventBus_Meteor_RingActive(MeteorEvents.RingActive input)
        {
            if (input.IsActive == false)
            {
                _controller.SpawnExtraMeteors();
            }
        }
        
        private void EventBus_Meteor_Deflected(ProjectileEvents.Deflected input)
        {
            if (input.Type == ProjectileType.Meteor)
            {
                TimerManager.Add(new TimerData
                {
                    Time = 0.5f,
                    OnEndAction = ()=> _controller.TransitionToMultiPage()
                }, UpdateGroup.Always);
                
            }
            else if (input.Type == ProjectileType.AbilitySphere)
            {
                _controller.TriggerSphereDeflected();
            }
        }
        
        private void EventBus_Projectile_Collision(ProjectileEvents.Collision input)
        {
            _controller.SendAdditionalProjectile((int)input.Type);
        }

        
        private void EventBus_Abilities_Active(AbilitiesEvents.NotifyIsActive input)
        {
            if (input.IsActive == false)
            {
                _controller.TransitionToFinish();
            }
            else
            {
                _controller.TransitionToAbilityRunning();
            }
        }
        
        #region GameScreen

        private void EventBus_GameScreen_Disable(GameScreenEvents.DisableScreen input)
        {
            if(input.ScreenType != ScreenType.Tutorial) return;
            
            if (input.RequestType == EventRequestType.Requested)
            {
                DisableTutorial();
            }
        }

        private void EventBus_GameScreen_Enable(GameScreenEvents.EnableScreen input)
        {
            if(input.ScreenType != ScreenType.Tutorial) return;
            
            if (input.RequestType == EventRequestType.Granted)
            {
                EnableTutorial();
            }
        }

        #endregion
        
        #endregion
    }
}