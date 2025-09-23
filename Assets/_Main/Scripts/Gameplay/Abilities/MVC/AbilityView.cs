using System.Collections;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityView : ManagedBehavior, IUpdatable, IObserver
    {
        private readonly Timer _abilityTimer = new Timer();
        private AbilityData _currentAbility;
        private AbilityController _controller;
        private AbilityDataController abilityDataController;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Ability;

        private void Start()
        {
            abilityDataController = new AbilityDataController(GameManager.Instance.EventManager, AbilitiesData_UpdateTimeScale);
            abilityDataController.OnAbilityStarted += AbilitiesData_OnAbilityStartedHandler;
        }

        public void ManagedUpdate()
        {

        }

        public void OnNotify(string message, params object[] args)
        {
            switch (message)
            {
                case AbilityObserverMessage.SelectAbility:
                    HandleSelectAbility((AbilityType)args[0]);
                    break;
                case AbilityObserverMessage.TriggerAbility:
                    HandleTriggerAbility((AbilityType)args[0]);
                    break;
                case AbilityObserverMessage.FinishAbility:
                    HandleFinishAbility((AbilityType)args[0]);
                    break;
            }
        }

        public void SetController(AbilityController controller)
        {
            _controller = controller;
        }

        #region Ability

        private void HandleSelectAbility(AbilityType enumType)
        {
            if (!abilityDataController.HasAbilityData(enumType))
            {
                Debug.LogWarning("AbilityData Does not exist");
                return;
            }
            
            _controller.TransitionToRunning();
        }

        private void HandleTriggerAbility(AbilityType enumType)
        {
            StartCoroutine(Coroutine_RunAbilityQueue(abilityDataController.GetAbilityStartQueue(enumType)));
        }

        private void HandleFinishAbility(AbilityType enumType)
        {
            StartCoroutine(Coroutine_RunAbilityQueue(abilityDataController.GetAbilityEndQueue(enumType)));
        }

        #endregion

        #region Coroutine

        private IEnumerator Coroutine_RunAbilityQueue(ActionQueue actionQueue)
        {
            while (!actionQueue.IsEmpty)
            {
                actionQueue.Run(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));

                yield return null;
            }
        }
        
        private IEnumerator Coroutine_RunAbilityTimer()
        {
            while (!_abilityTimer.GetHasEnded)
            {
                _abilityTimer.Run(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));

                yield return null;
            }
        }

        private IEnumerator Coroutine_UpdateTimeScale(TimeScaleData timeScaleData)
        {
            var currentTimeScale = timeScaleData.CurrentTimeScale;
            var duration = timeScaleData.TimeToUpdate;
            float elapsedTime = 0;
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float timeRatio = Mathf.Clamp01(elapsedTime / duration);
                currentTimeScale = Mathf.Lerp(currentTimeScale, timeScaleData.TargetTimeScale, timeRatio);
                
                foreach (var updateGroup in timeScaleData.UpdateGroups)
                {
                    CustomTime.SetChannelTimeScale(updateGroup, currentTimeScale);
                }
                
                yield return null;
            }
        }

        #endregion

        #region Handler

        private void AbilitiesData_OnAbilityStartedHandler(float activeTime)
        {
            _abilityTimer.Set(activeTime);
            _abilityTimer.OnEnd += AbilityTimer_OnEndHandler;
            StartCoroutine(Coroutine_RunAbilityTimer());
        }
        
        private void AbilityTimer_OnEndHandler()
        {
            _abilityTimer.OnEnd -= AbilityTimer_OnEndHandler;
            _controller.TransitionToEnable();
        }

        private void AbilitiesData_UpdateTimeScale(TimeScaleData timeScaleData)
        {
            StartCoroutine(Coroutine_UpdateTimeScale(timeScaleData));
        }

        #endregion
    }
}