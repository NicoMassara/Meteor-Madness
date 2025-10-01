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
                    EarthParameters.TimeValues.Destruction.StartShake),
                new(()=>Controller.SetDeathShake(false),
                    EarthParameters.TimeValues.Destruction.DeathShakeDuration),
                new(()=>Controller.TransitionToDestruction(),
                    EarthParameters.TimeValues.Destruction.ShowEarthDestruction)
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