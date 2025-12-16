namespace Plugins.NicolasMassara.CustomSoundManager
{
    public enum SoundChannel
    {
        None,
        Music,
        Sfx,
        UI
    }
    
    public enum MixerChannels
    {
        None,
        Master,
        Music,
        Sfx,
        UI
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