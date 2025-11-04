using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Observer;

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
            }
        }

        private void HandleEnable()
        {
            OnCosmeticEnable?.Invoke();
            CameraEventCaller.ZoomIn();
            SoundEventCaller.PlayMusic(MusicType.MainMenu);
        }

        private void HandleDisable()
        {
            SoundEventCaller.StopMusic();
        }
        
        private void HandleTriggerMainMenu()
        {
            GameManager.Instance.LoadMainMenu();
        }
    }
}