using System.Collections.Generic;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityDataController
    {
        private readonly Dictionary<AbilityType, AbilityData> _abilities = new Dictionary<AbilityType, AbilityData>();
        private readonly EventBusManager _eventBus;

        public UnityAction<float> OnAbilityStarted;
        public UnityAction OnAbilityFinished;

        public AbilityDataController(EventBusManager eventBus)
        {
            _eventBus = eventBus;
            
            CreateAbilityData();
        }

        #region Set Data

        private void CreateShieldData()
        {
            //Start Queue
            var startActions = new []
            {
                new ActionData(() =>
                {
                    //Debug.Log("Disable Inputs");
                    _eventBus.Publish(new SetEnableInputs{IsEnabled = false});
                }, 0f),
                new ActionData(() =>
                {
                    //Debug.Log("Gameplay Time Scale Set To 0");
                    CustomTime.GetChannel(UpdateGroup.Gameplay).TimeScale = 0;
                }, SuperShieldStartTimeValues.TimeBeforeDecreasingTimeScale),
                new ActionData(() =>
                {
                    //Debug.Log("Camera Zoom In");
                    _eventBus.Publish(new CameraZoomIn());
                }, SuperShieldStartTimeValues.TimeToZoomIn),
                new ActionData(() =>
                {
                    //Debug.Log("Enable Super Shield");
                    _eventBus.Publish(new SetSuperShield{});
                }, SuperShieldStartTimeValues.TimeToMoveFastShield),
                new ActionData(() =>
                {
                    //Debug.Log("Gameplay Time Scale Set To 1");
                    CustomTime.GetChannel(UpdateGroup.Gameplay).TimeScale = 1;
                    _eventBus.Publish(new SpawnRingMeteor());
                }, SuperShieldStartTimeValues.TimeBeforeIncreasingTimeScale),
                new ActionData(() =>
                {
                    //Debug.Log("Camera Zoom Out");
                    _eventBus.Publish(new CameraZoomOut());
                }, SuperShieldStartTimeValues.TimeToZoomOut),
                new ActionData(() =>
                {
                    var activeTime = _abilities[AbilityType.SuperShield].ActiveTime;
                    OnAbilityStarted?.Invoke(activeTime);
                }),
            };
            
            //End Queue
            var endActions = new []
            {
                new ActionData(() =>
                {
                    //Debug.Log("Gameplay Time Scale Set To 0");
                    CustomTime.GetChannel(UpdateGroup.Gameplay).TimeScale = 0;
                }, 0f),
                new ActionData(() =>
                {
                    //Debug.Log("Normal Shield Set");
                    _eventBus.Publish(new SetNormalShield());
                }, SuperShieldEndTimeValues.TimeBeforeDisableSuperShield),
                new ActionData(() =>
                {
                    //Debug.Log("Inputs Enable");
                    _eventBus.Publish(new SetEnableInputs{IsEnabled = true});
                }, SuperShieldEndTimeValues.TimeBeforeEnableInput),
                new ActionData(() =>
                {
                    //Debug.Log("Gameplay Time Scale Set To 1");
                    CustomTime.GetChannel(UpdateGroup.Gameplay).TimeScale = 1;
                    OnAbilityFinished?.Invoke();
                }, SuperShieldEndTimeValues.TimeBeforeRestoringTimeScale),
            };

            var shieldData = new AbilityData
            {
                ActiveTime = AbilitiesActiveTimeValues.SuperShield,
                AbilityType = AbilityType.SuperShield,
                StartActions = startActions,
                EndActions = endActions,
            };
            
            _abilities.Add(shieldData.AbilityType, shieldData);
        }

        private void CreateAbilityData()
        {
            
            CreateShieldData();
            
            #region SlowMotion

            //TotalShield
            ActionQueue slowQueue = new ActionQueue();
            ActionData[] slowActionData = new []
            {
                new ActionData(() =>
                {
                    //Disable Player Inputs
                    //Stops GameplayTime
                }, 0f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
            };

            #endregion
        }

        #endregion


        public bool HasAbilityData(AbilityType abilityType)
        {
            return _abilities.ContainsKey(abilityType);
        }

        public ActionQueue GetAbilityStartQueue(AbilityType abilityType)
        {
            return _abilities[abilityType].GetStartActionQueue();
        }
        
        public ActionQueue GetAbilityEndQueue(AbilityType abilityType)
        {
            return _abilities[abilityType].GetEndActionQueue();
        }
        
    }
    
    public class AbilityData
    {
        public float ActiveTime;
        public AbilityType AbilityType;
        public ActionData[] StartActions;
        public ActionData[] EndActions;
        

        public ActionQueue GetStartActionQueue()
        {
            return new ActionQueue(StartActions);
        }

        public ActionQueue GetEndActionQueue()
        {
            return new ActionQueue(EndActions);
        }
    }
}