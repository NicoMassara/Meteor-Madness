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
        public float HighScore;

        public GameModeDebugData()
        {
            DebugGUIManager.Instance.CreateGroup(DebugGUIKeys.Group.Gameplay, DebugGUISortingOrder.Group.Gameplay)
                ?.CreateSubGroup(DebugGUIKeys.SubGroup.GameMode, DebugGUISortingOrder.SubGroup.GameMode)
                ?.AddEntry(
                    () => $"Level:{CurrentLevel}",
                    () => $"Points:{PointsGained:F2}",
                    () => $"High Score:{HighScore:F2}",
                    () => $"Has High Score:{GetHasSurpasedHighScore()}",
                    () => $"Deflected:{DeflectedMeteor:F2}",
                    () => $"Inputs Enable:{InputsEnable}",
                    () => $"Paused:{IsPaused}"
                );
        }

        private bool GetHasSurpasedHighScore()
        {
            return PointsGained >= HighScore;
        }

    }    

#endif
    

}