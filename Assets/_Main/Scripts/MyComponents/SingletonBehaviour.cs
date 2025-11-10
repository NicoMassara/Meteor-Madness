using UnityEngine;

namespace _Main.Scripts.MyComponents
{
    public class SingletonBehaviour<T> : MonoBehaviour
    where T : SingletonBehaviour<T>
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
    }
}