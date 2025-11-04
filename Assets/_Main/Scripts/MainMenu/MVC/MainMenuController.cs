using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.MainMenu.States;

namespace _Main.Scripts.MainMenu.MVC
{
    public class MainMenuController
    {
        private readonly MainMenuMotor _motor;
        private FSM<States> _fsm;
        private enum States
        {
            Enable,
            Initial,
            Lore,
            Tutorial,
            Disable
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
            var temp = new List<MainMenuStateBase<States>>();
            _fsm = new FSM<States>();

            #region Variables

            var enable = new MainMenuEnableState<States>();
            var disable = new MainMenuDisableState<States>();
            var initial = new MainMenuInitialState<States>();
            var lore = new MainMenuLoreState<States>();
            var tutorial = new MainMenuTutorialState<States>();
            
            temp.Add(enable);
            temp.Add(disable);
            temp.Add(initial);
            temp.Add(lore);
            temp.Add(tutorial);


            #endregion

            #region Transitions

            enable.AddTransition(States.Initial, initial);
            
            initial.AddTransition(States.Lore, lore);
            initial.AddTransition(States.Tutorial, tutorial);
            initial.AddTransition(States.Disable, disable);
            
            lore.AddTransition(States.Initial, initial);
            
            tutorial.AddTransition(States.Initial, initial);
            tutorial.AddTransition(States.Disable, disable);
            
            disable.AddTransition(States.Enable, enable);

            #endregion

            foreach (var state in temp)
            {
                state.Initialize(this);
            }
            
            _fsm.SetInit(disable);
            _fsm.FSMName = "MainMenu";
        }

        #region Transitions

        private void SetTransition(States state)
        {
            _fsm?.Transitions(state);
        }
        
        public void TransitionToInitial()
        {
            SetTransition(States.Initial);
        }

        public void TransitionToLore()
        {
            SetTransition(States.Lore);
        }

        public void TransitionToEnable()
        {
            SetTransition(States.Enable);
        }

        public void TransitionToDisable()
        {
            SetTransition(States.Disable);
        }
        
        public void TransitionToTutorial()
        {
            SetTransition(States.Tutorial);
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

        #endregion
    }

    #region States

    public class MainMenuEnableState<T> : MainMenuStateBase<T>
    {
        public override void Awake()
        {
            Controller.Enable();
        }
    }
    
    public class MainMenuDisableState<T> : MainMenuStateBase<T>
    {
        public override void Awake()
        {
            Controller.Disable();
        }
    }
    public class MainMenuLoreState<T> : MainMenuStateBase<T>
    {
        public override void Awake()
        {
            Controller.Lore();
        }
    }
    public class MainMenuInitialState<T> : MainMenuStateBase<T>
    {
        public override void Awake()
        {
            Controller.Menu();
        }
    }
    
    public class MainMenuTutorialState<T> : MainMenuStateBase<T>
    {
        public override void Awake()
        {
            Controller.Tutorial();
        }
    }

    #endregion
    
}