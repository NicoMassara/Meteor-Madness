namespace _Main.Scripts.Gameplay.FSM.Earth
{
    public class EarthDestructionState<T> : EarthBaseState<T>
    {
        private readonly ActionQueue _queue = new ActionQueue();
        
        public override void Awake()
        {
            ActionData startDestruction = new ActionData(
                ()=>Controller.TriggerDestruction(),0f);
            ActionData endDestruction = new ActionData(
                ()=>Controller.TriggerEndDestruction(),1f);
            ActionData startRotation = new ActionData(
                ()=>Controller.SetRotation(true),GameTimeValues.StartRotatingAfterDeath);
            
            _queue.AddAction(startDestruction);
            _queue.AddAction(endDestruction);
            _queue.AddAction(startRotation);
        }

        public override void Execute(float deltaTime)
        {
            _queue.Run(deltaTime);
        }

        public override void Sleep()
        {
            base.Sleep();
        }
    }
}