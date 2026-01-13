using System;
using MeteorMadness.Core.FlyingObject;
using UnityEngine;

namespace _Main.Scripts.Environment.Comet
{
    [RequireComponent(typeof(CometView))]
    public class CometSetup : MonoBehaviour
    {
        private CometController _controller;
        private CometView.ICometView _view;
        
        private void Awake()
        {
            var view = GetComponent<CometView>();
            var motor = new CometMotor();
            
            motor.Subscribe(view);
            
            _view = view;
            _controller = new CometController(motor);
        }

        private void Start()
        {
            SetViewHandlers();
        }
        
        #region ViewHandlers

        private void SetViewHandlers()
        {
            _view.OnPositionChanged += View_OnPositionChangedHandler;
            _view.OnValuesSet += View_OnValuesSetHandler;
        }

        private void View_OnValuesSetHandler(CometValues values)
        {
            _controller.SetValues(values);
        }

        private void View_OnPositionChangedHandler(Vector2 position)
        {
            _controller.UpdatePosition(position);
        }

        #endregion
    }
    
    public class CometController : FlyingObjectController<CometValues>
    {
        public CometController(FlyingObjectMotor<CometValues> motor)
            : base(motor)
        {
        }
    }
    public class CometMotor : FlyingObjectMotor<CometValues> { }
}