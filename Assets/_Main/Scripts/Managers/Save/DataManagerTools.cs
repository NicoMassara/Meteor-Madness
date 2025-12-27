using MeteorMadness.Contracts;
using MeteorMadness.GlobalValues.Tools;

namespace MeteorMadness.Managers.Save
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