using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Observer;

namespace _Main.Scripts.GameScreens
{
    public class GameScreenView : ManagedBehavior, IObserver
    {
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case GameScreenObserverMessage.DisableScreen:
                    HandleDisableScreen((int)args[0]);
                    break;
                case GameScreenObserverMessage.LoadScreen:
                    HandleLoadGameScreen((int)args[0]);
                    break;
            }
        }
        
        private void HandleDisableScreen(int currentScreenIndex)
        {
            GameScreenEventCaller.DisableScreen((ScreenType)currentScreenIndex, EventRequestType.Request);
        }

        private void HandleLoadGameScreen(int currentScreenIndex)
        {
            GameScreenEventCaller.EnableScreen((ScreenType)currentScreenIndex, EventRequestType.Request);
        }
        
    }
}