using System.Collections.Generic;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
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
            var configData = GameConfigManager.Instance.GetGameplayData().AbilityTimeData;
            
            CreateSlowMotionData(configData);
            CreateShieldData(configData);
            CreateHealData(configData);
            CreateDoublePointsData(configData);
            CreateAutomaticData(configData);
        }

        #region Abilities Data

        private void CreateShieldData(IAbilityTimeConfigData configData)
        {
            var minTimeScale = 0.025f;
            var selectedAbility = AbilityType.SuperShield;
            var timeData = configData.GetAbilityTimeData(selectedAbility);

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
                        TimeToUpdate = timeData.ZoomIn,
                    });
                }, 0f),
                new ActionData(CameraZoomIn,
                    timeData.ZoomIn),
                new ActionData(() => { _eventBus.Publish(new ShieldEvents.EnableSuperShield());},
                    timeData.StartAction),
                new ActionData(() =>
                {
                    CameraZoomOut();
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects },
                        TargetTimeScale = 1f,
                        CurrentTimeScale = minTimeScale,
                        TimeToUpdate = timeData.SpeedUp,
                    });
                    _eventBus.Publish(new MeteorEvents.SpawnRing());
                }, timeData.ZoomOut),
                new ActionData(() =>
                {
                    _eventBus.Publish(new MeteorEvents.SpawnRing());
                }, timeData.SpeedUp),
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
                        TimeToUpdate = timeData.StopAction
                    });
                }, 0f),
                new ActionData(() =>
                    {
                        _eventBus.Publish(new ShieldEvents.EnableNormalShield());
                        _updateTimeScale.Invoke(new TimeScaleData
                        {
                            UpdateGroups = new UpdateGroup[] { UpdateGroup.Gameplay, UpdateGroup.Effects },
                            TargetTimeScale = 1f,
                            CurrentTimeScale = minTimeScale,
                            TimeToUpdate = timeData.SpeedUp,
                        });
                    },
                    timeData.StopAction),
                new ActionData(() =>
                {
                    _eventBus.Publish(new InputsEvents.SetEnable { IsEnable = true });
                    PublishAbilityActive(selectedAbility, false);
                    OnAbilityFinished?.Invoke(selectedAbility);
                }, timeData.SpeedUp),
            };

            var shieldData = new AbilityStoredData
            {
                ActiveTime = timeData.ActiveTime,
                AbilityType = selectedAbility,
                StartActions = startActions,
                EndActions = endActions,
            };

            _abilities.Add(selectedAbility, shieldData);
        }

        private void CreateHealData(IAbilityTimeConfigData configData)
        {
            var minTimeScale = 0.025f;
            var shieldTimeScale = 0.75f;
            var selectedAbility = AbilityType.Health;
            var timeData = configData.GetAbilityTimeData(selectedAbility);
            
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
                        TimeToUpdate = timeData.ZoomIn,
                    });
                    
                }, 0f),
                new ActionData(() => { _eventBus.Publish(new CameraEvents.ZoomIn()); },
                    timeData.ZoomIn),
                new ActionData(() => { _eventBus.Publish(new EarthEvents.Heal()); },
                    timeData.StartAction),
                new ActionData(() => { _eventBus.Publish(new CameraEvents.ZoomOut()); },
                    timeData.ZoomOut),
                new ActionData(() =>
                {
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                        TargetTimeScale = 1f,
                        CurrentTimeScale = minTimeScale,
                        TimeToUpdate = timeData.SpeedUp,
                    });
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Shield},
                        TargetTimeScale = 1f,
                        CurrentTimeScale = shieldTimeScale,
                        TimeToUpdate = timeData.SpeedUp,
                    });
                    
                    _eventBus.Publish(new EarthEvents.SetEnableDamage{DamageEnable = true});
                    RunActiveTimer(selectedAbility);
                }, timeData.SpeedUp),
                new ActionData(() =>
                {
                    _eventBus.Publish(new EarthEvents.SetEnableDamage{DamageEnable = true});
                    RunActiveTimer(selectedAbility);
                }, timeData.SpeedUp),
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

        private void CreateSlowMotionData(IAbilityTimeConfigData configData)
        {
            var minTimeScale = 0.5f;
            var selectedAbility = AbilityType.SlowMotion;
            var timeData = configData.GetAbilityTimeData(selectedAbility);

            //Start Queue
            var startActions = new[]
            {
                new ActionData(() =>
                {
                    PublishAbilityActive(selectedAbility, true);
                    
                    CustomTime.SetChannelTimeScale(UpdateGroup.Gameplay, 0.15f);
                    
                    CameraZoomIn();
                    
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Shield},
                        TargetTimeScale = 0.85f,
                        CurrentTimeScale = 1f,
                        TimeToUpdate = timeData.SlowDown,
                    });
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] 
                        { 
                            UpdateGroup.Gameplay, UpdateGroup.Camera
                        },
                        TargetTimeScale = minTimeScale,
                        CurrentTimeScale = 1f,
                        TimeToUpdate = timeData.SlowDown,
                    });
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] 
                        { 
                            UpdateGroup.Earth, UpdateGroup.Effects, 
                        },
                        TargetTimeScale = minTimeScale/2,
                        CurrentTimeScale = 1,
                        TimeToUpdate = timeData.SlowDown,
                    });
                }, 0f),
                new ActionData(null,
                    timeData.SlowDown),
                new ActionData(CameraZoomOut,
                    timeData.ZoomOut),
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
                    CustomTime.SetChannelPaused(new [] 
                    { 
                        UpdateGroup.Gameplay, 
                            
                    }, true);
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Shield},
                        TargetTimeScale = 1f,
                        CurrentTimeScale = 0.85f,
                        TimeToUpdate = timeData.SpeedUp,
                    });
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] 
                        { 
                            UpdateGroup.Gameplay, UpdateGroup.Camera
                        },
                        TargetTimeScale = 1f,
                        CurrentTimeScale = minTimeScale,
                        TimeToUpdate = timeData.SpeedUp
                    });
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] 
                        { 
                            UpdateGroup.Earth, UpdateGroup.Effects, 
                        },
                        TargetTimeScale = 1f,
                        CurrentTimeScale = minTimeScale/2,
                        TimeToUpdate = timeData.SpeedUp,
                    });
                    
                }, 0f),
                new ActionData(null, timeData.SpeedUp),
                new ActionData(CameraZoomOut,
                    timeData.ZoomOut),
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
                ActiveTime = timeData.ActiveTime,
                AbilityType = selectedAbility,
                StartActions = startActions,
                EndActions = endActions,
            };

            _abilities.Add(selectedAbility, abilityData);
        }

        private void CreateDoublePointsData(IAbilityTimeConfigData configData)
        {
            float targetTimeScale = 0.025f;
            var selectedAbility = AbilityType.DoublePoints;
            var timeData = configData.GetAbilityTimeData(selectedAbility);
            
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
                        TimeToUpdate = timeData.StartAction,
                    });
                    CameraZoomIn();
                },0f),
                new ActionData(() =>
                {
                    GameManager.Instance.EventManager.Publish(new ShieldEvents.SetGold{IsActive = true});
                },timeData.StartAction),
                new ActionData(() =>
                {
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                        TargetTimeScale = 1f,
                        CurrentTimeScale = targetTimeScale,
                        TimeToUpdate = timeData.ZoomOut,
                    });
                    CameraZoomOut();
                    RunActiveTimer(selectedAbility);
                },timeData.ZoomOut),
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
                ActiveTime = timeData.ActiveTime,
                AbilityType = selectedAbility,
                StartActions = startActions,
                EndActions = endActions,
            };

            _abilities.Add(selectedAbility, abilityData);
        }
        
        private void CreateAutomaticData(IAbilityTimeConfigData configData)
        {
            float targetTimeScale = 0.025f;
            var selectedAbility = AbilityType.Automatic;
            var timeData = configData.GetAbilityTimeData(selectedAbility);
            
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
                        TimeToUpdate = timeData.StartAction,
                    });
                    CameraZoomIn();
                },0f),
                new ActionData(() =>
                {
                    GameManager.Instance.EventManager.Publish(new ShieldEvents.SetAutomatic{IsActive = true});
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                        TargetTimeScale = 1f,
                        CurrentTimeScale = targetTimeScale,
                        TimeToUpdate = timeData.SpeedUp,
                    });
                    
                },timeData.StartAction),
                new ActionData(() =>
                {
                    CameraZoomOut();
                    RunActiveTimer(selectedAbility);
                },timeData.SpeedUp),
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
                        TimeToUpdate = timeData.SlowDown,
                    });
                }),
                new ActionData(() =>
                {
                    GameManager.Instance.EventManager.Publish(new ShieldEvents.SetAutomatic{IsActive = false});
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                        TargetTimeScale = 1f,
                        CurrentTimeScale = targetTimeScale,
                        TimeToUpdate = timeData.SpeedUp,
                    });
                },timeData.SlowDown),
                new ActionData(() =>
                {
                    CameraZoomOut();
                    PublishAbilityActive(selectedAbility, false);
                    OnAbilityFinished?.Invoke(selectedAbility);
                },timeData.SpeedUp),
            };

            var abilityData = new AbilityStoredData
            {
                ActiveTime = timeData.ActiveTime,
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