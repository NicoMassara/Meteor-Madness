namespace _Main.Scripts.Observer
{
    public struct ShieldObserverMessage
    {
        public const string Rotate = "A00001";
        public const string StopRotate = "A00002";
        public const string Deflect = "A00003";
        public const string PlayMoveSound = "A00004";
        public const string RestartPosition = "A00005";
        public const string SetActiveShield = "A00006";
        public const string SetSpriteType = "A00007";
    }

    public struct EarthObserverMessage
    {
        public const string RestartHealth = "A01001";
        public const string EarthCollision = "A01002";
        public const string DeclareDeath = "A01003";
        public const string TriggerDestruction = "A01004";
        public const string SetActiveDeathShake = "A01005";
        public const string Heal = "A01006";
        public const string SetRotation = "A01007";
        public const string TriggerEndDestruction = "A01008";
    }

    public struct GameModeObserverMessage
    {
        public const string StartCountdown = "A02001";
        public const string UpdateCountdown = "A02002";
        public const string CountdownFinish = "A02003";
        public const string StartGame = "A02004";
        public const string MeteorDeflect = "A02005";
        public const string EarthStartDestruction = "A02006";
        public const string EarthEndDestruction = "A02007";
        public const string EarthShaking = "A02008";
        public const string SetEnableSpawnMeteor = "A02009";
        public const string SpawnRingMeteor = "A02010";
        public const string GameFinish = "A02011";
        public const string UpdateGameLevel = "A02012";
    }

    public struct FlyingObjectObserverMessage
    {
        public const string SetValues = "A03001";
        public const string HandleCollision = "A03002";
    }

    public struct MeteorObserverMessage
    {
        public const string ShieldDeflection = "A13001";
        public const string EarthCollision = "A13002";
    }
}