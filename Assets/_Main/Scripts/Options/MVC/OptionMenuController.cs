using _Main.Scripts.Observer;

namespace _Main.Scripts.Options.MVC
{
    public class OptionMenuController : ObservableComponent
    {
        public void Enable()
        {
            NotifyAll(OptionsMenuObserverMessage.Enable);
        }

        public void Initialize()
        {
            NotifyAll(OptionsMenuObserverMessage.Initialize);
        }

        public void Disable()
        {
            NotifyAll(OptionsMenuObserverMessage.Disable);
        }

        public void TriggerMainMenu()
        {
            NotifyAll(OptionsMenuObserverMessage.TriggerMainMenu);
        }
    }
}