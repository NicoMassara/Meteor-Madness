using System;
using DG.Tweening;
using MeteorMadness.Animations;
using MeteorMadness.ScreenFlow.Base;
using MeteorMadness.ScreenFlow.MultiPage;
using UnityEngine;

namespace _Main.Scripts.MultiPage
{
    public class MultiPageViewAnimation : BaseViewAnimation<MultiPageUiAnimationSelector,MultiPageUiAnimationComponents>
    {

        [SerializeField] private MultiPageUiAnimationData animationData;
        
        #region Animators
        private class Animation_MainPanel_Open : SequenceUIAnimation<MultiPageUiAnimationComponents.IMainPanel, IPanelData>
        {
            private readonly AnimationHelper.PanelPosition _panel;

            public Animation_MainPanel_Open(MultiPageUiAnimationComponents.IMainPanel components, IPanelData animationData)
                : base(components, animationData)
            {
                
                _panel = new AnimationHelper.PanelPosition(UIComponents.MainPanel, 
                    AnimationData.OffscreenPosition, AnimationData.Offset);
            }

            protected override void Initialize()
            {
                UIComponents.MainPanel.gameObject.SetActive(false);
                UIComponents.MainPanel.anchoredPosition = _panel.OffScreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(true))
                    .Append(UIComponents.MainPanel.DOAnchorPos(_panel.StartPos, AnimationData.MovementDuration));
            }
        }

        private class Animation_MainPanel_Close : SequenceUIAnimation<MultiPageUiAnimationComponents.IMainPanel, IPanelData>
        {
            private readonly AnimationHelper.PanelPosition _panel;

            public Animation_MainPanel_Close(MultiPageUiAnimationComponents.IMainPanel components, IPanelData animationData)
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
        
        private IUiAnimation _animationOpen;
        private IUiAnimation _animationClose;
        
        private void Start()
        {
            _animationOpen = new Animation_MainPanel_Open(UIComponents, animationData.PanelOpenData);
            _animationClose = new Animation_MainPanel_Close(UIComponents, animationData.PanelCloseData);
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