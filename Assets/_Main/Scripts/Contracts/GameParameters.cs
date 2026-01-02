using UnityEngine;

namespace MeteorMadness.Contracts
{
    public struct GameParameters
    {
        public struct GameplayValues
        {
            public const int AngleSlots = 32;
            public const int MaxAbilityCount = 3;
            public const SystemLanguage DefaultLanguage = SystemLanguage.English;
            public const bool AdsEnable = false;
            public const bool AnalyticsDebugEnable = true;
            public const bool DoesSendAnalytics = false;
            public const bool DoesSaveProgress = true;
            public const bool HasInfiniteCoins = false;
            public const bool HasAllSkinsUnlocked = false;
            public const float BaseMeteorValue = 10f;
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