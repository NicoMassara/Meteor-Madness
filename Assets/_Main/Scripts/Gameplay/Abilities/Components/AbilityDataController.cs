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
        private readonly Dictionary<AbilityType, AbilityStoredData> _abilities = new Dictionary<AbilityType, AbilityStoredData>();
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
            CreateDoublePointsData();
            CreateAutomaticData();
        }

        #region Abilities Data

        private void CreateShieldData()
        {
            var minTimeScale = 0.025f;
            var selectedAbility = AbilityType.SuperShield;

            //Start Queue
            var startActions = new[]
            {
                new ActionData(() =>
                {
                    PublishAbilityActive(selectedAbility, true);
                    _eventBus.Publish(new InputsEvents.SetEnable { IsEnable = false });
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects },
                        TargetTimeScale = minTimeScale,
                        CurrentTimeScale = 1.0f,
                        TimeToUpdate = AbilityParameters.SuperShield.StartValues.TimeToZoomIn,
                    });
                }, 0f),
                new ActionData(CameraZoomIn,
                    AbilityParameters.SuperShield.StartValues.TimeToZoomIn),
                new ActionData(() => { _eventBus.Publish(new ShieldEvents.EnableSuperShield());},
                    AbilityParameters.SuperShield.StartValues.TimeToMoveFastShield),
                new ActionData(CameraZoomOut,
                    AbilityParameters.SuperShield.StartValues.TimeToZoomOut),
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
                }, AbilityParameters.SuperShield.StartValues.TimeBeforeIncreasingTimeScale),
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
                        TimeToUpdate = AbilityParameters.SuperShield.EndValues.TimeBeforeDisableSuperShield
                    });
                }, 0f),
                new ActionData(() => { _eventBus.Publish(new ShieldEvents.EnableNormalShield()); },
                    AbilityParameters.SuperShield.EndValues.TimeBeforeDisableSuperShield),
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
                    
                    PublishAbilityActive(selectedAbility, false);
                    OnAbilityFinished?.Invoke(selectedAbility);
                }, AbilityParameters.SuperShield.EndValues.TimeBeforeRestoringTimeScale),
            };

            var shieldData = new AbilityStoredData
            {
                ActiveTime = AbilityParameters.SuperShield.ActiveTime,
                AbilityType = selectedAbility,
                StartActions = startActions,
                EndActions = endActions,
            };

            _abilities.Add(selectedAbility, shieldData);
        }

        private void CreateHealData()
        {
            var minTimeScale = 0.025f;
            var shieldTimeScale = 0.75f;
            var timeToFinish = 0.25f;
            var selectedAbility = AbilityType.Health;
            
            //Start Queue
            var startActions = new[]
            {
                new ActionData(() =>
                {
                    PublishAbilityActive(selectedAbility, true);
                    _eventBus.Publish(new EarthEvents.SetEnableDamage{DamageEnable = false});
                    CustomTime.SetChannelTimeScale(UpdateGroup.Shield, shieldTimeScale);
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new[] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                        TargetTimeScale = minTimeScale,
                        CurrentTimeScale = 1.0f,
                        TimeToUpdate = AbilityParameters.Heal.StartValues.TimeToZoomIn,
                    });
                    
                }, 0f),
                new ActionData(() => { _eventBus.Publish(new CameraEvents.ZoomIn()); },
                    AbilityParameters.Heal.StartValues.TimeToZoomIn),
                new ActionData(() => { _eventBus.Publish(new EarthEvents.Heal()); },
                    AbilityParameters.Heal.StartValues.TimeToHeal),
                new ActionData(() => { _eventBus.Publish(new CameraEvents.ZoomOut()); },
                    AbilityParameters.Heal.StartValues.TimeToZoomOut),
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
                    
                    _eventBus.Publish(new EarthEvents.SetEnableDamage{DamageEnable = true});
                    RunActiveTimer(selectedAbility);
                }, AbilityParameters.Heal.StartValues.TimeBeforeIncreasingTimeScale),
            };

            var endActions = new[]
            {
                new ActionData(() =>
                {
                    OnAbilityFinished?.Invoke(selectedAbility);
                    PublishAbilityActive(selectedAbility, false);
                })
            };
            
            var healData = new AbilityStoredData
            {
                AbilityType = selectedAbility,
                StartActions = startActions,
                EndActions = endActions,
                HasInstantEffect = true
            };
            
            _abilities.Add(selectedAbility, healData);
        }

        private void CreateSlowMotionData()
        {
            var minTimeScale = 0.5f;
            var selectedAbility = AbilityType.SlowMotion;

            //Start Queue
            var startActions = new[]
            {
                new ActionData(() =>
                {
                    PublishAbilityActive(selectedAbility, true);
                    
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
                        TimeToUpdate = AbilityParameters.SlowMotion.StartValues.TimeToSlowDown,
                    });
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] 
                        { 
                            UpdateGroup.Gameplay, UpdateGroup.Camera
                        },
                        TargetTimeScale = minTimeScale,
                        CurrentTimeScale = 1f,
                        TimeToUpdate = AbilityParameters.SlowMotion.StartValues.TimeToSlowDown,
                    });
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] 
                        { 
                            UpdateGroup.Earth, UpdateGroup.Effects, 
                        },
                        TargetTimeScale = minTimeScale/2,
                        CurrentTimeScale = 1,
                        TimeToUpdate = AbilityParameters.SlowMotion.StartValues.TimeToSlowDown,
                    });
                }, AbilityParameters.SlowMotion.StartValues.TimeToSlowDown),
                new ActionData(CameraZoomOut,
                    AbilityParameters.SlowMotion.StartValues.TimeToZoomOut),
                new ActionData(() =>
                {
                    CustomTime.SetChannelTimeScale(UpdateGroup.Gameplay, minTimeScale);
                    CustomTime.SetChannelPaused(new [] 
                    { 
                        UpdateGroup.Gameplay, 
                            
                    }, false);
                    RunActiveTimer(selectedAbility);
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
                        TimeToUpdate = AbilityParameters.SlowMotion.EndValues.TimeToSpeedUp,
                    });
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] 
                        { 
                            UpdateGroup.Gameplay, UpdateGroup.Camera
                        },
                        TargetTimeScale = 1f,
                        CurrentTimeScale = minTimeScale,
                        TimeToUpdate = AbilityParameters.SlowMotion.EndValues.TimeToSpeedUp
                    });
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] 
                        { 
                            UpdateGroup.Earth, UpdateGroup.Effects, 
                        },
                        TargetTimeScale = 1f,
                        CurrentTimeScale = minTimeScale/2,
                        TimeToUpdate = AbilityParameters.SlowMotion.EndValues.TimeToSpeedUp,
                    });
                }, AbilityParameters.SlowMotion.EndValues.TimeToSpeedUp),
                new ActionData(CameraZoomOut,
                    AbilityParameters.SlowMotion.EndValues.TimeToZoomOut),
                new ActionData(() =>
                {
                    CustomTime.SetChannelPaused(new [] 
                    { 
                        UpdateGroup.Gameplay, 
                            
                    }, false);
                    PublishAbilityActive(selectedAbility, false);
                    OnAbilityFinished?.Invoke(selectedAbility);
                }),
            };

            var abilityData = new AbilityStoredData
            {
                ActiveTime = AbilityParameters.SlowMotion.ActiveTime,
                AbilityType = selectedAbility,
                StartActions = startActions,
                EndActions = endActions,
            };

            _abilities.Add(selectedAbility, abilityData);
        }

        private void CreateDoublePointsData()
        {
            float targetTimeScale = 0.025f;
            var selectedAbility = AbilityType.DoublePoints;
            
            //Start Queue
            var startActions = new[]
            {
                new ActionData(() =>
                {
                    PublishAbilityActive(selectedAbility, true);
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                        TargetTimeScale = targetTimeScale,
                        CurrentTimeScale = 1f,
                        TimeToUpdate = AbilityParameters.DoublePoints.StartValues.TimeToGoldShield,
                    });
                    CameraZoomIn();
                },0f),
                new ActionData(() =>
                {
                    GameManager.Instance.EventManager.Publish(new ShieldEvents.SetGold{IsActive = true});
                },AbilityParameters.DoublePoints.StartValues.TimeToGoldShield),
                new ActionData(() =>
                {
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                        TargetTimeScale = 1f,
                        CurrentTimeScale = targetTimeScale,
                        TimeToUpdate = AbilityParameters.DoublePoints.StartValues.TimeToZoomOut,
                    });
                    CameraZoomOut();
                    RunActiveTimer(selectedAbility);
                },AbilityParameters.DoublePoints.StartValues.TimeToZoomOut),
            };

            //End Queue
            var endActions = new[]
            {
                new ActionData(() =>
                {
                    GameManager.Instance.EventManager.Publish(new ShieldEvents.SetGold{IsActive = false});
                    PublishAbilityActive(selectedAbility, false);
                    OnAbilityFinished?.Invoke(selectedAbility);
                }),
            };

            var abilityData = new AbilityStoredData
            {
                ActiveTime = AbilityParameters.DoublePoints.ActiveTime,
                AbilityType = selectedAbility,
                StartActions = startActions,
                EndActions = endActions,
            };

            _abilities.Add(selectedAbility, abilityData);
        }
        
        private void CreateAutomaticData()
        {
            float targetTimeScale = 0.025f;
            var selectedAbility = AbilityType.Automatic;
            
            //Start Queue
            var startActions = new[]
            {
                new ActionData(() =>
                {
                    PublishAbilityActive(selectedAbility, true);
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                        TargetTimeScale = targetTimeScale,
                        CurrentTimeScale = 1f,
                        TimeToUpdate = 0.25f,
                    });
                    CameraZoomIn();
                },0f),
                new ActionData(() =>
                {
                    GameManager.Instance.EventManager.Publish(new ShieldEvents.SetAutomatic{IsActive = true});
                },0.25f),
                new ActionData(() =>
                {
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                        TargetTimeScale = 1f,
                        CurrentTimeScale = targetTimeScale,
                        TimeToUpdate = 0.25f,
                    });
                    CameraZoomOut();
                    RunActiveTimer(selectedAbility);
                },0.25f),
            };

            //End Queue
            var endActions = new[]
            {
                new ActionData(() =>
                {
                    CameraZoomIn();
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                        TargetTimeScale = targetTimeScale,
                        CurrentTimeScale = 1f,
                        TimeToUpdate = 0.5f,
                    });
                }),
                new ActionData(() =>
                {
                    GameManager.Instance.EventManager.Publish(new ShieldEvents.SetAutomatic{IsActive = false});
                },0.5f),
                new ActionData(() =>
                {
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                        TargetTimeScale = 1f,
                        CurrentTimeScale = targetTimeScale,
                        TimeToUpdate = 0.5f,
                    });
                    CameraZoomOut();
                    PublishAbilityActive(selectedAbility, false);
                    OnAbilityFinished?.Invoke(selectedAbility);
                },0.5f),
            };

            var abilityData = new AbilityStoredData
            {
                ActiveTime = AbilityParameters.DoublePoints.ActiveTime,
                AbilityType = selectedAbility,
                StartActions = startActions,
                EndActions = endActions,
            };

            _abilities.Add(selectedAbility, abilityData);
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

    public class AbilityStoredData
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