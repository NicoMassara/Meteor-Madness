using UnityEngine;

namespace MeteorMadness.Contracts.Interfaces
{
    public interface IMovement
    {
        public int GetCurrentSlot();
        public Vector2 GetPosition();
    }
}