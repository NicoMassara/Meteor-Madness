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
        public event Action OnTutorialFinished;
        
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
                case TutorialObserverMessage.Disable:
                    HandleDisable();
                    break;  
                case TutorialObserverMessage.SphereDeflected:
                    HandleSphereDeflected();
                    break;
            }
        }
        
        private void HandleEnable()
        {
            _currentMultiPageIndex = 0;
            OnTutorialEnable?.Invoke();
        }
        
        private void HandleDisable()
        {
            GameScreenEventCaller.DisableScreen(ScreenType.Tutorial, EventRequestType.Granted);
        }

        private void HandleSphereDeflected()
        {
            AbilitiesEventCaller.SetEnableUI(true);
        }
        
        private void HandleMultiPage()
        {
#if UNITY_ANDROID || UNITY_IOS
            
            InputsEventCaller.SetUIEnable(false);
                    
#endif
            ShieldEventCaller.Disable();
            CameraEventCaller.ZoomIn();
            var item = multiPageData[_currentMultiPageIndex];
            MultiPageUIEventCaller.Create(item, (ulong)_currentMultiPageIndex);
            _currentMultiPageIndex++;
            InputsEventCaller.SetEnable(false);
        }

        private void HandleMovement()
        {
            CameraEventCaller.ZoomOut();
            ProjectileEventCaller.UpdateLevel(0);
            GameManager.Instance.CanPlay = true;
            CameraEventCaller.ZoomOut();
            ShieldEventCaller.Enable();
            InputsEventCaller.SetEnable(true);
#if UNITY_ANDROID || UNITY_IOS
            
            InputsEventCaller.SetUIEnable(true);
            
#endif
            GameConfigManager.Instance.SetDamage(DamageTypes.None);
            
            for (int i = 0; i < 1; i++)
            {
                MeteorEventCaller.GrantSpawnSingle();
            }
        }
        
        private void HandleAbility()
        {
            ShieldEventCaller.Enable();
            CameraEventCaller.ZoomOut();
            AbilitiesEventCaller.SetNextSpawn(AbilityType.SuperShield);
            AbilitiesEventCaller.Enable();
            AbilitiesEventCaller.SetEnableUI(false);
            InputsEventCaller.SetEnable(true);
#if UNITY_ANDROID || UNITY_IOS
            
            InputsEventCaller.SetUIEnable(true);
                    
#endif
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
                MeteorEventCaller.GrantSpawnSingle();
            }
            else if (tempType == ProjectileType.AbilitySphere)
            {
                AbilitiesEventCaller.SetNextSpawn(AbilityType.SuperShield);
                AbilitiesEventCaller.GrantSpawn();
            }
        }
        
        private void HandleFinish()
        {
            GameManager.Instance.CanPlay = false;
            ProjectileEventCaller.DisableSpawn();
            ProjectileEventCaller.UpdateLevel(0);
            AbilitiesEventCaller.Disable();
            ShieldEventCaller.Disable();
            CameraEventCaller.ZoomIn();
            OnTutorialFinished?.Invoke();
        }
    }
}