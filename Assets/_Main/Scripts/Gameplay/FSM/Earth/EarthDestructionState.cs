namespace _Main.Scripts.Gameplay.FSM.Earth
{
    public class EarthDestructionState<T> : EarthBaseState<T>
    {
        private readonly ActionQueue _queue = new ActionQueue();
        
        public override void Awake()
        {
            var temp = new ActionData[]
            {
                new (()=>Controller.TriggerDestruction(),
                    EarthDestructionTimeValues.StartTriggerDestructionTime),
                new (()=>Controller.SetRotation(true),
                    EarthDestructionTimeValues.StartRotatingAfterDeath),
                new (()=>Controller.TriggerEndDestruction(),
                    EarthDestructionTimeValues.EndTriggerDestructionTime),
            };
            
            _queue.AddAction(temp);
        }

        public override void Execute(float deltaTime)
        {
            _queue.Run(deltaTime);
        }
    }
}