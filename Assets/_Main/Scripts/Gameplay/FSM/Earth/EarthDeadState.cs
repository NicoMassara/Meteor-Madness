using _Main.Scripts.FiniteStateMachine;

namespace _Main.Scripts.Gameplay.FSM.Earth
{
    public class EarthDeadState<T> : EarthBaseState<T>
    {
        
        public override void Awake()
        {
            Controller.SetRotation(false);
            Controller.TriggerDeath();
        }
    }
}