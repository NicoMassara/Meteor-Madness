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
            levelController.OnDeath += OnDeathHandler;
            levelController.OnStart += OnStartHandler;
            
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

        public void SetActiveCountdownPanel()
        {
           _motor.SetActiveCountdownPanel();
        }

        public void SetActivePlayPanel()
        {
            _motor.SetActivePlayPanel();
        }

        public void SetActiveDeathPanel()
        {
            _motor.SetActiveDeathPanel();
        }

        #endregion

        #region Handlers

        private void OnShieldHitHandler(int meteorCount)
        {
            _motor.SetDisplayedPoints(meteorCount);
            OnPointsChanged?.Invoke(meteorCount);
        }
        
        private void OnDeathHandler(int meteorAmount)
        {
            DeathTransition();
        }
        
        private void OnStartHandler()
        {
            StartTransition();
            _motor.RestartPoints();
        }

        #endregion
    }
}