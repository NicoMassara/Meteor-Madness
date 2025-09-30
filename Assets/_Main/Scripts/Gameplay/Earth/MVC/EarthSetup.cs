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
            _controller = new EarthController(_motor);

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
            
            eventBus.Subscribe<MeteorEvents.Collision>(EventBus_OnMeteorCollision);
            eventBus.Subscribe<EarthEvents.Restart>(EventBus_OnEarthRestart);
            eventBus.Subscribe<EarthEvents.DestructionStart>(EventBus_OnEarthStartDestruction);
            eventBus.Subscribe<EarthEvents.Heal>(EventBus_OnHealEarth);
            eventBus.Subscribe<GameModeEvents.SetEnable>(EventBus_OnGameModeEnable);
            eventBus.Subscribe<EarthEvents.SetEnableDamage>(EventBus_OnEarthCanTakeDamage);
        }

        private void EventBus_OnEarthCanTakeDamage(EarthEvents.SetEnableDamage input)
        {
            _controller.SetEnableDamage(input.DamageEnable);
        }

        private void EventBus_OnGameModeEnable(GameModeEvents.SetEnable input)
        {
            if (input.IsEnabled)
            {
                
            }
            else
            {
                _controller.TransitionToDefault();
            }
        }

        private void EventBus_OnHealEarth(EarthEvents.Heal input)
        {
            _controller.Heal(1f);
        }

        private void EventBus_OnEarthRestart(EarthEvents.Restart input)
        {
            _controller.TransitionToHeal();
        }

        private void EventBus_OnEarthStartDestruction(EarthEvents.DestructionStart input)
        {
            _controller.TransitionToShaking();
        }
        
        private void EventBus_OnMeteorCollision(MeteorEvents.Collision input)
        {
            _controller.HandleCollision(GameManager.Instance.GetMeteorDamage(), 
                input.Position, input.Rotation, input.Direction);
        }
        
        #endregion
    }
}