namespace _Main.Scripts.Gameplay.FSM.Ability
{
    public class AbilityRestartState<T> : AbilityBaseState<T>
    {
        public override void Awake()
        {
            Controller.RestartAbilities();
            Controller.TransitionToEnable();
        }
    }
}