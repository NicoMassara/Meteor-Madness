using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Main.Scripts.DebugTools
{
    public class DebugUIAbility : MonoBehaviour
    {
        [SerializeField] private Button spawnRandom;
        [SerializeField] private Button addSelected;
        [SerializeField] private TMP_Dropdown dropdownSpawnSelect;
        
        public UnityAction OnRandomSpawned;
        public UnityAction<AbilityType> OnAdded;

        private void OnValidate()   
        {
            SetDropdownValues(dropdownSpawnSelect);
        }

        private void Awake()
        {
            spawnRandom.onClick.AddListener(SpawnRandom);
            addSelected.onClick.AddListener(AddSelected);
        }

        private void SetDropdownValues(TMP_Dropdown dropdown)
        {
            if (dropdown != null)
            {
                dropdown.ClearOptions();
                
                foreach (var ability in Enum.GetValues(typeof(AbilityType)))
                {
                    if(ability is AbilityType.None or AbilityType.Default_MAX) continue;
                    dropdown.options.Add(new TMP_Dropdown.OptionData { text = ability.ToString() });
                }
                
                dropdown.RefreshShownValue();
            }
        }

        #region Spawn

        private void SpawnRandom()
        {
            OnRandomSpawned?.Invoke();
        }
        #endregion
        
        #region Add

        private void AddSelected()
        {
            var abilitySelected = (AbilityType)dropdownSpawnSelect.value;
            OnAdded?.Invoke(abilitySelected);
        }
        
        #endregion
    }
}