using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.GameScreens
{
    public class GameScreenView : ManagedBehavior, IUpdatable, IObserver
    {
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Always;
        
        public void ManagedUpdate() { }

        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case GameScreenObserverMessage.SetMainMenu:
                    HandleSetMainMenu();
                    break;
                case GameScreenObserverMessage.SetGameplay:
                    HandleSetGameplay();
                    break;
            }
        }

        private void HandleSetMainMenu()
        {
            GameManager.Instance.EventManager.Publish(new MainMenuScreenEnable());
            GameManager.Instance.EventManager.Publish(new CameraZoomIn());
        }
        
        private void HandleSetGameplay()
        {
            GameManager.Instance.EventManager.Publish(new GameModeScreenEnable());
            GameManager.Instance.EventManager.Publish(new CameraZoomOut());
        }
    }
}