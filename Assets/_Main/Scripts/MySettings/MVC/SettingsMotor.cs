using _Main.Scripts.Observer;

namespace _Main.Scripts.MySettings.MVC
{
    public class SettingsMotor : ObservableComponent
    {
        public void Disable()
        {
            NotifyAll(SettingsObserverMessage.Disable);
        }

        public void Enable()
        {
            NotifyAll(SettingsObserverMessage.Enable);
        }
        
        public void Close()
        {
            NotifyAll(SettingsObserverMessage.Close);
        }

        public void Initial()
        {
            NotifyAll(SettingsObserverMessage.Initial);
        }

        public void Vibration(bool isEnable)
        {
            NotifyAll(SettingsObserverMessage.Vibration,isEnable);
        }
        
        public void Volume(float volume)
        {
            NotifyAll(SettingsObserverMessage.Volume, volume);
        }
        
        public void Language(int index)
        {
            NotifyAll(SettingsObserverMessage.Language, index);
        }
        
    }
}