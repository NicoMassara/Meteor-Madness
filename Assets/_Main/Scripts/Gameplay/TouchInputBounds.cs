using UnityEngine;

namespace _Main.Scripts.Gameplay
{
    public static class TouchInputBounds
    {
        private static float _topOffset;
        private static float _bottomOffset;
        private static float _wOffset;

        // ReSharper disable once PossibleLossOfFraction
        public static void SetOffsets(float topOffset, float bottomOffset, float widthOffset)
        {
            _topOffset = Mathf.Lerp(0, 0.5f, topOffset);
            _bottomOffset = Mathf.Lerp(0, 0.5f, bottomOffset);
            _wOffset = Mathf.Lerp(0, 0.25f, widthOffset);
        }

        public static float GetBottomBound()
        {
            return Screen.height * _bottomOffset;
        }

        public static float GetTopBound()
        {
            return Screen.height * (1 - _topOffset);
        }

        public static Vector2 GetLeftZoneBounds()
        {
            return new Vector2(
                (Screen.width * _wOffset), 
                (Screen.width * 0.5f) - (Screen.width * _wOffset));
        }

        public static Vector2 GetRightZoneBounds()
        {
            return new Vector2(
                (Screen.width * 0.5f) + (Screen.width * _wOffset), 
                Screen.width * (1 - _wOffset));
        }
    }
}