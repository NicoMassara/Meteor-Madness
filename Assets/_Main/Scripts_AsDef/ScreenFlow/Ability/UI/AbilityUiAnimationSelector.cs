using System;
using _Main.Scripts.MyAnimations;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;

namespace MeteorMadness.Gameplay.Abilities
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