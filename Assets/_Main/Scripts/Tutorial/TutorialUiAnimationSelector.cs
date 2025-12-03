using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Utilities;
using UnityEngine;

namespace _Main.Scripts.Tutorial
{
    public class TutorialUiAnimationSelector : UiComponentsSelector<TutorialUiAnimationComponents> { }
    
    
    [Serializable]
    public class TutorialUiAnimationComponents : UiComponentsData,
        TutorialUiAnimationComponents.IHintPanel
    {
        public interface IHintPanel : IUiAnimationComponent
        {
            public RectTransform HintPanel { get; }
        }
        
        [Space(2)]
        [Header("Hint Panel")]
        [SerializeField] private RectTransform hintPanel;

        public RectTransform HintPanel => hintPanel;
    }
    
}