using System;
using System.Collections.Generic;
using System.Linq;
using _Main.Scripts.DebugGUI;
using _Main.Scripts.InspectorTools;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class TimerManager : ManagedBehavior, IUpdatable
    {
        public static TimerManager Instance =>  _instance != null ? _instance : (_instance = CreateInstance());
        protected static TimerManager _instance;

        private readonly RandomIdGenerator _idStorage = new RandomIdGenerator();

        private readonly List<ulong> _cancelAddIds = new List<ulong>();
        private readonly List<TimerManagerData> _running = new List<TimerManagerData>();
        private readonly List<TimerManagerData> _toAdd = new List<TimerManagerData>();
        private readonly List<TimerManagerData> _toRemove = new List<TimerManagerData>();
        private readonly Dictionary<ulong, TimerManagerData> _timerDic = new Dictionary<ulong, TimerManagerData>();
        private int RunningCount => _running.Count;

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Always;
        public TickGroup SelfTickGroup { get; } = TickGroup.QuarterTick;
        public float LastUpdateTime { get; set; }


        private class TimerManagerData
        {
            public Timer Timer;
            public UpdateGroup UpdateGroup;
            public ulong Id;
            public TimerId ExternalId;
        }

        public class TimerId
        {
            public ulong Id { get; private set; }
            public bool IsActive => Id > 0;

            public TimerId()
            {
                Id = 0;
            }

            public TimerId(ulong id)
            {
                this.Id = id;
            }

            public void Reset()
            {
                Id = 0;
            }
        }


        [SerializeField, ReadOnly] 
        private int activeCount = 0;
        
        private static TimerManager CreateInstance()
        {
            var gameObject = new GameObject(nameof(TimerManager))
            {
                hideFlags = HideFlags.DontSave,
            };
            DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<TimerManager>();
        }

        private void Awake()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            DebugGUIManager.Instance.CreateGroup(DebugGUIKeys.Group.Managers, DebugGUISortingOrder.Group.Managers)
                ?.AddEntry(
                    () => $"Timer Count: {RunningCount}"
                );
#endif
        }

        public void ExecuteUpdate()
        {
            ApplyPending();
            
            if(_running.Count == 0) return;
            
            foreach (var data in _running.ToList())
            {
                var dt = CustomTime.GetDeltaTimeByChannel(data.UpdateGroup);
                data.Timer.Run(dt);

                if (data.Timer.HasEnded)
                {
                    _toRemove.Add(data);
                }
            }
            
            ApplyPending();
        }
        
        private void ApplyPending()
        {
            if (_toAdd.Count > 0)
            {
                var cancelIds = new HashSet<ulong>(_cancelAddIds);

                foreach (var data in _toAdd)
                {
                    if (cancelIds.Contains(data.Id))
                    {
                        data.ExternalId.Reset();
                        _idStorage.Release(data.Id);
                        continue; 
                    }

                    activeCount++;
                    _running.Add(data);
                    _timerDic.Add(data.Id, data);
                }

                _toAdd.Clear();
            }
            
            if (_toRemove.Count > 0)
            {
                foreach (var data in _toRemove.ToList())
                {
                    activeCount--;
                    data.ExternalId.Reset();
                    _running.Remove(data);
                    _timerDic.Remove(data.Id);
                    _idStorage.Release(data.Id);
                }
            
                _toRemove.Clear();
            }
        }
        
        public static void Clear()
        {
            Instance._Clear();
        }
        
        private void _Clear()
        {
            foreach (var timer in _running)
            {
                _toRemove.Add(timer);
            }
        }
        
        public static TimerId Add(TimerData timerData, UpdateGroup updateGroup = UpdateGroup.Always)
        {
            return Instance._Add(timerData,updateGroup);
        }

        private TimerId _Add(TimerData timerData,UpdateGroup updateGroup = UpdateGroup.Always)
        {
            var generatedId = _idStorage.Generate();
            var externalId = new TimerId(generatedId);
            
            _toAdd.Add(new TimerManagerData
            {
                Timer = new Timer(timerData),
                UpdateGroup = updateGroup, 
                Id = generatedId,
                ExternalId = externalId
            });

            return externalId;
        }

        public static void Remove(ulong id)
        {
            Instance._Remove(id);
        }

        private void _Remove(ulong id)
        {
            if (_timerDic.TryGetValue(id, out var value))
            {
                Debug.Log($"Timer removed from running, id:{id}");
                _toRemove.Add(value);
            }
            else
            {
                Debug.Log($"Timer added to cancel queue, id:{id}");
                _cancelAddIds.Add(id);
            }
        }
    }
}