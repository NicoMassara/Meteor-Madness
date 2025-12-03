using _Main.Scripts.MyAnimations;
using _Main.Scripts.Observer;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.MySettings.MVC
{
    public class SettingsViewAnimation : BaseViewAnimation<SettingsUiAnimationSelector,SettingsUiAnimationComponents>
    {
        #region Animators
        private class MainPanelAnimator : SequenceUIAnimator<SettingsUiAnimationComponents.IMainPanel>
        {
            public MainPanelAnimator(SettingsUiAnimationComponents.IMainPanel components) : base(components) { }
            private const float FadeTime = 0.3f;
            private Vector2 _panelOriginalPos;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                Components.MainPanel.gameObject.SetActive(false);
                
                _panelOriginalPos = Components.MainPanel.anchoredPosition;
                
                _offscreenPos = AnimationHelper.GetOffscreenPos(Components.MainPanel, AnimationHelper.Direction.Right);
                Components.MainPanel.anchoredPosition = _offscreenPos;
            }

            protected override Sequence CreateFadeIn()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => Components.MainPanel.gameObject.SetActive(true))
                    .Append(Components.MainPanel.DOAnchorPos(_panelOriginalPos, FadeTime));
            }

            protected override Sequence CreateFadeOut()
            {
                return DOTween.Sequence()
                    .Append(Components.MainPanel.DOAnchorPos(_offscreenPos, FadeTime/2))
                    .AppendCallback(() => Components.MainPanel.gameObject.SetActive(false));
            }
        }

        #endregion

        private IAnimator _mainPanelAnimator;
        
        private void Start()
        {
            _mainPanelAnimator = new MainPanelAnimator(UIComponents);
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case SettingsObserverMessage.Enable:
                    HandleEnable();
                    break;
                case SettingsObserverMessage.StartDisable:
                    HandleDisable();
                    break;
            }
        }

        private void HandleEnable()
        {
            SetAnimator(_mainPanelAnimator);
        }
        
        private void HandleDisable()
        {
            ClearAnimator();
        }
    }
}