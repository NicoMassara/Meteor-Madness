using System;
using System.Collections;
using System.Collections.Generic;
using _Main.Scripts.Managers.UpdateManager;
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
        private NumberIncrementer _numberIncrementer;
        private ActionQueue _deathPanelActionQueue = new ActionQueue();
        private Coroutine _gameplayPointsCoroutine;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.UI;
        
        public void ManagedUpdate()
        {

        }

        private void Start()
        {
            restartButton.onClick.AddListener(RestartButton_OnClickHandler);
            deathText.text = UITextValues.DeathText;
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
                case GameModeObserverMessage.StartGameplay:
                    HandleStartGameplay();
                    break;
                case GameModeObserverMessage.MeteorDeflect:
                    HandleMeteorDeflect((float)args[0]);
                    break;
                case GameModeObserverMessage.EarthStartDestruction:
                    HandleEarthStartDestruction();
                    break;
                case GameModeObserverMessage.EarthEndDestruction:
                    HandleEarthEndDestruction((float)args[0]);
                    break;
                case GameModeObserverMessage.GameFinish:
                    HandleGameFinish();
                    break;
                case GameModeObserverMessage.GameRestart:
                    HandleGameRestart();
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

        public void DisableActivePanel()
        {
            _currentPanel?.SetActive(false);
            _currentPanel = null;
        }

        #endregion

        #region Start

        private void HandleStartCountdown()
        {
            DisableActivePanel();
            _numberIncrementer = new NumberIncrementer();
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
            _numberIncrementer?.ResetValues();
            UpdateGameplayScoreText(0);
        }
        
        private void HandleStartGameplay()
        {
            SetActivePanel(gameplayPanel);
        }

        #endregion

        #region Death

        private void HandleGameFinish()
        {
            DisableActivePanel();
        }
        
        private void HandleGameRestart()
        {
            DisableActivePanel();
        }
        
        private void HandleEarthEndDestruction(float deflectCount)
        {
            StartDeathPanelActionQueue(deflectCount);
        }

        private void HandleEarthStartDestruction()
        {
            DisableActivePanel();
        }

        #endregion

        #region Meteor

        private void HandleMeteorDeflect(float deflectCount)
        {
            if (_numberIncrementer.IsFinished)
            {
                _numberIncrementer.SetData(new NumberIncrementerData
                {
                    Target = (deflectCount * GameValues.VisualMultiplier),
                    Current = GetCurrentPoints(),
                    TargetTime = UIPanelTimeValues.GameplayPointsTimeToIncrease
                
                });
                
                StartCoroutine(IncreasePointsText(UpdateGameplayScoreText));
            }
            else
            {
                _numberIncrementer.SetNewTarget(deflectCount * GameValues.VisualMultiplier);
            }
        }

        #endregion

        #region Handler

        private void RestartButton_OnClickHandler()
        {
            buttonSound?.PlaySound();
            _controller.TransitionToRestart();
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
                if (!CustomTime.GetChannel(SelfUpdateGroup).IsPaused)
                {
                    _numberIncrementer.Run(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
                    increaseAction?.Invoke(GetCurrentPoints());
                }
                
                yield return null;
            }
        }

        #endregion

        #region Death Panel Queue Actions

        private void StartDeathPanelActionQueue(float deflectCount)
        {
            SetActiveDeathText(false);
            SetActiveDeathScoreText(false);
            SetActiveRestartButton(false);
            UpdateDeathScoreText(0);
            SetActivePanel(deathPanel);


            List<ActionData> tempList = new List<ActionData>
            {
                new (()=> SetActiveDeathText(true), 
                    UIPanelTimeValues.SetEnableDeathText),
                new (()=> SetActiveDeathScoreText(true), 
                    UIPanelTimeValues.SetEnableDeathScore)
            };
            
            if (deflectCount > 0)
            {
                _numberIncrementer.SetData(new NumberIncrementerData
                {
                    Target = deflectCount * GameValues.VisualMultiplier,
                    TargetTime = UIPanelTimeValues.DeathPointsTimeToIncrease,
                    ActionOnFinish = ()=>
                    {
                        _deathPanelActionQueue.AddAction(
                            new ActionData(
                                () => SetActiveRestartButton(true),
                                UIPanelTimeValues.EnableRestartButton));
                    }
                });
                
                tempList.Add(
                    new ActionData(
                        ()=> StartCoroutine(IncreasePointsText(UpdateDeathScoreText)),
                        UIPanelTimeValues.CountDeathScore));
                ;
            }
            else
            {
                tempList.Add(
                    new ActionData(
                        ()=> SetActiveRestartButton(true),
                        UIPanelTimeValues.EnableRestartButton));
            }

            
            _deathPanelActionQueue.AddAction(tempList);
            StartCoroutine(RunDeathPanelActionQueue());
        }

        private IEnumerator RunDeathPanelActionQueue()
        {
            while (!_deathPanelActionQueue.IsEmpty)
            {
                var dt = CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup);
                
                _deathPanelActionQueue.Run(dt);

                yield return null;
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