using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Tutorial.MVC
{
    public class TutorialView : ManagedBehavior, IObserver
    {
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case TutorialObserverMessage.Start:
                    HandleStart();
                    break;
                case TutorialObserverMessage.Movement:
                    HandleMovement();
                    break;
                case TutorialObserverMessage.Ability:
                    HandleAbility();
                    break;
                case TutorialObserverMessage.Finish:
                    HandleFinish();
                    break;
            }
        }
        
        private void HandleStart()
        {

        }

        private void HandleMovement()
        {
            GameEventCaller.Publish(new CameraEvents.ZoomOut());
            GameManager.Instance.CanPlay = true;
            ShieldEventCaller.SetEnableShield(true);
            InputsEventCaller.SetEnable(true);
            
            for (int i = 0; i < 1; i++)
            {
                MeteorEventCaller.GrantSpawnSingle();
            }
        }
        
        private void HandleAbility()
        {
            AbilitiesEventCaller.GrantSpawn();
            
            for (int i = 0; i < 5; i++)
            {
                MeteorEventCaller.GrantSpawnSingle();
            }
        }
        
        private void HandleFinish()
        {
            InputsEventCaller.SetEnable(false);
            GameManager.Instance.CanPlay = false;
            ShieldEventCaller.SetEnableShield(false);
            GameEventCaller.Publish(new CameraEvents.ZoomIn());
        }
    }
}