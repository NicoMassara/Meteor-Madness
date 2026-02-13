using System;
using _Main.Scripts.Common;
using MeteorMadness.GlobalValues._Main.Scripts.GlobalValues.Tools;
using MeteorMadness.GlobalValues.Tools.Observer;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.ScreenFlow._Main.Scripts.ScreenFlow.InputPanel
{
    internal class InputPanelView : ManagedBehavior, IUpdatable, 
        InputPanelView.IInputPanelView, IObserver
    {
        internal interface IInputPanelView
        {
            
        }

        [SerializeField] private RectTransform panelToShake;
        [Header("Shake Data")]
        [SerializeField] private ShakerCapDataSo shakeCapData;
        [SerializeField] private ShakerDataSo shakeData;
        
        private IShaker _shakerController;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;

        private void Start()
        {
            _shakerController = new UIShaker(panelToShake, shakeCapData);
        }
        
        public void ExecuteUpdate(float deltaTime)
        {
            _shakerController.Execute(deltaTime);
        }

        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case InputPanelObserverMessage.Collision:
                    HandleCollision((float)args[0]);
                    break;
            }
        }

        private void HandleCollision(float shakeMultiplier)
        {
            _shakerController.AddShake(shakeData,EarthShakerMultiplier.Get(shakeMultiplier));
        }
    }
}