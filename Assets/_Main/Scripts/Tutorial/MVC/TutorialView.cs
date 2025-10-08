using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Observer;

namespace _Main.Scripts.Tutorial.MVC
{
    public class TutorialView : ManagedBehavior, IObserver
    {
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
                case TutorialObserverMessage.Enable:
                    HandleEnable();
                    break;
                case TutorialObserverMessage.ExtraMeteors:
                    HandleExtraMeteors();
                    break;
            }
        }

        private void HandleEnable()
        {
            OnTutorialEnable?.Invoke();
        }

        private void HandleMovement()
        {
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
            AbilitiesEventCaller.GrantSpawn();
            AbilitiesEventCaller.SetCanUse(true);
        }
        
        private void HandleExtraMeteors()
        {
            GameModeEventCaller.UpdateLevel(9);
            
            for (int i = 0; i < 10; i++)
            {
                MeteorEventCaller.GrantSpawnSingle();
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