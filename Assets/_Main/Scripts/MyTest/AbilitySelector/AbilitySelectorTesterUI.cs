using UnityEngine;

namespace _Main.Scripts.MyTest.AbilitySelector
{
#if UNITY_EDITOR
    public class AbilitySelectorTesterUI : MonoBehaviour
    {
        private AbilitySelectorTester.IAbilitySelectorTester _tester;
        
        private AbilityType _lastAbility;
        private AbilityType _currentAbility;
        private int _currentLevel;
        private GUIStyle _style;
        
        private void Awake()
        {
            _tester = GetComponent<AbilitySelectorTester.IAbilitySelectorTester>();
        }

        private void Start()
        {
            _tester.OnAbilitySelected += AbilitySelected;
            _tester.OnLevelUpdated += OnLevelUpdated;
        }

        private void OnLevelUpdated(int level)
        {
            _currentLevel = level;
        }

        private void AbilitySelected(AbilityType ability)
        {
            _lastAbility = _currentAbility;
            _currentAbility = ability;
        }
        
        void OnGUI()
        {
            // Inicializar estilo (OnGUI se llama muchas veces)
            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.label);
                _style.fontSize = 36;
                _style.normal.textColor = Color.white;
            }

            int x = 20;
            int y = 20;
            int width = 600;
            int height = 50;
            int spacing = 60;

            GUI.Label(new Rect(x, y, width, height), $"Current Level: {_currentLevel}", _style);
            GUI.Label(new Rect(x, y + spacing, width, height), $"Current Ability: {_currentAbility}", _style);
            GUI.Label(new Rect(x, y + spacing * 2, width, height), $"Last Ability: {_lastAbility}", _style);
        }
    }
    
#endif
}