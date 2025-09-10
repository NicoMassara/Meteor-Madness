using _Main.Scripts.FyingObject;
using UnityEngine;

namespace _Main.Scripts.Comet
{
    public class CometView : FlyingObjectView<CometMotor,CometView>
    {
        protected override void HandleCollision(bool canMove, Vector2 position, bool doesShowParticles)
        {
            base.HandleCollision(canMove, position, doesShowParticles);
            ForceRecycle();
        }
    }
}