using MeteorMadness.Gameplay.FlyingObject;
using UnityEngine;

namespace MeteorMadness.Gameplay.Comet
{
    public class CometView : FlyingObjectView<CometMotor,CometView, FlyingObjectValues>
    {
        protected override void HandleCollision(bool canMove, Vector2 position, Vector2 direction, bool doesShowParticles)
        {
            base.HandleCollision(canMove, position, direction,doesShowParticles);
            Recycle();
        }
    }
}