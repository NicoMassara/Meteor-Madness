namespace _Main.Scripts.Gameplay.Shield.States
{
    public class ShieldAutomaticState<T> : ShieldBaseState<T>
    {
        public override void Awake()
        {
            Controller.SetActiveAutomatic(true);
        }

        public override void Sleep()
        {
            Controller.SetActiveAutomatic(false);
        }
    }
}