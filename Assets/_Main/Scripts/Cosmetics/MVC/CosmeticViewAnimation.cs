using System;
using _Main.Scripts.Managers;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Observer;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.MVC
{
    public class CosmeticViewAnimation : BaseViewAnimation<CosmeticUiAnimationSelector,CosmeticUiAnimationComponents>,
        CosmeticViewAnimation.ICosmeticViewAnimation
    {
        
        public interface ICosmeticViewAnimation : IBaseViewAnimation
        {
            
        }
        
        [SerializeField] private CosmeticUiAnimationData animData;
        
        #region Animators
        private class Animation_MainPanel_Open : SequenceUIAnimator<CosmeticUiAnimationComponents.IMainPanel,IPanelData>
        {
            private AnimationHelper.PanelPosition _panel;
            
            public Animation_MainPanel_Open(CosmeticUiAnimationComponents.IMainPanel components,
                IPanelData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.MainPanel, AnimationData.OffScreenPos);
                UIComponents.MainPanel.anchoredPosition = _panel.OffScreenPos;
            }
            
            
            protected override void Initialize()
            {
                UIComponents.MainPanel.gameObject.SetActive(false);
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(true))
                    .Append(UIComponents.MainPanel.DOAnchorPos(_panel.StartPos, AnimationData.MovementDuration));
            }
        }
        
        private class Animation_MainPanel_Close : SequenceUIAnimator<CosmeticUiAnimationComponents.IMainPanel,IPanelData>
        {
            public Animation_MainPanel_Close(CosmeticUiAnimationComponents.IMainPanel components,
                IPanelData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.MainPanel, AnimationData.OffScreenPos);
            }

            private AnimationHelper.PanelPosition _panel;
            
            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.MainPanel.DOAnchorPos(_panel.OffScreenPos, AnimationData.MovementDuration))
                    .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(false));
            }
            
        }

        #endregion

        private IUIAnimator _animationOpen;
        private IUIAnimator _animationClose;
        
        private void Start()
        {
            _animationOpen = new Animation_MainPanel_Open(UIComponents, animData.PanelOpenData);
            _animationClose = new Animation_MainPanel_Close(UIComponents, animData.PanelCloseData);
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case CosmeticObserverMessage.Enable:
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