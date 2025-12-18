using System;
using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Menu;
using _Main.Scripts.Menu.So;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Observer;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.MainMenu.MVC
{
    public class MainMenuViewAnimation : BaseViewAnimation<MainMenuUiAnimationSelector,MainMenuUiAnimationComponents>,
    MainMenuViewAnimation.IAnimator, MainMenuViewAnimation.IMainMenuViewAnimation
    {
        public interface IMainMenuViewAnimation : BaseViewAnimation<MainMenuUiAnimationSelector,MainMenuUiAnimationComponents>.IBaseViewAnimation
        {
            public event Action OnMainPanelOpened;
        }
        
        private interface IAnimator
        {
            public void PlayFirstOpenAnimation();
            public void PlayOpenAnimation(ScreenType screenType);
            public void PlayCloseAnimation(ScreenType screenType, Action onClose = null);
            public void TriggerClose();
        }
        
        public enum ScreenType
        {
            MainMenu,
            Lore,
            Tutorial,
            Credits,
            FirstGame,
        }

        #region IMainMenuViewAnimation

        public event Action OnMainPanelOpened;

        #endregion
        

        [SerializeField] private MenuUiAnimationData animData;
        
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
                FirstGame,
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

            private class MenuState<T> : BaseState<T>
            {
                private bool _isFirstOpen = true;
                public override bool IsManualSleep => true;
                
                public override void Awake()
                {
                    if (_isFirstOpen == false)
                    {
                        Controller.PlayOpenAnimation(ScreenType.MainMenu);
                    }
                    else
                    {
                        Controller.PlayFirstOpenAnimation();
                        _isFirstOpen = false;
                    }
                }

                public override void Sleep()
                {
                    Controller.PlayCloseAnimation(ScreenType.MainMenu, TriggerOnSleepFinished);
                }
            }

            private class DefaultState<T> : BaseState<T>
            {
                private readonly ScreenType _screenType;
                public override bool IsManualSleep => true;

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
                    Controller.PlayCloseAnimation(_screenType,TriggerOnSleepFinished);
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
                var mainMenu = new MenuState<States>();
                var lore = new DefaultState<States>(ScreenType.Lore);
                var tutorial = new DefaultState<States>(ScreenType.Tutorial);
                var credits = new DefaultState<States>(ScreenType.Credits);
                var first = new DefaultState<States>(ScreenType.FirstGame);
                var close = new CloseState<States>();
                
                temp.Add(none);
                temp.Add(mainMenu);
                temp.Add(lore);
                temp.Add(tutorial);
                temp.Add(credits);
                temp.Add(first);
                temp.Add(close);

                #endregion

                #region Transitions

                none.AddTransition(States.MainMenu, mainMenu);
                //
                mainMenu.AddTransition(States.Lore, lore);
                mainMenu.AddTransition(States.Tutorial, tutorial);
                mainMenu.AddTransition(States.Credits, credits);
                mainMenu.AddTransition(States.Closed, close);
                mainMenu.AddTransition(States.FirstGame, first);
                //
                first.AddTransition(States.MainMenu, mainMenu);
                first.AddTransition(States.Tutorial, tutorial);
                first.AddTransition(States.Closed, close);
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
            
            public void TransitionToFirstGame()
            {
                SetTransition(States.FirstGame);
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
        
        private class Animation_Menu_First_Open : SequenceUIAnimator<MainMenuUiAnimationComponents.IMainMenuPanel, IManuFirstOpenData>
        {
            private readonly AnimationHelper.PanelPosition _titlePanel;
            private readonly AnimationHelper.PanelPosition _rightButtonsPanel;
            private readonly AnimationHelper.PanelPosition _leftButtonsPanel;

            public Animation_Menu_First_Open(MainMenuUiAnimationComponents.IMainMenuPanel components,
                IManuFirstOpenData animationData)
                : base(components, animationData)
            {
                _titlePanel = new AnimationHelper.PanelPosition(UIComponents.GameTitle.rectTransform,
                    AnimationData.GameTitleOffscreenPos, AnimationData.TitleOffset);
                _rightButtonsPanel = new AnimationHelper.PanelPosition(UIComponents.RightButtonsPanel, 
                    AnimationData.RightButtonsOffscreenPos, AnimationData.RightButtonsOffset);
                _leftButtonsPanel = new AnimationHelper.PanelPosition(UIComponents.LeftButtonsPanel,
                    AnimationData.LeftButtonsOffscreenPos, AnimationData.LeftButtonsOffset);
            }

            protected override void Initialize()
            {
                UIComponents.MenuPanel.gameObject.SetActive(false);

                UIComponents.GameTitle.rectTransform.anchoredPosition = _titlePanel.OffScreenPos;
                UIComponents.RightButtonsPanel.anchoredPosition = _rightButtonsPanel.OffScreenPos;
                UIComponents.LeftButtonsPanel.anchoredPosition = _leftButtonsPanel.OffScreenPos;
            }
            

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.MenuPanel.gameObject.SetActive(true))
                    .AppendCallback(() => UIComponents.QuitButton.gameObject.SetActive(false))
                    .AppendInterval(AnimationData.ShowTitleDelay)
                    .Append(UIComponents.GameTitle.rectTransform.DOAnchorPos(_titlePanel.StartPos, AnimationData.TitleMovementDuration))
                    .AppendInterval(AnimationData.ShowButtonsDelay)
                    .Append(UIComponents.LeftButtonsPanel.DOAnchorPos(_leftButtonsPanel.StartPos, AnimationData.ButtonsMovementDuration))
                    .Join(UIComponents.RightButtonsPanel.DOAnchorPos(_rightButtonsPanel.StartPos, AnimationData.ButtonsMovementDuration))
                    .AppendInterval(AnimationData.FinishDelay)
                    .AppendCallback(() => UIComponents.QuitButton.gameObject.SetActive(true));
            }
        }

        private class Animation_Menu_Open : SequenceUIAnimator<MainMenuUiAnimationComponents.IMainMenuPanel, IMenuPanelData>
        {
            private readonly AnimationHelper.PanelPosition _titlePanel;
            private readonly AnimationHelper.PanelPosition _rightButtonsPanel;
            private readonly AnimationHelper.PanelPosition _leftButtonsPanel;

            public Animation_Menu_Open(MainMenuUiAnimationComponents.IMainMenuPanel components,
                IMenuPanelData animationData)
                : base(components, animationData)
            {
                _titlePanel = new AnimationHelper.PanelPosition(UIComponents.GameTitle.rectTransform,
                    AnimationData.GameTitleOffscreenPos, AnimationData.TitleOffset);
                _rightButtonsPanel = new AnimationHelper.PanelPosition(UIComponents.RightButtonsPanel, 
                    AnimationData.RightButtonsOffscreenPos, AnimationData.RightButtonsOffset);
                _leftButtonsPanel = new AnimationHelper.PanelPosition(UIComponents.LeftButtonsPanel,
                    AnimationData.LeftButtonsOffscreenPos, AnimationData.LeftButtonsOffset);
            }

            protected override void Initialize()
            {
                UIComponents.MenuPanel.gameObject.SetActive(false);

                UIComponents.GameTitle.rectTransform.anchoredPosition = _titlePanel.OffScreenPos;
                UIComponents.RightButtonsPanel.anchoredPosition = _rightButtonsPanel.OffScreenPos;
                UIComponents.LeftButtonsPanel.anchoredPosition = _leftButtonsPanel.OffScreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.MenuPanel.gameObject.SetActive(true))
                    .Append(UIComponents.GameTitle.rectTransform.DOAnchorPos(_titlePanel.StartPos, AnimationData.MovementDuration))
                    .Join(UIComponents.LeftButtonsPanel.DOAnchorPos(_leftButtonsPanel.StartPos, AnimationData.MovementDuration))
                    .Join(UIComponents.RightButtonsPanel.DOAnchorPos(_rightButtonsPanel.StartPos, AnimationData.MovementDuration))
                    .AppendCallback(() => UIComponents.QuitButton.gameObject.SetActive(true))
                    .AppendInterval(AnimationData.FinishDelay);
            }
        }

        private class Animation_Menu_Close : SequenceUIAnimator<MainMenuUiAnimationComponents.IMainMenuPanel, IMenuPanelData>
        {
            private readonly AnimationHelper.PanelPosition _titlePanel;
            private readonly AnimationHelper.PanelPosition _rightButtonsPanel;
            private readonly AnimationHelper.PanelPosition _leftButtonsPanel;

            public Animation_Menu_Close(MainMenuUiAnimationComponents.IMainMenuPanel components,
                IMenuPanelData animationData)
                : base(components, animationData)
            {
                _titlePanel = new AnimationHelper.PanelPosition(UIComponents.GameTitle.rectTransform,
                    AnimationData.GameTitleOffscreenPos, AnimationData.TitleOffset);
                _rightButtonsPanel = new AnimationHelper.PanelPosition(UIComponents.RightButtonsPanel, 
                    AnimationData.RightButtonsOffscreenPos, AnimationData.RightButtonsOffset);
                _leftButtonsPanel = new AnimationHelper.PanelPosition(UIComponents.LeftButtonsPanel,
                    AnimationData.LeftButtonsOffscreenPos, AnimationData.LeftButtonsOffset);
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.QuitButton.gameObject.SetActive(false))
                    .Join(UIComponents.GameTitle.rectTransform.DOAnchorPos(_titlePanel.OffScreenPos, AnimationData.MovementDuration))
                    .Join(UIComponents.LeftButtonsPanel.DOAnchorPos(_leftButtonsPanel.OffScreenPos, AnimationData.MovementDuration))
                    .Join(UIComponents.RightButtonsPanel.DOAnchorPos(_rightButtonsPanel.OffScreenPos, AnimationData.MovementDuration))
                    .AppendInterval(AnimationData.FinishDelay)
                    .AppendCallback(() => UIComponents.MenuPanel.gameObject.SetActive(false));
            }
        }

        #endregion

        #region Lore

        private class Animation_Lore_Open : SequenceUIAnimator<MainMenuUiAnimationComponents.ILorePanel, IBasePanelData>
        {
            private readonly AnimationHelper.PanelPosition _panel;

            public Animation_Lore_Open(MainMenuUiAnimationComponents.ILorePanel components, IBasePanelData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.LorePanel,
                    AnimationData.OffscreenDirection, AnimationData.Offset);
            }

            protected override void Initialize()
            {
                UIComponents.LorePanel.gameObject.SetActive(false);
                UIComponents.LorePanel.anchoredPosition = _panel.OffScreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.LorePanel.gameObject.SetActive(true))
                    .Append(UIComponents.LorePanel.DOAnchorPos(_panel.StartPos, AnimationData.MovementDuration));
            }
        }

        private class Animation_Lore_Close : SequenceUIAnimator<MainMenuUiAnimationComponents.ILorePanel, IBasePanelData>
        {
            private readonly AnimationHelper.PanelPosition _panel;

            public Animation_Lore_Close(MainMenuUiAnimationComponents.ILorePanel components,
                IBasePanelData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.LorePanel,
                    AnimationData.OffscreenDirection, AnimationData.Offset);
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.LorePanel.DOAnchorPos(_panel.OffScreenPos, AnimationData.MovementDuration))
                    .AppendCallback(() => UIComponents.LorePanel.gameObject.SetActive(false));
            }
        }

        #endregion

        #region Tutorial

        private class Animation_Tutorial_Open : SequenceUIAnimator<MainMenuUiAnimationComponents.ITutorialPanel, IBasePanelData>
        {
            private readonly AnimationHelper.PanelPosition _panel;

            public Animation_Tutorial_Open(MainMenuUiAnimationComponents.ITutorialPanel components,
                IBasePanelData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.TutorialPanel,
                    AnimationData.OffscreenDirection, AnimationData.Offset);
            }

            protected override void Initialize()
            {
                UIComponents.TutorialPanel.gameObject.SetActive(false);
                UIComponents.TutorialPanel.anchoredPosition = _panel.OffScreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.TutorialPanel.gameObject.SetActive(true))
                    .Append(UIComponents.TutorialPanel.DOAnchorPos(_panel.StartPos, AnimationData.MovementDuration));
            }
        }

        private class Animation_Tutorial_Close : SequenceUIAnimator<MainMenuUiAnimationComponents.ITutorialPanel, IBasePanelData>
        {
            private readonly AnimationHelper.PanelPosition _panel;

            public Animation_Tutorial_Close(MainMenuUiAnimationComponents.ITutorialPanel components,
                IBasePanelData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.TutorialPanel,
                    AnimationData.OffscreenDirection, AnimationData.Offset);
            }
            

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.TutorialPanel.DOAnchorPos(_panel.OffScreenPos, AnimationData.MovementDuration))
                    .AppendCallback(() => UIComponents.TutorialPanel.gameObject.SetActive(false));
            }
        }

        #endregion

        #region Credits

        private class Animation_Credits_Open : SequenceUIAnimator<MainMenuUiAnimationComponents.ICreditsPanel, IBasePanelData>
        {
            private readonly AnimationHelper.PanelPosition _panel;

            public Animation_Credits_Open(MainMenuUiAnimationComponents.ICreditsPanel components,
                IBasePanelData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.CreditsPanel,
                    AnimationData.OffscreenDirection, AnimationData.Offset);
            }

            protected override void Initialize()
            {
                UIComponents.CreditsPanel.gameObject.SetActive(false);
                UIComponents.CreditsPanel.anchoredPosition = _panel.OffScreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.CreditsPanel.gameObject.SetActive(true))
                    .Append(UIComponents.CreditsPanel.DOAnchorPos(_panel.StartPos, AnimationData.MovementDuration));
            }
        }

        private class Animation_Credits_Close : SequenceUIAnimator<MainMenuUiAnimationComponents.ICreditsPanel, IBasePanelData>
        {
            private readonly AnimationHelper.PanelPosition _panel;

            public Animation_Credits_Close(MainMenuUiAnimationComponents.ICreditsPanel components,
                IBasePanelData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.CreditsPanel,
                    AnimationData.OffscreenDirection, AnimationData.Offset);
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.CreditsPanel.DOAnchorPos(_panel.OffScreenPos, AnimationData.MovementDuration))
                    .AppendCallback(() => UIComponents.CreditsPanel.gameObject.SetActive(false));
            }
        }

        #endregion
        
        #region FirstGame

        private class Animation_FirstGame_Open : SequenceUIAnimator<MainMenuUiAnimationComponents.IFirstGamePanel, IBasePanelData>
        {
            private readonly AnimationHelper.PanelPosition _panel;

            public Animation_FirstGame_Open(MainMenuUiAnimationComponents.IFirstGamePanel components,
                IBasePanelData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.FirstGamePanel,
                    AnimationData.OffscreenDirection, AnimationData.Offset);
            }

            protected override void Initialize()
            {
                UIComponents.FirstGamePanel.gameObject.SetActive(false);
                UIComponents.FirstGamePanel.anchoredPosition = _panel.OffScreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.FirstGamePanel.gameObject.SetActive(true))
                    .Append(UIComponents.FirstGamePanel.DOAnchorPos(_panel.StartPos, AnimationData.MovementDuration));
            }
        }

        private class Animation_FirstGame_Close : SequenceUIAnimator<MainMenuUiAnimationComponents.IFirstGamePanel, IBasePanelData>
        {
            private readonly AnimationHelper.PanelPosition _panel;

            public Animation_FirstGame_Close(MainMenuUiAnimationComponents.IFirstGamePanel components,
                IBasePanelData animationData)
                : base(components, animationData)
            {
                _panel = new AnimationHelper.PanelPosition(UIComponents.FirstGamePanel,
                    AnimationData.OffscreenDirection, AnimationData.Offset);
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.FirstGamePanel.DOAnchorPos(_panel.OffScreenPos, AnimationData.MovementDuration))
                    .AppendCallback(() => UIComponents.FirstGamePanel.gameObject.SetActive(false));
            }
        }

        #endregion

        #endregion
        
        #endregion
        
        private MenuAnimator _animator;
        
        private IUIAnimator _animationMenuFirstOpen;
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
        //
        private IUIAnimator _animationFirstGameOpen;
        private IUIAnimator _animationFirstGameClose;

        private void Awake()
        {
            _animator = new MenuAnimator(this);
        }

        private void Start()
        {
            _animationMenuFirstOpen = new Animation_Menu_First_Open(UIComponents,animData.MenuFirstOpenData);
            _animationMenuOpen = new Animation_Menu_Open(UIComponents,animData.MenuOpenData);
            _animationMenuClose = new Animation_Menu_Close(UIComponents, animData.MenuCloseData);
            //
            _animationLoreOpen = new Animation_Lore_Open(UIComponents, animData.LoreOpenData);
            _animationLoreClose = new Animation_Lore_Close(UIComponents, animData.LoreCloseData);
            //
            _animationTutorialOpen = new Animation_Tutorial_Open(UIComponents, animData.TutorialOpenData);
            _animationTutorialClose = new Animation_Tutorial_Close(UIComponents, animData.TutorialCloseData);
            //
            _animationCreditsOpen = new Animation_Credits_Open(UIComponents, animData.CreditsOpenData);
            _animationCreditsClose = new Animation_Credits_Close(UIComponents, animData.CreditsCloseData);
            //
            _animationFirstGameOpen = new Animation_FirstGame_Open(UIComponents, animData.FirstGameOpenData);
            _animationFirstGameClose = new Animation_FirstGame_Close(UIComponents, animData.FirstGameCloseData);
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
                case MainMenuObserverMessage.FirstGame:
                    HandleFirstGame();
                    break;
                case MainMenuObserverMessage.StartDisable:
                    HandleDisable();
                    break;
            }
        }

        private void HandleFirstGame()
        {
            _animator.TransitionToFirstGame();
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
                    PlayAnimation(_animationMenuOpen,OnMainPanelOpened);
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
                case ScreenType.FirstGame:
                    PlayAnimation(_animationFirstGameOpen);
                    break;
            }
        }

        public void PlayCloseAnimation(ScreenType screenType, Action onClose = null)
        {
            switch (screenType)
            {
                case ScreenType.MainMenu:
                    PlayAnimation(_animationMenuClose,onClose);
                    break;
                case ScreenType.Lore:
                    PlayAnimation(_animationLoreClose,onClose);
                    break;
                case ScreenType.Tutorial:
                    PlayAnimation(_animationTutorialClose,onClose);
                    break;
                case ScreenType.Credits:
                    PlayAnimation(_animationCreditsClose,onClose);
                    break;
                case ScreenType.FirstGame:
                    PlayAnimation(_animationFirstGameClose,onClose);
                    break;
            }
        }

        public void PlayFirstOpenAnimation()
        {
            PlayAnimation(_animationMenuFirstOpen, OnMainPanelOpened);
        }
        
        public void TriggerClose()
        {
            TriggerOnPanelClosed();
        }

        #endregion
    }
}