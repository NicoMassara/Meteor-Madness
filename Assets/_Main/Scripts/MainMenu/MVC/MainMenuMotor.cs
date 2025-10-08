using _Main.Scripts.Observer;

namespace _Main.Scripts.MainMenu.MVC
{
    public class MainMenuMotor : ObservableComponent
    {
        public void Enable()
        {
            NotifyAll(MainMenuObserverMessage.Enable);
        }

        public void Disable()
        {
            NotifyAll(MainMenuObserverMessage.Disable);
        }

        public void Lore()
        {
            NotifyAll(MainMenuObserverMessage.LoreMenu);
        }

        public void Menu()
        {
            NotifyAll(MainMenuObserverMessage.MainMenu);
        }

        public void TriggerTutorial()
        {
            NotifyAll(MainMenuObserverMessage.Tutorial);
        }

        public void TriggerGameMode()
        {
            NotifyAll(MainMenuObserverMessage.GameMode);
        }

        public void TriggerQuit()
        {
            NotifyAll(MainMenuObserverMessage.Quit);
        }
    }
}