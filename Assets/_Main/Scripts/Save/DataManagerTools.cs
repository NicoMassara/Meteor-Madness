using _Main.Scripts.CustomId;

namespace _Main.Scripts.Save
{
    public class DataManagerTools
    {
        public static bool GetIsSaveEnabled()
        {
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            return GameParameters.GameplayValues.DoesSaveProgress;
#else
            return true;
#endif
        }
        
        public class GameplayStatsIdData
        {
            public GeneratedId CurrentScoreId;
            public GeneratedId CollisionId;
            public GeneratedId AbilityUseId;
            public GeneratedId DeflectId;
            public GeneratedId StreakId;
            public GeneratedId TimeId;
        }
    }
}