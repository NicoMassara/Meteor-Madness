using System;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Core.FlyingObject.Contracts;
using MeteorMadness.GlobalValues.Tools.Observer;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.Core.FlyingObject
{
    public abstract class FlyingObjectView<T> : ManagedBehavior, 
        FlyingObjectView<T>.IFlyingObjectView<T>,
        IPoolable<FlyingObjectView<T>>,
        IFlyingObject,
        IFlyingObjectWithValues<T>,
        IObserver,
        IDebugFlyingObject
        where T : FlyingObjectValues
    {
        #region Interfaces

        public interface IFlyingObjectView<T>
        {
            public event Action<Vector2> OnPositionChanged;
            public event Action<T> OnValuesSet;
            public event Action<Collider2D> OnTriggerEnter;
        }

        #endregion
        
        private FlyingObjectMovement _movement;
        
        #region IFlyingObjectView

        public event Action<Vector2> OnPositionChanged;
        public event Action<T> OnValuesSet;
        public event Action<Collider2D> OnTriggerEnter;

        #endregion

        #region IFlyingObject

        public event Action OnObjectEnabled;
        public event Action OnObjectDisabled;

        #endregion
        
        #region IPoolable

        public event Action<FlyingObjectView<T>> OnRecycle;

        #endregion

        #region IFixedUpdatable

        public abstract UpdateGroup MovementUpdateGroup { get; }
        public abstract TickGroup MovementTickGroup { get; }

        #endregion

        #region IDebugFlyingObject
        
        public float Speed { get; private set; }
        public Vector2 Position { get; private set; }

        #endregion

        protected virtual void Awake()
        {
            var rb2d = GetComponent<Rigidbody2D>();
            _movement = new FlyingObjectMovement(rb2d,MovementUpdateGroup,MovementTickGroup,
                (pos)=> OnPositionChanged?.Invoke(pos));
        }

        #region Observer
        
        public virtual void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case FlyingObjectObserverMessage.SetValues:
                    HandleSetValues((T)args[0]);
                    break;
                case FlyingObjectObserverMessage.HandleCollision:
                    HandleCollision((T)args[0]);
                    break;
                case FlyingObjectObserverMessage.UpdatePosition:
                    HandleUpdatePosition((Vector2)args[0]);
                    break;
            }
        }

        protected virtual void HandleSetValues(T data)
        {
            _movement.SetRigidbodyData(data.MovementSpeed, data.Rotation, data.Position);
            _movement.CanMove = true;
            OnObjectEnabled?.Invoke();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            Speed = data.MovementSpeed;
            Position = transform.position;
#endif
        }
        
        protected virtual void HandleCollision(T data)
        {
            Recycle();
        }
        
        protected virtual void HandleUpdatePosition(Vector2 position)
        {
            Position = position;
        }
        
        #endregion

        public virtual void SetValues(T data)
        {
            OnValuesSet?.Invoke(data);
        }

        protected void DisableObject()
        {
            _movement.CanMove = false;
            OnObjectDisabled?.Invoke();
        }

        public void Recycle()
        {
            OnRecycle?.Invoke(this);
            DisableObject();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            OnTriggerEnter?.Invoke(other);
        }
    }
}