using System.Collections.Generic;
using UnityEngine;

namespace _Main.Scripts.Gameplay.FSM.Level
{
    public class LevelDeathState<T> : LevelBaseState<T>
    {
        private readonly ActionQueue _actionQueue = new ActionQueue();
        
        public override void Awake()
        {
            Controller.StartEndLevel();
            
            SetupQueue();
        }

        public override void Execute()
        {
            _actionQueue.Run();
        }

        public override void Sleep()
        {
            Controller.RestartLevel();
        }

        private void SetupQueue()
        {
            //Zoom In
            _actionQueue.AddAction(
                new ActionData(()=> Controller.ZoomIn(), 
                    (float)(GameTimeValues.StartShake * 0.65))
            );
            
            //Start Shake
            _actionQueue.AddAction( 
                new ActionData(()=> Controller.StartEarthShake(),
                    (float)(GameTimeValues.StartShake * 0.35))
            );

            //Destroys Earth
            _actionQueue.AddAction( 
                new ActionData(()=> Controller.TriggerEarthDestruction(),
                    (float)(GameTimeValues.DeathShakeDuration))
            );
            
            //Shows UI
            _actionQueue.AddAction( 
                new ActionData(Queue_FinishLevel,
                    (float)(GameTimeValues.DestructionOnDeath))
            );
        }

        private void Queue_FinishLevel()
        {
            Controller.FinishEndLevel();
            Controller.StopEarthShake();
        }
    }
}