using System;
using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Gameplay.FSM.GameMode;
using _Main.Scripts.Managers;
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
            Death
        }

        public GameModeController(GameModeMotor motor)
        {
            _motor = motor;
        }

        public void Initialize()
        {
            InitializeFsm();
        }

        public void Execute()
        {
            _fsm?.Execute();
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
            
            temp.Add(start);
            temp.Add(gameplay);
            temp.Add(finish);
            temp.Add(death);

            #endregion

            #region Transitions

            start.AddTransition(States.Gameplay, gameplay);
            
            gameplay.AddTransition(States.Finish, finish);
            
            finish.AddTransition(States.Death, death);
            
            death.AddTransition(States.Start, start);
            

            #endregion

            foreach (var state in temp)
            {
                state.Initialize(this);
            }
            
            _fsm.SetInit(start);
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

        #endregion

        #endregion

        #region Level 

        public void StartCountdown()
        {
            _motor.StartCountdown(GameTimeValues.StartGameCount);
        }

        public void StartGame()
        {
            _motor.StartGame();
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
        
        public void HandleMeteorDeflect()
        {
            _motor.HandleMeteorDeflect();
        }


        public void SpawnSingleMeteor()
        {
            _motor.SpawnSingleMeteor();
        }

        public void HandleGameFinish()
        {
            _motor.HandleGameFinish();
        }
    }
}