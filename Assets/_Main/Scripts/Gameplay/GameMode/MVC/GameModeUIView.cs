using System;
using System.Collections;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Localization;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using NicolasMassara.CustomActionManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeUIView : ManagedBehavior, IObserver,
        IGameModeUISounds
    {
        [SerializeField] private GameModeUiPanelSelector uiSelector;
        
        private GameModeUIComponents _uiComponents;
        private NumberIncrementer _numberIncrementer = new NumberIncrementer();
        private Coroutine _gameplayPointsCoroutine;
        private IGameUIConfig _gameUIConfig;
        
        private bool _hasHighScore;
        private float _highScore;
        private string _scoreTextValue;
        
        public event Action OnMainMenuButtonPressed;
        public event Action OnRestartButtonPressed;
        public event Action OnPauseButtonPressed;
        public event Action OnResumeButtonPressed;
        public event Action OnOptionsButtonPressed;
        public event Action OnPointsAdded;
        
        private void Start()
        {
            GetUiComponents().PausePanel.OnResumeButtonPressed += () =>
            {
                OnResumeButtonPressed?.Invoke();
            };
            GetUiComponents().PausePanel.OnOptionsButtonPressed += () =>
            {
                OnOptionsButtonPressed?.Invoke();
            };
            GetUiComponents().PausePanel.OnMainMenuButtonPressed += () =>
            {
                OnMainMenuButtonPressed?.Invoke();
            };
            
            GetUiComponents().PauseButton.onClick.AddListener(() =>
            {
                OnPauseButtonPressed?.Invoke();
            });
            GetUiComponents().RestartButton.onClick.AddListener(() =>
            {
                OnRestartButtonPressed?.Invoke();
            });

            foreach (var button in GetUiComponents().MainMenuButtons)
            {
                button.onClick.AddListener(() =>
                {
                    OnMainMenuButtonPressed?.Invoke();
                });
            }

            GetUiComponents().DeathText.text = GetLocalizedString("Gameplay.Death.Title");

            _scoreTextValue = GetLocalizedString("Gameplay.Score");
            GetUiComponents().CountdownText.text = "";
            LocalizationEvents.OnLanguageChanged += Localization_OnLanguageChangedHandler;
        }

        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                // Initialize
                case GameModeObserverMessage.InitializeData:
                    HandleInitializeData();
                    break;
                
                // Enable / Disable
                case GameModeObserverMessage.Disable:
                    HandleDisable();
                    break;
                case GameModeObserverMessage.Enable:
                    HandleEnable();
                    break;
                
                // Countdouwn
                case GameModeObserverMessage.StartCountdown:
                    HandleStartCountdown();
                    break;
                case GameModeObserverMessage.UpdateCountdown:
                    HandleCountdown((float)args[0]);
                    break;
                case GameModeObserverMessage.CountdownFinish:
                    HandleCountdownFinish();
                    break;
                
                // GameMode
                case GameModeObserverMessage.StartGameplay:
                    HandleStartGameplay();
                    break;
                case GameModeObserverMessage.GameFinish:
                    HandleGameFinish();
                    break;
                case GameModeObserverMessage.GameRestart:
                    HandleGameRestart();
                    break;
                
                // Meteor
                case GameModeObserverMessage.MeteorDeflect:
                    HandleMeteorDeflect((float)args[0]);
                    break;
                
                //Earth
                case GameModeObserverMessage.EarthStartDestruction:
                    HandleEarthStartDestruction();
                    break;
                case GameModeObserverMessage.EarthEndDestruction:
                    HandleEarthEndDestruction((float)args[0]);
                    break;

                // Camera
                case GameModeObserverMessage.CameraZoomOut:
                    HandleCameraZoomOut();
                    break;
                case GameModeObserverMessage.CameraZoomIn:
                    HandleCameraZoomIn();
                    break;
                case GameModeObserverMessage.SetCanPause:
                    HandleSetCanPause((bool)args[0]);
                    break;
                
                // Screens
                case GameModeObserverMessage.TriggerMainMenu:
                    HandleTriggerMainMenu();
                    break;
                case GameModeObserverMessage.PausePanel:
                    HandlePausePanel((bool)args[0]);
                    break;
                case GameModeObserverMessage.GameplayPanel:
                    HandleGameplayPanel((bool)args[0]);
                    break;
                
                // Score
                case GameModeObserverMessage.SetHasHighScore:
                    HandleSetHighScore((bool)args[0],(float)args[1]);
                    break;
            }
        }

        private void HandleInitializeData()
        {
            _gameUIConfig = GameConfigManager.Instance.GetUIData();
        }

        private void HandleSetHighScore(bool hasNewHighScore, float highScore)
        {
            _highScore = highScore;
            _hasHighScore = hasNewHighScore;
        }

        private void HandleTriggerMainMenu()
        {
            GetUiComponents(). DisableActivePanel();
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
            GetUiComponents().SetActivePanel(GetUiComponents().CountdownPanel);
        }

        private void HandleDisable()
        {
            GetUiComponents().DisableActivePanel();
        }
        
        private void HandleGameplayPanel(bool isActive)
        {
            if (isActive)
            {
                GetUiComponents().SetActivePanel(GetUiComponents().GameplayPanel);
            }
            else
            {
                GetUiComponents().DisableActivePanel();
            }
        }
        
        private void HandlePausePanel(bool isActive)
        {
            if (isActive)
            {
                GetUiComponents().SetActivePanel(GetUiComponents().PausePanel.Panel);
            }
            else
            {
                GetUiComponents().DisableActivePanel();
            }

        }


        #region Start

        private void HandleStartCountdown()
        {
            GetUiComponents().DisableActivePanel();
            GetUiComponents().SetActivePanel(GetUiComponents().CountdownPanel);
        }
        
        private void HandleCountdown(float countdownTime)
        {
            var text = countdownTime >= 1 ? $"{GetLocalizedString("Gameplay.CountDownStart")} {(int)countdownTime}..." 
                : GetLocalizedString("Gameplay.CountdownFinish");
            GetUiComponents().CountdownText.text = text;
        }
        
        private void HandleCountdownFinish()
        {
            GetUiComponents().CountdownText.text = "";
        }
        
        private void HandleStartGameplay()
        {
            _hasHighScore = false;
            _numberIncrementer?.ResetValues();
            UpdateGameplayScoreText(0);
        }

        #endregion

        #region Death

        private void HandleGameFinish()
        {
            GetUiComponents().DisableActivePanel();
        }
        
        private void HandleGameRestart()
        {
            GetUiComponents().DisableActivePanel();
        }
        
        private void HandleEarthEndDestruction(float deflectCount)
        {
            StartDeathPanelActionQueue(deflectCount);
        }

        private void HandleEarthStartDestruction()
        {
            GetUiComponents().DisableActivePanel();
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

        #region ScoreTextUpdate

        private void UpdateGameplayScoreText(int points)
        {
            var text = $"{_scoreTextValue}: {points:D6}";
            GetUiComponents().ScoreText.text = text;
            GetUiComponents().PausePanel.SetScoreText(points);
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
                    OnPointsAdded?.Invoke();
                }
                
                yield return null;
            }
        }

        #endregion

        #region Death Panel Queue Actions

        private class IncreasePointsAction : IQueueAction
        {
            private readonly Action<int> _increaseAction;
            private readonly NumberIncrementer _incrementer;
            private readonly Func<int> _getCurrentPoints;
            private readonly Action _onPointsAdded;
            public ActionStatus CurrentStatus { get; private set; } = ActionStatus.Idle;

            public IncreasePointsAction(Action<int> increaseAction, NumberIncrementer incrementer, 
                Func<int> getCurrentPoints, Action onPointsAdded)
            {
                _increaseAction = increaseAction;
                _incrementer = incrementer;
                _getCurrentPoints = getCurrentPoints;
                _onPointsAdded = onPointsAdded;
            }

            public void OnStart()
            {
                CurrentStatus = ActionStatus.Running;
            }

            public ActionStatus OnUpdate(float deltaTime)
            {
                if (CustomTime.GetChannel(UpdateGroup.UI).IsPaused) return CurrentStatus;
                
                
                _incrementer.Run(CustomTime.GetDeltaTimeByChannel(UpdateGroup.UI));
                _increaseAction?.Invoke(_getCurrentPoints.Invoke());
                _onPointsAdded?.Invoke();
                
                CurrentStatus = _incrementer.IsFinished ? ActionStatus.Success : ActionStatus.Running; 
                
                return CurrentStatus;
            }

            public void OnInterrupt() { }
            public IQueueAction Copy()
            {
                return new IncreasePointsAction(_increaseAction, _incrementer, _getCurrentPoints, _onPointsAdded);
            }
        }
        

        private void StartDeathPanelActionQueue(float deflectCount)
        {
            // Prepares the UI
            
            SetActiveDeathText(false);
            SetActiveDeathScoreText(false);
            SetActiveRestartButtonPanel(false);
            SetActiveHighScoreText(false);
            UpdateDeathScoreText(0);
            UpdateHighScoreText(0);
            GetUiComponents().SetActivePanel(GetUiComponents().DeathPanel);

            var deathPanelData = _gameUIConfig.DeathUITimeData;

            // Enables the first UI Elements
            
            var actions = ActionBuilder.Start()
                .Do(new WaitSecondsAction(deathPanelData.ShowDeathUI))
                .Then(new WaitSecondsWithCallBack(deathPanelData.SetEnableDeathText, ()=> SetActiveDeathText(true)))
                .Then(new WaitSecondsWithCallBack(deathPanelData.SetEnableDeathScore, ()=> SetActiveDeathScoreText(true)))
                .Then(new WaitSecondsAction(deathPanelData.EnableHighScore));
            
            
            // Checks if has to increase UI Points
            
            if (deflectCount > 0)
            {
                actions.
                    Then(new InstantAction(() =>
                    {
                        _numberIncrementer.SetData(new NumberIncrementerData
                        {
                            Target = deflectCount * GetPointsMultiplier(),
                            TargetTime = deathPanelData.DeathPointsTimeToIncrease,
                        });
                    }))
                    .Then(new WaitSecondsAction(deathPanelData.CountDeathScore))
                    .Then(new IncreasePointsAction(UpdateDeathScoreText, _numberIncrementer, GetCurrentPoints,
                        OnPointsAdded));

            }
            
            // Enables HighScore text
            
            actions
                .Then(new WaitSecondsAction(deathPanelData.EnableHighScore))
                .Then(new SetIntAction(_hasHighScore ? 0 : GetHighScore(), UpdateHighScoreText))
                .Then(new SetBoolAction(true, SetActiveHighScoreText));
            
            // Checks if has a new high score 
            
            if (_hasHighScore)
            {
                actions
                    .Then(new InstantAction(() =>
                    {
                        _numberIncrementer.SetData(new NumberIncrementerData
                        {
                            Target = GetHighScore(),
                            TargetTime = deathPanelData.DeathPointsTimeToIncrease,
                        });
                    }))
                    .Then(new WaitSecondsAction(deathPanelData.CountHighScore))
                    .Then(new IncreasePointsAction(UpdateHighScoreText, _numberIncrementer, GetCurrentPoints,
                        OnPointsAdded));
            }
            
            // Enables Restart Button
            
            actions
                .Then(new WaitSecondsAction(deathPanelData.EnableRestartButton))
                .Then(new SetBoolAction(true, SetActiveRestartButtonPanel));
            
            // Add Sequence to Manager
            
            ActionManager.Add(actions.Build(),ActionManager.UpdateType.Update);
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
        
        private void Localization_OnLanguageChangedHandler()
        {
            var lastText = _scoreTextValue;
            _scoreTextValue = GetLocalizedString("Gameplay.Score");
            GetUiComponents().ScoreText.text = GetUiComponents().ScoreText.text.Replace(lastText, _scoreTextValue);
        }
        
    }
}