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
        StartScreen,
        Empty2
    }
    
    public enum MusicType
    {
        MainMenu,
        Gameplay,
        EndGame,
        Cosmetic
    }

    public enum UISoundType
    {
        Default,
        Confirm,
        Back
    }
}