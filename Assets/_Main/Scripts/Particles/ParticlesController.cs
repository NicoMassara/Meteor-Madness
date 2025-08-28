using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace _Main.Scripts.Particles
{
    public class ParticlesController : MonoBehaviour
    {
        [SerializeField] private ParticleBehaviour particlePrefab;
        private GenericPool<ParticleBehaviour> _pool;
        private readonly List<ParticleBehaviour> _activeParticles = new List<ParticleBehaviour>();

        private void Start()
        {
            _pool = new GenericPool<ParticleBehaviour>(particlePrefab, 5, 100);
        }
        
        public void RecycleAll()
        {
            for (int i = _activeParticles.Count - 1; i >= 0; i--)
            {
                _activeParticles[i].ForceRecycle();
            }
        }

        public void SpawnParticle(ParticleDataSo particleData, Vector3 position, Quaternion rotation)
        {
            var tempParticle = _pool.Get();
            tempParticle.SetValues(particleData.Sprite,particleData.TimeToFade,particleData.FadeSpeed, particleData.FadeScale,
                position + particleData.PositionOffset, 
                rotation.eulerAngles.z + particleData.RotationOffset);
            tempParticle.OnRecycle += Particle_OnRecycleHandler;
            _activeParticles.Add(tempParticle);
        }

        private void Particle_OnRecycleHandler(ParticleBehaviour particle)
        {
            particle.OnRecycle -= Particle_OnRecycleHandler;
            _activeParticles.Remove(particle);
            _pool.Release(particle);
        }
    }
}