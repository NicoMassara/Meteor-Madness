namespace _Main.Scripts.Gameplay.Shield.States
{
    public class ShieldGoldState<T> : ShieldBaseState<T>
    {
        public override void Awake()
        {
            Controller.SetActiveGold(true);
        }

        public override void Sleep()
        {
            Controller.SetActiveGold(false);
        }
    }
}