using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Utilities;
using TMPro;
using UnityEngine;

namespace _Main.Scripts.Menu
{
    public class MainMenuUiAnimationSelector : UiComponentsSelector<MainMenuUiAnimationComponents> { }

    [Serializable]
    public class MainMenuUiAnimationComponents : UiComponentsData,
        MainMenuUiAnimationComponents.IMainMenuPanel,
        MainMenuUiAnimationComponents.ILorePanel,
        MainMenuUiAnimationComponents.ITutorialPanel,
        MainMenuUiAnimationComponents.ICreditsPanel
    {
        public interface IMainMenuPanel : IUiAnimationComponent
        {
            public RectTransform MenuPanel { get; }
            public RectTransform LeftButtonsPanel { get; }
            public RectTransform RightButtonsPanel { get; }
            public RectTransform QuitButton { get; }
            public TMP_Text GameTitle { get; }
        }
        
        public interface ILorePanel : IUiAnimationComponent
        {
            public RectTransform LorePanel { get; }
        }
        
        public interface ITutorialPanel : IUiAnimationComponent
        {
            public RectTransform TutorialPanel { get; }
        }
        
        public interface ICreditsPanel : IUiAnimationComponent
        {
            public RectTransform CreditsPanel { get; }
        }
        
        
        [Space(2)]
        [Header("Main Menu Panel")]
        [SerializeField] private RectTransform menuPanel;
        [Header("Game Objects")]
        [SerializeField] private RectTransform leftButtonsPanel;
        [SerializeField] private RectTransform rightButtonsPanel;
        [SerializeField] private RectTransform quitButton;
        [Space(1)]
        [Header("Texts")]
        [SerializeField] private TMP_Text gameTitle;
        
        [Space(2)]
        [Header("Lore Panel")]
        [SerializeField] private RectTransform lorePanel;
        
        [Space(2)]
        [Header("Tutorial Panel")]
        [SerializeField] private RectTransform tutorialPanel;
        
        [Space(2)]
        [Header("Credits Panel")]
        [SerializeField] private RectTransform creditsPanel;

        
        // Menu Panel
        public RectTransform MenuPanel => menuPanel;
        public RectTransform LeftButtonsPanel => leftButtonsPanel;
        public RectTransform RightButtonsPanel => rightButtonsPanel;
        public RectTransform QuitButton => quitButton;
        public TMP_Text GameTitle => gameTitle;
        
        // Lore Panel
        public RectTransform LorePanel => lorePanel;
        
        // Tutorial Panel
        public RectTransform TutorialPanel => tutorialPanel;
        
        // Credits Panel
        public RectTransform CreditsPanel => creditsPanel;
    }
}