﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Main.Scripts.DebugTools
{
    public class DebugUIGameValues : MonoBehaviour
    {
        [Header("Damage")]
        [SerializeField] private TMP_Dropdown damageDropdown;
        [Header("Level")]
        [SerializeField] private Button increaseLevelButton;
        [SerializeField] private Button decreaseLevelButton;
        [SerializeField] private TMP_Text levelText;

        private int _currentLevel = 0;
        
        public UnityAction<DamageParameters.DamageTypes> OnDamageChange;
        public UnityAction<int> OnLevelChange;
        
        private void OnValidate()   
        {
            SetDropdownValues(damageDropdown);
        }

        private void Awake()
        {
            damageDropdown.onValueChanged.AddListener(OnValueChangedHandler);
            increaseLevelButton.onClick.AddListener(() =>
            {
                _currentLevel++;
                _currentLevel = Mathf.Clamp(_currentLevel, 0, 10);
                levelText.text = _currentLevel.ToString();
                OnLevelChange.Invoke(_currentLevel);
            });
            decreaseLevelButton.onClick.AddListener(() =>
            {
                _currentLevel--;
                _currentLevel = Mathf.Clamp(_currentLevel, 0, 10);
                levelText.text = _currentLevel.ToString();
                OnLevelChange.Invoke(_currentLevel);
            });

            _currentLevel = int.Parse(levelText.text);
        }

        private void Start()
        {
            OnLevelChange.Invoke(_currentLevel);
        }

        private void OnValueChangedHandler(int value)
        {
            var temp = (DamageParameters.DamageTypes)value;
            OnDamageChange?.Invoke(temp);
        }

        private void SetDropdownValues(TMP_Dropdown dropdown)
        {
            if (dropdown != null)
            {
                dropdown.ClearOptions();
                
                foreach (var ability in Enum.GetValues(typeof(DamageParameters.DamageTypes)))
                {
                    dropdown.options.Add(new TMP_Dropdown.OptionData { text = ability.ToString() });
                }
                
                dropdown.RefreshShownValue();
            }
        }
    }
}