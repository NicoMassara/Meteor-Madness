using MeteorMadness.Contracts.Interfaces.GameplayData.Earth;
using UnityEngine;

namespace MeteorMadness.Contracts.Interfaces.GameplayData
{
    public interface IGameplayConfig
    {
        public int LevelAmount { get; }
        public int PointsMultiplier { get; }
        public ILevelData LevelData { get; }
        public IProjectileData ProjectileData { get; }
        public IGameTimeData GameTimeData { get; }
        public IEarthTime EarthTimeData { get; }
        public ITouchInputData TouchInputData { get; }
    }

    public interface ILevelData
    {
        public int[] GetGameplayLevelRequierment();
    }
    
    public interface IProjectileData
    {
        public IMeteorRingData MeteorRingData { get; }
        public float MaxProjectileSpeed { get; }
        public float MeteorSpawnDelayAfterRing { get; }
        public float GetSpeedMultiplier(float index);
        public float GetTravelRatio(float index);
        public (int[] minSlot, int[] maxSlot) GetSlotData();
    }
 
    
    public interface IGameTimeData
    {
        public int StartGameCount { get; }
        public int CometSpawnDelay { get; }
        public int FirstCometSpawnDelay { get; }
        public float TimeToLoadGameScene { get; }
        public float TriggerRestart { get; }
        public float RestartEarth { get; }
    }
    
    public interface ITouchInputData
    {
        public float GetTopBound();

        public float GetBottomBound();

        public Vector2 GetLeftZoneBounds();
        public Vector2 GetRightZoneBounds();

    }
}