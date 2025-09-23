using System.Collections;
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

        [SerializeField, ReadOnly] 
        private int _timerRunningCount = 0;
        
        private static TimerManager CreateInstance()
        {
            var gameObject = new GameObject(nameof(TimerManager))
            {
                hideFlags = HideFlags.DontSave,
            };
            DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<TimerManager>();
        }

        public static void SetTimer(TimerData timerData,UpdateGroup updateGroup = UpdateGroup.Always)
        {
            Instance.StartCoroutine(Instance.Coroutine_RunTimer(timerData, updateGroup));
        }

        private IEnumerator Coroutine_RunTimer(TimerData timerData, UpdateGroup updateGroup)
        {
            var timer = new Timer(timerData);
            
            _timerRunningCount++;
            
            while (timer.GetHasEnded == false)
            {
                timer.Run(CustomTime.GetDeltaTimeByChannel(updateGroup));
                
                yield return null;
            }
            
            _timerRunningCount--;
        }
    }
}