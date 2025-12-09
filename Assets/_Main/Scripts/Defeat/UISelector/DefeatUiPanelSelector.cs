using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Defeat
{
    [AddComponentMenu("_Main/Defeat/UI Selector")]
    public class DefeatUiPanelSelector : UiComponentsSelector<DefeatUIComponents> { }
    
    [Serializable]
    public class DefeatUIComponents : UiComponentsData
    {
        [Header("Buttons Components")]
        public Button RestartButton;
        public Button MainMenuButtons;
    }
    
}