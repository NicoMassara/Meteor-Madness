namespace _Main.Scripts.Interfaces
{
    public interface ISettingsPanel
    {
        public IVolumeSlider VolumeSlider { get; }
        public IVibrationToggle VibrationToggle { get; }
        public ILanguageSelector LanguageSelector { get; }
    }
}