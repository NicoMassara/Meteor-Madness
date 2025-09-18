using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Managers.UpdateManager.Interfaces;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityView : ManagedBehavior, IUpdatable
    {
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Ability;
        public void ManagedUpdate()
        {

        }
    }
}