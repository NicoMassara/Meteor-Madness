using MeteorMadness.GlobalValues.Tools.Observer;

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
        public void TriggerOpened() => NotifyAll(CosmeticObserverMessage.Opened);
        public void TryUnlockSkin(int skinIndex) => NotifyAll(CosmeticObserverMessage.TryUnlockSkin, skinIndex);
        public void SkinUnlocked(int skinIndex) => NotifyAll(CosmeticObserverMessage.Unlocked, skinIndex);
        public void FailedToUnlock() => NotifyAll(CosmeticObserverMessage.FailedToUnlock);
        public void OpenFirstPanel() => NotifyAll(CosmeticObserverMessage.FirstOpen);
    }
}