using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldMovement
    {
        private readonly ShieldMovementSo _data;
        private readonly Transform _transform;
        private readonly int _angleOffset;
        
        private float _angularVelocity = 0f;
        public ShieldMovement(ShieldMovementSo data, Transform transform, int angleOffset = 0)
        {
            _data = data;
            _transform = transform; 
            _angleOffset = angleOffset;
        }
        
        private float GetAngularVelocity(float inputDirection, float deltaTime)
        {
            // ---- Dinámica: dv/dt = input*accel - dragCoef*vel ----
            float angularAcc = inputDirection * _data.Acceleration - _data.Drag * _angularVelocity;

            // Integración de la velocidad
            _angularVelocity += angularAcc * deltaTime;

            // Clampeo a la velocidad máxima
            _angularVelocity = Mathf.Clamp(_angularVelocity, -_data.MaxAngularVelocity, _data.MaxAngularVelocity);
            
            return _angularVelocity * deltaTime;
        }

        public void Move(float inputDirection, float deltaTime)
        {
            _transform.Rotate(0f,0f,GetAngularVelocity(inputDirection,deltaTime));
        }

        public float GetAngle()
        {
            return Mathf.Repeat(_transform.rotation.eulerAngles.z + _angleOffset, 360f);
        }

        public Vector2 GetPosition()
        {
            return new Vector2(_transform.position.x, _transform.position.y);
        }

    }
}