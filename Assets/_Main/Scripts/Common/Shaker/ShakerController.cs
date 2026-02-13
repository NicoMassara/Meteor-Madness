
using _Main.Scripts.Contracts.Interfaces;
using UnityEngine;


namespace _Main.Scripts.Common
{
    public class ShakerController : BaseShaker, IShaker
    {
        private readonly Transform _objectToShake;
        
        public ShakerController(Transform objectToShake, IShakerCap capData) : base(capData)
        {
            _objectToShake = objectToShake;
        }

        public void Execute(float deltaTime)
        {
            if(DoesShake())
                _objectToShake.localPosition = GetShakePosition(deltaTime);
        }

        public void AddShake(ShakeData data)
        {
            AddShake(data.Data,data.Direction, data.DirectionBias, data.Multiplier,_objectToShake.position);
        }

        public void AddShake(IShakerData shakeData, float multiplier = 1f)
        {
            AddShake(shakeData,Vector2.zero, 0f, multiplier, _objectToShake.position);
        }
        
    }
}