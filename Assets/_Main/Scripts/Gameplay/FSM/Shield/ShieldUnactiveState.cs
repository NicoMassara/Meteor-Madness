namespace _Main.Scripts.Gameplay.FSM.Shield
{
    public class ShieldUnactiveState<T> : ShieldBaseState<T>
    {
        public override void Awake()
        {
            Controller.SetActiveShield(false);
        }
    }
}