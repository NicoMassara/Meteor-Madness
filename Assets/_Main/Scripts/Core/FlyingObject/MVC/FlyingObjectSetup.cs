using System;
using MeteorMadness.Core.FlyingObject.Contracts;
using UnityEngine;

namespace MeteorMadness.Core.FlyingObject
{
    public abstract class FlyingObjectSetup<T, TV, TC> : MonoBehaviour
        where T : FlyingObjectValues
        where TV : IFlyingObjectView<T>
        where TC : IFlyingObjectController<T>
    {
        protected TV View { get; set; }
        protected TC Controller { get; set; }

        protected virtual void Awake()
        {
            
        }
        
        protected virtual void Start()
        {
            
        }
        
        protected virtual void InitializeViewHandlers()
        {
            View.OnPositionChanged += UpdatePosition;
            View.OnValuesSet += SetValues;
        }
        
        private void SetValues(T data) => Controller.SetValues(data);
        private void UpdatePosition(Vector2 position) => Controller.UpdatePosition(position);
    }
}