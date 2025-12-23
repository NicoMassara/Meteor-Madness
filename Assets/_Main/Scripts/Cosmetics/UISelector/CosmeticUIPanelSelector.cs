using System;
using _Main.Scripts.Cosmetics.Components;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Cosmetics
{
    public class CosmeticUIPanelSelector : UiComponentsSelector<CosmeticUIComponents> { }

    [Serializable]
    public class CosmeticUIComponents : UiComponentsData
    {
        [Space]
        [Header("Buttons")]
        public Button MainMenuButton;
        [Header("Select Buttons")]
        public Transform ButtonsContainer;
        public SkinSelectButton SkinSelectButton;
    }
}