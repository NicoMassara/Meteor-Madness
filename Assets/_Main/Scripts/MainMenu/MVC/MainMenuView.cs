using System;
using _Main.Scripts.GlobalEvents;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.MainMenu.MVC
{
    public class MainMenuView : ManagedBehavior, IObserver,
        IMainMenuSounds
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
                case MainMenuObserverMessage.TriggerOptions:
                    HandleTriggerOptions();
                    break;
                case MainMenuObserverMessage.MainPanelOpened:
                    HandleMainPanelOpened();
                    break;
            }
        }

        private void HandleMainPanelOpened()
        {
            AdsEvents.Banner_TriggerShow();
        }

        private void HandleEnable()
        {
            AdsEvents.Banner_TriggerLoad();
            OnMainMenuEnable?.Invoke();
            CameraEventCaller.ZoomIn(0.5f);
            EarthEventCaller.DisableDamage();
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
        
        private void HandleTriggerOptions()
        {
            GameManager.Instance.LoadOptionsMenu();
        }

        private void OnDestroy()
        {
            AdsEvents.Banner_TriggerDestroy();
        }

        private void OnApplicationQuit()
        {
            AdsEvents.Banner_TriggerDestroy();
        }
    }
}