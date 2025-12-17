using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.GameStats
{
    public class StatsUiSelector : UiComponentsSelector<StatsUiComponents> { }
    
    [Serializable]
    public class StatsUiComponents : UiComponentsData
    {
        [Header("Text Components")] 
        public TMP_Text DeflectAmountText;
        public TMP_Text CollisionAmountText;
        public TMP_Text AbilityUseAmountText;
        [Space]
        [Header("Buttons")]
        public Button BackButton;
    }
}