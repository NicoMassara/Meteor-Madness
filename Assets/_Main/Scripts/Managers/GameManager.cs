using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.ScriptableObjects;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class GameManager : ManagedBehavior
    {
        [SerializeField] private GameplayConfigSo gameplayConfigData;
        [SerializeField] private DamageTypes currentDamageDamageType = DamageTypes.Standard;
        public static GameManager Instance =>  _instance != null ? _instance : (_instance = CreateInstance());
        private static GameManager _instance;
        
        
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
            currentDamageDamageType = DamageTypes.Standard;
            EventManager = new EventBusManager();
        }

        public void LoadGameplay()
        {
            Instance.EventManager.Publish(new GameScreenEvents.SetGameScreen{Index = 1});
        }

        public void LoadMainMenu()
        {
            Instance.EventManager.Publish(new GameScreenEvents.SetGameScreen{Index = 0});
        }

        #region Damage

        public float GetMeteorDamage()
        {
            return currentDamageDamageType switch
            {
                DamageTypes.None => DamageParameters.Values.NoneDamage,
                DamageTypes.Standard => DamageParameters.Values.StandardMeteor,
                DamageTypes.Hard => DamageParameters.Values.HardMeteor,
                DamageTypes.Heavy => DamageParameters.Values.HeavyMeteor,
                DamageTypes.Brutal => DamageParameters.Values.BrutalMeteor,
                _ => DamageParameters.Values.StandardMeteor
            };
        }

        public void SetMeteorDamage(DamageTypes damage)
        {
            currentDamageDamageType = damage;
        }
        
        #endregion
    }
}