using System.Collections.Generic;
using _Main.Scripts.DebugGUI;
using _Main.Scripts.FiniteStateMachine;

namespace _Main.Scripts.MainMenu.MVC
{
    public class MainMenuController
    {
        private readonly MainMenuMotor _motor;
        private FSM<States> _fsm;
        private enum States
        {
            None,
            Enable,
            Disable,
            Menu,
            Lore,
            Tutorial,
            Credits,
            Options
        }

        
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
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _fsm.CreateDebugGUI(DebugGUISortingOrder.SubGroup.MainMenu);
#endif
            
            #region Variables

            var none = new StateBase<States>();
            var enable = new EnableState<States>();
            var disable = new DisableState<States>();
            var menu = new MenuState<States>();
            var lore = new LoreState<States>();
            var tutorial = new TutorialState<States>();
            var credits = new CreditsState<States>();
            var options = new OptionsState<States>();
            
            temp.Add(none);
            temp.Add(enable);
            temp.Add(disable);
            temp.Add(menu);
            temp.Add(lore);
            temp.Add(tutorial);
            temp.Add(credits);
            temp.Add(options);


            #endregion

            #region Transitions

            none.AddTransition(States.Enable, enable);
            
            enable.AddTransition(States.Menu, menu);
            
            menu.AddTransition(States.Lore, lore);
            menu.AddTransition(States.Tutorial, tutorial);
            menu.AddTransition(States.Disable, disable);
            menu.AddTransition(States.Credits, credits);
            menu.AddTransition(States.Options, options);
            
            lore.AddTransition(States.Menu, menu);
            
            credits.AddTransition(States.Menu, menu);
            
            tutorial.AddTransition(States.Menu, menu);
            tutorial.AddTransition(States.Disable, disable);
            
            options.AddTransition(States.Menu, menu);
            
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
        
        public void TransitionToOptions()
        {
            SetTransition(States.Options);
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
            _motor.Disable();
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
            _motor.TriggerGameMode();
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
        
        public void Credits()
        {
            _motor.Credits();
        }
        
        public void Options()
        {
            _motor.Options();
        }

        #endregion
        
    }

    #region States
    
    public class StateBase<T> : State<T>
    {
        protected MainMenuController Controller { get; private set; }

        public void Initialize(MainMenuController controller)
        {
            Controller = controller;
        }
    }
    
    public class EnableState<T> : StateBase<T>
    {
        public override void Awake()
        {
            Controller.Enable();
        }
    }
    
    public class DisableState<T> : StateBase<T>
    {
        public override void Awake()
        {
            Controller.Disable();
        }
    }
    public class LoreState<T> : StateBase<T>
    {
        public override void Awake()
        {
            Controller.Lore();
        }
    }
    public class MenuState<T> : StateBase<T>
    {
        public override void Awake()
        {
            Controller.Menu();
        }
    }
    
    public class TutorialState<T> : StateBase<T>
    {
        public override void Awake()
        {
            Controller.Tutorial();
        }
    }
    
    
    public class CreditsState<T> : StateBase<T>
    {
        public override void Awake()
        {
            Controller.Credits();
        }
    }
    
    public class OptionsState<T> : StateBase<T>
    {
        public override void Awake()
        {
            Controller.Options();
        }
    }

    #endregion
    
}