using MeteorMadness.Core.FlyingObject;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Environment.Comet
{
    public class CometView : FlyingObjectView<CometValues>,
        CometView.ICometView,
        IComet
    {
        [SerializeField] private Transform cometSprite;
        
        internal interface ICometView : IFlyingObjectView<CometValues> { }
        
        public override UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Effects;
        public override TickGroup SelfTickGroup { get; } = TickGroup.QuarterTarget;
        public Vector2 Position => transform.position;

        protected override void HandleSetValues(CometValues data)
        {
            base.HandleSetValues(data);
            cometSprite.localScale = data.Scale;
        }
    }
}