namespace _Main.Scripts
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
        Empty2,
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
}