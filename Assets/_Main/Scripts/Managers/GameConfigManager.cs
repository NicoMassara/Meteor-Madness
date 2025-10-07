using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.ScriptableObjects;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class GameConfigManager : ManagedBehavior
    {
        [SerializeField] private DamageTypes currentDamageDamageType = DamageTypes.Standard;
        [SerializeField] private GameplayConfigSo gameplayConfigSo;
        [SerializeField] private GameUIConfigSo gameUIConfigSo;
        
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
        
        public GameUIConfigSo GetUIData()
        {
            return gameUIConfigSo;
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