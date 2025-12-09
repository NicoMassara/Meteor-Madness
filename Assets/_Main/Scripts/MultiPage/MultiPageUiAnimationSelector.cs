using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Utilities;
using UnityEngine;

namespace _Main.Scripts.MultiPage
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