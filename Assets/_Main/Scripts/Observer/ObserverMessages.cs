namespace _Main.Scripts.Observer
{
    public struct ShieldObserverMessage
    {
        public const ulong Rotate = 0xA0001;
        public const ulong StopRotate = 0xA0002;
        public const ulong Deflect = 0xA0003;
        public const ulong PlayMoveSound = 0xA0004;
        public const ulong SetGold = 0xA0005;
        public const ulong SetActiveShield = 0xA0006;
        public const ulong SetActiveSuperShield = 0xA0007;
        public const ulong RestartPosition = 0xA0008;
        public const ulong SetAutomatic = 0xA0009;
        public const ulong SetSlow = 0xA0010;
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
    }

    public struct GameModeObserverMessage
    {
        public const ulong StartCountdown = 0xA2001;
        public const ulong UpdateCountdown = 0xA2002;
        public const ulong CountdownFinish = 0xA2003;
        public const ulong StartGameplay = 0xA2004;
        public const ulong MeteorDeflect = 0xA2005;
        public const ulong EarthStartDestruction = 0xA2006;
        public const ulong EarthEndDestruction = 0xA2007;
        public const ulong EarthShaking = 0xA2008;
        public const ulong SetEnableSpawnMeteor = 0xA2009;
        public const ulong PointsGained = 0xA2010;
        public const ulong GameFinish = 0xA2011;
        public const ulong UpdateGameLevel = 0xA2012;
        public const ulong GameRestart = 0xA2013;
        public const ulong EarthRestartFinish = 0xA2014;
        public const ulong GamePaused = 0xA2015;
        public const ulong Disable = 0xA2016;
        public const ulong InitializeValues = 0xA2017;
        public const ulong GrantProjectileSpawn = 0xA2018;
        public const ulong Enable = 0xA2019;
    }

    public struct FlyingObjectObserverMessage
    {
        public const ulong SetValues = 0xA3001;
        public const ulong HandleCollision = 0xA3002;
    }

    public struct MeteorObserverMessage
    {
        public const ulong ShieldDeflection = 0xB3001;
        public const ulong EarthCollision = 0xB3002;
    }
    
    public struct AbilitySphereObserverMessage
    {
        public const ulong ShieldDeflection = 0xC3001;
        public const ulong EarthCollision = 0xC3002;
    }

    public struct AbilityObserverMessage
    {
        public const ulong TriggerAbility = 0xA4001;
        public const ulong FinishAbility = 0xA4002;
        public const ulong AddAbility = 0xA4003;
        public const ulong SetCanUse = 0xA4004;
        public const ulong SetEnableUI = 0xA4005;
        public const ulong SelectAbility = 0xA4006;
        public const ulong Empty = 0xA4007;
        public const ulong RestartAbilities = 0xA4008;
        public const ulong RunActiveTimer = 0xA4009;
        public const ulong SetStorageFull = 0xA4010;
    }
    
    public struct GameScreenObserverMessage
    {
        public const ulong SetMainMenu = 0xA5001;
        public const ulong SetGameplay = 0xA5002;
        public const ulong SetTutorial = 0xA5003;
        public const ulong SetStartLoading = 0xA5004;
    }
    
    public struct TutorialObserverMessage
    {
        public const ulong Start = 0xA6001;
        public const ulong Movement = 0xA6002;
        public const ulong Disable = 0xA6003;
        public const ulong Ability = 0xA6004;
        public const ulong Finish = 0xA6005;
        public const ulong Empty1 = 0xA6006;
        public const ulong Empty2 = 0xA6007;
        public const ulong Enable = 0xA6008;
        public const ulong ExtraMeteors = 0xA6009;
        public const ulong AdditionalProjectile = 0xA6010;
    }

    public struct MainMenuObserverMessage
    {
        public const ulong Enable = 0xA7001;
        public const ulong Disable = 0xA7002;
        public const ulong MainMenu = 0xA7003;
        public const ulong LoreMenu = 0xA7004;
        public const ulong GameMode = 0xA7005;
        public const ulong Tutorial = 0xA7006;
        public const ulong Quit = 0xA7007;
    }
}