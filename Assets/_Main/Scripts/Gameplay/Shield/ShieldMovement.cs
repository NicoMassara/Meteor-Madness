using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldMovement
    {
        private ShieldMovementSo _data;
        
        private float _angularVelocity = 0f;
        public ShieldMovement(ShieldMovementSo data)
        {
            _data = data;
        }
        
        /*public float GetAngularVelocity(float inputDirection, float deltaTime)
        {
            _angularVelocity += inputDirection * _data.Acceleration * Time.deltaTime;

            // aplicamos drag (suaviza cuando no hay input)
            _angularVelocity = Mathf.Lerp(_angularVelocity, 0f, _data.Drag * Time.deltaTime);

            // limitamos para que no se dispare demasiado
            _angularVelocity = Mathf.Clamp(_angularVelocity, -_data.MaxAngularVelocity, _data.MaxAngularVelocity);
            
            return _angularVelocity * deltaTime;
        }*/
        
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