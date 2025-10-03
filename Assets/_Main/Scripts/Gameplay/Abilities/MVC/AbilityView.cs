using System;
using System.Collections;
using _Main.Scripts.Gameplay.AutoTarget;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using _Main.Scripts.Observer;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityView : ManagedBehavior, IUpdatable, IObserver
    {
        private AbilityStoredData currentAbilityStored;
        private AbilityDataController abilityDataController;
        
        public UnityAction OnAbilitySelected;
        public UnityAction OnAbilityFinished;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Ability;

        private void Start()
        {
            abilityDataController = new AbilityDataController(GameManager.Instance.EventManager, AbilitiesData_UpdateTimeScale);
            abilityDataController.OnAbilityStarted += AbilitiesData_OnAbilityStartedHandler;
        }

        public void ManagedUpdate()
        {

        }

        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case AbilityObserverMessage.AddAbility:
                    HandleAddAbility((int)args[0],(Vector2)args[1]);
                    break;
                case AbilityObserverMessage.SelectAbility:
                    HandleSelectAbility((int)args[0]);
                    break;
                case AbilityObserverMessage.TriggerAbility:
                    HandleTriggerAbility((int)args[0]);
                    break;
                case AbilityObserverMessage.FinishAbility:
                    HandleFinishAbility((int)args[0]);
                    break;
                case AbilityObserverMessage.RunActiveTimer:
                    HandleRunActiveTimer((int)args[0]);
                    break;
                case AbilityObserverMessage.SetStorageFull:
                    HandleSetStorageFull((bool)args[0]);
                    break;
            }
        }

        private void HandleAddAbility(int index, Vector2 position)
        {
            GameManager.Instance.EventManager.Publish(new FloatingTextEvents.Ability
            {
                Position = position,
                AbilityType = (AbilityType)index,
            });
        }

        private void HandleSetStorageFull(bool isFull)
        {
            GameManager.Instance.EventManager.Publish(new AbilitiesEvents.SetStorageFull{IsFull = isFull});
        }

        private void HandleRunActiveTimer(int abilityIndex)
        {
            abilityDataController.RunActiveTimer((AbilityType)abilityIndex);
        }

        #region Ability

        private void HandleSelectAbility(int abilityIndex)
        {
            if (!abilityDataController.HasAbilityData((AbilityType)abilityIndex))
            {
                Debug.LogWarning("AbilityData Does not exist");
                return;
            }
            
            OnAbilitySelected?.Invoke();
        }

        private void HandleTriggerAbility(int abilityIndex)
        {
            ActionManager.Add(abilityDataController.GetAbilityStartQueue(
                (AbilityType)abilityIndex),SelfUpdateGroup);
        }

        private void HandleFinishAbility(int abilityIndex)
        {
            if (abilityDataController.GetHasInstantEffect((AbilityType)abilityIndex)) return;
            
            ActionManager.Add(abilityDataController.GetAbilityEndQueue(
                (AbilityType)abilityIndex),SelfUpdateGroup);
        }

        #endregion

        #region Coroutine
        

        private IEnumerator Coroutine_UpdateTimeScale(TimeScaleData timeScaleData)
        {
            var currentTimeScale = timeScaleData.CurrentTimeScale;
            var duration = timeScaleData.TimeToUpdate;
            float elapsedTime = 0;
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float timeRatio = Mathf.Clamp01(elapsedTime / duration);
                currentTimeScale = Mathf.Lerp(currentTimeScale, timeScaleData.TargetTimeScale, timeRatio);
                
                foreach (var updateGroup in timeScaleData.UpdateGroups)
                {
                    CustomTime.SetChannelTimeScale(updateGroup, currentTimeScale);
                }
                
                yield return null;
            }
        }

        #endregion

        #region Handler

        private void AbilitiesData_OnAbilityStartedHandler(float activeTime)
        {
            TimerManager.Add(new TimerData
            {
                Time = activeTime,
                OnEndAction = ()=> OnAbilityFinished?.Invoke()
            }, SelfUpdateGroup);
        }

        private void AbilitiesData_UpdateTimeScale(TimeScaleData timeScaleData)
        {
            StartCoroutine(Coroutine_UpdateTimeScale(timeScaleData));
        }

        #endregion
    }
}