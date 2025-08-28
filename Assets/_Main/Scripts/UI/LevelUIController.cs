using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Gameplay;
using _Main.Scripts.UI.FSM.Level;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.UI
{
    public class LevelUIController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private LevelController levelController;
        
        private LevelUIMotor _motor;

        private int _finalPoints;
        private GameObject _currentPanel;
        private FSM<LevelUIState> _fsm;
        
        public UnityAction<int> OnPointsChanged;
        public UnityAction OnDestruction;

        private enum LevelUIState
        {
            Start,
            Play,
            Death
        }

        private void Awake()
        {
            _motor = GetComponent<LevelUIMotor>();
        }

        private void Start()
        {
            levelController.OnShieldHit += OnShieldHitHandler;
            levelController.OnEnd += OnEndHandler;
            levelController.OnStart += OnStartHandler;
            levelController.OnDestruction += OnDestructionHandler;
            
            _motor.OnRestartPressed += OnRestartPressedHandler;
            GameManager.Instance.OnPaused += GM_OnPausedHandler;
            InitializeFsm();
        }

        private void Update()
        {
            _fsm.Execute();
        }
        
        public int GetDisplayedPoints()
        {
            return _motor.GetDisplayedPoints();
        }
        
        public int GetDisplayedPointsFromText()
        {
            return _motor.GetDisplayedPointsFromText();
        }

        #region FSM

        private void InitializeFsm()
        {
            var temp = new List<LevelUIBaseState<LevelUIState>>();
            _fsm = new FSM<LevelUIState>();

            #region Variables

            var start = new LevelUIStartState<LevelUIState>();
            var play = new LevelUIPlayState<LevelUIState>();
            var death = new LevelUIDeathState<LevelUIState>();
            
            temp.Add(start);
            temp.Add(play);
            temp.Add(death);

            #endregion

            #region Transitions

            start.AddTransition(LevelUIState.Play, play);
            
            play.AddTransition(LevelUIState.Death, death);
            
            death.AddTransition(LevelUIState.Start, start);

            #endregion

            foreach (var state in temp)
            {
                state.Initialize(this);
            }
            
            _fsm.SetInit(start);
            _fsm.FSMName = "Level UI";
        }

        #region Transitions

        private void SetTransition(LevelUIState state)
        {
            _fsm?.Transitions(state);
        }
        
        public void StartTransition()
        {
            SetTransition(LevelUIState.Start);
        }

        public void PlayTransition()
        {
            SetTransition(LevelUIState.Play);
        }
        
        public void DeathTransition()
        {
            SetTransition(LevelUIState.Death);
        }

        #endregion

        #endregion

        #region Update Texts

        public void UpdatePointsText(int points)
        {
            _motor.UpdatePointsText(points);
        }

        public void UpdateCountdownText(int elapsedTime)
        {
            _motor.UpdateCountdownText(elapsedTime);
        }

        public void UpdateDeathPointsText(int points)
        {
            _motor.UpdateDeathPointsText(points);
        }

        #endregion

        #region Panels

        public void StartLevel()
        {
           _motor.StartLevel();
        }

        public void SetActivePlayPanel()
        {
            _motor.SetActivePlayPanel();
        }

        public void EndLevel()
        {
            _motor.EndLevel();
        }

        public void DisableCurrentPanel()
        {
            _motor.DisableCurrentPanel();
        }
        
        public void SetActiveRestartSubPanel(bool isActive)
        {
            _motor.SetActiveRestartSubPanel(isActive);
        }

        #endregion

        #region Handlers

        private void OnShieldHitHandler(int meteorCount)
        {
            _motor.SetDisplayedPoints(meteorCount);
            OnPointsChanged?.Invoke(meteorCount);
        }
        
        private void OnEndHandler(int meteorAmount)
        {
            DeathTransition();
        }
        
        private void OnStartHandler()
        {
            StartTransition();
            _motor.RestartPoints();
        }
        
        private void OnDestructionHandler()
        {
            OnDestruction?.Invoke();
        }
        
        private void OnRestartPressedHandler()
        {
            levelController.RestartGame();
        }
        
        private void GM_OnPausedHandler(bool isPaused)
        {
            if (isPaused)
            {
                _motor.EnablePausePanel();
            }
        }

        #endregion
    }
}