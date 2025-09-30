using System;
using System.Collections.Generic;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityDataController
    {
        private readonly Dictionary<AbilityType, AbilityData> _abilities = new Dictionary<AbilityType, AbilityData>();
        private readonly EventBusManager _eventBus;

        public UnityAction<float> OnAbilityStarted;
        public UnityAction OnAbilityFinished;
        public UnityAction<TimeScaleData> _updateTimeScale;

        public AbilityDataController(EventBusManager eventBus, UnityAction<TimeScaleData> updateTimeScale)
        {
            _eventBus = eventBus;
            _updateTimeScale = updateTimeScale;

            CreateAbilityData();
        }


        private void CreateAbilityData()
        {
            CreateShieldData();
            CreateHealData();
        }

        #region Abilities Data

        private void CreateShieldData()
        {
            var minTimeScale = 0.025f;

            //Start Queue
            var startActions = new[]
            {
                new ActionData(() =>
                {
                    _eventBus.Publish(new InputsEvents.SetEnable { IsEnable = false });
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects },
                        TargetTimeScale = minTimeScale,
                        CurrentTimeScale = 1.0f,
                        TimeToUpdate = SuperShieldStartTimeValues.TimeToZoomIn,
                    });
                }, 0f),
                new ActionData(CameraZoomIn,
                    SuperShieldStartTimeValues.TimeToZoomIn),
                new ActionData(() => { _eventBus.Publish(new ShieldEvents.EnableSuperShield()); },
                    SuperShieldStartTimeValues.TimeToMoveFastShield),
                new ActionData(CameraZoomOut,
                    SuperShieldStartTimeValues.TimeToZoomOut),
                new ActionData(() =>
                {
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects },
                        TargetTimeScale = 1f,
                        CurrentTimeScale = minTimeScale,
                        TimeToUpdate = 0.25f,
                    });
                    _eventBus.Publish(new MeteorEvents.SpawnRing());
                }, SuperShieldStartTimeValues.TimeBeforeIncreasingTimeScale),
                new ActionData(() =>
                {
                    var activeTime = _abilities[AbilityType.SuperShield].ActiveTime;
                    OnAbilityStarted?.Invoke(activeTime);
                }),
            };

            //End Queue
            var endActions = new[]
            {
                new ActionData(() =>
                {
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new UpdateGroup[] { UpdateGroup.Gameplay, UpdateGroup.Effects },
                        TargetTimeScale = minTimeScale,
                        CurrentTimeScale = 1.0f,
                        TimeToUpdate = SuperShieldEndTimeValues.TimeBeforeDisableSuperShield
                    });
                }, 0f),
                new ActionData(() => { _eventBus.Publish(new ShieldEvents.EnableNormalShield()); },
                    SuperShieldEndTimeValues.TimeBeforeDisableSuperShield),
                new ActionData(() =>
                {
                    _eventBus.Publish(new InputsEvents.SetEnable { IsEnable = true });
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new UpdateGroup[] { UpdateGroup.Gameplay, UpdateGroup.Effects },
                        TargetTimeScale = 1f,
                        CurrentTimeScale = minTimeScale,
                        TimeToUpdate = 0.25f,
                    });
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

        private void CreateHealData()
        {
            var minTimeScale = 0.025f;
            
            //Start Queue
            var startActions = new[]
            {
                new ActionData(() =>
                {
                    GameManager.Instance.EventManager.Publish(new EarthEvents.SetEnableDamage{DamageEnable = false});
                    CustomTime.SetChannelTimeScale(UpdateGroup.Shield, 0.25F);
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new[] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                        TargetTimeScale = minTimeScale,
                        CurrentTimeScale = 1.0f,
                        TimeToUpdate = SuperHealTimeValues.TimeToZoomIn,
                    });
                    
                }, 0f),
                new ActionData(() => { _eventBus.Publish(new CameraEvents.ZoomIn()); },
                    SuperHealTimeValues.TimeToZoomIn),
                new ActionData(() => { _eventBus.Publish(new EarthEvents.Heal()); },
                    SuperHealTimeValues.TimeToHeal),
                new ActionData(() => { _eventBus.Publish(new CameraEvents.ZoomOut()); },
                    SuperHealTimeValues.TimeToZoomOut),
                new ActionData(() =>
                {
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects, UpdateGroup.Shield},
                        TargetTimeScale = 1f,
                        CurrentTimeScale = minTimeScale,
                        TimeToUpdate = 0.25f,
                    });
                    
                    GameManager.Instance.EventManager.Publish(new EarthEvents.SetEnableDamage{DamageEnable = true});
                    OnAbilityStarted?.Invoke(0.1f);
                }, SuperHealTimeValues.TimeBeforeIncreasingTimeScale),
            };
            
            var healData = new AbilityData
            {
                AbilityType = AbilityType.Health,
                StartActions = startActions,
                HasInstantEffect = true
            };
            
            _abilities.Add(healData.AbilityType, healData);
        }


        #endregion


        private void CameraZoomIn()
        {
            GameManager.Instance.EventManager.Publish(new CameraEvents.ZoomIn());
        }

        private void CameraZoomOut()
        {
            GameManager.Instance.EventManager.Publish(new CameraEvents.ZoomOut());
        }

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

        public bool GetHasInstantEffect(AbilityType abilityType)
        {
            return _abilities[abilityType].GetHasInstantEffect();
        }

    }

    public class TimeScaleData
    {
        public UpdateGroup[] UpdateGroups;
        public float TargetTimeScale;
        public float CurrentTimeScale;
        public float TimeToUpdate;
    }

    public class AbilityData
    {
        public float ActiveTime;
        public bool HasInstantEffect;
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

        public bool GetHasInstantEffect()
        {
            return HasInstantEffect;
        }
    }
}