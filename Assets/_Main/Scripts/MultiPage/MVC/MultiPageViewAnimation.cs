using _Main.Scripts.MyAnimations;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.MultiPage
{
    public class MultiPageViewAnimation : BaseViewAnimation<MultiPageUiAnimationSelector,MultiPageUiAnimationComponents>
    {
        #region Animators
        private class MainPanelAnimator : SequenceUIAnimator<MultiPageUiAnimationComponents.IMainPanel>
        {
            public MainPanelAnimator(MultiPageUiAnimationComponents.IMainPanel components) : base(components) { }
            private const float FadeTime = 0.5f;
            private Vector2 _panelOriginalPos;
            private Vector2 _fadeInOffPos;
            private Vector2 _fadeOutOffPos;
            
            protected override void Initialize()
            {
                Components.MainPanel.gameObject.SetActive(false);
                
                _panelOriginalPos = Components.MainPanel.anchoredPosition;
                
                _fadeInOffPos = AnimationHelper.GetOffscreenPos(Components.MainPanel, AnimationHelper.Direction.Down);
                _fadeOutOffPos = AnimationHelper.GetOffscreenPos(Components.MainPanel, AnimationHelper.Direction.Down);
            }

            protected override Sequence CreateFadeIn()
            {
                Components.MainPanel.anchoredPosition = _fadeInOffPos;
                
                return DOTween.Sequence()
                    .AppendCallback(() => Components.MainPanel.gameObject.SetActive(true))
                    .Append(Components.MainPanel.DOAnchorPos(_panelOriginalPos, FadeTime));
            }

            protected override Sequence CreateFadeOut()
            {
                return DOTween.Sequence()
                    .Append(Components.MainPanel.DOAnchorPos(_fadeOutOffPos, FadeTime))
                    .AppendCallback(() => Components.MainPanel.gameObject.SetActive(false));
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
            
        }


        public void EnablePanel()
        {
            SetAnimator(_hintPanelAnimator);
        }

        public void DisablePanel()
        {
            ClearAnimator();
        }
    }
}