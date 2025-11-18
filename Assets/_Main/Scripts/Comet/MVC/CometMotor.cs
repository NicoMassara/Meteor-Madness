using _Main.Scripts.FlyingObject;
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