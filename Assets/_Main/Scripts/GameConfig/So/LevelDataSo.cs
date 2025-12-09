using _Main.Scripts.Interfaces;
using _Main.Scripts.GameConfig.Game;
using UnityEngine;

namespace _Main.Scripts.GameConfig
{
    [CreateAssetMenu(fileName = "SO_LevelData_Name", menuName = "Scriptable Objects/Game Config/Level Data", order = 0)]
    public class LevelDataSo : ScriptableObject, ILevelData
    {
        [Tooltip("Meteor deflect count needed to increase each internal level")] 
        [Range(1, 100)]
        [SerializeField] private int[] gameplayLevelRequierment;

        public int[] GetGameplayLevelRequierment()
        {
            return gameplayLevelRequierment;
        }

        public void ValidateByLevelAmount(int level)
        {
            GameConfigUtilities.UpdateArray(ref gameplayLevelRequierment, level);
        }
    }
}