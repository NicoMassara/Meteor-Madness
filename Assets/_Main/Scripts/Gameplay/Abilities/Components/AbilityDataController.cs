using System;
using System.Collections.Generic;
using _Main.Scripts.Contracts.Interfaces;
using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;
using NicolasMassara.CustomActionManager;
using NicolasMassara.CustomUpdateManager;

using UnityEngine;

namespace MeteorMadness.Gameplay.Abilities
{
    public class AbilityDataController
    {
        #region Commands

        private class WaitForBatchesToFinishAction : IQueueAction
        {
            public ActionStatus CurrentStatus { get; private set; } = ActionStatus.Idle;
            private readonly int _targetBatches;
            private readonly BatchType _batchType;
            private readonly bool _hasBatchSpawned;
            private int _batchesLeft;
            private int _activeBatches;

            public WaitForBatchesToFinishAction(int targetBatches, BatchType batchType, bool hasBatchSpawned = true)
            {
                // First batch is already spawned
                _targetBatches = targetBatches;
                _batchType = batchType;
                _hasBatchSpawned = hasBatchSpawned;
            }

            public void OnStart()
            {
                _batchesLeft = _hasBatchSpawned ? _targetBatches - 1 : _targetBatches;
                _activeBatches = 0;

                if (_hasBatchSpawned == false)
                {
                    ProjectileSpawner.Publish.RequestSpawn(_batchType);
                }

                ToggleSubscriptions(true);
                
                CurrentStatus = ActionStatus.Running;
            }

            public ActionStatus OnUpdate(float deltaTime) => CurrentStatus;

            public void OnInterrupt()
            {
                Cleanup();
                
                CurrentStatus = ActionStatus.Failure;
            }

            public IQueueAction Copy()
            {
                return new WaitForBatchesToFinishAction(_targetBatches, _batchType, _hasBatchSpawned);
            }
            
            private void OnProjectileReachedTargetHandler(ProjectileSpawnerEvents.ProjectileReachedTarget input)
            {
                if (input.IsLastFromBatch && _batchesLeft > 0)
                {
                    _batchesLeft--;
                    //Debug.Log($"Batch Spawned, Left: {_batchesLeft}");
                    ProjectileSpawner.Publish.RequestSpawn(_batchType);
                }
            }
    
            private void OnBatchSpawnedHandler(ProjectileSpawnerEvents.BatchSpawned input)
            {
                _activeBatches++;
                //Debug.Log($"Batch Added, Active: {_activeBatches}");
            }
    
            private void OnBatchFinishedHandler(ProjectileSpawnerEvents.BatchFinished input)
            {
                _activeBatches--;
        
                //Debug.Log($"Batch Removed, Active: {_activeBatches}");
                
                if (_batchesLeft <= 0 && _activeBatches <= 0)
                {
                    //Debug.Log("Finished");
                    Cleanup();
                    CurrentStatus = ActionStatus.Success;
                }
            }
            
            private void Cleanup()
            {
                ToggleSubscriptions(false);
            }
            
            private void ToggleSubscriptions(bool subscribe)
            {
                if (subscribe)
                {
                    ProjectileSpawner.Subscribe.BatchSpawned(OnBatchSpawnedHandler);
                    ProjectileSpawner.Subscribe.BatchFinished(OnBatchFinishedHandler);
                    ProjectileSpawner.Subscribe.ProjectileReachedTarget(OnProjectileReachedTargetHandler);
                }
                else
                {
                    ProjectileSpawner.Unsubscribe.BatchSpawned(OnBatchSpawnedHandler);
                    ProjectileSpawner.Unsubscribe.BatchFinished(OnBatchFinishedHandler);
                    ProjectileSpawner.Unsubscribe.ProjectileReachedTarget(OnProjectileReachedTargetHandler);
                }
            }
        }

        private class EnableShieldTypeAction : IQueueAction
        {
            public ActionStatus CurrentStatus { get; private set; } = ActionStatus.Idle; 
            
            private DynamicActionSequence _sequence;
            private readonly ShieldType _shieldType;
            private event Action OnShieldTypeStarted;

            public EnableShieldTypeAction(ShieldType shieldType)
            {
                _shieldType = shieldType;
            }

            public void OnStart()
            {
                _sequence = ActionBuilder.Start()
                    .Do(new InstantAction(()=> ShieldEventSubscriber.NotifyShieldTypeEnabled(OnShieldTypeEnabled)))
                    //.Then(new LogDebugAction("Shield Enabling"))
                    .Then(new InstantAction(() => ShieldEventCaller.RequestEnableShieldType(_shieldType)))
                        .WrapLast(a => new WaitForSignalWrapperAction(a,
                        subscribe: callback => OnShieldTypeStarted += callback,
                        unsubscribe: callback => OnShieldTypeStarted -= callback))
                    //.Then(new LogDebugAction("Shield Enable"))
                    .Then(new InstantAction(()=> ShieldEventUnSubscriber.NotifyShieldTypeEnabled(OnShieldTypeEnabled)))
                    .Build();

                CurrentStatus = ActionStatus.Running;
            }

            public ActionStatus OnUpdate(float deltaTime)
            {
                _sequence.OnUpdate(deltaTime);

                if (_sequence.CurrentStatus == ActionStatus.Success)
                {
                    CurrentStatus = ActionStatus.Success;
                }

                return CurrentStatus;
            }

            public void OnInterrupt()
            {
                CurrentStatus = ActionStatus.Failure;
            }

            public IQueueAction Copy()
            {
                return new EnableShieldTypeAction(_shieldType);
            }
            
            private void OnShieldTypeEnabled(ShieldEvents.NotifyShieldTypeEnabled input)
            {
                if (input.Type == _shieldType)
                {
                    OnShieldTypeStarted?.Invoke();
                }
            }
        }
        
        private class DisableShieldTypeAction : IQueueAction
        {
            public ActionStatus CurrentStatus { get; private set; } = ActionStatus.Idle; 
            
            private DynamicActionSequence _sequence;
            private readonly ShieldType _shieldType;
            private event Action OnShieldTypeFinished;

            public DisableShieldTypeAction(ShieldType shieldType)
            {
                _shieldType = shieldType;
            }

            public void OnStart()
            {
                _sequence = ActionBuilder.Start()
                    .Do(new InstantAction(()=> ShieldEventSubscriber.NotifyShieldTypeDisabled(OnShieldTypeDisabledHandler)))
                    .Then(new InstantAction(() => ShieldEventCaller.RequestDisableShieldType(_shieldType)))
                    .WrapLast(a => new WaitForSignalWrapperAction(a,
                        subscribe: callback => OnShieldTypeFinished += callback,
                        unsubscribe: callback => OnShieldTypeFinished -= callback))
                    .Then(new InstantAction(()=> ShieldEventUnSubscriber.NotifyShieldTypeDisabled(OnShieldTypeDisabledHandler)))
                    .Build();

                CurrentStatus = ActionStatus.Running;
            }

            public ActionStatus OnUpdate(float deltaTime)
            {
                _sequence.OnUpdate(deltaTime);

                if (_sequence.CurrentStatus == ActionStatus.Success)
                {
                    CurrentStatus = ActionStatus.Success;
                }

                return CurrentStatus;
            }

            public void OnInterrupt()
            {
                CurrentStatus = ActionStatus.Failure;
            }

            public IQueueAction Copy()
            {
                return new DisableShieldTypeAction(_shieldType);
            }
            
            private void OnShieldTypeDisabledHandler(ShieldEvents.NotifyShieldTypeDisabled input)
            {
                if (input.Type == _shieldType)
                {
                    OnShieldTypeFinished?.Invoke();
                }
            }
        }
        
        private class EnableBatchTypeAction : IQueueAction
        {
            public ActionStatus CurrentStatus { get; private set; } = ActionStatus.Idle; 
            
            private DynamicActionSequence _sequence;
            private readonly BatchType _batchType;
            private event Action OnBatchTypeStarted;

            public EnableBatchTypeAction(BatchType batchType)
            {
                _batchType = batchType;
            }

            public void OnStart()
            {
                _sequence = ActionBuilder.Start()
                    .Do(new InstantAction(ProjectileSpawner.Publish.Clear))
                    .Then(new InstantAction(ProjectileSpawner.Publish.Enable))
                    .Then(new InstantAction(() => ProjectileSpawner.Subscribe.BatchCreated(OnBatchCreatedHandler)))
                    .Then(new InstantAction(() => ProjectileSpawner.Publish.RequestSpawn(_batchType)))
                        .WrapLast(a => new WaitForSignalWrapperAction(a,
                        subscribe: callback => OnBatchTypeStarted += callback,
                        unsubscribe: callback => OnBatchTypeStarted -= callback))
                    .Then(new InstantAction(() => ProjectileSpawner.Unsubscribe.BatchCreated(OnBatchCreatedHandler)))
                    .Build();

                CurrentStatus = ActionStatus.Running;
            }

            public ActionStatus OnUpdate(float deltaTime)
            {
                _sequence.OnUpdate(deltaTime);

                if (_sequence.CurrentStatus == ActionStatus.Success)
                {
                    CurrentStatus = ActionStatus.Success;
                }
                return CurrentStatus;
            }

            public void OnInterrupt()
            {
                CurrentStatus = ActionStatus.Failure;
            }

            public IQueueAction Copy()
            {
                return new EnableBatchTypeAction(_batchType);
            }
            
            private void OnBatchCreatedHandler(ProjectileSpawnerEvents.BatchCreated input)
            {
                if (input.BatchType == _batchType)
                {
                    OnBatchTypeStarted?.Invoke();
                }
            }
        }
        
        private class SetChannelTimeScaleAction : IQueueAction
        {
            private readonly float _targetTimeScale;
            private readonly UpdateGroup[] _updateGroup;
            
            public ActionStatus CurrentStatus { get; } = ActionStatus.Success;

            public SetChannelTimeScaleAction(float targetTimeScale, UpdateGroup[] updateGroup)
            {
                this._targetTimeScale = targetTimeScale;
                _updateGroup = updateGroup;
            }

            public void OnStart()
            {
                CustomTime.SetChannelTimeScale(_updateGroup, _targetTimeScale);
            }

            public ActionStatus OnUpdate(float deltaTime) => ActionStatus.Success;
            public void OnInterrupt() { }
            public IQueueAction Copy()
            {
                return new SetChannelTimeScaleAction(_targetTimeScale, _updateGroup);
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

        #endregion
        
        private readonly Dictionary<AbilityType, AbilityStoredData> _abilities = new Dictionary<AbilityType, AbilityStoredData>();
        
        public event Action<float> OnAbilityStarted;
        public event Action<AbilityType> OnSequenceEnd;

        #region Commands/Actions

        private readonly IQueueAction _enableInputs;
        private readonly IQueueAction _disableInputs;
        
        private readonly IQueueAction _enableUIInputs;
        private readonly IQueueAction _disableUIInputs;

        private readonly IQueueAction _playSlowTimeSound;
        private readonly IQueueAction _playSpeedTimeSound;

        private readonly IQueueAction _cameraZoomIn;
        private readonly IQueueAction _cameraZoomOut;

        #endregion

        public AbilityDataController(Action speedTimeSound, Action slowTimeSound,
            ICameraTransportData zoomInData, ICameraTransportData zoomOutData)
        {
            _playSpeedTimeSound = new InstantAction(speedTimeSound);
            _playSlowTimeSound = new InstantAction(slowTimeSound);
            
            _cameraZoomIn = new InstantAction(() => CameraEventCaller.Transport(zoomInData));
            _cameraZoomOut = new InstantAction(() => CameraEventCaller.Transport(zoomOutData));
            
            _enableInputs = new SetBoolAction(true,SetInputsEnable);
            _disableInputs = new SetBoolAction(false,SetInputsEnable);
            
            _enableUIInputs = new SetBoolAction(true,SetUIInputsEnable);
            _disableUIInputs = new SetBoolAction(false,SetUIInputsEnable);
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
            var targetBatches = 3;
            var selectedAbility = AbilityType.SuperShield;
            var shieldType = ShieldType.Super;
            var batchType = BatchType.Ring;
            var timeData = configData.GetAbilityTimeData(selectedAbility);

            var shieldData = new AbilityStoredData
            {
                ActiveTime = timeData.ActiveTime,
                AbilityType = selectedAbility,
                Sequence = GetSequence()
            };

            _abilities.Add(selectedAbility, shieldData);
            return;

            IQueueAction GetSequence()
            {
                var actions = ActionBuilder.Start()
                    
                // === Start ===
                .Do(new LogDebugAction("Super Shield Starting"))
                .Then(new PublishAbilityActiveAction(selectedAbility, true))
                
                // - Disables input, plays sounds, slows time, zooms in
                .Then(_disableInputs)
                .Then(_playSlowTimeSound)
                .Then(new InstantAction(()=> CustomTime.GlobalFixedTimeScale = 0))
                .Then(new WaitSecondsAction(timeData.ZoomIn))
                .Then(_disableUIInputs)
                .Then(_cameraZoomIn)
                .Then(new WaitSecondsAction(timeData.StartAction))
                    
                // - Changes batch type and waits confirm
                .Then(new EnableBatchTypeAction(batchType))
                    
                // - Sets Shield and Waits
                .Then(new EnableShieldTypeAction(shieldType))
                    
                // - Zooms Out
                .Then(new WaitSecondsAction(timeData.ZoomOut))
                .Then(_cameraZoomOut)
                .Then(_enableUIInputs)
                .Then(new InstantAction(()=> CustomTime.GlobalFixedTimeScale = 1))
                //.Then(new LogDebugAction("Super Shield Start Finish"))
                
                // === Running ===
                //.Then(new LogDebugAction("Super Shield Running"))
                .Then(new WaitForBatchesToFinishAction(targetBatches, batchType))
                
                // === Finish ===
                //.Then(new LogDebugAction("Super Shield Finishing"))
                .Then(new InstantAction(()=> CustomTime.GlobalFixedTimeScale = 0))
                .Then(_playSlowTimeSound)
                .Then(new WaitSecondsAction(timeData.StopAction))
                .Then(new EnableBatchTypeAction(BatchType.Default))
                .Then(new DisableShieldTypeAction(shieldType))
                .Then(new InstantAction(()=> CustomTime.GlobalFixedTimeScale = 1))
                .Then(new WaitSecondsAction(timeData.SpeedUp))
                .Then(_enableInputs)
                .Then(new PublishAbilityActiveAction(selectedAbility, false))
                .Then(new InstantAction(()=> OnSequenceEnd?.Invoke(selectedAbility)))
                
                // === Build ===
                .Then(new WaitFramesAction(1))
                .Build();
                
                return actions;
            }
        }


        
        #endregion

        #region Heal
        
        private void CreateHealData(IAbilityTimeConfigData configData)
        {
            var shieldTimeScale = 0.75f;
            var selectedAbility = AbilityType.Health;
            var timeData = configData.GetAbilityTimeData(AbilityType.Health);
            
            var healData = new AbilityStoredData
            {
                AbilityType = selectedAbility,
                Sequence = GetSequence(shieldTimeScale),
            };
            
            _abilities.Add(selectedAbility, healData);
            return;
            
            IQueueAction GetSequence(float shieldMinTimeScale)
            {
                var actions = ActionBuilder.Start()
                
                // === Start ===
                .Do(new LogDebugAction("Health Starting"))
                .Then(_disableInputs)
                .Then(new PublishAbilityActiveAction(selectedAbility, true))
                .Then(new InstantAction(()=> CustomTime.GlobalFixedTimeScale = 0))
                .Then(_cameraZoomIn)
                .Then(_disableUIInputs)
                .Then(new InstantAction(EarthEventCaller.DisableDamage))
                .Then(_playSlowTimeSound)
                
                // === Running ===
                .Then(new WaitSecondsAction(timeData.StartAction))
                .Then(new InstantAction(EarthEventCaller.Heal))
                
                // === Finish ===
                .Then(new WaitSecondsAction(timeData.ZoomOut))
                .Then(_cameraZoomOut)
                .Then(_enableUIInputs)
                .Then(_enableInputs)
                .Then(new InstantAction(()=> CustomTime.GlobalFixedTimeScale = 1))
                .Then(new WaitSecondsAction(timeData.SpeedUp))
                .Then(_playSpeedTimeSound)
                .Then(new InstantAction(EarthEventCaller.EnableDamage))
                .Then(new PublishAbilityActiveAction(selectedAbility, false))
                .Then(new InstantAction(()=> OnSequenceEnd?.Invoke(selectedAbility)))
                .Do(new LogDebugAction("Health Finished"))
                
                // === Build ===
                .Then(new WaitFramesAction(1))
                .Build();
            
                return actions;
            }
        }
        
        #endregion

        #region SlowMotion

        private void CreateSlowMotionData(IAbilityTimeConfigData configData)
        {
            var targetBatches = configData.GetBatchAmount();
            var shieldType = ShieldType.Slow;
            var batchType = BatchType.SlowedDown;
            var selectedAbility = AbilityType.SlowMotion;
            var timeData = configData.GetAbilityTimeData(selectedAbility);

            var abilityData = new AbilityStoredData
            {
                ActiveTime = timeData.ActiveTime,
                AbilityType = selectedAbility,
                Sequence = GetSequence(),
            };

            _abilities.Add(selectedAbility, abilityData);
            return;
            
            
             IQueueAction GetSequence()
            {
                var actions = ActionBuilder.Start()

                // === Start ===
                .Do(new LogDebugAction("Slow Motion Starting"))
                .Then(_disableInputs)
                .Then(new PublishAbilityActiveAction(selectedAbility, true))
                .Then(new InstantAction(()=> CustomTime.GlobalFixedTimeScale = 0))
                .Then(_disableUIInputs)
                .Then(_cameraZoomIn)
                .Then(_playSlowTimeSound)
                .Then(new WaitSecondsAction(timeData.SlowDown))
                .Then(new EnableBatchTypeAction(batchType))
                .Then(new EnableShieldTypeAction(shieldType))
                .Then(new WaitSecondsAction(timeData.ZoomOut))
                .Then(_enableInputs)
                .Then(_cameraZoomOut)
                .Then(_enableUIInputs)
                .Then(new ParallelAction(new []
                {
                    new InstantAction(()=> CustomTime.GlobalFixedTimeScale = 1),
                    _cameraZoomOut
                }))
                
                // === Running ===
                .Then(new LogDebugAction("Slow Down Running"))
                .Then(new WaitForBatchesToFinishAction(targetBatches, batchType))
                
                // === Finish ===
                .Then(_disableInputs)
                .Then(_cameraZoomIn)
                .Then(_playSpeedTimeSound)
                .Then(new WaitSecondsAction(timeData.SlowDown))
                .Then(new DisableShieldTypeAction(shieldType))
                .Then(new WaitSecondsAction(timeData.ZoomOut))
                .Then(_cameraZoomOut)
                .Then(_enableInputs)
                .Then(new PublishAbilityActiveAction(selectedAbility, false))
                .Then(new InstantAction(()=> OnSequenceEnd?.Invoke(selectedAbility)))
                .Then(new LogDebugAction("Slow Motion Finished"))
                
                // === Build ===
                .Then(new WaitFramesAction(1))
                .Build();

                return actions;
            }

        }

        #endregion

        #region Double Points
        
        private void CreateDoublePointsData(IAbilityTimeConfigData configData)
        {
            var targetBatches = configData.GetBatchAmount();
            var shieldType = ShieldType.Gold;
            var batchType = BatchType.Ring;
            var selectedAbility = AbilityType.DoublePoints;
            var timeData = configData.GetAbilityTimeData(selectedAbility);
            

            var abilityData = new AbilityStoredData
            {
                ActiveTime = timeData.ActiveTime,
                AbilityType = selectedAbility,
                Sequence = GetSequence()
            };

            _abilities.Add(selectedAbility, abilityData);
            return;
            
            IQueueAction GetSequence()
            {
                var actions = ActionBuilder.Start()

                // === Start ===
                .Do(new LogDebugAction("Slow Motion Starting"))
                .Then(new PublishAbilityActiveAction(selectedAbility, true))
                .Then(_disableInputs)
                .Then(_disableUIInputs)
                .Then(_cameraZoomIn)
                .Then(new InstantAction(()=> CustomTime.GlobalFixedTimeScale = 0))
                .Then(new WaitSecondsAction(timeData.SlowDown))
                .Then(_playSlowTimeSound)
                .Then(new EnableBatchTypeAction(batchType))
                .Then(new EnableShieldTypeAction(shieldType))
                .Then(new WaitSecondsAction(timeData.ZoomOut))
                .Then(_enableInputs)
                .Then(_enableUIInputs)
                .Then(_cameraZoomOut)
                .Then(new InstantAction(()=> CustomTime.GlobalFixedTimeScale = 1))
                .Then(_playSpeedTimeSound)
                .Then(new WaitSecondsAction(timeData.SpeedUp))
                
                // === Running ===
                .Then(new LogDebugAction("Gold Shield Running"))
                .Then(new WaitForBatchesToFinishAction(targetBatches, batchType))
                
                // === Finish ===\
                .Then(new EnableBatchTypeAction(batchType))
                .Then(new DisableShieldTypeAction(shieldType))
                .Then(new PublishAbilityActiveAction(selectedAbility, false))
                .Then(new LogDebugAction("Slow Motion Starting"))
                .Then(new InstantAction(()=> OnSequenceEnd?.Invoke(selectedAbility)))
                
                // === Build ===
                .Then(new WaitFramesAction(1))
                .Build();

                return actions;
            }
        }

        #endregion

        #region Automatic

        private void CreateAutomaticData(IAbilityTimeConfigData configData)
        {
            var targetBatches = configData.GetBatchAmount();
            var shieldType = ShieldType.Automatic;
            var batchType = BatchType.Automatic;
            var selectedAbility = AbilityType.Automatic;
            var timeData = configData.GetAbilityTimeData(selectedAbility);
            

            var abilityData = new AbilityStoredData
            {
                ActiveTime = timeData.ActiveTime,
                AbilityType = selectedAbility,
                Sequence = GetSequence()
            };

            _abilities.Add(selectedAbility, abilityData);

            IQueueAction GetSequence()
            {
                // Slow Down
                var slowDownGameplay = new TimedTimeScaleUpdateAction(
                    targetValue: 0,  startValue:1, timeData.SlowDown, UpdateGroup.Gameplay );
            
                var slowDownEffects = new TimedTimeScaleUpdateAction(
                    targetValue: 0,  startValue:1, timeData.SlowDown, UpdateGroup.Effects );
            
                // Speed Up
                var speedUpTime = new TimedTimeScaleUpdateAction(
                    targetValue: 1f,  startValue:0, timeData.SpeedUp, UpdateGroup.Gameplay );
            
                var speedUpEffects = new TimedTimeScaleUpdateAction(
                    targetValue: 1f,  startValue:0, timeData.SpeedUp, UpdateGroup.Effects );
                
                var actions = ActionBuilder.Start()

                // === Start ===
                .Then(new LogDebugAction("Automatic Starting"))
                .Then(_disableInputs)
                .Then(new PublishAbilityActiveAction(selectedAbility, true))
                .Then(new ParallelAction(new [] {slowDownGameplay,slowDownEffects }))
                .Then(_disableUIInputs)
                .Then(_cameraZoomIn)
                .Then(_playSlowTimeSound)
                .Then(new WaitSecondsAction(timeData.StartAction))
                .Then(new EnableBatchTypeAction(batchType))
                .Then(new EnableShieldTypeAction(shieldType))
                .Then(_enableUIInputs)
                .Then(_cameraZoomOut)
                .Then(new LogDebugAction("Automatic Shield Active"))
                .Then(new ParallelAction(new [] {speedUpTime,speedUpEffects }))
                .Then(_playSpeedTimeSound)
                
                // === Running ===
                .Then(new LogDebugAction("Automatic Running"))
                .Then(new WaitForBatchesToFinishAction(targetBatches, batchType))
                
                // === Finish ===
                .Then(new PublishAbilityActiveAction(selectedAbility, false))
                .Then(new ParallelAction(new [] {slowDownGameplay,slowDownEffects }))
                .Then(_disableUIInputs)
                .Then(_cameraZoomIn)
                .Then(_disableInputs)
                .Then(_playSlowTimeSound)
                .Then(new WaitSecondsAction(timeData.StopAction))
                .Then(new ParallelAction(new [] {speedUpTime,speedUpEffects }))
                .Then(new EnableBatchTypeAction(BatchType.Default))
                .Then(new DisableShieldTypeAction(shieldType))
                .Then(_enableUIInputs)
                .Then(_cameraZoomOut)
                .Then(_enableInputs)
                .Then(new InstantAction(()=> OnSequenceEnd?.Invoke(selectedAbility)))
                .Then(new LogDebugAction("Automatic Finished"))
                
                // === Build ===
                .Then(new WaitFramesAction(1))
                .Build();

                return actions;
            }
        }

        #endregion
        
        #endregion
        
        private void SetInputsEnable(bool isEnable)
        {
            InputsEventCaller.SetEnable(isEnable);
        }
        
        private void SetUIInputsEnable(bool isEnable)
        {
#if UNITY_ANDROID || UNITY_IOS
            InputsEventCaller.SetUIEnable(isEnable);
#endif
        }

        public bool HasAbilityData(AbilityType abilityType)
        {
            return _abilities.ContainsKey(abilityType);
        }
        
        public IQueueAction GetActionQueue(AbilityType abilityType)
        {
            return _abilities[abilityType].GetActionQueue();
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
        public IQueueAction Sequence;
        

        public IQueueAction GetActionQueue()
        {
            return (DynamicActionSequence)Sequence.Copy();
        }
    }
}