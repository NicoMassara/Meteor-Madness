using System;
using _Main.Scripts.ShieldRotation.Contracts;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Core.FlyingObject;


namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.ProjectileObject
{
    public abstract class ProjectileObjectView<T> : FlyingObjectView<T>,
        IProjectile, 
        ITargetable
        where T : ProjectileData
    {
        protected override void Awake()
        {
            base.Awake();
            
        }

        private void Start()
        {
            OnRecycle += OnRecycleHandler;
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            base.OnNotify(message, args);

            switch (message)
            {
                
            }
        }

        #region ITargetable

        public bool CanBeTargeted { get; private set; }
        public event Action<ITargetable> OnTargetDeath;

        public void DisableTargetable()
        {
            CanBeTargeted = false;
        }

        public void EnableTargetable()
        {
            CanBeTargeted = true;
        }

        #endregion

        #region IProjectile
        
        public void SetEnableMovement(bool enable)
        {
            
        }
        
        #endregion
        
        #region Handlers

        private void OnRecycleHandler(FlyingObjectView<T> input)
        {
            OnTargetDeath?.Invoke(this);
        }
        #endregion
    }
}