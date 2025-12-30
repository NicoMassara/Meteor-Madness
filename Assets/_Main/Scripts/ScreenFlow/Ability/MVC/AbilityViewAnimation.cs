using System;
using DG.Tweening;
using MeteorMadness.Animations;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.Managers;
using MeteorMadness.ScreenFlow.Ability;
using MeteorMadness.ScreenFlow.Ability.So;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;

namespace MeteorMadness.Gameplay.Abilities
{
    public class AbilityViewAnimation : BaseViewAnimation<AbilityUiAnimationSelector,AbilityUIAnimationComponents>,
        AbilityViewAnimation.IAbilityViewAnimation
    {
        public interface IAbilityViewAnimation : BaseViewAnimation<AbilityUiAnimationSelector,AbilityUIAnimationComponents>.IBaseViewAnimation
        {
            public event Action OnDataInitialized;
        }

        [SerializeField] private AbilityUiAnimationData animData;
        
        #region Animators

        private class Animation_Initialize : UIComponentsInitializer<AbilityUIAnimationComponents.IMainPanel>
        {
            public Animation_Initialize(AbilityUIAnimationComponents.IMainPanel components) : base(components) { }

            protected override void Initialize()
            {
                UIComponents.MainPanel.gameObject.SetActive(false);
            }
        }

        private class Animation_MainPanel_Open : SequenceUIAnimation<AbilityUIAnimationComponents.IMainPanel, IPanelData>
        {
            private readonly AnimationHelper.PanelPosition _panel;

            public Animation_MainPanel_Open(AbilityUIAnimationComponents.IMainPanel components, IPanelData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.MainPanel, animationData.OffscreenPosition);
            }

            protected override void Initialize()
            {
                UIComponents.MainPanel.anchoredPosition = _panel.OffScreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(true))
                    .Append(UIComponents.MainPanel.DOAnchorPos(_panel.StartPos, AnimationData.MovementDuration))
                    ;
            }
        }

        private class Animation_MainPanel_Close : SequenceUIAnimation<AbilityUIAnimationComponents.IMainPanel, IPanelData>
        {
            private readonly AnimationHelper.PanelPosition _panel;

            public Animation_MainPanel_Close(AbilityUIAnimationComponents.IMainPanel components, IPanelData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.MainPanel, animationData.OffscreenPosition);
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.MainPanel.DOAnchorPos(_panel.OffScreenPos, AnimationData.MovementDuration))
                    .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(false));
            }
        }

        #endregion
        
        private IUiAnimation _animationPanelOpen;
        private IUiAnimation _animationPanelClose;
        
        public event Action OnDataInitialized;

        private void Start()
        {
            _animationPanelOpen = new Animation_MainPanel_Open(UIComponents, animData.OpenData);
            _animationPanelClose = new Animation_MainPanel_Close(UIComponents, animData.CloseData);
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case AbilityUIObserverMessage.Initialize:
                    HandleInitialize();
                    break;
                case AbilityUIObserverMessage.Enable:
                    HandleEnableUI();
                    break;
                case AbilityUIObserverMessage.Disable:
                    HandleDisableUI();
                    break;
            }
        }

        private void HandleInitialize()
        {
            var temp = new Animation_Initialize(UIComponents);
            OnDataInitialized?.Invoke();
            AbilitiesEventCaller.UiInitialized();
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