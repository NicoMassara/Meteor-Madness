using System;
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
            _viewUI.Ability.OnAdded = AbilityEventCaller.AddAbility;
            _viewUI.Ability.OnRandomSpawned = AbilityEventCaller.SpawnRandom;
            
            //Meteor
            _viewUI.Meteor.OnSingleSpawned = MeteorEventCaller.SpawnSingle;
            _viewUI.Meteor.OnRingSpawned = MeteorEventCaller.SpawnRandom;
            
            //Game Values
            _viewUI.GameValues.OnDamageChange = GameValuesEventCaller.SetDamageType;
            _viewUI.GameValues.OnLevelChange = GameValuesEventCaller.UpdateLevel;
            
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
                    GameManager.Instance.EventManager.Publish(new InputsEvents.SetEnable{IsEnable = true});
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