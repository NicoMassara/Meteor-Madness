using MeteorMadness.GlobalValues.Interfaces;
using UnityEngine;

namespace MeteorMadness.Managers.GameConfig.Game
{
    [CreateAssetMenu(fileName = "SO_GameConfig_Name", menuName = "Scriptable Objects/Game Config/Game Data", order = -1)]
    public class GameplayConfigSo : ScriptableObject
    {
        [Header("Values")]
        [Range(1,25)]
        [SerializeField] private int levelAmount;
        [Range(1,25)]
        [SerializeField] private int pointsMultiplier;

        [Space]
        [SerializeField] private LevelDataSo levelData; 
        [SerializeField] private ProjectileDataSo projectileData; 
        [SerializeField] private GameTimeDataSo gameTimeData; 
        [SerializeField] private EarthTimeDataSo earthTimeData; 
        [SerializeField] private TouchInputDataSo touchInputData; 

        #region Getters

        public int LevelAmount => levelAmount;

        public int PointsMultiplier => pointsMultiplier;
        
        public ILevelData LevelData => levelData;
        public IProjectileData ProjectileData => projectileData;
        public IGameTimeData GameTimeData => gameTimeData;
        public IEarthTime EarthTimeData => earthTimeData;
        public ITouchInputData TouchInputData => touchInputData;

        #endregion
        
        private void OnValidate()
        {
            levelData?.ValidateByLevelAmount(levelAmount);
            projectileData?.ValidateByLevelAmount(levelAmount);
        }
    }
}