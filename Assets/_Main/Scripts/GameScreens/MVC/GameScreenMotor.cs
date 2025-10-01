using _Main.Scripts.Observer;

namespace _Main.Scripts.GameScreens
{
    public class GameScreenMotor : ObservableComponent
    {
        public void SetActiveMainMenu()
        {
            NotifyAll(GameScreenObserverMessage.SetMainMenu);
        }

        public void SetActiveGameplay()
        {
            NotifyAll(GameScreenObserverMessage.SetGameplay);
        }
    }
}