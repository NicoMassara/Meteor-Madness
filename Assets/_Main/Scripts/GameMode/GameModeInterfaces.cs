using _Main.Scripts.MyAnimations;
using UnityEngine;

namespace _Main.Scripts.GameMode
{
    public interface IGameplayPanelData : IUiAnimationData
    {
        public AnimationHelper.Direction ScoreOffscreenPos { get; }
        public AnimationHelper.Direction PauseOffscreenPos { get; }
        public Vector2 ScoreOffset { get; }
        public Vector2 PauseOffset { get; }
        public float MovementDuration { get; }
        public float FinishDelay { get; }
    }

    public interface IScoreFinishAdding : IUiAnimationData
    {
        public float BounceScale { get; }
        public float BounceDuration { get; }
        public float BounceReturnTime { get; }
    }
    
    
    public interface IStreakFailed : IUiAnimationData
    {
        public float Duration { get; }
        public float HorizontalOffset { get; }
        public int Loops { get; }
    }

    public interface ICountdownUpdate : IUiAnimationData
    {
        public float FadeDuration { get; }
        public float BounceDuration { get; }
        public float FadeDelay { get; }
        public float FinalFadeDuration { get; }
        public float AeScaleDuration { get; }
        public float AeFinishDelay { get; }
    }

    public interface ICountdownFinish : IUiAnimationData
    {
        public float FadeDuration { get; }
        public float BounceDuration { get; }
        public float BounceScale { get; }
        public float FadeDelay { get; }
        public float FinalFadeDuration { get; }
        public float AeScaleDuration { get; }
        public float AeBounceScale { get; }
        public float AeScaleDelay { get; }
        public float AeFinishScaleDuration { get; }
        public float FinishDelay { get; }
    }

    public interface IStreakNotify : IUiAnimationData
    {
        public AnimationHelper.Direction StartPosition { get; }
        public AnimationHelper.Direction EndPosition { get; }
        public float MovementDuration { get; }
        public float StopDuration { get; }
        
        public Vector2 StartOffset { get; }
        public Vector2 EndOffset { get; }
    }
}