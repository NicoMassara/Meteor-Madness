using MeteorMadness.Animations;
using MeteorMadness.ScreenFlow.Base;

namespace MeteorMadness.ScreenFlow.Pause
{
    using UnityEngine;

    public interface IPausePanelOpenData : IUiAnimationData
    {
        public AnimationHelper.Direction TitleOffscreenPos { get; }
        public AnimationHelper.Direction LeftButtonsPanelOffscreenPos { get; }
        public AnimationHelper.Direction PointsTextPanelOffscreenPos { get; }
        public Vector2 TitleOffset { get; }
        public Vector2 LeftButtonsOffset { get; }
        public Vector2 PointsTextOffset { get; }
        public float BackgroundFadeDuration { get; }
        public float BackgroundFadeIntensity { get; }
        public float MovementDuration { get; }
        public float FinishDelay { get; }
    }

    public interface IPausePanelCloseData : IUiAnimationData
    {
        public AnimationHelper.Direction TitleOffscreenPos { get; }
        public AnimationHelper.Direction LeftButtonsPanelOffscreenPos { get; }
        public AnimationHelper.Direction PointsTextPanelOffscreenPos { get; }
        public Vector2 TitleOffset { get; }
        public Vector2 LeftButtonsOffset { get; }
        public Vector2 PointsTextOffset { get; }
        public float BackgroundFadeDuration { get; }
        public float BackgroundFadeIntensity { get; }
        public float MovementDuration { get; }
        public float FinishDelay { get; }
    }

}