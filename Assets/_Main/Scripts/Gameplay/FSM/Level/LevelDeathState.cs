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
                    (float)(GameTimeValues.DeathZoom))
            );
            
            //Start Shake
            _actionQueue.AddAction( 
                new ActionData(()=> Controller.StartEarthShake(),
                    (float)(GameTimeValues.StartShake * 0.35))
            );
            
            //Stops Shake
            _actionQueue.AddAction( 
                new ActionData(()=> Controller.StopEarthShake(),
                    (float)(GameTimeValues.DeathShakeDuration))
            );

            //Destroys Earth
            _actionQueue.AddAction( 
                new ActionData(()=> Controller.TriggerEarthDestruction(),
                    (float)(GameTimeValues.ShowEarthDestruction))
            );
            
            //Shows UI
            _actionQueue.AddAction( 
                new ActionData(()=> Controller.FinishEndLevel(),
                    (float)(GameTimeValues.ShowDeathUI))
            );
        }
    }
}