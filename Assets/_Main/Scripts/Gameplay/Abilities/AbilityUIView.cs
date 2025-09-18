using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Managers.UpdateManager.Interfaces;
using _Main.Scripts.Observer;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityUIView : ManagedBehavior, IUpdatable, IObserver
    {
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.UI;
        public void ManagedUpdate()
        {

        }

        public void OnNotify(string message, params object[] args)
        {
            
        }
    }
}