using _Main.Scripts.Observer;
using UnityEngine;

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
            NotifyAll(MainMenuObserverMessage.TriggerTutorial);
        }

        public void TriggerGameMode()
        {
            NotifyAll(MainMenuObserverMessage.TriggerGameMode);
        }

        public void TriggerQuit()
        {
            NotifyAll(MainMenuObserverMessage.Quit);
        }

        public void Tutorial()
        {
            NotifyAll(MainMenuObserverMessage.TutorialMenu);
        }

        public void TriggerCosmetic()
        {
            NotifyAll(MainMenuObserverMessage.TriggerCosmetic);
        }
    }
}