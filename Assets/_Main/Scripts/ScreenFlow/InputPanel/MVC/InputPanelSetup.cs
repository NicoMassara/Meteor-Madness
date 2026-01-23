using System;
using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
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

            _animation = GetComponent<InputPanelAnimationView>();
            _motor.Subscribe((IObserver)_animation);
            
            _inputUI = GetComponent<IInputUI>();
            
            InputsEventSubscriber.SetUIEnable(EventBus_Inputs_SetUIEnable);
            
        }

        private void EventBus_Inputs_SetUIEnable(InputsEvents.SetUIEnable input)
        {
            if (input.IsEnable)
                _motor.EnableUI();
            else
                _motor.DisableUI();
        }
    }
    
}