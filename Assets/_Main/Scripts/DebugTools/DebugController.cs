using _Main.Scripts.Managers;
using _Main.Scripts.MyCustoms;
using UnityEngine;

namespace _Main.Scripts.DebugTools
{
    [RequireComponent(typeof(DebugUIView))]
    public class DebugController : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        private DebugUIView _viewUI;
        private bool _isPanelActive;
        private bool _isShieldActive;

        private void Awake()
        {
            _viewUI = GetComponent<DebugUIView>();
            
            _viewUI.Initialize();
            
            // Ability
            _viewUI.Ability.OnAdded = (abilityType) =>
            {
                AbilitiesEventCaller.Add(new AbilityAddData{AbilityType = abilityType});
            };
            _viewUI.Ability.OnRandomSpawned = AbilitiesEventCaller.GrantSpawn;
            
            //Meteor
            _viewUI.Meteor.OnSingleSpawned = MeteorEventCaller.GrantSpawnSingle;
            _viewUI.Meteor.OnRingSpawned = MeteorEventCaller.SpawnRing;
            
            //Game Values
            _viewUI.GameValues.OnDamageChange = (damageType)=>
            {
                GameConfigManager.Instance.SetDamage(damageType);
            };
            _viewUI.GameValues.OnLevelChange = GameModeEventCaller.UpdateLevel;
            
            //Camera
            _viewUI.Camera.OnZoomIn = CameraEventCaller.ZoomIn;
            _viewUI.Camera.OnZoomOut = CameraEventCaller.ZoomOut;
            
            //Time Scale
            _viewUI.TimeScale.OnChangeScale += (timeScale) =>
            {
                CustomTime.GlobalTimeScale = timeScale;
                CustomTime.GlobalFixedTimeScale = timeScale;
                /*Time.timeScale = timeScale;
                Time.fixedDeltaTime = timeScale;*/
            };
            
        }

        private void Start()
        {
            //Default Values
            TimerManager.Add(new TimerData
            {
                Time = 1f,
                OnEndAction = () =>
                {
                    GameManager.Instance.CanPlay = true;
                    GameModeEventCaller.Start();
                    InputsEventCaller.SetEnable(true);
                    ShieldEventCaller.SetEnableShield(true);
                }
            });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                _isPanelActive = !_isPanelActive;
                panel.SetActive(_isPanelActive);
            }
        }
    }
}