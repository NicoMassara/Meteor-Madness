using _Main.Scripts.GameConfig.Game;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.GameConfig
{
    public class GameConfigManager : ManagedBehavior
    {
        [SerializeField] private DamageTypes currentDamageDamageType = DamageTypes.Standard;
        [SerializeField] private GameplayConfigSo gameplayConfigSo;
        
        public static GameConfigManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null)
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

        public float GetDamageValue()
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

        public DamageTypes GetDamageType()
        {
            return currentDamageDamageType;
        }

        public void SetDamage(DamageTypes damage)
        {
            currentDamageDamageType = damage;
        }
        
        #endregion
    }
}