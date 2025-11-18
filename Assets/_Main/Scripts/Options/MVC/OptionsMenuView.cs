using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using NicolasMassara.CustomUpdateManager;

namespace _Main.Scripts.Options.MVC
{
    public class OptionsMenuView : ManagedBehavior, IObserver
    {
        public event Action OnOptionsMenuEnable;
        public event Action OnOptionsMenuDisable;
        
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case OptionsMenuObserverMessage.Enable:
                    HandleEnable();
                    break;
                case OptionsMenuObserverMessage.Disable:
                    HandleDisable();
                    break;
                case OptionsMenuObserverMessage.TriggerMainMenu:
                    HandleMainMenu();
                    break;
            }
        }

        private void HandleEnable()
        {
            OnOptionsMenuEnable?.Invoke();
        }
        
        private void HandleDisable()
        {
            OnOptionsMenuDisable?.Invoke();
        }
        
        private void HandleMainMenu()
        {
            GameManager.Instance.LoadMainMenu();
        }
    }
}