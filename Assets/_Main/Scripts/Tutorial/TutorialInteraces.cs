using _Main.Scripts.MyAnimations;

namespace _Main.Scripts.Tutorial
{
    using UnityEngine;

    public interface IPanelData : IUiAnimationData
    {
        public float MovementDuration { get; }
        public AnimationHelper.Direction OffscreenPosition { get; }
        public Vector2 Offset { get; }
    }
    
}