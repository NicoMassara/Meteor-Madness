using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class GameManager : ManagedBehavior
    {
        [SerializeField] private DamageTypes currentDamageType = DamageTypes.Brutal;
        public static GameManager Instance =>  _instance != null ? _instance : (_instance = CreateInstance());
        private static GameManager _instance;
        
        private SceneController _sceneController;
        
        public bool CanPlay { get; set; }
        public bool IsPaused { get; set; }
        private int _currentPoints;
        
        public EventBusManager EventManager { get; private set; }
        
        private static GameManager CreateInstance()
        {
            var gameObject = new GameObject(nameof(GameManager))
            {
                hideFlags = HideFlags.DontSave,
            };
            DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<GameManager>();
        }

        private void Awake()
        {
            _sceneController = new SceneController();
            currentDamageType = DamageTypes.Standard;
            EventManager = new EventBusManager();
        }

        public void LoadGameplay()
        {
            Instance.EventManager.Publish(new SetGameScreen{Index = 1});
        }

        public void LoadMainMenu()
        {
            Instance.EventManager.Publish(new SetGameScreen{Index = 0});
        }

        #region Damage

        public float GetMeteorDamage()
        {
            return currentDamageType switch
            {
                DamageTypes.None => DamageValues.NoneDamage,
                DamageTypes.Standard => DamageValues.StandardMeteor,
                DamageTypes.Hard => DamageValues.HardMeteor,
                DamageTypes.Heavy => DamageValues.HeavyMeteor,
                DamageTypes.Brutal => DamageValues.BrutalMeteor,
                _ => DamageValues.StandardMeteor
            };
        }

        #endregion
    }
}