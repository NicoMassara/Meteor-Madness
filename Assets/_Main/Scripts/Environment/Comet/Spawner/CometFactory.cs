using System;
using System.Collections.Generic;
using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.Core.FlyingObject;
using MeteorMadness.GlobalValues.Tools;
using NicolasMassara.CustomTimerManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Environment.Comet.Spawner
{
    public class CometFactory : ManagedBehavior, IUpdatable
    {
        #region Distance Tracker
        private class CometDistanceTracker
        {
            private bool _hasComet;

            public event Action<IComet> OnTargetReached;

            private class CometData
            {
                public IComet Comet;
                public Vector2 TargetPosition;
                public float LastDistance;
            }
            
            private readonly List<CometData> _trackedComets = new();
            private readonly List<CometData> _toAdd = new();
            private readonly List<CometData> _toRemove = new();

            public void AddCometToTrack(IComet comet, Vector2 targetPosition)
            {
                _toAdd.Add(new CometData
                {
                    Comet = comet,
                    TargetPosition = targetPosition,
                    LastDistance = float.MaxValue,
                    
                });
                
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (comet is IDebugComet debugComet)
                {
                    debugComet.Distance = Vector2.Distance(targetPosition, comet.Position);
                }
#endif
            }

            public void ClearComets()
            {
                foreach (var data in _trackedComets)
                {
                    data.Comet.Recycle();
                }
                
                _trackedComets.Clear();
                _toAdd.Clear();
                _toRemove.Clear();
            }

            private void RemoveCometData(CometData data)
            {
                _toRemove.Add(data);
                _hasComet = _trackedComets.Count > 0;
            }

            private void ApplyPending()
            {
                if (_toAdd.Count > 0)
                {
                    foreach (var item in _toAdd)
                    {
                        if(item.Comet == null) continue;
                        
                        if (_trackedComets.Contains(item) == false)
                        {
                            _trackedComets.Add(item);
                        }
                    }
                    
                    _toAdd.Clear();
                }

                if (_toRemove.Count > 0)
                {
                    foreach (var item in _toRemove)
                    {
                        if(item.Comet == null) continue;
                        
                        if (_trackedComets.Contains(item))
                        {
                            _trackedComets.Remove(item);
                        }
                    }
                    
                    _toRemove.Clear();
                }
                
                _hasComet = _trackedComets.Count > 0;
            }

            public void Update()
            {
                ApplyPending();
                
                if(_hasComet == false) return;

                foreach (var data in _trackedComets)
                {
                    float currentDistance = Vector2.Distance(data.TargetPosition, data.Comet.Position);
                    
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    if (data.Comet is IDebugComet debugComet)
                    {
                        debugComet.Distance = currentDistance;
                    }
#endif
                    if (data.LastDistance - currentDistance <= 0f)
                    {
                        Debug.Log("Reached Target");
                        OnTargetReached?.Invoke(data.Comet);
                        RemoveCometData(data);
                    }
                    
                    data.LastDistance = currentDistance;
                }
            }
        }
        
        #endregion

        #region Ratio Tracker
        private class CometRatioDistanceTracker
        {
            private readonly IDistanceData _spawnData;
            private IComet _comet;
            private bool _hasComet;
            private Vector2 _startPosition;
            private Vector2 _targetPosition;

            public event Action<IComet> OnDistanceReached;

            public CometRatioDistanceTracker(IDistanceData spawnData)
            {
                _spawnData = spawnData;
            }

            public void SetCometToTrack(IComet comet, Vector2 targetPosition)
            {
                _comet = comet;
                _hasComet = true;
                _startPosition = comet.Position;
                _targetPosition = targetPosition;
                
            }

            public void ClearComet()
            {
                _comet = null;
                _hasComet = false;
            }

            public void Update()
            {
                if(_hasComet == false) return;
                
                var currentDistance = Vector2.Distance(_targetPosition, _comet.Position);
                var distanceToTarget = Vector2.Distance(_startPosition, _targetPosition);
                var travelRatio = currentDistance / distanceToTarget;
                
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (_comet is IDebugComet debugComet)
                {
                    debugComet.TravelRatio = travelRatio;
                }
#endif

                if (_spawnData.TravelRatioToTriggerSpawn >= travelRatio)
                {
                    OnDistanceReached?.Invoke(_comet);
                }
            }
        }
        
        #endregion
        
        #region Spawner
        private class CometSpawner
        {
            private readonly GenericPool<CometView> _pool;
            private readonly ICometData _spawnData;
            
            private bool _isBottomSpawn;
            
            private enum SpawnPosition
            {
                BottomRight,
                BottomLeft,
                TopRight,
                TopLeft,
                UpperLeft,
                LowerLeft,
                UpperRight,
                LowerRight
            }

            private float XOffset => _spawnData.SpawnOffset.x;
            private float YOffset => _spawnData.SpawnOffset.x;

            public CometSpawner(CometView cometPrefab, ICometData data , int startCapacity = 3)
            {
                _pool = new GenericPool<CometView>(cometPrefab, startCapacity, 10, "Comet");
                _spawnData = data;
            }
            
            public IComet SpawnComet(Camera worldCamera, out Vector2 targetPosition)
            {
                var spawnState = GetSpawnPosition();
                var spawnPosition = GetSpawnWorldPosition(spawnState,worldCamera);
                
                targetPosition = GetTargetPosition(spawnState,worldCamera);
                Vector2 direction = targetPosition - spawnPosition;
                
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                var comet = _pool.Get();
                var ratio = CometHelper.GetRandomFromRange(0,1);
                var scale = GetScaleFromRatio(ratio) * Vector2.one;
                var movementSpeed = GetSpeedFromRatio(ratio);
                
                comet.SetValues(new CometValues
                {
                    MovementSpeed = movementSpeed,
                    Rotation = rotation,
                    Position = spawnPosition,
                    Scale = scale,
                    Direction = direction,
                });
                comet.OnRecycle += Comet_OnRecycleHandler;
                _isBottomSpawn = !_isBottomSpawn;

#if UNITY_EDITOR 
                /*var debugColor = _isBottomSpawn ? Color.green : Color.red;
                Debug.DrawLine(spawnPosition, targetPosition, debugColor, 5f);*/
#endif
                
                return comet;
            }

            private void Comet_OnRecycleHandler(FlyingObjectView<CometValues> item)
            {
                item.OnRecycle -= Comet_OnRecycleHandler;
                _pool.Release((CometView)item);
            }

            private float GetSpeedFromRatio(float value)
            {
                var speedValue= _spawnData.SpeedRange.x + value * (_spawnData.SpeedRange.y - _spawnData.SpeedRange.x);
                return speedValue * GetSpeedVariation();
            }

            private float GetSpeedVariation()
            {
                float value =  Random.Range(0f, _spawnData.SpeedVariation);
                return Random.Range(0,1F) > 0.5f ? 1 + value : 1 - value;
            }

            private float GetScaleFromRatio(float value)
            {
                return _spawnData.ScaleRange.x + value * (_spawnData.ScaleRange.y - _spawnData.ScaleRange.x);
            }

            #region Position

            private SpawnPosition GetSpawnPosition()
            {
                bool isUpperBottom = Random.Range(0,1) < 0.75f;
                var value = Random.Range(0, 3);
                
                return (SpawnPosition)(isUpperBottom ? value : 4 + value);
            }
            
            private Vector2 GetSpawnWorldPosition(SpawnPosition spawnPos, Camera worldCamera)
            {
                var screenX = spawnPos switch
                {
                    SpawnPosition.BottomRight or SpawnPosition.TopRight => Random.Range(Screen.width/2, Screen.width),
                    SpawnPosition.BottomLeft or SpawnPosition.TopLeft => Random.Range(0,Screen.width/2),
                    SpawnPosition.UpperLeft or SpawnPosition.LowerLeft => 0,
                    SpawnPosition.UpperRight or SpawnPosition.LowerRight => Screen.width,
                    _ =>  Random.Range(Screen.width/2, Screen.width)
                };
                var screenY = spawnPos switch
                {
                    SpawnPosition.BottomRight or SpawnPosition.BottomLeft => 0,
                    SpawnPosition.TopRight or SpawnPosition.TopLeft => Screen.height,
                    SpawnPosition.UpperLeft or SpawnPosition.UpperRight => Random.Range(Screen.height/2, Screen.height),
                    SpawnPosition.LowerLeft or SpawnPosition.LowerRight => Random.Range(0, Screen.height/2),
                    _ =>  0
                };
                
                return GetPositionInWorld(worldCamera, screenX, screenY);
            }
            
            private Vector2 GetTargetPosition(SpawnPosition spawnPos, Camera worldCamera)
            {
                var screenX = spawnPos switch
                {
                    SpawnPosition.BottomRight or SpawnPosition.TopRight => Random.Range(0, Screen.width/2),
                    SpawnPosition.BottomLeft or SpawnPosition.TopLeft => Random.Range(Screen.width/2, Screen.width),
                    SpawnPosition.UpperLeft or SpawnPosition.LowerLeft => Screen.width,
                    SpawnPosition.UpperRight or SpawnPosition.LowerRight => 0,
                    _ =>  Random.Range(0, Screen.width/2)
                };
                var screenY = spawnPos switch
                {
                    SpawnPosition.BottomRight or SpawnPosition.BottomLeft => Screen.height,
                    SpawnPosition.TopRight or SpawnPosition.TopLeft => 0,
                    SpawnPosition.UpperLeft or SpawnPosition.LowerLeft => Random.Range(0, Screen.height/2),
                    SpawnPosition.UpperRight or SpawnPosition.LowerRight => Random.Range(Screen.height/2, Screen.height),
                    _ =>  Screen.height
                };
                
                return GetPositionInWorld(worldCamera, screenX, screenY);
            }

            private Vector2 GetPositionInWorld(Camera worldCamera, float screenX, float screenY)
            {
                return worldCamera.ScreenToWorldPoint(new Vector3(screenX + XOffset, screenY + YOffset, -10f));
            }
            
            #endregion
        }
        
        #endregion

        [SerializeField] private CometView cometPrefab;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private CometSpawnDataSo spawnData;
        public bool debugEnable;

        private CometSpawner _spawner;
        private CometRatioDistanceTracker _ratioTracker;
        private CometDistanceTracker _distanceTracker;
        private TimerManager.GeneratedId _spawnTimerId;
        

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        public TickGroup SelfTickGroup { get; } = TickGroup.QuarterTarget;

        private void Awake()
        {
            BootEvents.OnSubSystemRequestInitialize += Initialize;
        }
        
        private void Initialize()
        {            
            BootEvents.OnSubSystemRequestInitialize -= Initialize;

            _spawner = new CometSpawner(cometPrefab, spawnData);
            _ratioTracker = new CometRatioDistanceTracker(spawnData);
            _ratioTracker.OnDistanceReached += RatioTracker_OnDistanceReached;
            _distanceTracker = new CometDistanceTracker();
            _distanceTracker.OnTargetReached += DistanceTracker_OnTargetReached;
            
            SubscribeToEventBus();
            
            BootEvents.SubSystemInitialized();
        }

        public void ExecuteUpdate(float deltaTime)
        {
            _ratioTracker.Update();
            _distanceTracker.Update();
        }

        private void SpawnComet()
        {
            var comet = _spawner.SpawnComet(gameCamera, out Vector2 targetPosition);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            if(comet is IDebugComet debugComet)
                debugComet.DebugEnable = debugEnable;
#endif
            _ratioTracker.SetCometToTrack(comet,targetPosition);
            _distanceTracker.AddCometToTrack(comet,targetPosition);
        }

        #region Timer
        
        private void SetTimerToSpawn()
        {
            var variation = spawnData.SpawnDelayVariation;
            var spawnDelay = CometHelper.GetRandomFromRange(spawnData.SpawnDelay - variation, spawnData.SpawnDelay + variation);
            _spawnTimerId = TimerManager.Add(new TimerData(spawnDelay, SpawnComet));
        }

        private void PauseTimer() => TimerManager.Pause(_spawnTimerId);

        private void ResumeTimer() => TimerManager.Resume(_spawnTimerId);
        private void RemoveTimer() => TimerManager.Remove(_spawnTimerId);

        #endregion

        #region Handlers
        
        private void RatioTracker_OnDistanceReached(IComet comet)
        {
            _ratioTracker.ClearComet();
            SetTimerToSpawn();
        }
        
        private void DistanceTracker_OnTargetReached(IComet comet)
        {
            comet.Recycle();
        }
        
        #endregion

        #region Event Bus

        private void SubscribeToEventBus()
        {
            CometSpawnEventSubscriber.Enable(EventBus_Comet_Enable);
            CometSpawnEventSubscriber.Disable(EventBus_Comet_Disable);
            CometSpawnEventSubscriber.Pause(EventBus_Comet_Pause);
            CometSpawnEventSubscriber.Resume(EventBus_Comet_Resume);
        }

        private void EventBus_Comet_Enable(CometSpawnEvents.Enable input)
        {
            SetTimerToSpawn();
        }
        
        private void EventBus_Comet_Disable(CometSpawnEvents.Disable input)
        {
            _ratioTracker.ClearComet();
            _distanceTracker.ClearComets();
            RemoveTimer();
        }
        
        private void EventBus_Comet_Pause(CometSpawnEvents.Pause input)
        {
            PauseTimer();
        }
        
        private void EventBus_Comet_Resume(CometSpawnEvents.Resume input)
        {
            ResumeTimer();
        }

        #endregion
    }
}