namespace _Main.Scripts.DebugGUI
{
    public struct DebugGUIKeys
    {
        public struct Group
        {
            public const string Fsm = "FSM";
            public const string Gameplay = "Gameplay";
            public const string Input = "Input";
            public const string Managers = "Managers";
            public const string GameScreen = "GameScreen";
            public const string Fps = "FPS";
            public const string Sounds = "Sounds";
        }
    
        public struct SubGroup
        {
            public const string Ability = "Ability";
            public const string Earth = "Earth";
            public const string GameMode = "GameMode";
            public const string Shield = "Shield";
            public const string UpdateManager = "Update";
            public const string SoundChannel = "Channel";
        }
    }

    public struct DebugGUISortingOrder
    {
        public struct Group
        {
            public const int Fps = -1;
            public const int GameScreen = 0;
            public const int Managers = 1;
            public const int Gameplay = 2;
            public const int Fsm = 3;
            public const int Input = 4;
            public const int Sounds = 5;
        }
    
        public struct SubGroup
        {
            public const int MainMenu = -5;
            public const int GameMode = -4;
            public const int Tutorial = -3;
            public const int Cosmetics = -2;
            public const int Earth = 1;
            public const int Shield = 2;
            public const int Ability = 3;
            public const int UpdateManager = 0;
            public const int SoundChannel = 0;
        }
    }
}