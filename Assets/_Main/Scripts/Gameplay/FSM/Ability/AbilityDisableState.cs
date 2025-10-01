using UnityEngine.U2D.IK;

namespace _Main.Scripts.Gameplay.FSM.Ability
{
    public class AbilityDisableState<T> : AbilityBaseState<T>
    {
        public override void Awake()
        {
            Controller.SetCanUseAbility(false);
            Controller.SetEnableUIAbility(false);
        }
    }
}