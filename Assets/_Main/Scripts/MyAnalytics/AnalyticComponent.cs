using System.Collections.Generic;
using _Main.Scripts.Interfaces.Analytics;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.MyAnalytics
{
    public abstract class AnalyticComponent<T> : ManagedBehavior
    where T : IAnalyticComponent
    {
        protected T Component { get; private set; }

        private void Awake()
        {
            Component = GetComponent<T>();

            if (Component == null)
            {
                Debug.LogError($"IAnalyticComponent not found in {gameObject.name}");
            }
            
        }

        protected void SendEvent(string eventName, Dictionary<string, object> parameters = null)
        {
            AnalyticsManager.SendEvent(eventName,parameters);
        }
    }
}