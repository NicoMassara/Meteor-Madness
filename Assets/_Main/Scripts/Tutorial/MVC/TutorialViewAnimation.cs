using System;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Observer;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.Tutorial.MVC
{
    public class TutorialViewAnimation : BaseViewAnimation<TutorialUiAnimationSelector,TutorialUiAnimationComponents>
    {
        #region Animation Data

        [Serializable]
        private class PanelOpenData : UiAnimationData
        {
            
        }
        
        [SerializeField] private PanelOpenData panelOpenData;
        
        [Serializable]
        private class PanelCloseData : UiAnimationData
        {
            
        }
        
        [SerializeField] private PanelCloseData panelCloseData;

        #endregion
        
        #region Animators
        private class Animation_HintPanel_Open : SequenceUIAnimator<TutorialUiAnimationComponents.IHintPanel,PanelOpenData>
        {
            public Animation_HintPanel_Open(TutorialUiAnimationComponents.IHintPanel components, PanelOpenData animationData) 
                : base(components, animationData)
            {
            }
            
            private const float FadeTime = 0.3f;
            private Vector2 _panelOriginalPos;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                UIComponents.HintPanel.gameObject.SetActive(false);
                
                _panelOriginalPos = UIComponents.HintPanel.anchoredPosition;
                
                _offscreenPos = AnimationHelper.GetOffscreenPos(UIComponents.HintPanel, AnimationHelper.Direction.Right);
                UIComponents.HintPanel.anchoredPosition = _offscreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.HintPanel.gameObject.SetActive(true))
                    .Append(UIComponents.HintPanel.DOAnchorPos(_panelOriginalPos, FadeTime));
            }
        }
        private class Animation_HintPanel_Close : SequenceUIAnimator<TutorialUiAnimationComponents.IHintPanel,PanelCloseData>
        {
            public Animation_HintPanel_Close(TutorialUiAnimationComponents.IHintPanel components, PanelCloseData animationData)
                : base(components, animationData)
            {
            }


            private const float FadeTime = 0.3f;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                _offscreenPos = AnimationHelper.GetOffscreenPos(UIComponents.HintPanel, AnimationHelper.Direction.Left);
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.HintPanel.DOAnchorPos(_offscreenPos, FadeTime/2))
                    .AppendCallback(() => UIComponents.HintPanel.gameObject.SetActive(false));
            }
            
        }

        #endregion
        
        private IUIAnimator _animationPanelOpen;
        private IUIAnimator _animationPanelClose;
        
        private void Start()
        {
            _animationPanelOpen = new Animation_HintPanel_Open(UIComponents, panelOpenData);
            _animationPanelClose = new Animation_HintPanel_Close(UIComponents, panelCloseData);
        }
        
        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case TutorialObserverMessage.EnableHint:
                    HandleEnableHint();
                    break;
                case TutorialObserverMessage.DisableHint:
                    HandleDisableHint();
                    break;
            }
        }
        
        private void HandleEnableHint()
        {
            PlayAnimation(_animationPanelOpen,TriggerOnPanelOpened);
        }
        
        private void HandleDisableHint()
        {
            PlayAnimation(_animationPanelClose, TriggerOnPanelClosed);
        }
    }
}