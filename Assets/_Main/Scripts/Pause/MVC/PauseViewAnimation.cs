using _Main.Scripts.MyAnimations;
using _Main.Scripts.Observer;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.Pause
{
    public class PauseViewAnimation : BaseViewAnimation<PauseUiAnimationSelector,PauseUIAnimationComponents>,
        PauseViewAnimation.IPauseViewAnimation
    {
        public interface IPauseViewAnimation : BaseViewAnimation<PauseUiAnimationSelector,PauseUIAnimationComponents>.IBaseViewAnimation
        {
            
        }
        
        #region Animators
        private class Animation_MainPanel_Open : SequenceUIAnimator<PauseUIAnimationComponents.IMainPanel>
        {
            public Animation_MainPanel_Open(PauseUIAnimationComponents.IMainPanel uiComponents) : base(uiComponents) { }
            private const float FadeTime = 0.3f;
            private Vector2 _panelOriginalPos;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                UIComponents.MainPanel.gameObject.SetActive(false);
                
                _panelOriginalPos = UIComponents.MainPanel.anchoredPosition;
                
                _offscreenPos = AnimationHelper.GetOffscreenPos(UIComponents.MainPanel, AnimationHelper.Direction.Up);
                UIComponents.MainPanel.anchoredPosition = _offscreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(true))
                    .Append(UIComponents.MainPanel.DOAnchorPos(_panelOriginalPos, FadeTime));
            }
        }
        
        private class Animation_MainPanel_Close : SequenceUIAnimator<PauseUIAnimationComponents.IMainPanel>
        {
            public Animation_MainPanel_Close(PauseUIAnimationComponents.IMainPanel uiComponents) : base(uiComponents) { }
            private const float FadeTime = 0.3f;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                _offscreenPos = AnimationHelper.GetOffscreenPos(UIComponents.MainPanel, AnimationHelper.Direction.Up);
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