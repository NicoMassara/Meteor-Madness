using System;
using System.Collections.Generic;
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
        private class CometDistanceTracker
        {
            private const float DistanceThreshold = 0.5f;
            private bool _hasComet;

            public event Action<IComet> OnTargetReached;

            private class CometData
            {
                public IComet Comet;
                public Vector2 TargetPosition;
            }
            
            private readonly List<CometData> _trackedComets = new();

            public void AddCometToTrack(IComet comet, Vector2 targetPosition)
            {
                _trackedComets.Add(new CometData
                {
                    Comet = comet,
                    TargetPosition = targetPosition,
                });
                
                _hasComet = true;
            }

            private void RemoveCometData(CometData data)
            {
                _trackedComets.Remove(data);
                _hasComet = _trackedComets.Count > 0;
            }

            public void Update()
            {
                if(_hasComet == false) return;

                foreach (var data in _trackedComets)
                {
                    var currentDistance = Vector2.Distance(data.TargetPosition, data.Comet.Position);

                    if (currentDistance <= DistanceThreshold) continue;
                    {
                        OnTargetReached?.Invoke(data.Comet);
                        RemoveCometData(data);
                    }
                }
            }
        }
        private class CometRatioDistanceTracker
        {
            private IComet _comet;
            private IDistanceData _spawnData;
            private bool _hasComet;
            private Vector2 _startPosition;
            private Vector2 _targetPosition;

            public event Action<IComet> OnDistanceReached;

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
                var travelRatio = currentDistance / Vector2.Distance(_startPosition, _targetPosition);

                if (_spawnData.TravelRatioToTriggerSpawn >= travelRatio)
                {
                    OnDistanceReached?.Invoke(_comet);
                }
            }
        }
        private class CometSpawner
        {
            private readonly GenericPool<CometView> _pool;
            
            private ICometData _spawnData;
            private bool _isBottomSpawn;

            private float XOffset => _spawnData.SpawnOffset.x;
            private float YOffset => _spawnData.SpawnOffset.x;

            public CometSpawner(CometView cometPrefab, int startCapacity = 3)
            {
                _pool = new GenericPool<CometView>(cometPrefab, startCapacity);
            }

            private Vector2 GetSpawnWorldPosition(Camera worldCamera)
            {
                var screenX = Random.Range(XOffset , Screen.width - XOffset);
                var screenY = _isBottomSpawn ? YOffset : Screen.height - YOffset;
                return GetPositionInWorld(worldCamera, screenX, screenY);
            }
            
            private Vector2 GetTargetPosition(Camera worldCamera)
            {
                var screenX = Random.Range(XOffset , Screen.width - XOffset);
                var screenY = _isBottomSpawn ? Screen.height - YOffset : YOffset;
                return GetPositionInWorld(worldCamera, screenX, screenY);
            }

            private Vector2 GetPositionInWorld(Camera worldCamera, float screenX, float screenY)
            {
                return worldCamera.ScreenToWorldPoint(new Vector3(screenX, screenY, -10f));
            }

            public IComet SpawnComet(Camera worldCamera, out Vector2 targetPosition)
            {
                var spawnPosition = GetSpawnWorldPosition(worldCamera);
                targetPosition = GetTargetPosition(worldCamera);
                Vector2 direction = spawnPosition - targetPosition;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                var comet = _pool.Get();
                var scale = Vector2.one * Random.Range(_spawnData.ScaleRange.x, _spawnData.ScaleRange.y);
                comet.SetValues(new CometValues
                {
                    MovementSpeed = GetMovementSpeed(),
                    Rotation = rotation,
                    Position = spawnPosition,
                    Scale = scale,
                    Direction = direction,
                });
                comet.OnRecycle += Comet_OnRecycleHandler;

                return comet;
            }

            private void Comet_OnRecycleHandler(FlyingObjectView<CometValues> item)
            {
                item.OnRecycle -= Comet_OnRecycleHandler;
                _pool.Release((CometView)item);
            }

            private float GetMovementSpeed()
            {
                var variation = _spawnData.SpeedVariation;
                return CometHelper.GetRandomFromRange(_spawnData.MovementSpeed - variation, _spawnData.MovementSpeed + variation);
            }
        }

        [SerializeField] private CometView cometPrefab;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private CometSpawnDataSo spawnData;

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

            _spawner = new CometSpawner(cometPrefab);
            _ratioTracker = new CometRatioDistanceTracker();
            _ratioTracker.OnDistanceReached += RatioTracker_OnDistanceReached;
            _distanceTracker = new CometDistanceTracker();
            _distanceTracker.OnTargetReached += DistanceTracker_OnTargerReached;

            SetFirstSpawnTimer();
            
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
            _ratioTracker.SetCometToTrack(comet,targetPosition);
            _distanceTracker.AddCometToTrack(comet,targetPosition);
        }

        #region Timer

        private void SetFirstSpawnTimer()
        {
            _spawnTimerId = TimerManager.Add(new TimerData(spawnData.FirstSpawnDelay, SpawnComet));
        }
        
        private void SetTimerToSpawn()
        {
            var variation = spawnData.SpawnDelayVariation;
            var spawnDelay = CometHelper.GetRandomFromRange(spawnData.SpawnDelay - variation, spawnData.SpawnDelay + variation);
            _spawnTimerId = TimerManager.Add(new TimerData(spawnDelay, SpawnComet));
        }

        private void PauseTimer() => TimerManager.Pause(_spawnTimerId);

        private void ResumeTimer() => TimerManager.Resume(_spawnTimerId);

        #endregion

        #region Handlers
        
        private void RatioTracker_OnDistanceReached(IComet comet)
        {
            _ratioTracker.ClearComet();
            SetTimerToSpawn();
        }
        
        private void DistanceTracker_OnTargerReached(IComet comet)
        {
            comet.Recycle();
        }
        
        #endregion
    }
}