namespace MeteorMadness.GlobalValues.Tools.Observer
{
    public struct ShieldObserverMessage
    {
        public const ulong Rotate = 0xA0001;
        public const ulong StopRotate = 0xA0002;
        public const ulong Deflect = 0xA0003;
        public const ulong ChangedDirection = 0xA0004;
        public const ulong SetGold = 0xA0005;
        public const ulong SetActiveShield = 0xA0006;
        public const ulong SetActiveSuperShield = 0xA0007;
        public const ulong RestartPosition = 0xA0008;
        public const ulong SetAutomatic = 0xA0009;
        public const ulong SetSlow = 0xA0010;
        public const ulong ChangeMagnitude = 0xA0011;
    }

    public struct EarthObserverMessage
    {
        public const ulong RestartHealth = 0xA1001;
        public const ulong EarthCollision = 0xA1002;
        public const ulong DeclareDeath = 0xA1003;
        public const ulong TriggerDestruction = 0xA1004;
        public const ulong SetActiveDeathShake = 0xA1005;
        public const ulong Heal = 0xA1006;
        public const ulong SetRotation = 0xA1007;
        public const ulong TriggerEndDestruction = 0xA1008;
        public const ulong SetLowHealth = 0xA1009;
        public const ulong PreSlice = 0xA1010;
        public const ulong Debug_UpdateHealth = 0xB1010;
    }

    public struct GameModeObserverMessage
    {
        public const ulong StartCountdown  = 0xA2001; 
        public const ulong UpdateCountdown = 0xA2002; 
        public const ulong FinishCountdown  = 0xA2003; 
        public const ulong StartGameplay = 0xA2004;
        public const ulong SetEnableSpawnMeteor  = 0xA2005;
        public const ulong PointsGained = 0xA2006;
        public const ulong GameFinish  = 0xA2007;
        public const ulong UpdateGameLevel  = 0xA2008;
        public const ulong GamePaused  = 0xA2009;
        public const ulong ExecuteDisable  = 0xA2010;
        public const ulong InitializeData  = 0xA2011;
        public const ulong GrantProjectileSpawn = 0xA2012; 
        public const ulong SetCanPause = 0xA2013;
        public const ulong SaveScore = 0xA2014; 
        public const ulong GameResume = 0xA2015;
        public const ulong StartDisable = 0xA2016; 
        public const ulong StopGameplay = 0xA2017;
        public const ulong DisableGameplayUI = 0xA2018; 
        public const ulong EnableGameplayUI = 0xA2019;
        public const ulong PauseGameModeScreen = 0xA2020;
        public const ulong TriggerEarthDestruction = 0xA2021;
        public const ulong OpenPauseMenu = 0xA2022;
        public const ulong StartFinish = 0xA2023;
        public const ulong FinishAddingPoints = 0xA2024;
        public const ulong UpdateStreak = 0xA2025;
        public const ulong AbilityActive = 0xA2026;
        public const ulong GameInterrupted = 0xA2027;
        public const ulong CheatDetected = 0xA2028;
        public const ulong NotifyStreak = 0xA2029;

        //
        public const ulong Debug_MeteorDeflect = 0xB2001;
        public const ulong Debug_UpdateHighScore = 0xB2002;
    }

    public struct FlyingObjectObserverMessage
    {
        public const ulong SetValues = 0xA3001;
        public const ulong HandleCollision = 0xA3002;
        public const ulong UpdatePosition = 0xA3003;
        public const ulong Empty3 = 0xA3004;
        public const ulong Empty4 = 0xA3005;
        public const ulong Empty5 = 0xA3006;
        public const ulong Empty6 = 0xA3007;
        public const ulong Empty7 = 0xA3008;
        public const ulong Empty8 = 0xA3009;
        public const ulong Empty9 = 0xA3010;
    }
    
    public struct ProjectileObserverMessage
    {
        public const ulong ShieldDeflection = 0xB3001;
        public const ulong EarthCollision = 0xB3002;
        public const ulong Empty2 = 0xB3003;
        public const ulong Empty3 = 0xB3004;
        public const ulong Empty4 = 0xB3005;
        public const ulong Empty5 = 0xB3006;
        public const ulong Empty6 = 0xB3007;
        public const ulong Empty7 = 0xB3008;
        public const ulong Empty8 = 0xB3009;
        public const ulong Empty9 = 0xB3010;
    }

    public struct MeteorObserverMessage
    {
        public const ulong ShieldDeflection = 0xD001;
        public const ulong EarthCollision = 0xD3002;
    }
    
    public struct AbilitySphereObserverMessage
    {
        public const ulong ShieldDeflection = 0xD3001;
        public const ulong EarthCollision = 0xD3002;
    }

    public struct AbilityObserverMessage
    {
        public const ulong TriggerAbility = 0xA4001;
        public const ulong FinishAbility = 0xA4002;
        public const ulong AddAbility = 0xA4003;
        public const ulong SetCanUse = 0xA4004;
        public const ulong EnableUI = 0xA4005;
        public const ulong SelectAbility = 0xA4006;
        public const ulong ForceFinish = 0xA4007;
        public const ulong RestartAbilities = 0xA4008;
        public const ulong RunActiveTimer = 0xA4009;
        public const ulong SetStorageFull = 0xA4010;
        public const ulong DisableUI = 0xA4011;
        public const ulong Initialize = 0xA4012;
        public const ulong RemoveAbiltiyFromUI = 0xA4013;
        public const ulong Empty3 = 0xA4014;
        public const ulong Empty4 = 0xA4015;
        public const ulong Empty5 = 0xA4016;
        public const ulong Empty6 = 0xA4017;
        public const ulong Empty7 = 0xA4018;
        public const ulong Empty8 = 0xA4019;
        public const ulong Empty9 = 0xA4020;
    }
    
    public struct GameScreenObserverMessage
    {
        public const ulong DisableScreen = 0xA5001;
        public const ulong LoadScreen = 0xA5002;
        public const ulong ZoomIn = 0xA5003;
        public const ulong Empty2 = 0xA5004;
        public const ulong Empty3 = 0xA5005;
    }
    
    public struct TutorialObserverMessage
    {
        public const ulong EnableHint = 0xA6001;
        public const ulong Meteor = 0xA6002;
        public const ulong Disable = 0xA6003;
        public const ulong Ability = 0xA6004;
        public const ulong Finish = 0xA6005;
        public const ulong SphereDeflected = 0xA6006;
        public const ulong AbilityRunning = 0xA6007;
        public const ulong Enable = 0xA6008;
        public const ulong ExtraMeteors = 0xA6009;
        public const ulong AdditionalProjectile = 0xA6010;
        public const ulong MultiPage = 0xA6011;
        public const ulong DisableHint = 0xA6012;
        public const ulong RightMovement = 0xA6013;
        public const ulong LeftMovement = 0xA6014;
        public const ulong RightMovementFinished = 0xA6015;
        public const ulong LeftMovementFinished = 0xA6016;
        public const ulong Empty6 = 0xA6017;
    }

    public struct MainMenuObserverMessage
    {
        public const ulong Enable = 0xA7001;
        public const ulong Disable = 0xA7002;
        public const ulong MainMenu = 0xA7003;
        public const ulong LoreMenu = 0xA7004;
        public const ulong TriggerGameMode = 0xA7005;
        public const ulong TriggerTutorial = 0xA7006;
        public const ulong Quit = 0xA7007;
        public const ulong TutorialMenu = 0xA7008;
        public const ulong TriggerCosmetic = 0xA7009;
        public const ulong CreditsMenu = 0xA7010;
        public const ulong TriggerOptions = 0xA7011;
        public const ulong StartDisable = 0xA7012;
        public const ulong MainPanelOpened = 0xA7013;
        public const ulong FirstGame = 0xA7014;
        public const ulong LoreClosed = 0xA7015;
        public const ulong Stats = 0xA7016;
        public const ulong Empty1 = 0xA7017;
    }

    public struct CosmeticObserverMessage
    {
        public const ulong Enable = 0xA8001;
        public const ulong Disable = 0xA8002;
        public const ulong Initialize = 0xA8003;
        public const ulong TriggerMainMenu = 0xA8004;
        public const ulong SkinSelected = 0xA8005;
        public const ulong StartDisable = 0xA8006;
        public const ulong SkinChanged = 0xA8007;
        public const ulong Opened = 0xA8008;
        public const ulong TryUnlockSkin = 0xA8009;
        public const ulong Unlocked = 0xA8010;
        public const ulong FailedToUnlock = 0xA8011;
        public const ulong FirstOpen = 0xA8012;
    }

    public struct InputsUIObserverMessage
    {
        public const ulong SetEnableClock = 0xA9001;
        public const ulong SetEnableCounterClock = 0xA9002;
        public const ulong Empty1 = 0xA9003;
        public const ulong SetEnableUI = 0xA9004;
        public const ulong Initialize = 0xA9005;
        public const ulong Destroy = 0xA9006;
    }

    public struct SettingsObserverMessage
    {
        public const ulong Enable = 0xA1001;
        public const ulong Disable = 0xA1002;
        public const ulong Initial = 0xA1003;
        public const ulong Language = 0xA1004;
        public const ulong Vibration = 0xA1005;
        public const ulong Volume = 0xA1006;
        public const ulong Close = 0xA1007;
        public const ulong StartDisable = 0xA1008;
        public const ulong Empty1 = 0xA1009;
        public const ulong Empty2 = 0xA1010;
        public const ulong Empty3 = 0xA1011;
    }
    
    public struct CameraObserverMessage
    {
        public const ulong Move = 0xA1101;
        public const ulong Zoom = 0xA1102;
        public const ulong Shake = 0xA1103;
        public const ulong Empty3 = 0xA1104;
        public const ulong Empty4 = 0xA1105;
        public const ulong Empty5 = 0xA1106;
        public const ulong Empty6 = 0xA1107;
        public const ulong Empty7 = 0xA1108;
        public const ulong EnableGrayscale = 0xA1109;
        public const ulong DisableGrayscale = 0xA1110;
    }
    
    public struct DefeatObserverMessage
    {
        public const ulong ExecuteDisable   = 0xA1201;
        public const ulong StartDisable     = 0xA1202;
        public const ulong InitializeData   = 0xA1203;
        public const ulong Enable           = 0xA1204;
        public const ulong LoadData         = 0xA1205;
        public const ulong SendScore        = 0xA1206;
        public const ulong SendHighScore    = 0xA1207;
        public const ulong SendButtons      = 0xA1208;
        public const ulong EnableButtons    = 0xA1209;
        public const ulong Empty2    = 0xA120A;
        public const ulong SendAds          = 0xA120B;
        public const ulong RestartGame      = 0xA120C;
        public const ulong LoadMainMenu     = 0xA120D;
        public const ulong SendCoins        = 0xA120F;
        public const ulong CheckNewCoins    = 0xA1210;
        public const ulong UpdateCoins      = 0xA1211;
        public const ulong Empty3           = 0xA1212;
        public const ulong Empty4           = 0xA1213;
        public const ulong Empty5           = 0xA1214;
        public const ulong Empty6           = 0xA1215;
        public const ulong Empty7           = 0xA1216;
        public const ulong Empty8           = 0xA1217;
        public const ulong Empty9           = 0xA1218;
        public const ulong Empty10          = 0xA1219;
        public const ulong Empty11          = 0xA121A;
        public const ulong Empty12          = 0xA121B;
        public const ulong Empty13          = 0xA121C;
        public const ulong Empty14          = 0xA121D;
        public const ulong Empty15          = 0xA121E;
    }
    
    public struct PauseObserverMessage
    {
        public const ulong Initialize  = 0xA1301;
        public const ulong Enable  = 0xA1302;
        public const ulong StartDisable  = 0xA1303;
        public const ulong ExecuteDisable  = 0xA1304;
        public const ulong Options  = 0xA1305;
        public const ulong LoadMainMenu  = 0xA1306;
        public const ulong RestartEarth  = 0xA1307;
        public const ulong Empty8  = 0xA1308;
        public const ulong Empty9  = 0xA1309;
        public const ulong GameMode = 0xA130A;
    }

    public struct StatsObserverMessage
    {
        public const ulong Initialize  = 0xA1401;
        public const ulong Enable  = 0xA1402;
        public const ulong StartDisable  = 0xA1403;
        public const ulong ExecuteDisable  = 0xA1404;
        public const ulong MainMenu  = 0xA1405;
        public const ulong LoadTextData  = 0xA1406;
        public const ulong FirstOpen  = 0xA1407;
        public const ulong Opened  = 0xA1408;
        public const ulong Empty9  = 0xA1409;
        public const ulong Empty10 = 0xA140A;
    }

    public struct InputPanelObserverMessage
    {
        public const ulong Enable  = 0xA1501;
        public const ulong Disable = 0xA1502;
        public const ulong Collision  = 0xA1503;
        public const ulong Empty3  = 0xA1504;
        public const ulong Empty4 = 0xA1505;
        public const ulong Empty5  = 0xA1506;
        public const ulong Empty7  = 0xA1507;
        public const ulong Empty8  = 0xA1508;
        public const ulong Empty9  = 0xA1509;
        public const ulong Empty10 = 0xA150A;
    }

    public struct ProjectileSpawnerObserverMessage
    {
        public const ulong SpawnMeteor  = 0xA1601;
        public const ulong SpawnRing  = 0xA1602;
        public const ulong BatchSpawned  = 0xA1603;
        public const ulong Clear  = 0xA1604;
        public const ulong BatchDeflected  = 0xA1605;
        public const ulong Empty6  = 0xA1606;
        public const ulong Empty7  = 0xA1607;
        public const ulong Empty8  = 0xA1608;
        public const ulong Empty9  = 0xA1609;
        public const ulong Empty10 = 0xA160A;
    }

    public struct EmptyObserverMessage5
    {
        public const ulong Empty1  = 0xA1701;
        public const ulong Empty2  = 0xA1702;
        public const ulong Empty3  = 0xA1703;
        public const ulong Empty4  = 0xA1704;
        public const ulong Empty5  = 0xA1705;
        public const ulong Empty6  = 0xA1706;
        public const ulong Empty7  = 0xA1707;
        public const ulong Empty8  = 0xA1708;
        public const ulong Empty9  = 0xA1709;
        public const ulong Empty10 = 0xA170A;
    }

    public struct EmptyObserverMessage6
    {
        public const ulong Empty1  = 0xA1801;
        public const ulong Empty2  = 0xA1802;
        public const ulong Empty3  = 0xA1803;
        public const ulong Empty4  = 0xA1804;
        public const ulong Empty5  = 0xA1805;
        public const ulong Empty6  = 0xA1806;
        public const ulong Empty7  = 0xA1807;
        public const ulong Empty8  = 0xA1808;
        public const ulong Empty9  = 0xA1809;
        public const ulong Empty10 = 0xA180A;
    }

    public struct EmptyObserverMessage7
    {
        public const ulong Empty1  = 0xA1901;
        public const ulong Empty2  = 0xA1902;
        public const ulong Empty3  = 0xA1903;
        public const ulong Empty4  = 0xA1904;
        public const ulong Empty5  = 0xA1905;
        public const ulong Empty6  = 0xA1906;
        public const ulong Empty7  = 0xA1907;
        public const ulong Empty8  = 0xA1908;
        public const ulong Empty9  = 0xA1909;
        public const ulong Empty10 = 0xA190A;
    }

    public struct EmptyObserverMessage8
    {
        public const ulong Empty1  = 0xA1A01;
        public const ulong Empty2  = 0xA1A02;
        public const ulong Empty3  = 0xA1A03;
        public const ulong Empty4  = 0xA1A04;
        public const ulong Empty5  = 0xA1A05;
        public const ulong Empty6  = 0xA1A06;
        public const ulong Empty7  = 0xA1A07;
        public const ulong Empty8  = 0xA1A08;
        public const ulong Empty9  = 0xA1A09;
        public const ulong Empty10 = 0xA1A0A;
    }

    public struct EmptyObserverMessage9
    {
        public const ulong Empty1  = 0xA1B01;
        public const ulong Empty2  = 0xA1B02;
        public const ulong Empty3  = 0xA1B03;
        public const ulong Empty4  = 0xA1B04;
        public const ulong Empty5  = 0xA1B05;
        public const ulong Empty6  = 0xA1B06;
        public const ulong Empty7  = 0xA1B07;
        public const ulong Empty8  = 0xA1B08;
        public const ulong Empty9  = 0xA1B09;
        public const ulong Empty10 = 0xA1B0A;
    }

    public struct EmptyObserverMessage10
    {
        public const ulong Empty1  = 0xA1C01;
        public const ulong Empty2  = 0xA1C02;
        public const ulong Empty3  = 0xA1C03;
        public const ulong Empty4  = 0xA1C04;
        public const ulong Empty5  = 0xA1C05;
        public const ulong Empty6  = 0xA1C06;
        public const ulong Empty7  = 0xA1C07;
        public const ulong Empty8  = 0xA1C08;
        public const ulong Empty9  = 0xA1C09;
        public const ulong Empty10 = 0xA1C0A;
    }
}