using System.Collections.Generic;
using _Main.Scripts.Managers;
using UnityEngine;

namespace _Main.Scripts.Particles
{
    public class ParticlesController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private ParticleBehaviour particlePrefab;
        private GenericPool<ParticleBehaviour> _pool;
        private readonly List<ParticleBehaviour> _activeParticles = new List<ParticleBehaviour>();

        private void Awake()
        {
            _pool = new GenericPool<ParticleBehaviour>(particlePrefab, 5, 100);
            
            GameManager.Instance.EventManager.Subscribe<SpawnParticle>(EventBus_OnSpawnParticle);
        }
        
        public void RecycleAll()
        {
            for (int i = _activeParticles.Count - 1; i >= 0; i--)
            {
                _activeParticles[i].ForceRecycle();
            }
        }
        
        private void SpawnParticle(ParticleDataSo particleData, 
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
        
        private void EventBus_OnSpawnParticle(SpawnParticle input)
        {
            SpawnParticle(input.ParticleData, input.Position, 
                input.Rotation, input.MoveDirection);
        }
    }
}