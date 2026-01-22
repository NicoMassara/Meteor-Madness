using System;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;

namespace MeteorMadness.ScreenFlow._Main.Scripts.ScreenFlow.InputPanel.UISelector
{
    public class InputPanelUIAnimationSelector :  UiComponentsSelector<InputPanelUIAnimationComponents> { }
    
    [Serializable]
    public class InputPanelUIAnimationComponents : UiComponentsData,
        InputPanelUIAnimationComponents.IMainPanel
    {
        public interface IMainPanel : IUiAnimationComponent
        {
            public RectTransform MainPanel { get; }
        }
        
        [Space(2)]
        [Header("Main Panel")]
        [SerializeField] private RectTransform mainPanel;
        
        // Menu Panel
        public RectTransform MainPanel => mainPanel;
    }
}