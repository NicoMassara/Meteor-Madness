using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.ScriptableObjects;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class GameConfigManager : ManagedBehavior
    {
        [SerializeField] private GameplayConfigSo gameplayConfigSo;
        
        public static GameConfigManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public GameplayConfigSo GetGameplayData()
        {
            return gameplayConfigSo;
        }
    }
}