using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.Managers;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;
using MeteorMadness.Managers.GameConfig;

namespace _Main.Scripts.Earth
{
    [RequireComponent(typeof(EarthView))]
    public class EarthSetup : ManagedBehavior, IUpdatable
    {
        private EarthMotor _motor;
        private EarthController.IEarthController _controller;
        private EarthView _view;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        public TickGroup SelfTickGroup { get; } = TickGroup.HalfTarget;
        
        
        private void Awake()
        {
            BootEvents.OnSubSystemRequestInitialize += Initialize;
            _view = GetComponent<EarthView>();
        }

        private void Initialize()
        {
            BootEvents.OnSubSystemRequestInitialize -= Initialize;
            //
            
            _motor = new EarthMotor();
            _motor.Subscribe(_view);
            _controller = new EarthController(_motor, GameConfigManager.Instance.GetGameplayData().EarthTimeData.Destruction);
            _controller.Initialize();

            SetViewHandlers();
            SetEventBus();
            
            BootEvents.SubSystemInitialized();
        }

        public void ExecuteUpdate(float deltaTime)
        {
            _controller?.Execute(deltaTime);
        }

        #region ViewHandlers

        private void SetViewHandlers()
        {
            _view.OnHealed += _controller.FinishHealing;
        }

        #endregion

        #region Event Bus

        private void SetEventBus()
        {
            EarthEventSubscriber.Restart(EventBus_Earth_Restart);
            EarthEventSubscriber.DestructionStart(EventBus_Earth_DestructionStart);
            EarthEventSubscriber.Heal(EventBus_Earth_Heal);
            EarthEventSubscriber.EnableDamage(EventBus_Earth_Damage_Enable);
            EarthEventSubscriber.DisableDamage(EventBus_Earth_Damage_Disable);
            EarthEventSubscriber.PreSlice(EventBus_Earth_PreSlice);
            //
            ProjectileEventSubscriber.Collision(EventBus_Meteor_Collision);
        }



        #region Earth

        private void EventBus_Earth_PreSlice(EarthEvents.PreSlice input)
        {
            _controller.TryPreSlice();
        }
        
        private void EventBus_Earth_Damage_Enable(EarthEvents.EnableDamage input)
        {
            _controller.EnableDamage();
        }
        
        private void EventBus_Earth_Damage_Disable(EarthEvents.DisableDamage input)
        {
            _controller.DisableDamage();
        }
        
        private void EventBus_Earth_Heal(EarthEvents.Heal input)
        {
            _controller.TryHeal(1f);
        }
        
        private void EventBus_Earth_Restart(EarthEvents.Restart input)
        {
            _controller.TryRestart();
        }

        private void EventBus_Earth_DestructionStart(EarthEvents.DestructionStart input)
        {
            _controller.TryShake();
        }
        #endregion

        #region Meteor

        private void EventBus_Meteor_Collision(ProjectileEvents.Collision input)
        {
            _controller.TryCollision(GameConfigManager.Instance.GetDamageValue(), 
                input.Position, input.Rotation, input.Direction);
        }

        #endregion
        
        #endregion
    }
}