using System;
using System.Collections;
using System.Collections.Generic;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Localization;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeUIView : ManagedBehavior, IObserver
    {
        [SerializeField] private GameModeUiPanelSelector uiSelector;
        
        private GameModeUIComponents _uiComponents;
        private GameObject _currentPanel;
        private NumberIncrementer _numberIncrementer;
        private ActionQueue _deathPanelActionQueue = new ActionQueue();
        private Coroutine _gameplayPointsCoroutine;
        private IGameUIConfig _gameUIConfig;
        
        private bool _hasHighScore;
        private float _highScore;
        
        public event Action OnMainMenuButtonPressed;
        public event Action OnRestartButtonPressed;
        public event Action OnPauseButtonPressed;
        
        
        private void Start()
        {
            _gameUIConfig = GameConfigManager.Instance.GetUIData();
            
            GetUiComponents().RestartButton.onClick.AddListener(RestartButton_OnClickHandler);
            GetUiComponents().ResumeButton.onClick.AddListener(ResumeButton_OnClickHandler);
            GetUiComponents().PauseButton.onClick.AddListener(PauseButton_OnClickHandler);
            foreach (var button in GetUiComponents().MainMenuButtons)
            {
                button.onClick.AddListener(MainMenuButton_OnClickHandler);
            }

            GetUiComponents().DeathText.text = GetLocalizedString("Gameplay.Death.Title");
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
                case GameModeObserverMessage.CameraZoomOut:
                    HandleCameraZoomOut();
                    break;
                case GameModeObserverMessage.CameraZoomIn:
                    HandleCameraZoomIn();
                    break;
                case GameModeObserverMessage.SetCanPause:
                    HandleSetCanPause((bool)args[0]);
                    break;
                case GameModeObserverMessage.TriggerMainMenu:
                    HandleTriggerMainMenu();
                    break;
                case GameModeObserverMessage.SetHasHighScore:
                    HandleSetHighScore((bool)args[0],(float)args[1]);
                    break;
            }
        }

        private void HandleSetHighScore(bool hasNewHighScore, float highScore)
        {
            _highScore = highScore;
            _hasHighScore = hasNewHighScore;
        }

        private void HandleTriggerMainMenu()
        {
            DisableActivePanel();
        }

        private void HandleSetCanPause(bool canPause)
        {
            GetUiComponents().PauseButton.interactable = canPause;
        }
        
        private GameModeUIComponents GetUiComponents()
        {
            return _uiComponents ??= _uiComponents = uiSelector.GetPanelData();
        }
        
        private void HandleCameraZoomOut()
        {
            GetUiComponents().GameplayPanel.SetActive(true);
        }
        
        private void HandleCameraZoomIn()
        {
            GetUiComponents().GameplayPanel.SetActive(false);
        }

        private void HandleEnable()
        {
            GetUiComponents().MainPanel.SetActive(true);
        }

        private void HandleDisable()
        {
            GetUiComponents().MainPanel.SetActive(false);
        }
        
        private void HandleGamePaused(bool isPaused)
        {
            var panelToActive = isPaused ? GetUiComponents().PausePanel : GetUiComponents().GameplayPanel;
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
            _hasHighScore = false;
            _numberIncrementer = new NumberIncrementer();
            SetActivePanel(GetUiComponents().CountdownPanel);
        }
        
        private void HandleCountdown(float countdownTime)
        {
            var text = countdownTime >= 1 ? $"{GetLocalizedString("Gameplay.CountDownStart")} {(int)countdownTime}..." 
                : GetLocalizedString("Gameplay.CountdownFinish");
            GetUiComponents().CountdownText.text = text;
        }
        
        private void HandleCountdownFinish()
        {
            _numberIncrementer?.ResetValues();
            UpdateGameplayScoreText(0);
        }
        
        private void HandleStartGameplay()
        {
            SetActivePanel(GetUiComponents().GameplayPanel);
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
            SoundEventCaller.PlayUIButton(UISoundType.Back);
            OnRestartButtonPressed?.Invoke();
        }
        
        private void MainMenuButton_OnClickHandler()
        {
            SoundEventCaller.PlayUIButton(UISoundType.Accept);
            CameraEventCaller.ZoomIn();
            OnMainMenuButtonPressed?.Invoke();
        }
        
        private void ResumeButton_OnClickHandler()
        {
            SoundEventCaller.PlayUIButton(UISoundType.Accept);
            GameModeEventCaller.SetPause(false);
        }
        
        private void PauseButton_OnClickHandler()
        {
            SoundEventCaller.PlayUIButton(UISoundType.Accept);
            OnPauseButtonPressed?.Invoke();
        }


        #endregion

        #region ScoreTextUpdate

        private void UpdateGameplayScoreText(int points)
        {
            var text = $"{GetLocalizedString("Gameplay.Score")}: {points:D6}";
            GetUiComponents().ScoreText.text = text;
        }

        private void UpdateDeathScoreText(int points)
        {
            var text = $"{GetLocalizedString("Gameplay.Death.Score")}: {points:D6}";
            GetUiComponents().DeathScoreText.text = text;
        }

        private void UpdateHighScoreText(int points)
        {
            var text = $"{GetLocalizedString("Gameplay.HighScore")}: {points:D6}";
            GetUiComponents().HighScoreText.text = text;
        }

        private IEnumerator IncreasePointsText(Action<int> increaseAction = null)
        {
            while (!_numberIncrementer.IsFinished)
            {
                if (!CustomTime.GetChannel(UpdateGroup.UI).IsPaused)
                {
                    _numberIncrementer.Run(CustomTime.GetDeltaTimeByChannel(UpdateGroup.UI));
                    increaseAction?.Invoke(GetCurrentPoints());
                }
                
                yield return null;
            }
        }

        #endregion

        #region Death Panel Queue Actions

        private void AddHighScoreToActionQueue(IDeathUITime deathPanelData)
        {
            List<ActionData> tempList = new List<ActionData>
            {
                new (() =>
                {
                    UpdateHighScoreText(0);
                    SetActiveHighScoreText(true);
                })
            };
            
            _numberIncrementer.SetData(new NumberIncrementerData
            {
                Target = GetHighScore(),
                TargetTime = deathPanelData.DeathPointsTimeToIncrease,
                ActionOnFinish = () =>
                {
                    TimerManager.Add(new TimerData
                    {
                        Time = deathPanelData.EnableRestartButton,
                        OnEndAction = () =>
                        {
                            SetActiveRestartButtonPanel(true);
                        }
                    });
                }
            });
            
            tempList.Add(
                new ActionData(
                    ()=> StartCoroutine(IncreasePointsText(UpdateHighScoreText)),
                    deathPanelData.CountDeathScore));
            
            ActionManager.Add(new ActionQueue(tempList),UpdateGroup.UI);
            
        }

        private void StartDeathPanelActionQueue(float deflectCount)
        {
            SetActiveDeathText(false);
            SetActiveDeathScoreText(false);
            SetActiveRestartButtonPanel(false);
            SetActiveHighScoreText(false);
            UpdateDeathScoreText(0);
            UpdateHighScoreText(0);
            SetActivePanel(GetUiComponents().DeathPanel);

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
                                () =>
                                {
                                    if (_hasHighScore)
                                    {
                                        AddHighScoreToActionQueue(deathPanelData);
                                    }
                                    else
                                    {
                                        List<ActionData> highScoreList = new List<ActionData>
                                        {
                                            new(() =>
                                            {
                                                UpdateHighScoreText(GetHighScore());
                                                SetActiveHighScoreText(true);
                                            },deathPanelData.CountHighScore),
                                            new(() =>
                                            {
                                                SetActiveRestartButtonPanel(true);
                                            },deathPanelData.EnableRestartButton)
                                        };
                                        
                                        ActionManager.Add(new ActionQueue(highScoreList),UpdateGroup.UI);
                                    }


                                },
                                deathPanelData.EnableHighScore));
                    }
                });
                
                tempList.Add(
                    new ActionData(
                        ()=> StartCoroutine(IncreasePointsText(UpdateDeathScoreText)),
                        deathPanelData.CountDeathScore));
            }
            else
            {
                tempList.Add(
                    new ActionData(
                        ()=> SetActiveHighScoreText(true),
                        deathPanelData.EnableHighScore));
                tempList.Add(
                    new ActionData(
                        ()=> SetActiveRestartButtonPanel(true),
                        deathPanelData.EnableRestartButton));
            }
            
            
            _deathPanelActionQueue.AddAction(tempList);
            
            ActionManager.Add(_deathPanelActionQueue,UpdateGroup.UI);
        }
        
        private void SetActiveDeathText(bool isActive)
        {
            GetUiComponents().DeathText.gameObject.SetActive(isActive);
        }

        private void SetActiveDeathScoreText(bool isActive)
        {
            GetUiComponents().DeathScoreText.gameObject.SetActive(isActive);
        }
        
        private void SetActiveHighScoreText(bool isActive)
        {
            GetUiComponents().HighScoreText.gameObject.SetActive(isActive);
        }

        private void SetActiveRestartButtonPanel(bool isActive)
        {
            GetUiComponents().DeathButtonContainer.gameObject.SetActive(isActive);
        }
        

        #endregion

        private int GetHighScore()
        {
            return (int)_highScore * GetPointsMultiplier();
        }

        private int GetCurrentPoints()
        {
            return (int)_numberIncrementer.CurrentValue;
        }

        private int GetPointsMultiplier()
        {
            return GameConfigManager.Instance.GetGameplayData().PointsMultiplier;
        }
        private string GetLocalizedString(string key)
        {
            return LocalizationManager.Instance.GetText(key);
        }
        
    }
}