
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace _Main.Scripts
{
    public class GenericPool<T> where T : MonoBehaviour
    {
        private readonly ObjectPool<T> _pool;
        private readonly List<T> _active = new List<T>();

        public GenericPool(T prefab, int defaultCapacity = 20, int maxSize = 100)
        {
            _pool = new ObjectPool<T>(
                createFunc: () =>  UnityEngine.Object.Instantiate(prefab),
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: b => UnityEngine.Object.Destroy(b.gameObject),
                collectionCheck: true,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );

            for (int i = 0; i < defaultCapacity; i++)
            { 
                Get();
            }
            
            RecycleAll();
        }

        private void OnGet(T meteor)
        {
            meteor.gameObject.SetActive(true);
            _active.Add(meteor);
        }

        private void OnRelease(T meteor)
        {
            meteor.gameObject.SetActive(false);
            _active.Remove(meteor);
        }

        public T Get()
        {
            return _pool.Get();
        }

        public void Release(T item)
        {
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