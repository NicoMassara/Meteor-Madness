using System;
using _Main.Scripts.Managers;
using _Main.Scripts.MySettings;
using _Main.Scripts.Observer;
using NicolasMassara.CustomUpdateManager;

namespace _Main.Scripts.Cosmetics.MVC
{
    public class CosmeticView : ManagedBehavior, IObserver
    {
        public event Action OnCosmeticEnable;
        
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
                case CosmeticObserverMessage.AbilitySelect:
                    HandleAbilitySelect((int)args[0]);
                    break;
            }
        }

        private void HandleAbilitySelect(int skinIndex)
        {
            SkinManager.Instance.SelectSkin((SkinType)skinIndex);
            SkinManager.Instance.SaveSelected();
        }

        private void HandleEnable()
        {
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