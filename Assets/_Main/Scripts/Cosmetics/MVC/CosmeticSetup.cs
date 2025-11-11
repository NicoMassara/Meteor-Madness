using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.MVC
{
    [RequireComponent(typeof(CosmeticView))]
    [RequireComponent(typeof(CosmeticUIView))]
    public class CosmeticSetup : ManagedBehavior
    {
        private CosmeticView _view;
        private CosmeticUIView _ui;
        private CosmeticController _controller;
        private CosmeticMotor _motor;

        private void Awake()
        {
            _motor = new CosmeticMotor();
            _controller = new CosmeticController(_motor);
            
            
            _view = GetComponent<CosmeticView>();
            _ui = GetComponent<CosmeticUIView>();
            
            _motor.Subscribe(_view);
            _motor.Subscribe(_ui);
            
            _controller.InitializeValues();
            
            SetViewHandlers();
            SetUIViewHandlers();
            
            GameEventCaller.Subscribe<GameScreenEvents.EnableScreen>(EventBus_GameScreen_Enable);
            GameEventCaller.Subscribe<GameScreenEvents.DisableScreen>(EventBus_GameScreen_Disable);
        }
        
        private void EnableCosmetic()
        {
            _controller.TransitionToEnable();
            SubscribeEventBus();
        }

        private void DisableCosmetic()
        {
            UnsubscribeEventBus();
            _controller.TransitionToDisable();
        }

        #region View Handlers

        private void SetUIViewHandlers()
        {
            _ui.OnMainMenuButtonPressed += () => { _controller.TriggerMainMenu();};
        }

        private void SetViewHandlers()
        {
            _view.OnCosmeticEnable += ()=> _controller.TransitionToInitial();
        }

        #endregion

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
            if(input.ScreenType != ScreenType.Cosmetic) return;
            
            if (input.RequestType == EventRequestType.Requested)
            {
                DisableCosmetic();
            }
        }

        private void EventBus_GameScreen_Enable(GameScreenEvents.EnableScreen input)
        {
            if(input.ScreenType != ScreenType.Cosmetic) return;
            
            if (input.RequestType == EventRequestType.Granted)
            {
                EnableCosmetic();
            }
        }

        #endregion

        #endregion
    }
}