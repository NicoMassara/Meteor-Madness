using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.GameScreens.States;

namespace _Main.Scripts.GameScreens
{
    public class GameScreenController
    {
        private FSM<States> _fsm;
        private readonly GameScreenMotor _motor;
        
        private enum States
        {
            Disable,
            MainMenu,
            Gameplay,
            Tutorial,
        }

        public GameScreenController(GameScreenMotor motor)
        {
            _motor = motor;
        }

        public void Initialize()
        {
            InitializeFsm();
        }

        public void Execute(float deltaTime)
        {
            _fsm.Execute(deltaTime);
        }
        
        #region FSM

        private void InitializeFsm()
        {
            var temp = new List<GameScreenStateBase<States>>();
            _fsm = new FSM<States>();

            #region Variables

            var mainMenu = new GameScreenMainMenuState<States>();
            var gameplay = new GameScreenGameplayState<States>();
            var tutorial = new GameScreenTutorialState<States>();
            var disable = new GameScreenStartLoadingState<States>();
            
            temp.Add(mainMenu);
            temp.Add(gameplay);
            temp.Add(tutorial);
            temp.Add(disable);


            #endregion

            #region Transitions
            
            disable.AddTransition(States.MainMenu, mainMenu);

            mainMenu.AddTransition(States.Gameplay, gameplay);
            mainMenu.AddTransition(States.Tutorial, tutorial);
            
            gameplay.AddTransition(States.MainMenu, mainMenu);
            
            tutorial.AddTransition(States.MainMenu, mainMenu);

            #endregion

            foreach (var state in temp)
            {
                state.Initialize(this);
            }
            
            _fsm.SetInit(disable);
            _fsm.FSMName = "GameScreen";
        }

        #region Transitions

        private void SetTransition(States state)
        {
            _fsm?.Transitions(state);
        }
        
        public void TransitionToMainMenu()
        {
            SetTransition(States.MainMenu);
        }

        public void TransitionToGameplay()
        {
            SetTransition(States.Gameplay);
        }
        
        public void TransitionToTutorial()
        {
            SetTransition(States.Tutorial);
        }
        
        #endregion

        #endregion

        #region Motor

        public void SetActiveMainMenu()
        {
            _motor.SetActiveMainMenu();
        }

        public void SetActiveGameplay()
        {
            _motor.SetActiveGameplay();
        }

        public void SetActiveTutorial()
        {
            _motor.SetActiveTutorial();
        }
        
        public void SetActiveStartLoading()
        {
            _motor.SetActiveStartLoading();
        }

        #endregion


    }

    #region States

    public class GameScreenMainMenuState<T> : GameScreenStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetActiveMainMenu();
        }
    }
    
    public class GameScreenGameplayState<T> : GameScreenStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetActiveGameplay();
        }
    }
    
    public class GameScreenTutorialState<T> : GameScreenStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetActiveTutorial();
        }
    }

    public class GameScreenStartLoadingState<T> : GameScreenStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetActiveStartLoading();
        }
    }


    #endregion
}