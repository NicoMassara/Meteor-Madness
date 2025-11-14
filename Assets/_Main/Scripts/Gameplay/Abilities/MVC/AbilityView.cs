using System;
using System.Collections;
using _Main.Scripts.Gameplay.Abilities;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using _Main.Scripts.Observer;
using _Main.Scripts.Sounds;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityView : ManagedBehavior, IObserver
    {
        [Header("Sound Data")]
        [SerializeField] private SoundClassSo abilityAdd;
        [SerializeField] private SoundClassSo abilityTrigger;
        [SerializeField] private SoundClassSo slowTime;
        [SerializeField] private SoundClassSo speedTime;

        private ulong _finishAbilityTimerId;
        
        private AbilityStoredData currentAbilityStored;
        private AbilityDataController abilityDataController;
        
        public UnityAction OnAbilitySelected;
        public UnityAction OnAbilityFinished;
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Ability;
        
        public event Action OnAbilityTriggered;
        public event Action OnAbilityAdded;

        public event Action OnTimeSlowDown;
        public event Action OnTimeSpeedUp;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private AbilityDebugData _debugData;
#endif

        private void Awake()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData = new AbilityDebugData();
#endif
        }

        private void Start()
        {
            abilityDataController = new AbilityDataController(AbilitiesData_UpdateTimeScale, OnTimeSpeedUp, OnTimeSlowDown);
            abilityDataController.OnAbilityStarted += AbilitiesData_OnAbilityStartedHandler;
            abilityDataController.OnEndQueueFinished += AbilitiesData_OnEndQueueFinished;
            
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
                case AbilityObserverMessage.ForceFinish:
                    HandleForceFinish();
                    break;
            }
        }
        

        private void HandleAddAbility(int index, Vector2 position)
        {
            var ability = (AbilityType)index;
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.AddAbility(index);
#endif
            FloatingTextEventCaller.Spawn(new FloatingTextValues
            {
                Position = position,
                Offset = new Vector2(0, 1f),
                Text = AbilityDataGetter.GetDisplayName(ability),
                Color = AbilityDataGetter.GetColor(ability),
                DoesFade = true,
                DoesMove = true
            });
            OnAbilityAdded?.Invoke();
        }

        private void HandleSetStorageFull(bool isFull)
        {
            AbilitiesEventCaller.SetStorageFull(isFull);
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
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.RemoveAbility();
#endif
            
            OnAbilitySelected?.Invoke();
        }

        private void HandleTriggerAbility(int abilityIndex)
        {
            ActionManager.Add(abilityDataController.GetAbilityStartQueue(
                (AbilityType)abilityIndex),SelfUpdateGroup);
            
            GameModeEventCaller.SetEnablePause(false);
            
            OnAbilityTriggered?.Invoke();
        }

        private void HandleFinishAbility(int abilityIndex)
        {
            if (abilityDataController.GetHasInstantEffect((AbilityType)abilityIndex))
            {
                GameModeEventCaller.SetEnablePause(true);
                return;
            }

            ActionManager.Add(abilityDataController.GetAbilityEndQueue(
                (AbilityType)abilityIndex),SelfUpdateGroup);
        }
        
        private void HandleForceFinish()
        {
            TimerManager.Remove(ref _finishAbilityTimerId);
            
            OnAbilityFinished?.Invoke();
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
            _finishAbilityTimerId = TimerManager.Add(new TimerData
            {
                Time = activeTime,
                OnEndAction = ()=> OnAbilityFinished?.Invoke()
            }, SelfUpdateGroup);
        }

        private void AbilitiesData_UpdateTimeScale(TimeScaleData timeScaleData)
        {
            StartCoroutine(Coroutine_UpdateTimeScale(timeScaleData));
        }
        
        private void AbilitiesData_OnEndQueueFinished(AbilityType abilityType)
        {
            GameModeEventCaller.SetEnablePause(true);
        }

        #endregion
    }
}