using System;

namespace _Main.Scripts.MyTest.CosmeticUI
{
    public class TestEvents
    {
        public static event Action OnEarthShow;
        public static void ShowEarth() => OnEarthShow?.Invoke();
    }
}