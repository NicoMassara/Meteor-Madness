namespace _Main.Scripts.Gameplay.FSM.GameMode
{
    public class GameModeFinishState<T> : GameModeStateBase<T>
    {
        private ActionQueue _actionQueue = new ActionQueue();
        
        public override void Awake()
        {
            Controller.HandleGameFinish();
            
            _actionQueue.AddAction(new ActionData(
                () => Controller.HandleEarthStartDestruction(), 
                EarthDestructionTimeValues.StartEarthDestruction));
        }

        public override void Execute(float deltaTime)
        {
            _actionQueue.Run(deltaTime);
        }
    }
}