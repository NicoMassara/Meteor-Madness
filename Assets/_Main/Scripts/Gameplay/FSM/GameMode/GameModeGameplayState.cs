using _Main.Scripts.Managers;

namespace _Main.Scripts.Gameplay.FSM.GameMode
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