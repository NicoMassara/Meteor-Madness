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
        }
        
        public event Action OnInitialized;

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
            }
        }
        
        private void HandleInitialize()
        {
            OnInitialized?.Invoke();
        }

        private void HandleSkinSelected(int skinIndex)
        {
            SkinManager.Instance.PreviewSkin((SkinType)skinIndex);

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
    }
}