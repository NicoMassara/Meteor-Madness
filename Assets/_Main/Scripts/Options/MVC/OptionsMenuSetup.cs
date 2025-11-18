using System;
using _Main.Scripts.Managers;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Options.MVC
{
    [RequireComponent(typeof(OptionsMenuView))]
    [RequireComponent(typeof(OptionsMenuUIView))]
    public class OptionsMenuSetup : ManagedBehavior
    {
        private OptionsMenuView _view;
        private OptionsMenuUIView _ui;
        
        private OptionMenuController _controller;

        private void Awake()
        {
            _view = GetComponent<OptionsMenuView>();
            _ui = GetComponent<OptionsMenuUIView>();
            
            _controller.Subscribe(_view);
            _controller.Subscribe(_ui);
            
            SetViewHandlers();
            SetUIHandlers();
            
            GameEventCaller.Subscribe<GameScreenEvents.SetScreen>(EventBus_GameScreen_SetScreen);
        }
        
        #region View Handlers

        private void SetViewHandlers()
        {
            _view.OnOptionsMenuEnable += () => { _controller.Initialize(); };
        }

        private void SetUIHandlers()
        {
            _ui.OnBackToMenu += () => { _controller.TriggerMainMenu(); };
        }

        #endregion
        
        private void EventBus_GameScreen_SetScreen(GameScreenEvents.SetScreen input)
        {
            if (input.ScreenType == ScreenType.OptionsMenu &&
                input.IsEnable)
            {
                _controller.Enable();
            }
            else
            {
                _controller.Disable();
            }
        }
    }
}