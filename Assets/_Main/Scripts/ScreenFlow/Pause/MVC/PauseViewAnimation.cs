using System;
using DG.Tweening;
using MeteorMadness.Animations;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Pause
{
    public class PauseViewAnimation : BaseViewAnimation<PauseUiAnimationSelector,PauseUIAnimationComponents>,
        PauseViewAnimation.IPauseViewAnimation
    {
        public interface IPauseViewAnimation : BaseViewAnimation<PauseUiAnimationSelector,PauseUIAnimationComponents>.IBaseViewAnimation
        {
            
        }
        
        [SerializeField] private PauseUiAnimationData animData;
        
        #region Animators

        private class Animation_Initialize : UIComponentsInitializer<PauseUIAnimationComponents.IMainPanel>
        {
            public Animation_Initialize(PauseUIAnimationComponents.IMainPanel components) : base(components) { }

            protected override void Initialize()
            {
                UIComponents.MainPanel.gameObject.SetActive(false);
            }
        }

        private class Animation_MainPanel_Open : SequenceUIAnimation<PauseUIAnimationComponents.IMainPanel, IPausePanelOpenData>
        {
            private readonly AnimationHelper.PanelPosition _titlePanel;
            private readonly AnimationHelper.PanelPosition _leftButtonsPanel;
            private readonly AnimationHelper.PanelPosition _pointsTextPanel;
            
            public Animation_MainPanel_Open(PauseUIAnimationComponents.IMainPanel components, IPausePanelOpenData animationData)
                : base(components, animationData)
            {
                _titlePanel = new AnimationHelper.PanelPosition(UIComponents.TitleText, animationData.TitleOffscreenPos, animationData.TitleOffset);
                _leftButtonsPanel = new AnimationHelper.PanelPosition(UIComponents.LeftButtonsPanel, animationData.LeftButtonsPanelOffscreenPos, animationData.LeftButtonsOffset);
                _pointsTextPanel = new AnimationHelper.PanelPosition(UIComponents.PointsText, animationData.PointsTextPanelOffscreenPos, animationData.PointsTextOffset);
            }
            
            protected override void Initialize()
            {
                UIComponents.TitleText.anchoredPosition = _titlePanel.OffScreenPos;
                UIComponents.LeftButtonsPanel.anchoredPosition = _leftButtonsPanel.OffScreenPos;
                UIComponents.PointsText.anchoredPosition = _pointsTextPanel.OffScreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.BackgroundImage.DOFade(0f, 0f))
                    .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(true))
                    .Append(UIComponents.BackgroundImage.DOFade(AnimationData.BackgroundFadeIntensity, AnimationData.BackgroundFadeDuration))
                    .Join(UIComponents.TitleText.DOAnchorPos(_titlePanel.StartPos, AnimationData.MovementDuration))
                    .Join(UIComponents.LeftButtonsPanel.DOAnchorPos(_leftButtonsPanel.StartPos, AnimationData.MovementDuration))
                    .Join(UIComponents.PointsText.DOAnchorPos(_pointsTextPanel.StartPos, AnimationData.MovementDuration))
                    .AppendInterval(AnimationData.FinishDelay);
            }
        }

        private class Animation_MainPanel_Close : SequenceUIAnimation<PauseUIAnimationComponents.IMainPanel, IPausePanelCloseData>
        {
            private readonly AnimationHelper.PanelPosition _titlePanel;
            private readonly AnimationHelper.PanelPosition _leftButtonsPanel;
            private readonly AnimationHelper.PanelPosition _pointsTextPanel;
            
            public Animation_MainPanel_Close(PauseUIAnimationComponents.IMainPanel components, IPausePanelCloseData animationData)
                : base(components, animationData)
            {
                _titlePanel = new AnimationHelper.PanelPosition(UIComponents.TitleText, animationData.TitleOffscreenPos, animationData.TitleOffset);
                _leftButtonsPanel = new AnimationHelper.PanelPosition(UIComponents.LeftButtonsPanel, animationData.LeftButtonsPanelOffscreenPos, animationData.LeftButtonsOffset);
                _pointsTextPanel = new AnimationHelper.PanelPosition(UIComponents.PointsText, animationData.PointsTextPanelOffscreenPos, animationData.PointsTextOffset);
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Join(UIComponents.TitleText.DOAnchorPos(_titlePanel.OffScreenPos, AnimationData.MovementDuration))
                    .Join(UIComponents.LeftButtonsPanel.DOAnchorPos(_leftButtonsPanel.OffScreenPos, AnimationData.MovementDuration))
                    .Join(UIComponents.PointsText.DOAnchorPos(_pointsTextPanel.OffScreenPos, AnimationData.MovementDuration))
                    .Join(UIComponents.BackgroundImage.DOFade(0f, AnimationData.BackgroundFadeDuration))
                    .AppendInterval(AnimationData.FinishDelay)
                    .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(false));
            }
        }

        #endregion

        private IUiAnimation _animationPanelOpen;
        private IUiAnimation _animationPanelClose;
        
        private void Start()
        {
            _animationPanelOpen = new Animation_MainPanel_Open(UIComponents, animData.PanelOpenData);
            _animationPanelClose = new Animation_MainPanel_Close(UIComponents, animData.PanelCloseData);
            
            var initialize = new Animation_Initialize(UIComponents);
        }
        
        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case PauseObserverMessage.Initialize:
                    HandleInitialize();
                    break;
                case PauseObserverMessage.StartDisable:
                    HandleStartDisable();
                    break;
            }
        }
        
        private void HandleInitialize()
        {
            PlayAnimation(_animationPanelOpen, TriggerOnPanelOpened);
        }
        
        private void HandleStartDisable()
        {
            PlayAnimation(_animationPanelClose, TriggerOnPanelClosed);
        }
    }
}