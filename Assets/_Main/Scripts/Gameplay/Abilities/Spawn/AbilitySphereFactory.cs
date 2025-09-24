using System.Collections.Generic;
using _Main.Scripts.Gameplay.Abilities.Sphere;

namespace _Main.Scripts.Gameplay.Abilities.Spawn
{
    public class AbilitySphereFactory
    {
        private readonly AbilitySphereView _prefab;
        private readonly List<AbilitySphereView> _activeSpheres = new List<AbilitySphereView>();
        private GenericPool<AbilitySphereView> _pool;
        
        public int ActiveSpheresCount => _activeSpheres.Count;


        public AbilitySphereFactory(AbilitySphereView prefab)
        {
            _prefab = prefab;
            Initialize();
        }

        private void Initialize()
        {
            _pool = new GenericPool<AbilitySphereView>(_prefab);
        }

        public AbilitySphereView SpawnAbilitySphere()
        {
            var temp = _pool.Get();
            temp.OnRecycle += OnRecycleHandler;
            _activeSpheres.Add(temp);
            return temp;
        }
        
        public void RecycleAll()
        {
            for (int i = _activeSpheres.Count - 1; i >= 0; i--)
            {
                _activeSpheres[i].ForceRecycle();
            }
        }

        private void OnRecycleHandler(AbilitySphereView sphereView)
        {
            sphereView.OnDeflection = null;
            sphereView.OnEarthCollision = null;
            sphereView.OnRecycle -= OnRecycleHandler;
            _activeSpheres.Remove(sphereView);
            _pool.Release(sphereView);
        }
    }
}