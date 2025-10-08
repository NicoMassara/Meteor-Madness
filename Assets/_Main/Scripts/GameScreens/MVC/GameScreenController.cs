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
            MainMenu,
            Gameplay
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
            
            temp.Add(mainMenu);
            temp.Add(gameplay);


            #endregion

            #region Transitions

            mainMenu.AddTransition(States.Gameplay, gameplay);
            
            gameplay.AddTransition(States.MainMenu, mainMenu);

            #endregion

            foreach (var state in temp)
            {
                state.Initialize(this);
            }
            
            _fsm.SetInit(mainMenu);
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
        
        #endregion

        #endregion
        
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
    }
}