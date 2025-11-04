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
            
            GameEventCaller.Subscribe<GameScreenEvents.SetScreen>(EventBus_GameScreen_SetScreen);
        }

        private void EventBus_GameScreen_SetScreen(GameScreenEvents.SetScreen input)
        {
            if (input.ScreenType == ScreenType.Cosmetic &&
                input.IsEnable)
            {
                _controller.TransitionToEnable();
            }
            else
            {
                DisableCosmetic();
            }
        }
        
        private void EnableCosmetic()
        {
            SubscribeEventBus();
            _controller.TransitionToInitial();
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
            _view.OnCosmeticEnable += EnableCosmetic;
        }

        #endregion

        #region Event Bus

        private void SubscribeEventBus()
        {
            
        }
        
        private void UnsubscribeEventBus()
        {
            
        }

        #endregion
    }
}