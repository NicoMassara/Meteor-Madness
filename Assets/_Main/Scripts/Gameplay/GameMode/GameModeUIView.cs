using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using _Main.Scripts.Sounds;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeUIView : MonoBehaviour, IObserver
    {
        [Header("Texts")]
        [SerializeField] private Text countdownText;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text deathScoreText;
        [Space]
        [Header("Panels")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject countdownPanel;
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private GameObject deathPanel;
        [Space]
        [Header("Buttons")]
        [SerializeField] private Button restartButton;
        [Header("Sounds")] 
        [SerializeField] private SoundBehavior buttonSound;

        private GameObject _currentPanel;
        private GameModeController _controller;

        private void Start()
        {
            restartButton.onClick.AddListener(RestartButton_OnClickHandler);
        }

        public void OnNotify(string message, params object[] args)
        {
            switch (message)
            {
                case GameModeObserverMessage.StartCountdown:
                    HandleStartCountdown();
                    break;
                case GameModeObserverMessage.UpdateCountdown:
                    HandleCountdown((float)args[0]);
                    break;
                case GameModeObserverMessage.CountdownFinish:
                    HandleCountdownFinish();
                    break;
                case GameModeObserverMessage.StartGame:
                    HandleStartGame();
                    break;
                case GameModeObserverMessage.MeteorDeflect:
                    HandleMeteorDeflect((int)args[0]);
                    break;
                case GameModeObserverMessage.EarthDeath:
                    HandleEarthDeath();
                    break;
                case GameModeObserverMessage.EarthDestruction:
                    HandleEarthDestruction((int)args[0]);
                    break;
            }
        }
        
        public void SetController(GameModeController controller)
        {
            _controller = controller;
        }

        #region Panel

        private void SetActivePanel(GameObject panel)
        {
            _currentPanel?.SetActive(false);
            _currentPanel = panel;
            _currentPanel?.SetActive(true);
        }

        private void DisableActivePanel()
        {
            _currentPanel?.SetActive(false);
            _currentPanel = null;
        }

        #endregion

        #region Start

        private void HandleStartCountdown()
        {
            DisableActivePanel();
            SetActivePanel(countdownPanel);
            
            GameManager.Instance.EventManager.Publish(new EarthRestart());
        }
        
        private void HandleCountdown(float countdownTime)
        {
            var text = countdownTime >= 1 ? $"{UITextValues.GameCountdownText} {(int)countdownTime}..." 
                : UITextValues.GameCountdownFinish;
            countdownText.text = text;
        }
        
        private void HandleCountdownFinish()
        {
            UpdatePointsText(0);
        }
        
        private void HandleStartGame()
        {
            SetActivePanel(gameplayPanel);
            
        }

        #endregion

        #region Death

        private void HandleEarthDestruction(int deflectCount)
        {
            SetActivePanel(deathPanel);
            
            var text = $"{UITextValues.Points}: {deflectCount:D6}";
            deathScoreText.text = text;
        }

        private void HandleEarthDeath()
        {
            DisableActivePanel();
        }

        #endregion

        #region Meteor

        private void HandleMeteorDeflect(int deflectCount)
        {
            UpdatePointsText(deflectCount);
        }

        #endregion

        #region Handler

        private void RestartButton_OnClickHandler()
        {
            _controller.TransitionToStart();
            buttonSound?.PlaySound();
        }

        #endregion

        private void UpdatePointsText(int points)
        {
            var text = $"{UITextValues.Points}: {points:D6}";
            scoreText.text = text;
        }

    }
}