using _Main.Scripts.Interfaces;
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
                    Controller.GetEarthDestructionTimeValues().StartShake),
                new(()=>Controller.SetDeathShake(false),
                    Controller.GetEarthDestructionTimeValues().DeathShakeDuration),
                new(()=>Controller.TransitionToDestruction(),
                    Controller.GetEarthDestructionTimeValues().ShowEarthDestruction)
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