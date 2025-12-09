using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Abilities
{
    public class AbilityUiPanelSelector : UiComponentsSelector<AbilityUIComponents> { }
    
    [Serializable]
    public class AbilityUIComponents : UiComponentsData
    {
        [Space(2)] 
        [Header("Panels")]
        public GameObject MainPanel;
        public Image[] AbilitySprites;
    }
}