

using MeteorMadness.GlobalValues.Tools.Observer;

namespace MeteorMadness.ScreenFlow.GameMode
{
    public class GameScreenMotor : ObservableComponent
    {
        private int _lastScreenIndex = -1;
        private int _currentScreenIndex = -1;
        private int _newScreenIndex = -1;

        private void DisableCurrentScreen()
        {
            NotifyAll(GameScreenObserverMessage.DisableScreen, _currentScreenIndex);
        }

        public void LoadCurrentScreen()
        {
            NotifyAll(GameScreenObserverMessage.LoadScreen, _newScreenIndex);
            _currentScreenIndex = _newScreenIndex;
        }

        public void LoadScreenByIndex(int screenIndex)
        {
            if(screenIndex == 0) return;
            
            _currentScreenIndex = screenIndex;
            NotifyAll(GameScreenObserverMessage.LoadScreen, _currentScreenIndex);
        }
        
        public void LoadLastScreen()
        {
            SelectNewScreen(_lastScreenIndex);
        }

        public void SelectNewScreen(int screenIndex)
        {
            if (_currentScreenIndex == screenIndex)
            {
                return;
            }
            
            if (_currentScreenIndex > -1)
            {
                _lastScreenIndex = _currentScreenIndex;
                _newScreenIndex = screenIndex;
                DisableCurrentScreen();
            }
        }

        public void ZoomIn()
        {
            NotifyAll(GameScreenObserverMessage.ZoomIn);
        }
    }
}