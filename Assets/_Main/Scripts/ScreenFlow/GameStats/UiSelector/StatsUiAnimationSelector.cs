using System;
using MeteorMadness.ScreenFlow.Base;
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
            public RectTransform[] StatsTextsArray { get; }
            public RectTransform BackButton { get; }
        }

        [Space(2)]
        [Header("Main Panel")]
        [SerializeField] private RectTransform mainPanel;
        [SerializeField] private RectTransform titleText;
        [SerializeField] private RectTransform[] statsTextsArray;
        [SerializeField] private RectTransform backButton;
        
        public RectTransform MainPanel => mainPanel;
        public RectTransform TitleText => titleText;
        public RectTransform[] StatsTextsArray => statsTextsArray;
        public RectTransform BackButton => backButton;
    }
}