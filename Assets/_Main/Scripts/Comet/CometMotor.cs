using _Main.Scripts.FyingObject;
using UnityEngine;

namespace _Main.Scripts.Comet
{
    public class CometMotor : FlyingObjectMotor
    {
        public void HandleCollisionWithWall()
        {
            HandleCollision(false);
        }
    }
}