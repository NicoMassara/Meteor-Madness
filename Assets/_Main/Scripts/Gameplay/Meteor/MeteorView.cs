using _Main.Scripts.FyingObject;
using _Main.Scripts.Observer;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorView : FlyingObjectView<MeteorMotor, MeteorView>, IMeteor
    {
        public UnityAction<MeteorView, Vector3, Quaternion, Vector2> OnEarthCollision;
        public UnityAction<MeteorView, Vector3, Quaternion, Vector2> OnDeflection;
        public Vector2 Position => (Vector2)transform.position;
        
        public override void OnNotify(string message, params object[] args)
        {
            switch (message)
            {
                case MeteorObserverMessage.EarthCollision:
                    HandleEarthCollision(
                        (Vector2)args[0], (Quaternion)args[1],(Vector2)args[2]);
                    break;
                case MeteorObserverMessage.ShieldDeflection:
                    HandleShieldDeflection(
                        (Vector2)args[0], (Quaternion)args[1],(Vector2)args[2]);
                    break;
            }
            
            base.OnNotify(message, args);
        }

        private void HandleEarthCollision(Vector2 position, Quaternion rotation, Vector2 direction)
        {
            OnEarthCollision?.Invoke(this,position,rotation, direction);
        }
        
        private void HandleShieldDeflection(Vector2 position,Quaternion rotation, Vector2 direction)
        {
            OnDeflection?.Invoke(this,position,rotation,direction);
            HandleCollision(false, position, direction,true);
        }

    }
}