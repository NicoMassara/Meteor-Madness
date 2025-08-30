using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Main.Scripts
{
    public class ActionQueue
    {
        private Queue<ActionData> _actionQueue = new Queue<ActionData>();
        private ActionData _currentAction;
        private readonly Timer _timer = new Timer();
        public bool CanRun { get; set; } = true;

        public ActionQueue()
        {
            _timer.OnEnd += OnEndHandler;
        }

        public void Run()
        {
            if(CanRun == false) return;
            
            if (_currentAction == null && _actionQueue.Count > 0)
            {
                _currentAction = _actionQueue.Dequeue();
                _timer.Set(_currentAction.TimeToExecute);
            }

            _timer.Run();
        }

        public void AddAction(ActionData action)
        {
            _actionQueue.Enqueue(action);
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
        
        private void OnEndHandler()
        {
            _currentAction?.Execute();
            _currentAction = null;
        }
    }


    public class ActionData
    {
        private readonly Action _actionToDo;
        public float TimeToExecute { get; }

        public ActionData(Action actionToDo, float timeToExecute)
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