using System;
using _Main.Scripts.Interfaces.Analytics;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using NicolasMassara.CustomUpdateManager;

namespace _Main.Scripts.Cosmetics.MVC
{
    public class CosmeticView : ManagedBehavior, IObserver,
        ICosmeticsAnalytics
    {
        public event Action OnCosmeticEnable;

        #region ICosmeticsAnalytics
        
        public event Action OnCosmeticFirstEnable;
        public event Action<string> OnCosmeticChanged;

        #endregion
        
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case CosmeticObserverMessage.Enable:
                    HandleEnable();
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

        private void HandleSkinSelected(int skinIndex)
        {
            SkinManager.Instance.SelectSkin((SkinType)skinIndex);
            SkinManager.Instance.SaveSelected();
            OnCosmeticChanged?.Invoke(((SkinType)skinIndex).ToString());
        }

        private void HandleEnable()
        {
            if (GameManager.Instance.FlagsController.GetHasOpenedCosmetics() == false)
            {
                OnCosmeticFirstEnable?.Invoke();
            }
            
            OnCosmeticEnable?.Invoke();
            CameraEventCaller.LookLeft();
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