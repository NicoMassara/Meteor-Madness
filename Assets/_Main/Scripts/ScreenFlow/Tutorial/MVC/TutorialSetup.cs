using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.GlobalValues;
using MeteorMadness.Managers;
using NicolasMassara.CustomTimerManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Tutorial
{
    [RequireComponent(typeof(TutorialView))]
    [RequireComponent(typeof(TutorialUIView))]
    [RequireComponent(typeof(TutorialViewAnimation))]
    public class TutorialSetup : ManagedBehavior, IUpdatable
    {
        private TutorialController.ITutorialController _controller;
        private TutorialView _view;
        private TutorialViewAnimation _animator;
        private TutorialUIView _ui;

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Always;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;


        private void Awake()
        {
            _view = GetComponent<TutorialView>();
            _ui = GetComponent<TutorialUIView>();
            _animator = GetComponent<TutorialViewAnimation>();

            var motor = new TutorialMotor();
            _controller = new TutorialController(motor);

            motor.Subscribe(_view);
            motor.Subscribe(_ui);
            motor.Subscribe(_animator);

            SetViewHandlers();

            GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
            GameScreenEventSubscriber.DisableScreen(EventBus_GameScreen_Disable);
        }

        private void Start()
        {
            _controller.Initialize();
        }

        public void ExecuteUpdate(float deltaTime)
        {
            _controller?.Execute(deltaTime);
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
            _view.OnTutorialEnable += ViewOnTutorialEnable;
            _view.OnTutorialFinished += _controller.TransitionToMultiPage;
            _view.OnRightMovementFinished += _controller.TransitionToMultiPage;
            _view.OnLeftMovementFinished += _controller.TransitionToMultiPage;
            //
            _ui.OnHintTextEnable += _controller.EnableHint;
            _ui.OnHintTextDisable += _controller.DisableHint;
        }

        private void ViewOnTutorialEnable()
        {
            _controller.TransitionToMultiPage();
        }

        #endregion

        #region Event Bus

        private void SubscribeEventBus()
        {
            ProjectileEventSubscriber.Deflected(EventBus_Meteor_Deflected);
            ProjectileEventSubscriber.Collision(EventBus_Projectile_Collision);
            //
            AbilitiesEventSubscriber.NotifyIsActive(EventBus_Abilities_Active);
            //
            MeteorEventSubscriber.RingActive(EventBus_Meteor_RingActive);
            //
            MultiPageUIEventSubscriber.Finished(EventBus_MultiPage_Finished);
            //
            ShieldEventSubscriber.NotifyMovement(EventBus_Shield_Movement);
        }

        private void UnsubscribeEventBus()
        {
            ProjectileEventUnSubscriber.Deflected(EventBus_Meteor_Deflected);
            ProjectileEventUnSubscriber.Collision(EventBus_Projectile_Collision);
            //
            AbilitiesEventUnSubscriber.NotifyIsActive(EventBus_Abilities_Active);
            //
            MeteorEventUnSubscriber.RingActive(EventBus_Meteor_RingActive);
            //
            MultiPageUIEventUnSubscriber.Finished(EventBus_MultiPage_Finished);
            //
            ShieldEventUnSubscriber.NotifyMovement(EventBus_Shield_Movement);
        }

        private void EventBus_MultiPage_Finished(MultiPageUIEvents.Finished input)
        {
            switch (input.CreateId)
            {
                case 0:
                    _controller.TransitionToMoveRight();
                    break;
                case 1:
                    _controller.TransitionToMoveLeft();
                    break;
                case 2:
                    _controller.TransitionToMeteor();
                    break;
                case 3:
                    _controller.TransitionToAbility();
                    break;
                case 4:
                    GameManager.Instance.LoadMainMenu();
                    break;
                default:
                    Debug.Log("MultiPage_Finished - Finish Action Not Found");
                    break;
            }
        }

        private void EventBus_Shield_Movement(ShieldEvents.NotifyMovement input)
        {
            _controller.SetMovementDirection(input.Direction);
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
                TimerManager.Add(new TimerData(0.5f,
                    () => _controller.TransitionToMultiPage()));
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
            if (input.ScreenType != ScreenType.Tutorial) return;

            if (input.RequestType == EventRequestType.Requested)
            {
                DisableTutorial();
            }
        }

        private void EventBus_GameScreen_Enable(GameScreenEvents.EnableScreen input)
        {
            if (input.ScreenType != ScreenType.Tutorial) return;

            if (input.RequestType == EventRequestType.Granted)
            {
                EnableTutorial();
            }
        }

        #endregion

        #endregion
    }
}