using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Gameplay.Abilities
{
    public class AbilityUiPanelSelector : UiPanelSelector<AbilityUIComponents> { }
    
    [Serializable]
    public class AbilityUIComponents : UiComponentsData
    {
        [Space(2)] 
        [Header("Panels")]
        public GameObject MainPanel;
        public Image[] AbilitySprites;
    }
}