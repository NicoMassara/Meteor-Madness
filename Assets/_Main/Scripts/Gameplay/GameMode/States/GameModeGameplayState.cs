using _Main.Scripts.Managers;

namespace _Main.Scripts.Gameplay.GameMode.States
{
    public class GameModeGameplayState<T> : GameModeStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetEnableMeteorSpawn(true);
        }
        
        public override void Sleep()
        {
            Controller.SetEnableMeteorSpawn(false);
        }
    }
}