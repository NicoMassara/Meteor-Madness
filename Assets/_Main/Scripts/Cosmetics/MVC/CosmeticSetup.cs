using System;
using _Main.Scripts.GlobalEvents;
using _Main.Scripts.Managers;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.MVC
{
    [RequireComponent(typeof(CosmeticView))]
    [RequireComponent(typeof(CosmeticUIView))]
    [RequireComponent(typeof(CosmeticViewAnimation))]
    public class CosmeticSetup : ManagedBehavior
    {
        private CosmeticView.ICosmeticView _view;
        private CosmeticUIView.ICosmeticUIView _ui;
        private CosmeticViewAnimation.ICosmeticViewAnimation _animation;
        private CosmeticController _controller;
        private CosmeticMotor _motor;

        private void Awake()
        {
            _motor = new CosmeticMotor();
            _controller = new CosmeticController(_motor);
            
            
            var view = GetComponent<CosmeticView>();
            var ui = GetComponent<CosmeticUIView>();
            var anim = GetComponent<CosmeticViewAnimation>();
            
            _motor.Subscribe(view);
            _motor.Subscribe(ui);
            _motor.Subscribe(anim);
            
            _view = view;
            _ui = ui;
            _animation = anim;
            
            _controller.InitializeValues();
            
            SetViewHandlers();
            
            GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
            GameScreenEventSubscriber.DisableScreen(EventBus_GameScreen_Disable);
        }
        
        private void EnableCosmetic()
        {
            _controller.TransitionToInitialize();
            SubscribeEventBus();
        }

        private void DisableCosmetic()
        {
            UnsubscribeEventBus();
            _controller.TransitionToDisable();
        }

        #region View Handlers
        

        private void SetViewHandlers()
        {
            _view.OnInitialized += ()=> _controller.TransitionToEnable();
            //
            _ui.OnMainMenuButtonPressed += () => { _controller.TriggerMainMenu();};
            _ui.OnSkinSelected += (value) => _controller.SkinSelected(value);
            //
            _animation.OnPanelOpened += () => AdsEvents.Banner_TriggerShow();
            _animation.OnPanelClosed += () => _controller.ExecuteDisable();
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