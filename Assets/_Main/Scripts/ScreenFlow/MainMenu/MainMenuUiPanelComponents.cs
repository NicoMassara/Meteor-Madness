using System;
using MeteorMadness.ScreenFlow.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MeteorMadness.ScreenFlow.Menu
{
    public class MainMenuUiPanelComponents : UiComponentsSelector<MainMenuUiComponents> { }
    
    [Serializable]
    public class MainMenuUiComponents : UiComponentsData
    {
        [Space(2)] 
        [Space]
        [Header("Text Components")]
        public TMP_Text CreditsText;
        [Space]
        [Header("Buttons Components")]
        [Space(1)]
        [Header("Menu Panel")]
        public Button PlayButton;
        public Button TutorialButton;
        public Button CosmeticButton;
        public Button LoreButton;
        public Button StatsButton;
        public Button OptionsButton;
        public Button CreditsButton;
        public Button QuitButton;
        public Button FirstGame_Play;
        public Button FirstGame_Tutorial;
        public Button FirstGame_Close;
        [Space(1)]
        [Header("Tutorial Panel")]
        public Button OpenTutorialButton;
        [Space(1)]
        [Header("Back")]
        public Button[] BackButtons;
    }
}