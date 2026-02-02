
using System.Collections.Generic;
using MeteorMadness.Contracts.Interfaces;
using UnityEngine;
using UnityEngine.Pool;

namespace MeteorMadness.GlobalValues.Tools
{
    public class GenericPool<T> where T : MonoBehaviour
    {
        private readonly ObjectPool<T> _pool;
        private readonly List<T> _active = new List<T>();
        private readonly string _itemName;
        
        public GenericPool(T prefab, int defaultCapacity = 20, int maxSize = 100, string itemName = "Pool Object")
        {
            _pool = new ObjectPool<T>(
                createFunc: () =>  CreateObject(prefab),
                actionOnGet: OnGet, 
                actionOnRelease: OnRelease,
                actionOnDestroy: b => UnityEngine.Object.Destroy(b.gameObject),
                collectionCheck: true,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
            
            _itemName = itemName;

            for (int i = 0; i < defaultCapacity; i++)
            { 
                Get();
            }
            
            RecycleAll();
        }

        private T CreateObject(T prefab)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            var item = UnityEngine.Object.Instantiate(prefab);
            item.name = $"{_itemName} - {_active.Count+1}";
            return item;
#else
            return UnityEngine.Object.Instantiate(prefab);
#endif
        }

        private void OnGet(T meteor)
        {
            meteor.gameObject.SetActive(true);
            _active.Add(meteor);
        }

        private void OnRelease(T meteor)
        {
            meteor.gameObject.SetActive(false);
            meteor.transform.position = new Vector2(150f,150f);
            _active.Remove(meteor);
        }

        public T Get()
        {
            return _pool.Get();
        }

        public void Release(T item)
        {
            if(!_active.Contains(item)) return;
            
            _pool.Release(item);
        }

        public void RecycleAll()
        {
            int activeCount = _active.Count;

            for (int i = activeCount - 1; i >= 0; i--)
            {
                var item = _active[i];
                _pool.Release(item);
            }
        }
    }
}