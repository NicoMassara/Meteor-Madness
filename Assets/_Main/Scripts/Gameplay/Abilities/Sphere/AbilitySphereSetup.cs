using System;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilities.Sphere
{
    [RequireComponent(typeof(AbilitySphereView))]
    public class AbilitySphereSetup : ManagedBehavior
    {
        [SerializeField] private LayerMask shieldLayerMask;
        [SerializeField] private LayerMask earthLayerMask;
        
        private AbilitySphereMotor _motor;
        private AbilitySphereController _controller;
        private AbilitySphereView _view;

        private void Awake()
        {
            _view = GetComponent<AbilitySphereView>();
            _motor = new AbilitySphereMotor();
            _controller = new AbilitySphereController(_motor,shieldLayerMask,earthLayerMask);
            
            _motor.Subscribe(_view);
            SetViewHandlers();
        }
        
        #region ViewHandlers

        private void SetViewHandlers()
        {
            _view.OnPositionChanged += View_OnPositionChangedHandler;
            _view.OnValuesChanged += View_OnValuesChangedHandler;
            _view.OnCollisionDetected += View_OnCollisionDetectedHandler;
        }

        private void View_OnCollisionDetectedHandler(Collider2D other)
        {
            _controller.HandleTriggerEnter2D(other);
        }

        private void View_OnValuesChangedHandler(AbilitySphereValues values)
        {
            _controller.SetValues(values);
        }

        private void View_OnPositionChangedHandler(Vector2 position)
        {
            _controller.UpdatePosition(position);
        }

        #endregion
    }
}