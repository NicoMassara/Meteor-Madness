namespace _Main.Scripts.Gameplay.FSM.GameMode
{
    public class GameModeDeathState<T> : GameModeStateBase<T>
    {
        private ActionQueue _actionQueue = new ActionQueue();
        
        public override void Awake()
        {
            ActionData showUI = new ActionData(
                () => Controller.HandleEarthEndDestruction(), GameTimeValues.ShowDeathUI);
            _actionQueue.AddAction(showUI);
        }

        public override void Execute()
        {
            _actionQueue.Run();
        }
    }
}