using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Observer;
using _Main.Scripts.ScriptableObjects;
using UnityEngine;

namespace _Main.Scripts.Tutorial.MVC
{
    public class TutorialView : ManagedBehavior, IObserver
    {
        [SerializeField] private MultiPageTextDataSo[] multiPageData;
        private int _currentMultiPageIndex;
        
        public event Action OnTutorialEnable;
        
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case TutorialObserverMessage.Movement:
                    HandleMovement();
                    break;
                case TutorialObserverMessage.Ability:
                    HandleAbility();
                    break;
                case TutorialObserverMessage.Finish:
                    HandleFinish();
                    break;
                case TutorialObserverMessage.ExtraMeteors:
                    HandleExtraMeteors();
                    break;
                case TutorialObserverMessage.AdditionalProjectile:
                    HandleAdditionalProjectile((int)args[0]);
                    break;
                case TutorialObserverMessage.MultiPage:
                    HandleMultiPage();
                    break;
                case TutorialObserverMessage.Enable:
                    HandleEnable();
                    break;   
            }
        }
        
        private void HandleMultiPage()
        {
            CameraEventCaller.ZoomIn();
            var item = multiPageData[_currentMultiPageIndex];
            MultiPageUIEventCaller.Create(item, (ulong)_currentMultiPageIndex);
            _currentMultiPageIndex++;
            InputsEventCaller.SetEnable(false);
        }

        private void HandleEnable()
        {
            _currentMultiPageIndex = 0;
            OnTutorialEnable?.Invoke();
        }

        private void HandleMovement()
        {
            CameraEventCaller.ZoomOut();
            GameModeEventCaller.UpdateLevel(0);
            GameManager.Instance.CanPlay = true;
            CameraEventCaller.ZoomOut();
            ShieldEventCaller.SetEnableShield(true);
            InputsEventCaller.SetEnable(true);
            GameConfigManager.Instance.SetDamage(DamageTypes.None);
            
            for (int i = 0; i < 1; i++)
            {
                MeteorEventCaller.GrantSpawnSingle();
            }
        }
        
        private void HandleAbility()
        {
            CameraEventCaller.ZoomOut();
            AbilitiesEventCaller.SetNextSpawn(AbilityType.SuperShield);
            AbilitiesEventCaller.SetCanUse(true);
            InputsEventCaller.SetEnable(true);
            AbilitiesEventCaller.GrantSpawn();
        }
        
        private void HandleExtraMeteors()
        {
            /*GameModeEventCaller.UpdateLevel(9);
            
            for (int i = 0; i < 5; i++)
            {
                MeteorEventCaller.GrantSpawnSingle();
            }*/
        }
        
        private void HandleAdditionalProjectile(int index)
        {
            var tempType = (ProjectileType)index;

            if (tempType == ProjectileType.Meteor)
            {
                AbilitiesEventCaller.SetNextSpawn(AbilityType.SuperShield);
                MeteorEventCaller.GrantSpawnSingle();
            }
            else if (tempType == ProjectileType.AbilitySphere)
            {
                AbilitiesEventCaller.GrantSpawn();
            }
        }
        
        private void HandleFinish()
        {
            MeteorEventCaller.RecycleAll();
            GameModeEventCaller.UpdateLevel(0);
            AbilitiesEventCaller.SetCanUse(false);
            GameManager.Instance.CanPlay = false;
            GameModeEventCaller.Finish();
            ShieldEventCaller.SetEnableShield(false);
            CameraEventCaller.ZoomIn();
        }
    }
}