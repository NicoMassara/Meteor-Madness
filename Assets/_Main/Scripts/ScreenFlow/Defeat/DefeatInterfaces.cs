using MeteorMadness.Animations;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Defeat
{

    public interface IPanelOpen : IUiAnimationData
    {
        float FadeInDelay { get; }
        float FadeIntensity { get; }
        float FadeInDuration { get; }
        float ScaleDelay { get; }
        float ScaleDuration { get; }
        float BounceDelay { get; }
        float BounceScale { get; }
        float BounceDuration { get; }
        float BounceReturnTime { get; }
        float FinishDelay { get; }
    }
    public interface IPanelClose : IUiAnimationData
    {
        public float MovementDelay { get; }
        public float MovementDuration { get; }
        public AnimationHelper.Direction CurrentScoreOffscreenPosition { get; }
        public AnimationHelper.Direction TitleOffscreenPosition { get; }
        public AnimationHelper.Direction CoinsOffscreenPosition { get; }
        public AnimationHelper.Direction ButtonsOffscreenPosition { get; }
        public float FadeDelay { get; }
        public float BackgroundFadeDuration { get; }
        public Vector2 ButtonsOffScreenOffset { get; }
        public Vector2 CoinsOffScreenOffset { get; }
        public Vector2 TitleOffScreenOffset { get; }
        public Vector2 PointsOffScreenOffset { get; }
    }
    public interface ICurrentScoreIncrement : IUiAnimationData
    {
        public AnimationHelper.Direction OffscreenPosition { get; }
        public Vector2 OffscreenOffset { get; }
        public float MoveDuration { get; }
        public float FinishDelay { get; }
    }
    public interface IHighScoreIncrement : IUiAnimationData
    {
        public float NewScoreTextDelay { get; }
        public float NewScoreBounceScale { get; }
        public float NewScoreBounceDuration { get; }
        public float NewScoreBounceReturnTime { get; }
        public float FinishDelay { get;}
    }
    
    public interface IHighScoreBounce : IUiAnimationData
    {
        public float LoopDelay { get; }
        public float ScaleDuration { get; }
        public float TargetScale { get; }
        public float BounceDelay { get; }
        public float BounceDuration { get; }
        public float MinScale { get; }
    }
    
    public interface IButtonsOpen : IUiAnimationData
    {
        public AnimationHelper.Direction OffscreenPosition { get; }
        public Vector2 OffscreenOffset { get; }
        public float MoveDuration { get; }
        public float FinishDelay { get; }
    }

    public interface ICoinsOpen : IUiAnimationData
    {
        public AnimationHelper.Direction OffscreenPosition { get; }
        public Vector2 OffscreenOffset { get; }
        public float MoveDuration { get; }
        public float FinishDelay { get; }
    }
}