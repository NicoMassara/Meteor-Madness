using System.Collections.Generic;
using _Main.Scripts.MyComponents;
using _Main.Scripts.MyCustoms;
using UnityEngine;

namespace _Main.Scripts.Managers.UpdateManager
{

    public class UpdatableComponent<T> where T : IBaseUpdatable
    {
        private bool _isUpdating;
        
        private readonly List<T> _running = new List<T>();
        private readonly List<T> _toAdd = new List<T>();
        private readonly List<T> _toRemove = new List<T>();
        
        public bool IsPaused;
        public int RunningCount => _running.Count;
        
        public void UpdateComponents()
        {
            ApplyPending();
            
            _isUpdating = true;

            if (!IsPaused)
            {
                for (int i = 0; i < _running.Count; i++)
                {
                    var u = _running[i];
                
                    if(CustomTime.GetChannel(u.SelfUpdateGroup).IsPaused)
                        continue;
                    
                    u.ExecuteUpdate();
                }
            }
            
            _isUpdating = false;
            
            ApplyPending();
        }

        #region Add/Remove

        public void Add(T updatable)
        {
            if (_isUpdating)
            {
                if (!_toAdd.Contains(updatable))
                {
                    _toAdd.Add(updatable);
                }
            }
            else if (!_running.Contains(updatable))
            {
                _running.Add(updatable);
            }
        }
        
        public void Remove(T updatable)
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
                _running.Remove(updatable);
            }
        }
        
        private void ApplyPending()
        {
            if (_toAdd.Count > 0)
            {
                foreach (var a in _toAdd)
                {
                    if (!_running.Contains(a))
                    {
                        _running.Add(a);
                    }
                }
                _toAdd.Clear();
            }

            if (_toRemove.Count > 0)
            {
                foreach (var r in _toRemove)
                {
                    _running.Remove(r);
                }
                
                _toRemove.Clear();
            }
        }

        #endregion
    }
    

    public class UpdateManager : SingletonBehaviour<UpdateManager>
    {
        private readonly UpdatableComponent<IUpdatable> _updatableComponent = new UpdatableComponent<IUpdatable>();
        private readonly UpdatableComponent<IFixedUpdatable> _fixedUpdatableComponent = new UpdatableComponent<IFixedUpdatable>();
        private readonly UpdatableComponent<ILateUpdatable> _lateUpdatableComponent = new UpdatableComponent<ILateUpdatable>();
        
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD

        private UpdateManagerDebugData _debugData;
        
#endif
        public bool IsGlobalPaused { get; set; }
        
        private void Awake()
        {
            Application.targetFrameRate = 120;
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData = new UpdateManagerDebugData();
        
#endif
        }

        #region Update

        private void Update()
        {
            CustomTime.UpdateAll(IsGlobalPaused ? 0 : Time.unscaledDeltaTime);
            IsGlobalPaused = _updatableComponent.IsPaused;
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.UpdateFps(Time.deltaTime);
            _debugData.ManagedUpdateCount = _updatableComponent.RunningCount;
#endif
            
            _updatableComponent.UpdateComponents();
        }

        private void FixedUpdate()
        {
            CustomTime.FixedUpdateAll(IsGlobalPaused ? 0 : Time.fixedUnscaledDeltaTime);
            IsGlobalPaused = _fixedUpdatableComponent.IsPaused;
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.ManagedFixedUpdateCount = _fixedUpdatableComponent.RunningCount;
#endif
            _fixedUpdatableComponent.UpdateComponents();
        }
        
        
        private void LateUpdate()
        {
            IsGlobalPaused = _lateUpdatableComponent.IsPaused;
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.ManagedLateUpdateCount = _lateUpdatableComponent.RunningCount;
#endif
            _lateUpdatableComponent.UpdateComponents();
        }

        #endregion

        #region Register/Unregister
        
        public void Register(IManagedObject element)
        {
            if(element == null) return;
            
            if (element is IUpdatable updatable)
            {
                _updatableComponent.Add(updatable);
            }
            if (element is IFixedUpdatable fixedUpdatable)
            {
                _fixedUpdatableComponent.Add(fixedUpdatable);
            }
            if (element is ILateUpdatable lateUpdatable)
            {
                _lateUpdatableComponent.Add(lateUpdatable);
            }
        }

        public void Unregister(IManagedObject element)
        {
            if(element == null) return;
            
            if (element is IUpdatable updatable)
            {
                _updatableComponent.Remove(updatable);
            }
            if (element is IFixedUpdatable fixedUpdatable)
            {
                _fixedUpdatableComponent.Remove(fixedUpdatable);
            }
            if (element is ILateUpdatable lateUpdatable)
            {
                _lateUpdatableComponent.Remove(lateUpdatable);
            }  
        }
        
        #endregion
    }
    
    public enum UpdateGroup
    {
        Always,
        Gameplay,
        UI,
        Inputs,
        Ability,
        Earth,
        Effects,
        Shield,
        Camera
    }

    public enum TickGroup
    {
        EveryFrame,
        HalfTarget,
        QuarterTarget,
        EightTarget,
        EverySecond,
        DEFAULT_MAX
    }
}