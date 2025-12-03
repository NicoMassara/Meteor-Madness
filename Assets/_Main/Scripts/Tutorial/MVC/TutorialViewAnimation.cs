using _Main.Scripts.MyAnimations;
using _Main.Scripts.Observer;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.Tutorial.MVC
{
    public class TutorialViewAnimation : BaseViewAnimation<TutorialUiAnimationSelector,TutorialUiAnimationComponents >
    {
        #region Animators
        private class MainPanelAnimator : SequenceUIAnimator<TutorialUiAnimationComponents.IHintPanel>
        {
            public MainPanelAnimator(TutorialUiAnimationComponents.IHintPanel components) : base(components) { }
            private const float FadeTime = 0.15f;
            private Vector2 _panelOriginalPos;
            private Vector2 _fadeInOffPos;
            private Vector2 _fadeOutOffPos;
            
            protected override void Initialize()
            {
                Components.HintPanel.gameObject.SetActive(false);
                
                _panelOriginalPos = Components.HintPanel.anchoredPosition;
                
                _fadeInOffPos = AnimationHelper.GetOffscreenPos(Components.HintPanel, AnimationHelper.Direction.Right);
                _fadeOutOffPos = AnimationHelper.GetOffscreenPos(Components.HintPanel, AnimationHelper.Direction.Left);
            }

            protected override Sequence CreateFadeIn()
            {
                Components.HintPanel.anchoredPosition = _fadeInOffPos;
                
                return DOTween.Sequence()
                    .AppendCallback(() => Components.HintPanel.gameObject.SetActive(true))
                    .Append(Components.HintPanel.DOAnchorPos(_panelOriginalPos, FadeTime));
            }

            protected override Sequence CreateFadeOut()
            {
                return DOTween.Sequence()
                    .Append(Components.HintPanel.DOAnchorPos(_fadeOutOffPos, FadeTime))
                    .AppendCallback(() => Components.HintPanel.gameObject.SetActive(false));
            }
        }

        #endregion
        
        private IAnimator _hintPanelAnimator;
        
        private void Start()
        {
            _hintPanelAnimator = new MainPanelAnimator(UIComponents);
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
            SetAnimator(_hintPanelAnimator);
        }
        
        private void HandleDisableHint()
        {
            ClearAnimator();
        }
    }
}