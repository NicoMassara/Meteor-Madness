using System;
using System.Collections;
using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.Debug._Main.Scripts.Debug.MyTest.Gameplay;
using MeteorMadness.GlobalValues.Events;
using MeteorMadness.Managers;
using MeteorMadness.Managers.GameConfig;
using MeteorMadness.Managers.Localization;
using MeteorMadness.Managers.Save;
using NicolasMassara.CustomUpdateManager;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEditor;
using UnityEngine;

namespace MeteorMadness.Debug._Main.Scripts.Debug.MyTest.GameplayAbility
{
    public class GameplayAbilityTester : MonoBehaviour, IGameplayTester
    {
        [Header("Start Values")]
        [Range(0, 9)] [SerializeField] private int startLevel = 1;
        [SerializeField] private bool startMeteorsEnable;
        [Space(5)]
        [SerializeField] private TimeScales timeScale;

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

        private int _currentLevel;

        internal bool MeteorActive;
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
            CameraEventCaller.ZoomOut();
            ProjectileEventCaller.UpdateLevel(_currentLevel);
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

        public void ToggleMeteorSpawn()
        {
            MeteorActive = !MeteorActive;
            if(MeteorActive)
                ProjectileEventCaller.EnableSpawn();
            else
                ProjectileEventCaller.DisableSpawn();
        }

        #endregion

        #region Level

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

        public void UpdateLevel(int level)
        {
            _currentLevel--;
            _currentLevel = Mathf.Clamp(_currentLevel, 0, 9);
            ProjectileEventCaller.UpdateLevel(_currentLevel);
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
        
        #region Event Bus

        private void EventBus_Projectile_RequestSpawn(ProjectileEvents.RequestSpawn input)
        {
            if (input.RequestType == EventRequestType.Requested)
            {
                ProjectileEventCaller.GrantSpawn(ProjectileType.Meteor);
            }
        }

        #endregion
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