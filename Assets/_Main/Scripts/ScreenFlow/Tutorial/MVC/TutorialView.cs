using System;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.Contracts.Interfaces.Analytics;
using MeteorMadness.Contracts.Interfaces.Sounds;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.Managers;
using MeteorMadness.Managers.GameConfig;
using MeteorMadness.ScreenFlow.MultiPage;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Tutorial
{
    public class TutorialView : ManagedBehavior, IObserver, ITutorialSounds, ITutorialAnalytics
    {
        [SerializeField] private MultiPageTextDataSo[] mobileMultiPageData;
        [SerializeField] private MultiPageTextDataSo[] desktopMultiPageData;
        
        private int _currentMultiPageIndex;
        private const float ZoomOutTime = 0.5f;
        
        public event Action OnTutorialEnable;
        public event Action OnTutorialFinished;
        public event Action OnRightMovementFinished;
        public event Action OnLeftMovementFinished;
        
        
        
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                // === Enable / Disable === //
                
                case TutorialObserverMessage.Enable:
                    HandleEnable();
                    break;   
                
                case TutorialObserverMessage.Disable:
                    HandleDisable();
                    break;  
                
                case TutorialObserverMessage.Finish:
                    HandleFinish();
                    break;
                
                // === Multi Page === //
                
                case TutorialObserverMessage.MultiPage:
                    HandleMultiPage();
                    break;
                
                // === Movement === //
                
                case TutorialObserverMessage.RightMovement:
                    HandleRightMovement();
                    break;
                
                case TutorialObserverMessage.LeftMovement:
                    HandleLeftMovement();
                    break;
                
                // === Meteor === //
                
                case TutorialObserverMessage.Meteor:
                    HandleMeteor();
                    break;
                
                case TutorialObserverMessage.ExtraMeteors:
                    HandleExtraMeteors();
                    break;
                
                case TutorialObserverMessage.RightMovementFinished:
                    HandleRightMovementFinished();
                    break;
                
                case TutorialObserverMessage.LeftMovementFinished:
                    HandleLeftMovementFinished();
                    break;
                
                // === Ability === //
                
                case TutorialObserverMessage.AdditionalProjectile:
                    HandleAdditionalProjectile((int)args[0]);
                    break;
                
                case TutorialObserverMessage.SphereDeflected:
                    HandleSphereDeflected();
                    break;
                
                case TutorialObserverMessage.Ability:
                    HandleAbility();
                    break;
            }
        }
        
        #region Movement

        private void HandleLeftMovement()
        {
            CameraEventCaller.ZoomOut(ZoomOutTime);
            GameManager.Instance.CanPlay = true;
            ShieldEventCaller.Enable();
            InputsEventCaller.SetEnable(true);
            
#if UNITY_ANDROID || UNITY_IOS
            
            InputsEventCaller.SetUIEnable(true);
            
#endif
            
        }

        private void HandleRightMovement()
        {
            CameraEventCaller.ZoomOut(ZoomOutTime);
            GameManager.Instance.CanPlay = true;
            ShieldEventCaller.Enable();
            InputsEventCaller.SetEnable(true);
            
#if UNITY_ANDROID || UNITY_IOS
            
            InputsEventCaller.SetUIEnable(true);
            
#endif
        }
        
        private void HandleLeftMovementFinished()
        {
            OnRightMovementFinished?.Invoke();
        }

        private void HandleRightMovementFinished()
        {
            OnLeftMovementFinished?.Invoke();
        }

        #endregion

        #region Enable / Disable

        private void HandleEnable()
        {
            AdsEvents.Banner_TriggerHide();
            _currentMultiPageIndex = 0;
            OnTutorialEnable?.Invoke();
        }
        
        private void HandleDisable()
        {
            GameScreenEventCaller.DisableScreen(ScreenType.Tutorial, EventRequestType.Granted);
        }
        
        private void HandleFinish()
        {
            var hasPlayed = FlagsManager.GetHasPlayed();
            var hasCompletedTutorial = FlagsManager.GetHasCompletedTutorial();
            
            if (hasPlayed == false) FlagsManager.SetHasCompletedTutorial();
            if (hasCompletedTutorial == false) FlagsManager.SetHasPlayed();
            if (hasPlayed == false || hasCompletedTutorial == false) FlagsManager.SaveFlags();
            
            GameManager.Instance.CanPlay = false;
            ProjectileEventCaller.DisableSpawn();
            ProjectileEventCaller.UpdateLevel(0);
            AbilitiesEventCaller.Disable();
            ShieldEventCaller.Disable();
            CameraEventCaller.ZoomIn();
            OnTutorialFinished?.Invoke();
        }

        #endregion

        #region Ability

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
        
        private void HandleSphereDeflected()
        {
            AbilitiesEventCaller.EnableUI();
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

        #endregion
        
        #region Meteor

        private void HandleMeteor()
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
        
                
        private void HandleExtraMeteors()
        {
            /*GameModeEventCaller.UpdateLevel(9);

            for (int i = 0; i < 5; i++)
            {
                MeteorEventCaller.GrantSpawnSingle();
            }*/
        }

        #endregion
        
        #region Multi Page

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
            
            Debug.Log($"Index {_currentMultiPageIndex}, Item: {item}");
            MultiPageUIEventCaller.Create(item, (ulong)_currentMultiPageIndex);
#endif
            
            _currentMultiPageIndex++;
            InputsEventCaller.SetEnable(false);
        }

        #endregion
    }
}