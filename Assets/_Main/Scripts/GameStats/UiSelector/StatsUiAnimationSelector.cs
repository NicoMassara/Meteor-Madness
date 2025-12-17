using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Utilities;
using UnityEngine;

namespace _Main.Scripts.GameStats
{
    public class StatsUiAnimationSelector : UiComponentsSelector<StatsUiAnimationComponents> { }
    
    [Serializable]
    public class StatsUiAnimationComponents : UiComponentsData,
        StatsUiAnimationComponents.IMainPanel
    {
        public interface IMainPanel : IUiAnimationComponent
        {
            public RectTransform MainPanel { get; }
            public RectTransform TitleText { get; }
            public RectTransform DeflectAmountText { get; }
            public RectTransform CollisionAmountText { get; }
            public RectTransform AbilityUseAmountText { get; }
            public RectTransform BackButton { get; }
        }

        [Space(2)]
        [Header("Main Panel")]
        [SerializeField] private RectTransform mainPanel;
        [SerializeField] private RectTransform titleText;
        [SerializeField] private RectTransform deflectAmountText;
        [SerializeField] private RectTransform collisionAmountText;
        [SerializeField] private RectTransform abilityUseAmountText;
        [SerializeField] private RectTransform backButton;
        
        public RectTransform MainPanel => mainPanel;
        public RectTransform TitleText => titleText;
        public RectTransform DeflectAmountText => deflectAmountText;
        public RectTransform CollisionAmountText => collisionAmountText;
        public RectTransform AbilityUseAmountText => abilityUseAmountText;
        public RectTransform BackButton => backButton;
    }
}