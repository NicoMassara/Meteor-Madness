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

            public void Collision(float inputHealthRatio) => NotifyAll(InputPanelObserverMessage.Collision,inputHealthRatio);
        }

        private InputPanelAnimationView.IInputPanelAnimationView _animation;
        private InputPanelView.IInputPanelView _view;
        private IInputUI _inputUI;
        private Motor _motor;

        private void Awake()
        {
            _motor = new Motor();

            _animation = GetComponent<InputPanelAnimationView>();
            _view = GetComponent<InputPanelView.IInputPanelView>();
            _motor.Subscribe((IObserver)_animation);
            _motor.Subscribe((IObserver)_view);
            
            _inputUI = GetComponent<IInputUI>();
            
            InputsEventSubscriber.SetUIEnable(EventBus_Inputs_SetUIEnable);
            InputsEventSubscriber.ShakeUI(EventBus_Inputs_Shake);
        }

        private void EventBus_Inputs_SetUIEnable(InputsEvents.SetUIEnable input)
        {
            if (input.IsEnable)
                _motor.EnableUI();
            else
                _motor.DisableUI();
        }
        
        private void EventBus_Inputs_Shake(InputsEvents.ShakeUI input)
        {
            _motor.Collision(input.HealthRatio);
        }
    }
    
}