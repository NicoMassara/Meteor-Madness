using System;
using _Main.Scripts.Interfaces.Ads;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.MainMenu.MVC
{
    public class MainMenuView : ManagedBehavior, IObserver,
        IMainMenuSounds,
        IMainMenuBannerComponent
    {
        #region IMainMenuBannerComponent

        public event Action OnLoadAd;
        public event Action OnShowAd;
        public event Action OnHideAd;
        public event Action OnDestroyAd;

        #endregion
        
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
            OnShowAd?.Invoke();
        }

        private void HandleEnable()
        {
            OnLoadAd?.Invoke();
            OnMainMenuEnable?.Invoke();
            CameraEventCaller.ZoomIn(0.5f);
            EarthEventCaller.DisableDamage();
        }
        
        private void HandleDisable()
        {
            OnHideAd?.Invoke();
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
            OnDestroyAd?.Invoke();
        }

        private void OnApplicationQuit()
        {
            OnDestroyAd?.Invoke();
        }
    }
}