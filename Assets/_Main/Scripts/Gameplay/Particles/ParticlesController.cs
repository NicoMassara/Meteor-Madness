using System.Collections.Generic;
using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.GlobalValues.Tools;
using MeteorMadness.Managers;
using UnityEngine;

namespace MeteorMadness.Gameplay.Particles
{
    public class ParticlesController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private ParticleBehaviour particlePrefab;
        private GenericPool<ParticleBehaviour> _pool;
        private readonly List<ParticleBehaviour> _activeParticles = new List<ParticleBehaviour>();
        

        private void Awake()
        {
            _pool = new GenericPool<ParticleBehaviour>(particlePrefab, 35, 300, "Particle");
        }

        private void Start()
        {
            SetEventBus();
        }

        public void RecycleAll()
        {
            _pool.RecycleAll();
        }
        
        private void SpawnParticle(IParticleData particleData, 
            Vector3 position, Quaternion rotation, Vector3 moveDirection)
        {
            if (particleData == null)
            {
                Debug.LogWarning("Particle data is null");
                return;
            }
            
            var tempParticle = _pool.Get();
            tempParticle.SetValues(particleData, position, rotation.eulerAngles.z, moveDirection);
            tempParticle.OnRecycle += Particle_OnRecycleHandler;
            _activeParticles.Add(tempParticle);
        }

        private void Particle_OnRecycleHandler(ParticleBehaviour particle)
        {
            particle.OnRecycle -= Particle_OnRecycleHandler;
            _activeParticles.Remove(particle);
            _pool.Release(particle);
            
        }

        #region Handlers
        

        #endregion

        #region EventBus

        private void SetEventBus()
        {
            ParticleEventSubscriber.Spawn(EventBus_OnSpawnParticle);
        }


        private void EventBus_OnSpawnParticle(ParticleEvents.Spawn input)
        {
            SpawnParticle(input.ParticleData, input.Position, 
                input.Rotation, input.MoveDirection);
        }

        #endregion
    }
}