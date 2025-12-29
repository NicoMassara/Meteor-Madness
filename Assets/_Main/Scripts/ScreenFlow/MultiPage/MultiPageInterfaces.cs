using MeteorMadness.Animations;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.MultiPage
{
    public interface IPanelData : IUiAnimationData
    {
        float MovementDuration { get; }
        AnimationHelper.Direction OffscreenPosition { get; }
        public Vector2 Offset { get; }
    }
    
}