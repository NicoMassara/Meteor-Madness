using _Main.Scripts.CustomId;

namespace _Main.Scripts.Save
{
    public class DataManagerTools
    {
        public static bool GetIsSaveEnabled()
        {
            return GameParameters.GameplayValues.DoesSaveProgress;
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