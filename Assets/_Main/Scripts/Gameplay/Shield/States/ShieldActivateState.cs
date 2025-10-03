
namespace _Main.Scripts.Gameplay.Shield.States
{
    public class ShieldActivateState<T> : ShieldBaseState<T>
    {
        // ReSharper disable Unity.PerformanceAnalysis
        public override void Awake()
        {
            Controller.SetActiveShield(true);
        }
    }
}