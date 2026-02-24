using System;
using System.Collections;
using _Main.Scripts.Common.MyRandom;
using _Main.Scripts.EventBus;
using _Main.Scripts.GameCamera;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.GlobalValues.Events;
using MeteorMadness.Managers;
using MeteorMadness.Managers.GameConfig;
using MeteorMadness.Managers.Localization;
using MeteorMadness.Managers.Save;
using NicolasMassara.CustomUpdateManager;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MeteorMadness.Debug._Main.Scripts.Debug.MyTest.GameplayAbility
{
    public class GameplayAbilityTester : MonoBehaviour
    {
        [Header("Start Values")]
        [Range(0, LevelAmount-1)] [SerializeField] private int startLevel = 1;
        [SerializeField] private bool startMeteorsEnable;
        [SerializeField] private bool doesIncreaseLevel;
        [Space(5)]
        [SerializeField] private TimeScales timeScale;
        [SerializeField] private CameraTransportDataSo gameplayZoom;

        [Serializable]
        private class TimeScales
        {
            public bool isEnable;
            [Range(0,1)]
            public float globalTimeScale;
            [Range(0,1)]
            public float gameplayTimeScale;
            [Range(0,1)]
            public float shieldTimeScale;
            [Range(0,1)]
            public float effectsTimeScale;
        }

        private const int LevelAmount = GameParameters.GameplayValues.SpawnLevelAmount;
        
        private int _currentLevel;

        internal bool MeteorActive;
        internal bool AbilityActive;
        public event Action<int> OnLevelUpdated;
        
        
        private void Awake()
        {
            RandomService.Initialize();
            ProjectileSpawner.Subscribe.BatchDeflected(EventBus_Projectile_BatchDeflected);
            ProjectileSpawner.Subscribe.ProjectileReachedTarget(EventBus_Projectile_ProjectileReachedTarget);
            ProjectileSpawner.Subscribe.BatchDeflected(EventBus_Projectile_BatchDeflected);
            AbilitiesEventSubscriber.NotifyIsActive(EventBus_Ability_NotifyIsActive);
        }

        private void Start()
        {
            _currentLevel = startLevel;
            OnLevelUpdated?.Invoke(_currentLevel+1);
            Initialize();
            
        }

        private void Update()
        {
            if(timeScale.isEnable == false) return;
            
            CustomTime.GlobalTimeScale = timeScale.globalTimeScale;
            CustomTime.GlobalFixedTimeScale = timeScale.globalTimeScale;
            CustomTime.SetChannelTimeScale(UpdateGroup.Gameplay, timeScale.gameplayTimeScale);
            CustomTime.SetChannelTimeScale(UpdateGroup.Shield, timeScale.shieldTimeScale);
            CustomTime.SetChannelTimeScale(UpdateGroup.Effects, timeScale.effectsTimeScale);
        }

        #region Initializer
        
        private void Initialize()
        {
            CameraEventCaller.Transport(gameplayZoom);
            StartCoroutine(Coroutine_Initialize());
        }

        private void InitializeGameplay()
        {
            GameConfigManager.Instance.SetDamage(DamageTypes.None);
            GameManager.Instance.CanPlay = true;
            ShieldEventCaller.Enable();
            AbilitiesEventCaller.Enable();
            AbilitiesEventCaller.SetCanUse(true);
            EarthEventCaller.EnableDamage();
            InputsEventCaller.SetEnable(true);
            InputsEventCaller.SetUIEnable(true);
            //ProjectileEventCaller.UpdateLevel(_currentLevel);
            if (startMeteorsEnable)
            {
                ToggleMeteorSpawn();
            }
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

        #region Meteor

        public void ForceSpawn()
        {
            ProjectileSpawner.Publish.Enable();
            ProjectileSpawner.Publish.RequestSpawn(BatchType.Default);
            ProjectileSpawner.Publish.Disable();
        }
        
        public void ToggleMeteorSpawn()
        {
            MeteorActive = !MeteorActive;
            if (MeteorActive)
            {
                ProjectileSpawner.Publish.Enable();
                ProjectileSpawner.Publish.RequestSpawn(BatchType.Default);
            }
            else
                ProjectileSpawner.Publish.Disable();
        }
        
        public void ToggleAbilitySpawn()
        {
            AbilityActive = !AbilityActive;
            ProjectileSpawner.Publish.SetEnableAbilitySpawn(AbilityActive);
        }

        public void SimulateDeflect()
        {
            ProjectileEventCaller.Deflected(new DeflectData()
            {
                Direction = GetRandomDirection()
            });
        }
        
        public void SimulateCollision()
        {
            ProjectileEventCaller.Collision(new CollisionData
            {
                Direction = GetRandomDirection()
            });
        }

        #endregion

        private Vector2 GetRandomDirection()
        {
            return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        }

        #region Level

        public void IncreaseLevel()
        {
            _currentLevel++;
            _currentLevel = Mathf.Clamp(_currentLevel, 0, int.MaxValue);
            ProjectileSpawner.Publish.SetLevel(_currentLevel);
            OnLevelUpdated?.Invoke(_currentLevel+1);
        }

        public void DecreaseLevel()
        {
            _currentLevel--;
            _currentLevel = Mathf.Clamp(_currentLevel, 0, int.MaxValue);
            ProjectileSpawner.Publish.SetLevel(_currentLevel);
            OnLevelUpdated?.Invoke(_currentLevel+1);
        }

        public void UpdateLevel()
        {
            _currentLevel = Mathf.Clamp(_currentLevel, 0, LevelAmount-1);
            ProjectileSpawner.Publish.SetLevel(_currentLevel);
            OnLevelUpdated?.Invoke(_currentLevel);
        }

        #endregion

        #region Abilities

        public void AddAbility(AbilityType abilityType)
        {
            AbilitiesEventCaller.Add(new AbilityAddData
            {
                AbilityType = abilityType
            });
        }

        #endregion
        
        private void EventBus_Projectile_ProjectileReachedTarget(ProjectileSpawnerEvents.ProjectileReachedTarget input)
        {
            if (input.IsLastFromBatch)
            {
                ProjectileSpawner.Publish.RequestSpawn(BatchType.Default);
            }
        }

        private void EventBus_Projectile_BatchDeflected(ProjectileSpawnerEvents.BatchDeflected input)
        {
            if(doesIncreaseLevel)
                IncreaseLevel();
        }

        private void EventBus_Ability_NotifyIsActive(AbilitiesEvents.NotifyIsActive input)
        {
            if (input.IsActive)
            {
                ProjectileSpawner.Unsubscribe.BatchDeflected(EventBus_Projectile_BatchDeflected);
                ProjectileSpawner.Unsubscribe.ProjectileReachedTarget(EventBus_Projectile_ProjectileReachedTarget);
            }
            else
            {
                ProjectileSpawner.Subscribe.BatchDeflected(EventBus_Projectile_BatchDeflected);
                ProjectileSpawner.Subscribe.ProjectileReachedTarget(EventBus_Projectile_ProjectileReachedTarget);
            }
        }
    }
    
#if UNITY_EDITOR
    
    [CustomEditor(typeof(GameplayAbilityTester))]
    public class GameplayTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GameplayAbilityTester script = (GameplayAbilityTester)target;
            
            GUILayout.Space(10f);
            GUILayout.Label("Level");
            if (GUILayout.Button("Increase Level")) script.IncreaseLevel();
            if (GUILayout.Button("Decrease Level")) script.DecreaseLevel();
            if (GUILayout.Button("Toggle Meteor Spawn")) script.ToggleMeteorSpawn();
            if (GUILayout.Button("Toggle Ability Spawn")) script.ToggleAbilitySpawn();
            if (GUILayout.Button("Force Spawn")) script.ForceSpawn();
            
            GUILayout.Space(10f);
            if (GUILayout.Button("Simulate Deflect")) script.SimulateDeflect();
            if (GUILayout.Button("Simulate Collision")) script.SimulateCollision();
            
            GUILayout.Space(10f);
            GUILayout.Label("Abilities");
            if (GUILayout.Button("Add Super Shield")) script.AddAbility(AbilityType.SuperShield);
            if (GUILayout.Button("Add Health")) script.AddAbility(AbilityType.Health);
            if (GUILayout.Button("Add SlowMotion")) script.AddAbility(AbilityType.SlowMotion);
            if (GUILayout.Button("Add DoublePoints")) script.AddAbility(AbilityType.DoublePoints);
            if (GUILayout.Button("Add Automatic")) script.AddAbility(AbilityType.Automatic);
            
            EditorUtility.SetDirty(target);
        }
    }
#endif
}