using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Main.Scripts
{
    public class ActionQueue
    {
        private Queue<ActionData> _actionQueue = new Queue<ActionData>();
        private float _currentTimer;
        private ActionData _currentAction;

        public ActionQueue()
        {
            
        }

        public void Run()
        {
            if(_actionQueue.Count == 0) return;
            
            if (_currentTimer > 0)
            {
                if (_currentAction != null)
                {
                    _currentAction = _actionQueue.Dequeue();
                    _currentAction.Execute();
                }
                
                _currentTimer -= Time.deltaTime;

                if (_currentTimer <= 0)
                {
                    _currentAction = null;
                }
            }
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


    }


    public class ActionData
    {
        private readonly Action _actionToDo;
        public float TimeToFinish { get; private set; }

        public ActionData(Action actionToDo, float timeToFinish)
        {
            _actionToDo = actionToDo;
            TimeToFinish = timeToFinish;
        }

        public void Execute()
        {
            _actionToDo();
        }
    }
}