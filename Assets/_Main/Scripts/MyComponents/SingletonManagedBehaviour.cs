using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.MyComponents
{
    public class SingletonManagedBehaviour<T> : ManagedBehavior
    where T : SingletonManagedBehaviour<T>
    {
        public static T Instance =>  _instance != null ? _instance : (_instance = CreateInstance());
        protected static T _instance;
        
        private static T CreateInstance()
        {
            var gameObject = new GameObject(nameof(T))
            {
                hideFlags = HideFlags.DontSave,
            };
            DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<T>();
        }

        protected virtual void Awake()
        {
            _instance = this as T;
        }
    }
}