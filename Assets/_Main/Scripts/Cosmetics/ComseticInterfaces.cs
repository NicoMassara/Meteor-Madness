using _Main.Scripts.MyAnimations;

namespace _Main.Scripts.Cosmetics
{
    public interface IPanelData : IUiAnimationData
    {
        public AnimationHelper.Direction OffScreenPos { get; }
        public float MovementDuration { get; }
    }

    public interface IPanelCloseData : IUiAnimationData
    {
        public AnimationHelper.Direction OffScreenPos { get; }
        public float MovementDuration { get; }
    }
}