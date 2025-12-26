using MeteorMadness.Animations;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Tutorial
{

    public interface IPanelData : IUiAnimationData
    {
        public float MovementDuration { get; }
        public AnimationHelper.Direction OffscreenPosition { get; }
        public Vector2 Offset { get; }
    }
    
}