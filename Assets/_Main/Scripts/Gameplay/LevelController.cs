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
        private LevelMotor _motor;
        private FSM<LevelState> _fsm;
        
        public UnityAction<int> OnShieldHit;
        public UnityAction<int> OnDeath;
        public UnityAction OnStart;
        public UnityAction OnDestruction;
        
        private bool _isPaused;

        private enum LevelState
        {
            Start,
            Play,
            Death
        }

        private void Awake()
        {
            _motor = GetComponent<LevelMotor>();
        }

        private void Start()
        {
            _motor.OnShieldHit += OnShieldHitHandler;
            _motor.OnDeath += OnDeathHandler;
            _motor.OnStart += OnStartHandler;

            GameManager.Instance.OnPaused += GM_OnPausedHandler;
            InitializeFsm();
        }

        private void Update()
        {
            if (_isPaused == false)
            {
                _fsm.Execute();     
            }
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

        public void TriggerEarthDestruction()
        {
            OnDestruction?.Invoke();
            _motor.TriggerDestruction();
        }

        #region Motor Functions

        public void RunSpawnTimer()
        {
           _motor.RunSpawnTimer();
        }

        public void SpawnMeteor()
        {
            _motor.SpawnMeteor();
        }

        public void RestartSpawnTimer()
        {
            _motor.RestartSpawnTimer();
        }

        public void RestartLevel()
        {
            _motor.RestartLevel();
        }

        public void RestartGame()
        {
            StartTransition();
        }

        #endregion
        
        #region Motor Data

        public bool HasSpawnTimerEnd()
        {
            return _motor.HasSpawnTimerEnd();
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
            GameManager.Instance.IncreasePoints();
            OnShieldHit?.Invoke(GameManager.Instance.GetCurrentPoints());
        }
        
        private void GM_OnPausedHandler(bool isPaused)
        {
            _isPaused = isPaused;
        }

        #endregion
    }
}