using System;
using DG.Tweening;
using MeteorMadness.Animations;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Tutorial
{
    public class TutorialViewAnimation : BaseViewAnimation<TutorialUiAnimationSelector,TutorialUiAnimationComponents>
    {

        [SerializeField] private TutorialUiAnimationData animData;
        
        private class Animation_HintPanel_Open : SequenceUIAnimation<TutorialUiAnimationComponents.IHintPanel, IPanelData>
        {
            private readonly AnimationHelper.PanelPosition _panel;
            
            public Animation_HintPanel_Open(TutorialUiAnimationComponents.IHintPanel components, IPanelData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.HintPanel,
                    AnimationData.OffscreenPosition, AnimationData.Offset);
            }
            

            protected override void Initialize()
            {
                UIComponents.HintPanel.gameObject.SetActive(false);
                UIComponents.HintPanel.anchoredPosition = _panel.OffScreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.HintPanel.gameObject.SetActive(true))
                    .Append(UIComponents.HintPanel.DOAnchorPos(_panel.StartPos, AnimationData.MovementDuration));
            }
        }

        private class Animation_HintPanel_Close : SequenceUIAnimation<TutorialUiAnimationComponents.IHintPanel, IPanelData>
        {
            private readonly AnimationHelper.PanelPosition _panel;
            
            public Animation_HintPanel_Close(TutorialUiAnimationComponents.IHintPanel components, IPanelData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.HintPanel,
                    AnimationData.OffscreenPosition, AnimationData.Offset);
            }
            
            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.HintPanel.DOAnchorPos(_panel.OffScreenPos, AnimationData.MovementDuration))
                    .AppendCallback(() => UIComponents.HintPanel.gameObject.SetActive(false));
            }
        }

        
        private IUiAnimation _animationPanelOpen;
        private IUiAnimation _animationPanelClose;
        
        private void Start()
        {
            _animationPanelOpen = new Animation_HintPanel_Open(UIComponents, animData.PanelOpenData);
            _animationPanelClose = new Animation_HintPanel_Close(UIComponents, animData.PanelCloseData);
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