using System;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Meteor
{
    [RequireComponent(typeof(MeteorView))]
    public class MeteorSetup : MonoBehaviour
    {
        [SerializeField] private LayerMask shieldLayerMask;
        [SerializeField] private LayerMask earthLayerMask;
        
        private MeteorMotor _motor;
        private MeteorController _controller;
        
        private MeteorView _view;

        private void Awake()
        {
            _view = GetComponent<MeteorView>();
            
            _motor = new MeteorMotor();
            _controller = new MeteorController(_motor, shieldLayerMask, earthLayerMask);
            SetViewHandlers();
            _motor.Subscribe(_view);
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

        private void View_OnValuesChangedHandler(MeteorValuesData values)
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