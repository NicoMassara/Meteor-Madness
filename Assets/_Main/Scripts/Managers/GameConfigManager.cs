using System;
using _Main.Scripts.Gameplay.Projectile;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.ScriptableObjects;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class GameConfigManager : ManagedBehavior
    {
        [SerializeField] private DamageTypes currentDamageDamageType = DamageTypes.Standard;
        [SerializeField] private GameplayConfigSo gameplayConfigSo;
        
        public static GameConfigManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public GameplayConfigSo GetGameplayData()
        {
            return gameplayConfigSo;
        }
        
        
        #region Damage

        public float GetDamage()
        {
            return currentDamageDamageType switch
            {
                DamageTypes.None => DamageParameters.Values.NoneDamage,
                DamageTypes.Standard => DamageParameters.Values.StandardMeteor,
                DamageTypes.Hard => DamageParameters.Values.HardMeteor,
                DamageTypes.Heavy => DamageParameters.Values.HeavyMeteor,
                DamageTypes.Brutal => DamageParameters.Values.BrutalMeteor,
                _ => DamageParameters.Values.StandardMeteor
            };
        }

        public void SetDamage(DamageTypes damage)
        {
            currentDamageDamageType = damage;
        }
        
        #endregion
    }
}