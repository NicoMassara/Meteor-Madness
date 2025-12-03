using System;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Observer;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.MVC
{
    public class CosmeticViewAnimation : BaseViewAnimation<CosmeticUiAnimationSelector,CosmeticUiAnimationComponents>, IObserver
    {
        #region Animators
        private class MainPanelAnimator : SequenceUIAnimator<CosmeticUiAnimationComponents.IMainPanel>
        {
            public MainPanelAnimator(CosmeticUiAnimationComponents.IMainPanel components) : base(components) { }
            private const float FadeTime = 0.3f;
            private Vector2 _panelOriginalPos;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                Components.MainPanel.gameObject.SetActive(false);
                
                _panelOriginalPos = Components.MainPanel.anchoredPosition;
                
                _offscreenPos = AnimationHelper.GetOffscreenPos(Components.MainPanel, AnimationHelper.Direction.Left);
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
                case CosmeticObserverMessage.Enable:
                    HandleEnable();
                    break;
                case CosmeticObserverMessage.StartDisable:
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