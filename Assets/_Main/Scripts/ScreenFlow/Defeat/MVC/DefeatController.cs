using System.Collections.Generic;
using MeteorMadness.GlobalValues.Tools;
using MeteorMadness.Systems;


namespace MeteorMadness.ScreenFlow.Defeat
{
    public class DefeatController : 
        DefeatController.IDefeatController,
        DefeatController.IController
    {
        public interface IDefeatController
        {
            public void InitializeController();
            public void InitializeData();
            public void StartDisable();
            public void EnableScreen();
            public void ExecuteDisable();
            public void LoadScoreData(DefeatScreenData score);
            public void SendScore();
            public void SendHighScore();
            public void SendButtons();
            public void SetDataIsLoaded();
            public void EnableButtons();
            public void SendAd();
            public void RestartGame();
            public void LoadMainMenu();
            public void SendCoins();
            public void CheckForNewCoins();
            public void UpdateCoins(uint stored, uint gained);
        }

        #region Private Classes

        private interface IController
        {
            public void Enable();
            public void Disable();
            public void LoadData();
        }
        
        private class MainController
        {
            #region States

            private enum States
            {
                None,
                Initialize,
                Enable,
                Disable
            }

            private class BaseState<T> : State<T>
            {
                protected IController Controller { get; private set; }

                public void Initialize(IController controller)
                {
                    Controller = controller;
                }
            }

            private class EnableState<T> : BaseState<T>
            {
                public override void Awake() => Controller.Enable();
            }
            
            private class DisableState<T> : BaseState<T>
            {
                public override void Awake() => Controller.Disable();
            }
            
            private class InitializeState<T> : BaseState<T>
            {
                public override void Awake() => Controller.LoadData();
            }

            
            #endregion
            
            private class ActionGate : FsmActionGate<States>
            {
                public bool IsDisable { get; private set; }
                public bool IsDataLoaded { get; set; }
                
                public ActionGate(FSM<States> fsm) : base(fsm) { }

                protected override void OnEnterState(States state)
                {
                    IsDisable = state == States.Disable;
                }
            }

            private FSM<States> _fsm;
            private ActionGate _actionGate;

            public MainController(IController controller)
            {
                InitializeFsm(controller);
            }

            #region FSM

            private void InitializeFsm(IController controller)
            {
                var temp = new List<BaseState<States>>();
                _fsm = new FSM<States>("Defeat");
                _actionGate = new ActionGate(_fsm);

                #region Varibales

                var none = new BaseState<States>();
                var initialize = new InitializeState<States>();
                var enable = new EnableState<States>();
                var disable = new DisableState<States>();
                
                temp.Add(none);
                temp.Add(initialize);
                temp.Add(enable);
                temp.Add(disable);

                #endregion

                #region Transitions

                none.AddTransition(States.Initialize, initialize);
                //
                initialize.AddTransition(States.Enable, enable);
                //
                enable.AddTransition(States.Disable, disable);
                //
                disable.AddTransition(States.Initialize, initialize);

                #endregion
                
                foreach (var state in temp)
                {
                    state.Initialize(controller);
                }
            
                _fsm.SetInit(none);
            }

            #region Tranisionts

            private void SetTransition(States state)
            {
                _fsm?.Transitions(state);
            }
            
            public void TransitionToEnable()
            {
                if(_actionGate.IsDataLoaded)
                    SetTransition(States.Enable);
            }
        
            public void TransitionToDisable()
            {
                SetTransition(States.Disable);
            }
            
            public void TransitionToInitialize()
            {
                SetTransition(States.Initialize);
            }

            #endregion

            #endregion

            #region Public Getters

            public bool GetIsDisable() => _actionGate.IsDisable;

            #endregion

            public void SetDataIsLoaded()
            {
                _actionGate.IsDataLoaded = true;
            }

            public void UnloadData()
            {
                _actionGate.IsDataLoaded = false;
            }
        }

        #endregion
        
        private readonly DefeatMotor _motor;
        private MainController _controller;

        public DefeatController(DefeatMotor motor)
        {
            _motor = motor;
        }

        #region IDefeatController

        public void InitializeController()
        {
            _controller = new MainController(this);
        }

        public void InitializeData()
        {
            _controller.TransitionToInitialize();
        }

        public void StartDisable()
        {
            if (_controller.GetIsDisable()) return;
            
            _controller.TransitionToDisable();
        }

        public void EnableScreen() => _controller.TransitionToEnable();

        public void ExecuteDisable()
        {
            if (_controller.GetIsDisable())
            {
                _motor.ExecuteDisable();
            }
        }

        public void LoadScoreData(DefeatScreenData score)
            => _motor.LoadScoreData(score);

        public void SendScore() => _motor.SendScore();
        public void SendHighScore() => _motor.SendHighScore();
        public void SendButtons() => _motor.SendButtons();
        public void SetDataIsLoaded() => _controller.SetDataIsLoaded();
        public void EnableButtons() => _motor.EnableButtons();
        public void SendAd() => _motor.SendAd();
        public void RestartGame() => _motor.RestartGame();
        public void LoadMainMenu() => _motor.LoadMainMenu();
        public void SendCoins() => _motor.SendCoins();
        public void CheckForNewCoins() => _motor.CheckForNewCoins();
        public void UpdateCoins(uint stored, uint gained) => _motor.UpdateCoins(stored, gained);

        #endregion

        #region IController

        public void Enable() => _motor.Enable();

        public void Disable()
        {
            _controller.UnloadData();
            _motor.StartDisable();
        }

        public void LoadData() => _motor.LoadData();

        #endregion
    }
}