using UnityEngine;

namespace _Main.Scripts.Gameplay.FSM.Ability
{
    public class AbilityRunningState<T> : AbilityBaseState<T>
    {
        public override void Awake()
        {

            Controller.TriggerAbility();
        }

        public override void Sleep()
        {
            Controller.FinishAbility();
        }
    }
}