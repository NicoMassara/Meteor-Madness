using _Main.Scripts.Gameplay.Shield;

namespace _Main.Scripts.Gameplay.FSM.Shield
{
    public class ShieldSuperState<T> : ShieldBaseState<T>
    {
        private const float MovementDirection = 50f;
        
        public override void Awake()
        {
            Controller.SetActiveSuperShield(true);
        }

        public override void Execute(float deltaTime)
        {
            Controller.ForceRotate(MovementDirection);
        }

        public override void Sleep()
        {
            Controller.SetActiveSuperShield(false);
        }
    }
}