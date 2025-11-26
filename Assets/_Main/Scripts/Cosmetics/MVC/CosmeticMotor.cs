using _Main.Scripts.Observer;

namespace _Main.Scripts.Cosmetics.MVC
{
    public class CosmeticMotor : ObservableComponent
    {
        public void Initial()
        {
            NotifyAll(CosmeticObserverMessage.Initial);
        }

        public void Disable()
        {
            NotifyAll(CosmeticObserverMessage.Disable);
        }

        public void Enable()
        {
            NotifyAll(CosmeticObserverMessage.Enable);
        }

        public void TriggerMainMenu()
        {
            NotifyAll(CosmeticObserverMessage.TriggerMainMenu);
        }

        public void SelectAbility(int index)
        {
            NotifyAll(CosmeticObserverMessage.AbilitySelect,index);
        }
    }
}