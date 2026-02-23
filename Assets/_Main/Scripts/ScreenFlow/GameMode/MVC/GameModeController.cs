using System;
using System.Collections.Generic;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces.GameplayData;
using MeteorMadness.Systems;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.GameMode
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
            public void SetDoublePoints(bool isActive);
            public void IncreaseCollisionCount();
            public void IncreaseAbilityUseCount();
            public void IncreaseDeflectCount();
            public void DisableGameplayUI();
            public void EnableGameplayUI();
            public void Execute(float deltaTime);
            public void TransitionToSaveScore();
            public void TriggerFinishAddingPoints();
            public void TriggerPauseMenu();
            public void NotifyAbilityActive(AbilityType abilityType);
            public void SetHasLoseFocus(bool hasFocus);
            public void NotifyBatchDeflected();
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
            public void StartFinish();
            
            public void UpdateTimer(float deltaTime);
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
                SaveScore,
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
                private readonly int _timerCount;
                private const float TimeStep = 1.25f;
                private float _elapsedTime;
                private int _currentCount;

                public CountdownState(float countdownTime)
                {
                    _timerCount = (int)countdownTime;
                }
                
                private float _lastDisplayedTimer;
                
                public override void Awake()
                {
                    _elapsedTime = 0;
                    _currentCount = _timerCount;
                    Controller.StartCountdown();
                    Controller.UpdateCountdown(_currentCount);
                }

                public override void Execute(float deltaTime)
                {
                    _elapsedTime += deltaTime;

                    if (_elapsedTime >= TimeStep &&
                        _currentCount > 0)
                    {
                        _elapsedTime -= TimeStep;
                        _currentCount--;

                        if (_currentCount <= 0)
                            Controller.FinishCountdown();
                        else
                            Controller.UpdateCountdown(_currentCount);
                    }
                }
            }
            private class PlayingState<T> : BaseState<T>
            {
                public override void Awake()
                {
                    Controller.StartGameplay();
                }

                public override void Execute(float deltaTime)
                {
                    Controller.UpdateTimer(deltaTime);
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
                    Controller.StartFinish();
                }

                public override void Sleep()
                {
                    Controller.FinishGame();
                }
            }
            
            private class SaveScoreState<T> : BaseState<T>
            {
                public override void Awake()
                {
                    Controller.DisableProjectileSpawn();
                    Controller.SaveHighScoreValues();
                    
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
                public bool HasSavedScore { get; private set; }
                public bool IsGoingToPause { get; private set; }

                public GameModeActionGate(FSM<States> fsm) : base(fsm) { }
                
                protected override void OnEnterState(States state)
                {
                    IsPlaying = state == States.Playing;
                    IsPaused = state == States.Paused;
                }

                protected override void OnExitState(States state)
                {
                    WasPaused = state == States.Paused;
                    HasSavedScore = state == States.SaveScore;
                }

                protected override void OnNewState(States state)
                {
                    IsGoingToPause = state == States.Paused;
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
                

                #region Variables

                var none = new BaseState<States>();
                var initialize = new InitializeState<States>();
                var countdown = new CountdownState<States>(gameTimeData.StartGameCount);
                var playing = new PlayingState<States>();
                var paused = new PausedState<States>();
                var finished = new FinishedState<States>();
                var saveScore = new SaveScoreState<States>();
                var disable = new DisableState<States>();
                
                temp.Add(none);
                temp.Add(initialize);
                temp.Add(countdown);
                temp.Add(playing);
                temp.Add(paused);
                temp.Add(finished);
                temp.Add(saveScore);
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
                playing.AddTransition(States.SaveScore, saveScore);
                //
                paused.AddTransition(States.Countdown, countdown);  
                paused.AddTransition(States.Finished, finished);  
                //
                finished.AddTransition(States.Disable, disable);
                //
                saveScore.AddTransition(States.Disable, disable);
                //
                disable.AddTransition(States.Initialize, initialize);

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
            
            public void TransitionToSaveScore()
            {
                SetTransition(States.SaveScore);
            }
            
            #endregion

            #endregion

            #region Public Getters

            public bool GetWasPaused() => _actionGate.WasPaused;
            public bool GetIsInGameplay() => _actionGate.IsPlaying;
            public bool GetIsPaused() => _actionGate.IsPaused;

            #endregion
            
            public bool GetWasFinished() => _actionGate.HasSavedScore;

            public bool GetIsGoingToPause() => _actionGate.IsGoingToPause;
        }
        
        #endregion
        
        private readonly GameModeMotor _motor;
        private MainController _mainController;
        private bool _hasLoseFocus;
        
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

        public void StartDisable() => _motor.StartDisable();

        public void PauseGame() => _motor.PauseGame();

        public void InitializeData() => _motor.InitializeData();

        public void SaveHighScoreValues() => _motor.SaveScore();

        public void FinishCountdown()
        {
            if (_mainController.GetWasPaused() == false)
                _motor.EnableProjectileSpawn();
            
            _motor.FinishCountdown();
        }
        public void DisableProjectileSpawn() => _motor.DisableProjectileSpawn();
        public void StartCountdown() => _motor.StartCountdown();
        public void StartGameplay()
        {
            _motor.StartGameplay();
            
            if(_hasLoseFocus) 
                _mainController.TransitionToPaused();
        }

        public void StopGameplay()
        {
            //if(_mainController.GetIsGoingToPause()) return;
            _motor.StopGameplay();
        }

        public void FinishGame() => _motor.FinishGame();
        public void StartFinish() => _motor.StartFinish();

        public void UpdateTimer(float deltaTime)
        {
            if (_mainController.GetIsInGameplay())
                _motor.UpdateTimer(deltaTime);
        }

        public void UpdateCountdown(float remainingTime) => _motor.UpdateCountdown(remainingTime);

        #endregion

        #region IGameModeController

        #region Trantiions
        public void TransitionToInitialize()
        {
            if (_mainController.GetIsPaused())
                _mainController.TransitionToCountDown();
            else
                _mainController.TransitionToInitialize();
        }

        public void TransitionToCountDown() => _mainController.TransitionToCountDown();

        public void TransitionToPlaying()
        {
            _mainController.TransitionToPlaying();
        }

        public void TransitionToPaused() => _mainController.TransitionToPaused();

        public void TransitionToFinished() => _mainController.TransitionToFinished();

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
        
        public void TransitionToSaveScore()
        {
            _mainController.TransitionToSaveScore();
        }

        public void TriggerFinishAddingPoints()
        {
            if(_mainController.GetIsInGameplay())
                _motor.TriggerFinishAddingPoints();
        }

        public void TriggerPauseMenu()
        {
            if(_mainController.GetIsPaused())
                _motor.TriggerPauseMenu();
        }

        public void NotifyAbilityActive(AbilityType abilityType)
        {
            if (_mainController.GetIsInGameplay())
                _motor.NotifyAbilityActive(abilityType);
        }

        public void SetHasLoseFocus(bool hasFocus)
        {
            _hasLoseFocus = hasFocus;

            if (_hasLoseFocus) 
                _mainController.TransitionToPaused();
        }

        #endregion

        public void ExecuteDisable()
        {
            if (_mainController.GetWasFinished())
            {
                _motor.TriggerEarthDestruction();
            }

            _motor.ExecuteDisable();
        }

        public void EnablePause() => _motor.EnablePause();

        public void DisablePause() => _motor.DisablePause();

        public void HandleMeteorDeflect(Vector2 position, float projectileValue)
        {
            if(_mainController.GetIsInGameplay())
                _motor.HandleMeteorDeflect(position, projectileValue);
        }

        public void SetDoublePoints(bool isActive) => _motor.SetDoublePoints(isActive);

        public void NotifyBatchDeflected()
        {
            if (_mainController.GetIsInGameplay())
                _motor.NotifyBatchDeflected();
        }

        #region Stats

        public void IncreaseCollisionCount()
        {
            if(_mainController.GetIsInGameplay())
                _motor.IncreaseCollisionCount();
        }

        public void IncreaseAbilityUseCount()
        {
            if(_mainController.GetIsInGameplay())
                _motor.IncreaseAbilityUseCount();
        }
        
        public void IncreaseDeflectCount()
        {
            if(_mainController.GetIsInGameplay())
                _motor.IncreaseDeflectCount();
        }

        #endregion

        #region UI

        public void DisableGameplayUI() => _motor.DisableGameplayUI();

        public void EnableGameplayUI()
        {
            if (_mainController.GetIsInGameplay()) 
                _motor.EnableGameplayUI();

        }

        #endregion

        #endregion
    }
}