using _Main.Scripts.Observer;

namespace _Main.Scripts.Cosmetics.MVC
{
    public class CosmeticMotor : ObservableComponent
    {
        public void Initialize() => NotifyAll(CosmeticObserverMessage.Initialize);
        public void Disable() => NotifyAll(CosmeticObserverMessage.Disable);
        public void Enable() => NotifyAll(CosmeticObserverMessage.Enable);
        public void TriggerMainMenu() => NotifyAll(CosmeticObserverMessage.TriggerMainMenu);
        public void SelectSkin(int index) => NotifyAll(CosmeticObserverMessage.SkinSelected,index);
        public void StartDisable() => NotifyAll(CosmeticObserverMessage.StartDisable);
    }
}