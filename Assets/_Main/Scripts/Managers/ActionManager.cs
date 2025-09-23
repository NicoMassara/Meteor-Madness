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
        
        private readonly List<ActionQueueData> _running = new List<ActionQueueData>();
        private readonly List<ActionQueueData> _toAdd = new List<ActionQueueData>();
        private readonly List<ActionQueueData> _toRemove = new List<ActionQueueData>();
        private class ActionQueueData
        {
            public UpdateGroup UpdateGroup;
            public ActionQueue ActionQueue;
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
                }
            
                _toAdd.Clear();
            }
            
            if (_toRemove.Count > 0)
            {
                foreach (var data in _toRemove.ToList())
                {
                    activeCount--;
                    _running.Remove(data);
                }
            
                _toRemove.Clear();
            }
        }

        public static void Add(ActionQueue queueData, UpdateGroup updateGroup)
        {
            Instance.activeCount++;
            Instance._toAdd.Add(new ActionQueueData { UpdateGroup = updateGroup, ActionQueue = queueData });
        }
    }
}