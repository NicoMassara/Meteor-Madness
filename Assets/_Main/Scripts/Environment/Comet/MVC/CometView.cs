using System;
using MeteorMadness.Core.FlyingObject;
using MeteorMadness.Core.FlyingObject.Contracts;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Environment.Comet
{
    public class CometView : FlyingObjectView<CometValues>,
        CometView.ICometView,
        IDebugComet,
        IComet
    {
        [SerializeField] private Transform cometSprite;
        
        internal interface ICometView : IFlyingObjectView<CometValues> { }
        
        public override UpdateGroup MovementUpdateGroup { get; } = UpdateGroup.Effects;
        public override TickGroup MovementTickGroup { get; } = TickGroup.EveryFrame;
        

        protected override void HandleSetValues(CometValues data)
        {
            base.HandleSetValues(data);
            cometSprite.localScale = data.Scale;
            Scale = data.Scale.x;
        }
        private void LifeTimer_OnFinishedHandler()
        {
            Recycle();
        }

        #region IDebugComet

        public float TravelRatio { get; set; }
        public float Distance { get; set; }
        public float Scale { get; set; }

        #endregion
        
    }
}