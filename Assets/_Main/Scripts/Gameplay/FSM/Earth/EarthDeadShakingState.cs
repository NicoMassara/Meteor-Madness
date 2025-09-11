using UnityEngine;

namespace _Main.Scripts.Gameplay.FSM.Earth
{
    public class EarthDeadShakingState<T> : EarthBaseState<T>
    {
        private readonly ActionQueue _queue = new ActionQueue();

        public override void Awake()
        {
            ActionData startShake = new ActionData(
                ()=>Controller.SetDeathShake(true),GameTimeValues.StartShake);
            ActionData stopShake = new ActionData(
                ()=>Controller.SetDeathShake(false),GameTimeValues.DeathShakeDuration);
            ActionData transitionToDead = new ActionData(
                ()=>Controller.TransitionToDestruction(),GameTimeValues.ShowEarthDestruction);
            
            
            _queue.AddAction(startShake);
            _queue.AddAction(stopShake);
            _queue.AddAction(transitionToDead);
        }

        public override void Execute()
        {
            _queue.Run();
        }

        public override void Sleep()
        {
            base.Sleep();
        }
    }
}