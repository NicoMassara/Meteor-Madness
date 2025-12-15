using System;
using IngameDebugConsole;
using UnityEngine;

namespace _Main.Scripts.MyCommands
{
    public class CommandSpawner : MonoBehaviour
    {
        [SerializeField] private DebugLogManager debugLogManager;
        
        private void Awake()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            Instantiate(debugLogManager);
            
#endif
        }
    }
}