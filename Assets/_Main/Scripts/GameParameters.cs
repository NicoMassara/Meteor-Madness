namespace _Main.Scripts
{
    public struct GameValues
    {
        public const int VisualMultiplier = 150;
        public const int StartGameCount = 3;
        public const float PointsTextTimeToIncrease = 1.78f;
        public const float PointsTextTimeToIncreaseOnDeath = 5f;
        public const float StandardMeteorDamage = 0.1f;
        public const float HardMeteorDamage = 0.35f;
        public const float HeavyMeteorDamage = 0.5f;
        public const float BrutalMeteorDamage = 1f;
        public const int BaseMeteorNeed = 10;
        public const float MeteorRecycleTime = 5f;
    }

    public struct UITimeValues
    {
        public const float StartCountingPointsOnDeath = 0.5f;
        public const float EnableRestartButtonOnDeath = 0.75f;
        public const float EnableDeathPanel = 3f;
    }

    public struct GameTimeValues
    {
        public const float DestructionTimeOnDeath = 1.5f;
    }

    public struct UITextValues
    {
        public const string Points = "Score";
        public const string DeathPoints = "Your Score";
        public const string DeathText = "Humanity is Over";
        public const string StartText = "Prepare In";
    }
}