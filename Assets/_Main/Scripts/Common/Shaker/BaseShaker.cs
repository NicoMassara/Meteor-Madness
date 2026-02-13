using System;
using System.Collections.Generic;
using System.Linq;
using _Main.Scripts.Contracts.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Common
{
    public interface IShaker
    {
        public void Execute(float deltaTime);
        public void AddShake(ShakeData data);
        public void AddShake(IShakerData shakeData, float multiplier = 1f);
        
        public event Action OnShakeFinished;
    }

    public class BaseShaker
    {
        private class ShakeInstance
        {
            public readonly IShakerData Data;
            public float NoiseTime;
            public float Elapsed;

            public readonly Vector2 Direction;
            public readonly float DirectionalBias;

            public readonly float Multiplier;

            public ShakeInstance(IShakerData data, Vector2 direction, float directionalBias, float multiplier = 1f)
            {
                Data = data;
                NoiseTime = Random.value * 100f;
                Multiplier = multiplier > 0f ? multiplier : 1f;
               
                Direction = direction.sqrMagnitude > 0f ? direction.normalized : Vector2.zero;

                DirectionalBias = direction.sqrMagnitude > 0f ? Mathf.Clamp01(directionalBias) : 0;
               
                Elapsed = 0f;
            }
        }
        
        private readonly List<ShakeInstance> _shakerInstances = new();
        private readonly IShakerCap _cap;
        private Vector3 _preShakePosition;

        public event Action OnShakeFinished;

        public BaseShaker(IShakerCap capData)
        {
            _cap = capData;
        }

        protected Vector3 GetShakePosition(float deltaTime)
        {
            int highestPriority = int.MinValue;
            
            foreach (var s in _shakerInstances)
                if (s.Data.Priority > highestPriority)
                    highestPriority = s.Data.Priority;
            
            Vector2 totalOffset = Vector2.zero;
            
            foreach (var shakerData in _shakerInstances.ToList())
            {
                shakerData.NoiseTime += deltaTime * shakerData.Data.Frequency;
                shakerData.Elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(shakerData.Elapsed / shakerData.Data.Duration);
                float intensity = shakerData.Data.IntensityCurve.Evaluate(t);
            
                if (shakerData.Data.Priority == highestPriority && intensity > 0f)
                {
                    float x = Mathf.PerlinNoise(shakerData.NoiseTime, 0f) * 2f - 1f;
                    float y = Mathf.PerlinNoise(0f, shakerData.NoiseTime) * 2f - 1f;

                    Vector2 noise = new Vector2(x, y);
                    
                    float noiseMag = noise.magnitude;
                    if (noiseMag > 0f)
                        noise /= noiseMag;
                    
                    if (shakerData.DirectionalBias > 0f && shakerData.Direction != Vector2.zero)
                    {
                        Vector2 dir = shakerData.Direction;

                        float sign = Mathf.Sign(Vector2.Dot(noise, dir));
                        Vector2 directional = dir * sign;

                        noise = Vector2.Lerp(noise, directional, shakerData.DirectionalBias);
                        noise.Normalize();
                    }
                    
                    float strength = shakerData.Data.Magnitude * intensity * shakerData.Multiplier;
                    totalOffset += noise * strength;
                }
                
                if (shakerData.Elapsed >= shakerData.Data.Duration)
                {
                    _shakerInstances.Remove(shakerData);
                }
            }
            
            totalOffset = _cap.SmoothCap ? SoftCap(totalOffset) : HardCap(totalOffset);
            
            if (_shakerInstances.Count > 0)
            {
                return _preShakePosition + (Vector3)totalOffset;
            }
            else
            {
                OnShakeFinished?.Invoke();
                return _preShakePosition;
            }
        }

        protected bool DoesShake()
        {
            return _shakerInstances.Count > 0;
        }
        
        private Vector2 HardCap(Vector2 offset)
        {
            float mag = offset.magnitude;
            if (mag <= _cap.MaxOffset)
                return offset;

            return offset.normalized * _cap.MaxOffset;
        }
        
        private Vector2 SoftCap(Vector2 offset)
        {
            float mag = offset.magnitude;
            if (mag <= _cap.MaxOffset)
                return offset;

            float excess = mag - _cap.MaxOffset;
            float compressed = _cap.MaxOffset +
                               (1f - Mathf.Exp(-excess * _cap.SoftCapStrength)) / _cap.SoftCapStrength;

            return offset.normalized * compressed;
        }
        
        protected void AddShake(IShakerData shakeData, Vector2 direction, float directionalBias, float multiplier, Vector3 startPos)
        {
            if (shakeData == null)
            {
                Debug.LogWarning("Shake Data is not set!");
                return;
            }

            if (_shakerInstances.Count == 0)
            {
                _preShakePosition =  startPos;
            }

            if (shakeData.DoesCancelAll)
            {
                _shakerInstances.Clear();
            }

            _shakerInstances.Add(new ShakeInstance(shakeData, direction, directionalBias, multiplier));
        }
    }
}