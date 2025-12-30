using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Managers;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Stats
{
    [RequireComponent(typeof(StatsView))]
    [RequireComponent(typeof(StatsUIView))]
    [RequireComponent(typeof(StatsViewAnimation))]
    public class StatsSetup : MonoBehaviour
    {
        private StatsView.IStatsView _view;
        private StatsUIView.IStatsUIView _ui;
        private StatsViewAnimation.IStatsViewAnimation _animation;
        private StatsMotor _motor;

        private void Awake()
        {
            _motor = new StatsMotor();
            
            var view = GetComponent<StatsView>();
            var ui = GetComponent<StatsUIView>();
            var anim = GetComponent<StatsViewAnimation>();
            
            _motor.Subscribe(view);
            _motor.Subscribe(ui);
            _motor.Subscribe(anim);
            
            _view = view;
            _ui = ui;
            _animation = anim;
            
            SetViewHandlers();
            
            GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
            GameScreenEventSubscriber.DisableScreen(EventBus_GameScreen_Disable);
        }

        private void SetViewHandlers()
        {
            _view.OnInitialize += (value) => _motor.LoadTextData(value);
            _view.OnFirstOpen += () => _motor.TriggerFirstOpen();
            //
            _ui.OnBackButtonPressed += () => _motor.OpenMainMenu();
            _ui.OnTextsLoaded += () => _motor.Enable();
            //
            _animation.OnPanelClosed += () => _motor.ExecuteDisable();
            _animation.OnPanelOpened += () => _motor.PanelOpened();
        }
        
        private void EnableStats()
        {
            _motor.Initialize();
            SubscribeEventBus();
        }

        private void DisableStats()
        {
            _motor.StartDisable();
            UnsubscribeEventBus();
        }
        
        #region Event Bus

        private void SubscribeEventBus()
        {
            
        }
        
        private void UnsubscribeEventBus()
        {
            
        }
        
        #region GameScreen

        private void EventBus_GameScreen_Disable(GameScreenEvents.DisableScreen input)
        {
            if(input.ScreenType != ScreenType.Stats) return;
            
            if (input.RequestType == EventRequestType.Requested)
            {
                DisableStats();
            }
        }

        private void EventBus_GameScreen_Enable(GameScreenEvents.EnableScreen input)
        {
            if(input.ScreenType != ScreenType.Stats) return;
            
            if (input.RequestType == EventRequestType.Granted)
            {
                EnableStats();
            }
        }

        #endregion

        #endregion
    }
}