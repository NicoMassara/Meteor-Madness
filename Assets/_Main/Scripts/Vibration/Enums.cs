namespace MeteorMadness.Vibration
{
    public enum DurationType
    {
        None,
        ExtraShort,
        Short,
        MediumShort,
        Medium,
        MediumLong,
        Long,
        ExtraLong,
        SuperLong
    }

    public enum IntensityType
    {
        None,
        ExtraLight,
        Light,
        MediumLight,
        Medium,
        MediumHeavy,
        Heavy,
        ExtraHeavy,
        FullHard
    }
    
    public enum UIVibrationType
    {
        None,
        UIButtonAccept,
        UIButtonCancel
    }
    
    public enum VibrationApiType
    {
        None,
        OneShot,
        Waveform
    }

    public enum VibrationPriority
    {
        None = -1,
        Low = 1, // UI
        Medium = 120, // Minor Gameplay
        High = 150, // Damage
        Critical = 200, // Notifications
        SuperCritical = 255 
        
    }
}