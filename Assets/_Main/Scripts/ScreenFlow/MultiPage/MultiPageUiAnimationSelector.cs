using System;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.MultiPage
{
    public class MultiPageUiAnimationSelector : UiComponentsSelector<MultiPageUiAnimationComponents> { }
    
    [Serializable]
    public class MultiPageUiAnimationComponents : UiComponentsData,
        MultiPageUiAnimationComponents.IMainPanel
    {
        public interface IMainPanel : IUiAnimationComponent
        {
            public RectTransform MainPanel { get; }
        }
        
        [Space(2)]
        [Header("Main Panel")]
        [SerializeField] private RectTransform mainPanel;

        public RectTransform MainPanel => mainPanel;
    }
}