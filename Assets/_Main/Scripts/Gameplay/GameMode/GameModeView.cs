using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeView : MonoBehaviour, IObserver
    {
        [Header("Sounds")] 
        [SerializeField] private SoundBehavior gameplayTheme;
        [SerializeField] private SoundBehavior deathTheme;
        
        private GameModeController _controller;

        public void SetController(GameModeController controller)
        {
            _controller = controller;
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public void OnNotify(string message, params object[] args)
        {
            switch (message)
            {
                case GameModeObserverMessage.StartCountdown:
                    HandleStartCountdown();
                    break;
                case GameModeObserverMessage.CountdownFinish:
                    HandleCountdownFinish();
                    break;
                case GameModeObserverMessage.StartGame:
                    HandleStartGame();
                    break;
                case GameModeObserverMessage.EarthDeath:
                    HandleEarthDeath();
                    break;
                case GameModeObserverMessage.EarthShaking:
                    HandleEarthShake();
                    break;
                case GameModeObserverMessage.EarthDestruction:
                    HandleEarthDestruction();
                    break;
                case GameModeObserverMessage.SpawnSingleMeteor:
                    HandleSpawnSingleMeteor();
                    break;
                case GameModeObserverMessage.SpawnRingMeteor:
                    HandleSpawnRingMeteor();
                    break;
            }
        }

        #region Start

        private void HandleStartCountdown()
        {
            deathTheme?.StopSound();
            GameManager.Instance.EventManager.Publish(new CameraZoomOut());
        }
        
        private void HandleCountdownFinish()
        {
            _controller.TransitionToGameplay();
        }

        private void HandleStartGame()
        {
            GameManager.Instance.CanPlay = true;
            GameManager.Instance.EventManager.Publish(new ShieldEnable{IsEnabled = true});
            gameplayTheme?.PlaySound();
        }

        #endregion
        
        #region Earth

        private void HandleEarthDeath()
        {
            gameplayTheme?.StopSound();
            _controller.TransitionToFinish();
            GameManager.Instance.EventManager.Publish(new ShieldEnable{IsEnabled = false});
            GameManager.Instance.CanPlay = false;
            GameManager.Instance.EventManager.Publish(new RecycleAllMeteors());
        }
        
        private void HandleEarthShake()
        {
            GameManager.Instance.EventManager.Publish(new CameraZoomIn());
        }
        
        private void HandleEarthDestruction()
        {
            deathTheme?.PlaySound();
        }

        #endregion

        #region Meteor

        private void HandleSpawnSingleMeteor()
        {
            GameManager.Instance.EventManager.Publish(new SpawnSingleMeteor{Speed = GameValues.BaseMeteorSpeed});
        }
        private void HandleSpawnRingMeteor()
        {
            GameManager.Instance.EventManager.Publish(new SpawnRingMeteor{Speed = GameValues.BaseMeteorSpeed});
        }


        #endregion
    }
}