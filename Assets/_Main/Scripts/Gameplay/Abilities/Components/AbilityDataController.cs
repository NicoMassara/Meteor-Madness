using System;
using System.Collections.Generic;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers;
using NicolasMassara.CustomActionManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Abilies
{

    
    public class AbilityDataController
    {
        #region Commands

        private class TriggerAbilitySequenceState : Command
        {
            private readonly AbilityType _ability;
            private readonly Action<AbilityType> _abilityEvent;

            public TriggerAbilitySequenceState(AbilityType ability,
                Action<AbilityType> abilityEvent)
            {
                _ability = ability;
                _abilityEvent = abilityEvent;
            }

            public override ActionStatus OnExecute(float deltaTime)
            {
                _abilityEvent?.Invoke(_ability);
                
                return ActionStatus.Success;
            }
        }

        private class UpdateTimeScaleCommand : Command
        {
            private readonly TimeScaleData _timeScaleData;
            private event Action<TimeScaleData> UpdateTimeScale;
            
            public UpdateTimeScaleCommand(UpdateGroup[] updateGroups, float targetScale, float startScale, float delay,
                Action<TimeScaleData> updateTimeScale)
            {
                _timeScaleData = new TimeScaleData
                {
                    UpdateGroups = updateGroups,
                    TargetTimeScale = targetScale,
                    CurrentTimeScale = startScale,
                    TimeToUpdate = delay,
                };
                UpdateTimeScale = updateTimeScale;
            }
            
            public UpdateTimeScaleCommand(TimeScaleData timeScaleData, Action<TimeScaleData> updateTimeScale)
            {
                _timeScaleData = timeScaleData;
                UpdateTimeScale = updateTimeScale;
            }

            public override ActionStatus OnExecute(float deltaTime)
            {
                UpdateTimeScale?.Invoke(_timeScaleData);
                
                return ActionStatus.Success;
            }
        }

        private class SetBoolAction : IQueueAction
        {
            private readonly Action<bool> _boolAction;
            private readonly bool _boolValue;
            public ActionStatus CurrentStatus { get;} = ActionStatus.Success;

            public SetBoolAction(bool actionBool, Action<bool> boolAction)
            {
                _boolValue = actionBool;
                _boolAction = boolAction;
            }

            public void OnStart()
            {
                _boolAction?.Invoke(_boolValue);
            }

            public ActionStatus OnUpdate(float deltaTime)
            {
                return ActionStatus.Success;
            }

            public void OnInterrupt() { }
            public IQueueAction Copy()
            {
                return new SetBoolAction(_boolValue, _boolAction);
            }
        }

        private class SetChannelTimeScaleAction : IQueueAction
        {
            private readonly float targetTimeScale;
            private readonly UpdateGroup[] _updateGroup;
            
            public ActionStatus CurrentStatus { get; } = ActionStatus.Success;

            public SetChannelTimeScaleAction(float targetTimeScale, UpdateGroup[] updateGroup)
            {
                this.targetTimeScale = targetTimeScale;
                _updateGroup = updateGroup;
            }

            public void OnStart()
            {
                CustomTime.SetChannelTimeScale(_updateGroup, targetTimeScale);
            }

            public ActionStatus OnUpdate(float deltaTime) => ActionStatus.Success;
            public void OnInterrupt() { }
            public IQueueAction Copy()
            {
                return new SetChannelTimeScaleAction(targetTimeScale, _updateGroup);
            }
        }

        private class SetChannelPausedAction : IQueueAction
        {
            private readonly bool _isPaused;
            private readonly UpdateGroup[] _updateGroup;
            
            public ActionStatus CurrentStatus { get; } = ActionStatus.Success;

            public SetChannelPausedAction(bool isPaused, UpdateGroup[] updateGroup)
            {
                _isPaused = isPaused;
                _updateGroup = updateGroup;
            }

            public void OnStart()
            {
                CustomTime.SetChannelPaused(_updateGroup, true);
            }

            public ActionStatus OnUpdate(float deltaTime) => ActionStatus.Success;
            public void OnInterrupt() { }
            public IQueueAction Copy()
            {
                return new SetChannelPausedAction(_isPaused, _updateGroup);
            }
        }

        private class RunAbilityTimerAction : IQueueAction
        {
            private AbilityType _ability;
            private Action<AbilityType> _runAbilityTimer;
            
            public ActionStatus CurrentStatus { get; } = ActionStatus.Success;

            public RunAbilityTimerAction(AbilityType ability, Action<AbilityType> runAbilityTimer)
            {
                _ability = ability;
                _runAbilityTimer = runAbilityTimer;
            }

            public void OnStart()
            {
                _runAbilityTimer?.Invoke(_ability);
            }

            public ActionStatus OnUpdate(float deltaTime) => ActionStatus.Success;

            public void OnInterrupt() {}
            public IQueueAction Copy()
            {
                return new RunAbilityTimerAction(_ability, _runAbilityTimer);
            }
        }

        private class PublishAbilityActiveAction : IQueueAction
        {
            private readonly AbilityType _ability;
            private readonly bool _isActive;
            public ActionStatus CurrentStatus { get; } = ActionStatus.Success;

            public PublishAbilityActiveAction(AbilityType ability, bool isActive)
            {
                _ability = ability;
                _isActive = isActive;
            }

            public void OnStart()
            {
                AbilitiesEventCaller.NotifyIsActive(_ability, _isActive);
            }

            public ActionStatus OnUpdate(float deltaTime) => ActionStatus.Success;

            public void OnInterrupt() {}
            public IQueueAction Copy()
            {
                return new PublishAbilityActiveAction(_ability, _isActive);
            }
        }

        #endregion
        
        private readonly Dictionary<AbilityType, AbilityStoredData> _abilities = new Dictionary<AbilityType, AbilityStoredData>();
        
        public event Action<float> OnAbilityStarted;
        public event Action<AbilityType> OnStartQueueStarted;
        public event Action<AbilityType> OnStartQueueFinished;
        public event Action<AbilityType> OnEndQueueStart;
        public event Action<AbilityType> OnEndQueueFinished;
        private event Action<TimeScaleData> _updateTimeScale;


        #region Commands/Actions

        private readonly IQueueAction _enableInputs;
        private readonly IQueueAction _disableInputs;
        private readonly IQueueAction _enableAbilityUI;
        private readonly IQueueAction _disableAbilityUI;
        
        private readonly IQueueAction _playSlowTimeSound;
        private readonly IQueueAction _playSpeedTimeSound;

        private readonly IQueueAction _cameraZoomIn;
        private readonly IQueueAction _cameraZoomOut;

        #endregion
        

        public AbilityDataController(Action<TimeScaleData> updateTimeScale, 
            Action speedTimeSound, Action slowTimeSound)
        {
            _updateTimeScale = updateTimeScale;
            
            _playSpeedTimeSound = new InstantAction(speedTimeSound);
            _playSlowTimeSound = new InstantAction(slowTimeSound);
            
            _cameraZoomIn = new InstantAction(CameraEventCaller.ZoomIn);
            _cameraZoomOut = new InstantAction(CameraEventCaller.ZoomOut);
            
            _enableInputs = new SetBoolAction(true,SetInputsEnable);
            _disableInputs = new SetBoolAction(false,SetInputsEnable);
            _enableAbilityUI = new SetBoolAction(true,SetEnableAbilityUI);
            _disableAbilityUI = new SetBoolAction(false,SetEnableAbilityUI);

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
        
        #region Shield

        private IQueueAction GetShieldStartSequence(float minTimeScale, IAbilityTimeData timeData)
        {
            var start = new TriggerAbilitySequenceState(AbilityType.SuperShield, OnStartQueueStarted);  
            var end = new TriggerAbilitySequenceState(AbilityType.SuperShield, OnStartQueueFinished);  
            
            
            var slowDownTime = new UpdateTimeScaleCommand(new TimeScaleData
            {
                UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects },
                TargetTimeScale = minTimeScale,
                CurrentTimeScale = 1.0f,
                TimeToUpdate = timeData.SlowDown
            }, _updateTimeScale);
            
            
            var speedUpTime = new UpdateTimeScaleCommand(new TimeScaleData
            {
                UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects },
                TargetTimeScale = minTimeScale,
                CurrentTimeScale = 1.0f,
                TimeToUpdate = timeData.SpeedUp
            }, _updateTimeScale);
            
            return ActionBuilder.Start()
                .Do(new SimpleCommandAction(start))
                .Then(_disableInputs)
                .Then(_disableAbilityUI)
                .Then(new SimpleCommandAction(slowDownTime))
                .Then(_playSlowTimeSound)
                .Then(new WaitSecondsAction(timeData.ZoomIn))
                .Then(_cameraZoomIn)
                .Then(new WaitSecondsAction(timeData.StartAction))
                .Then(new InstantAction(ShieldEventCaller.EnableSuperShield))
                .Then(new InstantAction(MeteorEventCaller.SpawnRing))
                .Then(new WaitSecondsAction(timeData.ZoomOut))
                .Then(_cameraZoomOut)
                .Then(new WaitSecondsAction(timeData.SpeedUp))
                .Then(new SimpleCommandAction(speedUpTime))
                .Then(new SimpleCommandAction(end))
                .Then(_enableAbilityUI)
                .Build();
        }
        private IQueueAction GetShieldEndSequence(float minTimeScale, IAbilityTimeData timeData)
        {
            var start = new TriggerAbilitySequenceState(AbilityType.SuperShield, OnEndQueueStart);  
            var end = new TriggerAbilitySequenceState(AbilityType.SuperShield, OnEndQueueFinished); 
            
            var slowDownTime = new UpdateTimeScaleCommand(new TimeScaleData
            {
                UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects },
                TargetTimeScale = minTimeScale,
                CurrentTimeScale = 1.0f,
                TimeToUpdate = timeData.SlowDown
            }, _updateTimeScale);
            
            var speedUpTime = new UpdateTimeScaleCommand(new TimeScaleData
            {
                UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects },
                TargetTimeScale = minTimeScale,
                CurrentTimeScale = 1.0f,
                TimeToUpdate = timeData.SpeedUp
            }, _updateTimeScale);
            
            return ActionBuilder.Start()
                .Do(new SimpleCommandAction(start))
                .Then(new SimpleCommandAction(slowDownTime))
                .Then(_playSlowTimeSound)
                .Then(new WaitSecondsAction(timeData.StopAction))
                .Then(new InstantAction(ShieldEventCaller.EnableNormalShield))
                .Then(new SimpleCommandAction(speedUpTime))
                .Then(new WaitSecondsAction(timeData.SpeedUp))
                .Then(_enableInputs)
                .Then(new SimpleCommandAction(end))
                .Build();
        }
        private void CreateShieldData(IAbilityTimeConfigData configData)
        {
            var minTimeScale = 0.025f;
            var selectedAbility = AbilityType.SuperShield;
            var timeData = configData.GetAbilityTimeData(selectedAbility);

            var shieldData = new AbilityStoredData
            {
                ActiveTime = timeData.ActiveTime,
                AbilityType = selectedAbility,
                StartActions = GetShieldStartSequence(minTimeScale, timeData),
                EndActions = GetShieldEndSequence(minTimeScale, timeData),
            };

            _abilities.Add(selectedAbility, shieldData);
        }

        #endregion

        #region Heal

        private IQueueAction GetHealStartSequence(float minTimeScale, float shieldMinTimeScale, IAbilityTimeData timeData)
        {
            var startSequence = new TriggerAbilitySequenceState(AbilityType.Health, OnStartQueueStarted);  
            var endSequence = new TriggerAbilitySequenceState(AbilityType.Health, OnStartQueueFinished);
            
            var setShieldTimeScale = new SetChannelTimeScaleAction(shieldMinTimeScale, new[]{UpdateGroup.Shield});
            
            var disableEarthDamage = new SetBoolAction(false, EarthEventCaller.SetEnableDamage);
            var enableEarthDamage = new SetBoolAction(true, EarthEventCaller.SetEnableDamage);
            
            var slowDownTime = new UpdateTimeScaleCommand(new TimeScaleData
            {
                UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects },
                TargetTimeScale = minTimeScale,
                CurrentTimeScale = 1.0f,
                TimeToUpdate = timeData.SlowDown
            }, _updateTimeScale);
            
            var speedUpTime = new UpdateTimeScaleCommand(new TimeScaleData
            {
                UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects },
                TargetTimeScale = minTimeScale,
                CurrentTimeScale = 1.0f,
                TimeToUpdate = timeData.SpeedUp
            }, _updateTimeScale);
            
            var speedUpShield = new UpdateTimeScaleCommand(new TimeScaleData
            {
                UpdateGroups = new [] { UpdateGroup.Shield},
                TargetTimeScale = 1f,
                CurrentTimeScale = shieldMinTimeScale,
                TimeToUpdate = timeData.SpeedUp
            }, _updateTimeScale);

            var runAbilityTimer = new RunAbilityTimerAction(AbilityType.Health, RunActiveTimer);
            
            return ActionBuilder.Start()
                .Do(new SimpleCommandAction(startSequence))
                .Then(new PublishAbilityActiveAction(AbilityType.Health, true))
                .Then(disableEarthDamage)
                .Then(setShieldTimeScale)
                .Then(_disableAbilityUI)
                .Then(_disableInputs)
                .Then(_playSlowTimeSound)
                .Then(new SimpleCommandAction(slowDownTime))
                .Then(new WaitSecondsAction(timeData.ZoomIn))
                .Then(_cameraZoomIn)
                .Then(new WaitSecondsAction(timeData.StartAction))
                .Then(new InstantAction(EarthEventCaller.Heal))
                .Then(new WaitSecondsAction(timeData.ZoomOut))
                .Then(_cameraZoomOut)
                .Then(new SimpleCommandAction(speedUpTime))
                .Then(new SimpleCommandAction(speedUpShield))
                .Then(new WaitSecondsAction(timeData.SpeedUp))
                .Then(_playSpeedTimeSound)
                .Then(_enableInputs)
                .Then(_enableAbilityUI)
                .Then(runAbilityTimer)
                .Then(enableEarthDamage)
                .Then(new SimpleCommandAction(endSequence))
                .Build();
        }
        
        private IQueueAction GetHealEndSequence()
        {
            var start = new TriggerAbilitySequenceState(AbilityType.Health, OnEndQueueStart);  
            var end = new TriggerAbilitySequenceState(AbilityType.Health, OnEndQueueFinished); 
            
            return ActionBuilder.Start()
                .Do(new SimpleCommandAction(start))
                .Then(new SimpleCommandAction(end))
                .Then(new PublishAbilityActiveAction(AbilityType.Health, false))
                .Build();
        }

        private void CreateHealData(IAbilityTimeConfigData configData)
        {
            var minTimeScale = 0.025f;
            var shieldTimeScale = 0.75f;
            var selectedAbility = AbilityType.Health;
            var timeData = configData.GetAbilityTimeData(AbilityType.Health);
            
            
            var healData = new AbilityStoredData
            {
                AbilityType = selectedAbility,
                StartActions = GetHealStartSequence(minTimeScale,shieldTimeScale,timeData),
                EndActions = GetHealEndSequence(),
                HasInstantEffect = true
            };
            
            _abilities.Add(selectedAbility, healData);
        }
        
        #endregion

        #region SlowMotion

        private IQueueAction GetSlowMotionStartSequence(float minTimeScale, IAbilityTimeData timeData)
        {
            var startSequence = new TriggerAbilitySequenceState(AbilityType.SlowMotion, OnStartQueueStarted);  
            var endSequence = new TriggerAbilitySequenceState(AbilityType.SlowMotion, OnStartQueueFinished);
            
            var runAbilityTimer = new RunAbilityTimerAction(AbilityType.SlowMotion, RunActiveTimer);
            
            var slowDownShield = new UpdateTimeScaleCommand(new TimeScaleData
            {
                UpdateGroups = new [] { UpdateGroup.Shield},
                TargetTimeScale = 0.85f,
                CurrentTimeScale = 1.0f,
                TimeToUpdate = timeData.SlowDown
            }, _updateTimeScale);
            
            var slowDownGameplay = new UpdateTimeScaleCommand(new TimeScaleData
            {
                UpdateGroups = new [] { UpdateGroup.Gameplay},
                TargetTimeScale = minTimeScale,
                CurrentTimeScale = 1.0f,
                TimeToUpdate = timeData.SlowDown
            }, _updateTimeScale);
            
            var slowDownEarthAndEffects = new UpdateTimeScaleCommand(new TimeScaleData
            {
                UpdateGroups = new [] { UpdateGroup.Earth, UpdateGroup.Effects },
                TargetTimeScale = minTimeScale/2,
                CurrentTimeScale = 1.0f,
                TimeToUpdate = timeData.SlowDown
            }, _updateTimeScale);
            
            return ActionBuilder.Start()
                .Do(new SimpleCommandAction(startSequence))
                .Then(new PublishAbilityActiveAction(AbilityType.SlowMotion, true))
                .Then(new SetChannelPausedAction(true, new[]{UpdateGroup.Gameplay}))
                .Then(new SimpleCommandAction(slowDownShield))
                .Then(new SimpleCommandAction(slowDownGameplay))
                .Then(new SimpleCommandAction(slowDownEarthAndEffects))
                .Then(_disableAbilityUI)
                .Then(_disableInputs)
                .Then(_cameraZoomIn)
                .Then(_playSlowTimeSound)
                .Then(new WaitSecondsAction(timeData.SlowDown))
                .Then(new SetBoolAction(true, ShieldEventCaller.SetSlow))
                .Then(new WaitSecondsAction(timeData.ZoomOut))
                .Then(new SetChannelPausedAction(false, new[]{UpdateGroup.Gameplay}))
                .Then(_cameraZoomOut)
                .Then(_enableAbilityUI)
                .Then(_enableInputs)
                .Then(runAbilityTimer)
                .Then(new SimpleCommandAction(endSequence))
                .Build();
        }
        
        private IQueueAction GetSlowMotionEndSequence(float minTimeScale, IAbilityTimeData timeData)
        {
            var start = new TriggerAbilitySequenceState(AbilityType.SlowMotion, OnEndQueueStart);  
            var end = new TriggerAbilitySequenceState(AbilityType.SlowMotion, OnEndQueueFinished); 
            
            var speedUpShield = new UpdateTimeScaleCommand(new TimeScaleData
            {
                UpdateGroups = new [] { UpdateGroup.Shield},
                TargetTimeScale = 0.85f,
                CurrentTimeScale = 1.0f,
                TimeToUpdate = timeData.SlowDown
            }, _updateTimeScale);
            
            var speedUpGameplay = new UpdateTimeScaleCommand(new TimeScaleData
            {
                UpdateGroups = new [] { UpdateGroup.Gameplay},
                TargetTimeScale = minTimeScale,
                CurrentTimeScale = 1.0f,
                TimeToUpdate = timeData.SlowDown
            }, _updateTimeScale);
            
            var peedUpEarthAndEffects = new UpdateTimeScaleCommand(new TimeScaleData
            {
                UpdateGroups = new [] { UpdateGroup.Earth, UpdateGroup.Effects },
                TargetTimeScale = minTimeScale/2,
                CurrentTimeScale = 1.0f,
                TimeToUpdate = timeData.SlowDown
            }, _updateTimeScale);
            
            return ActionBuilder.Start()
                .Do(new SimpleCommandAction(start))
                .Then(_disableAbilityUI)
                .Then(_disableInputs)
                .Then(_cameraZoomIn)
                .Then(new SetChannelPausedAction(true, new[]{UpdateGroup.Gameplay}))
                .Then(new SimpleCommandAction(speedUpShield))
                .Then(new SimpleCommandAction(speedUpGameplay))
                .Then(new SimpleCommandAction(peedUpEarthAndEffects))
                .Then(_playSpeedTimeSound)
                .Then(new WaitSecondsAction(timeData.SlowDown))
                .Then(new SetBoolAction(false, ShieldEventCaller.SetSlow))
                .Then(new WaitSecondsAction(timeData.ZoomOut))
                .Then(_cameraZoomOut)
                .Then(_enableAbilityUI)
                .Then(_enableInputs)
                .Then(new SetChannelPausedAction(false, new[]{UpdateGroup.Gameplay}))
                .Then(new PublishAbilityActiveAction(AbilityType.SlowMotion, false))
                .Then(new SimpleCommandAction(end))
                .Build();
        }

        private void CreateSlowMotionData(IAbilityTimeConfigData configData)
        {
            var minTimeScale = 0.5f;
            var selectedAbility = AbilityType.SlowMotion;
            var timeData = configData.GetAbilityTimeData(selectedAbility);

            var abilityData = new AbilityStoredData
            {
                ActiveTime = timeData.ActiveTime,
                AbilityType = selectedAbility,
                StartActions = GetSlowMotionStartSequence(minTimeScale,timeData),
                EndActions = GetSlowMotionEndSequence(minTimeScale,timeData)
            };

            _abilities.Add(selectedAbility, abilityData);
        }
        
        #endregion

        #region Double Points

        private IQueueAction GetDoublePointsStartSequence(float minTimeScale, IAbilityTimeData timeData)
        {
            var startSequence = new TriggerAbilitySequenceState(AbilityType.DoublePoints, OnStartQueueStarted);  
            var endSequence = new TriggerAbilitySequenceState(AbilityType.DoublePoints, OnStartQueueFinished);

            var slowDownTime = new UpdateTimeScaleCommand(new TimeScaleData
            {
                UpdateGroups = new[] { UpdateGroup.Gameplay, UpdateGroup.Effects },
                TargetTimeScale = minTimeScale,
                CurrentTimeScale = 1f,
                TimeToUpdate = timeData.SlowDown,
            },_updateTimeScale);
            
            var speedUpTime = new UpdateTimeScaleCommand(new TimeScaleData
            {
                UpdateGroups = new[] { UpdateGroup.Gameplay, UpdateGroup.Effects },
                TargetTimeScale = 1f,
                CurrentTimeScale = minTimeScale,
                TimeToUpdate = timeData.SpeedUp,
            },_updateTimeScale);
            
            return ActionBuilder.Start()
                .Do(new SimpleCommandAction(startSequence))
                .Then(new PublishAbilityActiveAction(AbilityType.DoublePoints, true))
                .Then(_disableInputs)
                .Then(_disableAbilityUI)
                .Then(_cameraZoomIn)
                .Then(new SimpleCommandAction(slowDownTime))
                .Then(new WaitSecondsAction(timeData.SlowDown))
                .Then(_playSlowTimeSound)
                .Then(new SetBoolAction(true,ShieldEventCaller.SetGold))
                .Then(new SimpleCommandAction(speedUpTime))
                .Then(new WaitSecondsAction(timeData.SpeedUp))
                .Then(new WaitSecondsAction(timeData.ZoomOut))
                .Then(_playSpeedTimeSound)
                .Then(new RunAbilityTimerAction(AbilityType.DoublePoints,RunActiveTimer))
                .Then(_cameraZoomOut)
                .Then(_enableInputs)
                .Then(_enableAbilityUI)
                .Then(new SimpleCommandAction(endSequence))
                .Build();
        }
        
        private IQueueAction GetDoublePointsEndSequence(float minTimeScale, IAbilityTimeData timeData)
        {
            var startSequence = new TriggerAbilitySequenceState(AbilityType.DoublePoints, OnEndQueueStart);  
            var endSequence = new TriggerAbilitySequenceState(AbilityType.DoublePoints, OnEndQueueFinished); 
            
            return ActionBuilder.Start()
                .Do(new SimpleCommandAction(startSequence))
                .Then(new SetBoolAction(false,ShieldEventCaller.SetGold))
                .Then(new PublishAbilityActiveAction(AbilityType.DoublePoints, false))
                .Then(new SimpleCommandAction(endSequence))
                .Build();
        }

        private void CreateDoublePointsData(IAbilityTimeConfigData configData)
        {
            float targetTimeScale = 0.025f;
            var selectedAbility = AbilityType.DoublePoints;
            var timeData = configData.GetAbilityTimeData(selectedAbility);
            

            var abilityData = new AbilityStoredData
            {
                ActiveTime = timeData.ActiveTime,
                AbilityType = selectedAbility,
                StartActions = GetDoublePointsStartSequence(targetTimeScale,timeData),
                EndActions = GetDoublePointsEndSequence(targetTimeScale,timeData)
            };

            _abilities.Add(selectedAbility, abilityData);
        }

        #endregion

        #region Automatic

        public IQueueAction GetAutomaticStartSequence(float minTimeScale, IAbilityTimeData timeData)
        {
            var startSequence = new TriggerAbilitySequenceState(AbilityType.Automatic, OnStartQueueStarted);  
            var endSequence = new TriggerAbilitySequenceState(AbilityType.Automatic, OnStartQueueFinished);

            var slowDown = new UpdateTimeScaleCommand(new TimeScaleData
            {
                UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                TargetTimeScale = minTimeScale,
                CurrentTimeScale = 1f,
                TimeToUpdate = timeData.StartAction,
            },_updateTimeScale);
            
            var speedUp = new UpdateTimeScaleCommand(new TimeScaleData
            {
                UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                TargetTimeScale = 1f,
                CurrentTimeScale = minTimeScale,
                TimeToUpdate = timeData.StartAction,
            },_updateTimeScale);
            
            return ActionBuilder.Start()
                .Do(new SimpleCommandAction(startSequence))
                .Then(new PublishAbilityActiveAction(AbilityType.Automatic, true))
                .Then(new SimpleCommandAction(slowDown))
                .Then(_cameraZoomIn)
                .Then(_disableInputs)
                .Then(_disableAbilityUI)
                .Then(_playSlowTimeSound)
                .Then(new WaitSecondsAction(timeData.StartAction))
                .Then(new SetBoolAction(true,ShieldEventCaller.SetAutomatic))
                .Then(new SimpleCommandAction(speedUp))
                .Then(_cameraZoomOut)
                .Then(_playSpeedTimeSound)
                .Then(new RunAbilityTimerAction(AbilityType.Automatic, RunActiveTimer))
                .Then(_enableInputs)
                .Then(_enableAbilityUI)
                .Then(new SimpleCommandAction(endSequence))
                .Build();
        }
        
        public IQueueAction GetAutomaticEndSequence(float minTimeScale, IAbilityTimeData timeData)
        {
            var startSequence = new TriggerAbilitySequenceState(AbilityType.Automatic, OnEndQueueStart);  
            var endSequence = new TriggerAbilitySequenceState(AbilityType.Automatic, OnEndQueueFinished); 
            
            var slowDown = new UpdateTimeScaleCommand(new TimeScaleData
            {
                UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                TargetTimeScale = minTimeScale,
                CurrentTimeScale = 1f,
                TimeToUpdate = timeData.SlowDown,
            },_updateTimeScale);
            
            var speedUp = new UpdateTimeScaleCommand(new TimeScaleData
            {
                UpdateGroups = new [] { UpdateGroup.Gameplay, UpdateGroup.Effects},
                TargetTimeScale = 1f,
                CurrentTimeScale = minTimeScale,
                TimeToUpdate = timeData.SpeedUp,
            },_updateTimeScale);
            
            return ActionBuilder.Start()
                .Do(new SimpleCommandAction(startSequence))
                .Then(new PublishAbilityActiveAction(AbilityType.Automatic, false))
                .Then(new SimpleCommandAction(slowDown))
                .Then(_cameraZoomIn)
                .Then(_disableInputs)
                .Then(_disableAbilityUI)
                .Then(_playSlowTimeSound)
                .Then(new WaitSecondsAction(timeData.StopAction))
                .Then(_cameraZoomOut)
                .Then(_enableInputs)
                .Then(_enableAbilityUI)
                .Then(new SetBoolAction(false,ShieldEventCaller.SetAutomatic))
                .Then(new SimpleCommandAction(endSequence))
                .Build();
        }

        private void CreateAutomaticData(IAbilityTimeConfigData configData)
        {
            float targetTimeScale = 0.025f;
            var selectedAbility = AbilityType.Automatic;
            var timeData = configData.GetAbilityTimeData(selectedAbility);
            

            var abilityData = new AbilityStoredData
            {
                ActiveTime = timeData.ActiveTime,
                AbilityType = selectedAbility,
                StartActions = GetAutomaticStartSequence(targetTimeScale,timeData),
                EndActions = GetAutomaticEndSequence(targetTimeScale,timeData)
            };

            _abilities.Add(selectedAbility, abilityData);
        }

        #endregion
        
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

        public bool HasAbilityData(AbilityType abilityType)
        {
            return _abilities.ContainsKey(abilityType);
        }

        public IQueueAction GetAbilityStartQueue(AbilityType abilityType)
        {
            return _abilities[abilityType].GetStartActionQueue();
        }
        
        public IQueueAction GetAbilityEndQueue(AbilityType abilityType)
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
        public IQueueAction StartActions;
        public IQueueAction EndActions;
        

        public IQueueAction GetStartActionQueue()
        {
            return StartActions.Copy();
        }

        public IQueueAction GetEndActionQueue()
        {
            return EndActions.Copy();
        }

        public bool GetHasInstantEffect()
        {
            return HasInstantEffect;
        }
    }
}