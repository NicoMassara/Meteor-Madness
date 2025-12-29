using MeteorMadness.Contracts.Interfaces.Analytics;


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