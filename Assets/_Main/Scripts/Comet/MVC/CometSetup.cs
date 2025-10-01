using System;
using _Main.Scripts.FyingObject;
using UnityEngine;

namespace _Main.Scripts.Comet
{
    [RequireComponent(typeof(CometView))]
    public class CometSetup : MonoBehaviour
    {
        [SerializeField] private LayerMask cometWallMask;
        private CometMotor _motor;
        private CometController _controller;
        private CometView _view;

        private void Awake()
        {
            _view = GetComponent<CometView>();
            
            _motor = new CometMotor();
            _controller = new CometController(_motor, cometWallMask);
            
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

        private void View_OnValuesChangedHandler(FlyingObjectValues values)
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