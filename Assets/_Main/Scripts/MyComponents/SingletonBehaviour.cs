using System;
using UnityEngine;

namespace _Main.Scripts.MyComponents
{
    public abstract class SingletonBehaviour<T> : MonoBehaviour
    where T : SingletonBehaviour<T>
    {
        public static T Instance =>  _instance != null ? _instance : (_instance = CreateInstance());
        protected static T _instance;
        
        private static T CreateInstance()
        {
            var gameObject = new GameObject(typeof(T).ToString())
            {
                hideFlags = HideFlags.DontSave,
            };
            //Debug.Log($"Singleton Created: {typeof(T)}");
            DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<T>();
        }

        public static void LoadInstance() => Instance.Internal_LoadInstance();

        private void Internal_LoadInstance(){ }
    }
}