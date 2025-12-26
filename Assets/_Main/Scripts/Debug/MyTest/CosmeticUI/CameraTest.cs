using System;
using MeteorMadness.Managers;
using UnityEngine;

namespace _Main.Scripts.MyTest.CosmeticUI
{
    public class CameraTest : MonoBehaviour
    {
        [SerializeField] private GameObject grayScale;

        private void Start()
        {
            grayScale.SetActive(false);
            
            CameraEventSubscriber.EnableGrayscale(EventBus_Camera_GrayScale_Enable);
            CameraEventSubscriber.DisableGrayscale(EventBus_Camera_GrayScale_Disable);
        }

        private void EventBus_Camera_GrayScale_Enable(CameraEvents.GrayscaleEnable input) 
            => grayScale.SetActive(true);

        private void EventBus_Camera_GrayScale_Disable(CameraEvents.GrayscaleDisable input) 
            => grayScale.SetActive(false);
    }
}