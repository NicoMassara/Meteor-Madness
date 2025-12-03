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
        [Header("Panels")]
        public GameObject MainPanel;
        [Header("Button Components")]
        public Button MainMenuButton;
        [Header("Components")]
        [SerializeField] private SkinButtonSelector buttonSelector;

        public IButtonSelector ButtonSelector => buttonSelector;
    }
}