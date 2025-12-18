using _Main.Scripts.MyAnimations;
using UnityEngine;

namespace _Main.Scripts.MultiPage
{
    public interface IPanelData : IUiAnimationData
    {
        float MovementDuration { get; }
        AnimationHelper.Direction OffscreenPosition { get; }
        public Vector2 Offset { get; }
    }
    
}