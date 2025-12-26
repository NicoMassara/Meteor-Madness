using System;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Tutorial
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