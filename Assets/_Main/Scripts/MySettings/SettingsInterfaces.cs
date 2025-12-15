using _Main.Scripts.MyAnimations;
using UnityEngine;

namespace _Main.Scripts.MySettings
{
    public interface IPanelData : IUiAnimationData
    {
        AnimationHelper.Direction OffscreenPos { get; }
        float MovementDuration { get; }
        public Vector2 Offset { get; }
    }


}