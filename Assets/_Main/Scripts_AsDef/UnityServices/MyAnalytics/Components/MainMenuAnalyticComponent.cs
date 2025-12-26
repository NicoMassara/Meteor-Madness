using System;
using System.Collections.Generic;
using MeteorMadness.GlobalValues.Interfaces.Analytics;
using NicolasMassara.CustomUpdateManager;

namespace MeteorMadness.UnityServices.MyAnalytics.Components
{
    public class MainMenuAnalyticComponent : AnalyticComponent<IMainMenuAnalytics>, IUpdatable
    {
        private readonly Timer _loreTimer = new Timer();
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Always;
        public TickGroup SelfTickGroup { get; } = TickGroup.HalfTarget;
        
        private class Timer
        {
            public float ElapsedTime { get; private set; }

            private bool _canRun;

            public void Start() => _canRun = true;
            public void Restart()
            {
                Clear();
                Start();
            }

            public void Stop() => _canRun = false;
            public void Clear() => ElapsedTime = 0;

            public void Execute(float deltaTime)
            {
                if(_canRun) 
                    ElapsedTime += deltaTime;
            }
        }
        
        private void Start()
        {
            Component.OnLoreOpened += () =>
            {
                _loreTimer.Restart();
                SendEvent(AnalyticEventsName.MainMenu.Lore.Opened);
            };
            Component.OnLoreClosed += () =>
            {
                ProcessData();
            };
        }
        
        public void ExecuteUpdate(float deltaTime)
        { 
            _loreTimer.Execute(deltaTime);
        }

        private void ProcessData()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { $"{AnalyticEventsName.MainMenu.Lore.Closed.ElapsedTime}", 
                    _loreTimer.ElapsedTime },
            };
            
            //
            SendEvent(AnalyticEventsName.MainMenu.Lore.Closed.EventName, parameters);
        }
    }
}

