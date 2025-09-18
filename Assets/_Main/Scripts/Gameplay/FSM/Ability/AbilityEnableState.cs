using UnityEngine;

namespace _Main.Scripts.Gameplay.FSM.Ability
{
    public class AbilityEnableState<T> : AbilityBaseState<T>
    {
        public override void Awake()
        {
            Controller.SetCanUseAbility(true);
        }

        public override void Sleep()
        {
            Controller.SetCanUseAbility(false);
        }
    }
}