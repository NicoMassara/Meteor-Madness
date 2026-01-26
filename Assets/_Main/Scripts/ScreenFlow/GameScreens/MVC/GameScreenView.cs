using _Main.Scripts.EventBus;
using _Main.Scripts.GameCamera;
using MeteorMadness.Contracts;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.Managers;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.GameMode
{
    public class GameScreenView : ManagedBehavior, IObserver
    {
        [SerializeField] private CameraTransportDataSo cameraTransportData;
        
        private void Start()
        {
            if (GameManager.Instance.HadCorruptedSaveData)
            {
                Debug.LogWarning("Save data was corrupted, new save files were created!");
            }
        }

        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case GameScreenObserverMessage.ZoomIn:
                    HandleZoomIn();
                    break;
                case GameScreenObserverMessage.DisableScreen:
                    HandleDisableScreen((int)args[0]);
                    break;
                case GameScreenObserverMessage.LoadScreen:
                    HandleLoadGameScreen((int)args[0]);
                    break;
            }
        }

        private void HandleZoomIn()
        {
            CameraEventCaller.Transport(cameraTransportData);
        }

        private void HandleDisableScreen(int currentScreenIndex)
        {
            GameScreenEventCaller.DisableScreen((ScreenType)currentScreenIndex, EventRequestType.Requested);
        }

        private void HandleLoadGameScreen(int currentScreenIndex)
        {
            GameScreenEventCaller.EnableScreen((ScreenType)currentScreenIndex, EventRequestType.Granted);
        }
        
    }
}