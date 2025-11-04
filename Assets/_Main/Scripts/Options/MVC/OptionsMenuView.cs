using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Observer;

namespace _Main.Scripts.Options.MVC
{
    public class OptionsMenuView : ManagedBehavior, IObserver
    {
        public event Action OnOptionsMenuEnable;
        
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
            SoundEventCaller.PlayMusic(MusicType.MainMenu);
        }
        
        private void HandleDisable()
        {
            SoundEventCaller.StopMusic();
        }
        
        private void HandleMainMenu()
        {
            GameManager.Instance.LoadMainMenu();
        }
    }
}