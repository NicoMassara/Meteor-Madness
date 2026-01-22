using _Main.Scripts.GameStats;
using DG.Tweening;
using MeteorMadness.Animations;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Stats
{
    public class StatsViewAnimation : BaseViewAnimation<StatsUiAnimationSelector,StatsUiAnimationComponents>, IObserver, 
        StatsViewAnimation.IStatsViewAnimation
    {
        public interface IStatsViewAnimation : IBaseViewAnimation
        {
            
        }
        
        [SerializeField] private StatsUiAnimationData animData;

        #region Animations

        private class Animation_HintPanel_Open : SequenceUIAnimation<StatsUiAnimationComponents.IMainPanel, IPanelData>
        {
            private readonly AnimationHelper.PanelPosition _panel;
            
            public Animation_HintPanel_Open(StatsUiAnimationComponents.IMainPanel components, IPanelData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.MainPanel,
                    AnimationData.OffscreenPosition, AnimationData.Offset);
                UIComponents.MainPanel.gameObject.SetActive(false);
            }
            

            protected override void Initialize()
            {
                UIComponents.MainPanel.anchoredPosition = _panel.OffScreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(true))
                    .Append(UIComponents.MainPanel.DOAnchorPos(_panel.StartPos, AnimationData.MovementDuration));
            }
        }

        private class Animation_HintPanel_Close : SequenceUIAnimation<StatsUiAnimationComponents.IMainPanel, IPanelData>
        {
            private readonly AnimationHelper.PanelPosition _panel;
            
            public Animation_HintPanel_Close(StatsUiAnimationComponents.IMainPanel components, IPanelData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.MainPanel,
                    AnimationData.OffscreenPosition, AnimationData.Offset);
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
        
        private void Start()
        {
            _animationPanelOpen = new Animation_HintPanel_Open(UIComponents, animData.PanelOpenData);
            _animationPanelClose = new Animation_HintPanel_Close(UIComponents, animData.PanelCloseData);
        }
        
        
        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case StatsObserverMessage.Enable:
                    HandleEnable();
                    break;
                case StatsObserverMessage.StartDisable:
                    HandleStartDisable();
                    break;
            }
        }
        
        private void HandleEnable() => PlayAnimation(_animationPanelOpen,TriggerOnPanelOpened);
        private void HandleStartDisable() => PlayAnimation(_animationPanelClose, TriggerOnPanelClosed);
    }
}