using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Cosmetics
{
    public class CosmeticUIPanelSelector : UiPanelSelector<CosmeticUIComponents> { }

    [Serializable]
    public class CosmeticUIComponents : UiComponentsData
    {
        [Space]
        [Header("Button Components")]
        public Button MainMenuButton;
    }
}