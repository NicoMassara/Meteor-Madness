using System;
using System.Collections.Generic;
using MeteorMadness.GlobalValues;
using MeteorMadness.GlobalValues.Interfaces;
using MeteorMadness.Managers;
using NicolasMassara.CustomActionManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.Gameplay.Abilities
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

            public override ICommand Copy()
            {
                return new TriggerAbilitySequenceState(_ability, _abilityEvent);
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
                CustomTime.SetChannelPaused(_updateGroup, _isPaused);
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
        private class TimedTimeScaleUpdateAction : IQueueAction
        {
            private readonly float _startTimeScale;
            private readonly float _duration;
            private readonly float _targetTimeScale;
            private readonly UpdateGroup _updateGroup;
            private float _elapsed;
            
            public ActionStatus CurrentStatus { get; private set; } = ActionStatus.Idle;

            public TimedTimeScaleUpdateAction(float targetValue, float startValue, float duration, UpdateGroup updateGroup)
            {
                _startTimeScale = startValue;
                _duration = duration;
                _targetTimeScale = targetValue;
                _updateGroup = updateGroup;
            }

            public void OnStart()
            {
                CurrentStatus = ActionStatus.Running;
            }

            public ActionStatus OnUpdate(float deltaTime)
            {
                if (_elapsed < _duration)
                {
                    _elapsed += deltaTime;
                    float timeRatio = Mathf.Clamp01(_elapsed / _duration);
                    var current = Mathf.Lerp(_startTimeScale, _targetTimeScale, timeRatio);

                    if (timeRatio >= 1)
                    {
                        CurrentStatus = ActionStatus.Success;
                        current = _targetTimeScale;
                    }
                    
                    CustomTime.SetChannelTimeScale(_updateGroup, current);
                }

                return CurrentStatus;
            }

            public void OnInterrupt()
            {
                CustomTime.SetChannelTimeScale(_updateGroup, _targetTimeScale);
            }

            public IQueueAction Copy()
            {
                return new TimedTimeScaleUpdateAction(_targetTimeScale,_startTimeScale, _duration, _updateGroup);
            }
        }
        private class EnableShieldTypeAction : IQueueAction
        {
            private readonly ShieldType _shieldType;
            public ActionStatus CurrentStatus { get; } = ActionStatus.Success;

            public EnableShieldTypeAction(ShieldType shieldType)
            {
                _shieldType = shieldType;
            }

            public void OnStart()
            {
                ShieldEventCaller.RequestEnableShieldType(_shieldType);
            }

            public ActionStatus OnUpdate(float deltaTime)
            {
                return CurrentStatus;
            }

            public void OnInterrupt() { }

            public IQueueAction Copy()
            {
                return new EnableShieldTypeAction(_shieldType);
            }
        }
        private class DisableShieldTypeAction : IQueueAction
        {
            private readonly ShieldType _shieldType;
            public ActionStatus CurrentStatus { get; } = ActionStatus.Success;

            public DisableShieldTypeAction(ShieldType shieldType)
            {
                _shieldType = shieldType;
            }

            public void OnStart()
            {
                ShieldEventCaller.RequestDisableShieldType(_shieldType);
            }

            public ActionStatus OnUpdate(float deltaTime)
            {
                return CurrentStatus;
            }

            public void OnInterrupt() { }

            public IQueueAction Copy()
            {
                return new DisableShieldTypeAction(_shieldType);
            }
        }

        #endregion
        
        private readonly Dictionary<AbilityType, AbilityStoredData> _abilities = new Dictionary<AbilityType, AbilityStoredData>();
        
        public event Action<float> OnAbilityStarted;
        public event Action<AbilityType> OnStartQueueStarted;
        public event Action<AbilityType> OnStartQueueFinished;
        public event Action<AbilityType> OnEndQueueStart;
        public event Action<AbilityType> OnEndQueueFinished;
        
        private event Action OnSuperShieldFinished;

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

        public AbilityDataController(Action speedTimeSound, Action slowTimeSound)
        {
            _playSpeedTimeSound = new InstantAction(speedTimeSound);
            _playSlowTimeSound = new InstantAction(slowTimeSound);
            
            _cameraZoomIn = new InstantAction(()=> CameraEventCaller.ZoomIn(0.1f));
            _cameraZoomOut = new InstantAction(()=> CameraEventCaller.ZoomOut(0.1f));
            
            _enableInputs = new SetBoolAction(true,SetInputsEnable);
            _disableInputs = new SetBoolAction(false,SetInputsEnable);
            _enableAbilityUI = new SetBoolAction(true,SetEnableAbilityUI);
            _disableAbilityUI = new SetBoolAction(false,SetEnableAbilityUI);
        }
        
        public void Initialize(IAbilityTimeConfigData data)
        {
            CreateAbilityData(data);
        }
        
        private void CreateAbilityData(IAbilityTimeConfigData configData)
        {
            CreateSlowMotionData(configData);
            CreateShieldData(configData);
            CreateHealData(configData);
            CreateDoublePointsData(configData);
            CreateAutomaticData(configData);
        }

        #region Abilities Data
        
        #region Shield

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
        
        private IQueueAction GetShieldStartSequence(float minTimeScale, IAbilityTimeData timeData)
        {
            var start = new TriggerAbilitySequenceState(AbilityType.SuperShield, OnStartQueueStarted);  
            var end = new TriggerAbilitySequenceState(AbilityType.SuperShield, OnStartQueueFinished);  
            
            var slowDownGameplay = new TimedTimeScaleUpdateAction(
                targetValue: minTimeScale,  startValue: 1, timeData.SlowDown, UpdateGroup.Gameplay );
            
            var slowDownEffects = new TimedTimeScaleUpdateAction(
                targetValue: minTimeScale,  startValue: 1, timeData.SlowDown, UpdateGroup.Effects );
            
            var speedUpGameplay = new TimedTimeScaleUpdateAction(
                targetValue: 1,  startValue: minTimeScale, timeData.SpeedUp, UpdateGroup.Gameplay );
            
            var speedUpEffects = new TimedTimeScaleUpdateAction(
                targetValue: 1,  startValue: minTimeScale, timeData.SpeedUp, UpdateGroup.Effects );
            
            
            return ActionBuilder.Start()
                .Do(new SimpleCommandAction(start))
                .Then(new PublishAbilityActiveAction(AbilityType.SuperShield, true))
                .Then(_disableInputs)
                .Then(_disableAbilityUI)
                .Then(_playSlowTimeSound)
                .Then(new ParallelAction(new []{slowDownGameplay,slowDownEffects}))
                .Then(new WaitSecondsAction(timeData.ZoomIn))
                .Then(_cameraZoomIn)
                .Then(new WaitSecondsAction(timeData.StartAction))
                .Then(new EnableShieldTypeAction(ShieldType.Super))
                .Then(new WaitSecondsAction(timeData.ZoomOut))
                .Then(new InstantAction(MeteorEventCaller.SpawnRing))
                .Then(_cameraZoomOut)
                .Then(new ParallelAction(new []{speedUpGameplay,speedUpEffects}))
                .Then(_enableAbilityUI)
                .Then(new SimpleCommandAction(end))
                .Then(new WaitFramesAction(1))
                .Build();
        }
        private IQueueAction GetShieldEndSequence(float minTimeScale, IAbilityTimeData timeData)
        {
            var start = new TriggerAbilitySequenceState(AbilityType.SuperShield, OnEndQueueStart);  
            var end = new TriggerAbilitySequenceState(AbilityType.SuperShield, OnEndQueueFinished); 
            
            var slowDownGameplay = new TimedTimeScaleUpdateAction(
                targetValue: minTimeScale,  startValue: 1, timeData.SlowDown, UpdateGroup.Gameplay );
            
            var slowDownEffects = new TimedTimeScaleUpdateAction(
                targetValue: minTimeScale,  startValue: 1, timeData.SlowDown, UpdateGroup.Effects );
            
            var speedUpGameplay = new TimedTimeScaleUpdateAction(
                targetValue: 1,  startValue: minTimeScale, timeData.SpeedUp, UpdateGroup.Gameplay );
            
            var speedUpEffects = new TimedTimeScaleUpdateAction(
                targetValue: 1,  startValue: minTimeScale, timeData.SpeedUp, UpdateGroup.Effects );
            
            ;
            
            
            return ActionBuilder.Start()
                .Do(new SimpleCommandAction(start))
                .Then(new InstantAction(()=> ShieldEventSubscriber.NotifyShieldTypeDisabled(OnShieldTypeDisabled)))
                .Then(new ParallelAction(new []{slowDownGameplay,slowDownEffects}))
                .Then(_playSlowTimeSound)
                .Then(new WaitSecondsAction(timeData.StopAction))
                .Then(new DisableShieldTypeAction(ShieldType.Super))
                .Then(new WaitForEventAction(
                    subscribe: callback => OnSuperShieldFinished += callback,
                    unsubscribe: callback => OnSuperShieldFinished -= callback))
                .Then(new InstantAction(()=> ShieldEventUnSubscriber.NotifyShieldTypeDisabled(OnShieldTypeDisabled)))
                .Then(new ParallelAction(new []{speedUpGameplay,speedUpEffects}))
                .Then(new WaitSecondsAction(timeData.SpeedUp))
                .Then(_enableInputs)
                .Then(new PublishAbilityActiveAction(AbilityType.SuperShield, false))
                .Then(new SimpleCommandAction(end))
                .Build();
        }

        private void OnShieldTypeDisabled(Managers.ShieldEvents.NotifyShieldTypeDisabled input)
        {
            if(input.Type == ShieldType.Super)
                OnSuperShieldFinished?.Invoke();
        }

        #endregion

        #region Heal

        private IQueueAction GetHealStartSequence(float minTimeScale, float shieldMinTimeScale, IAbilityTimeData timeData)
        {
            var startSequence = new TriggerAbilitySequenceState(AbilityType.Health, OnStartQueueStarted);  
            var endSequence = new TriggerAbilitySequenceState(AbilityType.Health, OnStartQueueFinished);
            
            // SlowDown
            var setShieldTimeScale = new SetChannelTimeScaleAction(shieldMinTimeScale, new[]{UpdateGroup.Shield});
            
            var slowDownGameplay = new TimedTimeScaleUpdateAction(
                targetValue: minTimeScale,  startValue: 1, timeData.SlowDown, UpdateGroup.Gameplay );
            
            var slowDownEffects = new TimedTimeScaleUpdateAction(
                targetValue: minTimeScale,  startValue: 1, timeData.SlowDown, UpdateGroup.Effects );

            // SpeedUp
            var speedUpGameplay = new TimedTimeScaleUpdateAction(
                targetValue: 1,  startValue: minTimeScale, timeData.SpeedUp, UpdateGroup.Gameplay );
            
            var speedUpEffects = new TimedTimeScaleUpdateAction(
                targetValue: 1,  startValue: minTimeScale, timeData.SpeedUp, UpdateGroup.Effects );
            
            var speedUpShield = new TimedTimeScaleUpdateAction(
                targetValue: 1f,  startValue: shieldMinTimeScale, timeData.SpeedUp, UpdateGroup.Shield );

            var runAbilityTimer = new RunAbilityTimerAction(AbilityType.Health, RunActiveTimer);
            
            return ActionBuilder.Start()
                .Do(new SimpleCommandAction(startSequence))
                .Then(new PublishAbilityActiveAction(AbilityType.Health, true))
                .Then(new ParallelAction(new [] {slowDownGameplay,slowDownEffects }))
                .Then(setShieldTimeScale)
                //.Then(new WaitSecondsAction(timeData.ZoomIn))
                .Then(_cameraZoomIn)
                .Then(new InstantAction(EarthEventCaller.DisableDamage))
                .Then(_disableAbilityUI)
                .Then(_disableInputs)
                .Then(_playSlowTimeSound)
                .Then(new WaitSecondsAction(timeData.StartAction))
                .Then(new InstantAction(EarthEventCaller.Heal))
                .Then(new WaitSecondsAction(timeData.ZoomOut))
                .Then(_cameraZoomOut)
                .Then(new ParallelAction(new [] {speedUpGameplay,speedUpEffects,speedUpShield }))
                .Then(new WaitSecondsAction(timeData.SpeedUp))
                .Then(_playSpeedTimeSound)
                .Then(_enableInputs)
                .Then(_enableAbilityUI)
                .Then(runAbilityTimer)
                .Then(new SimpleCommandAction(endSequence))
                .Build();
        }
        
        private IQueueAction GetHealEndSequence()
        {
            var start = new TriggerAbilitySequenceState(AbilityType.Health, OnEndQueueStart);  
            var end = new TriggerAbilitySequenceState(AbilityType.Health, OnEndQueueFinished); 
            
            return ActionBuilder.Start()
                .Do(new SimpleCommandAction(start))
                .Then(new InstantAction(EarthEventCaller.EnableDamage))
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
            
            // Slow Down
            
            var slowDownShield = new TimedTimeScaleUpdateAction(
                targetValue: 0.85f,  startValue:1, timeData.SlowDown, UpdateGroup.Shield );
            
            var slowDownGameplay = new TimedTimeScaleUpdateAction(
                minTimeScale, startValue: 1, timeData.SlowDown, UpdateGroup.Gameplay );
            
            var slowDownEarth = new TimedTimeScaleUpdateAction(
                targetValue: minTimeScale/2, startValue: 1, timeData.SlowDown, UpdateGroup.Earth );
            
            var slowDownEffects = new TimedTimeScaleUpdateAction(
                targetValue: minTimeScale/2, startValue: 1, timeData.SlowDown, UpdateGroup.Effects );
            
            
            return ActionBuilder.Start()
                .Do(new SimpleCommandAction(startSequence))
                .Then(new PublishAbilityActiveAction(AbilityType.SlowMotion, true))
                .Then(new SetChannelPausedAction(true, new[]{UpdateGroup.Gameplay}))
                .Then(new ParallelAction(new []
                {
                    slowDownShield, slowDownGameplay, slowDownEarth,slowDownEffects
                }))
                .Then(_disableAbilityUI)
                .Then(_disableInputs)
                .Then(_cameraZoomIn)
                .Then(_playSlowTimeSound)
                .Then(new WaitSecondsAction(timeData.SlowDown))
                .Then(new EnableShieldTypeAction(ShieldType.Slow))
                .Then(new WaitSecondsAction(timeData.ZoomOut))
                .Then(_enableInputs)
                .Then(_cameraZoomOut)
                .Then(_enableAbilityUI)
                .Then(new SetChannelPausedAction(false, new[]{UpdateGroup.Gameplay}))
                .Then(runAbilityTimer)
                .Then(new SimpleCommandAction(endSequence))
                .Build();
        }
        
        private IQueueAction GetSlowMotionEndSequence(float minTimeScale, IAbilityTimeData timeData)
        {
            var start = new TriggerAbilitySequenceState(AbilityType.SlowMotion, OnEndQueueStart);  
            var end = new TriggerAbilitySequenceState(AbilityType.SlowMotion, OnEndQueueFinished); 
            
            // Speed Up
            var speedUpShield = new TimedTimeScaleUpdateAction(
                targetValue: 1f,  startValue: 0.85f, timeData.SpeedUp, UpdateGroup.Shield );
            
            var speedUpGameplay = new TimedTimeScaleUpdateAction(
                targetValue: 1f,  startValue: minTimeScale, timeData.SpeedUp, UpdateGroup.Gameplay );
            
            var speedUpEarth = new TimedTimeScaleUpdateAction(
                targetValue: 1f,  startValue: minTimeScale/2, timeData.SpeedUp, UpdateGroup.Earth );
            
            var speedUpEffects= new TimedTimeScaleUpdateAction(
                targetValue: 1f,  startValue: minTimeScale/2, timeData.SpeedUp, UpdateGroup.Effects );
            
            
            return ActionBuilder.Start()
                .Do(new SimpleCommandAction(start))
                .Then(_disableAbilityUI)
                .Then(_disableInputs)
                .Then(_cameraZoomIn)
                .Then(new SetChannelPausedAction(true, new[]{UpdateGroup.Gameplay}))
                .Then(new ParallelAction(new []
                {
                    speedUpShield, speedUpGameplay, speedUpEarth,speedUpEffects
                }))
                .Then(_playSpeedTimeSound)
                .Then(new WaitSecondsAction(timeData.SlowDown))
                .Then(new DisableShieldTypeAction(ShieldType.Slow))
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
            
            var slowDownGameplay = new TimedTimeScaleUpdateAction(
                targetValue: minTimeScale,  startValue:1, timeData.SlowDown, UpdateGroup.Gameplay );
            
            var slowDownEffects = new TimedTimeScaleUpdateAction(
                targetValue: minTimeScale,  startValue:1, timeData.SlowDown, UpdateGroup.Effects );
            
            var speedUpTime = new TimedTimeScaleUpdateAction(
                targetValue: 1f,  startValue: minTimeScale, timeData.SpeedUp, UpdateGroup.Gameplay );
            
            var speedUpEffects = new TimedTimeScaleUpdateAction(
                targetValue: 1f,  startValue: minTimeScale, timeData.SpeedUp, UpdateGroup.Effects );
            
            
            return ActionBuilder.Start()
                .Do(new SimpleCommandAction(startSequence))
                .Then(new PublishAbilityActiveAction(AbilityType.DoublePoints, true))
                .Then(_disableInputs)
                .Then(_disableAbilityUI)
                .Then(_cameraZoomIn)
                .Then(new ParallelAction(new [] { slowDownGameplay,slowDownEffects }))
                .Then(new WaitSecondsAction(timeData.SlowDown))
                .Then(_playSlowTimeSound)
                .Then(new EnableShieldTypeAction(ShieldType.Gold))
                .Then(new WaitSecondsAction(timeData.ZoomOut))
                .Then(_enableAbilityUI)
                .Then(_enableInputs)
                .Then(_cameraZoomOut)
                .Then(new ParallelAction(new [] {speedUpTime,speedUpEffects }))
                .Then(_playSpeedTimeSound)
                .Then(new WaitSecondsAction(timeData.SpeedUp))
                .Then(new RunAbilityTimerAction(AbilityType.DoublePoints,RunActiveTimer))
                .Then(new SimpleCommandAction(endSequence))
                .Build();
        }
        
        private IQueueAction GetDoublePointsEndSequence(float minTimeScale, IAbilityTimeData timeData)
        {
            var startSequence = new TriggerAbilitySequenceState(AbilityType.DoublePoints, OnEndQueueStart);  
            var endSequence = new TriggerAbilitySequenceState(AbilityType.DoublePoints, OnEndQueueFinished); 
            
            return ActionBuilder.Start()
                .Do(new SimpleCommandAction(startSequence))
                .Then(new DisableShieldTypeAction(ShieldType.Slow))
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

        private IQueueAction GetAutomaticStartSequence(float minTimeScale, IAbilityTimeData timeData)
        {
            var startSequence = new TriggerAbilitySequenceState(AbilityType.Automatic, OnStartQueueStarted);  
            var endSequence = new TriggerAbilitySequenceState(AbilityType.Automatic, OnStartQueueFinished);

            var slowDownGameplay = new TimedTimeScaleUpdateAction(
                targetValue: minTimeScale,  startValue:1, timeData.SlowDown, UpdateGroup.Gameplay );
            
            var slowDownEffects = new TimedTimeScaleUpdateAction(
                targetValue: minTimeScale,  startValue:1, timeData.SlowDown, UpdateGroup.Effects );
            
            var speedUpTime = new TimedTimeScaleUpdateAction(
                targetValue: 1f,  startValue:minTimeScale, timeData.SpeedUp, UpdateGroup.Gameplay );
            
            var speedUpEffects = new TimedTimeScaleUpdateAction(
                targetValue: 1f,  startValue:minTimeScale, timeData.SpeedUp, UpdateGroup.Effects );
            
            return ActionBuilder.Start()
                .Do(new SimpleCommandAction(startSequence))
                .Then(new PublishAbilityActiveAction(AbilityType.Automatic, true))
                .Then(new ParallelAction(new [] {slowDownGameplay,slowDownEffects }))
                .Then(_cameraZoomIn)
                .Then(_disableInputs)
                .Then(_disableAbilityUI)
                .Then(_playSlowTimeSound)
                .Then(new WaitSecondsAction(timeData.StartAction))
                .Then(new EnableShieldTypeAction(ShieldType.Automatic))
                .Then(new ParallelAction(new [] {speedUpTime,speedUpEffects }))
                .Then(_cameraZoomOut)
                .Then(_playSpeedTimeSound)
                .Then(new RunAbilityTimerAction(AbilityType.Automatic, RunActiveTimer))
                .Then(_enableAbilityUI)
                .Then(new SimpleCommandAction(endSequence))
                .Build();
        }
        
        private IQueueAction GetAutomaticEndSequence(float minTimeScale, IAbilityTimeData timeData)
        {
            var startSequence = new TriggerAbilitySequenceState(AbilityType.Automatic, OnEndQueueStart);  
            var endSequence = new TriggerAbilitySequenceState(AbilityType.Automatic, OnEndQueueFinished); 
            
            var slowDownGameplay = new TimedTimeScaleUpdateAction(
                targetValue: minTimeScale,  startValue:1, timeData.SlowDown, UpdateGroup.Gameplay );
            
            var slowDownEffects = new TimedTimeScaleUpdateAction(
                targetValue: minTimeScale,  startValue:1, timeData.SlowDown, UpdateGroup.Effects );
            
            var speedUpTime = new TimedTimeScaleUpdateAction(
                targetValue: 1f,  startValue:minTimeScale, timeData.SpeedUp, UpdateGroup.Gameplay );
            
            var speedUpEffects = new TimedTimeScaleUpdateAction(
                targetValue: 1f,  startValue:minTimeScale, timeData.SpeedUp, UpdateGroup.Effects );
            
            return ActionBuilder.Start()
                .Do(new SimpleCommandAction(startSequence))
                .Then(new PublishAbilityActiveAction(AbilityType.Automatic, false))
                .Then(new ParallelAction(new [] {slowDownGameplay,slowDownEffects }))
                .Then(_cameraZoomIn)
                .Then(_disableInputs)
                .Then(_disableAbilityUI)
                .Then(_playSlowTimeSound)
                .Then(new WaitSecondsAction(timeData.StopAction))
                .Then(new ParallelAction(new [] {speedUpTime,speedUpEffects }))
                .Then(_cameraZoomOut)
                .Then(_enableInputs)
                .Then(_enableAbilityUI)
                .Then(new DisableShieldTypeAction(ShieldType.Automatic))
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
            if (isEnable)
            {
                AbilitiesEventCaller.EnableUI();
            }
            else
            {
                AbilitiesEventCaller.DisableUI();
            }
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

    public class AbilityStoredData
    {
        public float ActiveTime;
        public AbilityType AbilityType;
        public IQueueAction StartActions;
        public IQueueAction EndActions;
        

        public IQueueAction GetStartActionQueue()
        {
            return (DynamicActionSequence)StartActions.Copy();
        }

        public IQueueAction GetEndActionQueue()
        {
            return (DynamicActionSequence)EndActions.Copy();
        }

        public bool GetHasInstantEffect()
        {
            return EndActions == null;
        }
    }
}