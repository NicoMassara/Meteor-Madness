using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Observer;

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
                case GameScreenObserverMessage.SetTutorial:
                    HandleSetTutorial();
                    break;
            }
        }

        private void HandleSetTutorial()
        {
            GameScreenEventCaller.SetGameScreen(ScreenType.Tutorial, true);
        }

        private void HandleSetMainMenu()
        {
            GameScreenEventCaller.SetGameScreen(ScreenType.MainMenu, true);
        }
        
        private void HandleSetGameplay()
        {
            GameScreenEventCaller.SetGameScreen(ScreenType.GameMode, true);
        }
    }
}