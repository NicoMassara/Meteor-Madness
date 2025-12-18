using System;
using System.Collections.Generic;
using UnityEngine;

namespace Plugins.NicolasMassara.CustomSoundManager
{
    public interface IPoolable<out T>
    {
        public event Action<T> OnRelease;
        
        public void OnGetFromPool();
        public void OnReleasedFromPool();
        public void OnDestroyFromPool();
        public void ReleaseFromPool();
    }
    
    public class GenericPool<T> where T : IPoolable<T>
    {
        private readonly List<T> _available;
        private readonly List<T> _inUse = new List<T>();
        private readonly Func<T> _createFunc;
        private readonly int _maxCapacity;

        public int InUseCount { get; private set; }
        public int AvailableCount { get; private set; }
        public int AllCount => InUseCount + AvailableCount;
        
        public string PoolName { get; set; }

        public GenericPool(Func<T> createFunc, 
            int defaultCapacity = 10, 
            int defaultCreated = 0,
            int maxCapacity = 1000)
        {
            _createFunc = createFunc;
            _available = new List<T>(defaultCapacity);

            if (defaultCreated > 0)
                for (int i = 0; i < defaultCreated; i++)
                {
                    var item = createFunc();
                    _available.Add(item);
                    item.OnReleasedFromPool();
                    AvailableCount++;
                }

            _maxCapacity = maxCapacity;
        }

        #region Public Methods

        public T Get() => Internal_Get();

        public void Release(T item) => Internal_Release(item);

        public void RecycleAll() => Internal_RecycleAll();
        public bool HasElement(T element) => Internal_HasElement(element);

        #region Destroy

        public void DestroyAll()
        {
            DestroyInUse();
            DestroyAvailable();
        }

        public void DestroyInUse() => Internal_DestroyInUse();

        public void DestroyAvailable() => Internal_DestroyAvailable();

        #endregion

        #endregion

        #region Internal

        private bool Internal_HasElement(T element)
        {
            return _available.Contains(element) || _inUse.Contains(element);
        }

        private T Internal_Get()
        {
            T item;

            if (_available.Count > 1)
            {
                var index = GetLastAvailableIndex();
                item = _available[index];
                 _available.RemoveAt(index);
            }
            else
            {
                item = _createFunc();
            }

            OnGet(item);
            return item;
        }

        private void Internal_Release(T item)
        {
            if (_available.Contains(item))
            {
                Debug.LogWarning($"This item has been already released from the {PoolName} pool.");
                return;
            }

            OnRelease(item);
            
            if (AvailableCount < _maxCapacity)
            {
                _available.Add(item);
            }
            else
            {
                Debug.LogWarning($"Item destroyed from {PoolName} pool, max amount limit reached!");
                OnDestroy(item);
            }
        }
        
        private void Internal_RecycleAll()
        {
            for (int i = _inUse.Count - 1; i >= 0; i--)
            {
                var item = _inUse[i];
                item.ReleaseFromPool();
            }
        }

        #region Destroy
        
        private void Internal_DestroyInUse()
        {
            for (int i = _inUse.Count - 1; i >= 0; i--) _inUse[i].OnDestroyFromPool();
        }
        
        private void Internal_DestroyAvailable()
        {
            for (int i = _available.Count - 1; i >= 0; i--) _available[i].OnDestroyFromPool();
        }
        
        #endregion
        
        #endregion

        #region Actions

        private void OnGet(T item)
        {
            item.OnRelease += Release;
            AvailableCount--;
            InUseCount++;
            _available.Remove(item);
            _inUse.Add(item);
            item.OnGetFromPool();
        }
        

        private void OnRelease(T item)
        {
            item.OnRelease -= Release;
            item.OnReleasedFromPool();
            _inUse.Remove(item);
            _available.Add(item);
            InUseCount--;
            AvailableCount++;
        }

        private void OnDestroy(T item)
        {
            item.OnDestroyFromPool();
        }
        
        #endregion
        
        private int GetLastAvailableIndex()
        {
            return _available.Count - 1;
        }
        
        public void Dispose() => this.DestroyAll();
    }
}