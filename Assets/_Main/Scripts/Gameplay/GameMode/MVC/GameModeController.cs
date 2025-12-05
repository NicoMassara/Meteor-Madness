using System;
using System.Collections.Generic;
using UnityEngine;
using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Interfaces;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeController : 
        GameModeController.IGameModeController,
        GameModeController.IController
    {
        public interface IGameModeController
        {
            public void TransitionToInitialize();
            public void TransitionToCountDown();
            public void TransitionToPlaying();
            public void TransitionToPaused();
            public void TransitionToFinished();
            public void TransitionToDisable();
            //
            public void InitializeController(IGameTimeData gameTimeData);
            public void ExecuteDisable();
            public void EnablePause();
            public void DisablePause();
            public void HandleMeteorDeflect(Vector2 position, float projectileValue);
            public void GrantProjectileSpawn(int projectileTypeIndex);
            public void SetDoublePoints(bool isActive);
            public void IncreaseCollisionCount();
            public void IncreaseAbilityUseCount();
            public void IncreaseDeflectCount();
            public void DisableGameplayUI();
            public void EnableGameplayUI();
            
            public void Execute(float deltaTime);
        }
        
        #region Private Classes
        
        private interface IController
        {
            public void StartDisable();
            public void PauseGame();
            public void InitializeData();
            public void SaveHighScoreValues();
            public void FinishCountdown();
            public void StartCountdown();
            public void StartGameplay();
            public void StopGameplay();
            public void UpdateCountdown(float remainingTime);
            public void DisableProjectileSpawn();
            public void FinishGame();
        }

        private class MainController
        {
            #region States
            
            private enum States
            {
                None,
                Initialize,
                Countdown,
                Playing,
                Paused,
                Finished,
                Disable
            }
            
            private class BaseState<T> : State<T>
            {
                protected IController Controller { get; private set; }

                public void Initialize(IController controller)
                {
                    this.Controller = controller;
                }
            }

            private class InitializeState<T> : BaseState<T>
            {
                public override void Awake()
                {
                    Controller.InitializeData();
                }
            }
            
            private class CountdownState<T> : BaseState<T>
            {
                private readonly float _countdownTime;

                public CountdownState(float countdownTime)
                {
                    _countdownTime = countdownTime + 1;
                }

                private float _startTimer;
                private float _lastDisplayedTimer;
                
                public override void Awake()
                {
                    _startTimer = _countdownTime;
                    Controller.StartCountdown();
                }

                public override void Execute(float deltaTime)
                {
                    _startTimer -= deltaTime;
                    int seconds = Mathf.CeilToInt(_startTimer);
                    
                    if (seconds != _lastDisplayedTimer)
                    {
                        _lastDisplayedTimer = seconds;
                        
                        Controller.UpdateCountdown(_startTimer);

                        if (_startTimer <= 0)
                        {
                            Controller.FinishCountdown();
                        }
                    }
                }
            }
            
            private class PlayingState<T> : BaseState<T>
            {
                public override void Awake()
                {
                    Controller.StartGameplay();
                }

                public override void Sleep()
                {
                    Controller.StopGameplay();
                }
            }
            
            private class PausedState<T> : BaseState<T>
            {
                public override void Awake()
                {
                    Controller.PauseGame();
                }
            }
            
            private class FinishedState<T> : BaseState<T>
            {
                public override void Awake()
                {
                    Controller.DisableProjectileSpawn();
                    Controller.SaveHighScoreValues();
                    Controller.FinishGame();
                    
                }
            }
            
            private class DisableState<T> : BaseState<T>
            {
                public override void Awake()
                {
                    Controller.StartDisable();
                }
            }

            #endregion
            
            private FSM<States> _fsm;
            
            private GameModeActionGate _actionGate;
            private class GameModeActionGate : FsmActionGate<States>
            {
                public bool WasPaused { get; private set; }
                public bool IsPaused { get; private set; }
                public bool IsPlaying { get; private set; }
                public GameModeActionGate(FSM<States> fsm) : base(fsm) { }
                
                protected override void OnEnterState(States state)
                {
                    IsPlaying = state == States.Playing;
                    IsPaused = state == States.Paused;
                }

                protected override void OnExitState(States state)
                {
                    WasPaused = state == States.Paused;
                }
            }

            public MainController(IController controller, IGameTimeData gameTimeData)
            {
                InitializeFsm(controller,gameTimeData);
            }

            public void Execute(float deltaTime)
            {
                _fsm?.Execute(deltaTime);
            }

            #region FSM
            
            private void InitializeFsm(IController controller, IGameTimeData gameTimeData)
            {
                var temp = new List<BaseState<States>>();
                _fsm = new FSM<States>("GameMode");
                _actionGate = new GameModeActionGate(_fsm);
                
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                _fsm.CreateDebugGUI(1);
    #endif

                #region Variables

                var none = new BaseState<States>();
                var initialize = new InitializeState<States>();
                var countdown = new CountdownState<States>(gameTimeData.StartGameCount);
                var playing = new PlayingState<States>();
                var paused = new PausedState<States>();
                var finished = new FinishedState<States>();
                var disable = new DisableState<States>();
                
                temp.Add(none);
                temp.Add(initialize);
                temp.Add(countdown);
                temp.Add(playing);
                temp.Add(paused);
                temp.Add(finished);
                temp.Add(disable);
                
                #endregion

                #region Transitions
                
                none.AddTransition(States.Initialize, initialize);
                //
                initialize.AddTransition(States.Countdown, countdown);
                //
                countdown.AddTransition(States.Playing, playing);
                //
                playing.AddTransition(States.Paused, paused);
                playing.AddTransition(States.Finished, finished);
                //
                paused.AddTransition(States.Countdown, countdown);  
                //
                finished.AddTransition(States.Disable, disable);

                #endregion

                foreach (var state in temp)
                {
                    state.Initialize(controller);
                }
                
                _fsm.SetInit(none);
            }

            #region Transitions

            private void SetTransition(States state)
            {
                _fsm?.Transitions(state);
            }
            
            public void TransitionToInitialize()
            {
                SetTransition(States.Initialize);
            }

            public void TransitionToCountDown()
            {
                SetTransition(States.Countdown);
            }

            public void TransitionToPlaying()
            {
                SetTransition(States.Playing);
            }
            
            public void TransitionToPaused()
            {
                SetTransition(States.Paused);
            }
            
            public void TransitionToFinished()
            {
                SetTransition(States.Finished);
            }
            
            public void TransitionToDisable()
            {
                SetTransition(States.Disable);
            }
            
            #endregion

            #endregion

            #region Public Getters

            public bool GetWasPaused() => _actionGate.WasPaused;
            public bool GetIsInGameplay() => _actionGate.IsPlaying;
            public bool GetIsPaused() => _actionGate.IsPaused;

            #endregion
            
        }
        
        #endregion
        
        private readonly GameModeMotor _motor;
        private MainController _mainController;
        
        
        public GameModeController(GameModeMotor motor)
        {
            _motor = motor;
        }

        public void InitializeController(IGameTimeData gameTimeData)
        {
            _mainController = new MainController(this,gameTimeData);
        }

        public void Execute(float deltaTime)
        {
            _mainController?.Execute(deltaTime);
        }

        #region IController

        public void StartDisable()
        {
            _motor.StartDisable();
        }

        public void PauseGame()
        {
            _motor.PauseGame();
        }
        
        public void UnPauseGame()
        {
            _motor.UnPauseGame();
        }
        
        public void InitializeData()
        {
            _motor.InitializeData();
        }
        
        public void SaveHighScoreValues()
        {
            _motor.SaveScore();   
        }
        
        public void FinishCountdown()
        {
            if (_mainController.GetWasPaused())
            {
                _motor.UnPauseGame();
            }
            else
            {
                _motor.EnableProjectileSpawn();
            }
            
            _motor.FinishCountdown();
        }
        
        public void DisableProjectileSpawn()
        {
            _motor.DisableProjectileSpawn();
        }
        
        public void StartCountdown()
        {
            _motor.StartCountdown();
        }
        
        public void StartGameplay()
        {
            _motor.StartGameplay();
        }
        
        public void StopGameplay()
        {
            _motor.StopGameplay();
        }

        public void FinishGame()
        {
            _motor.FinishGame();
        }

        public void UpdateCountdown(float remainingTime)
        {
            _motor.UpdateCountdown(remainingTime);
        }

        #endregion

        #region IGameModeController

        #region Trantiions
        public void TransitionToInitialize()
        {
            if (_mainController.GetIsPaused())
            {
                _mainController.TransitionToCountDown();

            }
            else
            {
                _mainController.TransitionToInitialize();
            }
        }

        public void TransitionToCountDown()
        {
            _mainController.TransitionToCountDown();
        }

        public void TransitionToPlaying()
        {
            _mainController.TransitionToPlaying();
        }

        public void TransitionToPaused()
        {
            _mainController.TransitionToPaused();
        }

        public void TransitionToFinished()
        {
            _mainController.TransitionToFinished();
        }

        public void TransitionToDisable()
        {
            if (_mainController.GetIsPaused())
            {
                _motor.TriggerPause();

            }
            else
            {
                _mainController.TransitionToDisable();
            }
        }

        #endregion

        public void ExecuteDisable()
        {
            _motor.ExecuteDisable();
        }

        public void EnablePause()
        {
            _motor.EnablePause();
        }

        public void DisablePause()
        {
            _motor.DisablePause();
        }

        public void HandleMeteorDeflect(Vector2 position, float projectileValue)
        {
            if(_mainController.GetIsInGameplay())
                _motor.HandleMeteorDeflect(position, projectileValue);
        }

        public void GrantProjectileSpawn(int projectileTypeIndex)
        {
            if(_mainController.GetIsInGameplay())
                _motor.GrantProjectileSpawn(projectileTypeIndex);
        }

        public void SetDoublePoints(bool isActive)
        {
            _motor.SetDoublePoints(isActive);
        }

        #region Stats

        public void IncreaseCollisionCount()
        {
            if(_mainController.GetIsInGameplay())
                return;
            _motor.IncreaseCollisionCount();
        }

        public void IncreaseAbilityUseCount()
        {
            if(_mainController.GetIsInGameplay())
                return;
            _motor.IncreaseAbilityUseCount();
        }
        
        public void IncreaseDeflectCount()
        {
            if(_mainController.GetIsInGameplay())
                return;
            _motor.IncreaseDeflectCount();
        }

        #endregion

        #region UI

        public void DisableGameplayUI()
        {
            _motor.DisableGameplayUI();
        }

        public void EnableGameplayUI()
        {
            _motor.EnableGameplayUI();
        }

        #endregion

        #endregion
        
    }
}