using System;
using _Main.Scripts.Menu;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Observer;
using DG.Tweening;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.MainMenu.MVC
{
    public class MainMenuViewAnimation : ManagedBehavior, IObserver
    {
        #region Animators
        private class MenuAnimator : SequenceUIAnimator<MainMenuUiAnimationComponents.IMainMenuPanel>
        {
            private const float TitlePos = 500;
            private const float LeftButtonsPos = -1000;
            private const float RightButtonsPos = 500;
            //
            private const float FadeOutTime = 0.25f; 
            
            private bool _hasPlayedFirstAnimation = false;
            
            public MenuAnimator(MainMenuUiAnimationComponents.IMainMenuPanel components) 
                : base(components) { }

            protected override void Initialize()
            {
                Components.MenuPanel.gameObject.SetActive(false);
            }

            protected override void RestartValues()
            {
                DOTween.Sequence()
                    .Join(Components.GameTitle.rectTransform.DOAnchorPosY(0, 0))
                    .Join(Components.LeftButtonsPanel.DOAnchorPosX(0,0))
                    .Join(Components.RightButtonsPanel.DOAnchorPosX(0, 0));
            }

            private Sequence CreateFirstAnimation()
            {
                _hasPlayedFirstAnimation = true;
                
                return DOTween.Sequence()
                    .AppendCallback(() => Components.MenuPanel.gameObject.SetActive(true))
                    .AppendCallback(() => Components.QuitButton.gameObject.SetActive(false))
                    .AppendInterval(0.25f)
                    .Append(Components.GameTitle.rectTransform.DOAnchorPosY(TitlePos, 0.5f).From())
                    .AppendInterval(0.5f)
                    .Append(Components.LeftButtonsPanel.DOAnchorPosX(LeftButtonsPos, 0.25f).From())
                    .Join(Components.RightButtonsPanel.DOAnchorPosX(RightButtonsPos, 0.25f).From())
                    .AppendInterval(0.5f)
                    .AppendCallback(() => Components.QuitButton.gameObject.SetActive(true));
            }
            
            private Sequence CreateNormalAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => Components.MenuPanel.gameObject.SetActive(true))
                    .Append(Components.GameTitle.rectTransform.DOAnchorPosY(TitlePos, FadeOutTime).From())
                    .Join(Components.LeftButtonsPanel.DOAnchorPosX(LeftButtonsPos, FadeOutTime).From())
                    .Join(Components.RightButtonsPanel.DOAnchorPosX(RightButtonsPos, FadeOutTime).From())
                    .AppendCallback(() => Components.QuitButton.gameObject.SetActive(true))
                    .AppendInterval(0.1f);
            }
            
            protected override Sequence CreateFadeIn()
            {
                return _hasPlayedFirstAnimation ? CreateNormalAnimation() : CreateFirstAnimation();
            }

            protected override Sequence CreateFadeOut()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => Components.QuitButton.gameObject.SetActive(false))
                    .Join(Components.GameTitle.rectTransform.DOAnchorPosY(TitlePos, FadeOutTime))
                    .Join(Components.LeftButtonsPanel.DOAnchorPosX(LeftButtonsPos, FadeOutTime))
                    .Join(Components.RightButtonsPanel.DOAnchorPosX(RightButtonsPos, FadeOutTime))
                    .AppendCallback(() => Components.MenuPanel.gameObject.SetActive(false));
            }
        }
        private class LoreAnimator : SequenceUIAnimator<MainMenuUiAnimationComponents.ILorePanel>
        {
            public LoreAnimator(MainMenuUiAnimationComponents.ILorePanel components) 
                : base(components) { }


            private const float FadeTime = 0.3f;
            private Vector2 _panelOriginalPos;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                Components.LorePanel.gameObject.SetActive(false);
                
                _panelOriginalPos = Components.LorePanel.anchoredPosition;
                
                _offscreenPos = AnimationHelper.GetOffscreenPos(Components.LorePanel, AnimationHelper.Direction.Down);
                Components.LorePanel.anchoredPosition = _offscreenPos;
            }

            protected override void RestartValues()
            {
                
            }

            protected override Sequence CreateFadeIn()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => Components.LorePanel.gameObject.SetActive(true))
                    .Append(Components.LorePanel.DOAnchorPos(_panelOriginalPos, FadeTime));
            }

            protected override Sequence CreateFadeOut()
            {
                return DOTween.Sequence()
                        .Append(Components.LorePanel.DOAnchorPos(_offscreenPos, FadeTime/2))
                    .AppendCallback(() => Components.LorePanel.gameObject.SetActive(false));
            }
        }
        private class TutorialAnimator : SequenceUIAnimator<MainMenuUiAnimationComponents.ITutorialPanel>
        {
            public TutorialAnimator(MainMenuUiAnimationComponents.ITutorialPanel components) 
                : base(components) { }
            
            private const float FadeTime = 0.3f;
            private Vector2 _panelOriginalPos;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                Components.TutorialPanel.gameObject.SetActive(false);
                
                _panelOriginalPos = Components.TutorialPanel.anchoredPosition;
                
                _offscreenPos = AnimationHelper.GetOffscreenPos(Components.TutorialPanel, AnimationHelper.Direction.Left);
                Components.TutorialPanel.anchoredPosition = _offscreenPos;
            }

            protected override Sequence CreateFadeIn()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => Components.TutorialPanel.gameObject.SetActive(true))
                    .Append(Components.TutorialPanel.DOAnchorPos(_panelOriginalPos, FadeTime));
            }

            protected override Sequence CreateFadeOut()
            {
                return DOTween.Sequence()
                    .Append(Components.TutorialPanel.DOAnchorPos(_offscreenPos, FadeTime/2))
                    .AppendCallback(() => Components.TutorialPanel.gameObject.SetActive(false));
            }
        }
        private class CreditsAnimator : SequenceUIAnimator<MainMenuUiAnimationComponents.ICreditsPanel>
        {
            public CreditsAnimator(MainMenuUiAnimationComponents.ICreditsPanel components) 
                : base(components) { }

            private const float FadeTime = 0.3f;
            private Vector2 _panelOriginalPos;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                Components.CreditsPanel.gameObject.SetActive(false);
                
                _panelOriginalPos = Components.CreditsPanel.anchoredPosition;
                
                _offscreenPos = AnimationHelper.GetOffscreenPos(Components.CreditsPanel, AnimationHelper.Direction.Right);
                Components.CreditsPanel.anchoredPosition = _offscreenPos;
            }

            protected override Sequence CreateFadeIn()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => Components.CreditsPanel.gameObject.SetActive(true))
                    .Append(Components.CreditsPanel.DOAnchorPos(_panelOriginalPos, FadeTime));
            }

            protected override Sequence CreateFadeOut()
            {
                return DOTween.Sequence()
                    .Append(Components.CreditsPanel.DOAnchorPos(_offscreenPos, FadeTime/2))
                    .AppendCallback(() => Components.CreditsPanel.gameObject.SetActive(false));
            }
        }

        #endregion
        
        [SerializeField] private MainMenuUiAnimationSelector uiPanelSelector;
        
        private IAnimator _menuAnimator;
        private IAnimator _loreAnimator;
        private IAnimator _tutorialAnimator;
        private IAnimator _creditsAnimator;

        private IAnimator _currentAnimator;
        
        public event Action OnPanelOpened;
        public event Action OnPanelClosed;

        private void Start()
        {

            var components = uiPanelSelector.GetPanelData();
            _menuAnimator = new MenuAnimator(components);
            _loreAnimator = new LoreAnimator(components);
            _tutorialAnimator = new TutorialAnimator(components);
            _creditsAnimator = new CreditsAnimator(components);
        }


        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case MainMenuObserverMessage.MainMenu:
                    HandleMainMenu();
                    break;
                case MainMenuObserverMessage.LoreMenu:
                    HandleLoreMenu();
                    break;
                case MainMenuObserverMessage.TutorialMenu:
                    HandleTutorialMenu();
                    break;
                case MainMenuObserverMessage.CreditsMenu:
                    HandleCreditsMenu();
                    break;
                case MainMenuObserverMessage.StartDisable:
                    HandleDisable();
                    break;
            }
        }
        
        
        private void HandleMainMenu()
        {
            SetCurrentAction(_menuAnimator);
        }
        private void HandleTutorialMenu()
        {
            SetCurrentAction(_tutorialAnimator);
        }
        
        private void HandleCreditsMenu()
        {
            SetCurrentAction(_creditsAnimator);
        }

        private void HandleLoreMenu()
        {
            SetCurrentAction(_loreAnimator);
        }
        
        private void HandleDisable()
        {
            ClearCurrentAction();
        }

        private void SetCurrentAction(IAnimator animator)
        {
            if (_currentAnimator != null)
            {
                _currentAnimator?.FadeOut(() =>
                {
                    OnPanelClosed?.Invoke();
                    
                    animator?.FadeIn(() =>
                    {
                        OnPanelOpened?.Invoke();
                    });
                });
            }
            else
            {
                animator?.FadeIn(() =>
                {
                    OnPanelOpened?.Invoke();
                });
            }

            _currentAnimator = animator;
        }

        private void ClearCurrentAction()
        {
            SetCurrentAction(null);
        }
    }
}