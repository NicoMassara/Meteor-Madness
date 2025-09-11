using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class GameManager : ManagedBehavior
    {
        [SerializeField] private DamageTypes currentDamageType = DamageTypes.Brutal;
        public static GameManager Instance;
        
        public bool CanPlay { get; set; }
        private int _currentPoints;
        
        public EventBusManager EventManager { get; private set; }

        private void Awake()
        {
            Instance = this;
            EventManager = new EventBusManager();
        }

        #region Damage

        public float GetMeteorDamage()
        {
            return currentDamageType switch
            {
                DamageTypes.None => DamageValues.NoneDamage,
                DamageTypes.Standard => DamageValues.StandardMeteor,
                DamageTypes.Hard => DamageValues.HardMeteor,
                DamageTypes.Heavy => DamageValues.HeavyMeteor,
                DamageTypes.Brutal => DamageValues.BrutalMeteor,
                _ => DamageValues.StandardMeteor
            };
        }

        public void SetDamage(DamageTypes damageType)
        {
            currentDamageType = damageType;
        }

        #endregion
        
        #region Points

        public void IncreasePoints(int multiplier = 1)
        {
            _currentPoints += 1 * multiplier;
        }

        public void ClearCurrentPoints()
        {
            _currentPoints = 0;
        }

        public int GetCurrentPoints()
        {
            return _currentPoints * GameValues.VisualMultiplier;
        }

        #endregion
    }
}