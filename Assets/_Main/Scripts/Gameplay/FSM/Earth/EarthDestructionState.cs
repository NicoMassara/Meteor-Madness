using UnityEngine;

namespace _Main.Scripts.Gameplay.FSM.Earth
{
    public class EarthDestructionState<T> : EarthBaseState<T>
    {
        private readonly ActionQueue _queue = new ActionQueue();
        
        public override void Awake()
        {
            if (Controller == null)
            {
                Debug.Log("Controller is null");
                return;
            }

            var temp = new ActionData[]
            {
                new (()=>Controller.TriggerDestruction(),
                    EarthParameters.TimeValues.Destruction.StartTriggerDestructionTime),
                new (()=>Controller.SetRotation(true),
                    EarthParameters.TimeValues.Destruction.StartRotatingAfterDeath),
                new (()=>Controller.TriggerEndDestruction(),
                    EarthParameters.TimeValues.Destruction.EndTriggerDestructionTime),
            };

            _queue.AddAction(temp);
        }

        public override void Execute(float deltaTime)
        {
            _queue.Run(deltaTime);
        }
    }
}