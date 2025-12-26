
using MeteorMadness.Animations;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Menu
{
    public interface IManuFirstOpenData : IUiAnimationData
    {
        public AnimationHelper.Direction GameTitleOffscreenPos { get; }
        public AnimationHelper.Direction LeftButtonsOffscreenPos { get; }
        public AnimationHelper.Direction RightButtonsOffscreenPos { get; }
        
        public Vector2 TitleOffset { get; }
        public Vector2 LeftButtonsOffset { get; }
        public Vector2 RightButtonsOffset { get; }

        public float ShowTitleDelay { get; }
        public float TitleMovementDuration { get; }
        public float ButtonsMovementDuration { get; }
        public float ShowButtonsDelay { get; }
        public float FinishDelay { get; }
    }

    public interface IMenuPanelData : IUiAnimationData
    {
        public AnimationHelper.Direction GameTitleOffscreenPos { get; }
        public AnimationHelper.Direction LeftButtonsOffscreenPos { get; }
        public AnimationHelper.Direction RightButtonsOffscreenPos { get; }
        
        public Vector2 TitleOffset { get; }
        public Vector2 LeftButtonsOffset { get; }
        public Vector2 RightButtonsOffset { get; }
        
        public float MovementDuration { get; }
        public float FinishDelay { get; }
    }

    public interface IBasePanelData : IUiAnimationData
    {
        public float MovementDuration { get; }
        public AnimationHelper.Direction OffscreenDirection { get; }
        public Vector2 Offset { get; }
    }
}