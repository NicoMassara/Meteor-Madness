namespace _Main.Scripts
{
    public struct GameValues
    {
        public const int VisualMultiplier = 150;
        public const float ShieldExtend = 1.5f;
        public const float BaseMeteorSpeed = 10f; //10
        public const int StartStreakShield = 5;
        public const int MaxStreakShield = 10;
        public const int ExtendedMaxHit = 10;
    }

    public struct DamageValues
    {
        public const float NoneDamage = 0f;
        public const float StandardMeteor = 0.1f;
        public const float HardMeteor = 0.35f;
        public const float HeavyMeteor = 0.5f;
        public const float BrutalMeteor = 1f;
    }

    public enum DamageTypes
    {
        None,
        Standard,
        Hard,
        Heavy,
        Brutal
    }

    public struct GameTimeValues
    {
        public const int StartGameCount = 3;
        public const float TimeToLoadGameScene = 1f;
        //Show Death
        public const float StartEarthDestruction = 1.25f;
        public const float StartShake = 1.5f;
        public const float DeathShakeDuration = 2f;
        public const float ShowEarthDestruction = 1.5f;
        public const float StartRotatingAfterDeath = 1f;
        public const float MeteorSpawnDelay = 8f;
        public const float MeteorDelayBetweenSpawn = 0.25f;
        public const float CometSpawnDelay = 8f;
        public const float FirstCometSpawnDelay = 10f;
    }

    public struct UITextValues
    {
        public const string Points = "Score";
        public const string DeathPoints = "Your Score";
        public const string DeathText = "Humanity is Over";
        public const string GameCountdownText = "Prepare In";
        public const string GameCountdownFinish = "Defend!";
    }

    public struct UIPanelTimeValues
    {
        public const float GameplayPointsTimeToIncrease = 0.35f;
        public const float ClosePauseMenu = 0.25f;
        // Death Panel
        public const float ShowDeathUI = 1.5f;
        public const float SetEnableDeathText = 1.5f;
        public const float SetEnableDeathScore = 1f;
        public const float DeathPointsTimeToIncrease = 1f;
        public const float CountDeathScore =  0.75f;
        public const float EnableRestartButton = 1f;

    }
}