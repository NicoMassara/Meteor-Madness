namespace _Main.Scripts.Sounds
{
    public class SoundBehaviourFactory
    {
        private readonly SoundBehavior _prefab;
        private GenericPool<SoundBehavior> _pool;

        public SoundBehaviourFactory(SoundBehavior prefab)
        {
            _prefab = prefab;
            
            Initialize();
        }

        private void Initialize()
        {
            _pool = new GenericPool<SoundBehavior>(_prefab, 10, 50);
        }

        public SoundBehavior GetSound()
        {
            var tempSound = _pool.Get();
            tempSound.OnRecycle += OnRecycleHandler;
            return tempSound;
        }

        public void RecycleAll()
        {
            _pool.RecycleAll();
        }

        private void OnRecycleHandler(SoundBehavior soundBehavior)
        {
            soundBehavior.OnRecycle -= OnRecycleHandler;
            _pool.Release(soundBehavior);
        }
    }
}