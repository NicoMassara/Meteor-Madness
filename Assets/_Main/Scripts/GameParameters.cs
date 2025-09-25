namespace _Main.Scripts
{
    public struct GameValues
    {
        public const int AngleSlots = 32;
        public const int VisualMultiplier = 150;
        public const float MaxMeteorSpeed = 20;
        public const int MaxAbilityCount = 3;
    }

    public struct AbilitiesActiveTimeValues
    {
        public const float SuperShield = 10f + MeteorRingValues.TotalTime;
    }

    public struct SuperShieldStartTimeValues
    {
        public const float TimeToZoomIn = 0.25f;
        public const float TimeToMoveFastShield = 0.45f;
        public const float TimeToZoomOut = 0.25f;
        public const float TimeBeforeIncreasingTimeScale = 0.25f;
    }

    public struct SuperShieldEndTimeValues
    {
        public const float TimeBeforeDisableSuperShield = 0.5f;
        public const float TimeBeforeRestoringTimeScale = 0.15f;
    }
    
    public struct SuperHealTimeValues
    {
        public const float TimeToZoomIn = 0.25f;
        public const float TimeToHeal = 0.5f;
        public const float TimeToZoomOut = 0.25f;
        public const float TimeBeforeIncreasingTimeScale = 0.25f;
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

    public struct MeteorRingValues
    {
        public const int MeteorAmount = 8;
        public const int RingsAmount = 5;
        public const int WavesAmount = 3;
        public const float DelayBetweenRings = 0.15f;
        public const float DelayBetweenWaves = 0.5f;

        public const float TotalTime = (((RingsAmount-1) * DelayBetweenRings)*(WavesAmount-1)) + 
                                       ((WavesAmount-1) * DelayBetweenWaves);
    }

    public struct GameTimeValues
    {
        public const float MeteorSpawnDelayAfterRing = 1f;
        public const int StartGameCount = 0;
        public const float TimeToLoadGameScene = 1f;
        public const float CometSpawnDelay = 8f;
        public const float FirstCometSpawnDelay = 1f;
    }

    public struct EarthDestructionTimeValues
    {
        public const float StartTriggerDestructionTime = 0f;
        public const float EndTriggerDestructionTime = 0.5f;
        public const float StartEarthDestruction = 1.25f;
        public const float StartShake = 0.5f;
        public const float DeathShakeDuration = 2f;
        public const float ShowEarthDestruction = 1f;
        public const float StartRotatingAfterDeath = 1f;
    }

    public struct EarthSliceTimeValues
    {
        public const float StartSlice = 0.05f;
        public const float MoveSlices = 0.05f;
        public const float ReturnToNormalTime = 0.5f;
        public const float ReturnSlices = 0.1f;
    }

    public struct GameRestartTimeValues
    {
        public const float TriggerRestart = 0f;
        public const float RestartEarth = 0.25f;
    }

    public struct EarthRestartTimeValues
    {
        public const float RestartZRotation = 0.25f;
        public const float RestartYRotation = 0.25f;
        public const float TimeBeforeRotateZ = 0.25f;
        public const float TimeBeforeRotateY = 0.25f;
        public const float RestartHealth = 0.75f;
        public const float FinishRestart = 0.25f;
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