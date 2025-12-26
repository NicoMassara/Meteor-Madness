using System;
using MeteorMadness.GlobalValues.Interfaces.Sounds;
using MeteorMadness.GlobalValues.Interfaces.Vibration;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.Managers;
using MeteorMadness.ScreenFlow.Base;
using NicolasMassara.CustomActionManager;
using Unity.Mathematics;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Defeat
{
    public class DefeatUIView : BaseViewUI<DefeatUiPanelSelector,DefeatUIComponents>, IObserver,
        DefeatUIView.IDefeatUIView, IDefeatUiSounds, IDefeatUIVibration
    {
        
        public interface IDefeatUIView
        {
            public event Action OnMainMenuButtonPressed;
            public event Action OnRestartButtonPressed;
            public event Action OnCoinsFinished;
        }
        
        public event Action OnMainMenuButtonPressed;
        public event Action OnCoinsFinished;
        public event Action OnRestartButtonPressed;

        private void Start()
        {
            UpdateNewCoinsText(0);
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                // === Enable / Disable === //
                case DefeatObserverMessage.InitializeData:
                    HandleInitializeData();
                    break;
                case DefeatObserverMessage.StartDisable:
                    HandleStartDisable();
                    break;
                case DefeatObserverMessage.EnableButtons:
                    HandleEnableButtons();
                    break;
                
                // === Coins === //
                case DefeatObserverMessage.UpdateCoins:
                    HandleUpdateCoins((uint)args[0], (uint)args[1]);
                    break;
            }
        }
        
        #region Coins
        
        private void HandleUpdateCoins(uint stored, uint gained)
        {
            var action = ActionBuilder.Start()
                    .Do(new WaitSecondsAction(0.5f))
                    .Then(new IncrementCoinsAction(0.75f, 0, gained,UpdateNewCoinsText))
                    .Then(new WaitSecondsAction(0.5f))
                    .Then(new IncrementCoinsAction(0.5f, stored, (stored+gained),UpdateStoredCoinsText))
                    .Then(new WaitSecondsAction(0.5f))
                    .Then(new InstantAction(()=> OnCoinsFinished?.Invoke()))
                    .Build();
            
            ActionManager.Add(action,ActionManager.UpdateType.Update);
        }


        private class IncrementCoinsAction : IQueueAction
        {
            public ActionStatus CurrentStatus { get; private set; } = ActionStatus.Idle;
            private readonly float _increaseTime;
            private readonly uint _startValue;
            private readonly uint _endValue;
            private readonly Action<uint> _incrementAction;
            private uint _currentValue;
            private float _elapsedTime;

            public IncrementCoinsAction(float increaseTime, uint startValue, uint endValue, Action<uint> incrementAction)
            {
                _increaseTime = increaseTime;
                _startValue = startValue;
                _endValue = endValue;
                _incrementAction = incrementAction;
            }

            public void OnStart()
            {
                CurrentStatus = ActionStatus.Running;
            }

            public ActionStatus OnUpdate(float deltaTime)
            {
                _elapsedTime += deltaTime;
                float ratio = Mathf.Clamp01(_elapsedTime / _increaseTime);
                _currentValue = (uint)math.lerp(_startValue, _endValue, ratio);
                
                if (ratio >= 1f)
                {
                    _currentValue = _endValue;
                    _incrementAction?.Invoke(_currentValue);
                    CurrentStatus = ActionStatus.Success;
                }
                else
                {
                    _incrementAction?.Invoke(_currentValue);
                }
                
                return CurrentStatus;
            }

            public void OnInterrupt()
            {
                _incrementAction?.Invoke(_endValue);
                CurrentStatus = ActionStatus.Failure;
            }

            public IQueueAction Copy()
            {
                return null;
            }
        }

        private void UpdateNewCoinsText(uint amount) 
            => UIComponents.SetNewCoinsText("Cosmetic.GainedCoins", amount);
        
        private void UpdateStoredCoinsText(uint amount) 
            => UIComponents.SetStoredCoinsText("Cosmetic.StoredCoins", amount);

        #endregion
        
        #region Enable / Disable
        

        private void HandleInitializeData()
        {
            UIComponents.SetDeathTitle(GameManager.Instance.DeathTitle);
            UpdateNewCoinsText(0);
            UIComponents.SetStoredCoinsText("Cosmetic.StoredCoins", GameManager.Instance.GetStoredCoins());
        }

        private void HandleStartDisable()
        {
            UIComponents.RestartButton_RemoveListener(TriggerOnRestartButtonPressed);
            UIComponents.ainMenuButton_RemoveListener(TriggerOnMainMenuButtonPressed);
        }

        private void HandleEnableButtons()
        {
            UIComponents.RestartButton_AddListener(TriggerOnRestartButtonPressed);
            UIComponents.MainMenuButton_AddListener(TriggerOnMainMenuButtonPressed);
        }
        
        private void TriggerOnRestartButtonPressed() 
            => OnRestartButtonPressed?.Invoke();
        private void TriggerOnMainMenuButtonPressed() 
            => OnMainMenuButtonPressed?.Invoke();
        
        #endregion
    }
}