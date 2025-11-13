namespace _Main.Scripts.Sounds
{
    public class SoundBehaviourFactory
    {
        private readonly SoundComponent _prefab;
        private readonly RandomIdGenerator _idGenerator = new RandomIdGenerator();
        private GenericPool<SoundComponent> _pool;

        public SoundBehaviourFactory(SoundComponent prefab)
        {
            _prefab = prefab;
            
            Initialize();
        }

        private void Initialize()
        {
            _pool = new GenericPool<SoundComponent>(_prefab, 10, 50);
        }

        public SoundComponent GetSound(out ulong soundId)
        {
            var tempSound = _pool.Get();
            tempSound.OnRecycle += OnRecycleHandler;
            soundId = _idGenerator.Generate();
            return tempSound;
        }
        
        public SoundComponent GetSound()
        {
            var tempSound = _pool.Get();
            tempSound.OnRecycle += OnRecycleHandler;
            return tempSound;
        }

        public void RecycleAll()
        {
            _pool.RecycleAll();
        }

        private void OnRecycleHandler(SoundComponent soundBehavior)
        {
            soundBehavior.OnRecycle -= OnRecycleHandler;
            _pool.Release(soundBehavior);
        }
    }
}