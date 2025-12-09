using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Defeat
{
    [AddComponentMenu("_Main/Defeat/UI Selector")]
    public class DefeatUiAnimationSelector : UiComponentsSelector<DefeatUiAnimationComponents> { }
    
    [Serializable]
    public class DefeatUiAnimationComponents : UiComponentsData,
        DefeatUiAnimationComponents.IScore,
        DefeatUiAnimationComponents.IMainPanel,
        DefeatUiAnimationComponents.IHighScore,
        DefeatUiAnimationComponents.IButtons
    {
        public interface IMainPanel : IUiAnimationComponent
        {
            public Image BackgroundImage { get; }
            public RectTransform MainPanel { get; }
            public RectTransform Title { get; }
            public RectTransform Score { get; }
            public RectTransform HighScorePanel { get; }
            public RectTransform HighScoreText { get; }
            public RectTransform ButtonsPanel { get; }
            public RectTransform SubHighScoreText { get;}
        }
        
        public interface IScore : IUiAnimationComponent
        {
            public RectTransform Score { get; }
        }
        
        public interface IHighScore : IUiAnimationComponent
        {
            public RectTransform HighScorePanel { get; }
            public RectTransform HighScoreText { get; }
            public RectTransform SubHighScoreText { get; }
        }
        
        public interface IButtons : IUiAnimationComponent
        {
            public RectTransform ButtonsPanel { get; }
        }
        
        
        [Header("Rect Transform")]
        [SerializeField] private RectTransform mainPanel;
        [SerializeField] private RectTransform titleText;
        [SerializeField] private RectTransform scoreText;
        [SerializeField] private RectTransform highScoreText;
        [SerializeField] private RectTransform highScorePanel;
        [SerializeField] private RectTransform subHighScoreText;
        [SerializeField] private RectTransform buttonsPanel;
        [Header("Images")]
        [SerializeField] private Image backgroundImage;

        public Image BackgroundImage => backgroundImage;
        public RectTransform MainPanel => mainPanel;
        public RectTransform Title => titleText;
        public RectTransform Score => scoreText;
        public RectTransform HighScorePanel => highScorePanel;
        public RectTransform HighScoreText => highScoreText;
        public RectTransform SubHighScoreText => subHighScoreText;
        public RectTransform ButtonsPanel => buttonsPanel;
        
        // Shared
        
        // IMainPanel
        
        // IScore
        
        // IHighScore

        // IButtons
    }
}