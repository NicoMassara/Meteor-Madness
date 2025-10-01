using _Main.Scripts.Gameplay.Shield;

namespace _Main.Scripts.Gameplay.FSM.Shield
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