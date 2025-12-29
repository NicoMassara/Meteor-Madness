using System;
using MeteorMadness.Contracts;
using UnityEngine;
using MeteorMadness.Contracts.Events;
using MeteorMadness.Contracts.Interfaces.GameplayData;
using MeteorMadness.GlobalValues.BaseSingleton;
using MeteorMadness.Managers.GameConfig.Game;

namespace MeteorMadness.Managers.GameConfig
{
    public class GameConfigManager : SingletonBehaviour<GameConfigManager>
    {
        #region Private Classes

        private class ConfigFileLoader
        {
            private const string Path = "ScriptableObjects/GameConfig/SO_GameConfig";

            public static IGameplayConfig GetGameplayConfig()
            {
                var loaded = Resources.Load<GameplayConfigSo>(Path);

                if (loaded)
                {
                    return loaded;
                }
                
                Debug.LogError($"Could not load GamePlayConfig at Resources/{Path}, " +
                               $"please check if the files exists or its name!");
                return null;
            }
        }

        #endregion
        
        
        private DamageTypes _currentDamageDamageType = DamageTypes.Standard;
        private IGameplayConfig _gameplayConfigSo;

        private void Awake()
        {
            BootEvents.OnMainSystemRequestInitialize += Initialize;
        }

        private void Initialize()
        {
            BootEvents.OnMainSystemRequestInitialize -= Initialize;
            //

            _gameplayConfigSo = ConfigFileLoader.GetGameplayConfig();
            
            BootEvents.MainSystemInitialized();
        }

        public IGameplayConfig GetGameplayData()
        {
            return _gameplayConfigSo;
        }
        
        #region Damage

        public float GetDamageValue()
        {
            return _currentDamageDamageType switch
            {
                DamageTypes.None => DamageParameters.Values.NoneDamage,
                DamageTypes.Standard => DamageParameters.Values.StandardMeteor,
                DamageTypes.Hard => DamageParameters.Values.HardMeteor,
                DamageTypes.Heavy => DamageParameters.Values.HeavyMeteor,
                DamageTypes.Brutal => DamageParameters.Values.BrutalMeteor,
                _ => DamageParameters.Values.StandardMeteor
            };
        }

        public DamageTypes GetDamageType()
        {
            return _currentDamageDamageType;
        }

        public void SetDamage(DamageTypes damage)
        {
            _currentDamageDamageType = damage;
        }
        
        #endregion
    }
}