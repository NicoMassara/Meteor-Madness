using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class GameManager : ManagedBehavior
    {
        [SerializeField] private DamageParameters.DamageTypes currentDamageDamageType = DamageParameters.DamageTypes.Standard;
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
            currentDamageDamageType = DamageParameters.DamageTypes.Standard;
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
                DamageParameters.DamageTypes.None => DamageParameters.Values.NoneDamage,
                DamageParameters.DamageTypes.Standard => DamageParameters.Values.StandardMeteor,
                DamageParameters.DamageTypes.Hard => DamageParameters.Values.HardMeteor,
                DamageParameters.DamageTypes.Heavy => DamageParameters.Values.HeavyMeteor,
                DamageParameters.DamageTypes.Brutal => DamageParameters.Values.BrutalMeteor,
                _ => DamageParameters.Values.StandardMeteor
            };
        }

        public void SetMeteorDamage(DamageParameters.DamageTypes damage)
        {
            currentDamageDamageType = damage;
        }
        
        #endregion
    }
}