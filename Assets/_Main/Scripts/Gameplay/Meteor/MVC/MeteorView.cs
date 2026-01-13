using System;
using _Main.Scripts.ShieldRotation.Contracts;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Meteor;
using MeteorMadness.Gameplay.FlyingObject;
using MeteorMadness.GlobalValues.Tools.Observer;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Meteor
{
    public class MeteorView : FlyingObjectView<MeteorMotor, MeteorView, MeteorValuesData>, 
        IMeteor, ITargetable, IProjectile
    {
        public UnityAction<MeteorCollisionData> OnEarthCollision { get; set; }
        public UnityAction<MeteorCollisionData> OnDeflection { get; set; }
        public Vector2 Position => (Vector2)transform.position;

        public bool CanBeTargeted { get; private set; }
        public bool EnableMovement { get; set; }

        public event Action<ITargetable> OnTargetDeath;
        public event Action OnDeath;
        
        public void DisableTargetable()
        {
            Debug.Log("Meteor Targetable Disabled");
            CanBeTargeted = false;
        }

        public void EnableTargetable()
        {
            Debug.Log("Meteor Targetable Enable");
            CanBeTargeted = true;
        }

        public override void SetValues(MeteorValuesData data)
        {
            base.SetValues(data);
            EnableTargetable();
        }
        
        public void SetEnableMovement(bool enable)
        {
            Movement.CanMove = enable;
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case MeteorObserverMessage.EarthCollision:
                    HandleEarthCollision(
                        (Vector2)args[0], 
                        (Quaternion)args[1],
                        (Vector2)args[2]);
                    break;
                case MeteorObserverMessage.ShieldDeflection:
                    HandleShieldDeflection(
                        (Vector2)args[0], 
                        (Quaternion)args[1],
                        (Vector2)args[2],
                        (float)args[3]);
                    break;
            }
            
            base.OnNotify(message, args);
        }

        private void HandleEarthCollision(Vector2 position, Quaternion rotation, Vector2 direction)
        {
            DestroyMeteor();
            
            OnEarthCollision?.Invoke(new MeteorCollisionData
            {
                Meteor = this,
                Position = position,
                Rotation = rotation,
                Direction = direction,
            });
        }
        
        private void HandleShieldDeflection(Vector2 position,Quaternion rotation, Vector2 direction, float value)
        {
            DestroyMeteor();
            
            OnDeflection?.Invoke(new MeteorCollisionData
            {
                Meteor = this,
                Position = position,
                Rotation = rotation,
                Direction = direction,
                Value = value
            });
            
            HandleCollision(false, position, direction,true);
        }
        
        private void DestroyMeteor()
        {
            OnDeath?.Invoke();
            OnTargetDeath?.Invoke(this);
        }

        private void OnGUI()
        {
            Camera cam = Camera.main;
            if (cam == null) return;

            // World → Screen
            Vector3 screenPos = cam.WorldToScreenPoint(transform.position);

            // Tamaño del panel
            float width = 140f;
            float height = 50f;

            // Convertir a coordenadas GUI (Y invertida)
            float x = screenPos.x - width * 0.5f;
            float y = Screen.height - screenPos.y - height - 10f;

            Rect rect = new Rect(x, y, width, height);
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 14;
            style.normal.textColor = CanBeTargeted ? Color.green : Color.red;
            
            GUI.Box(rect, $"Targetable: {CanBeTargeted}",style);
        }
    }

    public struct MeteorCollisionData
    {
        public MeteorView Meteor;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector2 Direction;
        public float Value;
    }
}