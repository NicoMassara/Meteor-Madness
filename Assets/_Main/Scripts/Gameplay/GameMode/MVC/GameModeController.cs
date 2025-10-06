using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Gameplay.FSM.GameMode;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeController 
    {
        private readonly GameModeMotor _motor;
        private FSM<States> _fsm;
        
        private enum States
        {
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
            
            temp.Add(start);
            temp.Add(gameplay);
            temp.Add(finish);
            temp.Add(death);
            temp.Add(restart);
            temp.Add(disable);

            #endregion

            #region Transitions

            start.AddTransition(States.Gameplay, gameplay);
            
            gameplay.AddTransition(States.Finish, finish);
            gameplay.AddTransition(States.Disable, disable);
            
            finish.AddTransition(States.Death, death);
            
            death.AddTransition(States.Restart, restart);
            death.AddTransition(States.Disable, disable);
            
            restart.AddTransition(States.Start, start);
            
            disable.AddTransition(States.Start, start);

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

        #region Level 

        public void StartCountdown()
        {
            _motor.StartCountdown(GameParameters.TimeValues.StartGameCount);
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
        
        public void HandleMeteorDeflect(Vector2 position, float meteorDeflectValue)
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
    }
}