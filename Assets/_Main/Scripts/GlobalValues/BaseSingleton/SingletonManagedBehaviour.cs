using MeteorMadness.Contracts.Events;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.GlobalValues.BaseSingleton
{
    public class SingletonManagedBehaviour<T> : ManagedBehavior
    where T : SingletonManagedBehaviour<T>
    {
        public static T Instance =>  _instance != null ? _instance : (_instance = CreateInstance());
        protected static T _instance;
        
        private static T CreateInstance()
        {
            var gameObject = new GameObject(typeof(T).ToString())
            {
                hideFlags = HideFlags.DontSave,
            };
            DontDestroyOnLoad(gameObject);
            SingletonEvents.OnDestroySingleton += () => DestroyImmediate(gameObject);
            return gameObject.AddComponent<T>();
        }

        protected virtual void Awake()
        {
            _instance = this as T;
        }
        
        public static void LoadInstance() => Instance.Internal_LoadInstance();

        private void Internal_LoadInstance(){ }
    }
}