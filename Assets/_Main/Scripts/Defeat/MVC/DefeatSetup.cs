using _Main.Scripts.Managers;
using UnityEngine;

namespace _Main.Scripts.Defeat
{
    [RequireComponent(typeof(DefeatUIView))]
    [RequireComponent(typeof(DefeatView))]
    [RequireComponent(typeof(DefeatViewAnimation))]
    public class DefeatSetup : MonoBehaviour
    {
        private DefeatView _view;
        private DefeatUIView _ui;
        private DefeatViewAnimation _animation;
        
        private DefeatController.IDefeatController _controller;
        
        
        private void Awake()
        {
            BootEvents.OnSubSystemRequestInitialize += Initialize;
        }

        private void Initialize()
        {
            BootEvents.OnSubSystemRequestInitialize -= Initialize;
            //

            var motor = new DefeatMotor();
            _controller = new DefeatController(motor);
            
            //
            
            _view = GetComponent<DefeatView>();
            _ui = GetComponent<DefeatUIView>();
            _animation = GetComponent<DefeatViewAnimation>();
            
            //

            motor.Subscribe(_view);
            motor.Subscribe(_ui);
            motor.Subscribe(_animation);
            
            //
            
            GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
            GameScreenEventSubscriber.DisableScreen(EventBus_GameScreen_Disable);
            
            //
            
            _controller.InitializeController();
            SetViewHandlers();
            
            BootEvents.TriggerOnSubSystemInitialized();
        }
        
        /// <summary>
        /// Step by Step - Enable
        /// 1 - GameScreenEvents Request Enable
        /// 2 - Defeat data is gotten from GameManager
        /// 3 - When data is already loaded it notifies it and screen does enable
        /// 4 - Main Animation and triggers OnPanelOpened when finishes
        /// 5 - Score Animations
        /// 6 - High Score Animation
        /// 7 - Buttons Animation
        /// 8 - Buttons Does Enable
        ///
        /// Step by Step - Disable
        /// 1 - OnRestartButtonPressed or OnMainMenuButtonPressed opens another screen
        /// 2 - GameScreenEvents requests to disable
        /// 3 - StartDisable is triggered
        /// 4 - Main Animation Fade Out
        /// 5 - When animations ends, OnPanelClosed is triggered and completly disables the screen
        /// </summary>
        
        private void SetViewHandlers()
        {
            _view.OnDataLoaded += (score, highScore, hasNewHigh) =>
            {
                _controller.LoadScoreData(score,highScore,hasNewHigh);
            };
            _view.OnDataInitialized += _controller.EnableScreen;
            //
            _ui.OnRestartButtonPressed += GameManager.Instance.LoadGameMode;
            _ui.OnMainMenuButtonPressed += GameManager.Instance.LoadMainMenu;
            //
            _animation.OnPanelOpened += _controller.SendScore;
            _animation.OnPanelClosed += _controller.ExecuteDisable;
            _animation.OnScoreFinished += _controller.SendHighScore;
            _animation.OnHighScoreFinished += _controller.SendButtons;
        }

        #region Enable / Disable

        private void DisableDefeat()
        {
            _controller.StartDisable();
        }
        
        private void EnableDefeat()
        {
            _controller.InitializeData();
        }

        #endregion

        #region Event Bus

        private void EventBus_GameScreen_Disable(GameScreenEvents.DisableScreen input)
        {
            if(input.ScreenType != ScreenType.Defeat) return;
            
            if (input.RequestType == EventRequestType.Requested)
            {
                DisableDefeat();
            }
        }

        private void EventBus_GameScreen_Enable(GameScreenEvents.EnableScreen input)
        {
            if(input.ScreenType != ScreenType.Defeat) return;
            
            if (input.RequestType == EventRequestType.Granted)
            {
                EnableDefeat();
            }
        }

        #endregion
    }
}