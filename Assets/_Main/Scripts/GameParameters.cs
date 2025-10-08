namespace _Main.Scripts
{
    public struct GameParameters
    {
        public struct GameplayValues
        {
            public const int AngleSlots = 32;
            public const int MaxAbilityCount = 3;
        }
    }

    public struct DamageParameters
    {
        public struct Values
        {
            public const float NoneDamage = 0f;
            public const float StandardMeteor = 0.1f;
            public const float HardMeteor = 0.35f;
            public const float HeavyMeteor = 0.5f;
            public const float BrutalMeteor = 1f;
        }
    }
}