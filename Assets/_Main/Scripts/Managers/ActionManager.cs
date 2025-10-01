using System.Collections.Generic;
using System.Linq;
using _Main.Scripts.InspectorTools;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class ActionManager : MonoBehaviour
    {
        public static ActionManager Instance =>  _instance != null ? _instance : (_instance = CreateInstance());
        private static ActionManager _instance;
        
        private readonly RandomIdGenerator _idStorage = new RandomIdGenerator();
        
        private readonly List<ActionQueueData> _running = new List<ActionQueueData>();
        private readonly List<ActionQueueData> _toAdd = new List<ActionQueueData>();
        private readonly List<ActionQueueData> _toRemove = new List<ActionQueueData>();
        private readonly Dictionary<ulong, ActionQueueData> _idsDic = new Dictionary<ulong, ActionQueueData>();
        
        private class ActionQueueData
        {
            public UpdateGroup UpdateGroup;
            public ActionQueue ActionQueue;
            public ulong Id;
        }
        
        [SerializeField, ReadOnly] 
        private int activeCount = 0;
        
        private static ActionManager CreateInstance()
        {
            var gameObject = new GameObject(nameof(ActionManager))
            {
                hideFlags = HideFlags.DontSave,
            };
            DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<ActionManager>();
        }
        
        private void Update()
        {
            ApplyPending();
            
            if (_running.Count == 0) return;
            
            foreach (var data in _running.ToList())
            {
                var dt = CustomTime.GetDeltaTimeByChannel(data.UpdateGroup);
                data.ActionQueue.Run(dt);

                if (data.ActionQueue.IsEmpty)
                {
                    activeCount--;
                    _toRemove.Add(data);
                }
            }
            
            ApplyPending();
        }

        private void ApplyPending()
        {
            if (_toAdd.Count > 0)
            {
                foreach (var data in _toAdd.ToList())
                {
                    activeCount++;
                    _running.Add(data);
                    _idsDic.Add(data.Id, data);
                }
            
                _toAdd.Clear();
            }
            
            if (_toRemove.Count > 0)
            {
                foreach (var data in _toRemove.ToList())
                {
                    activeCount--;
                    _running.Remove(data);
                    _idsDic.Remove(data.Id);
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
            foreach (var value in _running)
            {
                _toRemove.Add(value);
            }
        }
        
        public static ulong Add(ActionQueue queueData, UpdateGroup updateGroup = UpdateGroup.Always)
        {
            return Instance._Add(queueData,updateGroup);
        }

        private ulong _Add(ActionQueue queueData, UpdateGroup updateGroup = UpdateGroup.Always)
        {
            var id = _idStorage.Generate();
            _toAdd.Add(new ActionQueueData { UpdateGroup = updateGroup, ActionQueue = queueData, Id = id});
            return id;
        }
        
        public static void Remove(ulong id)
        {
            Instance._Remove(id);
        }

        private void _Remove(ulong id)
        {
            if (_idsDic.TryGetValue(id, out var value))
            {
                _toRemove.Add(value);
            }
        }
        
        
    }
}