namespace _Main.Scripts.DebugGUI
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
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
            public const string AbilitySpawner = "Ability Spawner";
            public const string Earth = "Earth";
            public const string GameMode = "GameMode";
            public const string Shield = "Shield";
            public const string UpdateManager = "Update";
            public const string SoundChannel = "Channel";
            public const string Music = "Music";
        }
    }

    public struct DebugGUISortingOrder
    {
        public struct Group
        {
            public const int Fps = -100;
            public const int GameScreen = -90;
            public const int Managers = -80;
            public const int Gameplay = -70;
            public const int Skin = -60;
            public const int Fsm = -50;
            public const int Input = -30;
            public const int Sounds = 0;
        }
    
        public struct SubGroup
        {
            public const int MainMenu = -50;
            public const int GameMode = -40;
            public const int Tutorial = -30;
            public const int Cosmetics = -20;
            public const int Camera = -19;
            public const int CameraLook = -18;
            public const int CameraZoom = -17;
            public const int Earth = 10;
            public const int Shield = 20;
            public const int Ability = 30;
            public const int UpdateManager = 1;
            public const int SoundChannel = 0;
            public const int Music = -11;
        }
    }
#endif
}