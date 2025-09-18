using System;
using System.Collections;
using System.Collections.Generic;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Managers.UpdateManager.Interfaces;
using _Main.Scripts.MyCustoms;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityView : ManagedBehavior, IUpdatable, IObserver
    {
        private readonly Timer _abilityTimer = new Timer();
        private AbilityData _currentAbility;
        private AbilityController _controller;
        private AbilityDataController abilityDataController;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Ability;

        private void Start()
        {
            abilityDataController = new AbilityDataController(GameManager.Instance.EventManager);
            abilityDataController.OnAbilityStarted += AbilitiesData_OnAbilityStartedHandler;
        }

        public void ManagedUpdate()
        {

        }

        public void OnNotify(string message, params object[] args)
        {
            switch (message)
            {
                case AbilityObserverMessage.SelectAbility:
                    HandleSelectAbility((AbilityType)args[0]);
                    break;
                case AbilityObserverMessage.TriggerAbility:
                    HandleTriggerAbility((AbilityType)args[0]);
                    break;
                case AbilityObserverMessage.FinishAbility:
                    HandleFinishAbility((AbilityType)args[0]);
                    break;
            }
        }

        public void SetController(AbilityController controller)
        {
            _controller = controller;
        }

        #region Ability

        private void HandleSelectAbility(AbilityType enumType)
        {
            if (!abilityDataController.HasAbilityData(enumType))
            {
                Debug.LogWarning("AbilityData Does not exist");
                return;
            }
            
            _controller.TransitionToRunning();
        }

        private void HandleTriggerAbility(AbilityType enumType)
        {
            if (!abilityDataController.HasAbilityData(enumType))
            {
                Debug.LogWarning("AbilityData Does not exist");
                return;
            }
            
            StartCoroutine(RunAbilityQueue(abilityDataController.GetAbilityStartQueue(enumType)));
        }

        private void HandleFinishAbility(AbilityType enumType)
        {
            StartCoroutine(RunAbilityQueue(abilityDataController.GetAbilityEndQueue(enumType)));
        }

        #endregion

        #region Coroutine

        private IEnumerator RunAbilityQueue(ActionQueue actionQueue)
        {
            while (!actionQueue.IsEmpty)
            {
                actionQueue.Run(CustomTime.GetChannel(SelfUpdateGroup).DeltaTime);

                yield return null;
            }
        }
        
        private IEnumerator RunAbilityTimer()
        {
            while (!_abilityTimer.GetHasEnded)
            {
                _abilityTimer.Run(CustomTime.GetChannel(SelfUpdateGroup).DeltaTime);

                yield return null;
            }
        }

        #endregion

        #region Handler

        private void AbilitiesData_OnAbilityStartedHandler(float activeTime)
        {
            _abilityTimer.Set(activeTime);
            _abilityTimer.OnEnd += AbilityTimer_OnEndHandler;
            StartCoroutine(RunAbilityTimer());
        }
        
        private void AbilityTimer_OnEndHandler()
        {
            _abilityTimer.OnEnd -= AbilityTimer_OnEndHandler;
            _controller.TransitionToEnable();
        }

        #endregion
    }
}