using System;
using MeteorMadness.Contracts.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace MeteorMadness.UI._Main.Scripts.UI
{
    public class AbilityUIButton : MonoBehaviour
    {
        private static readonly int AbilityCount = Shader.PropertyToID("_AbilityCount");
        private static readonly int AbilityInUse = Shader.PropertyToID("_AbilityInUse");
        
        [SerializeField] private Image buttonImage;
        
        private IAbilityButtonUpdater _buttonUpdater;
        private int _abilityCount;

        private void Awake()
        {
            _buttonUpdater = GetComponent<IAbilityButtonUpdater>();
        }

        private void Start()
        {
            _buttonUpdater.OnAbilityAdded += OnAbilityAddedHandler;
            _buttonUpdater.OnAbilityTriggered += OnAbilityTriggeredHandler;
            _buttonUpdater.OnAbilityFinished += OnAbilityFinishedHandler;

            UpdateAbilityCount(0);
            UpdateIsAbilityInUse(false);
        }
        
        private void UpdateAbilityCount(int abilityCount)
        {
            buttonImage.material.SetFloat(AbilityCount, abilityCount);
        }

        private void UpdateIsAbilityInUse(bool isAbilityInUse)
        {
            buttonImage.material.SetFloat(AbilityInUse, isAbilityInUse ? 1 : 0);
        }

        #region Handlers
        
        private void OnAbilityAddedHandler()
        {
            _abilityCount++;
            _abilityCount = Mathf.Clamp(_abilityCount, 0, int.MaxValue);
            UpdateAbilityCount(_abilityCount);
        }
        
        private void OnAbilityTriggeredHandler()
        {
            UpdateIsAbilityInUse(true);
            _abilityCount--;
            _abilityCount = Mathf.Clamp(_abilityCount, 0, int.MaxValue);
            UpdateAbilityCount(_abilityCount);
        }

        private void OnAbilityFinishedHandler()
        {
            UpdateIsAbilityInUse(false);
        }

        #endregion

    }
}