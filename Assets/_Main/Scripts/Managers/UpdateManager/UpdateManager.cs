using System;
using System.Collections.Generic;
using _Main.Scripts.Managers.UpdateManager.Interfaces;
using _Main.Scripts.MyCustoms;
using UnityEngine;

namespace _Main.Scripts.Managers.UpdateManager
{
    public class UpdateManager : MonoBehaviour
    {
        public static UpdateManager Instance =>  _instance != null ? _instance : (_instance = CreateInstance());
        protected static UpdateManager _instance;

        private readonly List<IUpdatable> _updatableObjects = new List<IUpdatable>();
        private readonly List<IFixedUpdatable> _fixedUpdatableObjects = new List<IFixedUpdatable>();
        private readonly List<ILateUpdatable> _lateUpdatableObjects = new List<ILateUpdatable>();
        
        private readonly List<IUpdatable> _toAdd = new List<IUpdatable>();
        private readonly List<IUpdatable> _toRemove = new List<IUpdatable>();
        private readonly List<IFixedUpdatable> _fixedToAdd = new List<IFixedUpdatable>();
        private readonly List<IFixedUpdatable> _fixedToRemove = new List<IFixedUpdatable>();
        private readonly List<ILateUpdatable> _lateToAdd = new List<ILateUpdatable>();
        private readonly List<ILateUpdatable> _lateToRemove = new List<ILateUpdatable>();
        
        public bool IsPaused { get; set; }
        
#pragma warning disable CS0414 // Field is assigned but its value is never used
        private bool _isUpdating = false;
        private bool _isFixedUpdating = false;
        private bool _isLateUpdating = false;
#pragma warning restore CS0414 // Field is assigned but its value is never used
        
        private static UpdateManager CreateInstance()
        {
            var gameObject = new GameObject(nameof(UpdateManager))
            {
                hideFlags = HideFlags.DontSave,
            };
            DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<UpdateManager>();
        }
        
        #region Update

        private void Update()
        {
            CustomTime.UpdateAll(IsPaused ? 0 : Time.unscaledDeltaTime);
            
            ApplyPending();
            
            
            _isUpdating = true;

            if (!IsPaused)
            {
                for (int i = 0; i < _updatableObjects.Count; i++)
                {
                    var u = _updatableObjects[i];
                
                    if(CustomTime.GetChannel(u.SelfUpdateGroup).IsPaused)
                        continue;
                
                    u.ManagedUpdate();
                }
            }
            
            _isUpdating = false;
            
            ApplyPending();
        }

        private void FixedUpdate()
        {
            CustomTime.FixedUpdateAll(IsPaused ? 0 : Time.fixedUnscaledDeltaTime);
            
            ApplyPendingFixed();
            
            _isFixedUpdating = true;

            if (!IsPaused)
            {
                for (int i = 0; i < _fixedUpdatableObjects.Count; i++)
                {
                    var u = _fixedUpdatableObjects[i];
                
                    if(CustomTime.GetChannel(u.SelfFixedUpdateGroup).IsPaused)
                        continue;

                    u.ManagedFixedUpdate();
                }
            }
            
            _isFixedUpdating = false;
            
            ApplyPendingFixed();
        }
        
        
        private void LateUpdate()
        {
            ApplyPendingLate();
            
            _isLateUpdating = true;

            if (!IsPaused)
            {
                for (int i = 0; i < _lateUpdatableObjects.Count; i++)
                {
                    var u = _lateUpdatableObjects[i];
                
                    if(CustomTime.GetChannel(u.SelfLateUpdateGroup).IsPaused)
                        continue;

                    u.ManagedLateUpdate();
                }
            }
            
            _isLateUpdating = false;
            
            ApplyPendingLate();
        }

        #endregion
        
        #region ApplyPending

        private void ApplyPending()
        {
            if (_toAdd.Count > 0)
            {
                foreach (var a in _toAdd)
                {
                    if (!_updatableObjects.Contains(a))
                    {
                        _updatableObjects.Add(a);
                    }
                }
                _toAdd.Clear();
                
            }

            if (_toRemove.Count > 0)
            {
                foreach (var r in _toRemove)
                {
                    _updatableObjects.Remove(r);
                }
                
                _toRemove.Clear();
            }
        }

        private void ApplyPendingFixed()
        {
            if (_fixedToAdd.Count > 0)
            {
                foreach (var a in _fixedToAdd)
                {
                    if (!_fixedUpdatableObjects.Contains(a))
                    {
                        _fixedUpdatableObjects.Add(a);
                    }
                }
                
                _fixedToAdd.Clear();
            }

            if (_fixedToRemove.Count > 0)
            {
                foreach (var r in _fixedToRemove)
                {
                    _fixedUpdatableObjects.Remove(r);
                }
                
                _fixedToRemove.Clear();
            }
        }
        
        private void ApplyPendingLate()
        {
            if (_lateToAdd.Count > 0)
            {
                foreach (var a in _lateToAdd)
                {
                    if (!_lateUpdatableObjects.Contains(a))
                    {
                        _lateUpdatableObjects.Add(a);
                    }
                }
                
                _lateToAdd.Clear();
            }

            if (_lateToRemove.Count > 0)
            {
                foreach (var r in _lateToRemove)
                {
                    _lateUpdatableObjects.Remove(r);
                }
                
                _lateToRemove.Clear();
            }
        }

        #endregion

        #region Add/Remove

        private void AddUpdatable(IUpdatable updatable)
        {
            if (_isUpdating)
            {
                if (!_toAdd.Contains(updatable))
                {
                    _toAdd.Add(updatable);
                }
            }
            else if (!_updatableObjects.Contains(updatable))
            {
                _updatableObjects.Add(updatable);
            }
        }

        private void RemoveUpdatable(IUpdatable updatable)
        {
            if (_isUpdating)
            {
                if (!_toRemove.Contains(updatable))
                {
                    _toRemove.Add(updatable);
                }
            }
            else
            { 
                _updatableObjects.Remove(updatable);
            }
        }
        
        private void AddFixedUpdatable(IFixedUpdatable updatable)
        {
            if (_isUpdating)
            {
                if (!_fixedToAdd.Contains(updatable))
                {
                    _fixedToAdd.Add(updatable);
                }
            }
            else if (!_fixedUpdatableObjects.Contains(updatable))
            {
                _fixedUpdatableObjects.Add(updatable);
            }
        }

        private void RemoveFixedUpdatable(IFixedUpdatable updatable)
        {
            if (_isUpdating)
            {
                if (!_fixedToRemove.Contains(updatable))
                {
                    _fixedUpdatableObjects.Add(updatable);
                }
            }
            else
            { 
                _fixedUpdatableObjects.Remove(updatable);
            }
        }
        
        private void AddLateUpdatable(ILateUpdatable updatable)
        {
            if (_isUpdating)
            {
                if (!_lateToAdd.Contains(updatable))
                {
                    _lateToAdd.Add(updatable);
                }
            }
            else if (!_lateUpdatableObjects.Contains(updatable))
            {
                _lateUpdatableObjects.Add(updatable);
            }
        }

        private void RemoveLateUpdatable(ILateUpdatable updatable)
        {
            if (_isUpdating)
            {
                if (!_lateToRemove.Contains(updatable))
                {
                    _lateToRemove.Add(updatable);
                }
            }
            else
            { 
                _lateUpdatableObjects.Remove(updatable);
            }
        }

        #endregion

        #region Register/Unregister
        
        public void Register(IManagedObject element)
        {
            if(element == null) return;
            
            if (element is IUpdatable updatable)
            {
                AddUpdatable(updatable);
            }
            if (element is IFixedUpdatable fixedUpdatable)
            {
                AddFixedUpdatable(fixedUpdatable);
            }
            if (element is ILateUpdatable lateUpdatable)
            {
                AddLateUpdatable(lateUpdatable);
            }
        }

        public void Unregister(IManagedObject element)
        {
            if(element == null) return;
            
            if (element is IUpdatable updatable)
            {
                RemoveUpdatable(updatable);
            }
            if (element is IFixedUpdatable fixedUpdatable)
            {
                RemoveFixedUpdatable(fixedUpdatable);
            }
            if (element is ILateUpdatable lateUpdatable)
            {
                RemoveLateUpdatable(lateUpdatable);
            }  
        }
        
        #endregion
    }
    
    public enum UpdateGroup
    {
        Gameplay,
        UI,
        Inputs,
        Ability,
        Earth,
        Effects,
        Always
    }
}