using System;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;
using UnityEngine.Events;
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
        [Header("Buttons")] 
        [SerializeField] private Button triggerButton;

        public void AddListenerToTriggerButton(UnityAction action) 
            => AddListenerToButton(triggerButton, action);
        public void RemoveListenerToTriggerButton(UnityAction action) 
            => RemoveListenerFromButton(triggerButton, action);
    }
}