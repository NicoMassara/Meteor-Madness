using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Gameplay.GameMode.States;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeController 
    {
        private readonly GameModeMotor _motor;
        private FSM<States> _fsm;
        
        private enum States
        {
            Enable,
            Start,
            Gameplay,
            Finish,
            Death,
            Restart,
            Disable
        }

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
            var temp = new List<GameModeStateBase<States>>();
            _fsm = new FSM<States>();

            #region Variables

            var start = new GameModeStartState<States>();
            var gameplay = new GameModeGameplayState<States>();
            var finish = new GameModeFinishState<States>();
            var death = new GameModeDeathState<States>();
            var restart = new GameModeRestartState<States>();
            var disable = new GameModeDisableState<States>();
            var enable = new GameModeEnableState<States>();
            
            temp.Add(enable);
            temp.Add(start);
            temp.Add(gameplay);
            temp.Add(finish);
            temp.Add(death);
            temp.Add(restart);
            temp.Add(disable);

            #endregion

            #region Transitions
            
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
            
            _fsm.SetInit(disable);
            _fsm.FSMName = "GameMode";
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
            _motor.HandleMeteorDeflect(position, meteorDeflectValue);
        }

        public void SetEnableMeteorSpawn(bool canSpawn)
        {
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
            _motor.SetDoublePoints(isEnable);
        }

        public void GrantProjectileSpawn(int projectileTypeIndex)
        {
            _motor.GrantSpawnMeteor(projectileTypeIndex);
        }

        public void SetEnable()
        {
            _motor.Enable();
        }

        #endregion
    }

    #region States

    public class GameModeEnableState<T> : GameModeStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetEnable();
        }
    }
    
    public class GameModeDisableState<T> : GameModeStateBase<T>
    {
        public override void Awake()
        {
            Controller.DisableGameMode();
        }
    }
    
    public class GameModeDeathState<T> : GameModeStateBase<T>
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
    
    public class GameModeFinishState<T> : GameModeStateBase<T>
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
    
    public class GameModeGameplayState<T> : GameModeStateBase<T>
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
    
    public class GameModeRestartState<T> : GameModeStateBase<T>
    {
        public override void Awake()
        {
            Controller.GameRestart();
        }
    }
    
    public class GameModeStartState<T> : GameModeStateBase<T>
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