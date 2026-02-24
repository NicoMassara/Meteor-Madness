using System;
using _Main.Scripts.Common.MyRandom;
using _Main.Scripts.Common.SelectorByWeight;
using MeteorMadness.Contracts;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Abilities;
using MeteorMadness.Gameplay.Abilities.So;
using UnityEditor;
using UnityEngine;

namespace MeteorMadness.Debug._Main.Scripts.DebugTools.MyTest.Ability
{
    public class AbilitySelectorTester : MonoBehaviour
    {
        [SerializeField] private AbilitySelectorDataSo dataSo;

        private RandomSelector<AbilityType, IAbilityWeightsData> _randomSelector;

        private void Start()
        {
            RandomService.Initialize();
            _randomSelector = new RandomSelector<AbilityType, IAbilityWeightsData>(5);
        }

        public void SendAbility()
        {
            var temp = _randomSelector.GetRandomItem(dataSo.GetBaseWeights(), dataSo.GetWeights());
            UnityEngine.Debug.Log(temp);
        }

        public void ClearHistory()
        {
            _randomSelector.ClearHistory();
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(AbilitySelectorTester))]
    public class AbilitySelectorTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            AbilitySelectorTester script = (AbilitySelectorTester)target;
            if (GUILayout.Button("Send Ability")) script.SendAbility();
            if (GUILayout.Button("Clear History")) script.ClearHistory();
        }
    }
    
#endif
}