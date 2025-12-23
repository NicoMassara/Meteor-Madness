using System;
using _Main.Scripts.Interfaces.Analytics;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using NicolasMassara.CustomUpdateManager;

namespace _Main.Scripts.Cosmetics.MVC
{
    public class CosmeticView : ManagedBehavior, IObserver,
        ICosmeticsAnalytics, CosmeticView.ICosmeticView
    {
        public interface ICosmeticView
        {
            public event Action OnInitialized;
            public event Action<SkinType> OnSkinUnlocked;
            public event Action OnFailedToUnlock;
        }

        #region ICosmeticView

        
        public event Action OnInitialized;
        public event Action<SkinType> OnSkinUnlocked;
        public event Action OnFailedToUnlock;

        #endregion

        #region ICosmeticsAnalytics
        
        public event Action OnCosmeticFirstEnable;
        public event Action<string> OnCosmeticChanged;

        #endregion
        
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case CosmeticObserverMessage.Initialize:
                    HandleInitialize();
                    break;
                case CosmeticObserverMessage.Enable:
                    HandleEnable();
                    break;
                case CosmeticObserverMessage.StartDisable:
                    HandleStartDisable();
                    break;
                case CosmeticObserverMessage.Disable:
                    HandleDisable();
                    break;
                case CosmeticObserverMessage.TriggerMainMenu:
                    HandleTriggerMainMenu();
                    break;
                case CosmeticObserverMessage.SkinSelected:
                    HandleSkinSelected((int)args[0]);
                    break;
                
                // === Unlock === //
                case CosmeticObserverMessage.TryUnlockSkin:
                    HandleTryUnlock((int)args[0]);
                    break;
            }
        }
        
        private void HandleInitialize()
        {
            OnInitialized?.Invoke();
        }

        private void HandleSkinSelected(int skinIndex)
        {
            SkinManager.Instance.PreviewSkin((SkinType)skinIndex);

            if (SkinManager.Instance.GetIsLocked(skinIndex))
            {
                CameraEventCaller.EnableGrayscale();
            }
            else
            {
                CameraEventCaller.DisableGrayscale();
            }

            OnCosmeticChanged?.Invoke(((SkinType)skinIndex).ToString());
        }

        private void HandleEnable()
        {
            if (GameManager.Instance.FlagsController.GetHasOpenedCosmetics() == false)
            {
                OnCosmeticFirstEnable?.Invoke();
            }
            
            CameraEventCaller.LookLeft();
        }
        
        private void HandleStartDisable()
        {
            CameraEventCaller.DisableGrayscale();
            SkinManager.Instance.SaveSelected();
        }

        private void HandleDisable()
        {
            GameScreenEventCaller.DisableScreen(ScreenType.Cosmetic, EventRequestType.Granted);
            CameraEventCaller.LookCenter();
        }
        
        private void HandleTriggerMainMenu()
        {

            GameManager.Instance.LoadMainMenu();
        }

        private void HandleTryUnlock(int skinIndex)
        {
            var type = (SkinType)skinIndex;
            var coinsTarget = SkinManager.Instance.GetSkinInformationByType(type).UnlockPrice;
            var hasEnough = SkinManager.Instance.GetContainsEnoughCoins(coinsTarget);

            if (hasEnough)
            {
                SkinManager.Instance.UnlockPreviewSkin();
                SkinManager.Instance.TryRemoveCoins(coinsTarget);
                SkinManager.Instance.UnlockSkin(skinIndex);
                OnSkinUnlocked?.Invoke(type);
                CameraEventCaller.DisableGrayscale();
            }
            else
            {
                OnFailedToUnlock?.Invoke();
            }
        }
    }
}