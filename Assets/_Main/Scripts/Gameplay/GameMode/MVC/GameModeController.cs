using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeController 
    {
        private readonly GameModeMotor _motor;
        private FSM<States> _fsm;
        
        private enum States
        {
            None,
            Enable,
            Start,
            Gameplay,
            Finish,
            Death,
            Restart,
            Disable
        }
        
        private class ActionGate
        {
            public bool IsInGameplay { get; private set; }
            public ActionGate(FSM<States> fsm)
            {
                fsm.OnEnterState += state =>
                {
                    IsInGameplay = state is States.Gameplay or States.Enable or States.Start;
                };
            }
        }
        
        private ActionGate _actionGate;

        public GameModeController(GameModeMotor motor)
        {
            _motor = motor;
        }

        public void Initialize()
        {
            InitializeFsm();
            InitializeValues();
        }

        public void Execute(float deltaTime)
        {
            _fsm?.Execute(deltaTime);
        }

        #region FSM

        private void InitializeFsm()
        {
            var temp = new List<BaseState<States>>();
            _fsm = new FSM<States>("GameMode");
            _actionGate = new ActionGate(_fsm);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _fsm.CreateDebugGUI(1);
#endif

            #region Variables

            var none = new BaseState<States>();
            var enable = new EnableState<States>();
            var start = new StartState<States>();
            var gameplay = new GameplayState<States>();
            var finish = new FinishState<States>();
            var death = new DeathState<States>();
            var restart = new RestartState<States>();
            var disable = new DisableState<States>();
            
            temp.Add(none);
            temp.Add(enable);
            temp.Add(start);
            temp.Add(gameplay);
            temp.Add(finish);
            temp.Add(death);
            temp.Add(restart);
            temp.Add(disable);

            #endregion

            #region Transitions
            
            none.AddTransition(States.Enable, enable);
            //
            enable.AddTransition(States.Start, start);
            
            start.AddTransition(States.Gameplay, gameplay);
            
            gameplay.AddTransition(States.Finish, finish);
            gameplay.AddTransition(States.Disable, disable);
            
            finish.AddTransition(States.Death, death);
            
            death.AddTransition(States.Restart, restart);
            death.AddTransition(States.Disable, disable);
            
            restart.AddTransition(States.Start, start);
            
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
        
        public void TransitionToStart()
        {
            SetTransition(States.Start);
        }

        public void TransitionToGameplay()
        {
            SetTransition(States.Gameplay);
        }
        
        public void TransitionToFinish()
        {
            SetTransition(States.Finish);
        }
        
        public void TransitionToDeath()
        {
            SetTransition(States.Death);
        }
        
        public void TransitionToRestart()
        {
            SetTransition(States.Restart);
        }

        public void TransitionToDisable()
        {
            SetTransition(States.Disable);
        }

        #endregion

        #endregion
        
        #region Motor
        
        #region Level 

        public void StartCountdown()
        {
            _motor.StartCountdown();
        }

        public void StartGameplay()
        {
            _motor.StartGameplay();
        }

        public void RestartValues()
        {
            _motor.RestartValues();
        }

        #endregion

        #region Earth

        public void HandleEarthStartDestruction()
        {
            _motor.HandleEarthStartDestruction();
        }

        public void HandleEarthEndDestruction()
        {
            _motor.HandleEarthEndDestruction();
        }
        
        public void HandleEarthShake()
        {
            _motor.HandleEarthShake();
        }

        #endregion

        public void HandleProjectileDeflect(Vector2 position, float meteorDeflectValue)
        {
            if(_actionGate.IsInGameplay == false) return;
            _motor.HandleMeteorDeflect(position, meteorDeflectValue);
        }

        public void SetEnableMeteorSpawn(bool canSpawn)
        {
            if(_actionGate.IsInGameplay == false) return;
            
            _motor.SetEnableMeteorSpawn(canSpawn);
        }

        public void HandleGameFinish()
        {
            _motor.HandleGameFinish();
        }

        public void HandleCountdownTimer(float deltaTime)
        {
            _motor.HandleCountdownTimer(deltaTime);
        }

        public void GameRestart()
        {
            _motor.GameRestart();
        }
        
        public void EarthRestartFinish()
        {
            _motor.EarthRestartFinish();
        }

        public void SetGamePause(bool isPaused)
        {
            _motor.SetGamePaused(isPaused);
        }
        
        public void DisableGameMode()
        {
            _motor.DisableGameMode();
        }

        private void InitializeValues()
        {
            _motor.InitializeValues();
        }

        public void SetDoesRestartGameMode(bool doesRestart)
        {
            _motor.SetDoesRestartGameMode(doesRestart);
        }

        public void SetDoublePoints(bool isEnable)
        {
            if(_actionGate.IsInGameplay == false) return;
            
            _motor.SetDoublePoints(isEnable);
        }

        public void GrantProjectileSpawn(int projectileTypeIndex)
        {
            if(_actionGate.IsInGameplay == false) return;
            
            _motor.GrantSpawnMeteor(projectileTypeIndex);
        }

        public void SetEnable()
        {
            _motor.Enable();
        }

        public void SetCanPause(bool canPause)
        {
            _motor.SetCanPause(canPause);
        }

        #endregion

        public void HandleCameraZoomOut()
        {
            if(_actionGate.IsInGameplay == false) return;
            _motor.HandleCameraZoomOut();
        }

        public void HandleCameraZoomIn()
        {
            if(_actionGate.IsInGameplay == false) return;
            _motor.HandleCameraZoomIn();
        }
    }

    #region States

    public class BaseState<T> : State<T>
    {
        protected GameModeController Controller { get; private set; }

        public void Initialize(GameModeController controller)
        {
            this.Controller = controller;
        }
    }
    
    public class EnableState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.SetEnable();
        }
    }
    
    public class DisableState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.DisableGameMode();
        }
    }
    
    public class DeathState<T> : BaseState<T>
    {
        private ActionQueue _actionQueue = new ActionQueue();
        
        public override void Awake()
        {
            Controller.HandleEarthEndDestruction();
        }

        public override void Execute(float deltaTime)
        {
            _actionQueue.Run(deltaTime);
        }
    }
    
    public class FinishState<T> : BaseState<T>
    {
        private ActionQueue _actionQueue = new ActionQueue();
        
        public override void Awake()
        {
            Controller.HandleGameFinish();
            Controller.HandleEarthStartDestruction();
        }

        public override void Execute(float deltaTime)
        {
            _actionQueue.Run(deltaTime);
        }
    }
    
    public class GameplayState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.SetEnableMeteorSpawn(true);
        }
        
        public override void Sleep()
        {
            Controller.SetEnableMeteorSpawn(false);
        }
    }
    
    public class RestartState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.GameRestart();
        }
    }
    
    public class StartState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.RestartValues();
            Controller.StartCountdown();
        }

        public override void Execute(float deltaTime)
        {
            Controller.HandleCountdownTimer(deltaTime);
        }

        public override void Sleep()
        {
            Controller.StartGameplay();
        }
    }

    #endregion
}