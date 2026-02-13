using System;
using MeteorMadness.Contracts.Interfaces.Skins;
using UnityEngine;

namespace _Main.Scripts.MyTest.CosmeticUI
{
    public class DummyMeteor : MonoBehaviour, IFlyingObjectSkin
    {
        public event Action OnSkinEnable;
        
        private void Awake()
        {
            TestEvents.OnEarthShow += OnSkinEnable;
        }
    }
}