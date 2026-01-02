using System;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Settings
{
    public class SettingsUiAnimationSelector : UiComponentsSelector<SettingsUiAnimationComponents> { }
    
    [Serializable]
    public class SettingsUiAnimationComponents : UiComponentsData,
        SettingsUiAnimationComponents.IMainPanel
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