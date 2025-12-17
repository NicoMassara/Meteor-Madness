using System;
using _Main.Scripts.Interfaces.Analytics;
using UnityEngine;

namespace _Main.Scripts.MyAnalytics.Components
{
    public class TutorialAnalyticComponent : AnalyticComponent<ITutorialAnalytics>
    {
        private void Start()
        {
            Component.OnTutorialFinished += () => SendEvent(AnalyticEventsName.Tutorial.Completed);
        }
    }
}