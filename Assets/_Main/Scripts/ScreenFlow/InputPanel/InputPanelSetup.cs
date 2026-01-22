using System;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.GlobalValues.Tools.Observer;
using UnityEngine;

namespace MeteorMadness.ScreenFlow._Main.Scripts.ScreenFlow.InputPanel
{
    public class InputPanelSetup : MonoBehaviour
    {
        private class Motor : ObservableComponent
        {
            public void EnableUI() => NotifyAll(InputPanelObserverMessage.Enable);

            public void DisableUI() => NotifyAll(InputPanelObserverMessage.Disable);
        }

        private InputPanelAnimationView.IInputPanelAnimationView _animation;
        private IInputUI _inputUI;
        private Motor _motor;

        private void Awake()
        {
            _motor = new Motor();
            _animation = GetComponent<InputPanelAnimationView.IInputPanelAnimationView>();
            
            _motor.Subscribe((IObserver)_animation);
            
            _inputUI = GetComponent<IInputUI>();

            SetHandlers();
        }
        

        private void SetHandlers()
        {
            _inputUI.OnEnable += OnEnableHandler;
            _inputUI.OnDisable += OnDisableHandler;
        }

        #region Handlers

        private void OnDisableHandler() => _motor.DisableUI();
        private void OnEnableHandler() => _motor.EnableUI();

        #endregion
    }
    
}