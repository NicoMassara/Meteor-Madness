using _Main.Scripts.Observer;

namespace _Main.Scripts.GameStats
{
    public class StatsMotor : ObservableComponent
    {
        public void Initialize() => NotifyAll(StatsObserverMessage.Initialize);
        public void Enable() => NotifyAll(StatsObserverMessage.Enable);
        public void StartDisable() => NotifyAll(StatsObserverMessage.StartDisable);
        public void ExecuteDisable() => NotifyAll(StatsObserverMessage.ExecuteDisable);
        public void OpenMainMenu() => NotifyAll(StatsObserverMessage.MainMenu);
        public void LoadTextData(StatsData data) => NotifyAll(StatsObserverMessage.LoadTextData,data);
    }
}