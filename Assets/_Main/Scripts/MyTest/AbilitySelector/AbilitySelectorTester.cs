using System;
using _Main.Scripts.Abilities.So;
using _Main.Scripts.Abilities.Spawn;
using UnityEditor;
using UnityEngine;

namespace _Main.Scripts.MyTest.AbilitySelector
{
    public class AbilitySelectorTester : MonoBehaviour,
        AbilitySelectorTester.IAbilitySelectorTester
    {
        public interface IAbilitySelectorTester
        {
            public event Action<AbilityType> OnAbilitySelected;
            public event Action<int> OnLevelUpdated;
        }
        
        [SerializeField] private AbilitySelectorDataSo selectorData;
        
        private AbilitySpawner.AbilitySelector _selector;
        private int _currentLevel;
        
        public event Action<AbilityType> OnAbilitySelected;
        public event Action<int> OnLevelUpdated;

        private void Start()
        {
            _selector = new AbilitySpawner.AbilitySelector(selectorData.GetRarityValues,selectorData.GetUnlockLevelValues);
        }

        public void ReleaseAbility()
        {
            var ability = _selector.GetAbilityToAdd();
            
            OnAbilitySelected?.Invoke(ability);
        }

        public void IncreaseLevel()
        {
            _currentLevel++;
            _selector.UpdateLevel(_currentLevel);
            OnLevelUpdated?.Invoke(_currentLevel);
        }

        public void DecreaseLevel()
        {
            _currentLevel--;
            _selector.UpdateLevel(_currentLevel);
            OnLevelUpdated?.Invoke(_currentLevel);
        }
    }
    
    [CustomEditor(typeof(AbilitySelectorTester))]
    public class AbilitySelectorTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Dibuja el inspector normal
            DrawDefaultInspector();

            // Agrega el botón
            AbilitySelectorTester script = (AbilitySelectorTester)target;
            
            if (GUILayout.Button("Release Ability")) script.ReleaseAbility();
            if (GUILayout.Button("Increase Level")) script.IncreaseLevel();
            if (GUILayout.Button("Decrease Level")) script.DecreaseLevel();
        }
    }
}