using UnityEngine;

namespace MeteorMadness.GlobalValues.Interfaces
{
    public interface IMovement
    {
        public int GetCurrentSlot();
        public Vector2 GetPosition();
    }
}