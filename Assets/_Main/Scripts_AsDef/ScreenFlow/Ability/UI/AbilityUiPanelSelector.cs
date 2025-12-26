using System;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;
using UnityEngine.UI;

namespace MeteorMadness.Gameplay.Abilities
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