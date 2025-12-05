using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Menu;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Observer;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.MainMenu.MVC
{
    public class MainMenuViewAnimation : BaseViewAnimation<MainMenuUiAnimationSelector,MainMenuUiAnimationComponents>,
    MainMenuViewAnimation.IAnimator
    {
        private interface IAnimator
        {
            public void PlayOpenAnimation(ScreenType screenType);
            public void PlayCloseAnimation(ScreenType screenType);
            public void TriggerClose();
        }
        
        public enum ScreenType
        {
            MainMenu,
            Lore,
            Tutorial,
            Credits
        }
        
        #region Animator

        private class MenuAnimator
        {
            #region States

            private enum States
            {
                None,
                MainMenu,
                Lore,
                Tutorial,
                Credits,
                Closed
            }
            
            private class BaseState<T> : State<T>
            {
                protected IAnimator Controller { get; private set; }

                public void Initialize(IAnimator controller)
                {
                    Controller = controller;
                }
            }
            
            private class CloseState<T> : BaseState<T>
            {
                public override void Awake()
                {
                    Controller.TriggerClose();
                }
            }

            private class DefaultState<T> : BaseState<T>
            {
                private readonly ScreenType _screenType;

                public DefaultState(ScreenType screenType)
                {
                    _screenType = screenType;
                }

                public override void Awake()
                {
                    Controller.PlayOpenAnimation(_screenType);
                }

                public override void Sleep()
                {
                    Controller.PlayCloseAnimation(_screenType);
                }
            }

            #endregion
            
            private FSM<States> _fsm;

            public MenuAnimator(IAnimator animator)
            {
                InitializeFsm(animator);
            }

            #region FSM

            private void InitializeFsm(IAnimator animator)
            {
                var temp = new List<BaseState<States>>();
                _fsm = new FSM<States>("MainMenu-Animator");
                
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                _fsm.CreateDebugGUI(5);
#endif

                #region Variables

                var none = new BaseState<States>();
                var mainMenu = new DefaultState<States>(ScreenType.MainMenu);
                var lore = new DefaultState<States>(ScreenType.Lore);
                var tutorial = new DefaultState<States>(ScreenType.Tutorial);
                var credits = new DefaultState<States>(ScreenType.Credits);
                var close = new CloseState<States>();
                
                temp.Add(none);
                temp.Add(mainMenu);
                temp.Add(lore);
                temp.Add(tutorial);
                temp.Add(credits);
                temp.Add(close);

                #endregion

                #region Transitions

                none.AddTransition(States.MainMenu, mainMenu);
                //
                mainMenu.AddTransition(States.Lore, lore);
                mainMenu.AddTransition(States.Tutorial, tutorial);
                mainMenu.AddTransition(States.Credits, credits);
                mainMenu.AddTransition(States.Closed, close);
                //
                tutorial.AddTransition(States.MainMenu, mainMenu);
                tutorial.AddTransition(States.Closed, close);
                //
                lore.AddTransition(States.MainMenu, mainMenu);
                //
                credits.AddTransition(States.MainMenu, mainMenu);
                //
                close.AddTransition(States.MainMenu, mainMenu);

                #endregion
                
                foreach (var state in temp)
                {
                    state.Initialize(animator);
                }
            
                _fsm.SetInit(none);
            }

            #region Tranisionts

            private void SetTransition(States state)
            {
                _fsm?.Transitions(state);
            }
            
            public void TransitionToClose()
            {
                SetTransition(States.Closed);
            }
            
            public void TransitionToMainMenu()
            {
                SetTransition(States.MainMenu);
            }
            
            public void TransitionToCredits()
            {
                SetTransition(States.Credits);
            }
        
            public void TransitionToLore()
            {
                SetTransition(States.Lore);
            }
            
            public void TransitionToTutorial()
            {
                SetTransition(States.Tutorial);
            }

            #endregion

            #endregion
        }


        #region Animations

        #region Menu

        private class Animation_Menu_Open : SequenceUIAnimator<MainMenuUiAnimationComponents.IMainMenuPanel>
        {
            private const float TitlePos = 500;
            private const float LeftButtonsPos = -1000;
            private const float RightButtonsPos = 500;
            //
            private const float FadeOutTime = 0.25f; 
            
            private bool _hasPlayedFirstAnimation = false;
            
            public Animation_Menu_Open(MainMenuUiAnimationComponents.IMainMenuPanel uiComponents) 
                : base(uiComponents) { }

            protected override void Initialize()
            {
                UIComponents.MenuPanel.gameObject.SetActive(false);
            }

            private Sequence CreateFirstAnimation()
            {
                _hasPlayedFirstAnimation = true;
                
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.MenuPanel.gameObject.SetActive(true))
                    .AppendCallback(() => UIComponents.QuitButton.gameObject.SetActive(false))
                    .AppendInterval(0.25f)
                    .Append(UIComponents.GameTitle.rectTransform.DOAnchorPosY(TitlePos, 0.5f).From())
                    .AppendInterval(0.5f)
                    .Append(UIComponents.LeftButtonsPanel.DOAnchorPosX(LeftButtonsPos, 0.25f).From())
                    .Join(UIComponents.RightButtonsPanel.DOAnchorPosX(RightButtonsPos, 0.25f).From())
                    .AppendInterval(0.5f)
                    .AppendCallback(() => UIComponents.QuitButton.gameObject.SetActive(true));
            }
            
            private Sequence CreateNormalAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.MenuPanel.gameObject.SetActive(true))
                    .Append(UIComponents.GameTitle.rectTransform.DOAnchorPosY(TitlePos, FadeOutTime).From())
                    .Join(UIComponents.LeftButtonsPanel.DOAnchorPosX(LeftButtonsPos, FadeOutTime).From())
                    .Join(UIComponents.RightButtonsPanel.DOAnchorPosX(RightButtonsPos, FadeOutTime).From())
                    .AppendCallback(() => UIComponents.QuitButton.gameObject.SetActive(true))
                    .AppendInterval(0.1f);
            }
            
            protected override Sequence CreateAnimation()
            {
                return _hasPlayedFirstAnimation ? CreateNormalAnimation() : CreateFirstAnimation();
            }
            
        }
        private class Animation_Menu_Close : SequenceUIAnimator<MainMenuUiAnimationComponents.IMainMenuPanel>
        {
            private const float TitlePos = 500;
            private const float LeftButtonsPos = -1000;
            private const float RightButtonsPos = 500;
            //
            private const float FadeOutTime = 0.25f; 
            
            private bool _hasPlayedFirstAnimation = false;
            
            public Animation_Menu_Close(MainMenuUiAnimationComponents.IMainMenuPanel uiComponents) 
                : base(uiComponents) { }

            protected override void Initialize()
            {
                UIComponents.MenuPanel.gameObject.SetActive(false);
            }

            protected override void RestartValues()
            {
                DOTween.Sequence()
                    .Join(UIComponents.GameTitle.rectTransform.DOAnchorPosY(0, 0))
                    .Join(UIComponents.LeftButtonsPanel.DOAnchorPosX(0,0))
                    .Join(UIComponents.RightButtonsPanel.DOAnchorPosX(0, 0));
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.QuitButton.gameObject.SetActive(false))
                    .Join(UIComponents.GameTitle.rectTransform.DOAnchorPosY(TitlePos, FadeOutTime))
                    .Join(UIComponents.LeftButtonsPanel.DOAnchorPosX(LeftButtonsPos, FadeOutTime))
                    .Join(UIComponents.RightButtonsPanel.DOAnchorPosX(RightButtonsPos, FadeOutTime))
                    .AppendCallback(() => UIComponents.MenuPanel.gameObject.SetActive(false));
            }
        }

        #endregion

        #region Lore

        private class Animation_Lore_Open : SequenceUIAnimator<MainMenuUiAnimationComponents.ILorePanel>
        {
            public Animation_Lore_Open(MainMenuUiAnimationComponents.ILorePanel uiComponents) 
                : base(uiComponents) { }


            private const float FadeTime = 0.3f;
            private Vector2 _panelOriginalPos;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                UIComponents.LorePanel.gameObject.SetActive(false);
                
                _panelOriginalPos = UIComponents.LorePanel.anchoredPosition;
                
                _offscreenPos = AnimationHelper.GetOffscreenPos(UIComponents.LorePanel, AnimationHelper.Direction.Down);
                UIComponents.LorePanel.anchoredPosition = _offscreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.LorePanel.gameObject.SetActive(true))
                    .Append(UIComponents.LorePanel.DOAnchorPos(_panelOriginalPos, FadeTime));
            }
        }
        private class Animation_Lore_Close : SequenceUIAnimator<MainMenuUiAnimationComponents.ILorePanel>
        {
            public Animation_Lore_Close(MainMenuUiAnimationComponents.ILorePanel uiComponents) 
                : base(uiComponents) { }


            private const float FadeTime = 0.3f;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                UIComponents.LorePanel.gameObject.SetActive(false);
                
                _offscreenPos = AnimationHelper.GetOffscreenPos(UIComponents.LorePanel, AnimationHelper.Direction.Down);
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.LorePanel.DOAnchorPos(_offscreenPos, FadeTime/2))
                    .AppendCallback(() => UIComponents.LorePanel.gameObject.SetActive(false));
            }
        }

        #endregion

        #region Tutorial
        private class Animation_Tutorial_Open : SequenceUIAnimator<MainMenuUiAnimationComponents.ITutorialPanel>
        {
            public Animation_Tutorial_Open(MainMenuUiAnimationComponents.ITutorialPanel uiComponents) 
                : base(uiComponents) { }
            
            private const float FadeTime = 0.3f;
            private Vector2 _panelOriginalPos;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                UIComponents.TutorialPanel.gameObject.SetActive(false);
                
                _panelOriginalPos = UIComponents.TutorialPanel.anchoredPosition;
                
                _offscreenPos = AnimationHelper.GetOffscreenPos(UIComponents.TutorialPanel, AnimationHelper.Direction.Left);
                UIComponents.TutorialPanel.anchoredPosition = _offscreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.TutorialPanel.gameObject.SetActive(true))
                    .Append(UIComponents.TutorialPanel.DOAnchorPos(_panelOriginalPos, FadeTime));
            }
        }
        private class Animation_Tutorial_Close : SequenceUIAnimator<MainMenuUiAnimationComponents.ITutorialPanel>
        {
            public Animation_Tutorial_Close(MainMenuUiAnimationComponents.ITutorialPanel uiComponents) 
                : base(uiComponents) { }
            
            private const float FadeTime = 0.3f;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                UIComponents.TutorialPanel.gameObject.SetActive(false);
                
                _offscreenPos = AnimationHelper.GetOffscreenPos(UIComponents.TutorialPanel, AnimationHelper.Direction.Left);
            }
            
            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.TutorialPanel.DOAnchorPos(_offscreenPos, FadeTime/2))
                    .AppendCallback(() => UIComponents.TutorialPanel.gameObject.SetActive(false));
            }
        }

        #endregion

        #region Credits
        private class Animation_Credtis_Open : SequenceUIAnimator<MainMenuUiAnimationComponents.ICreditsPanel>
        {
            public Animation_Credtis_Open(MainMenuUiAnimationComponents.ICreditsPanel uiComponents) 
                : base(uiComponents) { }

            private const float FadeTime = 0.3f;
            private Vector2 _panelOriginalPos;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                UIComponents.CreditsPanel.gameObject.SetActive(false);
                
                _panelOriginalPos = UIComponents.CreditsPanel.anchoredPosition;
                
                _offscreenPos = AnimationHelper.GetOffscreenPos(UIComponents.CreditsPanel, AnimationHelper.Direction.Right);
                UIComponents.CreditsPanel.anchoredPosition = _offscreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.CreditsPanel.gameObject.SetActive(true))
                    .Append(UIComponents.CreditsPanel.DOAnchorPos(_panelOriginalPos, FadeTime));
            }
        }
        private class Animation_Credtis_Close : SequenceUIAnimator<MainMenuUiAnimationComponents.ICreditsPanel>
        {
            public Animation_Credtis_Close(MainMenuUiAnimationComponents.ICreditsPanel uiComponents) 
                : base(uiComponents) { }

            private const float FadeTime = 0.3f;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                UIComponents.CreditsPanel.gameObject.SetActive(false);
                
                _offscreenPos = AnimationHelper.GetOffscreenPos(UIComponents.CreditsPanel, AnimationHelper.Direction.Right);
            }
            

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.CreditsPanel.DOAnchorPos(_offscreenPos, FadeTime/2))
                    .AppendCallback(() => UIComponents.CreditsPanel.gameObject.SetActive(false));
            }
        }

        #endregion
        
        #endregion
        
        #endregion
        
        private MenuAnimator _animator;
        
        private IUIAnimator _animationMenuOpen;
        private IUIAnimator _animationMenuClose;
        //
        private IUIAnimator _animationLoreOpen;
        private IUIAnimator _animationLoreClose;
        //
        private IUIAnimator _animationTutorialOpen;
        private IUIAnimator _animationTutorialClose;
        //
        private IUIAnimator _animationCreditsOpen;
        private IUIAnimator _animationCreditsClose;

        private void Awake()
        {
            _animator = new MenuAnimator(this);
        }

        private void Start()
        {
            _animationMenuOpen = new Animation_Menu_Open(UIComponents);
            _animationMenuClose = new Animation_Menu_Close(UIComponents);
            //
            _animationLoreOpen = new Animation_Lore_Open(UIComponents);
            _animationLoreClose = new Animation_Lore_Close(UIComponents);
            //
            _animationTutorialOpen = new Animation_Tutorial_Open(UIComponents);
            _animationTutorialClose = new Animation_Tutorial_Close(UIComponents);
            //
            _animationCreditsOpen = new Animation_Credtis_Open(UIComponents);
            _animationCreditsClose = new Animation_Credtis_Close(UIComponents);
        }

        #region Observer
        
        public override void OnNotify(ulong message, params object[] args)
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
            _animator.TransitionToMainMenu();
        }
        private void HandleTutorialMenu()
        {
            _animator.TransitionToTutorial();
        }
        
        private void HandleCreditsMenu()
        {
            _animator.TransitionToCredits();
        }

        private void HandleLoreMenu()
        {
            _animator.TransitionToLore();
        }
        
        private void HandleDisable()
        {
            _animator.TransitionToClose();
        }
        
        #endregion
        
        #region IAnimator

        public void PlayOpenAnimation(ScreenType screenType)
        {
            switch (screenType)
            {
                case ScreenType.MainMenu:
                    PlayAnimation(_animationMenuOpen);
                    break;
                case ScreenType.Lore:
                    PlayAnimation(_animationLoreOpen);
                    break;
                case ScreenType.Tutorial:
                    PlayAnimation(_animationTutorialOpen);
                    break;
                case ScreenType.Credits:
                    PlayAnimation(_animationCreditsOpen);
                    break;
            }
        }

        public void PlayCloseAnimation(ScreenType screenType)
        {
            switch (screenType)
            {
                case ScreenType.MainMenu:
                    PlayAnimation(_animationMenuClose);
                    break;
                case ScreenType.Lore:
                    PlayAnimation(_animationLoreClose);
                    break;
                case ScreenType.Tutorial:
                    PlayAnimation(_animationTutorialClose);
                    break;
                case ScreenType.Credits:
                    PlayAnimation(_animationCreditsClose);
                    break;
            }
        }
        
        public void TriggerClose()
        {
            TriggerOnPanelClosed();
        }

        #endregion
    }
}