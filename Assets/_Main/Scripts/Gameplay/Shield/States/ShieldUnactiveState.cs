namespace _Main.Scripts.Gameplay.Shield.States
{
    public class ShieldUnactiveState<T> : ShieldBaseState<T>
    {
        public override void Awake()
        {
            Controller.SetActiveShield(false);
        }
    }
}