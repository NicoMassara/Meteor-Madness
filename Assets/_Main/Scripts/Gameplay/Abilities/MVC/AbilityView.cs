using System.Collections;
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
        private AbilityData _currentAbility;
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
        

        #region Ability

        private void HandleSelectAbility(AbilityType enumType)
        {
            if (!abilityDataController.HasAbilityData(enumType))
            {
                Debug.LogWarning("AbilityData Does not exist");
                return;
            }
            
            OnAbilitySelected?.Invoke();
        }

        private void HandleTriggerAbility(AbilityType enumType)
        {
            GameManager.Instance.EventManager.Publish(new AbilitiesEvents.EnableSpawner{IsEnable = true});
            ActionManager.Add(abilityDataController.GetAbilityStartQueue(enumType),SelfUpdateGroup);
        }

        private void HandleFinishAbility(AbilityType enumType)
        {
            GameManager.Instance.EventManager.Publish(new AbilitiesEvents.EnableSpawner{IsEnable = false});
            
            if (abilityDataController.GetHasInstantEffect(enumType)) return;
            
            ActionManager.Add(abilityDataController.GetAbilityEndQueue(enumType),SelfUpdateGroup);
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