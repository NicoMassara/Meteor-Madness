﻿using System;
using System.Collections;
using System.Collections.Generic;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using _Main.Scripts.Observer;
using _Main.Scripts.Sounds;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeUIView : ManagedBehavior, IObserver, IUpdatable
    {
        [Header("Texts")]
        [SerializeField] private TMP_Text countdownText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text deathScoreText;
        [SerializeField] private TMP_Text deathText;
        [Space] 
        [Header("Panels")] 
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject countdownPanel;
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private GameObject deathPanel;
        [Space]
        [Header("Buttons")]
        [SerializeField] private GameObject deathButtonContainer;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button[] mainMenuButtons;
        [Header("Sounds")] 
        [SerializeField] private SoundBehavior buttonSound;
        
        private GameObject _currentPanel;
        private NumberIncrementer _numberIncrementer;
        private ActionQueue _deathPanelActionQueue = new ActionQueue();
        private Coroutine _gameplayPointsCoroutine;
        private IGameUIConfig _gameUIConfig;
        
        public UnityAction OnMainMenuButtonPressed;
        public UnityAction OnRestartButtonPressed;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.UI;
        
        public void ManagedUpdate() { }

        private void Start()
        {
            _gameUIConfig = GameConfigManager.Instance.GetUIData();
            
            restartButton.onClick.AddListener(RestartButton_OnClickHandler);
            resumeButton.onClick.AddListener(ResumeButton_OnClickHandler);
            foreach (var button in mainMenuButtons)
            {
                button.onClick.AddListener(MainMenuButton_OnClickHandler);
            }
            deathText.text = _gameUIConfig.TextData.DeathText;
        }
        
        public void OnNotify(ulong message, params object[] args)
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
                case GameModeObserverMessage.GamePaused:
                    HandleGamePaused((bool)args[0]);
                    break;
                case GameModeObserverMessage.Disable:
                    HandleDisable();
                    break;
                case GameModeObserverMessage.Enable:
                    HandleEnable();
                    break;

            }
        }

        private void HandleEnable()
        {
            mainPanel.SetActive(true);
        }

        private void HandleDisable()
        {
            mainPanel.SetActive(false);
        }
        
        private void HandleGamePaused(bool isPaused)
        {
            var panelToActive = isPaused ? pausePanel : gameplayPanel;
            SetActivePanel(panelToActive);
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
            var text = countdownTime >= 1 ? $"{_gameUIConfig.TextData.GameCountdownText} {(int)countdownTime}..." 
                : _gameUIConfig.TextData.GameCountdownFinish;
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
                    Target = (deflectCount * GetPointsMultiplier()),
                    Current = GetCurrentPoints(),
                    TargetTime = _gameUIConfig.GameplayPointsTimeToIncrease
                
                });
                
                StartCoroutine(IncreasePointsText(UpdateGameplayScoreText));
            }
            else
            {
                _numberIncrementer.SetNewTarget(deflectCount * GetPointsMultiplier());
            }
        }

        #endregion

        #region Handler

        private void RestartButton_OnClickHandler()
        {
            buttonSound?.PlaySound();
            OnRestartButtonPressed?.Invoke();

        }
        
        private void MainMenuButton_OnClickHandler()
        {
            buttonSound?.PlaySound();
            CameraEventCaller.ZoomIn();
            OnMainMenuButtonPressed?.Invoke();
        }
        
        private void ResumeButton_OnClickHandler()
        {
            GameModeEventCaller.SetPause(false);
        }


        #endregion

        #region ScoreTextUpdate

        private void UpdateGameplayScoreText(int points)
        {
            var text = $"{_gameUIConfig.TextData.Points}: {points:D6}";
            scoreText.text = text;
        }

        private void UpdateDeathScoreText(int points)
        {
            var text = $"{_gameUIConfig.TextData.DeathPoints}: {points:D6}";
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
            SetActiveRestartButtonPanel(false);
            UpdateDeathScoreText(0);
            SetActivePanel(deathPanel);

            var deathPanelData = _gameUIConfig.DeathUITimeData;
            

            List<ActionData> tempList = new List<ActionData>
            {
                new (null, 
                    deathPanelData.ShowDeathUI),
                new (()=> SetActiveDeathText(true), 
                    deathPanelData.SetEnableDeathText),
                new (()=> SetActiveDeathScoreText(true), 
                    deathPanelData.SetEnableDeathScore)
            };
            
            if (deflectCount > 0)
            {
                _numberIncrementer.SetData(new NumberIncrementerData
                {
                    Target = deflectCount * GetPointsMultiplier(),
                    TargetTime = deathPanelData.DeathPointsTimeToIncrease,
                    ActionOnFinish = ()=>
                    {
                        _deathPanelActionQueue.AddAction(
                            new ActionData(
                                () => SetActiveRestartButtonPanel(true),
                                deathPanelData.EnableRestartButton));
                    }
                });
                
                tempList.Add(
                    new ActionData(
                        ()=> StartCoroutine(IncreasePointsText(UpdateDeathScoreText)),
                        deathPanelData.CountDeathScore));
                ;
            }
            else
            {
                tempList.Add(
                    new ActionData(
                        ()=> SetActiveRestartButtonPanel(true),
                        deathPanelData.EnableRestartButton));
            }

            
            _deathPanelActionQueue.AddAction(tempList);
            
            ActionManager.Add(_deathPanelActionQueue,SelfUpdateGroup);
        }

        private void SetActiveDeathText(bool isActive)
        {
            deathText.gameObject.SetActive(isActive);
        }

        private void SetActiveDeathScoreText(bool isActive)
        {
            deathScoreText.gameObject.SetActive(isActive);
        }

        private void SetActiveRestartButtonPanel(bool isActive)
        {
            deathButtonContainer.gameObject.SetActive(isActive);
        }
        

        #endregion
        
        private int GetCurrentPoints()
        {
            return (int)_numberIncrementer.CurrentValue;
        }

        private int GetPointsMultiplier()
        {
            return GameConfigManager.Instance.GetGameplayData().PointsMultiplier;
        }
    }
}