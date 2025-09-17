using UnityEngine;

namespace _Main.Scripts.Gameplay.FSM.Earth
{
    public class EarthDeadShakingState<T> : EarthBaseState<T>
    {
        private readonly ActionQueue _queue = new ActionQueue();

        public override void Awake()
        {
            var temp = new ActionData[]
            {
                new(()=>Controller.SetDeathShake(true),
                    EarthDestructionTimeValues.StartShake),
                new(()=>Controller.SetDeathShake(false),
                    EarthDestructionTimeValues.DeathShakeDuration),
                new(()=>Controller.TransitionToDestruction(),
                    EarthDestructionTimeValues.ShowEarthDestruction)
            };
            
            _queue.AddAction(temp);
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