using _Main.Scripts.Observer;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.GameScreens
{
    public class GameScreenView : ManagedBehavior, IObserver
    {
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        
        private GameScreenDebugData _debugData;
        
#endif

        private void Awake()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData = new GameScreenDebugData();
            
#endif
        }

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
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.LastScreen = (ScreenType)currentScreenIndex;
            
#endif
            
            GameScreenEventCaller.DisableScreen((ScreenType)currentScreenIndex, EventRequestType.Requested);
        }

        private void HandleLoadGameScreen(int currentScreenIndex)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.CurrentScreen = (ScreenType)currentScreenIndex;
            
#endif
            GameScreenEventCaller.EnableScreen((ScreenType)currentScreenIndex, EventRequestType.Granted);
        }
        
    }
}