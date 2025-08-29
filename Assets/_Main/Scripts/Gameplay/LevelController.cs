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
        public UnityAction<int> OnEarthHit;
        public UnityAction<int> OnEnd;
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
            _motor.OnEarthHit += OnEarthHitHandler;
            _motor.OnEnd += OnEndHandler;
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

        public void TriggerEarthDestruction()
        {
            OnDestruction?.Invoke();
            _motor.TriggerDestruction();
        }

        #region Motor Functions
        
        public void StartLevel()
        {
            _motor.StartLevel();
            GameManager.Instance.ClearCurrentPoints();
        }

        public void StartEndLevel()
        {
            _motor.StartEndLevel();
        }
        
        public void FinishEndLevel()
        {
            _motor.FinishEndLevel();
        }

        public void ZoomIn()
        {
            _motor.ZoomIn();
        }

        public void SpawnMeteor()
        {
            _motor.SpawnMeteor();
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

        #endregion

        #region Handlers

        private void OnStartHandler()
        {
            OnStart?.Invoke();
        }

        private void OnEndHandler(int meteorHitCount)
        {
            OnEnd?.Invoke(meteorHitCount);
        }

        private void OnShieldHitHandler(int meteorAmount)
        {
            GameManager.Instance.IncreasePoints();
            OnShieldHit?.Invoke(GameManager.Instance.GetCurrentPoints());
        }
        
        private void GM_OnPausedHandler(bool isPaused)
        {
            _motor.SetPaused(isPaused);
            _isPaused = isPaused;
        }
        
        private void OnEarthHitHandler(int arg0)
        {
            OnEarthHit?.Invoke(arg0);
        }

        #endregion

        public void StartEarthShake()
        {
            _motor.StartEarthShake();
        }

        public void StopEarthShake()
        {
            _motor.StopEarthShake();
        }
    }
}