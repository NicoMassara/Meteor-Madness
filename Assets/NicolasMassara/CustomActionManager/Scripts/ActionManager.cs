using System;
using System.Collections.Generic;
using UnityEngine;

namespace NicolasMassara.CustomActionManager
{
    public class ActionManager : MonoBehaviour
    {
        public enum PriorityTick
        {
            None,
            Low,
            MediumLow,
            Medium,
            MediumHigh,
            High,
            EveryFrame
        }

        public enum UpdateType
        {
            None, 
            Update,
            Fixed,
            Late
        }
        
        //====================================================
        //                       SINGLETON
        //====================================================
        
        public static ActionManager Instance =>  _instance != null ? _instance : (_instance = CreateInstance());
        private static ActionManager _instance;
        
        private static ActionManager CreateInstance()
        {
            var gameObject = new GameObject(nameof(ActionManager))
            {
                hideFlags = HideFlags.DontSave,
            };
            DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<ActionManager>();
        }
        
        #region ID Generator

        public class GeneratedId
        {
            public ulong Id { get; private set; }
            public bool IsActive => Id > 0;
            private event Action<GeneratedId> _onRelease;

            public GeneratedId()
            {
            }

            public GeneratedId(ulong id, Action<GeneratedId> onRelease)
            {
                Id = id;
                _onRelease = onRelease;
            }

            public void Release()
            {
                _onRelease?.Invoke(this);
            }

            public void Reset()
            {
                Id = 0;
            }
        }

        private class RandomIdGenerator
        {
            private const ulong NullId = 0; // Default ID used as a null
            
            private readonly HashSet<ulong> _inUseId = new HashSet<ulong>(); // In Used ID List 
            private readonly System.Random _random = new System.Random();
            
            /// <summary>
            /// Generates a random GeneratedId
            /// That contains an ulong used as the ID
            /// </summary>
            /// <returns></returns>
            public GeneratedId Generate()
            {
                ulong value;
                int attempts = 0;

                do
                {
                    value = NextUlong();
                    attempts++;

                    if (attempts > 100)
                    {
                        break;
                    }

                } while (_inUseId.Contains(value));

                _inUseId.Add(value);
                
                var generatedId = new GeneratedId(value,Release);
                
                return generatedId;
            }
            
            private void Release(GeneratedId idData)
            {
                _inUseId.Remove(idData.Id);
                idData.Reset();
            }
            
            private ulong NextUlong()
            {
                ulong value;

                do
                {
                    byte[] bytes = new byte[8];
                    _random.NextBytes(bytes);
                    value = BitConverter.ToUInt64(bytes, 0);

                } while (value == NullId);

                return value;
            }
        }

        #endregion
        
        #region Action Factory

        public class ActionFactory
        {
            private readonly List<ActionQueueRunner> _availableQueue = new List<ActionQueueRunner>();
            private int AvailableCount => _availableQueue.Count;

            public ActionFactory(int initializeCount = 10)
            {
                for (int i = 0; i < initializeCount; i++)
                    _availableQueue.Add(new ActionQueueRunner());
            }

            public ActionQueueRunner GetActionQueue()
            {
                if (AvailableCount == 0) return new ActionQueueRunner();

                var timer = _availableQueue[0];
                _availableQueue.RemoveAt(0);
                return timer;
            }

            public void ReturnActionQueue(ActionQueueRunner actionQueue)
            {
                actionQueue.ResetValues();
                _availableQueue.Add(actionQueue);
            }
        }

        #endregion

        #region QueueRunner

        public class ActionQueueRunner
        {
            private class TimerTools
            {
                public static float GetPriorityTick(PriorityTick group, float frameTime, float targetFrameRate)
                {
                    float baseFrameTime = targetFrameRate > 0 ? 1f / targetFrameRate : frameTime;

                    return group switch
                    {
                        PriorityTick.EveryFrame => baseFrameTime,
                        PriorityTick.High => baseFrameTime * 2,
                        PriorityTick.MediumHigh => baseFrameTime * 4,
                        PriorityTick.Medium => baseFrameTime * 8,
                        PriorityTick.MediumLow => baseFrameTime * 16,
                        PriorityTick.Low => baseFrameTime * 32,
                        PriorityTick.None => 1f,
                        _ => baseFrameTime
                    };
                }
            }
            
            private IQueueAction _current = null;
            private bool _isPaused = false;
            private float _elapsedSinceLastTick = 0;
            private int _targetFrameRate = -1;
            private PriorityTick _priority = PriorityTick.EveryFrame;
            private ActionStatus _status = ActionStatus.Idle;
            public bool IsRunning => _status == ActionStatus.Running;
            public bool HasFinished => _status == ActionStatus.Success || _status == ActionStatus.Failure;
            
            
            #region Setters

            public ActionQueueRunner AddAction(IQueueAction data)
            {
                _current = data;
                
                return this;
            }
            
            public ActionQueueRunner SetTargetFrameRate(int targetFrameRate)
            {
                _targetFrameRate = targetFrameRate;
                return this;
            }

            public ActionQueueRunner SetPriority(PriorityTick priority)
            {
                _priority = priority;
                return this;
            }

            #endregion

            #region Actions

            public void Pause() => _isPaused = true;
            public void Resume() => _isPaused = false;

            public void Interrupt()
            {
                _current?.OnInterrupt();
                _current = null;
                _status = ActionStatus.Success;
            }

            #endregion
            
            public void Execute(float deltaTime, float frameTime)
            {
                if (_isPaused) return;

                if (_current == null)
                {
                    Debug.Log("Current is Null");
                    _status = ActionStatus.Failure;
                    return;
                }

                UpdateActionQueue(deltaTime, frameTime);
            }

            public void ResetValues()
            {
                _current = null;
                _isPaused = false;
                _status = ActionStatus.Idle;
                _elapsedSinceLastTick = 0;
            }

            private void UpdateActionQueue(float deltaTime, float frameTime)
            {
                _status = ActionStatus.Running;

                if (_priority == PriorityTick.EveryFrame)
                {
                    _current.OnUpdate(deltaTime);
                }
                else
                {
                    _elapsedSinceLastTick += deltaTime;
                    float interval = TimerTools.GetPriorityTick(_priority, frameTime, _targetFrameRate);

                    while (_elapsedSinceLastTick >= interval)
                    {
                        _current.OnUpdate(interval);

                        _elapsedSinceLastTick -= interval;
                    }
                }


                if (_current.CurrentStatus == ActionStatus.Success)
                {
                    _status = ActionStatus.Success;
                }
            }
        }

        #endregion

        #region Runner

        private interface IRunner
        {
            public int RunningCount { get; }
            public void Execute(float deltaTime, float frameTime);
            public void Pause(ulong id);
            public void Resume(ulong id);
            public ActionQueueRunner GetActionQueue(ulong id);
            public void Add(ActionQueueData queueData);
            public void Remove(ulong id);
            public void Clear();
        }
        
        private class Runner : IRunner
        {
            private readonly List<ulong> _cancelIds = new List<ulong>();
            private readonly List<ActionQueueData> _running = new List<ActionQueueData>();
            private readonly List<ActionQueueData> _toAdd = new List<ActionQueueData>();
            private readonly List<ActionQueueData> _toRemove = new List<ActionQueueData>();
            private readonly Dictionary<ulong, ActionQueueData> _idsDic = new Dictionary<ulong, ActionQueueData>();
            private event Action<ActionQueueRunner> OnReturned;
            private event Action<ulong> OnRemoved;
            
            public int RunningCount => _running.Count;

            public Runner(Action<ActionQueueRunner> onReturned, Action<ulong> onRemoved)
            {
                OnReturned = onReturned;
                OnRemoved = onRemoved;
            }
            
            private void ApplyPending()
            {
                if (_toAdd.Count > 0)
                {
                    var cancelIds = new HashSet<ulong>(_cancelIds);
                
                    foreach (var data in _toAdd)
                    {
                        if (cancelIds.Contains(data.ExternalId.Id))
                        {
                            data.ExternalId.Release();
                            continue;
                        }
                    
                        _running.Add(data);
                        _idsDic.Add(data.ExternalId.Id, data);
                    }
            
                    _toAdd.Clear();
                }
            
                if (_toRemove.Count > 0)
                {
                    foreach (var data in _toRemove)
                    {
                        _running.Remove(data);
                        _idsDic.Remove(data.ExternalId.Id);
                        OnReturned?.Invoke(data.ActionQueue);
                        OnRemoved?.Invoke(data.ExternalId.Id);
                        data.ExternalId.Release();
                    }
            
                    _toRemove.Clear();
                }
            }
                
            #region IRunner

            public void Execute(float deltaTime, float frameTime)
            {
                ApplyPending();
            
                if (_running.Count == 0) return;
            
                foreach (var data in _running)
                {
                    data.ActionQueue.Execute(Time.deltaTime, Time.unscaledDeltaTime);

                    if (data.ActionQueue.HasFinished)
                    {
                        _toRemove.Add(data);
                    }
                }
            
                ApplyPending();
            }

            public void Pause(ulong id)
            {
                _idsDic[id].ActionQueue.Pause();
            }

            public void Resume(ulong id)
            {
                _idsDic[id].ActionQueue.Resume();
            }

            public ActionQueueRunner GetActionQueue(ulong id)
            {
                return _idsDic[id].ActionQueue;
            }

            public void Add(ActionQueueData queueData)
            {
                _toAdd.Add(queueData);
            }

            public void Remove(ulong id)
            {
                // If _idsDic doesn't have it, it means it's not running yet, so it can be cancelled
                
                if (_idsDic.ContainsKey(id) == false)
                {
                    _cancelIds.Add(id);
                }
                else
                {
                    _idsDic[id].ActionQueue.Interrupt();
                }
            }

            public void Clear()
            {
                foreach (var value in _running)
                {
                    _toRemove.Add(value);
                }
            }
            
            #endregion
        }
        
        #endregion

        private IRunner _updateRunner;
        private IRunner _fixedUpdateRunner;
        private IRunner _lateUpdateRunner;

        private readonly ActionFactory _actionFactory = new ActionFactory();
        private readonly RandomIdGenerator _idStorage = new RandomIdGenerator();
        
        private readonly Dictionary<ulong, UpdateType> _updateById = new Dictionary<ulong, UpdateType>();
        
        public class ActionQueueData
        {
            public ActionQueueRunner ActionQueue { get; private set; }
            public GeneratedId ExternalId { get; private set; }
            public UpdateType UpdateType { get; private set; }

            public ActionQueueData(ActionQueueRunner actionQueue, GeneratedId externalId, UpdateType updateType)
            {
                ActionQueue = actionQueue;
                ExternalId = externalId;
                UpdateType = updateType;
            }
        }

        private void Awake()
        {
            _updateRunner = new Runner(_actionFactory.ReturnActionQueue, RemoveFromUpdateDic);
            _fixedUpdateRunner = new Runner(_actionFactory.ReturnActionQueue, RemoveFromUpdateDic);
            _lateUpdateRunner = new Runner(_actionFactory.ReturnActionQueue, RemoveFromUpdateDic);
        }

        private void RemoveFromUpdateDic(ulong id)
        {
            if(_updateById.ContainsKey(id) == false) return;
            
            _updateById.Remove(id);
        }

        #region Action Logic

        private void Update()
        {
            _updateRunner?.Execute(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void FixedUpdate()
        {
            _fixedUpdateRunner?.Execute(Time.fixedTime, Time.fixedUnscaledTime);
        }

        private void LateUpdate()
        {
            _lateUpdateRunner?.Execute(Time.deltaTime, Time.unscaledDeltaTime);
        }
        

        #endregion
        
        #region Public API

        #region ActionQueue Settings
        public static bool Pause(GeneratedId id) => Instance.PauseInternal(id);
        public static bool Resume(GeneratedId id) => Instance.ResumeInternal(id);

        #endregion

        public static ActionQueueRunner GetActionQueue(GeneratedId id) => Instance.GetActionQueueInternal(id);
        public static GeneratedId Add(IQueueAction queueData, UpdateType updateType, PriorityTick priority = PriorityTick.EveryFrame) => Instance.AddInternal(queueData,updateType,priority);
        public static bool Remove(GeneratedId id) => Instance.RemoveInternal(id);
        public static void Clear(UpdateType updateType) => Instance.ClearInternal(updateType);

        #endregion

        #region Internal API

        #region ActionQueue Settings

        private bool DoesContainIdInRunner(GeneratedId id)
        {
            return _updateById.ContainsKey(id.Id);
        }


        private bool PauseInternal(GeneratedId id)
        {
            if(id == null) return false;

            if (DoesContainIdInRunner(id) == false) return false;

            var updateType = _updateById[id.Id];

            switch (updateType)
            {
                case UpdateType.Update:
                    _updateRunner.Pause(id.Id);
                    break;
                case UpdateType.Fixed:
                    _fixedUpdateRunner.Pause(id.Id);
                    break;
                case UpdateType.Late:
                    _lateUpdateRunner.Pause(id.Id);
                    break;
            }
            

            return true;

        }
        
        private bool ResumeInternal(GeneratedId id)
        {
            if(id == null) return false;

            if (DoesContainIdInRunner(id) == false) return false;

            var updateType = _updateById[id.Id];

            switch (updateType)
            {
                case UpdateType.Update:
                    _updateRunner.Resume(id.Id);
                    break;
                case UpdateType.Fixed:
                    _fixedUpdateRunner.Resume(id.Id);
                    break;
                case UpdateType.Late:
                    _lateUpdateRunner.Resume(id.Id);
                    break;
            }
            
            return true;
        }

        #endregion


        private ActionQueueRunner GetActionQueueInternal(GeneratedId id)
        {
            if(id == null) return null;
            if (DoesContainIdInRunner(id) == false) return null;
            
            var updateType = _updateById[id.Id];

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
            return updateType switch
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
            {
                UpdateType.Update => _updateRunner.GetActionQueue(id.Id),
                UpdateType.Fixed => _fixedUpdateRunner.GetActionQueue(id.Id),
                UpdateType.Late => _lateUpdateRunner.GetActionQueue(id.Id)
            };
        }
        
        private GeneratedId AddInternal(IQueueAction queueData, UpdateType updateType, PriorityTick priority = PriorityTick.EveryFrame)
        {
            if (updateType == UpdateType.None)
            {
                Debug.Log("Action Will not be Added! UpdateType must be set to be added!");
                return null;
            }

            var generatedId = _idStorage.Generate();
            var action = _actionFactory.GetActionQueue();
            action.AddAction(queueData).SetTargetFrameRate(Application.targetFrameRate).SetPriority(priority);
            var actionData = new ActionQueueData(action, generatedId, updateType);

            switch (updateType)
            {
                case UpdateType.Update:
                    _updateRunner.Add(actionData);
                    break;
                case UpdateType.Fixed:
                    _fixedUpdateRunner.Add(actionData);
                    break;
                case UpdateType.Late:
                    _lateUpdateRunner.Add(actionData);
                    break;
            }
            
            _updateById.Add(generatedId.Id, updateType);
            
            return generatedId;
        }
        
        private bool RemoveInternal(GeneratedId id)
        {
            if(id == null) return false;
            if (DoesContainIdInRunner(id) == false) return false;
            
            var updateType = _updateById[id.Id];

            switch (updateType)
            {
                case UpdateType.Update:
                    _updateRunner.Remove(id.Id);
                    break;
                case UpdateType.Fixed:
                    _fixedUpdateRunner.Remove(id.Id);
                    break;
                case UpdateType.Late:
                    _lateUpdateRunner.Remove(id.Id);
                    break;
            }
            
            return true;
        }

        private void ClearInternal(UpdateType updateType)
        {
            switch (updateType)
            {
                case UpdateType.Update:
                    _updateRunner.Clear();
                    break;
                case UpdateType.Fixed:
                    _fixedUpdateRunner.Clear();
                    break;
                case UpdateType.Late:
                    _lateUpdateRunner.Clear();
                    break;
            }
        }

        #endregion
    }
}