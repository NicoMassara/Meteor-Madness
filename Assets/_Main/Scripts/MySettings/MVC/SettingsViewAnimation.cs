using _Main.Scripts.MyAnimations;
using _Main.Scripts.Observer;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.MySettings.MVC
{
    public class SettingsViewAnimation : BaseViewAnimation<SettingsUiAnimationSelector,SettingsUiAnimationComponents>,
        SettingsViewAnimation.IPauseViewAnimation
    {
        
        public interface IPauseViewAnimation : 
            BaseViewAnimation<SettingsUiAnimationSelector,SettingsUiAnimationComponents>.IBaseViewAnimation
        {
            
        }
        
        #region Animators
        private class Animation_MainPanel_Open : SequenceUIAnimator<SettingsUiAnimationComponents.IMainPanel>
        {
            public Animation_MainPanel_Open(SettingsUiAnimationComponents.IMainPanel uiComponents) : base(uiComponents) { }
            private const float FadeTime = 0.3f;
            private Vector2 _panelOriginalPos;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                UIComponents.MainPanel.gameObject.SetActive(false);
                
                _panelOriginalPos = UIComponents.MainPanel.anchoredPosition;
                
                _offscreenPos = AnimationHelper.GetOffscreenPos(UIComponents.MainPanel, AnimationHelper.Direction.Right);
                UIComponents.MainPanel.anchoredPosition = _offscreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(true))
                    .Append(UIComponents.MainPanel.DOAnchorPos(_panelOriginalPos, FadeTime));
            }
        }
        
        private class Animation_MainPanel_Close : SequenceUIAnimator<SettingsUiAnimationComponents.IMainPanel>
        {
            public Animation_MainPanel_Close(SettingsUiAnimationComponents.IMainPanel uiComponents) : base(uiComponents) { }
            private const float FadeTime = 0.3f;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                _offscreenPos = AnimationHelper.GetOffscreenPos(UIComponents.MainPanel, AnimationHelper.Direction.Right);
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.MainPanel.DOAnchorPos(_offscreenPos, FadeTime/2))
                    .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(false));
            }
            
        }

        #endregion

        private IUIAnimator _animationPanelOpen;
        private IUIAnimator _animationPanelClose;
        
        private void Start()
        {
            _animationPanelOpen = new Animation_MainPanel_Open(UIComponents);
            _animationPanelClose = new Animation_MainPanel_Close(UIComponents);
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