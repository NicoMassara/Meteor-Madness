using System;
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
        
        #region Animation Data

        [Serializable]
        private class PanelOpenData : UiAnimationData
        {
            public AnimationHelper.Direction offscreenPos = AnimationHelper.Direction.Up;
            public float movementDuration = 0.25f;
        }
        
        [SerializeField] private PanelOpenData panelOpenData;
        
        [Serializable]
        private class PanelCloseData : UiAnimationData
        {
            public AnimationHelper.Direction offscreenPos = AnimationHelper.Direction.Up;
            public float movementDuration = 0.25f;
        }
        
        [SerializeField] private PanelCloseData panelCloseData;

        #endregion
        
        #region Animators

        private class Animation_Initialize : UIComponentsInitializer<PauseUIAnimationComponents.IMainPanel>
        {
            public Animation_Initialize(PauseUIAnimationComponents.IMainPanel components) : base(components)
            {
            }

            protected override void Initialize()
            {
                UIComponents.MainPanel.gameObject.SetActive(false);
            }
        }

        private class Animation_MainPanel_Open : SequenceUIAnimator<PauseUIAnimationComponents.IMainPanel,PanelOpenData>
        {
            private readonly AnimationHelper.PanelPosition _panel;
            
            public Animation_MainPanel_Open(PauseUIAnimationComponents.IMainPanel components, PanelOpenData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.MainPanel,AnimationData.offscreenPos);
            }

            
            protected override void Initialize()
            {
                UIComponents.MainPanel.anchoredPosition = _panel.OffScreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(true))
                    .Append(UIComponents.MainPanel.DOAnchorPos(_panel.StartPos, AnimationData.movementDuration));
            }
        }
        private class Animation_MainPanel_Close : SequenceUIAnimator<PauseUIAnimationComponents.IMainPanel,PanelCloseData>
        {
            private readonly AnimationHelper.PanelPosition _panel;
            
            public Animation_MainPanel_Close(PauseUIAnimationComponents.IMainPanel components, PanelCloseData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.MainPanel,AnimationData.offscreenPos);
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.MainPanel.DOAnchorPos(_panel.OffScreenPos, AnimationData.movementDuration))
                    .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(false));
            }
            
        }

        #endregion

        private IUIAnimator _animationPanelOpen;
        private IUIAnimator _animationPanelClose;
        
        private void Start()
        {
            _animationPanelOpen = new Animation_MainPanel_Open(UIComponents, panelOpenData);
            _animationPanelClose = new Animation_MainPanel_Close(UIComponents, panelCloseData);
            
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