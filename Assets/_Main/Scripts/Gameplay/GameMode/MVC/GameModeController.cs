using System;
using System.Collections.Generic;
using System.Timers;
using _Main.Scripts.FiniteStateMachine;
using NicolasMassara.CustomActionManager;
using NicolasMassara.CustomTimerManager;
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
            Pause,
            Leaving,
            Disable,
            Countdown,
            // Asleep -> Is used when is in Pause to change screens and return to Pause without going to Disable
            Asleep 
        }

        private class GameModeActionGate : FsmActionGate<States>
        {
            public GameModeActionGate(FSM<States> fsm) : base(fsm) { }
            
            public bool IsInGameplay { get; private set; }
            public bool CanPause { get; private set; }
            public bool CanDisableSpawn { get; private set; }
            public bool CanEnableSpawn { get; private set; }
            public bool IsAsleep { get; private set; }
            public bool IsGoingToSleep { get; set; }

            public bool WasAsleep { get; private set; }
            public bool DoesResume { get; private set; }

            protected override void OnNewState(States state)
            {
                //
                CanPause = state is States.Pause 
                           && CurrentState is States.Gameplay;
                //
                CanDisableSpawn = state is States.Leaving or States.Finish &&
                                  (CurrentState is States.Gameplay or States.Pause);
                //
                WasAsleep = LastState is States.Asleep;
            }

            protected override void OnEnterState(States state)
            {
                IsInGameplay = state is States.Gameplay;
                //
                CanEnableSpawn = state is States.Enable;
                //
                IsAsleep = state is States.Asleep;
                //
                DoesResume = state is States.Countdown 
                             && LastState is States.Pause;

            }

            protected override void OnExitState(States state)
            {
                WasAsleep = state is States.Asleep;
            }
        }
        
        private GameModeActionGate _actionGate;

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
            _actionGate = new GameModeActionGate(_fsm);
            
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
            var leaving = new LeavingState<States>();
            var pause = new PauseState<States>(()=> _actionGate.WasAsleep);
            var disable = new DisableState<States>();
            var asleep = new AsleepState<States>();
            var countDown = new CountdownState<States>();
            
            temp.Add(none);
            temp.Add(enable);
            temp.Add(start);
            temp.Add(gameplay);
            temp.Add(finish);
            temp.Add(death);
            temp.Add(restart);
            temp.Add(leaving);
            temp.Add(disable);
            temp.Add(pause);
            temp.Add(asleep);
            temp.Add(countDown);

            #endregion

            #region Transitions
            
            none.AddTransition(States.Enable, enable);
            //
            enable.AddTransition(States.Start, start);
            
            start.AddTransition(States.Countdown, countDown);
            
            countDown.AddTransition(States.Gameplay, gameplay);
            
            gameplay.AddTransition(States.Finish, finish);
            gameplay.AddTransition(States.Pause, pause);
            
            pause.AddTransition(States.Countdown, countDown);
            pause.AddTransition(States.Leaving, leaving);
            pause.AddTransition(States.Asleep, asleep);
            asleep.AddTransition(States.Pause, pause);
            
            finish.AddTransition(States.Death, death);
            
            death.AddTransition(States.Restart, restart);
            death.AddTransition(States.Disable, disable);
            death.AddTransition(States.Leaving, leaving);
            
            restart.AddTransition(States.Start, start);
            
            leaving.AddTransition(States.Disable, disable);
            
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
            if (_actionGate.IsAsleep)
            {
                SetTransition(States.Pause);
            }
            else
            {
                SetTransition(States.Enable);
            }
        }
        
        public void TransitionToDisable()
        {
            if (_actionGate.IsGoingToSleep)
            {
                SetTransition(States.Asleep);
            }
            else
            {
                SetTransition(States.Disable);
            }
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
        
        public void TransitionToCountDown()
        {
            SetTransition(States.Countdown);
        }
        
        public void TransitionToRestart()
        {
            SetTransition(States.Restart);
        }
        
        public void TransitionToLeaving()
        {
            SetTransition(States.Leaving);
        }
        
        public void TransitionToPause()
        {
            SetTransition(States.Pause);
        }

        #endregion

        #endregion
        
        #region Motor
        
        #region Level 
        
        public void SetDoesRestartGameMode(bool doesRestart)
        {
            _motor.SetDoesRestartGameMode(doesRestart);
        }

        public void SetDoublePoints(bool isEnable)
        {
            if(_actionGate.IsInGameplay == false) return;
            
            _motor.SetDoublePoints(isEnable);
        }
        
        public void SetEnable()
        {
            _motor.Enable();
        }

        public void SetCanPause(bool canPause)
        {
            _motor.SetCanPause(canPause);
        }

        public void StartCountdown()
        {
            _motor.StartCountdown();
        }

        public void StartGameplay()
        {
            if (_actionGate.DoesResume == false)
            {
                _motor.StartGameplay();
            }
        }

        public void RestartValues()
        {
            _motor.RestartValues();
        }

        public void SetHighScore(float highScore)
        {
            _motor.SetHighScore(highScore);
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

        public void Pause()
        {
            if (_actionGate.CanPause == false)
            {
                //Debug.Log("Cannot pause game");
                return;
            }
            
            _motor.PauseGame();
        }
        
        public void UnPauseGame()
        {
            if(_actionGate.DoesResume == false)
            {
                //Debug.Log("Cannot unpause game");
                return;
            }
            
            _motor.UnPauseGame();
        }
        
        public void DisableGameMode()
        {
            _motor.DisableGameMode();
        }

        private void InitializeValues()
        {
            _motor.InitializeValues();
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
        
        public void EarthRestartFinish()
        {
            _motor.EarthRestartFinish();
        }

        #endregion

        #region Projectile
        
        public void GrantProjectileSpawn(int projectileTypeIndex)
        {
            if(_actionGate.IsInGameplay == false) return;
            
            _motor.GrantSpawnMeteor(projectileTypeIndex);
        }

        public void HandleProjectileDeflect(Vector2 position, float meteorDeflectValue)
        {
            if(_actionGate.IsInGameplay == false) 
                return;
            
            _motor.HandleMeteorDeflect(position, meteorDeflectValue);
        }

        public void EnableMeteorSpawn()
        {
            if(_actionGate.CanEnableSpawn == false)
            {
                Debug.Log("Cannot enable spawn");
                return;
            }

            _motor.SetEnableMeteorSpawn(true);
        }

        public void DisableMeteorSpawn()
        {
            if(_actionGate.CanDisableSpawn == false)
            {
                return;
            }

            _motor.SetEnableMeteorSpawn(false);
        }

        #endregion
        
        #region Camera

        public void HandleCameraZoomOut()
        {
            if(_actionGate.IsInGameplay == false) 
                return;
            
            _motor.HandleCameraZoomOut();
        }

        public void HandleCameraZoomIn()
        {
            if(_actionGate.IsInGameplay == false) 
                return;
            
            _motor.HandleCameraZoomIn();
        }

        #endregion

        #endregion

        #region Screens

        public void TriggerMainMenu()
        {
            _motor.TriggerMainMenu();
        }
        
        public void SetGameplayPanel(bool isActive)
        {
            _motor.SetGameplayPanel(isActive);
        }

        public void SetPausePanel(bool isActive)
        {
            _motor.SetPausePanel(isActive);
        }
        
        public void TriggerOptions()
        {
            _motor.TriggerOptions();
        }

        #endregion

        public void Asleep()
        {
            _motor.Asleep();
        }

        public void Leaving()
        {
            _motor.Leaving();
        }

        public void SetToSleep()
        {
            _actionGate.IsGoingToSleep = true;
        }

        public void WakeUp()
        {
            _actionGate.IsGoingToSleep = false;
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
        public override void Awake()
        {
            Controller.HandleEarthEndDestruction();
        }
    }
    
    public class FinishState<T> : BaseState<T>
    {
        
        public override void Awake()
        {
            Controller.HandleGameFinish();
            Controller.HandleEarthStartDestruction();
        }
        
    }
    
    public class GameplayState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.EnableMeteorSpawn();
            Controller.SetGameplayPanel(true);
        }

        public override void Sleep()
        {
            Controller.SetGameplayPanel(false);
            Controller.DisableMeteorSpawn();
        }
    }
    
    public class RestartState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.GameRestart();
        }
    }
    
    public class PauseState<T> : BaseState<T>
    {
        private IQueueAction _actionQueue;
        private readonly Func<bool> _getWasAsleep;

        public PauseState(Func<bool> getWasAsleep)
        {
            _getWasAsleep = getWasAsleep;
        }

        public override void Awake()
        {
            var actions = ActionBuilder.Start()
                .Do(new InstantAction(() =>
                {
                    Controller.Pause();
                    Controller.WakeUp();
                }))
                .Then(new WaitSecondsAction(0.25f))
                .WrapLast(a=> new ConditionalWrapperAction(a,
                    () => _getWasAsleep?.Invoke() == false))
                .Then(new SetBoolAction(true,Controller.SetPausePanel))
                .Build();

            _actionQueue = actions;
        }

        public override void Execute(float deltaTime)
        {
            if (_actionQueue.CurrentStatus == ActionStatus.Running)
            {
                _actionQueue?.OnUpdate(deltaTime);
            }
        }

        public override void Sleep()
        {
            Controller.SetPausePanel(false);
        }
    }
    
    public class LeavingState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.DisableMeteorSpawn();
            Controller.Leaving();
        }
    }
    
    public class StartState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.RestartValues();
            Controller.TransitionToCountDown();
        }
    }
    
    public class CountdownState<T> : BaseState<T>
    {
        private bool _enableCountdown;
        
        public override void Awake()
        {
            TimerManager.Add(new TimerData(0.25f, 
                ()=> _enableCountdown = false,
                ()=> _enableCountdown = true));
            
            Controller.StartCountdown();
        }
        
        public override void Execute(float deltaTime)
        {
            if (_enableCountdown == false) return;
            
            Controller.HandleCountdownTimer(deltaTime);
        }
        
        public override void Sleep()
        {
            Controller.StartGameplay();
            Controller.UnPauseGame();
        }
    }
    
    public class AsleepState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.Asleep();
        }
    }

    #endregion
}