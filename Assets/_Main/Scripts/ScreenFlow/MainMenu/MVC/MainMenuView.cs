using System;
using _Main.Scripts.EventBus;
using _Main.Scripts.GameCamera;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.Contracts.Interfaces.Analytics;
using MeteorMadness.Contracts.Interfaces.Sounds;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.Managers;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Menu
{
    public class MainMenuView : ManagedBehavior, IObserver,
        IMainMenuSounds,
        IMainMenuAnalytics
    {
        [SerializeField] private CameraTransportDataSo cameraTransportData;
        
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
            if (FlagsManager.GetHasOpenedLore() == false)
            {
                FlagsManager.SetHasOpenedLore();
                FlagsManager.SaveFlags();
            }
            
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
            CameraEventCaller.Transport(cameraTransportData);
            EarthEventCaller.DisableDamage();
        }
    }
}