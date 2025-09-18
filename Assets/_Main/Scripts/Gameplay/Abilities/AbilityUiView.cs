using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Managers.UpdateManager.Interfaces;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityUiView : ManagedBehavior, IUpdatable
    {
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.UI;
        public void ManagedUpdate()
        {

        }
    }
}