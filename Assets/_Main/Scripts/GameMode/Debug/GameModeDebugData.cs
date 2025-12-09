using _Main.Scripts.DebugGUI;

namespace _Main.Scripts.GameMode
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD

    public class GameModeDebugData
    {
        public int CurrentLevel;
        public uint PointsGained;
        public bool IsPaused;
        public bool InputsEnable;
        public float DeflectedMeteor;

        public GameModeDebugData()
        {
            DebugGUIManager.Instance.CreateGroup(DebugGUIKeys.Group.Gameplay, DebugGUISortingOrder.Group.Gameplay)
                ?.CreateSubGroup(DebugGUIKeys.SubGroup.GameMode, DebugGUISortingOrder.SubGroup.GameMode)
                ?.AddEntry(
                    () => $"Level:{CurrentLevel}",
                    () => $"Points:{PointsGained:F2}",
                    () => $"Deflected:{DeflectedMeteor:F2}",
                    () => $"Inputs Enable:{InputsEnable}",
                    () => $"Paused:{IsPaused}"
                );
        }

    }    

#endif
    

}