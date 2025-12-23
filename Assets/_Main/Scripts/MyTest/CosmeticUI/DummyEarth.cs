using System;
using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.MyTest.CosmeticUI
{
    public class DummyEarth : MonoBehaviour, IEarthSkin
    {
        public event Action<float> OnHealthChanged;
        public event Action OnShow;
        public event Action OnHide;

        private void Awake()
        {
            TestEvents.OnEarthShow += OnShow;
        }
    }
}