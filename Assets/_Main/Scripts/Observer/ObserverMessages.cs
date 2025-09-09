namespace _Main.Scripts.Observer
{
    public struct ShieldObserverMessage
    {
        public const string Rotate = "A0001";
        public const string StopRotate = "A0002";
        public const string Hit = "A0003";
        public const string PlayMoveSound = "A0004";
        public const string RestartPosition = "A0005";
        public const string SetActiveShield = "A0006";
        public const string SetSpriteType = "A0007";
    }

    public struct EarthObserverMessage
    {
        public const string RestartHealth = "A1001";
        public const string MakeDamage = "A1002";
        public const string DeclareDeath = "A1003";
        public const string TriggerDestruction = "A1004";
        public const string SetActiveDeathShake = "A1005";
        public const string Heal = "A1006";
        public const string SetSprite = "A1007";
    }
}