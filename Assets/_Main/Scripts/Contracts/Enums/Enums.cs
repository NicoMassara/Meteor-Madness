namespace MeteorMadness.Contracts
{
    public enum ProjectileType
    {
        None,
        Meteor,
        AbilitySphere
    }
    
    public enum EventRequestType
    {
        Requested,
        Granted,
        Denied
    }
    
    public enum AbilityType
    {
        None,
        SuperShield,
        Health,
        SlowMotion,
        DoublePoints,
        Automatic,
        
        Default_MAX
    }
    
    public enum DamageTypes
    {
        None,
        Standard,
        Hard,
        Heavy,
        Brutal
    }

    public enum ScreenType
    {
        None,
        MainMenu,
        GameMode,
        Tutorial,
        Cosmetic,
        OptionsMenu,
        Defeat,
        Pause,
        Stats,
        Empty3,
        Empty4,
        Empty5,
        Empty6,
        Empty7,
        Empty8,
        Empty9,
        Empty10,
        
    }
    
    public enum MusicType
    {
        MainMenu,
        Gameplay,
        EndGame,
        Cosmetic
    }
    
    
    public enum ShieldType
    {
        None,
        Super,
        Gold,
        Automatic,
        Slow
    }
    
    public enum SkinType
    {
        None,
        //
        Default,
        Pizza,
        Vinyl,
        GoldenEarth,
        Meteor,
        Empty4,
        Empty5,
        Empty6,
        Empty7,
        Empty8,
        Empty9,
        Empty10,
        Empty11,
        Empty12,
        Empty13,
        Empty14,
        Empty15,
        Empty16,
        Empty17,
        Empty18,
        Empty19,
        Empty20,
        Empty21,
        Empty22,
        Empty23,
        Empty24,
        Empty25,
        Empty26,
        Empty27,
        Empty28,
        Empty29,
        Empty30,
        Empty31,
        Empty32,
        Empty33,
        Empty34,
        Empty35,
        Empty36,
        Empty37,
        Empty38,
        Empty39,
        Empty40,
        Empty41,
        Empty42,
        Empty43,
        Empty44,
        Empty45,
        Empty46,
        Empty47,
        Empty48,
        Empty49,
        Empty50,
        Empty51,
        Empty52,
        Empty53,
        Empty54,
        Empty55,
        Empty56,
        Empty57,
        Empty58,
        Empty59,
        Empty60,
        Empty61,
        Empty62,
        Empty63,
        Empty64,
        Empty65,
        Empty66,
        Empty67,
        Empty68,
        Empty69,
        Empty70,
        //
        DEFAULT_MAX
    }

    public enum StatType
    {
        None,
        //
        HighScore,
        Collision,
        Deflect,
        Ability,
        Streak,
        TimesPlayed,
        TotalScored,
        LongestTime,
        //
        DEFAULT_MAX
    }
}