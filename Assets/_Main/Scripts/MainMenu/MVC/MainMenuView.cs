using System;
using _Main.Scripts.GlobalEvents;
using _Main.Scripts.Interfaces.Analytics;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.MainMenu.MVC
{
    public class MainMenuView : ManagedBehavior, IObserver,
        IMainMenuSounds,
        IMainMenuAnalytics
    {
        public event Action OnMainMenuEnable;

        #region IMainMenuAnalytics

        public event Action OnLoreOpened;
        public event Action OnLoreClosed;

        #endregion
        
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
                case MainMenuObserverMessage.Quit:
                    HandleQuit();
                    break;

                
                // === Screens === // 
                case MainMenuObserverMessage.LoreMenu:
                    HandleLoreMenu();
                    break;
                case MainMenuObserverMessage.LoreClosed:
                    HandleLoreClosed();
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
                case MainMenuObserverMessage.TriggerGameMode:
                    HandleGameMode();
                    break;
                case MainMenuObserverMessage.Stats:
                    HandleStats();
                    break;
            }
        }

        
        #region Screens
        
        private void HandleStats()
        {
            GameManager.Instance.LoadStatsScreen();
        }
        
        private void HandleLoreMenu()
        {
            GameManager.Instance.FlagsController.GetHasOpenedLore();
            OnLoreOpened?.Invoke();
        }
        
        private void HandleLoreClosed()
        {
            OnLoreClosed?.Invoke();
        }
        
        private void HandleGameMode()
        {
            AdsEvents.Banner_TriggerHide();
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
        
        private void HandleTriggerOptions()
        {
            GameManager.Instance.LoadOptionsMenu();
        }

        #endregion
        

        private void HandleMainPanelOpened()
        {
            AdsEvents.Banner_TriggerShow();
        }

        
        private void HandleDisable()
        {
            GameScreenEventCaller.DisableScreen(ScreenType.MainMenu, EventRequestType.Granted);
        }
        
        private void HandleQuit()
        {
            GameManager.Instance.QuitGame();
        }
        private void OnDestroy()
        {
            AdsEvents.Banner_TriggerDestroy();
        }

        private void OnApplicationQuit()
        {
            AdsEvents.Banner_TriggerDestroy();
        }
        
        private void HandleEnable()
        {
            OnMainMenuEnable?.Invoke();
            CameraEventCaller.ZoomIn(0.5f);
            EarthEventCaller.DisableDamage();
        }
    }
}