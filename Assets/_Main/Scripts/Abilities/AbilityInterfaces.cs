using _Main.Scripts.MyAnimations;

namespace _Main.Scripts.Abilities
{
    
    public interface IPanelData : IUiAnimationData
    {
        AnimationHelper.Direction OffscreenPosition { get; }
        float MovementDuration { get; }
    }

    public interface IAbilityPanelCloseData : IUiAnimationData
    {
        AnimationHelper.Direction OffscreenPosition { get; }
        float MovementDuration { get; }
    }

}