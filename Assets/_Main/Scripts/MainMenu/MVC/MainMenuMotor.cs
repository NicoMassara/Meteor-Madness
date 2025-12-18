using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.MainMenu.MVC
{
    public class MainMenuMotor : ObservableComponent
    {
        private bool _loreEnabled;
        
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
            _loreEnabled = true;
            NotifyAll(MainMenuObserverMessage.LoreMenu);
        }

        public void Menu()
        {
            if (_loreEnabled)
            {
                NotifyAll(MainMenuObserverMessage.LoreClosed);
                _loreEnabled = false;
            }

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

        public void Credits()
        {
            NotifyAll(MainMenuObserverMessage.CreditsMenu);
        }

        public void TriggerOptions()
        {
            NotifyAll(MainMenuObserverMessage.TriggerOptions);
        }

        public void StartDisable()
        {
            NotifyAll(MainMenuObserverMessage.StartDisable);
        }

        public void TriggerMainPanelOpen()
        {
            NotifyAll(MainMenuObserverMessage.MainPanelOpened);
        }

        public void FirstGame()
        {
            NotifyAll(MainMenuObserverMessage.FirstGame);
        }

        public void Stats()
        {
            NotifyAll(MainMenuObserverMessage.Stats);
        }
    }
}