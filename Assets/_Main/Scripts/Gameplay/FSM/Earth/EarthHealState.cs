namespace _Main.Scripts.Gameplay.FSM.Earth
{
    public class EarthHealState<T> : EarthBaseState<T>
    {
        public override void Awake()
        {
            Controller.RestartHealth();
        }
    }
}