using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Utilities;

namespace _Main.Scripts.Gameplay
{
    public class CoreUISelector : UiComponentsSelector<CoreUIData> { }

    [Serializable]
    public class CoreUIData : UiComponentsData { }
}