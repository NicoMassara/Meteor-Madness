using _Main.Scripts.Interfaces;

namespace _Main.Scripts.FloatingScore
{
    public interface IFloatingText : IPoolable<IFloatingText>
    {
        public void SetValues(FloatingTextValues values);
    }
}