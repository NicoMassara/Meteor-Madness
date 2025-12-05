using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Utilities;
using TMPro;
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
            public RectTransform ScorePanel { get; }
            public RectTransform PauseButton { get; }
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
        [SerializeField] private RectTransform scorePanel;
        [SerializeField] private RectTransform pauseButton;

        // Countdown
        public RectTransform ScorePanel => scorePanel;
        public RectTransform CountdownPanel => countdownPanel;
        
        // Gameplay
        public RectTransform CountdownText => countdownText;
        public RectTransform GameplayPanel => gameplayPanel;
        public RectTransform PauseButton => pauseButton;
    }
}