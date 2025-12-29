using System.Collections.Generic;
using MeteorMadness.Contracts.Interfaces.Analytics;

namespace MeteorMadness.Services.MyAnalytics.Components
{
    public class CosmeticsAnalyticComponent :  AnalyticComponent<ICosmeticsAnalytics>
    {
        private void Start()
        {
            Component.OnFirstOpen += () =>
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