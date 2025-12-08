using _Main.Scripts.Managers;
using UnityEngine;

namespace _Main.Scripts.Defeat
{
    [RequireComponent(typeof(DefeatUIView))]
    [RequireComponent(typeof(DefeatView))]
    [RequireComponent(typeof(DefeatViewAnimation))]
    public class DefeatSetup : MonoBehaviour
    {
        private DefeatView.IDefeatView _view;
        private DefeatUIView.IDefeatUIView _ui;
        private DefeatViewAnimation.IDefeatViewAnimation _animation;
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
            
            var view  = GetComponent<DefeatView>();
            var ui = GetComponent<DefeatUIView>();
            var anim = GetComponent<DefeatViewAnimation>();

            motor.Subscribe(view);
            motor.Subscribe(ui);
            motor.Subscribe(anim);
            
            _view = view;
            _ui = ui;
            _animation = anim;
            
            //
            
            GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
            GameScreenEventSubscriber.DisableScreen(EventBus_GameScreen_Disable);
            
            //
            
            _controller.InitializeController();
            SetViewHandlers();
            
            BootEvents.SubSystemInitialized();
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
        /// 1 - OnRestartButtonPressed or OnMainMenuButtonPressed requests another screen
        /// 2 - GameScreenEvents requests to disable
        /// 3 - StartDisable is triggered
        /// 4 - Main Animation Fade Out
        /// 5 - When animations ends, OnPanelClosed triggers Earth's restart
        /// 5 - When Earth's restart finished, ExecuteDisable is triggered
        /// </summary>
        
        private void SetViewHandlers()
        {
            _view.OnDataLoaded += (score, highScore, hasNewHigh) =>
            {
                _controller.LoadScoreData(score,highScore,hasNewHigh);
            };
            _view.OnDataInitialized += _controller.SetDataIsLoaded;
            //
            _ui.OnRestartButtonPressed += GameManager.Instance.LoadGameMode;
            _ui.OnMainMenuButtonPressed += GameManager.Instance.LoadMainMenu;
            //
            _animation.OnPanelOpened += _controller.SendScore;
            _animation.OnPanelClosed += () =>
            {
                EarthEventCaller.Restart();
                
#if UNITY_EDITOR
                
                DebugDefeatEvents.TriggerDefeatScreenClosed();
#endif
            };
            _animation.OnScoreFinished += _controller.SendHighScore;
            _animation.OnHighScoreFinished += _controller.SendButtons;
            _animation.OnButtonsFinished += () =>
            {
                _controller.EnableButtons();
                
#if UNITY_EDITOR
                
                DebugDefeatEvents.TriggerDefeatScreenAnimationFinished();
#endif
            };

            
            

        }

        #region Enable / Disable

        private void DisableDefeat()
        {
            UnsubscribeEventsBus();
            _controller.StartDisable();
        }
        
        private void EnableDefeat()
        {
            _controller.InitializeData();
            SubscribeEventsBus();
        }

        #endregion

        #region Event Bus

        private void SubscribeEventsBus()
        {
            EarthEventSubscriber.DestructionFinished(EventBus_Earth_Destruction_Finished);
            EarthEventSubscriber.RestartFinished(EventBus_Earth_Restart_Finished);
        }
        
        private void UnsubscribeEventsBus()
        {
            EarthEventUnSubscriber.DestructionFinished(EventBus_Earth_Destruction_Finished);
        }

        #region Earth

        private void EventBus_Earth_Destruction_Finished(EarthEvents.DestructionFinished input)
        {
            _controller.EnableScreen();
        }
        
        private void EventBus_Earth_Restart_Finished(EarthEvents.RestartFinished input)
        {
            _controller.ExecuteDisable();
        }

        #endregion
        
        #region Game Screens

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

        #endregion
    }
}