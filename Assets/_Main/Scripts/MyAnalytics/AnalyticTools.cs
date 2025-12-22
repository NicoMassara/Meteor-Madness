namespace _Main.Scripts.MyAnalytics
{
    public struct AnalyticEventsName
    {
        public struct Defeat
        {
            public const string Saved = "defeat_saved";
            public const string Restart = "defeat_restart";
            public const string MainMenu = "defeat_mainMenu";
        }
        
        public struct Cosmetics
        {
            public const string Opened = "cosmetics_opened";
            
            public struct Changed
            {
                public const string EventName = "cosmetics_changed";
                public const string SkinType = "skin_changed";
            }
        }
        
        public struct Tutorial
        {
            public const string Completed = "tutorial_completed";
        }
        
        public struct MainMenu
        {
            public struct Lore
            {
                public const string Opened = "mainmenu_lore_open";
                
                public struct Closed
                {
                    public const string EventName = "mainmenu_lore_close";
                    public const string ElapsedTime = "time_seconds";
                }
            }
        }
        
        public struct GameMode
        {
            public const string Start = "game_start";
            
            public struct Interrupted
            {
                public const string EventName = "game_interrupted";
                public const string ElapsedTime = "time_seconds";
                public const string GainedPoints = "points";
                public const string Level = "level";
                public const string Ability = "ability";
                public const string MaxStreak = "streak";
            }
            
            public struct Completed
            {
                public const string EventName = "game_complete";
                public const string ElapsedTime = "time_seconds";
                public const string GainedPoints = "points";
                public const string Level = "level";
                public const string Ability = "ability";
                public const string MaxStreak = "streak";
            }
        }
    }
}