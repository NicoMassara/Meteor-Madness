using _Main.Scripts.Comet;
using _Main.Scripts.FyingObject;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorMotor : FlyingObjectMotor
    {
        public void HandleShieldDeflection()
        {
            NotifyAll(MeteorObserverMessage.ShieldDeflection, Position);
        }

        public void HandleEarthCollision()
        {
            NotifyAll(MeteorObserverMessage.EarthCollision, Position, Rotation);
        }
    }
}