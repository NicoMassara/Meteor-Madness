using _Main.Scripts.DebugGUI;

namespace _Main.Scripts.Gameplay.GameMode
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD

    public class GameModeDebugData
    {
        public int CurrentLevel;
        public float PointsGained;
        public bool IsPaused;
        public bool InputsEnable;
        public float DeflectedMeteor;

        public GameModeDebugData()
        {
            DebugGUIManager.Instance.CreateGroup(DebugGUIKeys.Group.Gameplay)
                ?.CreateSubGroup(DebugGUIKeys.SubGroup.GameMode)
                ?.AddEntry(
                    () => $"Level:{CurrentLevel}",
                    () => $"Points:{PointsGained:F2}",
                    () => $"Deflected:{IsPaused}",
                    () => $"Inputs Enable:{InputsEnable}",
                    () => $"Paused:{DeflectedMeteor}"
                );
        }
    }    

#endif
    

}