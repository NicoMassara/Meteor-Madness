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
        public UnityAction<AbilityType> OnAbilityFinished;
        public UnityAction<TimeScaleData> _updateTimeScale;

        public AbilityDataController(EventBusManager eventBus, UnityAction<TimeScaleData> updateTimeScale)
        {
            _eventBus = eventBus;
            _updateTimeScale = updateTimeScale;

            CreateAbilityData();
        }


        private void CreateAbilityData()
        {
            CreateSlowMotionData();
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
                    PublishAbilityActive(AbilityType.SuperShield, true);
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
                new ActionData(() => { _eventBus.Publish(new ShieldEvents.EnableSuperShield());},
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
                    RunActiveTimer(AbilityType.SuperShield);
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
                    
                    PublishAbilityActive(AbilityType.SuperShield, false);
                    OnAbilityFinished?.Invoke(AbilityType.SuperShield);
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
            var shieldTimeScale = 0.75f;
            var timeToFinish = 0.25f;
            
            //Start Queue
            var startActions = new[]
            {
                new ActionData(() =>
                {
                    PublishAbilityActive(AbilityType.Health, true);
                    GameManager.Instance.EventManager.Publish(new EarthEvents.SetEnableDamage{DamageEnable = false});
                    CustomTime.SetChannelTimeScale(UpdateGroup.Shield, shieldTimeScale);
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
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                        TargetTimeScale = 1f,
                        CurrentTimeScale = minTimeScale,
                        TimeToUpdate = timeToFinish,
                    });
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Shield},
                        TargetTimeScale = 1f,
                        CurrentTimeScale = shieldTimeScale,
                        TimeToUpdate = timeToFinish,
                    });
                    
                    GameManager.Instance.EventManager.Publish(new EarthEvents.SetEnableDamage{DamageEnable = true});
                    OnAbilityStarted?.Invoke(timeToFinish);
                }, SuperHealTimeValues.TimeBeforeIncreasingTimeScale),
            };

            var endActions = new[]
            {
                new ActionData(() =>
                {
                    OnAbilityFinished?.Invoke(AbilityType.Health);
                    PublishAbilityActive(AbilityType.Health, false);
                })
            };
            
            var healData = new AbilityData
            {
                AbilityType = AbilityType.Health,
                StartActions = startActions,
                EndActions = endActions,
                HasInstantEffect = true
            };
            
            _abilities.Add(healData.AbilityType, healData);
        }

        private void CreateSlowMotionData()
        {
            var minTimeScale = 0.5f;

            //Start Queue
            var startActions = new[]
            {
                new ActionData(() =>
                {
                    PublishAbilityActive(AbilityType.SlowMotion, true);
                    CustomTime.SetChannelTimeScale(UpdateGroup.Gameplay, 0.15f);
                    CameraZoomIn();
                    
                }, 0f),
                new ActionData(() =>
                {
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Shield},
                        TargetTimeScale = 0.85f,
                        CurrentTimeScale = 1f,
                        TimeToUpdate = 0.5f,
                    });
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] 
                        { 
                            UpdateGroup.Gameplay, UpdateGroup.Camera
                        },
                        TargetTimeScale = minTimeScale,
                        CurrentTimeScale = 1f,
                        TimeToUpdate = 0.5f,
                    });
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] 
                        { 
                            UpdateGroup.Earth, UpdateGroup.Effects, 
                        },
                        TargetTimeScale = minTimeScale/2,
                        CurrentTimeScale = 1,
                        TimeToUpdate = 0.5f,
                    });
                }, 0.25f),
                new ActionData(CameraZoomOut,
                    0.75f),
                new ActionData(() =>
                {
                    CustomTime.SetChannelTimeScale(UpdateGroup.Gameplay, minTimeScale);
                    CustomTime.SetChannelPaused(new [] 
                    { 
                        UpdateGroup.Gameplay, 
                            
                    }, false);
                    RunActiveTimer(AbilityType.SlowMotion);
                }),
            };

            //End Queue
            var endActions = new[]
            {
                new ActionData(() =>
                {
                    // ReSharper disable once ConvertClosureToMethodGroup
                    CameraZoomIn();
                    
                }, 0f),
                new ActionData(() =>
                {
                    CustomTime.SetChannelPaused(new [] 
                    { 
                        UpdateGroup.Gameplay, 
                            
                    }, true);
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Shield},
                        TargetTimeScale = 1f,
                        CurrentTimeScale = 0.85f,
                        TimeToUpdate = 0.25f,
                    });
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] 
                        { 
                            UpdateGroup.Gameplay, UpdateGroup.Camera
                        },
                        TargetTimeScale = 1f,
                        CurrentTimeScale = minTimeScale,
                        TimeToUpdate = 0.25f,
                    });
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] 
                        { 
                            UpdateGroup.Earth, UpdateGroup.Effects, 
                        },
                        TargetTimeScale = 1f,
                        CurrentTimeScale = minTimeScale/2,
                        TimeToUpdate = 0.25f,
                    });
                }, 0.25f),
                new ActionData(CameraZoomOut,
                    0.5f),
                new ActionData(() =>
                {
                    CustomTime.SetChannelPaused(new [] 
                    { 
                        UpdateGroup.Gameplay, 
                            
                    }, false);
                    PublishAbilityActive(AbilityType.SlowMotion, false);
                    OnAbilityFinished?.Invoke(AbilityType.SlowMotion);
                }),
            };

            var abilityData = new AbilityData
            {
                ActiveTime = 5F,
                AbilityType = AbilityType.SlowMotion,
                StartActions = startActions,
                EndActions = endActions,
            };

            _abilities.Add(abilityData.AbilityType, abilityData);
        }

        private void CreateDoublePointData()
        {
            //Start Queue
            var startActions = new[]
            {
                new ActionData(() =>
                {
                    PublishAbilityActive(AbilityType.DoublePoints, true);
                    RunActiveTimer(AbilityType.DoublePoints);
                }),
            };

            //End Queue
            var endActions = new[]
            {
                new ActionData(() =>
                {
                    PublishAbilityActive(AbilityType.DoublePoints, false);
                    OnAbilityFinished?.Invoke(AbilityType.SlowMotion);
                }),
            };

            var abilityData = new AbilityData
            {
                ActiveTime = 5f,
                AbilityType = AbilityType.DoublePoints,
                StartActions = startActions,
                EndActions = endActions,
            };

            _abilities.Add(abilityData.AbilityType, abilityData);
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

        public void RunActiveTimer(AbilityType abilityType)
        {
            var activeTime = _abilities[abilityType].ActiveTime;
            OnAbilityStarted?.Invoke(activeTime);
        }

        private void PublishAbilityActive(AbilityType abilityType, bool isActive)
        {
            GameManager.Instance.EventManager.Publish(
                new AbilitiesEvents.SetActive{ AbilityType = abilityType, IsActive = isActive });
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