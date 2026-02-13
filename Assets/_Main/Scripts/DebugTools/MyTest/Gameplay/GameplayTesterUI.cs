using System;
using TMPro;
using UnityEngine;

namespace MeteorMadness.Debug._Main.Scripts.Debug.MyTest.Gameplay
{
    public class GameplayTesterUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text levelText;

        private void Awake()
        {
            var view = GetComponent<IGameplayTester>();
            view.OnLevelUpdated += UpdateLevel;
        }

        private void UpdateLevel(int input)
        {
            levelText.text = $"Level: {input}";
        }
    }
}