using System;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;
using UnityEngine.UI;

namespace MeteorMadness.ScreenFlow.Defeat
{
    public class DefeatUiAnimationSelector : UiComponentsSelector<DefeatUiAnimationComponents> { }
    
    [Serializable]
    public class DefeatUiAnimationComponents : UiComponentsData,
        DefeatUiAnimationComponents.IScore,
        DefeatUiAnimationComponents.IMainPanel,
        DefeatUiAnimationComponents.IHighScore,
        DefeatUiAnimationComponents.ICoins,
        DefeatUiAnimationComponents.IButtons
    {
        public interface IMainPanel : IUiAnimationComponent
        {
            public Image BackgroundImage { get; }
            public RectTransform MainPanel { get; }
            public RectTransform Title { get; }
            public RectTransform ScorePanel { get; }
            public RectTransform ScoreText { get; }
            public RectTransform CoinsPanel { get; }
            public RectTransform ButtonsPanel { get; }
            public RectTransform SubHighScoreText { get;}
        }
        
        public interface IScore : IUiAnimationComponent
        {
            public RectTransform ScorePanel { get; }
            public RectTransform ScoreText { get; }
        }
        
        public interface IHighScore : IUiAnimationComponent
        {
            public RectTransform SubHighScoreText { get; }
        }
        
        public interface ICoins : IUiAnimationComponent
        {
            public RectTransform CoinsPanel { get; }
        }
    
        public interface IButtons : IUiAnimationComponent
        {
            public RectTransform ButtonsPanel { get; }
        }
        
        
        [Header("Rect Transform")]
        [SerializeField] private RectTransform mainPanel;
        [SerializeField] private RectTransform titleText;
        [SerializeField] private RectTransform scoreText;
        [SerializeField] private RectTransform scorePanel;
        [SerializeField] private RectTransform subHighScoreText;
        [SerializeField] private RectTransform coinsPanel;
        [SerializeField] private RectTransform buttonsPanel;
        [Header("Images")]
        [SerializeField] private Image backgroundImage;

        public Image BackgroundImage => backgroundImage;
        public RectTransform MainPanel => mainPanel;
        public RectTransform Title => titleText;
        public RectTransform ScoreText => scoreText;
        public RectTransform ScorePanel => scorePanel;
        public RectTransform SubHighScoreText => subHighScoreText;
        public RectTransform CoinsPanel => coinsPanel;
        public RectTransform ButtonsPanel => buttonsPanel;
        
    }
}