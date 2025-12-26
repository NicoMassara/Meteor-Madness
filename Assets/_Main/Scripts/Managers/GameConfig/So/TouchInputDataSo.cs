using MeteorMadness.GlobalValues.Interfaces;
using UnityEngine;

namespace MeteorMadness.Managers.GameConfig
{
    [CreateAssetMenu(fileName = "SO_TouchInputData_Name", menuName = "Scriptable Objects/Inputs/Touch Data", order = -1)]
    public class TouchInputDataSo : ScriptableObject, ITouchInputData
    {
        [Header("Width")] 
        [Range(0, 1)] 
        [SerializeField] private float centerOffset = 0.1f;
        [Range(0,1)]
        [SerializeField] private float sideOffset = 0.1f;
        [Header("Height")]
        [Range(0,1)]
        [SerializeField] private float topOffset = 0.1f;
        [Range(0,1)]
        [SerializeField] private float bottomOffset = 0.1f;

        public float CenterOffset => centerOffset;
        public float SideOffset => sideOffset;
        public float TopOffset => topOffset;
        public float BottomOffset => bottomOffset;
        
        public float GetTopBound()
        {
            return Screen.height * (1 - GetLerpedTop());
        }
        
        public float GetBottomBound()
        {
            return Screen.height * GetLerpedBottom();
        }

        public Vector2 GetLeftZoneBounds()
        {
            return new Vector2(
                (Screen.width * GetLerpedSide()), 
                (Screen.width * 0.5f) - (Screen.width * GetLerpedCenter()));
        }

        public Vector2 GetRightZoneBounds()
        {
            return new Vector2(
                (Screen.width * 0.5f) + (Screen.width * GetLerpedCenter()), 
                Screen.width * (1 - GetLerpedSide()));
        }

        private float GetLerpedTop()
        {
            return Mathf.Lerp(0, 0.5f, topOffset);
        }
        
        private float GetLerpedBottom()
        {
            return Mathf.Lerp(0, 0.5f, bottomOffset);
        }

        private float GetLerpedSide()
        {
            return Mathf.Lerp(0, 0.25f, sideOffset);
        }

        private float GetLerpedCenter()
        {
            return Mathf.Lerp(0, 0.25f, centerOffset);
        }

    }
}