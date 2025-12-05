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
        DefeatUiAnimationComponents.IHighScore
    {
        public interface IMainPanel : IUiAnimationComponent
        {
            public Image BackgroundImage { get; }
            public RectTransform MainPanel { get; }
            public RectTransform Score { get; }
            public RectTransform HighScore { get; }
            public RectTransform ButtonsPanel { get; }
        }
        
        public interface IScore : IUiAnimationComponent
        {
            public RectTransform Score { get; }
        }
        
        public interface IHighScore : IUiAnimationComponent
        {
            public RectTransform HighScore { get; }
        }
        
        public interface IButtons
        {
            
        }
        
        
        [Header("Rect Transform")]
        [SerializeField] private RectTransform mainPanel;
        [SerializeField] private RectTransform scoreText;
        [SerializeField] private RectTransform highScoreText;
        [SerializeField] private RectTransform buttonsPanel;
        [Header("Images")]
        [SerializeField] private Image backgroundImage;

        public Image BackgroundImage => backgroundImage;
        public RectTransform MainPanel => mainPanel;
        public RectTransform Score => scoreText;
        public RectTransform HighScore => highScoreText;
        public RectTransform ButtonsPanel => buttonsPanel;
        
        // Shared
        
        // IMainPanel
        
        // IScore
        
        // IHighScore

        // IButtons
    }
}