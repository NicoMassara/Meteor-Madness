using System;
using UnityEngine;

namespace MeteorMadness.GlobalValues.Tools
{
    public class RemoveMobileObjects : MonoBehaviour
    {
        [SerializeField] private bool doesRemove;
        [SerializeField] private GameObject[] objectsToRemove;

        private void Awake()
        {
            if(SystemInfo.deviceType == DeviceType.Handheld) return;

            for (int i = 0; i < objectsToRemove.Length; i++)
            {
                Destroy(objectsToRemove[i]);
            }
        }
    }
}