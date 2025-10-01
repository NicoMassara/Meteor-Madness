namespace _Main.Scripts.Gameplay.FSM.GameMode
{
    public class GameModeDeathState<T> : GameModeStateBase<T>
    {
        private ActionQueue _actionQueue = new ActionQueue();
        
        public override void Awake()
        {
            _actionQueue.AddAction(new ActionData(
                () => Controller.HandleEarthEndDestruction(), 
                UIParameters.PanelTimeValues.ShowDeathUI));
        }

        public override void Execute(float deltaTime)
        {
            _actionQueue.Run(deltaTime);
        }
    }
}