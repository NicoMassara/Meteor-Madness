using System;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Observer;
using _Main.Scripts.Sounds;
using NicolasMassara.CustomActionManager;
using NicolasMassara.CustomTimerManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Abilities
{
    public class AbilityView : ManagedBehavior, IObserver, IAbilitySounds,
        AbilityView.IAbilityView
    {
        public interface IAbilityView
        {
            public event Action OnAbilitySelected;
            public event Action OnAbilityFinished;

        }
        
        [Header("Sound Data")]
        [SerializeField] private SoundClassSo abilityAdd;
        [SerializeField] private SoundClassSo abilityTrigger;
        [SerializeField] private SoundClassSo slowTime;
        [SerializeField] private SoundClassSo speedTime;
        
        private TimerManager.GeneratedId _finishAbilityTimerId;
        private ActionManager.GeneratedId _actionId;
        
        private AbilityStoredData currentAbilityStored;
        private AbilityDataController abilityDataController;
        

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Ability;
        
        public event Action OnAbilitySelected;
        public event Action OnAbilityFinished;
        
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
            abilityDataController = new AbilityDataController(OnTimeSpeedUp, OnTimeSlowDown);
            abilityDataController.OnAbilityStarted += AbilitiesData_OnAbilityStartedHandler;
            abilityDataController.OnEndQueueFinished += AbilitiesData_OnEndQueueFinished;
            abilityDataController.Initialize();
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
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.CurrentAbility = (AbilityType)abilityIndex;
#endif
            
            _actionId = ActionManager.Add(abilityDataController.GetAbilityStartQueue(
                (AbilityType)abilityIndex),ActionManager.UpdateType.Update);
            
            GameModeEventCaller.SetEnablePause(false);
            
            OnAbilityTriggered?.Invoke();
        }

        private void HandleFinishAbility(int abilityIndex)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.CurrentAbility = (AbilityType)0;
#endif
            
            if (abilityDataController.GetHasInstantEffect((AbilityType)abilityIndex))
            {
                GameModeEventCaller.SetEnablePause(true);
                return;
            }

            _actionId = ActionManager.Add(abilityDataController.GetAbilityEndQueue(
                (AbilityType)abilityIndex),ActionManager.UpdateType.Update,ActionManager.PriorityTick.EveryFrame);
        }
        
        private void HandleForceFinish()
        {
            if (_actionId != null
                && _actionId.IsActive)
            {
                ActionManager.Remove(_actionId);
            }
            else if(_finishAbilityTimerId != null 
                    && _finishAbilityTimerId.IsActive)
            {
                TimerManager.Remove(_finishAbilityTimerId);
            }
            
            OnAbilityFinished?.Invoke();
        }

        #endregion
        
        #region Handler

        private void AbilitiesData_OnAbilityStartedHandler(float activeTime)
        {
            _finishAbilityTimerId = TimerManager.Add(new TimerData(activeTime, 
                ()=> OnAbilityFinished?.Invoke()));
        }
        
        private void AbilitiesData_OnEndQueueFinished(AbilityType abilityType)
        {
            GameModeEventCaller.SetEnablePause(true);
        }

        #endregion
    }
}