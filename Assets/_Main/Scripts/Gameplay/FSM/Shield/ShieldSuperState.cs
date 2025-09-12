using _Main.Scripts.Gameplay.Shield;

namespace _Main.Scripts.Gameplay.FSM.Shield
{
    public class ShieldSuperState<T> : ShieldBaseState<T>
    {
        public override void Awake()
        {
            Controller.SetSpriteByEnum(SpriteType.Super);
        }

        public override void Execute(float deltaTime)
        {
            Controller.ForceRotation();
        }
    }
}