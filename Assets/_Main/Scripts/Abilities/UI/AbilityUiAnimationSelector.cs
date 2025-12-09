using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Utilities;
using UnityEngine;

namespace _Main.Scripts.Abilities
{
    public class AbilityUiAnimationSelector : UiComponentsSelector<AbilityUIAnimationComponents> { }
    
    [Serializable]
    public class AbilityUIAnimationComponents : UiComponentsData,
        AbilityUIAnimationComponents.IMainPanel
    {
        public interface IMainPanel : IUiAnimationComponent
        {
            public RectTransform MainPanel { get; }
        }
        
        [Header("Main Panel")]
        [SerializeField] private RectTransform mainPanel;

        public RectTransform MainPanel => mainPanel;
    }
}