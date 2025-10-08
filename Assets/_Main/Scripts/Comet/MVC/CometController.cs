using _Main.Scripts.FyingObject;
using UnityEngine;

namespace _Main.Scripts.Comet
{
    public class CometController : FlyingObjectController<CometMotor, FlyingObjectValues>
    {
        private readonly LayerMask _cometWallMask;
        
        public CometController(CometMotor motor, LayerMask cometWallMask) : base(motor)
        {
            _cometWallMask = cometWallMask;
        }

        public override void HandleTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & _cometWallMask) != 0)
            {
                Motor.HandleCollisionWithWall();
            }
        }
    }
}