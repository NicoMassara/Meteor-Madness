
using UnityEngine;
using UnityEngine.Pool;

namespace _Main.Scripts.Particles
{
    public class GenericPool<T> where T : MonoBehaviour
    {
        private readonly ObjectPool<T> _pool;

        public GenericPool(T prefab, int defaultCapacity = 20, int maxSize = 100)
        {
            _pool = new ObjectPool<T>(
                createFunc: () =>  UnityEngine.Object.Instantiate(prefab),
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: b => UnityEngine.Object.Destroy(b.gameObject),
                collectionCheck: true,
                defaultCapacity: 20,
                maxSize: 500
            );
        }

        private void OnGet(T meteor)
        {
            meteor.gameObject.SetActive(true);
        }

        private void OnRelease(T meteor)
        {
            meteor.gameObject.SetActive(false);
        }

        public T Get()
        {
            return _pool.Get();
        }

        public void Release(T item)
        {
            _pool.Release(item);
        }
    }
}