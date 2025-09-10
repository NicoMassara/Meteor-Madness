using _Main.Scripts.FyingObject;
using UnityEngine;

namespace _Main.Scripts.Comet
{
    public class CometMotor : FlyingObjectMotor
    {
        public CometMotor(Vector2 position) : base(position)
        {
        }

        public void HandleCollisionWithWall()
        {
            HandleCollision(false);
        }
    }
}