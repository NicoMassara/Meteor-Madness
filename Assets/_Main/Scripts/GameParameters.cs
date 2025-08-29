namespace _Main.Scripts
{
    public struct GameValues
    {
        public const int VisualMultiplier = 150;
        public const int StartGameCount = 3;
        public const float ShieldExtend = 1.5f;
        public const float BaseMeteorSpeed = 10f;
        public const int StartStreakShield = 5;
        public const int MaxStreakShield = 10;
        public const int ExtendedMaxHit = 10;
    }

    public struct DamageValues
    {
        public const float StandardMeteor = 0.1f;
        public const float HardMeteor = 0.35f;
        public const float HeavyMeteor = 0.5f;
        public const float BrutalMeteor = 1f;
    }

    public struct UITimeValues
    {
        public const float StartCountingPointsOnDeath = 0.5f;
        public const float EnableRestartButtonOnDeath = 0.75f;
        public const float EnableDeathPanel = 3f;
    }

    public struct GameTimeValues
    {
        public const float TimeToLoadGameScene = 1f;
        public const float DestructionOnDeath = 1.5f;
        public const float ClosePauseMenu = 0.25f;
        public const float DeathShakeDuration = 3f;
        public const float StartShake = 3f;
        public const float MeteorRecycle = 0.5f;
        public const float PointsTextTimeToIncrease = 1.78f;
        public const float PointsTextTimeToIncreaseOnDeath = 5f;
    }

    public struct UITextValues
    {
        public const string Points = "Score";
        public const string DeathPoints = "Your Score";
        public const string DeathText = "Humanity is Over";
        public const string StartText = "Prepare In";
    }
}