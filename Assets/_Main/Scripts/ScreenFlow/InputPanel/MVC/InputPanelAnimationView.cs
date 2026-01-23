using DG.Tweening;
using MeteorMadness.Animations;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.ScreenFlow._Main.Scripts.ScreenFlow.InputPanel.So;
using MeteorMadness.ScreenFlow._Main.Scripts.ScreenFlow.InputPanel.UISelector;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;

namespace MeteorMadness.ScreenFlow._Main.Scripts.ScreenFlow.InputPanel
{
    public class InputPanelAnimationView : BaseViewAnimation<InputPanelUIAnimationSelector,InputPanelUIAnimationComponents>,
        InputPanelAnimationView.IInputPanelAnimationView
    {
        public interface IInputPanelAnimationView : IBaseViewAnimation
        {
            
        }
        
        #region Animators
        private class Animation_MainPanel_Open : SequenceUIAnimation<InputPanelUIAnimationComponents.IMainPanel, IPanelData>
        {
            private readonly AnimationHelper.PanelPosition _panel;

            public Animation_MainPanel_Open(InputPanelUIAnimationComponents.IMainPanel components, IPanelData animationData)
                : base(components, animationData)
            {
                
                _panel = new AnimationHelper.PanelPosition(UIComponents.MainPanel, 
                    AnimationData.OffscreenPosition, AnimationData.Offset);
                
                UIComponents.MainPanel.gameObject.SetActive(false);
                UIComponents.MainPanel.anchoredPosition = _panel.OffScreenPos;
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

        private class Animation_MainPanel_Close : SequenceUIAnimation<InputPanelUIAnimationComponents.IMainPanel, IPanelData>
        {
            private readonly AnimationHelper.PanelPosition _panel;

            public Animation_MainPanel_Close(InputPanelUIAnimationComponents.IMainPanel components, IPanelData animationData)
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
        
        [SerializeField] private InputPanelAnimationData animationData;
        
        private IUiAnimation _animationOpen;
        private IUiAnimation _animationClose;

        private void Start()
        {
            _animationOpen = new Animation_MainPanel_Open(UIComponents, animationData.PanelOpenData);
            _animationClose = new Animation_MainPanel_Close(UIComponents, animationData.PanelCloseData);
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case InputPanelObserverMessage.Enable:
                    HandleEnable();
                    break;
                
                case InputPanelObserverMessage.Disable:
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
            PlayAnimation(_animationClose, TriggerOnPanelClosed);
        }
    }
}