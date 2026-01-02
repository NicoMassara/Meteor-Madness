
using MeteorMadness.Animations;
using MeteorMadness.ScreenFlow.Base;

namespace MeteorMadness.ScreenFlow.Ability
{
    public interface IPanelData : IUiAnimationData
    {
        AnimationHelper.Direction OffscreenPosition { get; }
        float MovementDuration { get; }
    }
}