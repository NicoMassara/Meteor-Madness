using _Main.Scripts.Comet;
using _Main.Scripts.FyingObject;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorView : FlyingObjectView<MeteorMotor, MeteorView>
    {
        public UnityAction<MeteorView, Vector3, Quaternion> OnEarthCollision;
        public UnityAction<MeteorView, Vector3> OnDeflection;
        
        public override void OnNotify(string message, params object[] args)
        {
            switch (message)
            {
                case MeteorObserverMessage.EarthCollision:
                    HandleEarthCollision((Vector2)args[0], (Quaternion)args[1]);
                    break;
                case MeteorObserverMessage.ShieldDeflection:
                    HandleShieldDeflection((Vector2)args[0]);
                    break;
            }
            
            base.OnNotify(message, args);
        }

        private void HandleEarthCollision(Vector2 position, Quaternion rotation)
        {
            OnEarthCollision?.Invoke(this,position,rotation);
        }
        
        private void HandleShieldDeflection(Vector2 position)
        {
            OnDeflection?.Invoke(this,position);
            HandleCollision(false, position, true);
        }
    }
}