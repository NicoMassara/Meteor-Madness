namespace _Main.Scripts.Gameplay.FSM.GameMode
{
    public class GameModeFinishState<T> : GameModeStateBase<T>
    {
        private ActionQueue _actionQueue = new ActionQueue();
        
        public override void Awake()
        {
            Controller.HandleGameFinish();
            Controller.HandleEarthStartDestruction();
        }

        public override void Execute(float deltaTime)
        {
            _actionQueue.Run(deltaTime);
        }
    }
}