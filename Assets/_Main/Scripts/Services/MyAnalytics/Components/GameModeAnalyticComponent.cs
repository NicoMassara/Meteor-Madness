
using System.Collections.Generic;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.Contracts.Interfaces.Analytics;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.Services.MyAnalytics.Components
{
    public class GameModeAnalyticComponent : AnalyticComponent<IGameModeAnalytics>, IUpdatable
    {
        [Range(0,10)]
        [SerializeField] private int minTimeToRegister;
        private float _gainedPoints;
        private int _internalLevel;
        private int _maxStreak;
        private int _currentStreak;
        private bool _canRegister;
        private readonly Timer _gameModeTimer = new Timer();
        private readonly Dictionary<AbilityType, int> _abilityUseCount = new Dictionary<AbilityType, int>();
        private string _selectedSkin;

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
                _canRun = true;
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
            SkinEvents.OnSkinChanged += value => _selectedSkin = value;
            
            Component.OnInitialized += () =>
            {
                ClearAbilityDic();
                _canRegister = true;
                _gameModeTimer.Restart();
            };
            
            Component.OnScoreSaved += () =>
            {
                _gameModeTimer.Stop();
                if (_gameModeTimer.ElapsedTime >= minTimeToRegister)
                {
                    SendFinishData();
                }
            };
            
            Component.OnGameInterrupted += () =>
            {
                _gameModeTimer.Stop();
                if (_gameModeTimer.ElapsedTime >= minTimeToRegister)
                {
                    SendInterruptedData();
                }
                
            };
            Component.OnPaused += () => _gameModeTimer.Stop();
            Component.OnResume += () => _gameModeTimer.Start();
            Component.OnPointGained += (value) => _gainedPoints += value;
            Component.OnAbilityTriggered += (value) => AddAbility(value);
            Component.OnLevelUpdate += (value) => _internalLevel = value;
            Component.OnStreakUpdated += (value) =>
            {
                _currentStreak = (int)value;

                if (_currentStreak > _maxStreak)
                {
                    _maxStreak = _currentStreak;
                }
            };

            InitializeAbilityDic();
        }

        public void ExecuteUpdate(float deltaTime)
        { 
            if ((_gameModeTimer.ElapsedTime) >= (minTimeToRegister + 3) &&
                _canRegister)
            {
                SendEvent(AnalyticEventsName.GameMode.Start);
                _canRegister = false;
            }
            
            _gameModeTimer.Execute(deltaTime);
        }
        
        #region Ability

        private void InitializeAbilityDic()
        {
            for (int i = 1; i < (int)AbilityType.Default_MAX; i++)
            {
                _abilityUseCount.Add((AbilityType)i, 0);
            }
        }

        private void ClearAbilityDic()
        {
            for (int i = 1; i < (int)AbilityType.Default_MAX; i++)
            {
                _abilityUseCount[(AbilityType)i] = 0;
            }
        }

        private void AddAbility(AbilityType abilityType)
        {
            _abilityUseCount[abilityType]++;
        }

        #endregion

        private void SendInterruptedData()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { $"{AnalyticEventsName.GameMode.GameplayNames.ElapsedTime}", 
                    _gameModeTimer.ElapsedTime },
                { $"{AnalyticEventsName.GameMode.GameplayNames.GainedPoints}", 
                    _gainedPoints },
                { $"{AnalyticEventsName.GameMode.GameplayNames.Level}",
                    _internalLevel },
                { $"{AnalyticEventsName.GameMode.GameplayNames.MaxStreak}",
                    _maxStreak },
                { $"{AnalyticEventsName.GameMode.GameplayNames.Skin}",
                    _selectedSkin }
            };
            
            foreach (var ability in _abilityUseCount)
            {
                parameters[$"{AnalyticEventsName.GameMode.GameplayNames.Ability}_{ability.Key}"] = ability.Value;
            }
            
            //
            SendEvent(AnalyticEventsName.GameMode.GameplayNames.EventName, parameters);
        }
        
        private void SendFinishData()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { $"{AnalyticEventsName.GameMode.GameplayNames.ElapsedTime}", 
                    _gameModeTimer.ElapsedTime },
                { $"{AnalyticEventsName.GameMode.GameplayNames.GainedPoints}", 
                    _gainedPoints },
                { $"{AnalyticEventsName.GameMode.GameplayNames.Level}",
                    _internalLevel },
                { $"{AnalyticEventsName.GameMode.GameplayNames.MaxStreak}",
                    _maxStreak },
                { $"{AnalyticEventsName.GameMode.GameplayNames.Skin}",
                    _selectedSkin }
            };
            
            foreach (var ability in _abilityUseCount)
            {
                parameters[$"{AnalyticEventsName.GameMode.GameplayNames.Ability}_{ability.Key}"] = ability.Value;
            }
            
            //
            SendEvent(AnalyticEventsName.GameMode.GameplayNames.EventName, parameters);
        }
    }
}