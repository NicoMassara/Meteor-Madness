using System;
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
            _view.SetController(_controller);
            
            SetEventBus();
        }

        private void Start()
        {
            _controller.Initialize();
        }

        public void ManagedUpdate()
        {
            _controller.Execute(CustomTime.GetChannel(SelfUpdateGroup).DeltaTime);
        }

        #region Event Bus

        private void SetEventBus()
        {
            var eventBus = GameManager.Instance.EventManager;
            
            eventBus.Subscribe<MeteorCollision>(EventBus_OnMeteorCollision);
            eventBus.Subscribe<EarthRestart>(EventBus_OnEarthRestart);
            eventBus.Subscribe<EarthStartDestruction>(EventBus_OnEarthStartDestruction);
        }

        private void EventBus_OnEarthRestart(EarthRestart input)
        {
            _controller.TransitionToDefault();
        }

        private void EventBus_OnEarthStartDestruction(EarthStartDestruction input)
        {
            _controller.TransitionToShaking();
        }
        
        private void EventBus_OnMeteorCollision(MeteorCollision input)
        {
            _controller.HandleCollision(GameManager.Instance.GetMeteorDamage(), 
                input.Position, input.Rotation, input.Direction);
        }
        
        #endregion
    }
}