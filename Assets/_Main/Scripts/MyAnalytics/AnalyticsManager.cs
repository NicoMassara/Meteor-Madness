using System;
using System.Collections.Generic;
using _Main.Scripts.GlobalEvents;
using _Main.Scripts.MyComponents;
using Unity.Services.Analytics;
using UnityEngine;

namespace _Main.Scripts.MyAnalytics
{
    public class AnalyticsManager : SingletonBehaviour<AnalyticsManager>
    {
        private bool _isTest;
        private bool _doesSend = true;
        private bool _hasInitialized = false;

        private const bool DoesDebug = true;

        private void Awake()
        {
            BootEvents.OnSubSystemRequestInitialize += Initialize;
        }

        private void Initialize()
        {
            BootEvents.OnSubSystemRequestInitialize -= Initialize;
            //
            _hasInitialized = true;
            //
            BootEvents.SubSystemInitialized();
        }

        private void Start()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _isTest = GameParameters.GameplayValues.AnalyticsDebugEnable;
            _doesSend = GameParameters.GameplayValues.DoesSendAnalytics;
#endif
        }

        public static void SendEvent(string eventName, Dictionary<string, object> parameters = null)
        => Instance.Internal_SendEvent(eventName, parameters);

        private void Internal_SendEvent(string eventName, Dictionary<string, object> parameters = null)
        {
            if (_hasInitialized == false)
            {
                if(DoesDebug)
                    Debug.LogWarning("Analytics Manager has not been initialized.");
                return;
            }
            
            // Adds "is_test" flag
            
            parameters ??= new Dictionary<string, object>();
            parameters["is_test"] = _isTest;

            if (parameters.Count == 1) // Only is_test flag
            {
                if(_doesSend)
                    AnalyticsService.Instance.RecordEvent(eventName);
                
                if(DoesDebug)
                    Debug.Log($"Analytic event sent: {eventName} (no parameters added, test={_isTest})");
            }
            else
            {
                CustomEvent evento = new CustomEvent(eventName);
                foreach (var pair in parameters)
                    evento[pair.Key] = pair.Value;

                if(_doesSend)
                    AnalyticsService.Instance.RecordEvent(evento);
                
                
                if(DoesDebug)
                    Debug.Log($"Analytic event sent: {eventName} with {parameters.Count-1} parameters (test={_isTest})"); // Not counting "is_test" parameter
            }
        }
    }
}