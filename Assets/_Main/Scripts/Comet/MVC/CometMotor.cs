using _Main.Scripts.FyingObject;
using UnityEngine;

namespace _Main.Scripts.Comet
{
    public class CometMotor : FlyingObjectMotor<FlyingObjectValues>
    {
        public void HandleCollisionWithWall()
        {
            HandleCollision(false);
        }
    }
}