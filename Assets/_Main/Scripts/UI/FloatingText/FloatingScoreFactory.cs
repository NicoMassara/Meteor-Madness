using MeteorMadness.GlobalValues.Tools;

namespace MeteorMadness.UI.FloatingText
{
    public class FloatingScoreFactory
    {
        private readonly GenericPool<FloatingTextBehaviour> _pool;
        
        public FloatingScoreFactory(FloatingTextBehaviour prefab)
        {
            _pool = new GenericPool<FloatingTextBehaviour>(prefab, 35, 100, "Floating Text");
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