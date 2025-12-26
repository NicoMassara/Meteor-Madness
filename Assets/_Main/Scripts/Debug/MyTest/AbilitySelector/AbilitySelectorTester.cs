using System;
using MeteorMadness.Gameplay.Abilities.So;
using MeteorMadness.Gameplay.Abilities.Spawn;
using MeteorMadness.GlobalValues;
using UnityEditor;
using UnityEngine;

namespace _Main.Scripts.MyTest.AbilitySelector
{
    
    public class AbilitySelectorTester : MonoBehaviour,
        AbilitySelectorTester.IAbilitySelectorTester
    {
        public interface IAbilitySelectorTester
        {
#if UNITY_EDITOR
            public event Action<AbilityType> OnAbilitySelected;
            public event Action<int> OnLevelUpdated;
#endif
        }
        
#if UNITY_EDITOR
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
#endif
    }
    

#if UNITY_EDITOR
    
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
    
#endif
}