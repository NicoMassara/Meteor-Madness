
namespace _Main.Scripts.Gameplay.Shield.States
{
    public class ShieldSuperState<T> : ShieldBaseState<T>
    {
        private const float MovementDirection = 1f;
        
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