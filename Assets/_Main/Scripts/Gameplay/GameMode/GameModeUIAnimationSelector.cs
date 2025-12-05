using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeUIAnimationSelector : UiComponentsSelector<GameModeUIAnimationComponents> { }
    
    [Serializable]
    public class GameModeUIAnimationComponents : UiComponentsData,
        GameModeUIAnimationComponents.IGameplayPanel,
        GameModeUIAnimationComponents.ICountdownPanel
    {
        public interface IGameplayPanel : IUiAnimationComponent
        {
            public RectTransform GameplayPanel { get; }
            public RectTransform ScoreText { get; }
            public Button PauseButton { get; }
        }
        
        public interface ICountdownPanel : IUiAnimationComponent
        {
            public RectTransform CountdownPanel { get; }
            public RectTransform CountdownText { get; }
        }

        [Header("Countdown")]
        [SerializeField] private RectTransform countdownPanel;
        [SerializeField] private RectTransform countdownText;
        [Header("Gameplay")]
        [SerializeField] private RectTransform gameplayPanel;
        [SerializeField] private RectTransform scoreText;
        [SerializeField] private Button pauseButton;

        // Countdown
        public RectTransform ScoreText => scoreText;
        public RectTransform CountdownPanel => countdownPanel;
        
        // Gameplay
        public RectTransform CountdownText => countdownText;
        public RectTransform GameplayPanel => gameplayPanel;
        public Button PauseButton => pauseButton;
    }
}