using System;
using System.Collections;
using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.Debug._Main.Scripts.Debug.MyTest.Cosmetics;
using MeteorMadness.GlobalValues.Events;
using MeteorMadness.Managers;
using MeteorMadness.Managers.GameConfig;
using MeteorMadness.Managers.Localization;
using MeteorMadness.Managers.Save;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEditor;
using UnityEngine;

namespace MeteorMadness.Debug._Main.Scripts.Debug.MyTest.Gameplay
{
    public interface IGameplayTester
    {
        public event Action<int> OnLevelUpdated;
    }

    public class GameplayTester : MonoBehaviour, IGameplayTester
    {
        [SerializeField] private DamageTypes damageType;
        [Range(0,9)]
        [SerializeField] private int startLevel = 1;

        private int _currentLevel;
        public event Action<int> OnLevelUpdated;
        
        private void Awake()
        {
            ProjectileEventSubscriber.RequestSpawn(EventBus_Projectile_RequestSpawn);
        }

        private void Start()
        {
            _currentLevel = startLevel;
            OnLevelUpdated?.Invoke(_currentLevel);
            Initialize();
        }
        
        #region Initializer
        
        private void Initialize()
        {
            StartCoroutine(Coroutine_Initialize());
        }

        private void InitializeGameplay()
        {
            GameConfigManager.Instance.SetDamage(damageType);
            GameManager.Instance.CanPlay = true;
            ShieldEventCaller.Enable();
            EarthEventCaller.EnableDamage();
            InputsEventCaller.SetEnable(true);
            CameraEventCaller.ZoomOut();
            ProjectileEventCaller.EnableSpawn();
            ProjectileEventCaller.UpdateLevel(_currentLevel);
        }

        private IEnumerator Coroutine_Initialize()
        {
            var currentMainSystems = 0;
            var currentSubSystems = 0;
            var hasLoadedData = false;
            var hasLoadedLocalization = false;
            var hasLoadedSounds = false;

            BootEvents.OnMainSystemInitialized += () => currentMainSystems++;
            BootEvents.OnSubSystemInitialized += () => currentSubSystems++;
            SaveDataEvents.OnSaveInitialized += () => hasLoadedData = true;
            LocalizationEvents.OnLocalizationLoaded += () => hasLoadedLocalization = true;
            SoundEvents.OnSoundManagerInitialized += () => hasLoadedSounds = true;
            
            DataManager.LoadInstance();
            LocalizationManager.LoadInstance();
            GameConfigManager.LoadInstance();
            SoundManager.LoadInstance();
            
            yield return new WaitForSeconds(0.1f);
            
            yield return new WaitUntil(()=> hasLoadedData);
            yield return new WaitUntil(()=> hasLoadedLocalization);
            
            SoundEvents.InitializeSoundManager();
            
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(()=> hasLoadedSounds);
            
            BootEvents.InitializeMainSystem();
            
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(()=> currentMainSystems >= 1);
            
            BootEvents.InitializeSubSystems();
            
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(()=> currentSubSystems >= 1);
            
            
            BootEvents.TriggerOnGameLoaded();
            yield return new WaitForEndOfFrame();
            
            InitializeGameplay();
            
            yield return null;
        }
        
        #endregion

        private void GrantProjectile()
        {
            ProjectileEventCaller.GrantSpawn(ProjectileType.Meteor);
        }

        public void IncreaseLevel()
        {
            _currentLevel++;
            _currentLevel = Mathf.Clamp(_currentLevel, 0, 9);
            ProjectileEventCaller.UpdateLevel(_currentLevel);
            OnLevelUpdated?.Invoke(_currentLevel);
        }

        public void DecreaseLevel()
        {
            _currentLevel--;
            _currentLevel = Mathf.Clamp(_currentLevel, 0, 9);
            ProjectileEventCaller.UpdateLevel(_currentLevel);
            OnLevelUpdated?.Invoke(_currentLevel);
        }

        public void HealEarth()
        {
            EarthEventCaller.Heal();
        }

        #region Event Bus

        private void EventBus_Projectile_RequestSpawn(ProjectileEvents.RequestSpawn input)
        {
            if (input.RequestType == EventRequestType.Requested)
            {
                GrantProjectile();
            }
        }

        #endregion
    }

#if UNITY_EDITOR
    
    [CustomEditor(typeof(GameplayTester))]
    public class GameplayTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GameplayTester script = (GameplayTester)target;
            if (GUILayout.Button("Increase Level")) script.IncreaseLevel();
            if (GUILayout.Button("Decrease Level")) script.DecreaseLevel();
            if (GUILayout.Button("Heal Earth")) script.HealEarth();
        }
    }
#endif
}