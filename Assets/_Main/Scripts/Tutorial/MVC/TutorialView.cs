using System;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using _Main.Scripts.ScriptableObjects;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Tutorial.MVC
{
    public class TutorialView : ManagedBehavior, IObserver, ITutorialSounds
    {
        [SerializeField] private MultiPageTextDataSo[] mobileMultiPageData;
        [SerializeField] private MultiPageTextDataSo[] desktopMultiPageData;
        private int _currentMultiPageIndex;
        private const float ZoomOutTime = 0.5f;
        
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
            AbilitiesEventCaller.EnableUI();
        }
        
        private void HandleMultiPage()
        {
            ShieldEventCaller.Disable();
            CameraEventCaller.ZoomIn();
            
#if UNITY_ANDROID || UNITY_IOS
            
            InputsEventCaller.SetUIEnable(false);
                        var item = mobileMultiPageData[_currentMultiPageIndex];
            MultiPageUIEventCaller.Create(item, (ulong)_currentMultiPageIndex);
#else
            var item = desktopMultiPageData[_currentMultiPageIndex];
            MultiPageUIEventCaller.Create(item, (ulong)_currentMultiPageIndex);
#endif
            
            _currentMultiPageIndex++;
            InputsEventCaller.SetEnable(false);
        }

        private void HandleMovement()
        {
            CameraEventCaller.ZoomOut(ZoomOutTime);
            ProjectileEventCaller.UpdateLevel(0);
            GameManager.Instance.CanPlay = true;
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
            CameraEventCaller.ZoomOut(ZoomOutTime);
            AbilitiesEventCaller.SetNextSpawn(AbilityType.SuperShield);
            AbilitiesEventCaller.Enable();
            AbilitiesEventCaller.DisableUI();
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