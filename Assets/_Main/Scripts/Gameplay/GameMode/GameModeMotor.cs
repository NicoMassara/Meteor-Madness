using _Main.Scripts.Gameplay.Meteor;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeMotor : ObservableComponent
    {
        private MeteorSpeedController _meteorSpeedController;
        private int _meteorDeflectCount;
        private int _meteorCollisionCount;

        private float _startTimer;
        private bool _doesHandleTimer;

        public GameModeMotor()
        {
            Initialize();
        }

        private void Initialize()
        {
            _meteorSpeedController = new MeteorSpeedController();
        }
                
        public void Execute()
        {
            if (_doesHandleTimer)
            {
                HandleTimer();
            }
        }

        private void HandleTimer()
        {
            _startTimer -= Time.deltaTime;

            NotifyAll(GameModeObserverMessage.UpdateCountdown, _startTimer);
            
            if (_startTimer <= 0)
            {
                _doesHandleTimer = false;
                NotifyAll(GameModeObserverMessage.CountdownFinish);
            }
        }

        public void StartCountdown(float time)
        {
            _startTimer = time + 1;
            _doesHandleTimer = true;
            NotifyAll(GameModeObserverMessage.StartCountdown);
        }

        public void StartGame()
        {
            NotifyAll(GameModeObserverMessage.StartGame);
        }

        public void HandleMeteorDeflect()
        {
            _meteorDeflectCount++;
            _meteorSpeedController.CheckForNextLevel(_meteorDeflectCount);
            NotifyAll(GameModeObserverMessage.MeteorDeflect,_meteorDeflectCount);
        }

        public void RestartValues()
        {
            _meteorCollisionCount = 0;
            _meteorDeflectCount = 0;
            _meteorSpeedController.RestartAll();
        }

        #region Earth

        public void HandleEarthShake()
        {
            NotifyAll(GameModeObserverMessage.EarthShaking);
        }

        public void HandleEarthStartDestruction()
        {
            NotifyAll(GameModeObserverMessage.EarthStartDestruction);
        }

        public void HandleEarthEndDestruction()
        {
            NotifyAll(GameModeObserverMessage.EarthEndDestruction, _meteorDeflectCount);
        }

        #endregion
        
        public void SpawnSingleMeteor()
        {
            NotifyAll(GameModeObserverMessage.SpawnSingleMeteor);
        }

        public void HandleGameFinish()
        {
            NotifyAll(GameModeObserverMessage.GameFinish);
        }
    }
}