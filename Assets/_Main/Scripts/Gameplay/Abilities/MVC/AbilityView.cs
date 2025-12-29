using System;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces.Sounds;
using MeteorMadness.Contracts.Interfaces.Vibration;
using MeteorMadness.Gameplay.Abilities.So;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.GlobalValues.Utilities;
using MeteorMadness.Managers;
using NicolasMassara.CustomActionManager;
using NicolasMassara.CustomTimerManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.Gameplay.Abilities
{
    public class AbilityView : ManagedBehavior, IObserver, IAbilitySounds,
        AbilityView.IAbilityView, IAbilityVibration
    {
        public interface IAbilityView
        {
            public event Action OnAbilitySelected;
            public event Action OnAbilityFinished;

        }
        
        [SerializeField] private AbilityConfigTimeDataSo abilityTimeData;
        private TimerManager.GeneratedId _finishAbilityTimerId;
        private ActionManager.GeneratedId _actionId;
        
        private AbilityStoredData currentAbilityStored;
        private AbilityDataController abilityDataController;
        
        public event Action OnAbilitySelected;
        public event Action OnAbilityFinished;
        
        public event Action OnAbilityTriggered;
        public event Action OnAbilityAdded;
        public event Action OnTimeSlowDown;
        public event Action OnTimeSpeedUp;

        private void Start()
        {
            abilityDataController = new AbilityDataController(OnTimeSpeedUp, OnTimeSlowDown);
            abilityDataController.OnAbilityStarted += AbilitiesData_OnAbilityStartedHandler;
            abilityDataController.OnEndQueueFinished += AbilitiesData_OnEndQueueFinished;
            abilityDataController.Initialize(abilityTimeData);

            GameManager.Instance.OnPaused += GM_OnPausedHandler;
            GameManager.Instance.OnResumed += GM_OnResumedHandler;
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
                
                // === UI === //
                case AbilityObserverMessage.Initialize:
                    HandleInitialize();
                    break;
                
                case AbilityObserverMessage.EnableUI:
                    HandleEnableUI();
                    break;
                
                case AbilityObserverMessage.DisableUI:
                    HandleDisableUI();
                    break;
                
                case AbilityObserverMessage.RestartAbilities:
                    HandleRestartAbilities();
                    break;
            }
        }

        private void HandleRestartAbilities() => AbilitiesUIEventCaller.Restart();
        private void HandleDisableUI() => AbilitiesUIEventCaller.DisableUI();
        private void HandleEnableUI() => AbilitiesUIEventCaller.EnableUI();
        private void HandleInitialize() => AbilitiesUIEventCaller.Initialize();


        private void HandleAddAbility(int index, Vector2 position)
        {
            var ability = (AbilityType)index;
            
            FloatingTextEventCaller.Spawn(new FloatingTextValues
            {
                Position = position,
                Offset = new Vector2(0, 1f),
                Text = AbilityDataGetter.GetDisplayName(ability),
                Color = AbilityColorHelper.GetColor(ability),
                DoesFade = true,
                DoesMove = true
            });
            
            AbilitiesUIEventCaller.Add(index);
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
            
            OnAbilitySelected?.Invoke();
        }

        private void HandleTriggerAbility(int abilityIndex)
        {
            _actionId = ActionManager.Add(abilityDataController.GetAbilityStartQueue(
                (AbilityType)abilityIndex),ActionManager.UpdateType.Update);
            
            GameModeEventCaller.SetEnablePause(false);
            
            AbilitiesUIEventCaller.SelectAbility();
            OnAbilityTriggered?.Invoke();
        }

        private void HandleFinishAbility(int abilityIndex)
        {
            
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
        
        private void GM_OnResumedHandler()
        {
            TimerManager.Resume(_finishAbilityTimerId);
        }

        private void GM_OnPausedHandler()
        {
            TimerManager.Pause(_finishAbilityTimerId);
        }

        #endregion
    }
}