using System;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Observer;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.MVC
{
    public class CosmeticViewAnimation : BaseViewAnimation<CosmeticUiAnimationSelector,CosmeticUiAnimationComponents>
    {
        
        #region Animators
        private class Animation_MainPanel_Open : SequenceUIAnimator<CosmeticUiAnimationComponents.IMainPanel>
        {
            public Animation_MainPanel_Open(CosmeticUiAnimationComponents.IMainPanel uiComponents) : base(uiComponents) { }
            private const float FadeTime = 0.3f;
            private Vector2 _panelOriginalPos;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                UIComponents.MainPanel.gameObject.SetActive(false);
                
                _panelOriginalPos = UIComponents.MainPanel.anchoredPosition;
                
                _offscreenPos = AnimationHelper.GetOffscreenPos(UIComponents.MainPanel, AnimationHelper.Direction.Left);
                UIComponents.MainPanel.anchoredPosition = _offscreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(true))
                    .Append(UIComponents.MainPanel.DOAnchorPos(_panelOriginalPos, FadeTime));
            }
        }
        
        private class Animation_MainPanel_Close : SequenceUIAnimator<CosmeticUiAnimationComponents.IMainPanel>
        {
            public Animation_MainPanel_Close(CosmeticUiAnimationComponents.IMainPanel uiComponents) : base(uiComponents) { }
            private const float FadeTime = 0.3f;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                _offscreenPos = AnimationHelper.GetOffscreenPos(UIComponents.MainPanel, AnimationHelper.Direction.Left);
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.MainPanel.DOAnchorPos(_offscreenPos, FadeTime/2))
                    .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(false));
            }
            
        }

        #endregion

        private IUIAnimator _animationOpen;
        private IUIAnimator _animationClose;
        
        private void Start()
        {
            _animationOpen = new Animation_MainPanel_Open(UIComponents);
            _animationClose = new Animation_MainPanel_Close(UIComponents);
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case CosmeticObserverMessage.Initial:
                    HandleEnable();
                    break;
                case CosmeticObserverMessage.StartDisable:
                    HandleDisable();
                    break;
            }
        }

        private void HandleEnable()
        {
            PlayAnimation(_animationOpen, TriggerOnPanelOpened);
        }
        
        private void HandleDisable()
        {
            PlayAnimation(_animationClose,TriggerOnPanelClosed);
        }
    }
}