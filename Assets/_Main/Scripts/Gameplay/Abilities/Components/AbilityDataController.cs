using System;
using System.Collections.Generic;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers;
using NicolasMassara.CustomUpdateManager;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityDataController
    {
        private readonly Dictionary<AbilityType, AbilityStoredData> _abilities = new Dictionary<AbilityType, AbilityStoredData>();
        
        public UnityAction<float> OnAbilityStarted;
        public UnityAction<AbilityType> OnStartQueueStarted;
        public UnityAction<AbilityType> OnStartQueueFinished;
        public UnityAction<AbilityType> OnEndQueueStart;
        public UnityAction<AbilityType> OnEndQueueFinished;
        public UnityAction<TimeScaleData> _updateTimeScale;
        private event Action _playSpeedTimeSound;
        private event Action _playSlowTimeSound;

        public AbilityDataController(UnityAction<TimeScaleData> updateTimeScale, 
            Action speedTimeSound, Action slowTimeSound)
        {
            _updateTimeScale = updateTimeScale;
            _playSpeedTimeSound += speedTimeSound;
            _playSlowTimeSound += slowTimeSound;

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
                    OnStartQueueStarted?.Invoke(selectedAbility);
                    PublishAbilityActive(selectedAbility, true);
                    SetInputsEnable(false);
                    SetEnableAbilityUI(false);
                    
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects },
                        TargetTimeScale = minTimeScale,
                        CurrentTimeScale = 1.0f,
                        TimeToUpdate = timeData.ZoomIn,
                    });

                    PlaySlowTimeSound();
                }, 0f),
                new ActionData(() =>
                {
                    CameraZoomIn();
                }, timeData.ZoomIn),
                new ActionData(() =>
                    {
                        ShieldEventCaller.EnableSuperShield();
                    },
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
                    MeteorEventCaller.SpawnRing();
                }, timeData.ZoomOut),
                new ActionData(() =>
                {
                    OnStartQueueFinished?.Invoke(selectedAbility);
                    SetEnableAbilityUI(true);
                }, timeData.SpeedUp),
            };

            //End Queue
            var endActions = new[]
            {
                new ActionData(() =>
                {
                    OnEndQueueStart?.Invoke(selectedAbility);
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new UpdateGroup[] { UpdateGroup.Gameplay, UpdateGroup.Effects },
                        TargetTimeScale = minTimeScale,
                        CurrentTimeScale = 1.0f,
                        TimeToUpdate = timeData.StopAction
                    });
                    PlaySpeedTimeSound();
                }, 0f),
                new ActionData(() =>
                {
                    ShieldEventCaller.EnableNormalShield();
                }, timeData.StopAction),
                new ActionData(() =>
                {
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new UpdateGroup[] { UpdateGroup.Gameplay, UpdateGroup.Effects },
                        TargetTimeScale = 1f,
                        CurrentTimeScale = minTimeScale,
                        TimeToUpdate = timeData.SpeedUp,
                    });
                }, timeData.SpeedUp),
                new ActionData(() =>
                {
                    SetInputsEnable(true);
                    PublishAbilityActive(selectedAbility, false);
                    OnEndQueueFinished?.Invoke(selectedAbility);
                }, 0f),
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
                    OnStartQueueFinished?.Invoke(selectedAbility);
                    PublishAbilityActive(selectedAbility, true);
                    EarthEventCaller.SetEnableDamage(false);
                    CustomTime.SetChannelTimeScale(UpdateGroup.Shield, shieldTimeScale);
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new[] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                        TargetTimeScale = minTimeScale,
                        CurrentTimeScale = 1.0f,
                        TimeToUpdate = timeData.ZoomIn,
                    });
                    
                    SetInputsEnable(false);
                    SetEnableAbilityUI(false);
                    PlaySlowTimeSound();
                    
                }, 0f),
                new ActionData(() =>
                    {
                       CameraZoomIn();
                    },
                    timeData.ZoomIn),
                new ActionData(() =>
                    {
                        EarthEventCaller.Heal();
                    },
                    timeData.StartAction),
                new ActionData(() =>
                    {
                        CameraZoomOut();
                    },
                    timeData.ZoomOut),
                new ActionData(() =>
                {
                    SetInputsEnable(true);
                    SetEnableAbilityUI(true);
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
                    
                    PlaySpeedTimeSound();
                    
                    RunActiveTimer(selectedAbility);
                }, timeData.SpeedUp),
                new ActionData(() =>
                {
                    EarthEventCaller.SetEnableDamage(true);
                    RunActiveTimer(selectedAbility);
                    OnStartQueueFinished?.Invoke(selectedAbility);
                }, timeData.SpeedUp),
            };

            var endActions = new[]
            {
                new ActionData(() =>
                {
                    OnStartQueueStarted?.Invoke(selectedAbility);
                    OnEndQueueFinished?.Invoke(selectedAbility);
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
                    OnStartQueueStarted?.Invoke(selectedAbility);
                    PublishAbilityActive(selectedAbility, true);
                    
                    CustomTime.SetChannelTimeScale(UpdateGroup.Gameplay, 0.15f);
                    SetEnableAbilityUI(false);
                    SetInputsEnable(false);
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
                    
                    PlaySlowTimeSound();
                }, 0f),
                new ActionData(()=> ShieldEventCaller.SetSlow(true),
                    timeData.SlowDown),
                new ActionData(() =>
                {
                    CameraZoomOut();
                },timeData.ZoomOut),
                new ActionData(() =>
                {
                    SetEnableAbilityUI(true);
                    SetInputsEnable(true);
                    CustomTime.SetChannelTimeScale(UpdateGroup.Gameplay, minTimeScale);
                    CustomTime.SetChannelPaused(new [] 
                    { 
                        UpdateGroup.Gameplay, 
                            
                    }, false);
                    RunActiveTimer(selectedAbility);
                    OnStartQueueFinished?.Invoke(selectedAbility);
                }),
            };

            //End Queue
            var endActions = new[]
            {
                new ActionData(() =>
                {
                    OnEndQueueFinished?.Invoke(selectedAbility);
                    SetEnableAbilityUI(false);
                    SetInputsEnable(false);
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
                    
                    PlaySpeedTimeSound();
                    
                }, 0f),
                new ActionData(()=> ShieldEventCaller.SetSlow(false), timeData.SpeedUp),
                new ActionData(() =>
                    {
                        CameraZoomOut();
                    },
                    timeData.ZoomOut),
                new ActionData(() =>
                {
                    SetEnableAbilityUI(true);
                    SetInputsEnable(true);
                    CustomTime.SetChannelPaused(new [] 
                    { 
                        UpdateGroup.Gameplay, 
                            
                    }, false);
                    PublishAbilityActive(selectedAbility, false);
                    OnEndQueueFinished?.Invoke(selectedAbility);
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
                    OnStartQueueStarted?.Invoke(selectedAbility);
                    SetEnableAbilityUI(false);
                    SetInputsEnable(false);
                    PublishAbilityActive(selectedAbility, true);
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                        TargetTimeScale = targetTimeScale,
                        CurrentTimeScale = 1f,
                        TimeToUpdate = timeData.StartAction,
                    });
                    CameraZoomIn();
                    
                    PlaySlowTimeSound();
                },0f),
                new ActionData(() =>
                {
                    ShieldEventCaller.SetGold(true);
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
                    
                    PlaySpeedTimeSound();
                    CameraZoomOut();
                    RunActiveTimer(selectedAbility);
                },timeData.ZoomOut),
                new ActionData(() =>
                {
                    SetEnableAbilityUI(true);
                    SetInputsEnable(true);
                    OnStartQueueFinished?.Invoke(selectedAbility);
                })
            };

            //End Queue
            var endActions = new[]
            {
                new ActionData(() =>
                {
                    OnEndQueueStart?.Invoke(selectedAbility);
                    ShieldEventCaller.SetGold(false);
                    PublishAbilityActive(selectedAbility, false);
                    OnEndQueueFinished?.Invoke(selectedAbility);
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
                    OnStartQueueStarted?.Invoke(selectedAbility);
                    PublishAbilityActive(selectedAbility, true);
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                        TargetTimeScale = targetTimeScale,
                        CurrentTimeScale = 1f,
                        TimeToUpdate = timeData.StartAction,
                    });
                    CameraZoomIn();
                    SetEnableAbilityUI(false);
                    SetInputsEnable(false);
                    
                    PlaySlowTimeSound();
                },0f),
                new ActionData(() =>
                {
                    ShieldEventCaller.SetAutomatic(true);
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                        TargetTimeScale = 1f,
                        CurrentTimeScale = targetTimeScale,
                        TimeToUpdate = timeData.SpeedUp,
                    });
                    
                    PlaySpeedTimeSound();
                    
                },timeData.StartAction),
                new ActionData(() =>
                {
                    CameraZoomOut();
                    RunActiveTimer(selectedAbility);
                },timeData.SpeedUp),
                new ActionData(() =>
                {
                    SetEnableAbilityUI(true);
                    SetInputsEnable(true);
                    OnStartQueueFinished?.Invoke(selectedAbility);
                })
            };

            //End Queue
            var endActions = new[]
            {
                new ActionData(() =>
                {
                    OnEndQueueStart?.Invoke(selectedAbility);
                    CameraZoomIn();
                    SetEnableAbilityUI(false);
                    SetInputsEnable(false);
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                        TargetTimeScale = targetTimeScale,
                        CurrentTimeScale = 1f,
                        TimeToUpdate = timeData.SlowDown,
                    });
                    PlaySlowTimeSound();
                }),
                new ActionData(() =>
                {
                    ShieldEventCaller.SetAutomatic(false);
                    _updateTimeScale.Invoke(new TimeScaleData
                    {
                        UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                        TargetTimeScale = 1f,
                        CurrentTimeScale = targetTimeScale,
                        TimeToUpdate = timeData.SpeedUp,
                    });
                    
                    PlaySpeedTimeSound();
                },timeData.SlowDown),
                new ActionData(() =>
                {
                    CameraZoomOut();
                    PublishAbilityActive(selectedAbility, false);

                },timeData.SpeedUp),
                new ActionData(() =>
                {
                    SetEnableAbilityUI(true);
                    SetInputsEnable(true);
                    OnEndQueueFinished?.Invoke(selectedAbility);
                })
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
        
        
        private void SetInputsEnable(bool isEnable)
        {
            InputsEventCaller.SetEnable(isEnable);
#if UNITY_ANDROID || UNITY_IOS
            InputsEventCaller.SetUIEnable(isEnable);
#endif
        }

        private void SetEnableAbilityUI(bool isEnable)
        {
            AbilitiesEventCaller.SetEnableUI(isEnable);
        }

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
                new AbilitiesEvents.NotifyIsActive{ AbilityType = abilityType, IsActive = isActive });
        }
        
        private void PlaySpeedTimeSound()
        {
            _playSpeedTimeSound?.Invoke();
        }

        private void PlaySlowTimeSound()
        {
            _playSlowTimeSound?.Invoke();
        }

    }

    public struct TimeScaleData
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