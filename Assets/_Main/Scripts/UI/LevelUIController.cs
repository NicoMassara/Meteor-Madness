
using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Gameplay;
using _Main.Scripts.UI.FSM.Level;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Main.Scripts.UI
{
    public class LevelUIController : MonoBehaviour
    {
        [FormerlySerializedAs("levelControllers")]
        [Header("UI Elements")]
        [SerializeField] private LevelController levelController;
        [SerializeField] private GameObject startPanel;
        [SerializeField] private GameObject pointsPanel;
        [SerializeField] private GameObject deathPanel;
        [Header("Texts")]
        [SerializeField] private Text countdownText;
        [SerializeField] private Text playPointsText;
        [SerializeField] private Text deathPointsText;

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

        private void Start()
        {
            levelController.OnShieldHit += OnShieldHitHandler;
            levelController.OnDeath += OnDeathHandler;
            levelController.OnStart += OnStartHandler;
            
            pointsPanel.SetActive(false);
            deathPanel.SetActive(false);
            startPanel.SetActive(false);
            
            InitializeFsm();
        }

        private void Update()
        {
            _fsm.Execute();
        }

        #region FSM

        private void InitializeFsm()
        {
            var temp = new List<LevelUIBaseState<LevelUIState>>();
            _fsm = new FSM<LevelUIState>();

            #region Variables

            var start = new LevelUIStartState<LevelUIState>(startPanel);
            var play = new LevelUIPlayState<LevelUIState>(pointsPanel);
            var death = new LevelUIDeathState<LevelUIState>(deathPanel);
            
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
            playPointsText.text = $"Save Points: {points}";
        }

        public void UpdateCountdownText(int elapsedTime)
        {
            if (elapsedTime > 0)
            {
                countdownText.text = $"{elapsedTime}...";
            }
            else
            {
                countdownText.text = "Defend!";
            }


        }

        public void UpdateDeathPointsText(int points)
        {
            deathPointsText.text = $"Points: {points}";
        }

        public int GetCurrentDisplayed()
        {
            // Reads the number currently shown in the text
            if (int.TryParse(playPointsText.text, out int val))
                return val;
            return 0;
        }

        #endregion

        public void ChangeCurrentPanel(GameObject panel)
        {
            if (_currentPanel != null)
            {
                _currentPanel.SetActive(false);
            }
            
            _currentPanel = panel;
            _currentPanel.SetActive(true);
        }

        public int GetFinalPoints()
        {
            return _finalPoints;
        }

        #region Handlers

        private void OnShieldHitHandler(int meteorCount)
        {
            OnPointsChanged?.Invoke(meteorCount);
        }
        
        private void OnDeathHandler(int meteorAmount)
        {
            Debug.Log(meteorAmount);
            _finalPoints = meteorAmount * GameValues.VisualMultiplier;
            DeathTransition();
        }
        
        private void OnStartHandler()
        {
            StartTransition();
        }

        #endregion
    }
}