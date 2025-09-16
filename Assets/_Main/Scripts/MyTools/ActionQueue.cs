using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Main.Scripts
{
    public class ActionQueue
    {
        private Queue<ActionData> _actionQueue = new Queue<ActionData>();
        private ActionData _currentAction;
        private readonly Timer _executeTimer = new Timer();
        public bool IsEmpty => _graceTimer <= 0;

        //Time added to delay the Queue finish, used to added extra Actions and avoid 
        //the Coroutine to stop 
        private const float GraceTime = 3f;
        private float _graceTimer;

        public ActionQueue()
        {
            _executeTimer.OnEnd += Timer_OnEndHandler;
        }

        public void Run(float deltaTime)
        {
            if (_currentAction == null)
            {
                //Is empty
                if (_actionQueue.Count == 0)
                {
                    _graceTimer -= deltaTime;
                }
                //Has actions
                else if (_actionQueue.Count > 0)
                {
                    _currentAction = _actionQueue.Dequeue();
                    _executeTimer.Set(_currentAction.TimeToExecute);
                }
            }
            else
            {
                _executeTimer.Run(deltaTime);
            }
        }
        
        private void Timer_OnEndHandler()
        {
            _currentAction.Execute();
            _currentAction = null;
        }

        public void AddAction(ActionData action)
        {
            _actionQueue.Enqueue(action);
            _graceTimer = GraceTime;
        }

        public void RemoveAction(ActionData action)
        {
            if (_actionQueue.Contains(action))
            {
                Queue<ActionData> newQueue = new Queue<ActionData>();

                while (_actionQueue.Count > 0)
                {
                    var current = _actionQueue.Dequeue();
                    if (current != action)   // keep all except the one to remove
                        newQueue.Enqueue(current);
                }

                _actionQueue = newQueue;
            }
        }
    }


    public class ActionData
    {
        private readonly Action _actionToDo;
        public float TimeToExecute { get; }
        
        public ActionData(Action actionToDo, float timeToExecute = 0)
        {
            _actionToDo = actionToDo;
            TimeToExecute = timeToExecute;
        }
        public void Execute()
        {
            _actionToDo?.Invoke();
        }
    }
}