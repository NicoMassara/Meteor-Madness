using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.GameScreens
{
    public class GameScreenMotor : ObservableComponent
    {
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
            _currentScreenIndex = screenIndex;
            NotifyAll(GameScreenObserverMessage.LoadScreen, _currentScreenIndex);
        }

        public void SelectNewScreen(int screenIndex)
        {
            if (_currentScreenIndex == screenIndex)
            {
                return;
            }
            
            if (_currentScreenIndex > -1)
            {
                _newScreenIndex = screenIndex;
                DisableCurrentScreen();
            }
        }
    }
}