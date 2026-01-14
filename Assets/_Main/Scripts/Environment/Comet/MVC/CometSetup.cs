using System;
using _Main.Scripts.Core.FlyingObject.Components;
using MeteorMadness.Core.FlyingObject;
using MeteorMadness.Core.FlyingObject.Contracts;
using UnityEngine;

namespace _Main.Scripts.Environment.Comet
{
    [RequireComponent(typeof(CometView))]
    [RequireComponent(typeof(FlyingObjectTrail))]
    internal class CometSetup : FlyingObjectSetup<CometValues, CometView.ICometView, CometController.ICometController>
    {
        protected override void Awake()
        {
            var view = GetComponent<CometView>();
            var motor = new CometMotor();
            
            motor.Subscribe(view);
            
            base.View = view;
            base.Controller  = new CometController(motor);
            
            InitializeViewHandlers();
        }
    }
    
    internal class CometController : FlyingObjectController<CometValues, CometMotor>,
        CometController.ICometController
    {
        internal interface ICometController : IFlyingObjectController<CometValues> { }
        
        internal CometController(CometMotor motor)
            : base(motor) { }
    }
    internal class CometMotor : FlyingObjectMotor<CometValues> { }
}