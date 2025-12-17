using System;
using _Main.Scripts.Interfaces.Analytics;
using UnityEngine;

namespace _Main.Scripts.MyAnalytics.Components
{
    public class DefeatAnalyticComponent : AnalyticComponent<IDefeatAnalytics>
    {
        private void Start()
        {
            Component.OnGameSaved += () => SendEvent(AnalyticEventsName.Defeat.Saved);
            Component.OnRestart += () => SendEvent(AnalyticEventsName.Defeat.Restart);
            Component.OnMainMenu += () => SendEvent(AnalyticEventsName.Defeat.MainMenu);
        }
    }
}