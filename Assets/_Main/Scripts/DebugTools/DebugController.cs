using _Main.Scripts.Managers;
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
                    GameManager.Instance.EventManager.Publish(new GameModeEvents.Start());
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