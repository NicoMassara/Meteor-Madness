using _Main.Scripts.Observer;

namespace _Main.Scripts.GameScreens
{
    public class GameScreenMotor : ObservableComponent
    {
        private int _currentScreenIndex = -1;


        public void DisableCurrentScreen()
        {
            NotifyAll(GameScreenObserverMessage.DisableScreen, _currentScreenIndex);
        }

        public void LoadCurrentScreen()
        {
            NotifyAll(GameScreenObserverMessage.LoadScreen, _currentScreenIndex);
        }

        public void SelectNewScreen(int screenIndex)
        {
            DisableCurrentScreen();
            _currentScreenIndex = screenIndex;
        }

        public void SetActiveMainMenu()
        {
            _currentScreenIndex = 1;
        }

        public void SetActiveGameplay()
        {
            DisableCurrentScreen();
            _currentScreenIndex = 2;
        }

        public void SetActiveTutorial()
        {
            DisableCurrentScreen();
            _currentScreenIndex = 3;
        }

        public void SetActiveCosmetic()
        {
            DisableCurrentScreen();
            _currentScreenIndex = 4;
        }
        
        public void SetActiveStartLoading()
        {
            DisableCurrentScreen();
            _currentScreenIndex = 5;
        }
    }
}