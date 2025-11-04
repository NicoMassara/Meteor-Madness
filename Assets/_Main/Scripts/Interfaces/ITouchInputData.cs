using UnityEngine;

namespace _Main.Scripts.Interfaces
{
    public interface ITouchInputData
    {
        public float GetTopBound();

        public float GetBottomBound();

        public Vector2 GetLeftZoneBounds();
        public Vector2 GetRightZoneBounds();

    }
}