using MeteorMadness.Contracts.Interfaces.Analytics;


namespace MeteorMadness.Services.MyAnalytics.Components
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