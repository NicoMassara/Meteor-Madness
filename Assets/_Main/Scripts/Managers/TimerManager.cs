using System.Collections.Generic;
using System.Linq;
using _Main.Scripts.InspectorTools;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class TimerManager : MonoBehaviour
    {
        public static TimerManager Instance =>  _instance != null ? _instance : (_instance = CreateInstance());
        protected static TimerManager _instance;
        
        private readonly List<TimerManagerData> _running = new List<TimerManagerData>();
        private readonly List<TimerManagerData> _toAdd = new List<TimerManagerData>();
        private readonly List<TimerManagerData> _toRemove = new List<TimerManagerData>();

        private class TimerManagerData
        {
            public Timer Timer;
            public UpdateGroup UpdateGroup;
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

        private void Update()
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

        public static void AddTimer(TimerData timerData,UpdateGroup updateGroup = UpdateGroup.Always)
        {
            Instance._toAdd.Add(new TimerManagerData { Timer = new Timer(timerData), UpdateGroup = updateGroup });
        }
    }
}