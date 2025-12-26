using System;
using MeteorMadness.GlobalValues.Interfaces.Analytics;
using UnityEngine;

namespace MeteorMadness.Services.MyAnalytics.Components
{
    public class TutorialAnalyticComponent : AnalyticComponent<ITutorialAnalytics>
    {
        private void Start()
        {
            Component.OnTutorialFinished += () => SendEvent(AnalyticEventsName.Tutorial.Completed);
        }
    }
}