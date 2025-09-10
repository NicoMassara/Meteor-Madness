using System;
using System.Collections.Generic;
using _Main.Scripts.Managers.UpdateManager.Interfaces;
using UnityEngine;

namespace _Main.Scripts.Managers.UpdateManager
{
    public class UpdateManager : MonoBehaviour
    {
        public static UpdateManager Instance =>  _instance != null ? _instance : (_instance = CreateInstance());
        protected static UpdateManager _instance;

        private readonly List<IUpdatable> _updatableObjects = new List<IUpdatable>();
        private readonly List<ILateUpdatable> _lateUpdatableObjects = new List<ILateUpdatable>();
        private readonly List<IFixedUpdatable> _fixedUpdatableObjects = new List<IFixedUpdatable>();

        public bool IsPaused { get; set; }

        private static UpdateManager CreateInstance()
        {
            var gameObject = new GameObject(nameof(UpdateManager))
            {
                hideFlags = HideFlags.DontSave,
            };
            DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<UpdateManager>();
        }
        
        private void Update()
        {
            if (IsPaused) return;

            for (var index = 0; index < _updatableObjects.Count; index++)
            {
                var t = _updatableObjects[index];
                t?.ManagedUpdate();
            }
        }
        
        private void LateUpdate()
        {
            if (IsPaused) return;

            for (var index = 0; index < _lateUpdatableObjects.Count; index++)
            {
                var t = _lateUpdatableObjects[index];
                t?.ManagedLateUpdate();
            }
        }
        private void FixedUpdate()
        {
            if (IsPaused) return;

            for (var index = 0; index < _fixedUpdatableObjects.Count; index++)
            {
                var t = _fixedUpdatableObjects[index];
                t?.ManagedFixedUpdate();
            }
        }

        public void Register(IManagedObject element)
        {
            if (element is IUpdatable updatable)
            {
                _updatableObjects.Add(updatable);
            }
            if (element is ILateUpdatable lateUpdatable)
            {
                _lateUpdatableObjects.Add(lateUpdatable);
            }
            if (element is IFixedUpdatable fixedUpdatable)
            {
                _fixedUpdatableObjects.Add(fixedUpdatable);
            }
        }

        public void Unregister(IManagedObject element)
        {
            if (element is IUpdatable updatable)
            {
                _updatableObjects.Remove(updatable);
            }
            if (element is ILateUpdatable lateUpdatable)
            {
                _lateUpdatableObjects.Remove(lateUpdatable);
            }
            if (element is IFixedUpdatable fixedUpdatable)
            {
                _fixedUpdatableObjects.Remove(fixedUpdatable);
            }  
        }

        public void Clear()
        {
            _updatableObjects.Clear();
            _lateUpdatableObjects.Clear();
            _fixedUpdatableObjects.Clear();
        }
    }
}