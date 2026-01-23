using System.Collections.Generic;
using MeteorMadness.Systems;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Menu
{
    public class MainMenuController : MainMenuController.IIMainMenuController
    {
        public interface IIMainMenuController
        {
            public void Initialize();
            public void TransitionToEnable();
            public void TransitionToDisable();
            public void TransitionToMenu();
            public void TriggerGameMode();
            public void TriggerTutorial();
            public void TransitionToLore();
            public void TriggerQuit();
            public void TransitionToCredits();
            public void TransitionToTutorial();
            public void TriggerOptions();
            public void TriggerCosmetic();
            public void ExecuteDisable();
            public void MainPanelOpened();
            public void SetHasPlayed(bool hasPlayed);
            public void TriggerStats();
        }
        
        private readonly MainMenuMotor _motor;
        private FSM<States> _fsm;

        private bool _hasPlayed = true;
        
        #region States
    
        private enum States
        {
            None,
            Enable,
            Disable,
            Menu,
            FirstGame,
            Lore,
            Tutorial,
            Credits
        }
        
        private class StateBase<T> : State<T>
        {
            protected MainMenuController Controller { get; private set; }

            public void Initialize(MainMenuController controller)
            {
                Controller = controller;
            }
        }
    
        private class EnableState<T> : StateBase<T>
        {
            public override void Awake()
            {
                Controller.Enable();
            }
        }
    
        private class DisableState<T> : StateBase<T>
        {
            public override void Awake()
            {
                Controller.Disable();
            }
        }
        private class LoreState<T> : StateBase<T>
        {
            public override void Awake()
            {
                Controller.Lore();
            }
        }
        private class MenuState<T> : StateBase<T>
        {
            public override void Awake()
            {
                Controller.Menu();
            }
        }
    
        private class TutorialState<T> : StateBase<T>
        {
            public override void Awake()
            {
                Controller.Tutorial();
            }
        }
    
    
        private class CreditsState<T> : StateBase<T>
        {
            public override void Awake()
            {
                Controller.Credits();
            }
        }
        
        private class FirstGameState<T> : StateBase<T>
        {
            public override void Awake()
            {
                Controller.FirstGame();
            }
        }

        #endregion
        
        public MainMenuController(MainMenuMotor motor)
        {
            _motor = motor;
        }

        public void Initialize()
        {
            InitializeFsm();
        }

        #region FSM

        private void InitializeFsm()
        {
            var temp = new List<StateBase<States>>();
            _fsm = new FSM<States>("MainMenu");
            
            #region Variables

            var none = new StateBase<States>();
            var enable = new EnableState<States>();
            var first = new FirstGameState<States>();
            var disable = new DisableState<States>();
            var menu = new MenuState<States>();
            var lore = new LoreState<States>();
            var tutorial = new TutorialState<States>();
            var credits = new CreditsState<States>();
            
            temp.Add(none);
            temp.Add(first);
            temp.Add(enable);
            temp.Add(disable);
            temp.Add(menu);
            temp.Add(lore);
            temp.Add(tutorial);
            temp.Add(credits);

            #endregion

            #region Transitions

            none.AddTransition(States.Enable, enable);
            
            enable.AddTransition(States.Menu, menu);
            
            menu.AddTransition(States.FirstGame, first);
            menu.AddTransition(States.Lore, lore);
            menu.AddTransition(States.Tutorial, tutorial);
            menu.AddTransition(States.Disable, disable);
            menu.AddTransition(States.Credits, credits);
            
            first.AddTransition(States.Menu, menu);
            first.AddTransition(States.Tutorial, tutorial);
            first.AddTransition(States.Disable, disable);
            
            lore.AddTransition(States.Menu, menu);
            
            credits.AddTransition(States.Menu, menu);
            
            tutorial.AddTransition(States.Menu, menu);
            tutorial.AddTransition(States.Disable, disable);
            
            disable.AddTransition(States.Enable, enable);

            #endregion

            foreach (var state in temp)
            {
                state.Initialize(this);
            }
            
            _fsm.SetInit(none);
        }

        #region Transitions

        private void SetTransition(States state)
        {
            _fsm?.Transitions(state);
        }
        
        public void TransitionToEnable()
        {
            SetTransition(States.Enable);
        }

        public void TransitionToDisable()
        {
            SetTransition(States.Disable);
        }
        
        public void TransitionToMenu()
        {
            SetTransition(States.Menu);
        }

        public void TransitionToLore()
        {
            SetTransition(States.Lore);
        }
        
        public void TransitionToTutorial()
        {
            SetTransition(States.Tutorial);
        }
        
        public void TransitionToCredits()
        {
            SetTransition(States.Credits);
        }
        
        private void TransitionToFirstGame()
        {
            SetTransition(States.FirstGame);
        }
        
        #endregion

        #endregion

        #region Motor Caller

        public void Enable()
        {
            _motor.Enable();
        }

        public void Disable()
        {
            _motor.StartDisable();
        }
        
        public void ExecuteDisable()
        {
            if (_fsm.CurrentState == States.Disable)
            {
                _motor.Disable();
            }
        }

        public void MainPanelOpened()
        {
            _motor.TriggerMainPanelOpen();
        }

        public void SetHasPlayed(bool hasPlayed)
        {
            _hasPlayed = hasPlayed;
        }

        public void TriggerStats()
        {
            _motor.Stats();
        }

        public void Lore()
        {
            _motor.Lore();
        }

        public void Menu()
        {
            _motor.Menu();
        }
        
        public void Tutorial()
        {
            _motor.Tutorial();
        }

        public void TriggerGameMode()
        {
            if (_hasPlayed == false)
            {
                TransitionToFirstGame();
            }
            else
            {
                _motor.TriggerGameMode();
            }
        }
        
        public void TriggerTutorial()
        {
            _motor.TriggerTutorial();
        }

        public void TriggerQuit()
        {
            _motor.TriggerQuit();
        }
        
        public void TriggerCosmetic()
        {
            _motor.TriggerCosmetic();
        }
        
        public void TriggerOptions()
        {
            _motor.TriggerOptions();
        }
        
        public void Credits()
        {
            _motor.Credits();
        }
        
        public void FirstGame()
        {
            _motor.FirstGame();
        }
        #endregion
    }
    
}