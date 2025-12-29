using MeteorMadness.Animations;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Settings
{
    public interface IPanelData : IUiAnimationData
    {
        AnimationHelper.Direction OffscreenPos { get; }
        float MovementDuration { get; }
        public Vector2 Offset { get; }
    }


}