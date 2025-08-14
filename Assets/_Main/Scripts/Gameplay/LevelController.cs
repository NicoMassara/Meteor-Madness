using System;
using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Gameplay.FSM.Level;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay
{
    public class LevelController : MonoBehaviour
    {
        private LevelMotor _levelMotor;
        private FSM<LevelState> _fsm;
        
        public UnityAction<int> OnShieldHit;
        public UnityAction<int> OnDeath;
        public UnityAction OnStart;

        public enum LevelState
        {
            Start,
            Play,
            Death
        }

        private void Awake()
        {
            _levelMotor = GetComponent<LevelMotor>();
        }

        private void Start()
        {
            _levelMotor.OnShieldHit += OnShieldHitHandler;
            _levelMotor.OnDeath += OnDeathHandler;
            _levelMotor.OnStart += OnStartHandler;
            
            InitializeFsm();
        }

        private void Update()
        {
            _fsm.Execute();
        }

        #region FSM

        private void InitializeFsm()
        {
            var temp = new List<LevelBaseState<LevelState>>();
            _fsm = new FSM<LevelState>();

            #region Variables

            var start = new LevelStartState<LevelState>();
            var play = new LevelPlayState<LevelState>();
            var death = new LevelDeathState<LevelState>();
            
            temp.Add(start);
            temp.Add(play);
            temp.Add(death);

            #endregion

            #region Transitions

            start.AddTransition(LevelState.Play, play);
            
            play.AddTransition(LevelState.Death, death);
            
            death.AddTransition(LevelState.Start, start);

            #endregion

            foreach (var state in temp)
            {
                state.Initialize(this);
            }
            
            _fsm.SetInit(start);
            _fsm.FSMName = "Level";
        }

        #region Transitions

        private void SetTransition(LevelState state)
        {
            _fsm?.Transitions(state);
        }
        
        public void StartTransition()
        {
            SetTransition(LevelState.Start);
        }

        public void PlayTransition()
        {
            SetTransition(LevelState.Play);
        }
        
        public void DeathTransition()
        {
            SetTransition(LevelState.Death);
        }

        #endregion

        #endregion

        public void TriggerStart()
        {
            OnStart?.Invoke();
        }

        #region Motor Functions

        public void RunSpawnTimer()
        {
           _levelMotor.RunSpawnTimer();
        }

        public void SpawnMeteor()
        {
            _levelMotor.SpawnMeteor();
        }

        public void RestartSpawnTimer()
        {
            _levelMotor.RestartSpawnTimer();
        }

        public void RestartLevel()
        {
            _levelMotor.RestartLevel();
        }

        #endregion
        
        #region Motor Data

        public bool HasSpawnTimerEnd()
        {
            return _levelMotor.HasSpawnTimerEnd();
        }

        #endregion

        #region Handlers

        private void OnStartHandler()
        {
            OnStart?.Invoke();
        }

        private void OnDeathHandler(int meteorHitCount)
        {
            OnDeath?.Invoke(meteorHitCount);
        }

        private void OnShieldHitHandler(int meteorAmount)
        {
            OnShieldHit?.Invoke(meteorAmount);
        }

        #endregion
    }
}