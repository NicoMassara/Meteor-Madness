using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Utilities;
using UnityEngine;

namespace _Main.Scripts.GameMode
{
    public class GameModeUIAnimationSelector : UiComponentsSelector<GameModeUIAnimationComponents> { }
    
    [Serializable]
    public class GameModeUIAnimationComponents : UiComponentsData,
        GameModeUIAnimationComponents.IGameplayPanel,
        GameModeUIAnimationComponents.ICountdownPanel
    {
        public interface IGameModeUIAnimation : IUiAnimationComponent { }
        
        public interface IGameplayPanel : IGameModeUIAnimation
        {
            public RectTransform GameplayPanel { get; }
            public RectTransform ScorePanel { get; }
            public RectTransform ScoreText { get; }
            public RectTransform StreakText { get; }
            public RectTransform PauseButton { get; }
        }
        
        public interface ICountdownPanel : IGameModeUIAnimation
        {
            public RectTransform CountdownPanel { get; }
            public RectTransform CountdownText { get; }
        }


        [Header("Countdown")]
        [SerializeField] private RectTransform countdownPanel;
        [SerializeField] private RectTransform countdownText;
        [Header("Gameplay")]
        [SerializeField] private RectTransform gameplayPanel;
        [SerializeField] private RectTransform scorePanel;
        [SerializeField] private RectTransform scoreText;
        [SerializeField] private RectTransform streakText;
        [SerializeField] private RectTransform pauseButton;

        // Countdown
        public RectTransform ScorePanel => scorePanel;
        public RectTransform CountdownPanel => countdownPanel;
        
        // Gameplay
        public RectTransform CountdownText => countdownText;
        public RectTransform GameplayPanel => gameplayPanel;
        public RectTransform PauseButton => pauseButton;
        public RectTransform ScoreText => scoreText;
        public RectTransform StreakText => streakText;
    }
}