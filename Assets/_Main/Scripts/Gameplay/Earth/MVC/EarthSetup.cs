using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Earth
{
    [RequireComponent(typeof(EarthView))]
    public class EarthSetup : ManagedBehavior, IUpdatable
    {
        private EarthMotor _motor;
        private EarthController _controller;
        private EarthView _view;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        
        private void Awake()
        {
            _view = GetComponent<EarthView>();
            
            _motor = new EarthMotor();
            _motor.Subscribe(_view);
            _controller = new EarthController(_motor, GameConfigManager.Instance.GetGameplayData().EarthTimeData.Destruction);

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

        #region ViewHandlers

        private void SetViewHandlers()
        {
            _view.OnHealed += View_OnHealedHandler;
        }

        private void View_OnHealedHandler()
        {
            _controller.TransitionToDefault();
        }

        #endregion

        #region Event Bus

        private void SetEventBus()
        {
            var eventBus = GameManager.Instance.EventManager;
            
            eventBus.Subscribe<EarthEvents.Restart>(EventBus_Earth_Restart);
            eventBus.Subscribe<EarthEvents.DestructionStart>(EventBus_Earth_DestructionStart);
            eventBus.Subscribe<EarthEvents.Heal>(EventBus_Earth_Heal);
            eventBus.Subscribe<EarthEvents.SetEnableDamage>(EventBus_Earth_SetEnableDamage);
            eventBus.Subscribe<GameModeEvents.Disable>(EventBus_GameMode_Disable);
            eventBus.Subscribe<MeteorEvents.Collision>(EventBus_Meteor_Collision);
        }

        private void EventBus_Earth_SetEnableDamage(EarthEvents.SetEnableDamage input)
        {
            _controller.SetEnableDamage(input.DamageEnable);
        }

        private void EventBus_GameMode_Disable(GameModeEvents.Disable input)
        {
            _controller.TransitionToDefault();
        }

        private void EventBus_Earth_Heal(EarthEvents.Heal input)
        {
            _controller.Heal(1f);
        }

        private void EventBus_Earth_Restart(EarthEvents.Restart input)
        {
            _controller.TransitionToHeal();
        }

        private void EventBus_Earth_DestructionStart(EarthEvents.DestructionStart input)
        {
            _controller.TransitionToShaking();
        }
        
        private void EventBus_Meteor_Collision(MeteorEvents.Collision input)
        {
            _controller.HandleCollision(GameConfigManager.Instance.GetDamageValue(), 
                input.Position, input.Rotation, input.Direction);
        }
        
        #endregion
    }
}