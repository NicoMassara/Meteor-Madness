using System;
using MeteorMadness.Contracts.Interfaces;
using UnityEngine;

namespace MeteorMadness.Contracts
{
    public struct FloatingTextValues
    {
        public Vector2 Position;
        public Vector2 Offset;
        public Color Color;
        public bool DoesMove;
        public bool DoesFade;
        public string Text;
    }
    
    public struct CollisionData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector2 Direction;
        public ProjectileType Type;
    }
    
    public struct DeflectData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector2 Direction;
        public byte Value;
        public ProjectileType Type;
    }
    
    public struct ProjectileSpawnData
    {
        public ProjectileType ProjectileType;
        public Vector2 Position;
        public Vector2 Direction;
        public float MovementMultiplier;
    }
    
    public struct ParticleSpawnData
    {
        public IParticleData ParticleData; 
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 MoveDirection;
    }
    
    public struct AbilityAddData
    {
        public AbilityType AbilityType;
        public Vector2 Position;
    }
    
    
    [Serializable]
    public struct UnlockBits128
    {
        public ulong low;
        public ulong high;
    }
}