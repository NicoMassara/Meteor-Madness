namespace Plugins.NicolasMassara.CustomSoundManager
{
    public enum SoundChannel
    {
        None,
        Music,
        Collision,
        Meteor,
        Deflection,
        Sfx,
        UI
    }
    
    public enum MixerChannels
    {
        None,
        Master,
        Music,
        Sfx,
        UI,
        Gameplay,
        Meteor,
        Shield,
        Earth
    }
    
    public enum SoundState
    {
        Stopped,
        Foreground,
        Paused,
        Fading,
        Background,
    }
    
    public enum UISoundType
    {
        Default,
        Confirm,
        Back
    }
}