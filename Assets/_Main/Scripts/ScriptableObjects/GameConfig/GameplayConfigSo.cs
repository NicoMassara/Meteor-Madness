using _Main.Scripts.Interfaces;
using _Main.Scripts.ScriptableObjects.AbilityTime;
using _Main.Scripts.ScriptableObjects.GameConfig;
using UnityEngine;

namespace _Main.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SO_GameConfig_Name", menuName = "Scriptable Objects/Game Config/Game Data", order = -1)]
    public class GameplayConfigSo : ScriptableObject
    {
        [Header("Values")]
        [Range(1,25)]
        [SerializeField] private int levelAmount;
        [Range(10,500)]
        [SerializeField] private int pointsMultiplier;

        [Space]
        [SerializeField] private LevelDataSo levelData; 
        [SerializeField] private AbilitySelectorDataSo abilitySelectorData; 
        [SerializeField] private ProjectileDataSo projectileData; 
        [SerializeField] private GameTimeDataSo gameTimeData; 
        [SerializeField] private AbilityConfigTimeDataSo abilityTimeData; 
        [SerializeField] private EarthTimeDataSo earthTimeData; 

        #region Getters

        public int LevelAmount => levelAmount;

        public int PointsMultiplier => pointsMultiplier;
        
        public ILevelData LevelData => levelData;
        public IAbilitySelector AbilitySelectorData => abilitySelectorData;
        public IProjectileData ProjectileData => projectileData;
        public IGameTimeData GameTimeData => gameTimeData;
        public IAbilityTimeConfigData AbilityTimeData => abilityTimeData;
        public IEarthTime EarthTimeData => earthTimeData;

        #endregion
        
        private void OnValidate()
        {
            abilitySelectorData?.ValidateByLevelAmount(levelAmount);
            levelData?.ValidateByLevelAmount(levelAmount);
            projectileData?.ValidateByLevelAmount(levelAmount);
        }
    }
}