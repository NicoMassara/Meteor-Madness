using System;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthCamera : MonoBehaviour
    {
        [SerializeField] private Camera earthCamera;
        [SerializeField] private RenderTexture earthTexture;

        private void Awake()
        {
            earthCamera.backgroundColor = new Color(0, 0, 0, 0); // transparent
            earthCamera.clearFlags = CameraClearFlags.SolidColor;
            earthCamera.targetTexture = earthTexture;
        }
    }
}