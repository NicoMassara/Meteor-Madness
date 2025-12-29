using MeteorMadness.Gameplay.FlyingObject;
using UnityEngine;

namespace MeteorMadness.Gameplay.Comet
{
    public class CometMotor : FlyingObjectMotor<FlyingObjectValues>
    {
        public void HandleCollisionWithWall()
        {
            HandleCollision(false);
        }
    }
}