using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Pause
{
    public class PauseUiAnimationSelector : UiComponentsSelector<PauseUIAnimationComponents> { }
    
    [Serializable]
    public class PauseUIAnimationComponents : UiComponentsData,
        PauseUIAnimationComponents.IMainPanel
    {
        public interface IMainPanel : IUiAnimationComponent
        {
            public RectTransform MainPanel { get; }
            public RectTransform TitleText { get; }
            public RectTransform LeftButtonsPanel { get; }
            public RectTransform PointsText { get; }
            public Image BackgroundImage { get; }
        }
        
        [Header("Countdown")]
        [SerializeField] private RectTransform mainPanel;
        [SerializeField] private RectTransform titleText;
        [SerializeField] private RectTransform leftButtonsPanel;
        [SerializeField] private RectTransform pointsText;
        [SerializeField] private Image backgroundImage;
        
        public RectTransform MainPanel => mainPanel;
        public RectTransform TitleText => titleText;
        public RectTransform LeftButtonsPanel => leftButtonsPanel;
        public RectTransform PointsText => pointsText;
        public Image BackgroundImage => backgroundImage;
    }
}