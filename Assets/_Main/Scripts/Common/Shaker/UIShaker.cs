using _Main.Scripts.Contracts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.Common
{
    public class UIShaker : BaseShaker, IShaker
    {
        private readonly RectTransform _objectToShake;
        
        public UIShaker(RectTransform objectToShake, IShakerCap capData) : base(capData)
        {
            _objectToShake = objectToShake;
        }
        
        public void Execute(float deltaTime)
        {
            if(DoesShake())
                _objectToShake.anchoredPosition = GetShakePosition(deltaTime);
        }
        
        public void AddShake(ShakeData data)
        {
            AddShake(data.Data,data.Direction, data.DirectionBias, data.Multiplier,
                _objectToShake.anchoredPosition);
        }

        public void AddShake(IShakerData shakeData, float multiplier = 1f)
        {
            AddShake(shakeData,Vector2.zero, 0f, multiplier, 
                _objectToShake.anchoredPosition);
        }
    }
}