namespace _Main.Scripts.Gameplay.FSM.Earth
{
    public class EarthDestructionState<T> : EarthBaseState<T>
    {
        private readonly ActionQueue _queue = new ActionQueue();
        
        public override void Awake()
        {
            Controller.TriggerDestruction();
            Controller.TriggerEndDestruction();
            //Could Trigger 'EndDestruction' on a Queue
            
            ActionData startRotation = new ActionData(
                ()=>Controller.SetRotation(true),GameTimeValues.StartRotatingAfterDeath);
            
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