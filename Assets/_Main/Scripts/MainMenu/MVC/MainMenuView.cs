using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Observer;

namespace _Main.Scripts.MainMenu.MVC
{
    public class MainMenuView : ManagedBehavior, IObserver
    {
        public event Action OnMainMenuEnable;
        
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case MainMenuObserverMessage.Enable:
                    HandleEnable();
                    break;
                case MainMenuObserverMessage.Disable:
                    HandleDisable();
                    break;
                case MainMenuObserverMessage.TriggerGameMode:
                    HandleGameMode();
                    break;
                case MainMenuObserverMessage.Quit:
                    HandleQuit();
                    break;
                case MainMenuObserverMessage.TriggerTutorial:
                    HandleTutorial();
                    break;
                case MainMenuObserverMessage.TriggerCosmetic:
                    HandleCosmetic();
                    break;
                case MainMenuObserverMessage.MainMenu:
                    break;
                
            }
        }

        private void HandleEnable()
        {
            OnMainMenuEnable?.Invoke();
            CameraEventCaller.ZoomIn();
        }
        
        private void HandleDisable()
        {
            GameScreenEventCaller.DisableScreen(ScreenType.MainMenu, EventRequestType.Granted);
        }
        
        private void HandleGameMode()
        {
            GameManager.Instance.LoadGameMode();
        }
        
        private void HandleTutorial()
        {
            GameManager.Instance.LoadTutorial();
        }
        
        private void HandleCosmetic()
        {
            GameManager.Instance.LoadCosmeticMenu();
        }

        private void HandleQuit()
        {
            GameManager.Instance.QuitGame();
        }
    }
}