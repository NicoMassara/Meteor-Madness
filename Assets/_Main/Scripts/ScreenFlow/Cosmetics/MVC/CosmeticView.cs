using System;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces.Analytics;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.Managers;
using MeteorMadness.Managers.Cosmetics;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.MVC
{
    public class CosmeticView : ManagedBehavior, IObserver,
        ICosmeticsAnalytics, CosmeticView.ICosmeticView
    {
        public interface ICosmeticView
        {
            public event Action OnInitialized;
            public event Action OnFirstOpen;
            public event Action<SkinType> OnSkinUnlocked;
            public event Action OnFailedToUnlock;
        }

        #region ICosmeticView

        
        public event Action OnInitialized;
        public event Action<SkinType> OnSkinUnlocked;
        public event Action OnFailedToUnlock;
        public event Action OnFirstOpen;

        #endregion

        #region ICosmeticsAnalytics
        
        public event Action<string> OnCosmeticChanged;

        #endregion
        
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                // === Enable / Disable === //
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
                case CosmeticObserverMessage.Opened:
                    HandleOpened();
                    break;
                
                
                // === Unlock === //
                case CosmeticObserverMessage.TryUnlockSkin:
                    HandleTryUnlock((int)args[0]);
                    break;
                
                // === Skin === //
                case CosmeticObserverMessage.SkinSelected:
                    HandleSkinSelected((int)args[0]);
                    break;
            }
        }

        private void HandleOpened()
        {
            if (FlagsManager.GetHasOpenedCosmetics() == false)
            {
                OnFirstOpen?.Invoke();
            }
        }

        private void HandleInitialize()
        {
            if (FlagsManager.GetHasOpenedCosmetics() == false)
            {
                FlagsManager.SetHasOpenedCosmetics();
                FlagsManager.SaveFlags();
            }
            
            OnInitialized?.Invoke();
        }

        private void HandleSkinSelected(int skinIndex)
        {
            if(skinIndex == -1) return;
            
            SkinManager.Instance.PreviewSkin((SkinType)skinIndex);

            if (SkinManager.Instance.GetIsLocked(skinIndex))
            {
                Debug.Log($"{(SkinType)skinIndex} skin Is Locked");
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
            var hasEnough = SkinManager.Instance.GetContainsEnoughCoins((uint)coinsTarget);

            if (hasEnough)
            {
                SkinManager.Instance.UnlockPreviewSkin();
                SkinManager.Instance.TryRemoveCoins((uint)coinsTarget);
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