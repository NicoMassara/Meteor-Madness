using _Main.Scripts.MyAnimations;

namespace MeteorMadness.ScreenFlow._Main.Scripts_AsDef.ScreenFlow.Ability
{
    public interface IPanelData : IUiAnimationData
    {
        AnimationHelper.Direction OffscreenPosition { get; }
        float MovementDuration { get; }
    }
}