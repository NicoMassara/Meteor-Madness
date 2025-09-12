using System;
using System.Collections;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Managers.UpdateManager.Interfaces;
using _Main.Scripts.MyCustoms;
using _Main.Scripts.Observer;
using _Main.Scripts.Sounds;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeUIView : ManagedBehavior, IObserver, IUpdatable
    {
        [Header("Texts")]
        [SerializeField] private Text countdownText;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text deathScoreText;
        [SerializeField] private Text deathText;
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
        private readonly NumberIncrementer _numberIncrementer = new NumberIncrementer();
        private ActionQueue _deathPanelActionQueue = new ActionQueue();
        
        public UpdateManager.UpdateGroup UpdateGroup { get; } = UpdateManager.UpdateGroup.UI;

        private void Start()
        {
            restartButton.onClick.AddListener(RestartButton_OnClickHandler);
            deathText.text = UITextValues.DeathText;
        }
        
        public void ManagedUpdate()
        {
            _deathPanelActionQueue.Run(CustomTime.DeltaTime);
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
                case GameModeObserverMessage.EarthStartDestruction:
                    HandleEarthStartDestruction();
                    break;
                case GameModeObserverMessage.EarthEndDestruction:
                    HandleEarthEndDestruction((int)args[0]);
                    break;
                case GameModeObserverMessage.GameFinish:
                    HandleGameFinish();
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
        }
        
        private void HandleCountdown(float countdownTime)
        {
            var text = countdownTime >= 1 ? $"{UITextValues.GameCountdownText} {(int)countdownTime}..." 
                : UITextValues.GameCountdownFinish;
            countdownText.text = text;
        }
        
        private void HandleCountdownFinish()
        {
            UpdateGameplayScoreText(0);
        }
        
        private void HandleStartGame()
        {
            SetActivePanel(gameplayPanel);
        }

        #endregion

        #region Death

        private void HandleGameFinish()
        {
            DisableActivePanel();
        }
        
        private void HandleEarthEndDestruction(int deflectCount)
        {
            StartDeathPanelActionQueue(deflectCount);
        }

        private void HandleEarthStartDestruction()
        {
            DisableActivePanel();
        }

        #endregion

        #region Meteor

        private void HandleMeteorDeflect(int deflectCount)
        {
            _numberIncrementer.SetData(new NumberIncrementerData
            {
                Target = deflectCount * GameValues.VisualMultiplier,
                Current = GetCurrentPoints(),
                TargetTime = UIPanelTimeValues.GameplayPointsTimeToIncrease
                
            });
            
            StartCoroutine(IncreasePointsText(UpdateGameplayScoreText));
        }

        #endregion

        #region Handler

        private void RestartButton_OnClickHandler()
        {
            _controller.TransitionToStart();
            buttonSound?.PlaySound();
        }

        #endregion

        #region ScoreTextUpdate

        private void UpdateGameplayScoreText(int points)
        {
            var text = $"{UITextValues.Points}: {points:D6}";
            scoreText.text = text;
        }

        private void UpdateDeathScoreText(int points)
        {
            var text = $"{UITextValues.DeathPoints}: {points:D6}";
            deathScoreText.text = text;
        }

        private IEnumerator IncreasePointsText(Action<int> increaseAction = null)
        {
            while (!_numberIncrementer.IsFinished)
            {
                _numberIncrementer.Run(CustomTime.DeltaTime);
                increaseAction?.Invoke(GetCurrentPoints());
                
                yield return null;
            }
        }

        #endregion

        #region Death Panel Queue Actions

        private void StartDeathPanelActionQueue(int deflectCount)
        {
            SetActiveDeathText(false);
            SetActiveDeathScoreText(false);
            SetActiveRestartButton(false);
            UpdateDeathScoreText(0);
            SetActivePanel(deathPanel);
            
            _deathPanelActionQueue.AddAction(
                new ActionData(()=> SetActiveDeathText(true), UIPanelTimeValues.SetEnableDeathText));
            _deathPanelActionQueue.AddAction(
                new ActionData(()=> SetActiveDeathScoreText(true), UIPanelTimeValues.SetEnableDeathScore));

            if (deflectCount > 0)
            {
                _numberIncrementer.SetData(new NumberIncrementerData
                {
                    Target = deflectCount * GameValues.VisualMultiplier,
                    TargetTime = UIPanelTimeValues.DeathPointsTimeToIncrease,
                    ActionOnFinish = ()=> _deathPanelActionQueue.AddAction(
                        new ActionData(()=> SetActiveRestartButton(true),UIPanelTimeValues.EnableRestartButton))
                
                });
                
                _deathPanelActionQueue.AddAction(
                    new ActionData(()=> StartCoroutine(
                        IncreasePointsText(UpdateDeathScoreText)),UIPanelTimeValues.CountDeathScore));
                ;
            }
            else
            {
                _deathPanelActionQueue.AddAction(
                    new ActionData(()=> SetActiveRestartButton(true),UIPanelTimeValues.EnableRestartButton));
            }
            
            
        }

        private void SetActiveDeathText(bool isActive)
        {
            deathText.gameObject.SetActive(isActive);
        }

        private void SetActiveDeathScoreText(bool isActive)
        {
            deathScoreText.gameObject.SetActive(isActive);
        }

        private void SetActiveRestartButton(bool isActive)
        {
            restartButton.gameObject.SetActive(isActive);
        }

        #endregion
        
        
        private int GetCurrentPoints()
        {
            return (int)_numberIncrementer.CurrentValue;
        }
    }
}