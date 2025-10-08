namespace _Main.Scripts.Gameplay.FSM.GameMode
{
    public class GameModeDeathState<T> : GameModeStateBase<T>
    {
        private ActionQueue _actionQueue = new ActionQueue();
        
        public override void Awake()
        {
            Controller.HandleEarthEndDestruction();
        }

        public override void Execute(float deltaTime)
        {
            _actionQueue.Run(deltaTime);
        }
    }
}