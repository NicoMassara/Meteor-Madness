using System;
using DG.Tweening;
using MeteorMadness.Animations;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Settings
{
    public class SettingsViewAnimation : BaseViewAnimation<SettingsUiAnimationSelector,SettingsUiAnimationComponents>,
        SettingsViewAnimation.IPauseViewAnimation
    {
        
        public interface IPauseViewAnimation : 
            BaseViewAnimation<SettingsUiAnimationSelector,SettingsUiAnimationComponents>.IBaseViewAnimation
        {
            
        }

        [SerializeField] private SettingsUiAnimationData animData;
        
        #region Animators
        private class Animation_MainPanel_Open : SequenceUIAnimator<SettingsUiAnimationComponents.IMainPanel, IPanelData>
        {
            private readonly AnimationHelper.PanelPosition _panel;
            
            public Animation_MainPanel_Open(SettingsUiAnimationComponents.IMainPanel components, IPanelData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.MainPanel, AnimationData.OffscreenPos);
            }
            
            protected override void Initialize()
            {
                UIComponents.MainPanel.gameObject.SetActive(false);
                UIComponents.MainPanel.anchoredPosition = _panel.OffScreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(true))
                    .Append(UIComponents.MainPanel.DOAnchorPos(_panel.StartPos, AnimationData.MovementDuration));
            }
        }

        private class Animation_MainPanel_Close : SequenceUIAnimator<SettingsUiAnimationComponents.IMainPanel, IPanelData>
        {
            private readonly AnimationHelper.PanelPosition _panel;
            
            public Animation_MainPanel_Close(SettingsUiAnimationComponents.IMainPanel components, IPanelData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.MainPanel, AnimationData.OffscreenPos);
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.MainPanel.DOAnchorPos(_panel.OffScreenPos, AnimationData.MovementDuration))
                    .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(false));
            }
        }
        #endregion

        private IUIAnimator _animationPanelOpen;
        private IUIAnimator _animationPanelClose;
        
        private void Start()
        {
            _animationPanelOpen = new Animation_MainPanel_Open(UIComponents, animData.OpenData);
            _animationPanelClose = new Animation_MainPanel_Close(UIComponents, animData.CloseData);
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case SettingsObserverMessage.Enable:
                    HandleEnable();
                    break;
                case SettingsObserverMessage.StartDisable:
                    HandleDisable();
                    break;
            }
        }

        private void HandleEnable()
        {
            PlayAnimation(_animationPanelOpen,TriggerOnPanelOpened);
        }
        
        private void HandleDisable()
        {
            PlayAnimation(_animationPanelClose,TriggerOnPanelClosed);
        }
    }
}