using _Main.Scripts.MyTools;

namespace _Main.Scripts.Gameplay.FloatingScore
{
    public class FloatingScoreFactory
    {
        private readonly GenericPool<FloatingTextBehaviour> _pool;
        
        public FloatingScoreFactory(FloatingTextBehaviour prefab)
        {
            _pool = new GenericPool<FloatingTextBehaviour>(prefab, 50, 100);
        }

        public IFloatingText Get()
        {
            IFloatingText item = _pool.Get();
            item.OnRecycle += OnRecycleHandler;
            return item;
        }

        private void Release(IFloatingText item)
        {
            item.OnRecycle -= OnRecycleHandler;
            _pool.Release((FloatingTextBehaviour)item);
        }

        public void RecycleAll()
        {
            _pool.RecycleAll();
        }
        
        private void OnRecycleHandler(IFloatingText item)
        {
            Release(item);
        }
    }
}