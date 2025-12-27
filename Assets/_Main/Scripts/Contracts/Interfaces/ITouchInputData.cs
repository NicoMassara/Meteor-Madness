using UnityEngine;

namespace MeteorMadness.Contracts.Interfaces
{
    public interface ITouchInputData
    {
        public float GetTopBound();

        public float GetBottomBound();

        public Vector2 GetLeftZoneBounds();
        public Vector2 GetRightZoneBounds();

    }
}