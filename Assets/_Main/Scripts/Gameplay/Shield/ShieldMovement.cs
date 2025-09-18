using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldMovement
    {
        private readonly ShieldMovementSo _data;
        
        private float _angularVelocity = 0f;
        public ShieldMovement(ShieldMovementSo data)
        {
            _data = data;
        }
        
        public float GetAngularVelocity(float inputDirection, float deltaTime)
        {
            // ---- Dinámica: dv/dt = input*accel - dragCoef*vel ----
            float angularAcc = inputDirection * _data.Acceleration - _data.Drag * _angularVelocity;

            // Integración de la velocidad
            _angularVelocity += angularAcc * deltaTime;

            // Clampeo a la velocidad máxima
            _angularVelocity = Mathf.Clamp(_angularVelocity, -_data.MaxAngularVelocity, _data.MaxAngularVelocity);
            
            return _angularVelocity * deltaTime;
        }

    }
}