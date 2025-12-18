using _Main.Scripts.MyAnimations;
using UnityEngine;

namespace _Main.Scripts.GameStats
{
    public interface IPanelData : IUiAnimationData
    {
        public float MovementDuration { get; }
        public AnimationHelper.Direction OffscreenPosition { get; }
        public Vector2 Offset { get; }
    }
}