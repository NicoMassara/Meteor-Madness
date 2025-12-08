using System;
using _Main.Scripts.MyAnimations;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.MultiPage
{
    public class MultiPageViewAnimation : BaseViewAnimation<MultiPageUiAnimationSelector,MultiPageUiAnimationComponents>
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
        private class Animation_MainPanel_Open : SequenceUIAnimator<MultiPageUiAnimationComponents.IMainPanel,PanelOpenData>
        {
            public Animation_MainPanel_Open(MultiPageUiAnimationComponents.IMainPanel components, PanelOpenData animationData)
                : base(components, animationData)
            {
            }
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
        
        private class Animation_MainPanel_Close : SequenceUIAnimator<MultiPageUiAnimationComponents.IMainPanel,PanelCloseData>
        {
            public Animation_MainPanel_Close(MultiPageUiAnimationComponents.IMainPanel components, PanelCloseData animationData)
                : base(components, animationData)
            {
            }

            private const float FadeTime = 0.3f;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                _offscreenPos = AnimationHelper.GetOffscreenPos(UIComponents.MainPanel, AnimationHelper.Direction.Down);
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
            _animationOpen = new Animation_MainPanel_Open(UIComponents, panelOpenData);
            _animationClose = new Animation_MainPanel_Close(UIComponents, panelCloseData);
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            
        }


        public void EnablePanel()
        {
            PlayAnimation(_animationOpen, TriggerOnPanelOpened);
        }

        public void DisablePanel()
        {
            PlayAnimation(_animationClose, TriggerOnPanelClosed);
        }
    }
}