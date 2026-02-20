using System;
using _Main.Scripts.EventBus;
using _Main.Scripts.GameCamera;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Contracts.Interfaces.Sounds;
using MeteorMadness.Contracts.Interfaces.Vibration;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Abilities;
using MeteorMadness.Gameplay.Abilities.So;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.GlobalValues.Utilities;
using NicolasMassara.CustomActionManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.Gameplay.Abilities
{
    public class AbilityView : ManagedBehavior, IAbilitySounds,
        IAbilityView, 
        IAbilityVibration,
        IAbilityButtonUpdater
    {
        [SerializeField] private CameraTransportDataSo zoomInData;
        [SerializeField] private CameraTransportDataSo zoomOutData;
        [SerializeField] private AbilityConfigTimeDataSo abilityTimeData;
        
        private ActionManager.GeneratedId _actionId;
        
        private AbilityStoredData _currentAbilityStored;
        private AbilityDataController _abilityDataController;
            
        #region IAbilityView
        
        public event Action OnAbilityFinished;
        
        public event Action OnAbilityTriggered;
        public event Action OnAbilityAdded;

        #endregion

        #region IAbilityButtonUpdater

        public event Action OnRestart;

        #endregion
        
        public event Action OnTimeSlowDown;
        public event Action OnTimeSpeedUp;

        private void Start()
        {
            _abilityDataController = new AbilityDataController(
                OnTimeSpeedUp, OnTimeSlowDown,
                zoomInData,zoomOutData);
            _abilityDataController.OnAbilityStarted += AbilitiesData_OnAbilityStartedHandler;
            _abilityDataController.OnSequenceEnd += AbilitiesData_OnEndQueueFinished;
            
            _abilityDataController.Initialize(abilityTimeData);
        }
        
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case AbilityObserverMessage.AddAbility:
                    HandleAddAbility((int)args[0],(Vector2)args[1]);
                    break;
                case AbilityObserverMessage.TriggerAbility:
                    HandleTriggerAbility((int)args[0]);
                    break;
                case AbilityObserverMessage.ForceFinish:
                    HandleForceFinish();
                    break;
                case AbilityObserverMessage.RestartValues:
                    HandleRestartValues();
                    break;
            }
        }

        #region Observer Handlers

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
            
            OnAbilityAdded?.Invoke();
        }

        
        private void HandleTriggerAbility(int abilityIndex)
        {
            var abilityType =  (AbilityType)abilityIndex;
            
            if (_abilityDataController.HasAbilityData(abilityType) == false)
            {
                throw new Exception("Ability data does not exists");
            }

            var abilitySequence = _abilityDataController.GetActionQueue(abilityType);
            _actionId = ActionManager.Add(abilitySequence,ActionManager.UpdateType.Update);
            
            OnAbilityTriggered?.Invoke();
        }
        
        private void HandleForceFinish()
        {
            if (_actionId != null && _actionId.IsActive)
            {
                ActionManager.Remove(_actionId);
            }
        }
        
        private void HandleRestartValues()
        {
            OnRestart?.Invoke();
        }

        #endregion
        

        private void AbilitiesData_OnEndQueueFinished(AbilityType abilityType)
        {
            OnAbilityFinished?.Invoke();
        }

        private void AbilitiesData_OnAbilityStartedHandler(float time)
        {
            
        }
        
    }
}