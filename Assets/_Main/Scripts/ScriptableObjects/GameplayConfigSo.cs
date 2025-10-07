using _Main.Scripts.Interfaces;
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
        [SerializeField] private GameLevelDataSo levelData; 
        [SerializeField] private GameAbilitySelectorSo abilitySelector; 
        [SerializeField] private GameProjectileDataSo projectileData; 

        #region Getters

        public int LevelAmount => levelAmount;

        public int PointsMultiplier => pointsMultiplier;
        
        public IGameLevelData LevelData => levelData;

        public IGameAbilitySelector AbilitySelector => abilitySelector;

        public IGameProjectileData ProjectileData => projectileData;

        #endregion
        
        private void OnValidate()
        {
            abilitySelector?.ValidateByLevelAmount(levelAmount);
            levelData?.ValidateByLevelAmount(levelAmount);
            projectileData?.ValidateByLevelAmount(levelAmount);
        }
    }
}