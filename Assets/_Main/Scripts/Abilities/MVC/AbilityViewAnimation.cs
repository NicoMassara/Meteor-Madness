using System;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Observer;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.Abilities
{
    public class AbilityViewAnimation : BaseViewAnimation<AbilityUiAnimationSelector,AbilityUIAnimationComponents>,
        AbilityViewAnimation.IAbilityViewAnimation
    {
        public interface IAbilityViewAnimation : BaseViewAnimation<AbilityUiAnimationSelector,AbilityUIAnimationComponents>.IBaseViewAnimation
        {
            public event Action OnDataInitialized;
        }

        #region Animation Data

        
        [Serializable]
        private class PanelOpenData : UiAnimationData
        {
            public AnimationHelper.Direction offscreenPosition = AnimationHelper.Direction.Down;
            public float movementDuration = 0.3f;
        }
        
        [SerializeField] private PanelOpenData panelOpenData;
        
        [Serializable]
        private class PanelCloseData : UiAnimationData
        {
            public AnimationHelper.Direction offscreenPosition = AnimationHelper.Direction.Down;
            public float movementDuration = 0.3f;
        }
        
        [SerializeField] private PanelCloseData panelCloseData;

        #endregion
        
        #region Animators

        private class Animation_Initialize : UIComponentsInitializer<AbilityUIAnimationComponents.IMainPanel>
        {
            public Animation_Initialize(AbilityUIAnimationComponents.IMainPanel components) : base(components)
            {
            }

            protected override void Initialize()
            {
                UIComponents.MainPanel.gameObject.SetActive(false);
            }
        }

        private class Animation_MainPanel_Open : SequenceUIAnimator<AbilityUIAnimationComponents.IMainPanel,PanelOpenData>
        {
            private readonly AnimationHelper.PanelPosition _panel;
            
            public Animation_MainPanel_Open(AbilityUIAnimationComponents.IMainPanel components,
                PanelOpenData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.MainPanel,AnimationData.offscreenPosition);
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
        
        private class Animation_MainPanel_Close : SequenceUIAnimator<AbilityUIAnimationComponents.IMainPanel, PanelCloseData>
        {
            private readonly AnimationHelper.PanelPosition _panel;

            public Animation_MainPanel_Close(AbilityUIAnimationComponents.IMainPanel components,
                PanelCloseData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.MainPanel,AnimationData.offscreenPosition);
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.MainPanel.DOAnchorPos(_panel.OffScreenPos, AnimationData.movementDuration))
                    .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(false))
                    ;
            }
            
        }

        #endregion

        
        private IUIAnimator _animationPanelOpen;
        private IUIAnimator _animationPanelClose;
        
        public event Action OnDataInitialized;

        private void Start()
        {
            _animationPanelOpen = new Animation_MainPanel_Open(UIComponents, panelOpenData);
            _animationPanelClose = new Animation_MainPanel_Close(UIComponents, panelCloseData);
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case AbilityObserverMessage.Initialize:
                    HandleInitialize();
                    break;
                case AbilityObserverMessage.EnableUI:
                    HandleEnableUI();
                    break;
                case AbilityObserverMessage.DisableUI:
                    HandleDisableUI();
                    break;
            }
        }

        private void HandleInitialize()
        {
            var temp = new Animation_Initialize(UIComponents);
            OnDataInitialized?.Invoke();
        }

        private void HandleEnableUI()
        {
            PlayAnimation(_animationPanelOpen, TriggerOnPanelOpened);
        }
        
        private void HandleDisableUI()
        {
            PlayAnimation(_animationPanelClose, TriggerOnPanelClosed);
        }
    }
}