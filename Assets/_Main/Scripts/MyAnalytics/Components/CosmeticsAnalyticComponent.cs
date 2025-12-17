using System;
using System.Collections.Generic;
using _Main.Scripts.Interfaces.Analytics;
using UnityEngine;

namespace _Main.Scripts.MyAnalytics.Components
{
    public class CosmeticsAnalyticComponent :  AnalyticComponent<ICosmeticsAnalytics>
    {
        private void Start()
        {
            Component.OnCosmeticFirstEnable += () =>
            {
                SendEvent(AnalyticEventsName.Cosmetics.Opened);
            };
            
            Component.OnCosmeticChanged += (skinName) =>
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { AnalyticEventsName.Cosmetics.Changed.SkinType, skinName }
                };
                    
                SendEvent(AnalyticEventsName.Cosmetics.Changed.EventName, parameters);
            };
        }
    }
}