using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Options
{
    public class OptionsMenuUIPanelSelector : UiPanelSelector<OptionsMenuUIComponents> { }
    
    [Serializable]
    public class OptionsMenuUIComponents : UiComponentsData
    {
        [Header("Buttons Components")]
        public Button MainMenuButton;
        public Button LanguageButton;
    }
}