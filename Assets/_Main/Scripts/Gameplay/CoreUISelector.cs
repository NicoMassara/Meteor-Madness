using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Utilities;

namespace _Main.Scripts.Gameplay
{
    public class CoreUISelector : UiPanelSelector<CoreUIData> { }

    [Serializable]
    public class CoreUIData : UiComponentsData { }
}